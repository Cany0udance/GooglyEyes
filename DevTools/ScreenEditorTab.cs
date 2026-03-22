using System.Text;
using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;

namespace GooglyEyes;

/// <summary>
/// Editor tab for placing googly eyes on screen backgrounds
/// (character select scenes, ancient event backgrounds, etc).
/// Supports both static placement and Spine bone-anchored placement with full
/// animation scrubbing and bone segment timelines.
/// </summary>
public class ScreenEditorTab : EditorTab
{
    public override string TabName => "Screens";
    public override string TabTooltip => "Place googly eyes on character select and event backgrounds.";

    // ── Sidebar ──
    private ScrollContainer _sidebarScroll;
    private VBoxContainer _sceneList;
    private Button _selectedButton;
    private bool _listBuilt;
    private LineEdit _searchInput;
    private OptionButton _filterDropdown;
    private enum FilterMode { All, Configured, Unconfigured }
    private FilterMode _filterMode = FilterMode.All;

    // ── Scene / Spine ──
    private Node _currentScene;
    private Node2D _spineSprite; // the raw SpineSprite node
    private bool _hasSpine;
    private MegaSprite _animController;
    private MegaAnimationState _animState;
    private MegaTrackEntry _cachedTrackEntry;
    private GodotObject _skeletonGodot;
    private Node2D _spineNode;
    private string _currentScenePath;

    // ── Placements ──
    private readonly List<EyePlacement> _placements = new();
    private EyePlacement _selectedEye;
    private EyePlacement _dragging;
    private bool _isDragging;

    // ── Animation ──
    private OptionButton _animDropdown;
    private HSlider _frameSlider;
    private Label _frameLabel;
    private Button _playPauseButton;
    private bool _isPlaying;
    private string _currentAnimName = "animation";
    private readonly List<string> _availableAnims = new();
    private Label _animPanelHint;

    // ── Secondary track ──
    private OptionButton _track1Dropdown;
    private readonly List<string> _availableTrack1Anims = new();
    private string _currentTrack1Anim = "";
    private string _appliedTrack1Anim = "";

    // ── Bone anchoring ──
    private string _selectedAnchorBone = "root";
    private LineEdit _anchorBoneInput;
    private Button _showBonesButton;
    private bool _showingBones;
    private readonly List<Node2D> _boneMarkers = new();
    private readonly List<BoneInfo> _boneData = new();
    private Godot.Collections.Array _cachedBones;
    private CheckBox _showAllBoneNamesCheckbox;
    private Label _anchorPosLabel;
    private Label _anchorPanelHint;

    // ── Selection extras ──
    private CheckBox _hiddenByDefaultCheckbox;
    private SpinBox _opacityInput;
    private Label _opacityLabel;

    // ── Bone segments ──
    private VBoxContainer _segmentList;
    private ScrollContainer _segmentScroll;
    private Button _splitHereButton;
    private Button _clearSegmentsButton;
    private Label _activeSegmentLabel;
    private int _editingSegmentIndex = -1;

    // ── Panel containers ──
    private Control _animPanelContainer;
    private Control _bonePanelContainer;

    // ═══════════════════════════════════════════════
    //  INNER TYPES (same as MonsterEditorTab)
    // ═══════════════════════════════════════════════

    private struct BoneInfo { public string Name; public Vector2 SpineWorldPos; }

    private class BoneSegment
    {
        public float StartTime;
        public float EndTime;
        public string BoneName = "root";
        public Vector2 SpineOffset;
        public bool HasSpineOffset;
        public bool Hidden;
        public float OpacityStart = 1f;
        public float OpacityEnd = 1f;
    }

    private class EyePlacement
    {
        public Vector2 Position;
        public float Scale = 1f;
        public float Opacity = 1f;
        public string AnchorBone = "root";
        public Sprite2D EyeSprite;
        public Sprite2D IrisSprite;
        public Node2D Container;
        public Vector2 SpineOffset;
        public bool HasSpineOffset;
        public bool HiddenByDefault;
        public Dictionary<string, List<BoneSegment>> BoneTimelines = new();
    }

    // Sidebar entry: display name + scene path
    private class SceneEntry
    {
        public string DisplayName;
        public string ScenePath;
    }

    // ═══════════════════════════════════════════════
    //  LIFECYCLE
    // ═══════════════════════════════════════════════

    protected override void OnRegister()
    {
        var screenSize = Screen.GetViewportRect().Size;
        BuildSidebarContainer(screenSize);
        BuildAnimationPanel(screenSize);
        BuildBonePanel(screenSize);
        BuildSelectionExtras(screenSize);
    }

    public override void BuildSidebarItems()
    {
        if (_listBuilt) return;
        _listBuilt = true;

        // ── Character Select Backgrounds ──
        AddSidebarHeader("Character Select");

        foreach (var character in ModelDb.AllCharacters.OrderBy(c => c.Id.Entry))
        {
            var id = character.Id.Entry;
            var path = character.CharacterSelectBg;
            if (!ResourceLoader.Exists(path)) continue;

            AddSceneButton(id, path);
        }

        // ── Ancient Event Backgrounds ──
        AddSidebarHeader("Ancients");

        foreach (var ancient in ModelDb.AllAncients.OrderBy(a => a.Id.Entry))
        {
            var id = ancient.Id.Entry;
            var path = SceneHelper.GetScenePath("events/background_scenes/" + id.ToLowerInvariant());
            if (!ResourceLoader.Exists(path)) continue;

            AddSceneButton(id, path);
        }
    }

