using System.Text;
using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace GooglyEyes;

/// <summary>
/// Editor tab for placing googly eyes on creature sprites (monsters and characters).
/// Handles Spine skeleton animations, bone anchoring, per-animation overrides,
/// timed bone segments, and export.
/// </summary>
public class MonsterEditorTab : EditorTab
{
    public override string TabName => "Creatures";
    public override string TabTooltip => "Place googly eyes on monster and character sprites.";

    // ── Sidebar ──
    private ScrollContainer _sidebarScroll;
    private VBoxContainer _monsterList;
    private Button _selectedButton;
    private bool _listBuilt;
    private LineEdit _searchInput;
    private OptionButton _filterDropdown;
    private enum FilterMode { All, Configured, Unconfigured }
    private FilterMode _filterMode = FilterMode.All;

    // ── Spine / visuals ──
    private NCreatureVisuals _currentVisuals;
    private MegaSprite _animController;
    private MegaAnimationState _animState;
    private MegaTrackEntry _cachedTrackEntry;
    private GodotObject _skeletonGodot;
    private Node2D _spineNode;
    private string _currentMonsterId;

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
    private string _currentAnimName = "idle_loop";
    private readonly List<string> _availableAnims = new();
    private Label _animPanelHint;

    // ── Secondary track ──
    private OptionButton _track1Dropdown;
    private readonly List<string> _availableTrack1Anims = new();
    private string _currentTrack1Anim = "";
    private string _appliedTrack1Anim = "";

    // ── Bone anchoring ──
    private string _selectedAnchorBone = "head";
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

    // ── Animation override panel ──
    private CheckBox _overrideCheckbox;
    private Label _overrideAnimLabel;
    private Label _overrideInfoLabel;
    private LineEdit _overrideBoneInput;
    private CheckBox _overrideVisibleCheckbox;
    private SpinBox _overrideOpacityInput;
    private Label _overrideOpacityLabel;
    private Control _overrideFieldsRow;
    private Label _overrideHintLabel;

    // ── Panel containers (toggled as a unit on tab switch) ──
    private Control _animPanelContainer;
    private Control _bonePanelContainer;

    // ═══════════════════════════════════════════════
    //  INNER TYPES
    // ═══════════════════════════════════════════════

    private struct BoneInfo
    {
        public string Name;
        public Vector2 SpineWorldPos;
    }

    private class BoneSegment
    {
        public float StartTime;
        public float EndTime;
        public string BoneName = "head";
        public Vector2 SpineOffset;
        public bool HasSpineOffset;
        public bool Hidden;
        public float OpacityStart = 1f;
        public float OpacityEnd = 1f;
    }

    /// <summary>
    /// Per-animation override for a single eye. Stores how this eye should
    /// differ from its defaults during a specific animation. Null fields
    /// mean "use the eye's default."
    ///
    /// Time segments live inside the override for the rare case where a
    /// bone switch is needed mid-animation.
    /// </summary>
    private class AnimationOverride
    {
        public string BoneName;           // null → use eye default AnchorBone
        public Vector2 SpineOffset;
        public bool HasSpineOffset;
        public bool? Hidden;              // null → use eye default HiddenByDefault
        public float? Opacity;            // null → use eye default Opacity
        public List<BoneSegment> Segments = new();
    }

    private class EyePlacement
    {
        public Vector2 Position;
        public float Scale = 1f;
        public float Opacity = 1f;
        public string AnchorBone = "head";
        public Sprite2D EyeSprite;
        public Sprite2D IrisSprite;
        public Node2D Container;
        public Vector2 SpineOffset;
        public bool HasSpineOffset;
        public bool HiddenByDefault;
        public Dictionary<string, AnimationOverride> AnimOverrides = new();
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

        // ── Monsters ──
        AddSidebarHeader("Monsters");
        foreach (MonsterModel monster in ModelDb.AllAbstractModelSubtypes
                     .Where(t => t.IsSubclassOf(typeof(MonsterModel)))
                     .Select(t => ModelDb.GetByIdOrNull<MonsterModel>(ModelDb.GetId(t)))
                     .Where(m => m != null)
                     .OrderBy(m => m.Id.Entry))
        {
            var id = monster.Id.Entry;
            var monsterRef = monster;
            AddCreatureButton(id, () => monsterRef.CreateVisuals(), v => monsterRef.SetupSkins(v.SpineBody, v.SpineBody.GetSkeleton()));
        }

        // ── Characters ──
        AddSidebarHeader("Characters");
        foreach (CharacterModel character in ModelDb.AllCharacters.OrderBy(c => c.Id.Entry))
        {
            var id = character.Id.Entry;
            var charRef = character;
            AddCreatureButton(id, () => charRef.CreateVisuals());
        }
    }

