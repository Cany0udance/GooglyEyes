using System.Text;
using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace GooglyEyes;

/// <summary>
/// Editor tab for placing googly eyes on monster sprites.
/// Handles Spine skeleton animations, bone anchoring, timed bone segments, and export.
/// </summary>
public class MonsterEditorTab : EditorTab
{
    public override string TabName => "Monsters";
    public override string TabTooltip => "Place googly eyes on monster sprites.";

    // ── Sidebar ──
    private ScrollContainer _sidebarScroll;
    private VBoxContainer _monsterList;
    private Button _selectedButton;
    private bool _listBuilt;

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
        /// <summary>Opacity at the start of this segment (0..1). Lerps to OpacityEnd over the segment duration.</summary>
        public float OpacityStart = 1f;
        /// <summary>Opacity at the end of this segment (0..1).</summary>
        public float OpacityEnd = 1f;
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
        public Dictionary<string, List<BoneSegment>> BoneTimelines = new();
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

        foreach (MonsterModel monster in ModelDb.Monsters.OrderBy(m => m.Id.Entry))
        {
            var id = monster.Id.Entry;
            bool hasConfig = GooglyEyesRegistry.Configs.ContainsKey(id);

            var button = new Button
            {
                Text = id, Flat = true,
                Alignment = HorizontalAlignment.Left,
                CustomMinimumSize = new Vector2(0, 32),
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
            };
            ApplyFont(button, 14, hasConfig ? EditorTheme.AccentGreen : EditorTheme.TextNormal);
            button.AddThemeColorOverride("font_hover_color", EditorTheme.TextBright);

            var monsterRef = monster;
            button.Pressed += () => SelectMonster(monsterRef, id);
            _monsterList.AddChild(button);
        }
    }

    public override void Activate()
    {
        _sidebarScroll.Visible = true;
        SetPanelsVisible(true);
        BuildSidebarItems();
    }

    public override void Deactivate()
    {
        _sidebarScroll.Visible = false;
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
        _sidebarScroll = new ScrollContainer
        {
            Position = new Vector2(0, 62),
            Size = new Vector2(EditorTheme.SidebarWidth, screenSize.Y - 62),
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

        _bonePanelContainer.AddChild(new ColorRect { Color = EditorTheme.PanelBg, Position = Vector2.Zero, Size = new Vector2(pw, ph) });

        // Title
        _bonePanelContainer.AddChild(MakeLabel("Bone Anchoring", new Vector2(10, 4), new Vector2(120, 16), 11, EditorTheme.Accent));
        _anchorPanelHint = MakeLabel("Which bone this eye follows. Most eyes only need the default.",
            new Vector2(140, 4), new Vector2(pw - 160, 16), 10, EditorTheme.TextDim);
        _bonePanelContainer.AddChild(_anchorPanelHint);

        // Row 2: default bone
        float row2 = 26;
        _bonePanelContainer.AddChild(MakeLabel("Default bone:", new Vector2(10, row2 + 3), new Vector2(100, 24), 12, EditorTheme.TextNormal));

        _anchorBoneInput = new LineEdit
        {
            Text = "head", Position = new Vector2(112, row2 - 2), Size = new Vector2(130, 30),
            TooltipText = "The bone this eye follows during idle and any animation without segments."
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
        _showBonesButton.TooltipText = "Show bone markers on the preview.";
        _showBonesButton.Pressed += ToggleBoneMarkers;
        _bonePanelContainer.AddChild(_showBonesButton);

        _showAllBoneNamesCheckbox = new CheckBox
        {
            Text = "All names", Position = new Vector2(370, row2 - 2), Size = new Vector2(100, 30),
            TooltipText = "Label every bone, not just head/face/eye."
        };
        ApplyFont(_showAllBoneNamesCheckbox, 11, EditorTheme.TextNormal);
        _showAllBoneNamesCheckbox.Toggled += _ => { if (_showingBones) RebuildBoneMarkerNodes(); };
        _bonePanelContainer.AddChild(_showAllBoneNamesCheckbox);

        _anchorPosLabel = MakeLabel("", new Vector2(480, row2 + 3), new Vector2(300, 24), 11, EditorTheme.Accent);
        _bonePanelContainer.AddChild(_anchorPosLabel);

        // Divider
        float divY = row2 + 34;
        _bonePanelContainer.AddChild(new ColorRect { Color = EditorTheme.Divider, Position = new Vector2(10, divY), Size = new Vector2(pw - 20, 1) });

        // Segment header
        float segHdrY = divY + 6;
        _bonePanelContainer.AddChild(MakeLabel("Per-animation bone switching", new Vector2(10, segHdrY), new Vector2(220, 16), 11, EditorTheme.AccentWarm));
        _bonePanelContainer.AddChild(MakeLabel("Only needed if an eye must follow different bones at different times during one animation.",
            new Vector2(240, segHdrY), new Vector2(pw - 260, 16), 10, EditorTheme.TextDim));

        // Segment buttons
        float btnY = segHdrY + 20;
        _splitHereButton = MakeButton("Split here", new Vector2(10, btnY), new Vector2(100, 26), 11);
        _splitHereButton.TooltipText = "Split the timeline at the current scrub position.";
        _splitHereButton.Pressed += SplitAtScrubPosition;
        _bonePanelContainer.AddChild(_splitHereButton);

        _clearSegmentsButton = MakeButton("Clear segments", new Vector2(120, btnY), new Vector2(115, 26), 11);
        _clearSegmentsButton.TooltipText = "Remove all segments for this animation.";
        _clearSegmentsButton.Pressed += ClearSegmentsForSelected;
        _bonePanelContainer.AddChild(_clearSegmentsButton);

        _activeSegmentLabel = MakeLabel("", new Vector2(250, btnY + 3), new Vector2(pw - 270, 20), 11, EditorTheme.TextDim);
        _bonePanelContainer.AddChild(_activeSegmentLabel);

        // Scrollable segment list
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
    //  MONSTER SELECTION
    // ═══════════════════════════════════════════════

    private void SelectMonster(MonsterModel monster, string monsterId)
    {
        ClearAll();
        ClearBoneMarkers();
        HighlightButton(monsterId);

        _showingBones = false;
        _showBonesButton.Text = "Show Bones";
        _skeletonGodot = null;
        _spineNode = null;
        _animState = null;
        _cachedTrackEntry = null;
        _currentMonsterId = monsterId;
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

        _currentVisuals = monster.CreateVisuals();
        PreviewRoot.AddChild(_currentVisuals);
        _currentVisuals.Position = new Vector2(
            PreviewArea.Size.X / 2,
            PreviewArea.Size.Y / 2 + _currentVisuals.Bounds.Size.Y * 0.25f
        );

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

        SetInfo(monsterId + " loaded — click the preview to place eyes!");
        AdvanceWorkflow(WorkflowStep.PlaceEyes);

        if (LoadPresetsForMonster(monsterId))
        {
            SetInfo(monsterId + " loaded with " + _placements.Count + " preset eye(s) from registry.");
            if (_placements.Count > 0) AdvanceWorkflow(WorkflowStep.AdjustEyes);
        }
        RefreshSegmentUI();
    }

    private void HighlightButton(string monsterId)
    {
        if (_selectedButton != null && GodotObject.IsInstanceValid(_selectedButton))
        {
            bool prev = GooglyEyesRegistry.Configs.ContainsKey(_selectedButton.Text);
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
        sb.AppendLine("}},");

        GD.Print("[GooglyEyes] === EXPORT ===");
        GD.Print(sb.ToString());
        GD.Print("[GooglyEyes] === END EXPORT ===");

        int totalSegs = _placements.Sum(p => p.BoneTimelines.Values.Sum(l => l.Count));
        SetOutput("Exported " + _placements.Count + " eye(s), " + totalSegs + " segment(s) — check console!");
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
        RefreshSegmentUI();
    }

    // ═══════════════════════════════════════════════
    //  PRESET LOADING
    // ═══════════════════════════════════════════════

    private bool LoadPresetsForMonster(string monsterId)
    {
        if (!GooglyEyesRegistry.Configs.TryGetValue(monsterId, out var configs)) return false;
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
                            OpacityStart = src.OpacityStart,
                            OpacityEnd = src.OpacityEnd
                        };
                        if (!src.Hidden) { seg.SpineOffset = src.Offset; seg.HasSpineOffset = true; }
                        editorSegs.Add(seg);
                    }
                    placement.BoneTimelines[kvp.Key] = editorSegs;
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
        string[] common = {
            "idle_loop", "hurt", "attack", "cast", "dead", "die", "revive",
            "idle", "hit", "spawn", "entrance", "taunt",
            "buff", "debuff", "charge_up", "attack_heavy"
        };
        foreach (var name in common)
        {
            if (_animController.HasAnimation(name))
            {
                _availableAnims.Add(name);
                _animDropdown.AddItem(name);
            }
        }
        if (_availableAnims.Count > 0) _animDropdown.Selected = 0;
        PopulateTrack1List();
    }

    private void PopulateTrack1List()
    {
        _track1Dropdown.Clear();
        _availableTrack1Anims.Clear();
        _currentTrack1Anim = "";

        _track1Dropdown.AddItem("(none)");
        _availableTrack1Anims.Add("");

        string[] patterns = {
            "_tracks/charge_up_1", "_tracks/charged_1",
            "_tracks/charge_up_2", "_tracks/charged_2",
            "_tracks/charge_up_3", "_tracks/charged_3",
            "_tracks/charge_up_4", "_tracks/charged_4",
            "_tracks/charged_0", "_tracks/attack_heavy"
        };
        foreach (var p in patterns)
        {
            if (_animController.HasAnimation(p))
            {
                _availableTrack1Anims.Add(p);
                _track1Dropdown.AddItem(p);
            }
        }
        for (int i = 1; i <= 10; i++)
        {
            var cu = "_tracks/charge_up_" + i;
            var cd = "_tracks/charged_" + i;
            if (!_availableTrack1Anims.Contains(cu) && _animController.HasAnimation(cu))
            {
                _availableTrack1Anims.Add(cu);
                _track1Dropdown.AddItem(cu);
            }
            if (!_availableTrack1Anims.Contains(cd) && _animController.HasAnimation(cd))
            {
                _availableTrack1Anims.Add(cd);
                _track1Dropdown.AddItem(cd);
            }
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

    /// <summary>
    /// Resolves the opacity for an eye at the given time.
    /// If a segment is active and not hidden, lerps between OpacityStart and OpacityEnd.
    /// Otherwise uses the eye's base Opacity.
    /// </summary>
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
            // Past all segments: use last segment's end opacity
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

    // ═══════════════════════════════════════════════
    //  SEGMENT LOGIC
    // ═══════════════════════════════════════════════

    private void UpdateActiveSegmentLabel(float time)
    {
        if (_selectedEye == null || _activeSegmentLabel == null) return;
        if (!_selectedEye.BoneTimelines.TryGetValue(_currentAnimName, out var segs) || segs.Count == 0)
        {
            _activeSegmentLabel.Text = "";
            return;
        }

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
                    + (isEditing ? "  ← EDITING (drag eye to reposition)" : "");
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
        if (_currentAnimName == "idle_loop") { SetOutput("Switch to a non-idle animation first."); return; }

        float duration = GetCurrentAnimDuration();
        if (duration <= 0f) return;
        float splitTime = GetCurrentScrubTime();

        if (!_selectedEye.BoneTimelines.ContainsKey(_currentAnimName))
            _selectedEye.BoneTimelines[_currentAnimName] = new List<BoneSegment>();

        var segments = _selectedEye.BoneTimelines[_currentAnimName];

        if (segments.Count == 0)
        {
            if (splitTime <= 0.01f || splitTime >= duration - 0.01f)
            {
                SetOutput("Scrub to a point between start and end before splitting.");
                return;
            }

            bool inherit = _selectedEye.HiddenByDefault;
            float baseOpacity = _selectedEye.Opacity;
            segments.Add(new BoneSegment { StartTime = 0f, EndTime = splitTime, BoneName = _selectedEye.AnchorBone, Hidden = inherit, OpacityStart = baseOpacity, OpacityEnd = baseOpacity });
            segments.Add(new BoneSegment { StartTime = splitTime, EndTime = duration, BoneName = _selectedEye.AnchorBone, Hidden = inherit, OpacityStart = baseOpacity, OpacityEnd = baseOpacity });
            CacheSegmentOffset(segments[0]);
            CacheSegmentOffset(segments[1]);

            SetOutput(inherit
                ? "Split at " + splitTime.ToString("F2") + "s — toggle 'Visible' on the segment where this eye should appear."
                : "Split at " + splitTime.ToString("F2") + "s — click 'Edit' to set bone and position.");
        }
        else
        {
            for (int i = 0; i < segments.Count; i++)
            {
                var seg = segments[i];
                if (splitTime > seg.StartTime + 0.01f && splitTime < seg.EndTime - 0.01f)
                {
                    // Interpolate opacity at the split point
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
            && _selectedEye.BoneTimelines.TryGetValue(_currentAnimName, out var segs)
            && _editingSegmentIndex < segs.Count)
        {
            CacheSegmentOffsetFromCurrentPose(segs[_editingSegmentIndex]);
        }
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
            if (segs.Count == 1) SetOutput("Only one segment left — removed. Eye uses default bone.");
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
        if (_currentAnimName == "idle_loop")
        {
            _segmentList.AddChild(MakeLabel("Switch to a non-idle animation to set up bone segments.",
                Vector2.Zero, new Vector2(600, 20), 11, EditorTheme.TextDim));
            return;
        }
        if (!_selectedEye.BoneTimelines.TryGetValue(_currentAnimName, out var segments) || segments.Count == 0)
        {
            string hint = _selectedEye.HiddenByDefault
                ? "This eye is hidden by default — scrub to where it should appear, click 'Split here', then toggle 'Visible'."
                : "No segments — eye follows default bone ('" + _selectedEye.AnchorBone + "'). To switch mid-animation: scrub, click 'Split here'.";
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