    private void AddSceneButton(string displayName, string scenePath)
    {
        bool hasConfig = ScreenGooglyEyesRegistry.Configs.ContainsKey(scenePath);

        var button = new Button
        {
            Text = displayName, Flat = true,
            Alignment = HorizontalAlignment.Left,
            CustomMinimumSize = new Vector2(0, 32),
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        button.SetMeta("scene_path", scenePath);
        button.SetMeta("display_name", displayName);
        ApplyFont(button, 12, hasConfig ? EditorTheme.AccentGreen : EditorTheme.TextNormal);
        button.AddThemeColorOverride("font_hover_color", EditorTheme.TextBright);
        button.Pressed += () => SelectScene(displayName, scenePath);
        _sceneList.AddChild(button);
    }

    public override void Activate()
    {
        _sidebarScroll.Visible = true;
        _searchInput.Visible = true;
        _filterDropdown.Visible = true;
        SetPanelsVisible(true);
        BuildSidebarItems();
    }

    public override void Deactivate()
    {
        _sidebarScroll.Visible = false;
        _searchInput.Visible = false;
        _filterDropdown.Visible = false;
        SetPanelsVisible(false);
        ClearAll();
        ClearBoneMarkers();
        ClearScene();
        _selectedButton = null;
    }

    public override void Process(double delta)
    {
        if (_showingBones && _boneData.Count > 0 && _skeletonGodot != null)
            UpdateBoneMarkerPositions();

        if (_isPlaying && _cachedTrackEntry != null && _spineNode != null && GodotObject.IsInstanceValid(_spineNode))
        {
            float duration = _cachedTrackEntry.GetAnimationEnd();
            if (duration > 0f)
            {
                float time = _cachedTrackEntry.GetTrackTime() % duration;
                _frameSlider.SetValueNoSignal(time / duration * 100.0);
                _frameLabel.Text = $"{time:F2}s / {duration:F2}s";
            }
        }

        UpdateEyePositionsFromSkeleton();
    }

    public override void Cleanup()
    {
        ClearAll();
        ClearBoneMarkers();
        ClearScene();
    }

    // ═══════════════════════════════════════════════
    //  SELECTION STATE
    // ═══════════════════════════════════════════════

    public override bool HasSelection => _selectedEye != null;

    public override string SelectionLabel
    {
        get
        {
            if (_selectedEye == null) return "No eye selected";
            int idx = _placements.IndexOf(_selectedEye) + 1;
            string tag = _selectedEye.HiddenByDefault ? " (hidden by default)" : "";
            return "Eye #" + idx + tag;
        }
    }

    public override float SelectionScale => _selectedEye?.Scale ?? 1f;

    public override string SelectionHint
    {
        get
        {
            if (_selectedEye == null) return "Click an eye to select it. Adjust scale or duplicate.";
            int totalSegs = _selectedEye.BoneTimelines.Values.Sum(list => list.Count);
            return totalSegs > 0
                ? "This eye has bone segments on " + _selectedEye.BoneTimelines.Count + " animation(s)."
                : "Drag to reposition. Use Scale to resize.";
        }
    }

    // ═══════════════════════════════════════════════
    //  UI BUILDING
    // ═══════════════════════════════════════════════

    private void BuildSidebarContainer(Vector2 screenSize)
    {
        float controlsY = 62f;

        _searchInput = new LineEdit
        {
            PlaceholderText = "Search...",
            Position = new Vector2(4, controlsY),
            Size = new Vector2(EditorTheme.SidebarWidth - 8, 28),
            ClearButtonEnabled = true
        };
        if (Font != null) _searchInput.AddThemeFontOverride("font", Font);
        _searchInput.AddThemeFontSizeOverride("font_size", 12);
        _searchInput.TextChanged += _ => RefreshSidebarFilter();
        _searchInput.Visible = false;
        Screen.AddChild(_searchInput);

        _filterDropdown = new OptionButton
        {
            Position = new Vector2(4, controlsY + 32),
            Size = new Vector2(EditorTheme.SidebarWidth - 8, 26)
        };
        if (Font != null) _filterDropdown.AddThemeFontOverride("font", Font);
        _filterDropdown.AddThemeFontSizeOverride("font_size", 11);
        _filterDropdown.AddItem("All");
        _filterDropdown.AddItem("Configured");
        _filterDropdown.AddItem("Unconfigured");
        _filterDropdown.Selected = 0;
        _filterDropdown.ItemSelected += idx =>
        {
            _filterMode = (FilterMode)(int)idx;
            RefreshSidebarFilter();
        };
        _filterDropdown.Visible = false;
        Screen.AddChild(_filterDropdown);

        float scrollTop = controlsY + 64f;
        _sidebarScroll = new ScrollContainer
        {
            Position = new Vector2(0, scrollTop),
            Size = new Vector2(EditorTheme.SidebarWidth, screenSize.Y - scrollTop),
            HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled,
            Visible = false
        };
        Screen.AddChild(_sidebarScroll);

        _sceneList = new VBoxContainer { CustomMinimumSize = new Vector2(EditorTheme.SidebarWidth - 20, 0) };
        _sceneList.AddThemeConstantOverride("separation", 2);
        _sidebarScroll.AddChild(_sceneList);
    }

    private void AddSidebarHeader(string text)
    {
        var spacer = new Control { CustomMinimumSize = new Vector2(0, 6) };
        spacer.SetMeta("section_header", text);
        _sceneList.AddChild(spacer);

        var header = new Label
        {
            Text = text,
            CustomMinimumSize = new Vector2(0, 24),
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        header.SetMeta("section_header", text);
        ApplyFont(header, 12, EditorTheme.Accent);
        _sceneList.AddChild(header);

        var divider = new ColorRect
        {
            Color = EditorTheme.Divider,
            CustomMinimumSize = new Vector2(0, 1),
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        divider.SetMeta("section_header", text);
        _sceneList.AddChild(divider);
    }

    private void BuildAnimationPanel(Vector2 screenSize)
    {
        float px = EditorTheme.SidebarWidth + EditorTheme.Padding;
        float py = screenSize.Y - 415;
        float pw = screenSize.X - EditorTheme.SidebarWidth - EditorTheme.Padding * 2;

        _animPanelContainer = new Control
        {
            Position = new Vector2(px, py),
            Size = new Vector2(pw, 82),
            Visible = false,
            MouseFilter = Control.MouseFilterEnum.Ignore
        };
        Screen.AddChild(_animPanelContainer);

        _animPanelContainer.AddChild(new ColorRect { Color = EditorTheme.PanelBg, Position = Vector2.Zero, Size = new Vector2(pw, 82) });
        _animPanelContainer.AddChild(MakeLabel("Animation", new Vector2(10, 4), new Vector2(100, 16), 11, EditorTheme.Accent));

        _animPanelHint = MakeLabel("Scrub or play to preview. Eyes follow their bones in real time.",
            new Vector2(120, 4), new Vector2(pw - 140, 16), 10, EditorTheme.TextDim);
        _animPanelContainer.AddChild(_animPanelHint);

        float row1 = 22;
        _animDropdown = new OptionButton { Position = new Vector2(10, row1), Size = new Vector2(150, 28) };
        if (Font != null) _animDropdown.AddThemeFontOverride("font", Font);
        _animDropdown.AddThemeFontSizeOverride("font_size", 12);
        _animDropdown.ItemSelected += OnAnimationSelected;
        _animPanelContainer.AddChild(_animDropdown);

        _frameSlider = new HSlider
        {
            Position = new Vector2(170, row1 + 2), Size = new Vector2(200, 26),
            MinValue = 0, MaxValue = 100, Step = 0.1, Value = 0
        };
        _frameSlider.ValueChanged += OnFrameSliderChanged;
        _animPanelContainer.AddChild(_frameSlider);

        _frameLabel = MakeLabel("0.00s / 0.00s", new Vector2(380, row1 + 4), new Vector2(120, 24), 12, EditorTheme.TextDim);
        _animPanelContainer.AddChild(_frameLabel);

        _playPauseButton = MakeButton("Play", new Vector2(510, row1), new Vector2(60, 28), 12);
        _playPauseButton.Pressed += TogglePlayPause;
        _animPanelContainer.AddChild(_playPauseButton);

        float row2 = row1 + 30;
        _animPanelContainer.AddChild(MakeLabel("Layer:", new Vector2(10, row2 + 3), new Vector2(45, 24), 11, EditorTheme.TextDim));

        _track1Dropdown = new OptionButton { Position = new Vector2(55, row2), Size = new Vector2(180, 26) };
        if (Font != null) _track1Dropdown.AddThemeFontOverride("font", Font);
        _track1Dropdown.AddThemeFontSizeOverride("font_size", 11);
        _track1Dropdown.ItemSelected += OnTrack1Selected;
        _animPanelContainer.AddChild(_track1Dropdown);
    }

    private void BuildSelectionExtras(Vector2 screenSize)
    {
        float px = EditorTheme.SidebarWidth + EditorTheme.Padding;
        float py = screenSize.Y - 278;
        float row = py + 24;

        _hiddenByDefaultCheckbox = new CheckBox
        {
            Text = "Hidden by default",
            Position = new Vector2(px + 420, row - 2),
            Size = new Vector2(160, 30),
            TooltipText = "Eye is invisible unless a non-hidden bone segment makes it appear."
        };
        ApplyFont(_hiddenByDefaultCheckbox, 11, EditorTheme.TextNormal);
        _hiddenByDefaultCheckbox.Toggled += pressed =>
        {
            if (_selectedEye == null) return;
            _selectedEye.HiddenByDefault = pressed;
            _selectedEye.Container.Modulate = pressed ? new Color(1, 1, 1, 0.25f) : new Color(1, 1, 1, _selectedEye.Opacity);
        };
        Screen.AddChild(_hiddenByDefaultCheckbox);

        _opacityLabel = MakeLabel("Opacity:", new Vector2(px + 600, row + 3), new Vector2(60, 24), 13, EditorTheme.TextNormal);
        Screen.AddChild(_opacityLabel);

        _opacityInput = new SpinBox
        {
            MinValue = 0.0, MaxValue = 1.0, Step = 0.05, Value = 1.0,
            Position = new Vector2(px + 660, row - 2), Size = new Vector2(90, 30),
            Editable = true, TooltipText = "Eye opacity."
        };
        if (Font != null) _opacityInput.AddThemeFontOverride("font", Font);
        _opacityInput.AddThemeFontSizeOverride("font_size", 12);
        _opacityInput.ValueChanged += val =>
        {
            if (_selectedEye == null) return;
            _selectedEye.Opacity = (float)val;
            _selectedEye.Container.Modulate = new Color(1, 1, 1, _selectedEye.Opacity);
        };
        Screen.AddChild(_opacityInput);
    }

    private void BuildBonePanel(Vector2 screenSize)
    {
        float px = EditorTheme.SidebarWidth + EditorTheme.Padding;
        float py = screenSize.Y - 213;
        float pw = screenSize.X - EditorTheme.SidebarWidth - EditorTheme.Padding * 2;
        float ph = 208f;

        _bonePanelContainer = new Control
        {
            Position = new Vector2(px, py),
            Size = new Vector2(pw, ph),
            Visible = false,
            MouseFilter = Control.MouseFilterEnum.Ignore
        };
        Screen.AddChild(_bonePanelContainer);

        _bonePanelContainer.AddChild(new ColorRect { Color = EditorTheme.PanelBg, Position = Vector2.Zero, Size = new Vector2(pw, ph) });
        _bonePanelContainer.AddChild(MakeLabel("Bone Anchoring", new Vector2(10, 4), new Vector2(120, 16), 11, EditorTheme.Accent));
        _anchorPanelHint = MakeLabel("Which bone this eye follows.",
            new Vector2(140, 4), new Vector2(pw - 160, 16), 10, EditorTheme.TextDim);
        _bonePanelContainer.AddChild(_anchorPanelHint);

        float row2 = 26;
        _bonePanelContainer.AddChild(MakeLabel("Default bone:", new Vector2(10, row2 + 3), new Vector2(100, 24), 12, EditorTheme.TextNormal));

        _anchorBoneInput = new LineEdit
        {
            Text = "root", Position = new Vector2(112, row2 - 2), Size = new Vector2(130, 30),
            TooltipText = "The bone this eye follows."
        };
        if (Font != null) _anchorBoneInput.AddThemeFontOverride("font", Font);
        _anchorBoneInput.AddThemeFontSizeOverride("font_size", 12);
        _anchorBoneInput.TextChanged += text =>
        {
            _selectedAnchorBone = text;
            if (_selectedEye != null) _selectedEye.AnchorBone = text;
        };
        _bonePanelContainer.AddChild(_anchorBoneInput);

        _showBonesButton = MakeButton("Show Bones", new Vector2(255, row2 - 2), new Vector2(105, 30), 11);
        _showBonesButton.Pressed += ToggleBoneMarkers;
        _bonePanelContainer.AddChild(_showBonesButton);

        _showAllBoneNamesCheckbox = new CheckBox
        {
            Text = "All names", Position = new Vector2(370, row2 - 2), Size = new Vector2(100, 30)
        };
        ApplyFont(_showAllBoneNamesCheckbox, 11, EditorTheme.TextNormal);
        _showAllBoneNamesCheckbox.Toggled += _ => { if (_showingBones) RebuildBoneMarkerNodes(); };
        _bonePanelContainer.AddChild(_showAllBoneNamesCheckbox);

        _anchorPosLabel = MakeLabel("", new Vector2(480, row2 + 3), new Vector2(300, 24), 11, EditorTheme.Accent);
        _bonePanelContainer.AddChild(_anchorPosLabel);

        float divY = row2 + 34;
        _bonePanelContainer.AddChild(new ColorRect { Color = EditorTheme.Divider, Position = new Vector2(10, divY), Size = new Vector2(pw - 20, 1) });

        float segHdrY = divY + 6;
        _bonePanelContainer.AddChild(MakeLabel("Per-animation bone switching", new Vector2(10, segHdrY), new Vector2(220, 16), 11, EditorTheme.AccentWarm));
        _bonePanelContainer.AddChild(MakeLabel("Only needed if an eye must follow different bones at different times.",
            new Vector2(240, segHdrY), new Vector2(pw - 260, 16), 10, EditorTheme.TextDim));

        float btnY = segHdrY + 20;
        _splitHereButton = MakeButton("Split here", new Vector2(10, btnY), new Vector2(100, 26), 11);
        _splitHereButton.Pressed += SplitAtScrubPosition;
        _bonePanelContainer.AddChild(_splitHereButton);

        _clearSegmentsButton = MakeButton("Clear segments", new Vector2(120, btnY), new Vector2(115, 26), 11);
        _clearSegmentsButton.Pressed += ClearSegmentsForSelected;
        _bonePanelContainer.AddChild(_clearSegmentsButton);

        _activeSegmentLabel = MakeLabel("", new Vector2(250, btnY + 3), new Vector2(pw - 270, 20), 11, EditorTheme.TextDim);
        _bonePanelContainer.AddChild(_activeSegmentLabel);

        float listY = btnY + 30;
        float listH = ph - listY - 4;
        _segmentScroll = new ScrollContainer
        {
            Position = new Vector2(4, listY),
            Size = new Vector2(pw - 8, listH),
            HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled
        };
        _bonePanelContainer.AddChild(_segmentScroll);

        _segmentList = new VBoxContainer { CustomMinimumSize = new Vector2(pw - 24, 0) };
        _segmentList.AddThemeConstantOverride("separation", 2);
        _segmentScroll.AddChild(_segmentList);
    }

    private void SetPanelsVisible(bool visible)
    {
        _animPanelContainer.Visible = visible;
        _bonePanelContainer.Visible = visible;
        _hiddenByDefaultCheckbox.Visible = visible;
        _opacityLabel.Visible = visible;
        _opacityInput.Visible = visible;
    }

    // ═══════════════════════════════════════════════
    //  SIDEBAR FILTER
    // ═══════════════════════════════════════════════

    private void RefreshSidebarFilter()
    {
        string search = _searchInput.Text?.Trim() ?? "";
        bool hasSearch = search.Length > 0;

        string currentSection = null;
        var sectionHasVisible = new Dictionary<string, bool>();

        foreach (var child in _sceneList.GetChildren())
        {
            if (child is not Control ctrl) continue;

            if (ctrl.HasMeta("section_header"))
            {
                currentSection = ctrl.GetMeta("section_header").AsString();
                if (!sectionHasVisible.ContainsKey(currentSection))
                    sectionHasVisible[currentSection] = false;
                continue;
            }

            if (child is not Button btn || !btn.HasMeta("scene_path")) continue;

            string path = btn.GetMeta("scene_path").AsString();
            string name = btn.GetMeta("display_name").AsString();
            bool hasConfig = ScreenGooglyEyesRegistry.Configs.ContainsKey(path);

            bool matchesSearch = !hasSearch || name.Contains(search, StringComparison.OrdinalIgnoreCase);
            bool matchesFilter = _filterMode switch
            {
                FilterMode.Configured => hasConfig,
                FilterMode.Unconfigured => !hasConfig,
                _ => true
            };

            bool visible = matchesSearch && matchesFilter;
            btn.Visible = visible;

            if (visible && currentSection != null)
                sectionHasVisible[currentSection] = true;
        }

        foreach (var child in _sceneList.GetChildren())
        {
            if (child is not Control ctrl || !ctrl.HasMeta("section_header")) continue;
            string section = ctrl.GetMeta("section_header").AsString();
            ctrl.Visible = sectionHasVisible.TryGetValue(section, out bool any) && any;
        }
    }

    // ═══════════════════════════════════════════════
    //  SCENE SELECTION
    // ═══════════════════════════════════════════════

    private void SelectScene(string displayName, string scenePath)
    {
        ClearAll();
        ClearBoneMarkers();
        ClearScene();
        HighlightButton(displayName, scenePath);

        _showingBones = false;
        _showBonesButton.Text = "Show Bones";
        _skeletonGodot = null;
        _spineNode = null;
        _spineSprite = null;
        _hasSpine = false;
        _animController = null;
        _animState = null;
        _cachedTrackEntry = null;
        _currentScenePath = scenePath;
        _isPlaying = false;
        _playPauseButton.Text = "Play";
        _editingSegmentIndex = -1;
        _currentTrack1Anim = "";
        _appliedTrack1Anim = "";

        Screen.ResetZoomPan();

        // Load the scene
        var packedScene = ResourceLoader.Load<PackedScene>(scenePath);
        if (packedScene == null)
        {
            SetInfo("Failed to load scene: " + scenePath);
            return;
        }

        _currentScene = packedScene.Instantiate();
        PreviewRoot.AddChild(_currentScene);

        // Center the scene in the preview area
        if (_currentScene is Control ctrl)
        {
            ctrl.Position = new Vector2(
                (PreviewArea.Size.X - ctrl.Size.X) / 2f,
                (PreviewArea.Size.Y - ctrl.Size.Y) / 2f
            );
        }
        else if (_currentScene is Node2D node2d)
        {
            node2d.Position = new Vector2(
                PreviewArea.Size.X / 2f,
                PreviewArea.Size.Y / 2f
            );
        }

        // Find the SpineSprite in the scene tree
        var spineNode = FindSpineSprite(_currentScene);
        if (spineNode != null)
        {
            try
            {
                _spineSprite = spineNode as Node2D;
                _animController = new MegaSprite((Variant)spineNode);
                _animState = _animController.GetAnimationState();
                var skeleton = _animController.GetSkeleton();
                if (skeleton != null)
                {
                    _skeletonGodot = skeleton.BoundObject as GodotObject;
                    _spineNode = _animController.BoundObject as Node2D;
                }
                _hasSpine = _skeletonGodot != null;
                PopulateAnimationList();
                if (_availableAnims.Count > 0)
                {
                    string startAnim = _availableAnims[_animDropdown.Selected >= 0 ? _animDropdown.Selected : 0];
                    SetAnimationPaused(startAnim, 0f);
                }
            }
            catch (Exception e)
            {
                GD.PrintErr("[GooglyEyes] Failed to set up Spine from scene: " + e);
                _animController = null;
                _hasSpine = false;
            }
        }

        // Show/hide bone and animation panels based on whether spine was found
        _animPanelContainer.Visible = _hasSpine;
        _bonePanelContainer.Visible = _hasSpine;
        _hiddenByDefaultCheckbox.Visible = _hasSpine;

        string modeHint = _hasSpine ? "" : " (static scene — no bones or animations)";
        SetInfo(displayName + " loaded" + modeHint + " — click the preview to place eyes!");
        AdvanceWorkflow(WorkflowStep.PlaceEyes);

        if (LoadPresets(scenePath))
        {
            SetInfo(displayName + " loaded with " + _placements.Count + " preset eye(s).");
            if (_placements.Count > 0) AdvanceWorkflow(WorkflowStep.AdjustEyes);
        }
        RefreshSegmentUI();
    }

    /// <summary>Recursively searches for a SpineSprite node in the tree.</summary>
    private static Node FindSpineSprite(Node root)
    {
        if (root.GetClass() == "SpineSprite") return root;
        foreach (var child in root.GetChildren())
        {
            var found = FindSpineSprite(child);
            if (found != null) return found;
        }
        return null;
    }

    private void ClearScene()
    {
        if (_currentScene != null && GodotObject.IsInstanceValid(_currentScene))
        {
            _currentScene.QueueFree();
            _currentScene = null;
        }
        _spineSprite = null;
        _hasSpine = false;
        _animController = null;
        _animState = null;
        _cachedTrackEntry = null;
        _skeletonGodot = null;
        _spineNode = null;
        _currentScenePath = null;
    }

    private void HighlightButton(string displayName, string scenePath)
    {
        if (_selectedButton != null && GodotObject.IsInstanceValid(_selectedButton))
        {
            string prevPath = _selectedButton.GetMeta("scene_path").AsString();
            bool prev = ScreenGooglyEyesRegistry.Configs.ContainsKey(prevPath);
            ApplyFont(_selectedButton, 12, prev ? EditorTheme.AccentGreen : EditorTheme.TextNormal);
        }
        foreach (var child in _sceneList.GetChildren())
        {
            if (child is Button btn && btn.HasMeta("scene_path") && btn.GetMeta("scene_path").AsString() == scenePath)
            {
                _selectedButton = btn;
                ApplyFont(btn, 12, EditorTheme.SelectedEntryColor);
                break;
            }
        }
    }

    // ═══════════════════════════════════════════════
    //  INPUT
    // ═══════════════════════════════════════════════

    public override bool HasEyeAt(Vector2 contentPos) => FindEyeAt(contentPos) != null;

    public override bool HandleInput(InputEvent @event, Vector2 localPos, Vector2 contentPos)
    {
        if (_currentScene == null) return false;

        if (@event is InputEventMouseButton mb)
        {
            if (mb.ButtonIndex == MouseButton.Left && mb.Pressed)
            {
                var hit = FindEyeAt(contentPos);
                if (hit != null)
                {
                    SelectEye(hit);
                    _dragging = hit;
                    _isDragging = true;
                    AdvanceWorkflow(WorkflowStep.AdjustEyes);
                }
                else
                {
                    DeselectCurrent();
                    PlaceEye(contentPos);
                    SelectEye(_placements[^1]);
                    if (Screen.CurrentStep == WorkflowStep.PlaceEyes && _placements.Count >= 2)
                        AdvanceWorkflow(WorkflowStep.AdjustEyes);
                }
                return true;
            }
            if (mb.ButtonIndex == MouseButton.Left && !mb.Pressed)
            {
                if (_isDragging && _dragging != null && _hasSpine)
                {
                    if (_editingSegmentIndex >= 0 && _dragging == _selectedEye
                        && _selectedEye.BoneTimelines.TryGetValue(_currentAnimName, out var segs)
                        && _editingSegmentIndex < segs.Count)
                    {
                        CacheSegmentOffsetFromCurrentPose(segs[_editingSegmentIndex]);
                        RefreshSegmentUI();
                    }
                    else
                    {
                        CacheSpineOffset(_dragging);
                    }
                }
                _isDragging = false;
                _dragging = null;
                return true;
            }
            if (mb.ButtonIndex == MouseButton.Right && mb.Pressed)
            {
                var hit = FindEyeAt(contentPos);
                if (hit != null) { RemoveEye(hit); return true; }
            }
            if (mb.ButtonIndex == MouseButton.WheelUp)
            {
                var hit = FindEyeAt(contentPos);
                if (hit != null) { ResizeEye(hit, 0.1f); return true; }
            }
            if (mb.ButtonIndex == MouseButton.WheelDown)
            {
                var hit = FindEyeAt(contentPos);
                if (hit != null) { ResizeEye(hit, -0.1f); return true; }
            }
        }
        else if (@event is InputEventMouseMotion && _isDragging && _dragging != null)
        {
            _dragging.Position = contentPos;
            _dragging.Container.Position = contentPos;
            return true;
        }

        return false;
    }

    // ═══════════════════════════════════════════════
    //  TOOLBAR ACTIONS
    // ═══════════════════════════════════════════════

    public override void Export()
    {
        if (_placements.Count == 0 || string.IsNullOrEmpty(_currentScenePath))
        {
            SetOutput("Nothing to export.");
            return;
        }
        AdvanceWorkflow(WorkflowStep.Export);

        var sb = new StringBuilder();
        sb.AppendLine("// Googly Eyes config for screen: " + _currentScenePath);
        sb.AppendLine("{ \"" + _currentScenePath + "\", new EyeConfig[] {");

        if (_hasSpine)
        {
            EnsureSkeletonRefs();
            float spineScale = _spineNode?.Scale.X ?? 1f;

            foreach (var p in _placements)
            {
                var offset = ComputeSpineOffset(p);
                if (offset == null) return;

                sb.Append("    new EyeConfig { ");
                sb.Append("Offset = new Vector2(" + offset.Value.X.ToString("F1") + "f, " + offset.Value.Y.ToString("F1") + "f), ");
                sb.Append("Scale = " + (p.Scale / spineScale).ToString("F2") + "f, ");
                sb.Append("AnchorBone = \"" + p.AnchorBone + "\"");

                if (p.Opacity < 0.99f)
                    sb.Append(", Opacity = " + p.Opacity.ToString("F2") + "f");
                if (p.HiddenByDefault)
                    sb.Append(", HiddenByDefault = true");

                if (p.BoneTimelines.Count > 0)
                {
                    sb.AppendLine(",");
                    sb.Append("        BoneSegments = new Dictionary<string, BoneSegment[]> {");
                    foreach (var tlKvp in p.BoneTimelines)
                    {
                        if (tlKvp.Value.Count == 0) continue;
                        sb.AppendLine();
                        sb.Append("            { \"" + tlKvp.Key + "\", new BoneSegment[] {");
                        foreach (var seg in tlKvp.Value)
                        {
                            sb.AppendLine();
                            sb.Append("                new BoneSegment { ");
                            sb.Append("StartTime = " + seg.StartTime.ToString("F2") + "f, ");
                            sb.Append("EndTime = " + seg.EndTime.ToString("F2") + "f, ");
                            if (seg.Hidden)
                            {
                                sb.Append("Hidden = true");
                            }
                            else
                            {
                                var segOff = seg.HasSpineOffset ? seg.SpineOffset : Vector2.Zero;
                                sb.Append("BoneName = \"" + seg.BoneName + "\", ");
                                sb.Append("Offset = new Vector2(" + segOff.X.ToString("F1") + "f, " + segOff.Y.ToString("F1") + "f)");
                                if (seg.OpacityStart < 0.99f || seg.OpacityEnd < 0.99f)
                                {
                                    sb.Append(", OpacityStart = " + seg.OpacityStart.ToString("F2") + "f");
                                    sb.Append(", OpacityEnd = " + seg.OpacityEnd.ToString("F2") + "f");
                                }
                            }
                            sb.Append(" },");
                        }
                        sb.AppendLine();
                        sb.Append("            } },");
                    }
                    sb.AppendLine();
                    sb.Append("        }");
                }
                sb.AppendLine(" },");
            }
        }
        else
        {
            // Static scene: offset from scene root position
            Vector2 scenePos = GetScenePosition();

            foreach (var p in _placements)
            {
                var offset = p.Position - scenePos;
                sb.Append("    new EyeConfig { ");
                sb.Append("Offset = new Vector2(" + offset.X.ToString("F1") + "f, " + offset.Y.ToString("F1") + "f), ");
                sb.Append("Scale = " + p.Scale.ToString("F2") + "f");
                if (p.Opacity < 0.99f)
                    sb.Append(", Opacity = " + p.Opacity.ToString("F2") + "f");
                sb.AppendLine(" },");
            }
        }

        sb.AppendLine("}},");

        GD.Print("[GooglyEyes] === SCREEN EXPORT ===");
        GD.Print(sb.ToString());
        GD.Print("[GooglyEyes] === END SCREEN EXPORT ===");

        int totalSegs = _placements.Sum(p => p.BoneTimelines.Values.Sum(l => l.Count));
        SetOutput("Exported " + _placements.Count + " eye(s)" + (_hasSpine ? ", " + totalSegs + " segment(s)" : " (static)") + " — check console!");
    }

    private Vector2 GetScenePosition()
    {
        if (_currentScene is Control c) return c.Position;
        if (_currentScene is Node2D n) return n.Position;
        return Vector2.Zero;
    }

    public override void ClearAll()
    {
        foreach (var p in _placements)
            if (GodotObject.IsInstanceValid(p.Container)) p.Container.QueueFree();
        _placements.Clear();
        DeselectCurrent();
    }

    public override void UndoLast()
    {
        if (_placements.Count > 0) RemoveEye(_placements[^1]);
    }

    public override void DuplicateSelected()
    {
        if (_selectedEye == null) return;
        PlaceEye(_selectedEye.Position + new Vector2(30, 30));
        var n = _placements[^1];
        n.Scale = _selectedEye.Scale;
        n.Opacity = _selectedEye.Opacity;
        n.AnchorBone = _selectedEye.AnchorBone;
        n.HiddenByDefault = _selectedEye.HiddenByDefault;
        n.Container.Scale = Vector2.One * n.Scale;
        n.Container.Modulate = new Color(1, 1, 1, n.Opacity);
        foreach (var kvp in _selectedEye.BoneTimelines)
        {
            n.BoneTimelines[kvp.Key] = kvp.Value.Select(s => new BoneSegment
            {
                StartTime = s.StartTime, EndTime = s.EndTime,
                BoneName = s.BoneName, SpineOffset = s.SpineOffset,
                HasSpineOffset = s.HasSpineOffset, Hidden = s.Hidden,
                OpacityStart = s.OpacityStart, OpacityEnd = s.OpacityEnd
            }).ToList();
        }
        SelectEye(n);
    }

    public override void OnScaleChanged(double value)
    {
        if (_selectedEye == null) return;
        _selectedEye.Scale = (float)value;
        _selectedEye.Container.Scale = Vector2.One * _selectedEye.Scale;
    }

    public override void DeselectCurrent()
    {
        _selectedEye = null;
        _editingSegmentIndex = -1;
        _hiddenByDefaultCheckbox.SetPressedNoSignal(false);
        _opacityInput.SetValueNoSignal(1.0);
        Screen.RefreshSelectionPanel();
        RefreshSegmentUI();
    }

    // ═══════════════════════════════════════════════
    //  EYE MANAGEMENT
    // ═══════════════════════════════════════════════

    private EyePlacement FindEyeAt(Vector2 pos)
    {
        float bestDist = float.MaxValue;
        EyePlacement best = null;
        foreach (var p in _placements)
        {
            if (!p.Container.Visible) continue;
            if (!IsWithinEye(pos, p.Position, p.Scale)) continue;
            float dist = p.Position.DistanceTo(pos);
            if (dist < bestDist) { bestDist = dist; best = p; }
        }
        return best;
    }

    private void PlaceEye(Vector2 contentPos)
    {
        var container = CreateEyeContainer(contentPos);
        var placement = new EyePlacement
        {
            Position = contentPos, Scale = 1f,
            AnchorBone = _hasSpine ? _anchorBoneInput.Text : "",
            EyeSprite = container.GetChild<Sprite2D>(0),
            IrisSprite = container.GetChild<Sprite2D>(1),
            Container = container
        };
        _placements.Add(placement);
        if (_hasSpine) CacheSpineOffset(placement);
        SetInfo(_placements.Count + " eye(s) placed.");
    }

    private void RemoveEye(EyePlacement p)
    {
        if (p == _selectedEye) DeselectCurrent();
        if (GodotObject.IsInstanceValid(p.Container)) p.Container.QueueFree();
        _placements.Remove(p);
        SetInfo(_placements.Count + " eye(s) remaining.");
    }

    private void ResizeEye(EyePlacement p, float delta)
    {
        p.Scale = Mathf.Max(0.01f, p.Scale + delta * 0.5f);
        p.Container.Scale = Vector2.One * p.Scale;
        if (p == _selectedEye) Screen.SetScaleValueNoSignal(p.Scale);
    }

    private void SelectEye(EyePlacement eye)
    {
        _selectedEye = eye;
        _anchorBoneInput.Text = eye.AnchorBone;
        _hiddenByDefaultCheckbox.SetPressedNoSignal(eye.HiddenByDefault);
        _opacityInput.SetValueNoSignal(eye.Opacity);
        _editingSegmentIndex = -1;
        Screen.RefreshSelectionPanel();
        RefreshSegmentUI();
    }

    // ═══════════════════════════════════════════════
    //  PRESET LOADING
    // ═══════════════════════════════════════════════

    private bool LoadPresets(string scenePath)
    {
        if (!ScreenGooglyEyesRegistry.Configs.TryGetValue(scenePath, out var configs)) return false;
        if (configs.Length == 0) return false;

        if (_hasSpine)
        {
            if (_skeletonGodot == null || _spineNode == null) return false;

            if (_availableAnims.Count > 0)
            {
                string startAnim = _availableAnims[_animDropdown.Selected >= 0 ? _animDropdown.Selected : 0];
                SetAnimationPaused(startAnim, 0f);
            }
            float spineScale = _spineNode.Scale.X;

            foreach (var config in configs)
            {
                if (string.IsNullOrEmpty(config.AnchorBone)) continue;
                var anchorBone = _skeletonGodot.Call("find_bone", config.AnchorBone).AsGodotObject();
                if (anchorBone == null)
                {
                    GD.PrintErr("[GooglyEyes] Screen preset: bone '" + config.AnchorBone + "' not found, skipping.");
                    continue;
                }

                var bx = (float)anchorBone.Call("get_world_x");
                var by = (float)anchorBone.Call("get_world_y");
                var bonePreviewPos = SpineToPreview(new Vector2(bx, by));
                var rotatedOffset = RotateByBone(config.Offset, anchorBone);
                var previewPos = bonePreviewPos + rotatedOffset * _spineNode.Scale;

                float editorScale = config.Scale * spineScale;
                var container = CreateEyeContainer(previewPos, editorScale);

                var placement = new EyePlacement
                {
                    Position = previewPos, Scale = editorScale,
                    AnchorBone = config.AnchorBone,
                    Opacity = config.Opacity,
                    EyeSprite = container.GetChild<Sprite2D>(0),
                    IrisSprite = container.GetChild<Sprite2D>(1),
                    Container = container,
                    HiddenByDefault = config.HiddenByDefault,
                    SpineOffset = config.Offset, HasSpineOffset = true
                };

                if (config.HiddenByDefault) container.Modulate = new Color(1, 1, 1, 0.25f);
                else if (config.Opacity < 0.99f) container.Modulate = new Color(1, 1, 1, config.Opacity);

                if (config.BoneSegments != null)
                {
                    foreach (var kvp in config.BoneSegments)
                    {
                        if (kvp.Value == null || kvp.Value.Length == 0) continue;
                        var editorSegs = new List<BoneSegment>();
                        foreach (var src in kvp.Value)
                        {
                            var seg = new BoneSegment
                            {
                                StartTime = src.StartTime, EndTime = src.EndTime,
                                BoneName = src.BoneName ?? config.AnchorBone,
                                Hidden = src.Hidden,
                                OpacityStart = src.OpacityStart, OpacityEnd = src.OpacityEnd
                            };
                            if (!src.Hidden) { seg.SpineOffset = src.Offset; seg.HasSpineOffset = true; }
                            editorSegs.Add(seg);
                        }
                        placement.BoneTimelines[kvp.Key] = editorSegs;
                    }
                }
                _placements.Add(placement);
            }
        }
        else
        {
            // Static scene: offset from scene root position
            Vector2 scenePos = GetScenePosition();

            foreach (var config in configs)
            {
                var previewPos = scenePos + config.Offset;
                var container = CreateEyeContainer(previewPos, config.Scale);

                if (config.Opacity < 0.99f) container.Modulate = new Color(1, 1, 1, config.Opacity);

                _placements.Add(new EyePlacement
                {
                    Position = previewPos, Scale = config.Scale,
                    Opacity = config.Opacity,
                    EyeSprite = container.GetChild<Sprite2D>(0),
                    IrisSprite = container.GetChild<Sprite2D>(1),
                    Container = container
                });
            }
        }

        return _placements.Count > 0;
    }

    // ═══════════════════════════════════════════════
    //  ANIMATION
    // ═══════════════════════════════════════════════

    private void PopulateAnimationList()
    {
        _animDropdown.Clear();
        _availableAnims.Clear();
        _track1Dropdown.Clear();
        _availableTrack1Anims.Clear();
        _track1Dropdown.AddItem("(none)");
        _availableTrack1Anims.Add("");

        if (_animController == null) return;

        var skeleton = _animController.GetSkeleton();
        if (skeleton == null) return;
        var data = skeleton.GetData();
        if (data == null) return;

        var bound = data.BoundObject as GodotObject;
        if (bound == null) return;

        var anims = bound.Call("get_animations").AsGodotArray();
        foreach (var animVariant in anims)
        {
            var anim = animVariant.AsGodotObject();
            var name = anim.Call("get_name").AsString();
            if (string.IsNullOrEmpty(name)) continue;

            if (name.Contains("_tracks/"))
            {
                _availableTrack1Anims.Add(name);
                _track1Dropdown.AddItem(name);
            }
            else
            {
                _availableAnims.Add(name);
                _animDropdown.AddItem(name);
            }
        }

        if (_availableAnims.Count > 0)
        {
            int bestIdx = _availableAnims.IndexOf("animation");
            if (bestIdx < 0) bestIdx = _availableAnims.FindIndex(n => n.Contains("idle", StringComparison.OrdinalIgnoreCase));
            if (bestIdx < 0) bestIdx = _availableAnims.FindIndex(n => n.Contains("loop", StringComparison.OrdinalIgnoreCase));
            _animDropdown.Selected = bestIdx >= 0 ? bestIdx : 0;
        }
        _track1Dropdown.Selected = 0;
    }

    private void OnAnimationSelected(long index)
    {
        if (index < 0 || index >= _availableAnims.Count) return;
        _isPlaying = false;
        _playPauseButton.Text = "Play";
        _editingSegmentIndex = -1;
        SetAnimationPaused(_availableAnims[(int)index], 0f);
        RefreshSegmentUI();
    }

    private void OnTrack1Selected(long index)
    {
        if (index < 0 || index >= _availableTrack1Anims.Count) return;
        _currentTrack1Anim = _availableTrack1Anims[(int)index];
        ApplyTrack1();
    }

    private void ApplyTrack1()
    {
        if (_animController == null || _currentTrack1Anim == _appliedTrack1Anim) return;
        _appliedTrack1Anim = _currentTrack1Anim;
        _animState ??= _animController.GetAnimationState();
        if (string.IsNullOrEmpty(_currentTrack1Anim))
        {
            try { _animState.AddEmptyAnimation(1); } catch { }
            return;
        }
        var entry = _animState.SetAnimation(_currentTrack1Anim, true, 1);
        entry?.SetTimeScale(_isPlaying ? 1f : 0f);
    }

    private void OnFrameSliderChanged(double value)
    {
        if (_isPlaying) return;
        SetAnimationPaused(_currentAnimName, (float)(value / 100.0));
    }

    private void TogglePlayPause()
    {
        if (_animController == null) return;
        _isPlaying = !_isPlaying;
        _playPauseButton.Text = _isPlaying ? "Pause" : "Play";

        if (_cachedTrackEntry != null)
        {
            if (_isPlaying) { _cachedTrackEntry.SetTimeScale(1f); _cachedTrackEntry.SetLoop(true); }
            else _cachedTrackEntry.SetTimeScale(0f);
        }

        _animState ??= _animController.GetAnimationState();
        try
        {
            var t1 = _animState.GetCurrent(1);
            t1?.SetTimeScale(_isPlaying ? 1f : 0f);
        }
        catch (NullReferenceException) { }
    }

    private void SetAnimationPaused(string animName, float normalizedTime)
    {
        if (_animController == null) return;
        _currentAnimName = animName;
        _animState ??= _animController.GetAnimationState();
        var entry = _animState.SetAnimation(animName, false);
        if (entry == null) return;
        _cachedTrackEntry = entry;
        entry.SetTimeScale(0f);
        float duration = entry.GetAnimationEnd();
        float time = normalizedTime * duration;
        entry.SetTrackTime(time);
        _animState.Update(0f);
        _animState.Apply(_animController.GetSkeleton());
        _frameLabel.Text = $"{time:F2}s / {duration:F2}s";
        if (_showingBones) RefreshBonePositions();
    }

    private void PoseSkeletonAt(string animName, float time)
    {
        if (_animController == null) return;
        _animState ??= _animController.GetAnimationState();
        var entry = _animState.SetAnimation(animName, false);
        if (entry == null) return;
        entry.SetTimeScale(0f);
        entry.SetTrackTime(time);
        _animState.Update(0f);
        _animState.Apply(_animController.GetSkeleton());
    }

    private float GetCurrentAnimDuration() => _cachedTrackEntry?.GetAnimationEnd() ?? 0f;
    private float GetCurrentScrubTime() => (float)(_frameSlider.Value / 100.0) * GetCurrentAnimDuration();

    // ═══════════════════════════════════════════════
    //  EYE POSITION TRACKING
    // ═══════════════════════════════════════════════

    private void UpdateEyePositionsFromSkeleton()
    {
        if (_animController == null || _skeletonGodot == null || _spineNode == null) return;
        if (!GodotObject.IsInstanceValid(_spineNode) || _cachedTrackEntry == null) return;

        float dur = _cachedTrackEntry.GetAnimationEnd();
        float currentTime = dur > 0f ? _cachedTrackEntry.GetTrackTime() % dur : 0f;

        UpdateActiveSegmentLabel(currentTime);

        foreach (var p in _placements)
        {
            if (!GodotObject.IsInstanceValid(p.Container)) continue;
            if (_isDragging && p == _dragging) continue;

            bool shouldHide = ResolveHidden(p, _currentAnimName, currentTime);
            if (shouldHide)
            {
                p.Container.Modulate = new Color(1, 1, 1, 0.15f);
                continue;
            }
            p.Container.Modulate = p.HiddenByDefault
                ? new Color(1, 1, 1, 0.7f)
                : new Color(1, 1, 1, ResolveOpacity(p, _currentAnimName, currentTime));

            string activeBone = ResolveActiveBone(p, _currentAnimName, currentTime);
            var bone = _skeletonGodot.Call("find_bone", activeBone).AsGodotObject();
            if (bone == null) continue;

            var bonePos = new Vector2((float)bone.Call("get_world_x"), (float)bone.Call("get_world_y"));
            var offset = ResolveSpineOffset(p, _currentAnimName, currentTime);
            var rotated = RotateByBone(offset, bone);
            var previewPos = SpineToPreview(bonePos) + rotated * _spineNode.Scale;

            p.Container.Position = previewPos;
            p.Position = previewPos;
        }
    }

    private string ResolveActiveBone(EyePlacement p, string anim, float time)
    {
        if (p.BoneTimelines.TryGetValue(anim, out var segs) && segs.Count > 0)
        {
            foreach (var s in segs)
                if (time >= s.StartTime && time < s.EndTime) return s.BoneName;
            return segs[^1].BoneName;
        }
        return p.AnchorBone;
    }

    private Vector2 ResolveSpineOffset(EyePlacement p, string anim, float time)
    {
        if (p.BoneTimelines.TryGetValue(anim, out var segs) && segs.Count > 0)
        {
            foreach (var s in segs)
                if (time >= s.StartTime && time < s.EndTime)
                    return s.HasSpineOffset ? s.SpineOffset : p.SpineOffset;
            var last = segs[^1];
            return last.HasSpineOffset ? last.SpineOffset : p.SpineOffset;
        }
        return p.HasSpineOffset ? p.SpineOffset : Vector2.Zero;
    }

    private bool ResolveHidden(EyePlacement p, string anim, float time)
    {
        if (p.BoneTimelines.TryGetValue(anim, out var segs) && segs.Count > 0)
        {
            foreach (var s in segs)
                if (time >= s.StartTime && time < s.EndTime) return s.Hidden;
            return segs[^1].Hidden;
        }
        return p.HiddenByDefault;
    }

    private float ResolveOpacity(EyePlacement p, string anim, float time)
    {
        if (p.BoneTimelines.TryGetValue(anim, out var segs) && segs.Count > 0)
        {
            foreach (var s in segs)
            {
                if (time >= s.StartTime && time < s.EndTime)
                {
                    if (s.Hidden) return 0f;
                    float segDur = s.EndTime - s.StartTime;
                    if (segDur <= 0f) return s.OpacityStart;
                    float t = (time - s.StartTime) / segDur;
                    return Mathf.Lerp(s.OpacityStart, s.OpacityEnd, t);
                }
            }
            var last = segs[^1];
            return last.Hidden ? 0f : last.OpacityEnd;
        }
        return p.Opacity;
    }

    // ═══════════════════════════════════════════════
    //  BONE MARKERS
    // ═══════════════════════════════════════════════

    private void ToggleBoneMarkers()
    {
        if (_showingBones) { ClearBoneMarkers(); _showBonesButton.Text = "Show Bones"; _showingBones = false; }
        else { ShowBoneMarkers(); _showBonesButton.Text = "Hide Bones"; _showingBones = true; }
    }

    private void ShowBoneMarkers()
    {
        ClearBoneMarkers();
        EnsureSkeletonRefs();
        if (_skeletonGodot == null) return;
        RefreshBonePositions();
        RebuildBoneMarkerNodes();
    }

    private void EnsureSkeletonRefs()
    {
        if (_skeletonGodot == null && _animController != null)
        {
            var skeleton = _animController.GetSkeleton();
            if (skeleton != null)
            {
                _skeletonGodot = skeleton.BoundObject as GodotObject;
                _spineNode = _animController.BoundObject as Node2D;
            }
        }
    }

    private void RefreshBonePositions()
    {
        _boneData.Clear();
        _cachedBones = null;
        if (_skeletonGodot == null) return;
        _cachedBones = _skeletonGodot.Call("get_bones").AsGodotArray();
        foreach (var bv in _cachedBones)
        {
            var bone = bv.AsGodotObject();
            var data = bone.Call("get_data").AsGodotObject();
            _boneData.Add(new BoneInfo
            {
                Name = data.Call("get_bone_name").AsString(),
                SpineWorldPos = new Vector2((float)bone.Call("get_world_x"), (float)bone.Call("get_world_y"))
            });
        }
        if (_boneMarkers.Count > 0) RebuildBoneMarkerNodes();
    }

    private void UpdateBoneMarkerPositions()
    {
        if (_cachedBones == null) return;
        for (int i = 0; i < _cachedBones.Count && i < _boneData.Count; i++)
        {
            var bone = _cachedBones[i].AsGodotObject();
            _boneData[i] = new BoneInfo
            {
                Name = _boneData[i].Name,
                SpineWorldPos = new Vector2((float)bone.Call("get_world_x"), (float)bone.Call("get_world_y"))
            };
        }
        for (int i = 0; i < _boneData.Count && i < _boneMarkers.Count; i++)
        {
            var contentPos = SpineToPreview(_boneData[i].SpineWorldPos);
            _boneMarkers[i].Position = contentPos * Screen.Zoom + Screen.PanOffset;
        }
    }

    private void RebuildBoneMarkerNodes()
    {
        foreach (var m in _boneMarkers) if (GodotObject.IsInstanceValid(m)) m.QueueFree();
        _boneMarkers.Clear();
        foreach (var info in _boneData)
        {
            var marker = new Node2D();
            bool isAnchor = info.Name == _selectedAnchorBone;
            marker.AddChild(new ColorRect
            {
                Color = isAnchor ? new Color(1f, 0.2f, 0.2f, 0.9f) : new Color(0.4f, 0.8f, 0.4f, 0.5f),
                Position = new Vector2(-3, -3),
                Size = new Vector2(isAnchor ? 8 : 6, isAnchor ? 8 : 6)
            });
            if (_showAllBoneNamesCheckbox.ButtonPressed
                || info.Name.Contains("head") || info.Name.Contains("neck")
                || info.Name.Contains("eye") || info.Name.Contains("face")
                || info.Name == _selectedAnchorBone)
            {
                marker.AddChild(MakeLabel(info.Name, new Vector2(6, -8), new Vector2(150, 16), 10,
                    isAnchor ? new Color(1f, 0.3f, 0.3f) : EditorTheme.TextDim));
            }
            PreviewArea.AddChild(marker);
            _boneMarkers.Add(marker);
        }
    }

    private void ClearBoneMarkers()
    {
        foreach (var m in _boneMarkers) if (GodotObject.IsInstanceValid(m)) m.QueueFree();
        _boneMarkers.Clear();
        _boneData.Clear();
        _cachedBones = null;
    }

    // ═══════════════════════════════════════════════
    //  COORDINATE TRANSFORMS
    // ═══════════════════════════════════════════════

    private Vector2 SpineToPreview(Vector2 spinePos)
    {
        if (_currentScene == null || _spineNode == null) return spinePos;
        // The SpineSprite's position in the scene + spine world coords * scale
        Vector2 scenePos = _currentScene is Control c ? c.Position : (_currentScene is Node2D n ? n.Position : Vector2.Zero);
        Node2D spineParent = _spineSprite;
        Vector2 spineLocalPos = spineParent != null ? spineParent.Position : Vector2.Zero;
        return scenePos + spineLocalPos + spinePos * _spineNode.Scale;
    }

    private float GetBoneWorldRotation(GodotObject bone)
    {
        float a = (float)bone.Call("get_a");
        float c = (float)bone.Call("get_c");
        return Mathf.Atan2(c, a);
    }

    private Vector2 RotateByBone(Vector2 offset, GodotObject bone)
    {
        float rot = GetBoneWorldRotation(bone);
        float cos = Mathf.Cos(rot), sin = Mathf.Sin(rot);
        return new Vector2(offset.X * cos - offset.Y * sin, offset.X * sin + offset.Y * cos);
    }

    private Vector2 UnrotateByBone(Vector2 offset, GodotObject bone)
    {
        float rot = -GetBoneWorldRotation(bone);
        float cos = Mathf.Cos(rot), sin = Mathf.Sin(rot);
        return new Vector2(offset.X * cos - offset.Y * sin, offset.X * sin + offset.Y * cos);
    }

    // ═══════════════════════════════════════════════
    //  SPINE OFFSET CACHING
    // ═══════════════════════════════════════════════

    private void CacheSpineOffset(EyePlacement p)
    {
        if (_skeletonGodot == null || _spineNode == null || string.IsNullOrEmpty(p.AnchorBone))
        { p.HasSpineOffset = false; return; }
        var bone = _skeletonGodot.Call("find_bone", p.AnchorBone).AsGodotObject();
        if (bone == null) { p.HasSpineOffset = false; return; }
        var bonePreview = SpineToPreview(new Vector2((float)bone.Call("get_world_x"), (float)bone.Call("get_world_y")));
        var worldOffset = (p.Position - bonePreview) / _spineNode.Scale;
        p.SpineOffset = UnrotateByBone(worldOffset, bone);
        p.HasSpineOffset = true;
    }

    private Vector2? ComputeSpineOffset(EyePlacement p)
    {
        if (_skeletonGodot == null || string.IsNullOrEmpty(p.AnchorBone)) return Vector2.Zero;
        var bone = _skeletonGodot.Call("find_bone", p.AnchorBone).AsGodotObject();
        if (bone == null) { SetOutput("Bone '" + p.AnchorBone + "' not found!"); return null; }
        var bonePreview = SpineToPreview(new Vector2((float)bone.Call("get_world_x"), (float)bone.Call("get_world_y")));
        return UnrotateByBone((p.Position - bonePreview) / _spineNode.Scale, bone);
    }

    // ═══════════════════════════════════════════════
    //  SEGMENT LOGIC
    // ═══════════════════════════════════════════════

    private void UpdateActiveSegmentLabel(float time)
    {
        if (_selectedEye == null || _activeSegmentLabel == null) return;
        if (!_selectedEye.BoneTimelines.TryGetValue(_currentAnimName, out var segs) || segs.Count == 0)
        { _activeSegmentLabel.Text = ""; return; }

        for (int i = 0; i < segs.Count; i++)
        {
            if (time < segs[i].StartTime || time >= segs[i].EndTime) continue;
            var seg = segs[i];
            bool isEditing = i == _editingSegmentIndex;
            if (seg.Hidden)
            {
                _activeSegmentLabel.Text = "@ " + time.ToString("F2") + "s → segment " + (i + 1) + " (hidden)";
                ApplyFont(_activeSegmentLabel, 11, EditorTheme.TextDim);
            }
            else
            {
                string opInfo = "";
                if (seg.OpacityStart < 0.99f || seg.OpacityEnd < 0.99f)
                {
                    float segDur = seg.EndTime - seg.StartTime;
                    float t = segDur > 0f ? (time - seg.StartTime) / segDur : 0f;
                    opInfo = ", α=" + Mathf.Lerp(seg.OpacityStart, seg.OpacityEnd, t).ToString("F2");
                }
                _activeSegmentLabel.Text = "@ " + time.ToString("F2") + "s → segment " + (i + 1)
                    + " (" + seg.BoneName + ", visible" + opInfo + ")"
                    + (isEditing ? "  ← EDITING" : "");
                ApplyFont(_activeSegmentLabel, 11, isEditing ? EditorTheme.AccentWarm : EditorTheme.AccentGreen);
            }
            return;
        }
        _activeSegmentLabel.Text = "@ " + time.ToString("F2") + "s → past all segments";
        ApplyFont(_activeSegmentLabel, 11, EditorTheme.TextDim);
    }

    private void SplitAtScrubPosition()
    {
        if (_selectedEye == null) { SetOutput("Select an eye first."); return; }
        float duration = GetCurrentAnimDuration();
        if (duration <= 0f) return;
        float splitTime = GetCurrentScrubTime();

        if (!_selectedEye.BoneTimelines.ContainsKey(_currentAnimName))
            _selectedEye.BoneTimelines[_currentAnimName] = new List<BoneSegment>();
        var segments = _selectedEye.BoneTimelines[_currentAnimName];

        if (segments.Count == 0)
        {
            if (splitTime <= 0.01f || splitTime >= duration - 0.01f)
            { SetOutput("Scrub to a point between start and end."); return; }
            bool inherit = _selectedEye.HiddenByDefault;
            float baseOp = _selectedEye.Opacity;
            segments.Add(new BoneSegment { StartTime = 0f, EndTime = splitTime, BoneName = _selectedEye.AnchorBone, Hidden = inherit, OpacityStart = baseOp, OpacityEnd = baseOp });
            segments.Add(new BoneSegment { StartTime = splitTime, EndTime = duration, BoneName = _selectedEye.AnchorBone, Hidden = inherit, OpacityStart = baseOp, OpacityEnd = baseOp });
            CacheSegmentOffset(segments[0]);
            CacheSegmentOffset(segments[1]);
            SetOutput("Split at " + splitTime.ToString("F2") + "s.");
        }
        else
        {
            for (int i = 0; i < segments.Count; i++)
            {
                var seg = segments[i];
                if (splitTime > seg.StartTime + 0.01f && splitTime < seg.EndTime - 0.01f)
                {
                    float segDur = seg.EndTime - seg.StartTime;
                    float t = segDur > 0f ? (splitTime - seg.StartTime) / segDur : 0f;
                    float midOp = Mathf.Lerp(seg.OpacityStart, seg.OpacityEnd, t);
                    var newSeg = new BoneSegment
                    {
                        StartTime = splitTime, EndTime = seg.EndTime,
                        BoneName = seg.BoneName, Hidden = seg.Hidden,
                        OpacityStart = midOp, OpacityEnd = seg.OpacityEnd
                    };
                    seg.OpacityEnd = midOp;
                    seg.EndTime = splitTime;
                    segments.Insert(i + 1, newSeg);
                    CacheSegmentOffset(seg);
                    CacheSegmentOffset(newSeg);
                    SetOutput("Split segment " + (i + 1) + " at " + splitTime.ToString("F2") + "s.");
                    break;
                }
            }
        }
        _editingSegmentIndex = -1;
        RefreshSegmentUI();
    }

    private void StartEditingSegment(int index)
    {
        if (_selectedEye == null) return;
        if (!_selectedEye.BoneTimelines.TryGetValue(_currentAnimName, out var segs)) return;
        if (index < 0 || index >= segs.Count) return;
        _editingSegmentIndex = index;
        var seg = segs[index];
        float mid = (seg.StartTime + seg.EndTime) / 2f;
        float dur = GetCurrentAnimDuration();
        if (dur > 0f) { _isPlaying = false; _playPauseButton.Text = "Play"; SetAnimationPaused(_currentAnimName, mid / dur); }
        SetInfo("EDITING segment " + (index + 1) + ": drag the eye to set offset from '" + seg.BoneName + "'.");
        RefreshSegmentUI();
    }

    private void StopEditingSegment()
    {
        if (_selectedEye != null && _editingSegmentIndex >= 0
            && _selectedEye.BoneTimelines.TryGetValue(_currentAnimName, out var segs)
            && _editingSegmentIndex < segs.Count)
            CacheSegmentOffsetFromCurrentPose(segs[_editingSegmentIndex]);
        _editingSegmentIndex = -1;
        SetInfo(_placements.Count + " eye(s) placed.");
        RefreshSegmentUI();
    }

    private void RemoveSegment(int index)
    {
        if (_selectedEye == null) return;
        if (!_selectedEye.BoneTimelines.TryGetValue(_currentAnimName, out var segs)) return;
        if (index < 0 || index >= segs.Count) return;
        if (_editingSegmentIndex == index) _editingSegmentIndex = -1;
        else if (_editingSegmentIndex > index) _editingSegmentIndex--;
        if (segs.Count > 1)
        {
            if (index == 0) segs[1].StartTime = segs[0].StartTime;
            else if (index == segs.Count - 1) segs[index - 1].EndTime = segs[index].EndTime;
            else segs[index - 1].EndTime = segs[index].EndTime;
        }
        segs.RemoveAt(index);
        if (segs.Count <= 1)
        {
            _selectedEye.BoneTimelines.Remove(_currentAnimName);
            _editingSegmentIndex = -1;
            if (segs.Count == 1) SetOutput("Only one segment left — removed.");
        }
        RefreshSegmentUI();
    }

    private void ClearSegmentsForSelected()
    {
        if (_selectedEye == null) return;
        _selectedEye.BoneTimelines.Remove(_currentAnimName);
        _editingSegmentIndex = -1;
        RefreshSegmentUI();
        SetOutput("Cleared segments for " + _currentAnimName + ".");
    }

    private void CacheSegmentOffset(BoneSegment seg)
    {
        if (_selectedEye == null || _skeletonGodot == null || _spineNode == null) return;
        float dur = GetCurrentAnimDuration();
        if (dur <= 0f) return;
        float mid = (seg.StartTime + seg.EndTime) / 2f;
        var savedSlider = _frameSlider.Value;
        PoseSkeletonAt(_currentAnimName, mid);
        var bone = _skeletonGodot.Call("find_bone", seg.BoneName).AsGodotObject();
        if (bone != null)
        {
            var bp = SpineToPreview(new Vector2((float)bone.Call("get_world_x"), (float)bone.Call("get_world_y")));
            seg.SpineOffset = UnrotateByBone((_selectedEye.Position - bp) / _spineNode.Scale, bone);
            seg.HasSpineOffset = true;
        }
        else seg.HasSpineOffset = false;
        SetAnimationPaused(_currentAnimName, (float)(savedSlider / 100.0));
    }

    private void CacheSegmentOffsetFromCurrentPose(BoneSegment seg)
    {
        if (_selectedEye == null || _skeletonGodot == null || _spineNode == null) return;
        var bone = _skeletonGodot.Call("find_bone", seg.BoneName).AsGodotObject();
        if (bone != null)
        {
            var bp = SpineToPreview(new Vector2((float)bone.Call("get_world_x"), (float)bone.Call("get_world_y")));
            seg.SpineOffset = UnrotateByBone((_selectedEye.Position - bp) / _spineNode.Scale, bone);
            seg.HasSpineOffset = true;
        }
        else seg.HasSpineOffset = false;
    }

    private void RefreshSegmentUI()
    {
        foreach (var child in _segmentList.GetChildren())
            if (child is Node n && GodotObject.IsInstanceValid(n)) n.QueueFree();

        if (_selectedEye == null)
        {
            _segmentList.AddChild(MakeLabel("Select an eye first.", Vector2.Zero, new Vector2(600, 20), 11, EditorTheme.TextDim));
            return;
        }
        if (!_selectedEye.BoneTimelines.TryGetValue(_currentAnimName, out var segments) || segments.Count == 0)
        {
            string hint = "No segments — eye follows default bone ('" + _selectedEye.AnchorBone + "'). To switch: scrub, click 'Split here'.";
            var lbl = MakeLabel(hint, Vector2.Zero, new Vector2(600, 50), 11, EditorTheme.TextDim);
            lbl.AutowrapMode = TextServer.AutowrapMode.WordSmart;
            _segmentList.AddChild(lbl);
            return;
        }
        for (int i = 0; i < segments.Count; i++)
            BuildSegmentRow(segments, i);
    }

    private void BuildSegmentRow(List<BoneSegment> segments, int i)
    {
        var seg = segments[i];
        int idx = i;
        bool isEditing = i == _editingSegmentIndex;

        var row = new HBoxContainer();
        row.AddThemeConstantOverride("separation", 4);
        if (isEditing) row.AddChild(new ColorRect { Color = EditorTheme.SegmentActiveBg, CustomMinimumSize = new Vector2(4, 0) });

        var idxLbl = new Label { Text = (i + 1) + ".", CustomMinimumSize = new Vector2(20, 0) };
        ApplyFont(idxLbl, 12, isEditing ? EditorTheme.AccentWarm : EditorTheme.TextDim);
        row.AddChild(idxLbl);

        var timeLbl = new Label { Text = seg.StartTime.ToString("F2") + "s – " + seg.EndTime.ToString("F2") + "s", CustomMinimumSize = new Vector2(120, 0) };
        ApplyFont(timeLbl, 11, EditorTheme.TextNormal);
        row.AddChild(timeLbl);

        var boneEdit = new LineEdit
        {
            Text = seg.BoneName, CustomMinimumSize = new Vector2(100, 0),
            Editable = isEditing && !seg.Hidden
        };
        if (Font != null) boneEdit.AddThemeFontOverride("font", Font);
        boneEdit.AddThemeFontSizeOverride("font_size", 11);
        if (isEditing && !seg.Hidden) boneEdit.TextChanged += text => seg.BoneName = text;
        if (seg.Hidden) boneEdit.Modulate = new Color(1, 1, 1, 0.4f);
        row.AddChild(boneEdit);

        var visCheck = new CheckBox { Text = "Visible", ButtonPressed = !seg.Hidden, CustomMinimumSize = new Vector2(72, 0) };
        ApplyFont(visCheck, 10, seg.Hidden ? EditorTheme.TextDim : EditorTheme.AccentGreen);
        visCheck.Toggled += pressed => { seg.Hidden = !pressed; RefreshSegmentUI(); };
        row.AddChild(visCheck);

        if (!seg.Hidden)
        {
            if (isEditing)
            {
                var doneBtn = new Button { Text = "Done editing", CustomMinimumSize = new Vector2(95, 0) };
                if (Font != null) doneBtn.AddThemeFontOverride("font", Font);
                doneBtn.AddThemeFontSizeOverride("font_size", 11);
                doneBtn.Pressed += StopEditingSegment;
                row.AddChild(doneBtn);
            }
            else
            {
                var editBtn = new Button { Text = "Edit", CustomMinimumSize = new Vector2(45, 0) };
                if (Font != null) editBtn.AddThemeFontOverride("font", Font);
                editBtn.AddThemeFontSizeOverride("font_size", 11);
                editBtn.Pressed += () => StartEditingSegment(idx);
                row.AddChild(editBtn);
            }
        }

        var removeBtn = new Button { Text = "X", CustomMinimumSize = new Vector2(26, 0) };
        if (Font != null) removeBtn.AddThemeFontOverride("font", Font);
        removeBtn.AddThemeFontSizeOverride("font_size", 11);
        removeBtn.Pressed += () => RemoveSegment(idx);
        row.AddChild(removeBtn);

        if (!seg.Hidden)
        {
            var offLbl = new Label { Text = seg.HasSpineOffset ? "offset set" : "no offset", CustomMinimumSize = new Vector2(70, 0) };
            ApplyFont(offLbl, 10, seg.HasSpineOffset ? EditorTheme.AccentGreen : EditorTheme.TextDim);
            row.AddChild(offLbl);

            var opStartLbl = new Label { Text = "α:", CustomMinimumSize = new Vector2(16, 0) };
            ApplyFont(opStartLbl, 10, EditorTheme.TextDim);
            row.AddChild(opStartLbl);

            var opStartSpin = new SpinBox
            {
                MinValue = 0.0, MaxValue = 1.0, Step = 0.05, Value = seg.OpacityStart,
                CustomMinimumSize = new Vector2(70, 0), Editable = isEditing
            };
            if (Font != null) opStartSpin.AddThemeFontOverride("font", Font);
            opStartSpin.AddThemeFontSizeOverride("font_size", 10);
            if (isEditing) opStartSpin.ValueChanged += v => seg.OpacityStart = (float)v;
            row.AddChild(opStartSpin);

            var arrowLbl = new Label { Text = "→", CustomMinimumSize = new Vector2(14, 0) };
            ApplyFont(arrowLbl, 10, EditorTheme.TextDim);
            row.AddChild(arrowLbl);

            var opEndSpin = new SpinBox
            {
                MinValue = 0.0, MaxValue = 1.0, Step = 0.05, Value = seg.OpacityEnd,
                CustomMinimumSize = new Vector2(70, 0), Editable = isEditing
            };
            if (Font != null) opEndSpin.AddThemeFontOverride("font", Font);
            opEndSpin.AddThemeFontSizeOverride("font_size", 10);
            if (isEditing) opEndSpin.ValueChanged += v => seg.OpacityEnd = (float)v;
            row.AddChild(opEndSpin);
        }

        _segmentList.AddChild(row);
    }
}