    private void AddCreatureButton(string id, Func<NCreatureVisuals> createVisuals, Action<NCreatureVisuals> postSetup = null)
    {
        bool hasConfig = CreatureGooglyEyesRegistry.Configs.ContainsKey(id);
        var button = new Button
        {
            Text = id, Flat = true,
            Alignment = HorizontalAlignment.Left,
            CustomMinimumSize = new Vector2(0, 32),
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        button.SetMeta("creature_id", id);
        ApplyFont(button, 14, hasConfig ? EditorTheme.AccentGreen : EditorTheme.TextNormal);
        button.AddThemeColorOverride("font_hover_color", EditorTheme.TextBright);
        button.Pressed += () => SelectCreature(id, createVisuals, postSetup);
        _monsterList.AddChild(button);
    }

    private void AddSidebarHeader(string text)
    {
        var spacer = new Control { CustomMinimumSize = new Vector2(0, 6) };
        spacer.SetMeta("section_header", text);
        _monsterList.AddChild(spacer);

        var header = new Label
        {
            Text = text,
            CustomMinimumSize = new Vector2(0, 24),
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        header.SetMeta("section_header", text);
        ApplyFont(header, 12, EditorTheme.Accent);
        _monsterList.AddChild(header);

        var divider = new ColorRect
        {
            Color = EditorTheme.Divider,
            CustomMinimumSize = new Vector2(0, 1),
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        divider.SetMeta("section_header", text);
        _monsterList.AddChild(divider);
    }

    private void RefreshSidebarFilter()
    {
        string search = _searchInput.Text?.Trim() ?? "";
        bool hasSearch = search.Length > 0;

        string currentSection = null;
        var sectionHasVisible = new Dictionary<string, bool>();

        foreach (var child in _monsterList.GetChildren())
        {
            if (child is not Control ctrl) continue;
            if (ctrl.HasMeta("section_header"))
            {
                currentSection = ctrl.GetMeta("section_header").AsString();
                if (!sectionHasVisible.ContainsKey(currentSection))
                    sectionHasVisible[currentSection] = false;
                continue;
            }
            if (child is not Button btn || !btn.HasMeta("creature_id")) continue;

            string id = btn.GetMeta("creature_id").AsString();
            bool hasConfig = CreatureGooglyEyesRegistry.Configs.ContainsKey(id);
            bool matchesSearch = !hasSearch || id.Contains(search, StringComparison.OrdinalIgnoreCase);
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

        foreach (var child in _monsterList.GetChildren())
        {
            if (child is not Control ctrl) continue;
            if (!ctrl.HasMeta("section_header")) continue;
            string section = ctrl.GetMeta("section_header").AsString();
            ctrl.Visible = sectionHasVisible.TryGetValue(section, out bool any) && any;
        }
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
        if (_currentVisuals != null && GodotObject.IsInstanceValid(_currentVisuals))
        {
            _currentVisuals.QueueFree();
            _currentVisuals = null;
        }
        _currentMonsterId = null;
        _selectedButton = null;
    }

    public override void Process(double delta)
    {
        if (_showingBones && _boneData.Count > 0 && _skeletonGodot != null)
            UpdateBoneMarkerPositions();

        if (_isPlaying && _cachedTrackEntry != null && GodotObject.IsInstanceValid(_spineNode))
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
        if (_currentVisuals != null && GodotObject.IsInstanceValid(_currentVisuals))
        {
            _currentVisuals.QueueFree();
            _currentVisuals = null;
        }
    }

    // ═══════════════════════════════════════════════
    //  SELECTION STATE (read by screen)
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
            if (_selectedEye == null)
                return "Click an eye to select it. Adjust scale or duplicate.";
            int overrideCount = _selectedEye.AnimOverrides.Count;
            if (overrideCount > 0)
                return "This eye has overrides on " + overrideCount + " animation(s).";
            return "Drag to reposition. Use Scale to resize.";
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

        _monsterList = new VBoxContainer { CustomMinimumSize = new Vector2(EditorTheme.SidebarWidth - 20, 0) };
        _monsterList.AddThemeConstantOverride("separation", 2);
        _sidebarScroll.AddChild(_monsterList);
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
            Visible = false
        };
        Screen.AddChild(_animPanelContainer);

        _animPanelContainer.AddChild(new ColorRect { Color = EditorTheme.PanelBg, Position = Vector2.Zero, Size = new Vector2(pw, 82) });
        _animPanelContainer.AddChild(MakeLabel("Animation", new Vector2(10, 4), new Vector2(100, 16), 11, EditorTheme.Accent));

        _animPanelHint = MakeLabel("Scrub or play to preview. Eyes follow their bones in real time.",
            new Vector2(120, 4), new Vector2(pw - 140, 16), 10, EditorTheme.TextDim);
        _animPanelContainer.AddChild(_animPanelHint);

        // Row 1: track 0
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

        // Row 2: track 1
        float row2 = row1 + 30;
        _animPanelContainer.AddChild(MakeLabel("Layer:", new Vector2(10, row2 + 3), new Vector2(45, 24), 11, EditorTheme.TextDim));

        _track1Dropdown = new OptionButton { Position = new Vector2(55, row2), Size = new Vector2(180, 26) };
        if (Font != null) _track1Dropdown.AddThemeFontOverride("font", Font);
        _track1Dropdown.AddThemeFontSizeOverride("font_size", 11);
        _track1Dropdown.TooltipText = "Optional secondary animation on track 1.";
        _track1Dropdown.ItemSelected += OnTrack1Selected;
        _animPanelContainer.AddChild(_track1Dropdown);

        _animPanelContainer.AddChild(MakeLabel("(optional — for enemies with layered animations like charge states)",
            new Vector2(245, row2 + 3), new Vector2(pw - 260, 20), 10, EditorTheme.TextDim));
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
            TooltipText = "Eye is invisible unless an animation override makes it appear."
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
            Editable = true, TooltipText = "Eye opacity. 1.0 = fully visible, 0.0 = invisible."
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
            Visible = false
        };
        Screen.AddChild(_bonePanelContainer);

        _bonePanelContainer.AddChild(new ColorRect
        {
            Color = EditorTheme.PanelBg, Position = Vector2.Zero, Size = new Vector2(pw, ph)
        });

        // ── Row 0: Title ──
        _bonePanelContainer.AddChild(MakeLabel("Bone Anchoring",
            new Vector2(10, 4), new Vector2(120, 16), 11, EditorTheme.Accent));
        _anchorPanelHint = MakeLabel("Which bone this eye follows. Override per-animation below.",
            new Vector2(140, 4), new Vector2(pw - 160, 16), 10, EditorTheme.TextDim);
        _bonePanelContainer.AddChild(_anchorPanelHint);

        // ── Row 1: Default bone ──
        float row1 = 24;
        _bonePanelContainer.AddChild(MakeLabel("Default bone:",
            new Vector2(10, row1 + 3), new Vector2(100, 24), 12, EditorTheme.TextNormal));

        _anchorBoneInput = new LineEdit
        {
            Text = "head",
            Position = new Vector2(112, row1 - 2),
            Size = new Vector2(130, 30),
            TooltipText = "The bone this eye follows when no animation override applies."
        };
        if (Font != null) _anchorBoneInput.AddThemeFontOverride("font", Font);
        _anchorBoneInput.AddThemeFontSizeOverride("font_size", 12);
        _anchorBoneInput.TextChanged += text =>
        {
            _selectedAnchorBone = text;
            if (_selectedEye != null) _selectedEye.AnchorBone = text;
        };
        _bonePanelContainer.AddChild(_anchorBoneInput);

        _showBonesButton = MakeButton("Show Bones", new Vector2(255, row1 - 2), new Vector2(105, 30), 11);
        _showBonesButton.TooltipText = "Show bone markers on the preview.";
        _showBonesButton.Pressed += ToggleBoneMarkers;
        _bonePanelContainer.AddChild(_showBonesButton);

        _showAllBoneNamesCheckbox = new CheckBox
        {
            Text = "All names",
            Position = new Vector2(370, row1 - 2),
            Size = new Vector2(100, 30),
            TooltipText = "Label every bone, not just head/face/eye."
        };
        ApplyFont(_showAllBoneNamesCheckbox, 11, EditorTheme.TextNormal);
        _showAllBoneNamesCheckbox.Toggled += _ => { if (_showingBones) RebuildBoneMarkerNodes(); };
        _bonePanelContainer.AddChild(_showAllBoneNamesCheckbox);

        _anchorPosLabel = MakeLabel("",
            new Vector2(480, row1 + 3), new Vector2(300, 24), 11, EditorTheme.Accent);
        _bonePanelContainer.AddChild(_anchorPosLabel);

        // ── Divider ──
        float divY = row1 + 32;
        _bonePanelContainer.AddChild(new ColorRect
        {
            Color = EditorTheme.Divider, Position = new Vector2(10, divY), Size = new Vector2(pw - 20, 1)
        });

        // ── Row 2: Animation override header ──
        float ovY = divY + 6;
        _overrideAnimLabel = MakeLabel("Animation Override",
            new Vector2(10, ovY), new Vector2(250, 16), 11, EditorTheme.AccentWarm);
        _bonePanelContainer.AddChild(_overrideAnimLabel);

        _overrideCheckbox = new CheckBox
        {
            Text = "Override for this animation",
            Position = new Vector2(270, ovY - 3),
            Size = new Vector2(220, 22),
            TooltipText = "When checked, this eye uses different settings during this animation."
        };
        ApplyFont(_overrideCheckbox, 11, EditorTheme.TextNormal);
        _overrideCheckbox.Toggled += OnOverrideCheckboxToggled;
        _bonePanelContainer.AddChild(_overrideCheckbox);

        _overrideInfoLabel = MakeLabel("",
            new Vector2(500, ovY), new Vector2(pw - 520, 16), 10, EditorTheme.TextDim);
        _bonePanelContainer.AddChild(_overrideInfoLabel);

        // ── Row 3: Override fields (bone, visible, opacity) ──
        float fieldsY = ovY + 22;
        _overrideFieldsRow = new Control
        {
            Position = new Vector2(0, fieldsY),
            Size = new Vector2(pw, 30),
            Visible = false
        };
        _bonePanelContainer.AddChild(_overrideFieldsRow);

        _overrideFieldsRow.AddChild(MakeLabel("Bone:",
            new Vector2(20, 5), new Vector2(40, 24), 12, EditorTheme.TextNormal));

        _overrideBoneInput = new LineEdit
        {
            Text = "head",
            Position = new Vector2(62, 0),
            Size = new Vector2(120, 28),
            TooltipText = "Bone to follow during this animation. Leave blank to use the eye's default."
        };
        if (Font != null) _overrideBoneInput.AddThemeFontOverride("font", Font);
        _overrideBoneInput.AddThemeFontSizeOverride("font_size", 12);
        _overrideBoneInput.TextChanged += OnOverrideBoneChanged;
        _overrideFieldsRow.AddChild(_overrideBoneInput);

        _overrideVisibleCheckbox = new CheckBox
        {
            Text = "Visible",
            Position = new Vector2(195, 0),
            Size = new Vector2(80, 28),
            ButtonPressed = true,
            TooltipText = "Whether this eye is visible during this animation."
        };
        ApplyFont(_overrideVisibleCheckbox, 11, EditorTheme.TextNormal);
        _overrideVisibleCheckbox.Toggled += OnOverrideVisibleToggled;
        _overrideFieldsRow.AddChild(_overrideVisibleCheckbox);

        _overrideOpacityLabel = MakeLabel("Opacity:",
            new Vector2(285, 5), new Vector2(55, 24), 11, EditorTheme.TextNormal);
        _overrideFieldsRow.AddChild(_overrideOpacityLabel);

        _overrideOpacityInput = new SpinBox
        {
            MinValue = 0.0, MaxValue = 1.0, Step = 0.05, Value = 1.0,
            Position = new Vector2(340, 0),
            Size = new Vector2(80, 28),
            TooltipText = "Eye opacity during this animation."
        };
        if (Font != null) _overrideOpacityInput.AddThemeFontOverride("font", Font);
        _overrideOpacityInput.AddThemeFontSizeOverride("font_size", 11);
        _overrideOpacityInput.ValueChanged += OnOverrideOpacityChanged;
        _overrideFieldsRow.AddChild(_overrideOpacityInput);

        _overrideFieldsRow.AddChild(MakeLabel("(drag eye to reposition for this animation)",
            new Vector2(430, 5), new Vector2(pw - 450, 24), 10, EditorTheme.TextDim));

        _overrideHintLabel = MakeLabel("Select an eye and a non-idle animation to configure overrides.",
            new Vector2(20, fieldsY + 2), new Vector2(pw - 40, 20), 10, EditorTheme.TextDim);
        _bonePanelContainer.AddChild(_overrideHintLabel);

        // ── Divider 2 ──
        float div2Y = fieldsY + 32;
        _bonePanelContainer.AddChild(new ColorRect
        {
            Color = EditorTheme.Divider, Position = new Vector2(10, div2Y), Size = new Vector2(pw - 20, 1)
        });

        // ── Segment header ──
        float segHdrY = div2Y + 4;
        _bonePanelContainer.AddChild(MakeLabel("Time Segments (Advanced)",
            new Vector2(10, segHdrY), new Vector2(200, 16), 11, EditorTheme.TextDim));
        _bonePanelContainer.AddChild(MakeLabel(
            "Split the timeline to switch bones mid-animation. Most eyes don't need this.",
            new Vector2(220, segHdrY), new Vector2(pw - 240, 16), 10, EditorTheme.TextDim));

        // Segment buttons
        float btnY = segHdrY + 18;
        _splitHereButton = MakeButton("Split here", new Vector2(10, btnY), new Vector2(100, 24), 11);
        _splitHereButton.TooltipText = "Split the timeline at the current scrub position.";
        _splitHereButton.Pressed += SplitAtScrubPosition;
        _bonePanelContainer.AddChild(_splitHereButton);

        _clearSegmentsButton = MakeButton("Clear segments", new Vector2(120, btnY), new Vector2(115, 24), 11);
        _clearSegmentsButton.TooltipText = "Remove all time segments for this animation (keeps the override).";
        _clearSegmentsButton.Pressed += ClearSegmentsForSelected;
        _bonePanelContainer.AddChild(_clearSegmentsButton);

        _activeSegmentLabel = MakeLabel("",
            new Vector2(250, btnY + 3), new Vector2(pw - 270, 20), 11, EditorTheme.TextDim);
        _bonePanelContainer.AddChild(_activeSegmentLabel);

        // Scrollable segment list
        float listY = btnY + 27;
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
    //  CREATURE SELECTION
    // ═══════════════════════════════════════════════

    private void SelectCreature(string creatureId, Func<NCreatureVisuals> createVisuals, Action<NCreatureVisuals> postSetup = null)
    {
        ClearAll();
        ClearBoneMarkers();
        HighlightButton(creatureId);
        _showingBones = false;
        _showBonesButton.Text = "Show Bones";
        _skeletonGodot = null;
        _spineNode = null;
        _animState = null;
        _cachedTrackEntry = null;
        _currentMonsterId = creatureId;
        _isPlaying = false;
        _playPauseButton.Text = "Play";
        _editingSegmentIndex = -1;
        _currentTrack1Anim = "";
        _appliedTrack1Anim = "";
        Screen.ResetZoomPan();

        if (_currentVisuals != null && GodotObject.IsInstanceValid(_currentVisuals))
        {
            _currentVisuals.QueueFree();
            _currentVisuals = null;
        }

        _currentVisuals = createVisuals();
        PreviewRoot.AddChild(_currentVisuals);
        _currentVisuals.Position = new Vector2(
            PreviewArea.Size.X / 2,
            PreviewArea.Size.Y / 2 + _currentVisuals.Bounds.Size.Y * 0.25f
        );

        try { postSetup?.Invoke(_currentVisuals); }
        catch (Exception e) { GD.PrintErr("[GooglyEyes] SetupSkins error: " + e); }

        _availableAnims.Clear();
        _animDropdown.Clear();
        if (_currentVisuals.HasSpineAnimation)
        {
            _animController = _currentVisuals.SpineBody;
            _animState = _animController.GetAnimationState();
            var skeleton = _animController.GetSkeleton();
            if (skeleton != null)
            {
                _skeletonGodot = skeleton.BoundObject as GodotObject;
                _spineNode = _animController.BoundObject as Node2D;
            }
            PopulateAnimationList();
            SetAnimationPaused("idle_loop", 0f);
        }

        SetInfo(creatureId + " loaded — click the preview to place eyes!");
        AdvanceWorkflow(WorkflowStep.PlaceEyes);

        if (LoadPresetsForMonster(creatureId))
        {
            SetInfo(creatureId + " loaded with " + _placements.Count + " preset eye(s) from registry.");
            if (_placements.Count > 0) AdvanceWorkflow(WorkflowStep.AdjustEyes);
        }

        RefreshOverridePanel();
        RefreshSegmentUI();
    }

    private void HighlightButton(string monsterId)
    {
        if (_selectedButton != null && GodotObject.IsInstanceValid(_selectedButton))
        {
            bool prev = CreatureGooglyEyesRegistry.Configs.ContainsKey(_selectedButton.Text);
            ApplyFont(_selectedButton, 14, prev ? EditorTheme.AccentGreen : EditorTheme.TextNormal);
        }
        foreach (var child in _monsterList.GetChildren())
        {
            if (child is Button btn && btn.Text == monsterId)
            {
                _selectedButton = btn;
                ApplyFont(btn, 14, EditorTheme.SelectedEntryColor);
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
        if (_currentVisuals == null) return false;

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
                if (_isDragging && _dragging != null)
                {
                    if (_editingSegmentIndex >= 0
                        && _dragging == _selectedEye
                        && _selectedEye.AnimOverrides.TryGetValue(_currentAnimName, out var editOv)
                        && _editingSegmentIndex < editOv.Segments.Count)
                    {
                        CacheSegmentOffsetFromCurrentPose(editOv.Segments[_editingSegmentIndex]);
                    }
                    else if (_dragging.AnimOverrides.TryGetValue(_currentAnimName, out var dragOv))
                    {
                        CacheOverrideOffset(dragOv);
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
        if (_placements.Count == 0 || string.IsNullOrEmpty(_currentMonsterId))
        {
            SetOutput("Nothing to export.");
            return;
        }

        AdvanceWorkflow(WorkflowStep.Export);
        EnsureSkeletonRefs();
        float spineScale = _spineNode?.Scale.X ?? 1f;

        var sb = new StringBuilder();
        sb.AppendLine("// Googly Eyes config for: " + _currentMonsterId);
        sb.AppendLine("{ \"" + _currentMonsterId + "\", new EyeConfig[] {");

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

            if (p.AnimOverrides.Count > 0)
            {
                sb.AppendLine(",");
                sb.Append("        BoneSegments = new Dictionary<string, BoneSegment[]> {");

                foreach (var ovKvp in p.AnimOverrides)
                {
                    var animName = ovKvp.Key;
                    var ov = ovKvp.Value;

                    sb.AppendLine();
                    sb.Append("            { \"" + animName + "\", new BoneSegment[] {");

                    if (ov.Segments.Count > 0)
                    {
                        foreach (var seg in ov.Segments)
                        {
                            sb.AppendLine();
                            sb.Append("                new BoneSegment { ");
                            sb.Append("StartTime = " + seg.StartTime.ToString("F2") + "f, ");
                            sb.Append("EndTime = " + seg.EndTime.ToString("F2") + "f, ");
                            EmitSegmentFields(sb, seg);
                            sb.Append(" },");
                        }
                    }
                    else
                    {
                        // Whole-animation override → single segment spanning full duration
                        float duration = GetAnimDuration(animName);
                        sb.AppendLine();
                        sb.Append("                new BoneSegment { ");
                        sb.Append("StartTime = 0.00f, ");
                        sb.Append("EndTime = " + duration.ToString("F2") + "f, ");

                        bool hidden = ov.Hidden ?? p.HiddenByDefault;
                        if (hidden)
                        {
                            sb.Append("Hidden = true");
                        }
                        else
                        {
                            string bone = ov.BoneName ?? p.AnchorBone;
                            var segOff = ov.HasSpineOffset ? ov.SpineOffset : Vector2.Zero;
                            sb.Append("BoneName = \"" + bone + "\", ");
                            sb.Append("Offset = new Vector2(" + segOff.X.ToString("F1") + "f, " + segOff.Y.ToString("F1") + "f)");
                            float opacity = ov.Opacity ?? p.Opacity;
                            if (opacity < 0.99f)
                            {
                                sb.Append(", OpacityStart = " + opacity.ToString("F2") + "f");
                                sb.Append(", OpacityEnd = " + opacity.ToString("F2") + "f");
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

        sb.AppendLine("}},");
        GD.Print("[GooglyEyes] === EXPORT ===");
        GD.Print(sb.ToString());
        GD.Print("[GooglyEyes] === END EXPORT ===");

        int totalOverrides = _placements.Sum(p => p.AnimOverrides.Count);
        int totalSegs = _placements.Sum(p => p.AnimOverrides.Values.Sum(ov => ov.Segments.Count));
        SetOutput("Exported " + _placements.Count + " eye(s), "
            + totalOverrides + " override(s), "
            + totalSegs + " segment(s) — check console!");
    }

    /// <summary>Helper: emits the fields for a single BoneSegment in the export.</summary>
    private void EmitSegmentFields(StringBuilder sb, BoneSegment seg)
    {
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
    }

    /// <summary>
    /// Looks up the duration of a named animation from the skeleton data.
    /// Used at export time for whole-animation overrides.
    /// </summary>
    private float GetAnimDuration(string animName)
    {
        if (_animController == null) return 1f;
        var skeleton = _animController.GetSkeleton();
        var data = skeleton?.GetData();
        var bound = data?.BoundObject as GodotObject;
        if (bound == null) return 1f;

        var anims = bound.Call("get_animations").AsGodotArray();
        foreach (var animVariant in anims)
        {
            var anim = animVariant.AsGodotObject();
            if (anim.Call("get_name").AsString() == animName)
                return (float)anim.Call("get_duration");
        }
        return 1f;
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

        foreach (var kvp in _selectedEye.AnimOverrides)
        {
            var srcOv = kvp.Value;
            var newOv = new AnimationOverride
            {
                BoneName = srcOv.BoneName,
                SpineOffset = srcOv.SpineOffset,
                HasSpineOffset = srcOv.HasSpineOffset,
                Hidden = srcOv.Hidden,
                Opacity = srcOv.Opacity
            };
            foreach (var seg in srcOv.Segments)
            {
                newOv.Segments.Add(new BoneSegment
                {
                    StartTime = seg.StartTime, EndTime = seg.EndTime,
                    BoneName = seg.BoneName, SpineOffset = seg.SpineOffset,
                    HasSpineOffset = seg.HasSpineOffset, Hidden = seg.Hidden,
                    OpacityStart = seg.OpacityStart, OpacityEnd = seg.OpacityEnd
                });
            }
            n.AnimOverrides[kvp.Key] = newOv;
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
        RefreshOverridePanel();
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
            AnchorBone = _anchorBoneInput.Text,
            EyeSprite = container.GetChild<Sprite2D>(0),
            IrisSprite = container.GetChild<Sprite2D>(1),
            Container = container
        };
        _placements.Add(placement);
        CacheSpineOffset(placement);
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
        RefreshOverridePanel();
        RefreshSegmentUI();
    }

    // ═══════════════════════════════════════════════
    //  PRESET LOADING
    // ═══════════════════════════════════════════════

    private bool LoadPresetsForMonster(string monsterId)
    {
        if (!CreatureGooglyEyesRegistry.Configs.TryGetValue(monsterId, out var configs)) return false;
        if (configs.Length == 0 || _skeletonGodot == null || _spineNode == null) return false;

        SetAnimationPaused("idle_loop", 0f);
        float spineScale = _spineNode.Scale.X;

        foreach (var config in configs)
        {
            var anchorBone = _skeletonGodot.Call("find_bone", config.AnchorBone).AsGodotObject();
            if (anchorBone == null)
            {
                GD.PrintErr("[GooglyEyes] Preset load: bone '" + config.AnchorBone + "' not found, skipping.");
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

            if (config.HiddenByDefault)
                container.Modulate = new Color(1, 1, 1, 0.25f);
            else if (config.Opacity < 0.99f)
                container.Modulate = new Color(1, 1, 1, config.Opacity);

            // ── Convert old BoneSegments → AnimOverrides ──
            if (config.BoneSegments != null)
            {
                foreach (var kvp in config.BoneSegments)
                {
                    if (kvp.Value == null || kvp.Value.Length == 0) continue;

                    var ov = new AnimationOverride();

                    // Check if this is a single full-span segment (whole-animation override)
                    float animDur = GetAnimDuration(kvp.Key);
                    bool isSingleFullSpan = kvp.Value.Length == 1
                        && kvp.Value[0].StartTime <= 0.01f
                        && kvp.Value[0].EndTime >= animDur - 0.1f;

                    if (isSingleFullSpan)
                    {
                        // Convert to a clean override with no time segments
                        var src = kvp.Value[0];
                        ov.Hidden = src.Hidden;
                        if (!src.Hidden)
                        {
                            ov.BoneName = src.BoneName ?? config.AnchorBone;
                            ov.SpineOffset = src.Offset;
                            ov.HasSpineOffset = true;
                            ov.Opacity = (src.OpacityStart + src.OpacityEnd) / 2f;
                        }
                    }
                    else
                    {
                        // Multiple segments → keep them, infer override from first
                        var first = kvp.Value[0];
                        ov.BoneName = first.BoneName ?? config.AnchorBone;
                        ov.Hidden = first.Hidden;

                        foreach (var src in kvp.Value)
                        {
                            var seg = new BoneSegment
                            {
                                StartTime = src.StartTime, EndTime = src.EndTime,
                                BoneName = src.BoneName ?? config.AnchorBone,
                                Hidden = src.Hidden,
                                OpacityStart = src.OpacityStart,
                                OpacityEnd = src.OpacityEnd
                            };
                            if (!src.Hidden) { seg.SpineOffset = src.Offset; seg.HasSpineOffset = true; }
                            ov.Segments.Add(seg);
                        }
                    }

                    placement.AnimOverrides[kvp.Key] = ov;
                }
            }

            _placements.Add(placement);
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

        if (bound != null)
        {
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
        }

        if (_availableAnims.Count > 0)
        {
            int bestIdx = _availableAnims.IndexOf("idle_loop");
            if (bestIdx < 0) bestIdx = _availableAnims.FindIndex(n => n.Equals("idle", StringComparison.OrdinalIgnoreCase));
            if (bestIdx < 0) bestIdx = _availableAnims.FindIndex(n => n.Contains("idle", StringComparison.OrdinalIgnoreCase));
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
        RefreshOverridePanel();
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
            try { _animState.AddEmptyAnimation(1); } catch { /* ignored */ }
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
        catch (NullReferenceException)
        {
            // Track 1 entry had a stale native binding — harmless, ignore.
        }
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
    //  RESOLVE METHODS
    // ═══════════════════════════════════════════════

    private string ResolveActiveBone(EyePlacement p, string anim, float time)
    {
        if (!p.AnimOverrides.TryGetValue(anim, out var ov))
            return p.AnchorBone;

        if (ov.Segments.Count > 0)
        {
            foreach (var s in ov.Segments)
                if (time >= s.StartTime && time < s.EndTime)
                    return s.BoneName;
            return ov.Segments[^1].BoneName;
        }

        return ov.BoneName ?? p.AnchorBone;
    }

    private bool ResolveHidden(EyePlacement p, string anim, float time)
    {
        if (!p.AnimOverrides.TryGetValue(anim, out var ov))
            return p.HiddenByDefault;

        if (ov.Segments.Count > 0)
        {
            foreach (var s in ov.Segments)
                if (time >= s.StartTime && time < s.EndTime)
                    return s.Hidden;
            return ov.Segments[^1].Hidden;
        }

        return ov.Hidden ?? p.HiddenByDefault;
    }

    private Vector2 ResolveSpineOffset(EyePlacement p, string anim, float time)
    {
        if (!p.AnimOverrides.TryGetValue(anim, out var ov))
            return p.HasSpineOffset ? p.SpineOffset : Vector2.Zero;

        if (ov.Segments.Count > 0)
        {
            foreach (var s in ov.Segments)
            {
                if (time >= s.StartTime && time < s.EndTime)
                    return s.HasSpineOffset ? s.SpineOffset
                         : ov.HasSpineOffset ? ov.SpineOffset
                         : p.HasSpineOffset  ? p.SpineOffset
                         : Vector2.Zero;
            }
            var last = ov.Segments[^1];
            return last.HasSpineOffset ? last.SpineOffset
                 : ov.HasSpineOffset   ? ov.SpineOffset
                 : p.HasSpineOffset    ? p.SpineOffset
                 : Vector2.Zero;
        }

        return ov.HasSpineOffset ? ov.SpineOffset
             : p.HasSpineOffset  ? p.SpineOffset
             : Vector2.Zero;
    }

    private float ResolveOpacity(EyePlacement p, string anim, float time)
    {
        if (!p.AnimOverrides.TryGetValue(anim, out var ov))
            return p.Opacity;

        if (ov.Segments.Count > 0)
        {
            foreach (var s in ov.Segments)
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
            var last = ov.Segments[^1];
            return last.Hidden ? 0f : last.OpacityEnd;
        }

        return ov.Opacity ?? p.Opacity;
    }

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
                p.Container.Modulate = new Color(1, 1, 1, 0.15f);
            else if (p.HiddenByDefault)
                p.Container.Modulate = new Color(1, 1, 1, 0.7f);
            else
                p.Container.Modulate = new Color(1, 1, 1, ResolveOpacity(p, _currentAnimName, currentTime));

            string activeBone = ResolveActiveBone(p, _currentAnimName, currentTime);
            var bone = _skeletonGodot.Call("find_bone", activeBone).AsGodotObject();
            if (bone == null) continue;

            var bonePos = new Vector2((float)bone.Call("get_world_x"), (float)bone.Call("get_world_y"));
            var spineOffset = ResolveSpineOffset(p, _currentAnimName, currentTime);
            var rotated = RotateByBone(spineOffset, bone);
            var previewPos = SpineToPreview(bonePos) + rotated * _spineNode.Scale;

            p.Container.Position = previewPos;
            p.Position = previewPos;
        }
    }

    // ═══════════════════════════════════════════════
    //  OVERRIDE PANEL CALLBACKS
    // ═══════════════════════════════════════════════

    private void OnOverrideCheckboxToggled(bool pressed)
    {
        if (_selectedEye == null) return;

        if (pressed)
        {
            if (!_selectedEye.AnimOverrides.ContainsKey(_currentAnimName))
            {
                var ov = new AnimationOverride
                {
                    BoneName = _selectedEye.AnchorBone,
                    Hidden = _selectedEye.HiddenByDefault,
                    Opacity = _selectedEye.Opacity
                };
                _selectedEye.AnimOverrides[_currentAnimName] = ov;
                CacheOverrideOffset(ov);
            }
        }
        else
        {
            _selectedEye.AnimOverrides.Remove(_currentAnimName);
            _editingSegmentIndex = -1;
        }

        RefreshOverridePanel();
        RefreshSegmentUI();
        Screen.RefreshSelectionPanel();
    }

    private void OnOverrideBoneChanged(string text)
    {
        if (_selectedEye == null) return;
        if (!_selectedEye.AnimOverrides.TryGetValue(_currentAnimName, out var ov)) return;
        ov.BoneName = string.IsNullOrWhiteSpace(text) ? null : text;
    }

    private void OnOverrideVisibleToggled(bool pressed)
    {
        if (_selectedEye == null) return;
        if (!_selectedEye.AnimOverrides.TryGetValue(_currentAnimName, out var ov)) return;
        ov.Hidden = !pressed;
    }

    private void OnOverrideOpacityChanged(double value)
    {
        if (_selectedEye == null) return;
        if (!_selectedEye.AnimOverrides.TryGetValue(_currentAnimName, out var ov)) return;
        ov.Opacity = (float)value;
    }

    // ═══════════════════════════════════════════════
    //  OVERRIDE PANEL REFRESH
    // ═══════════════════════════════════════════════

    /// <summary>
    /// Updates the override section of the bone panel based on the
    /// currently selected eye and animation.
    /// </summary>
    private void RefreshOverridePanel()
    {
        bool hasEye = _selectedEye != null;
        bool hasAnim = !string.IsNullOrEmpty(_currentAnimName);

        _overrideAnimLabel.Text = hasAnim
            ? "Animation Override: " + _currentAnimName
            : "Animation Override";

        if (!hasEye || !hasAnim)
        {
            _overrideCheckbox.Visible = false;
            _overrideFieldsRow.Visible = false;
            _overrideHintLabel.Visible = true;
            _overrideHintLabel.Text = "Select an eye and an animation to configure overrides.";
            _overrideInfoLabel.Text = "";
            _splitHereButton.Disabled = true;
            _clearSegmentsButton.Disabled = true;
            return;
        }

        _overrideCheckbox.Visible = true;
        bool hasOverride = _selectedEye.AnimOverrides.TryGetValue(_currentAnimName, out var ov);

        _overrideCheckbox.SetPressedNoSignal(hasOverride);

        if (hasOverride)
        {
            _overrideFieldsRow.Visible = true;
            _overrideHintLabel.Visible = false;
            _overrideBoneInput.Text = ov.BoneName ?? _selectedEye.AnchorBone;
            _overrideVisibleCheckbox.SetPressedNoSignal(!(ov.Hidden ?? false));
            _overrideOpacityInput.SetValueNoSignal(ov.Opacity ?? _selectedEye.Opacity);
            _splitHereButton.Disabled = false;
            _clearSegmentsButton.Disabled = ov.Segments.Count == 0;

            int segCount = ov.Segments.Count;
            _overrideInfoLabel.Text = segCount > 0
                ? segCount + " time segment(s)"
                : "whole-animation override";
        }
        else
        {
            _overrideFieldsRow.Visible = false;
            _overrideHintLabel.Visible = true;
            _overrideHintLabel.Text = "No override — eye uses defaults (bone: "
                + _selectedEye.AnchorBone
                + ", " + (_selectedEye.HiddenByDefault ? "hidden" : "visible") + ").";
            _overrideInfoLabel.Text = "";
            _splitHereButton.Disabled = true;
            _clearSegmentsButton.Disabled = true;
        }
    }

    // ═══════════════════════════════════════════════
    //  BONE MARKERS
    // ═══════════════════════════════════════════════

    private void ToggleBoneMarkers()
    {
        if (_showingBones)
        {
            ClearBoneMarkers();
            _showBonesButton.Text = "Show Bones";
            _showingBones = false;
        }
        else
        {
            ShowBoneMarkers();
            _showBonesButton.Text = "Hide Bones";
            _showingBones = true;
        }
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
                || info.Name.Contains("jaw") || info.Name == _selectedAnchorBone)
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
        if (_currentVisuals == null || _spineNode == null) return spinePos;
        return _currentVisuals.Position + _spineNode.Position + spinePos * _spineNode.Scale;
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

    /// <summary>
    /// Caches the spine-space offset for an animation override,
    /// based on the eye's current preview position and the override's bone.
    /// </summary>
    private void CacheOverrideOffset(AnimationOverride ov)
    {
        if (_selectedEye == null || _skeletonGodot == null || _spineNode == null)
        {
            ov.HasSpineOffset = false;
            return;
        }

        string boneName = ov.BoneName ?? _selectedEye.AnchorBone;
        var bone = _skeletonGodot.Call("find_bone", boneName).AsGodotObject();
        if (bone == null) { ov.HasSpineOffset = false; return; }

        var bonePreview = SpineToPreview(new Vector2(
            (float)bone.Call("get_world_x"),
            (float)bone.Call("get_world_y")));
        var worldOffset = (_selectedEye.Position - bonePreview) / _spineNode.Scale;
        ov.SpineOffset = UnrotateByBone(worldOffset, bone);
        ov.HasSpineOffset = true;
    }

    // ═══════════════════════════════════════════════
    //  SEGMENT LOGIC
    // ═══════════════════════════════════════════════

    private void UpdateActiveSegmentLabel(float time)
    {
        if (_selectedEye == null || _activeSegmentLabel == null) return;

        if (!_selectedEye.AnimOverrides.TryGetValue(_currentAnimName, out var ov) || ov.Segments.Count == 0)
        {
            if (ov != null)
                _activeSegmentLabel.Text = "override active (no segments)";
            else
                _activeSegmentLabel.Text = "";
            return;
        }

        var segs = ov.Segments;
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
                string opacityInfo = "";
                if (seg.OpacityStart < 0.99f || seg.OpacityEnd < 0.99f)
                {
                    float segDur = seg.EndTime - seg.StartTime;
                    float t = segDur > 0f ? (time - seg.StartTime) / segDur : 0f;
                    float curAlpha = Mathf.Lerp(seg.OpacityStart, seg.OpacityEnd, t);
                    opacityInfo = ", α=" + curAlpha.ToString("F2");
                }
                _activeSegmentLabel.Text = "@ " + time.ToString("F2") + "s → segment " + (i + 1)
                    + " (" + seg.BoneName + ", visible" + opacityInfo + ")"
                    + (isEditing ? "  ← EDITING" : "");
                ApplyFont(_activeSegmentLabel, 11, isEditing ? EditorTheme.AccentWarm : EditorTheme.AccentGreen);
            }
            return;
        }

        _activeSegmentLabel.Text = "@ " + time.ToString("F2") + "s → past all segments, using last";
        ApplyFont(_activeSegmentLabel, 11, EditorTheme.TextDim);
    }

    private void SplitAtScrubPosition()
    {
        if (_selectedEye == null) { SetOutput("Select an eye first."); return; }

        // Ensure an override exists (auto-create one if needed)
        if (!_selectedEye.AnimOverrides.TryGetValue(_currentAnimName, out var ov))
        {
            ov = new AnimationOverride
            {
                BoneName = _selectedEye.AnchorBone,
                Hidden = _selectedEye.HiddenByDefault,
                Opacity = _selectedEye.Opacity
            };
            CacheOverrideOffset(ov);
            _selectedEye.AnimOverrides[_currentAnimName] = ov;
            _overrideCheckbox.SetPressedNoSignal(true);
            RefreshOverridePanel();
        }

        var segments = ov.Segments;
        float duration = GetCurrentAnimDuration();
        if (duration <= 0f) return;
        float splitTime = GetCurrentScrubTime();

        if (segments.Count == 0)
        {
            if (splitTime <= 0.01f || splitTime >= duration - 0.01f)
            {
                SetOutput("Scrub to a point between start and end before splitting.");
                return;
            }

            string bone = ov.BoneName ?? _selectedEye.AnchorBone;
            bool hidden = ov.Hidden ?? _selectedEye.HiddenByDefault;
            float opacity = ov.Opacity ?? _selectedEye.Opacity;

            segments.Add(new BoneSegment
            {
                StartTime = 0f, EndTime = splitTime, BoneName = bone,
                Hidden = hidden, OpacityStart = opacity, OpacityEnd = opacity
            });
            segments.Add(new BoneSegment
            {
                StartTime = splitTime, EndTime = duration, BoneName = bone,
                Hidden = hidden, OpacityStart = opacity, OpacityEnd = opacity
            });
            CacheSegmentOffset(segments[0]);
            CacheSegmentOffset(segments[1]);
            SetOutput("Split at " + splitTime.ToString("F2") + "s — click 'Edit' to set bone and position per segment.");
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
                    float midOpacity = Mathf.Lerp(seg.OpacityStart, seg.OpacityEnd, t);

                    var newSeg = new BoneSegment
                    {
                        StartTime = splitTime, EndTime = seg.EndTime,
                        BoneName = seg.BoneName, Hidden = seg.Hidden,
                        OpacityStart = midOpacity, OpacityEnd = seg.OpacityEnd
                    };
                    seg.OpacityEnd = midOpacity;
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
        RefreshOverridePanel();
        RefreshSegmentUI();
    }

    private void StartEditingSegment(int index)
    {
        if (_selectedEye == null) return;
        if (!_selectedEye.AnimOverrides.TryGetValue(_currentAnimName, out var ov)) return;
        if (index < 0 || index >= ov.Segments.Count) return;

        _editingSegmentIndex = index;
        var seg = ov.Segments[index];
        float mid = (seg.StartTime + seg.EndTime) / 2f;
        float dur = GetCurrentAnimDuration();
        if (dur > 0f)
        {
            _isPlaying = false;
            _playPauseButton.Text = "Play";
            SetAnimationPaused(_currentAnimName, mid / dur);
        }
        SetInfo("EDITING segment " + (index + 1) + ": drag the eye to set offset from '" + seg.BoneName + "'.");
        RefreshSegmentUI();
    }

    private void StopEditingSegment()
    {
        if (_selectedEye != null && _editingSegmentIndex >= 0
            && _selectedEye.AnimOverrides.TryGetValue(_currentAnimName, out var ov)
            && _editingSegmentIndex < ov.Segments.Count)
        {
            CacheSegmentOffsetFromCurrentPose(ov.Segments[_editingSegmentIndex]);
        }
        _editingSegmentIndex = -1;
        SetInfo(_placements.Count + " eye(s) placed.");
        RefreshSegmentUI();
    }

    private void RemoveSegment(int index)
    {
        if (_selectedEye == null) return;
        if (!_selectedEye.AnimOverrides.TryGetValue(_currentAnimName, out var ov)) return;
        var segs = ov.Segments;
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

        if (segs.Count <= 1 && segs.Count == 1)
        {
            // One segment left — collapse back into the override
            var last = segs[0];
            ov.BoneName = last.BoneName;
            if (last.HasSpineOffset) { ov.SpineOffset = last.SpineOffset; ov.HasSpineOffset = true; }
            ov.Hidden = last.Hidden;
            ov.Opacity = last.OpacityStart;
            segs.Clear();
            _editingSegmentIndex = -1;
            SetOutput("Only one segment left — collapsed into override.");
        }

        RefreshOverridePanel();
        RefreshSegmentUI();
    }

    private void ClearSegmentsForSelected()
    {
        if (_selectedEye == null) return;
        if (!_selectedEye.AnimOverrides.TryGetValue(_currentAnimName, out var ov)) return;
        ov.Segments.Clear();
        _editingSegmentIndex = -1;
        RefreshOverridePanel();
        RefreshSegmentUI();
        SetOutput("Cleared time segments for " + _currentAnimName + ". Override still active.");
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

    // ═══════════════════════════════════════════════
    //  SEGMENT UI
    // ═══════════════════════════════════════════════

    private void RefreshSegmentUI()
    {
        foreach (var child in _segmentList.GetChildren())
            if (child is Node n && GodotObject.IsInstanceValid(n)) n.QueueFree();

        if (_selectedEye == null)
        {
            _segmentList.AddChild(MakeLabel("Select an eye first.",
                Vector2.Zero, new Vector2(600, 20), 11, EditorTheme.TextDim));
            return;
        }

        if (!_selectedEye.AnimOverrides.TryGetValue(_currentAnimName, out var ov) || ov.Segments.Count == 0)
        {
            string hint = _selectedEye.AnimOverrides.ContainsKey(_currentAnimName)
                ? "No time segments. Eye uses the override above for the entire animation."
                : "No override active. Add one above, then split if you need mid-animation bone switching.";

            var lbl = MakeLabel(hint, Vector2.Zero, new Vector2(600, 40), 11, EditorTheme.TextDim);
            lbl.AutowrapMode = TextServer.AutowrapMode.WordSmart;
            _segmentList.AddChild(lbl);
            return;
        }

        for (int i = 0; i < ov.Segments.Count; i++)
            BuildSegmentRow(ov.Segments, i);
    }

    private void BuildSegmentRow(List<BoneSegment> segments, int i)
    {
        var seg = segments[i];
        int idx = i;
        bool isEditing = i == _editingSegmentIndex;

        var row = new HBoxContainer();
        row.AddThemeConstantOverride("separation", 4);

        if (isEditing)
            row.AddChild(new ColorRect { Color = EditorTheme.SegmentActiveBg, CustomMinimumSize = new Vector2(4, 0) });

        // Index
        var idxLbl = new Label { Text = (i + 1) + ".", CustomMinimumSize = new Vector2(20, 0) };
        ApplyFont(idxLbl, 12, isEditing ? EditorTheme.AccentWarm : EditorTheme.TextDim);
        row.AddChild(idxLbl);

        // Time range
        var timeLbl = new Label { Text = seg.StartTime.ToString("F2") + "s – " + seg.EndTime.ToString("F2") + "s", CustomMinimumSize = new Vector2(120, 0) };
        ApplyFont(timeLbl, 11, EditorTheme.TextNormal);
        row.AddChild(timeLbl);

        // Bone name
        var boneEdit = new LineEdit
        {
            Text = seg.BoneName, CustomMinimumSize = new Vector2(100, 0),
            Editable = isEditing && !seg.Hidden,
            TooltipText = seg.Hidden ? "Hidden during this segment." : (isEditing ? "Type the bone name." : "Click 'Edit' to change.")
        };
        if (Font != null) boneEdit.AddThemeFontOverride("font", Font);
        boneEdit.AddThemeFontSizeOverride("font_size", 11);
        if (isEditing && !seg.Hidden) boneEdit.TextChanged += text => seg.BoneName = text;
        if (seg.Hidden) boneEdit.Modulate = new Color(1, 1, 1, 0.4f);
        row.AddChild(boneEdit);

        // Visible toggle
        var visCheck = new CheckBox { Text = "Visible", ButtonPressed = !seg.Hidden, CustomMinimumSize = new Vector2(72, 0) };
        ApplyFont(visCheck, 10, seg.Hidden ? EditorTheme.TextDim : EditorTheme.AccentGreen);
        visCheck.Toggled += pressed => { seg.Hidden = !pressed; RefreshSegmentUI(); };
        row.AddChild(visCheck);

        // Edit / Done
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

        // Remove
        var removeBtn = new Button { Text = "X", CustomMinimumSize = new Vector2(26, 0) };
        if (Font != null) removeBtn.AddThemeFontOverride("font", Font);
        removeBtn.AddThemeFontSizeOverride("font_size", 11);
        removeBtn.Pressed += () => RemoveSegment(idx);
        row.AddChild(removeBtn);

        // Offset indicator
        if (!seg.Hidden)
        {
            var offLbl = new Label { Text = seg.HasSpineOffset ? "offset set" : "no offset", CustomMinimumSize = new Vector2(70, 0) };
            ApplyFont(offLbl, 10, seg.HasSpineOffset ? EditorTheme.AccentGreen : EditorTheme.TextDim);
            row.AddChild(offLbl);

            // Opacity start
            var opStartLbl = new Label { Text = "α:", CustomMinimumSize = new Vector2(16, 0), TooltipText = "Opacity at segment start" };
            ApplyFont(opStartLbl, 10, EditorTheme.TextDim);
            row.AddChild(opStartLbl);

            var opStartSpin = new SpinBox
            {
                MinValue = 0.0, MaxValue = 1.0, Step = 0.05,
                Value = seg.OpacityStart,
                CustomMinimumSize = new Vector2(70, 0),
                Editable = isEditing,
                TooltipText = "Opacity at the start of this segment (0 = invisible, 1 = fully visible)."
            };
            if (Font != null) opStartSpin.AddThemeFontOverride("font", Font);
            opStartSpin.AddThemeFontSizeOverride("font_size", 10);
            if (isEditing) opStartSpin.ValueChanged += v => { seg.OpacityStart = (float)v; };
            row.AddChild(opStartSpin);

            // Arrow
            var arrowLbl = new Label { Text = "→", CustomMinimumSize = new Vector2(14, 0) };
            ApplyFont(arrowLbl, 10, EditorTheme.TextDim);
            row.AddChild(arrowLbl);

            // Opacity end
            var opEndSpin = new SpinBox
            {
                MinValue = 0.0, MaxValue = 1.0, Step = 0.05,
                Value = seg.OpacityEnd,
                CustomMinimumSize = new Vector2(70, 0),
                Editable = isEditing,
                TooltipText = "Opacity at the end of this segment. If different from start, opacity lerps over the segment duration."
            };
            if (Font != null) opEndSpin.AddThemeFontOverride("font", Font);
            opEndSpin.AddThemeFontSizeOverride("font_size", 10);
            if (isEditing) opEndSpin.ValueChanged += v => { seg.OpacityEnd = (float)v; };
            row.AddChild(opEndSpin);
        }

        _segmentList.AddChild(row);
    }
}