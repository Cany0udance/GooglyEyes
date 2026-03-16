using System.Reflection;
using System.Text;
using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

namespace GooglyEyes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Godot;

public partial class NGooglyEyesEditorScreen : NSubmenu
{
    private const float Padding = 16f;
    private const float SidebarWidth = 280f;
    private static readonly Color BgColor = new(0.07f, 0.07f, 0.1f, 1f);
    private static readonly Color PanelBg = new(0.1f, 0.1f, 0.15f, 1f);
    private static readonly Color PreviewBg = new(0.05f, 0.05f, 0.07f, 1f);
    private static readonly Color Accent = new(0.4f, 0.55f, 0.95f, 1f);
    private static readonly Color AccentWarm = new(0.95f, 0.6f, 0.3f, 1f);
    private static readonly Color AccentGreen = new(0.3f, 0.75f, 0.45f, 1f);
    private static readonly Color TextBright = new(0.9f, 0.9f, 0.93f, 1f);
    private static readonly Color TextNormal = new(0.72f, 0.72f, 0.78f, 1f);
    private static readonly Color TextDim = new(0.45f, 0.45f, 0.52f, 1f);
    private static readonly Color HelpBg = new(0.12f, 0.14f, 0.22f, 0.95f);
    private static readonly Color StepActiveBg = new(0.4f, 0.55f, 0.95f, 0.25f);
    private static readonly Color StepInactiveBg = new(0.1f, 0.1f, 0.15f, 0.5f);
    private static readonly Color StepCompleteBg = new(0.3f, 0.7f, 0.4f, 0.25f);
    private static readonly Color SegmentActiveBg = new(0.4f, 0.55f, 0.95f, 0.3f);
    private static readonly Color SegmentInactiveBg = new(0.12f, 0.12f, 0.18f, 0.5f);

    private Font _font;

    // Core UI
    private VBoxContainer _monsterList;
    private Control _previewArea;
    private Node2D _previewRoot;
    private Label _infoLabel;
    private Label _outputLabel;
    private Button _exportButton;
    private Button _clearButton;
    private Button _undoButton;
    private ScrollContainer _sidebarScroll;

    // Selected eye controls
    private EyePlacement _selectedEye;
    private SpinBox _scaleInput;
    private Label _selectedLabel;
    private Button _duplicateButton;
    private Label _selectionPanelHint;
    private CheckBox _hiddenByDefaultCheckbox;

    // Bone anchoring
    private string _selectedAnchorBone = "head";
    private LineEdit _anchorBoneInput;
    private Button _showBonesButton;
    private bool _showingBones;
    private readonly List<Node2D> _boneMarkers = new();
    private GodotObject _skeletonGodot;
    private Node2D _spineNode;
    private Label _anchorPosLabel;
    private Label _anchorPanelHint;
    private CheckBox _showAllBoneNamesCheckbox;

    // Spine / visuals
    private NCreatureVisuals _currentVisuals;
    private MegaSprite _animController;
    private string _currentMonsterId;
    private readonly List<EyePlacement> _placements = new();
    private EyePlacement _dragging;
    private bool _isDragging;
    private Texture2D _eyeTexture;
    private Texture2D _irisTexture;

    // Zoom and pan
    private float _zoom = 1f;
    private Vector2 _panOffset = Vector2.Zero;
    private bool _isPanning;
    private Vector2 _panStart;

    // Animation controls
    private OptionButton _animDropdown;
    private HSlider _frameSlider;
    private Label _frameLabel;
    private Button _playPauseButton;
    private bool _isPlaying;
    private string _currentAnimName = "idle_loop";
    private readonly List<string> _availableAnims = new();
    private Label _animPanelHint;

    // Workflow guidance
    private enum WorkflowStep { SelectMonster, PlaceEyes, AdjustEyes, Export }
    private WorkflowStep _currentStep = WorkflowStep.SelectMonster;
    private readonly Label[] _stepLabels = new Label[4];
    private readonly ColorRect[] _stepBgs = new ColorRect[4];
    private Label _stepHintLabel;

    // Help overlay
    private Control _helpOverlay;
    private bool _helpVisible;
    private Button _helpToggleButton;

    // Bone segment UI (within bone panel)
    private VBoxContainer _segmentList;
    private ScrollContainer _segmentScroll;
    private Button _splitHereButton;
    private Button _clearSegmentsButton;
    private Label _activeSegmentLabel;
    /// <summary>Which segment index is selected for editing (dragging). -1 = none.</summary>
    private int _editingSegmentIndex = -1;

    private struct BoneInfo
    {
        public string Name;
        public Vector2 SpineWorldPos;
    }

    private readonly List<BoneInfo> _boneData = new();

    /// <summary>
    /// A timed anchor bone segment: from StartTime to EndTime (in seconds),
    /// the eye follows the specified bone at a given offset.
    /// </summary>
    private class BoneSegment
    {
        public float StartTime;
        public float EndTime;
        public string BoneName = "head";
        /// <summary>Offset from bone, in spine-local space. Set by dragging the eye while this segment is active.</summary>
        public Vector2 SpineOffset;
        public bool HasSpineOffset;
        /// <summary>When true, the eye is invisible during this segment.</summary>
        public bool Hidden;
    }

    private class EyePlacement
    {
        public Vector2 Position;
        public float Scale = 1f;
        public string AnchorBone = "head";
        public Sprite2D EyeSprite;
        public Sprite2D IrisSprite;
        public Node2D Container;

        /// <summary>Default spine-local offset from anchor bone.</summary>
        public Vector2 SpineOffset;
        public bool HasSpineOffset;

        /// <summary>When true, this eye is hidden by default and only appears during segments that are not hidden.</summary>
        public bool HiddenByDefault;

        /// <summary>
        /// Per-animation bone segment timelines. Key = animation name.
        /// If present, these override AnchorBone for that animation.
        /// </summary>
        public Dictionary<string, List<BoneSegment>> BoneTimelines = new();
    }

    protected override Control InitialFocusedControl => null;

    // ════════════════════════════════════════════════
    //  LIFECYCLE
    // ════════════════════════════════════════════════

    public override void _Ready()
    {
        try
        {
            SetAnchorsPreset(LayoutPreset.FullRect);
            var screenSize = GetViewportRect().Size;

            _font = ResourceLoader.Load<Font>("res://fonts/kreon_regular.ttf");
            _eyeTexture = ResourceLoader.Load<Texture2D>("res://GooglyEyes/googly_eye.png");
            _irisTexture = ResourceLoader.Load<Texture2D>("res://GooglyEyes/googly_iris.png");

            var bg = new ColorRect { Color = BgColor };
            bg.SetAnchorsPreset(LayoutPreset.FullRect);
            AddChild(bg);

            BuildSidebar(screenSize);
            BuildWorkflowBar(screenSize);
            BuildPreviewPanel(screenSize);
            BuildAnimationControls(screenSize);
            BuildToolbar(screenSize);
            BuildSelectionPanel(screenSize);
            BuildBonePanel(screenSize);
            BuildBackButton();
            BuildHelpToggle(screenSize);
            BuildHelpOverlay(screenSize);
            BuildMonsterList();

            UpdateWorkflowStep(WorkflowStep.SelectMonster);
        }
        catch (Exception e)
        {
            GD.PrintErr("[GooglyEyes] Editor _Ready error: " + e.ToString());
        }
    }

    public override void _Process(double delta)
    {
        // Update bone marker positions from skeleton
        if (_showingBones && _boneData.Count > 0)
        {
            if (_skeletonGodot != null)
            {
                var allBones = _skeletonGodot.Call("get_bones").AsGodotArray();
                for (int i = 0; i < allBones.Count && i < _boneData.Count; i++)
                {
                    var bone = allBones[i].AsGodotObject();
                    var wx = (float)bone.Call("get_world_x");
                    var wy = (float)bone.Call("get_world_y");
                    _boneData[i] = new BoneInfo
                    {
                        Name = _boneData[i].Name,
                        SpineWorldPos = new Vector2(wx, wy)
                    };
                }
            }

            for (int i = 0; i < _boneData.Count && i < _boneMarkers.Count; i++)
            {
                var contentPos = SpineToPreview(_boneData[i].SpineWorldPos);
                var screenPos = contentPos * _zoom + _panOffset;
                _boneMarkers[i].Position = screenPos;
            }
        }

        // During playback, update the slider and frame label
        if (_isPlaying && _animController != null)
        {
            var animState = _animController.GetAnimationState();
            var current = animState.GetCurrent(0);
            if (current != null)
            {
                float duration = current.GetAnimationEnd();
                if (duration > 0f)
                {
                    float time = current.GetTrackTime() % duration;
                    _frameSlider.SetValueNoSignal(time / duration * 100.0);
                    _frameLabel.Text = $"{time:F2}s / {duration:F2}s";
                }
            }
        }

        // Always: move eyes to follow bones (works during playback AND scrubbing)
        UpdateEyePositionsFromSkeleton();
    }

    /// <summary>
    /// Moves all visible eyes to follow their resolved anchor bone.
    /// Runs every frame so eyes track during both playback and manual scrubbing.
    /// </summary>
    private void UpdateEyePositionsFromSkeleton()
    {
        if (_animController == null || _skeletonGodot == null || _spineNode == null) return;

        var animState = _animController.GetAnimationState();
        var currentEntry = animState.GetCurrent(0);
        if (currentEntry == null) return;

        float currentTime = 0f;
        float dur = currentEntry.GetAnimationEnd();
        if (dur > 0f)
            currentTime = currentEntry.GetTrackTime() % dur;

        // Update the active segment indicator
        UpdateActiveSegmentLabel(currentTime);

        foreach (var p in _placements)
        {
            if (!IsInstanceValid(p.Container)) continue;
            if (_isDragging && p == _dragging) continue;

            // Resolve visibility: check if eye should be hidden right now
            bool shouldHide = ResolveHidden(p, _currentAnimName, currentTime);

            if (shouldHide)
            {
                // In the editor, show as ghosted rather than fully invisible
                p.Container.Modulate = new Color(1, 1, 1, 0.15f);
                continue;
            }
            else
            {
                p.Container.Modulate = p.HiddenByDefault ? new Color(1, 1, 1, 0.7f) : Colors.White;
            }

            string activeBone = ResolveActiveBone(p, _currentAnimName, currentTime);

            var bone = _skeletonGodot.Call("find_bone", activeBone).AsGodotObject();
            if (bone == null) continue;

            var wx = (float)bone.Call("get_world_x");
            var wy = (float)bone.Call("get_world_y");
            var bonePos = new Vector2(wx, wy);

            Vector2 offset = ResolveSpineOffset(p, _currentAnimName, currentTime);
            Vector2 rotatedOffset = RotateByBone(offset, bone);
            var previewPos = SpineToPreview(bonePos) + rotatedOffset * _spineNode.Scale;
            p.Container.Position = previewPos;
            p.Position = previewPos;
        }
    }

    private string ResolveActiveBone(EyePlacement p, string animName, float time)
    {
        if (p.BoneTimelines.TryGetValue(animName, out var segments) && segments.Count > 0)
        {
            foreach (var seg in segments)
            {
                if (time >= seg.StartTime && time < seg.EndTime)
                    return seg.BoneName;
            }
            return segments[^1].BoneName;
        }
        return p.AnchorBone;
    }

    private Vector2 ResolveSpineOffset(EyePlacement p, string animName, float time)
    {
        if (p.BoneTimelines.TryGetValue(animName, out var segments) && segments.Count > 0)
        {
            foreach (var seg in segments)
            {
                if (time >= seg.StartTime && time < seg.EndTime)
                    return seg.HasSpineOffset ? seg.SpineOffset : p.SpineOffset;
            }
            var last = segments[^1];
            return last.HasSpineOffset ? last.SpineOffset : p.SpineOffset;
        }
        return p.HasSpineOffset ? p.SpineOffset : Vector2.Zero;
    }

    /// <summary>
    /// Determines if an eye should be hidden at the given time.
    /// - If HiddenByDefault and no segments for this anim: hidden.
    /// - If HiddenByDefault and segments exist: hidden only during Hidden segments.
    /// - If not HiddenByDefault and segments exist: hidden only during Hidden segments.
    /// - If not HiddenByDefault and no segments: visible.
    /// </summary>
    private bool ResolveHidden(EyePlacement p, string animName, float time)
    {
        if (p.BoneTimelines.TryGetValue(animName, out var segments) && segments.Count > 0)
        {
            foreach (var seg in segments)
            {
                if (time >= seg.StartTime && time < seg.EndTime)
                    return seg.Hidden;
            }
            // Past all segments: use last segment's state
            return segments[^1].Hidden;
        }
        // No segments for this animation: use default
        return p.HiddenByDefault;
    }

    /// <summary>
    /// Updates the label that shows which bone segment is active at the current scrub time.
    /// </summary>
    private void UpdateActiveSegmentLabel(float time)
    {
        if (_selectedEye == null || _activeSegmentLabel == null) return;

        if (!_selectedEye.BoneTimelines.TryGetValue(_currentAnimName, out var segments) || segments.Count == 0)
        {
            _activeSegmentLabel.Text = "";
            return;
        }

        for (int i = 0; i < segments.Count; i++)
        {
            if (time >= segments[i].StartTime && time < segments[i].EndTime)
            {
                bool isEditing = i == _editingSegmentIndex;
                var seg = segments[i];

                if (seg.Hidden)
                {
                    _activeSegmentLabel.Text = "@ " + time.ToString("F2") + "s → segment " + (i + 1) + " (hidden)";
                    ApplyFont(_activeSegmentLabel, 11, TextDim);
                }
                else
                {
                    _activeSegmentLabel.Text = "@ " + time.ToString("F2") + "s → segment " + (i + 1)
                        + " (" + seg.BoneName + ", visible)"
                        + (isEditing ? "  ← EDITING (drag eye to reposition)" : "");
                    ApplyFont(_activeSegmentLabel, 11, isEditing ? AccentWarm : AccentGreen);
                }
                return;
            }
        }

        _activeSegmentLabel.Text = "@ " + time.ToString("F2") + "s → past all segments, using last";
        ApplyFont(_activeSegmentLabel, 11, TextDim);
    }

    // ════════════════════════════════════════════════
    //  FONT HELPERS
    // ════════════════════════════════════════════════

    private void ApplyFont(Control control, int size, Color color)
    {
        if (_font != null)
            control.AddThemeFontOverride("font", _font);
        control.AddThemeFontSizeOverride("font_size", size);
        control.AddThemeColorOverride("font_color", color);
    }

    private Label MakeLabel(string text, Vector2 pos, Vector2 size, int fontSize, Color color)
    {
        var label = new Label { Text = text, Position = pos, Size = size };
        ApplyFont(label, fontSize, color);
        return label;
    }

    private Button MakeButton(string text, Vector2 pos, Vector2 size, int fontSize = 14)
    {
        var button = new Button { Text = text, Position = pos, Size = size };
        if (_font != null) button.AddThemeFontOverride("font", _font);
        button.AddThemeFontSizeOverride("font_size", fontSize);
        return button;
    }

    // ════════════════════════════════════════════════
    //  WORKFLOW BAR
    // ════════════════════════════════════════════════

    private static readonly string[] StepNames = { "1. Pick Monster", "2. Place Eyes", "3. Adjust", "4. Export" };
    private static readonly string[] StepHints =
    {
        "Start by choosing a monster from the sidebar on the left.",
        "Click anywhere on the preview to place a googly eye. Scroll over an eye to resize it. Right-click an eye to remove it.",
        "Fine-tune: select an eye to change its scale or anchor bone. Use bone segments for animations where the eye should switch bones.",
        "Happy with the placement? Hit 'Export to Console' to generate the config code you can paste into your mod."
    };

    private void BuildWorkflowBar(Vector2 screenSize)
    {
        float barX = SidebarWidth + Padding;
        float barY = Padding;
        float barWidth = screenSize.X - SidebarWidth - Padding * 2;
        float stepWidth = barWidth / 4f;

        for (int i = 0; i < 4; i++)
        {
            var stepBg = new ColorRect
            {
                Color = StepInactiveBg,
                Position = new Vector2(barX + stepWidth * i + 2, barY),
                Size = new Vector2(stepWidth - 4, 28)
            };
            AddChild(stepBg);
            _stepBgs[i] = stepBg;

            var stepLabel = MakeLabel(StepNames[i],
                new Vector2(barX + stepWidth * i + 10, barY + 4),
                new Vector2(stepWidth - 20, 20), 12, TextDim);
            AddChild(stepLabel);
            _stepLabels[i] = stepLabel;
        }

        _stepHintLabel = MakeLabel(StepHints[0],
            new Vector2(barX, barY + 32),
            new Vector2(barWidth, 20), 12, AccentWarm);
        _stepHintLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        AddChild(_stepHintLabel);
    }

    private void UpdateWorkflowStep(WorkflowStep step)
    {
        _currentStep = step;
        int idx = (int)step;
        for (int i = 0; i < 4; i++)
        {
            if (i < idx)
            {
                _stepBgs[i].Color = StepCompleteBg;
                ApplyFont(_stepLabels[i], 12, new Color(0.5f, 0.8f, 0.55f, 1f));
            }
            else if (i == idx)
            {
                _stepBgs[i].Color = StepActiveBg;
                ApplyFont(_stepLabels[i], 12, TextBright);
            }
            else
            {
                _stepBgs[i].Color = StepInactiveBg;
                ApplyFont(_stepLabels[i], 12, TextDim);
            }
        }
        _stepHintLabel.Text = StepHints[idx];
    }

    // ════════════════════════════════════════════════
    //  HELP OVERLAY
    // ════════════════════════════════════════════════

    private void BuildHelpToggle(Vector2 screenSize)
    {
        _helpToggleButton = MakeButton("? Help",
            new Vector2(screenSize.X - 100, screenSize.Y - 45),
            new Vector2(85, 34), 13);
        _helpToggleButton.Pressed += ToggleHelp;
        AddChild(_helpToggleButton);
    }

    private void BuildHelpOverlay(Vector2 screenSize)
    {
        float overlayWidth = 480f;
        float overlayHeight = 560f;
        float ox = (screenSize.X - overlayWidth) / 2f;
        float oy = (screenSize.Y - overlayHeight) / 2f;

        _helpOverlay = new Control { Position = Vector2.Zero, Size = screenSize, Visible = false };
        AddChild(_helpOverlay);

        var dimBg = new ColorRect { Color = new Color(0f, 0f, 0f, 0.6f) };
        dimBg.SetAnchorsPreset(LayoutPreset.FullRect);
        _helpOverlay.AddChild(dimBg);

        var cardBg = new ColorRect { Color = HelpBg, Position = new Vector2(ox, oy), Size = new Vector2(overlayWidth, overlayHeight) };
        _helpOverlay.AddChild(cardBg);

        float cx = ox + 20f;
        float cy = oy + 16f;

        _helpOverlay.AddChild(MakeLabel("Googly Eyes Editor — Quick Reference",
            new Vector2(cx, cy), new Vector2(overlayWidth - 40, 28), 17, TextBright));
        cy += 36f;

        string[] helpLines =
        {
            "BASICS",
            "1. Pick a monster from the sidebar.",
            "2. Click the preview to place googly eyes.",
            "3. Scroll over an eye to resize, right-click to remove.",
            "4. Middle-click + drag to pan, scroll empty area to zoom.",
            "",
            "BONE ANCHORING",
            "Each eye follows a bone (default: 'head'). Click an eye,",
            "then change the bone name in the Bone Anchoring panel.",
            "Use 'Show Bones' to see what bones are available.",
            "",
            "SWITCHING BONES MID-ANIMATION",
            "Sometimes an eye should follow different bones at",
            "different points during an animation. Here's how:",
            "",
            "1. Select an eye and pick a non-idle animation.",
            "2. Scrub the timeline to where the switch should happen.",
            "3. Click 'Split here' — this creates two segments.",
            "4. Click 'Edit' on a segment to activate it.",
            "5. Change the bone name, then drag the eye in the",
            "   preview to set its position for that segment.",
            "6. Click 'Done editing' when finished.",
            "",
            "HIDING EYES",
            "Tick 'Hidden by default' on an eye to make it invisible",
            "during idle. Then add segments on other animations and",
            "leave some visible, some with 'Hide' checked. Example:",
            "an enemy faces left (1 eye). During 'attack' it turns",
            "to face you — add a visible segment for the 2nd eye.",
            "",
            "EXPORT",
            "Hit 'Export to Console' to print the config code."
        };

        foreach (var line in helpLines)
        {
            bool isHeader = line.Length > 0 && line == line.ToUpper();
            _helpOverlay.AddChild(MakeLabel(line,
                new Vector2(cx, cy), new Vector2(overlayWidth - 40, 18),
                isHeader ? 13 : 12, isHeader ? Accent : TextNormal));
            cy += isHeader ? 22f : 16f;
        }

        var closeBtn = MakeButton("Close",
            new Vector2(ox + overlayWidth / 2f - 40f, oy + overlayHeight - 46f),
            new Vector2(80, 32), 14);
        closeBtn.Pressed += ToggleHelp;
        _helpOverlay.AddChild(closeBtn);
    }

    private void ToggleHelp()
    {
        _helpVisible = !_helpVisible;
        _helpOverlay.Visible = _helpVisible;
    }

    // ════════════════════════════════════════════════
    //  SIDEBAR
    // ════════════════════════════════════════════════

    private void BuildSidebar(Vector2 screenSize)
    {
        AddChild(new ColorRect { Color = PanelBg, Position = Vector2.Zero, Size = new Vector2(SidebarWidth, screenSize.Y) });
        AddChild(MakeLabel("Googly Eyes Editor", new Vector2(Padding, Padding), new Vector2(SidebarWidth - Padding * 2, 30), 18, TextBright));
        AddChild(MakeLabel("Pick a monster to begin.", new Vector2(Padding, 42), new Vector2(SidebarWidth - Padding * 2, 16), 11, TextDim));

        _sidebarScroll = new ScrollContainer
        {
            Position = new Vector2(0, 62),
            Size = new Vector2(SidebarWidth, screenSize.Y - 62),
            HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled
        };
        AddChild(_sidebarScroll);

        _monsterList = new VBoxContainer { CustomMinimumSize = new Vector2(SidebarWidth - 20, 0) };
        _monsterList.AddThemeConstantOverride("separation", 2);
        _sidebarScroll.AddChild(_monsterList);
    }

    // ════════════════════════════════════════════════
    //  PREVIEW PANEL
    // ════════════════════════════════════════════════

    private void BuildPreviewPanel(Vector2 screenSize)
    {
        float previewX = SidebarWidth + Padding;
        float previewTop = Padding + 58f;
        float previewWidth = screenSize.X - SidebarWidth - Padding * 2;
        float previewHeight = screenSize.Y - 420 - 58f;

        AddChild(new ColorRect { Color = PreviewBg, Position = new Vector2(previewX, previewTop), Size = new Vector2(previewWidth, previewHeight) });

        _previewArea = new Control
        {
            Position = new Vector2(previewX, previewTop),
            Size = new Vector2(previewWidth, previewHeight),
            ClipContents = true
        };
        AddChild(_previewArea);

        _previewRoot = new Node2D();
        _previewArea.AddChild(_previewRoot);

        _infoLabel = MakeLabel("Select a monster from the sidebar to get started.",
            new Vector2(previewX + 10, previewTop + 5), new Vector2(previewWidth - 20, 30), 13, TextDim);
        AddChild(_infoLabel);
    }

    // ════════════════════════════════════════════════
    //  ANIMATION CONTROLS
    // ════════════════════════════════════════════════

    private void BuildAnimationControls(Vector2 screenSize)
    {
        float panelX = SidebarWidth + Padding;
        float panelY = screenSize.Y - 415;
        float panelWidth = screenSize.X - SidebarWidth - Padding * 2;

        AddChild(new ColorRect { Color = PanelBg, Position = new Vector2(panelX, panelY), Size = new Vector2(panelWidth, 55) });
        AddChild(MakeLabel("Animation", new Vector2(panelX + 10, panelY + 4), new Vector2(100, 16), 11, Accent));
        _animPanelHint = MakeLabel("Scrub or play to preview. Eyes follow their bones in real time.",
            new Vector2(panelX + 120, panelY + 4), new Vector2(panelWidth - 140, 16), 10, TextDim);
        AddChild(_animPanelHint);

        float row = panelY + 22;

        _animDropdown = new OptionButton { Position = new Vector2(panelX + 10, row), Size = new Vector2(150, 30) };
        if (_font != null) _animDropdown.AddThemeFontOverride("font", _font);
        _animDropdown.AddThemeFontSizeOverride("font_size", 12);
        _animDropdown.ItemSelected += OnAnimationSelected;
        AddChild(_animDropdown);

        _frameSlider = new HSlider
        {
            Position = new Vector2(panelX + 170, row + 2),
            Size = new Vector2(200, 26),
            MinValue = 0, MaxValue = 100, Step = 0.1, Value = 0
        };
        _frameSlider.ValueChanged += OnFrameSliderChanged;
        AddChild(_frameSlider);

        _frameLabel = MakeLabel("0.00s / 0.00s",
            new Vector2(panelX + 380, row + 4), new Vector2(120, 24), 12, TextDim);
        AddChild(_frameLabel);

        _playPauseButton = MakeButton("Play", new Vector2(panelX + 510, row), new Vector2(60, 30), 12);
        _playPauseButton.Pressed += TogglePlayPause;
        AddChild(_playPauseButton);
    }

    // ════════════════════════════════════════════════
    //  TOOLBAR
    // ════════════════════════════════════════════════

    private void BuildToolbar(Vector2 screenSize)
    {
        float y = screenSize.Y - 350;
        float x = SidebarWidth + Padding;

        _clearButton = MakeButton("Clear All", new Vector2(x, y), new Vector2(100, 34), 12);
        _clearButton.TooltipText = "Remove every placed eye.";
        _clearButton.Pressed += ClearAllPlacements;
        AddChild(_clearButton);

        _undoButton = MakeButton("Undo", new Vector2(x + 110, y), new Vector2(70, 34), 12);
        _undoButton.TooltipText = "Remove the last placed eye.";
        _undoButton.Pressed += UndoLastPlacement;
        AddChild(_undoButton);

        _exportButton = MakeButton("Export to Console", new Vector2(x + 190, y), new Vector2(150, 34), 12);
        _exportButton.TooltipText = "Print config code to Godot's output console.";
        _exportButton.Pressed += ExportConfig;
        AddChild(_exportButton);

        _outputLabel = MakeLabel("", new Vector2(x + 360, y + 5), new Vector2(400, 28), 12, Accent);
        AddChild(_outputLabel);
    }

    // ════════════════════════════════════════════════
    //  SELECTION PANEL
    // ════════════════════════════════════════════════

    private void BuildSelectionPanel(Vector2 screenSize)
    {
        float panelX = SidebarWidth + Padding;
        float panelY = screenSize.Y - 305;
        float panelWidth = screenSize.X - SidebarWidth - Padding * 2;

        AddChild(new ColorRect { Color = PanelBg, Position = new Vector2(panelX, panelY), Size = new Vector2(panelWidth, 55) });
        AddChild(MakeLabel("Selected Eye", new Vector2(panelX + 10, panelY + 4), new Vector2(100, 16), 11, Accent));
        _selectionPanelHint = MakeLabel("Click an eye to select it. Adjust scale or duplicate.",
            new Vector2(panelX + 120, panelY + 4), new Vector2(panelWidth - 140, 16), 10, TextDim);
        AddChild(_selectionPanelHint);

        float row = panelY + 24;

        _selectedLabel = MakeLabel("No eye selected", new Vector2(panelX + 10, row + 3), new Vector2(120, 24), 13, TextDim);
        AddChild(_selectedLabel);

        AddChild(MakeLabel("Scale:", new Vector2(panelX + 130, row + 3), new Vector2(50, 24), 13, TextNormal));

        _scaleInput = new SpinBox
        {
            MinValue = 0.01, MaxValue = 5.0, Step = 0.01, Value = 1.0,
            Position = new Vector2(panelX + 180, row - 2), Size = new Vector2(100, 30),
            Editable = true, TooltipText = "Eye size multiplier."
        };
        if (_font != null) _scaleInput.AddThemeFontOverride("font", _font);
        _scaleInput.AddThemeFontSizeOverride("font_size", 12);
        _scaleInput.ValueChanged += OnScaleInputChanged;
        AddChild(_scaleInput);

        _duplicateButton = MakeButton("Duplicate", new Vector2(panelX + 300, row - 2), new Vector2(100, 30), 12);
        _duplicateButton.TooltipText = "Copy this eye (including bone segments).";
        _duplicateButton.Pressed += DuplicateSelectedEye;
        AddChild(_duplicateButton);

        _hiddenByDefaultCheckbox = new CheckBox
        {
            Text = "Hidden by default",
            Position = new Vector2(panelX + 420, row - 2),
            Size = new Vector2(160, 30),
            TooltipText = "Eye is invisible unless a non-hidden bone segment makes it appear. Use for eyes that only show during certain animations."
        };
        ApplyFont(_hiddenByDefaultCheckbox, 11, TextNormal);
        _hiddenByDefaultCheckbox.Toggled += (pressed) =>
        {
            if (_selectedEye != null)
            {
                _selectedEye.HiddenByDefault = pressed;
                // In idle with no segments, hide/show immediately
                _selectedEye.Container.Modulate = pressed ? new Color(1, 1, 1, 0.25f) : Colors.White;
            }
        };
        AddChild(_hiddenByDefaultCheckbox);
    }

    // ════════════════════════════════════════════════
    //  BONE PANEL (default bone + per-anim segments)
    // ════════════════════════════════════════════════

    private void BuildBonePanel(Vector2 screenSize)
    {
        float panelX = SidebarWidth + Padding;
        float panelY = screenSize.Y - 240;
        float panelWidth = screenSize.X - SidebarWidth - Padding * 2;
        float panelHeight = 235f;

        AddChild(new ColorRect { Color = PanelBg, Position = new Vector2(panelX, panelY), Size = new Vector2(panelWidth, panelHeight) });

        // ── Title row ──
        AddChild(MakeLabel("Bone Anchoring", new Vector2(panelX + 10, panelY + 4), new Vector2(120, 16), 11, Accent));
        _anchorPanelHint = MakeLabel("Which bone this eye follows. Most eyes only need the default.",
            new Vector2(panelX + 140, panelY + 4), new Vector2(panelWidth - 160, 16), 10, TextDim);
        AddChild(_anchorPanelHint);

        // ── Row 2: Default bone + Show Bones ──
        float row2 = panelY + 26;

        AddChild(MakeLabel("Default bone:", new Vector2(panelX + 10, row2 + 3), new Vector2(100, 24), 12, TextNormal));

        _anchorBoneInput = new LineEdit
        {
            Text = "head", Position = new Vector2(panelX + 112, row2 - 2), Size = new Vector2(130, 30),
            TooltipText = "The bone this eye follows during idle and any animation without segments."
        };
        if (_font != null) _anchorBoneInput.AddThemeFontOverride("font", _font);
        _anchorBoneInput.AddThemeFontSizeOverride("font_size", 12);
        _anchorBoneInput.TextChanged += (text) =>
        {
            _selectedAnchorBone = text;
            if (_selectedEye != null) _selectedEye.AnchorBone = text;
        };
        AddChild(_anchorBoneInput);

        _showBonesButton = MakeButton("Show Bones", new Vector2(panelX + 255, row2 - 2), new Vector2(105, 30), 11);
        _showBonesButton.TooltipText = "Show bone markers on the preview. Green = bone, red = current default.";
        _showBonesButton.Pressed += ToggleBoneMarkers;
        AddChild(_showBonesButton);

        _showAllBoneNamesCheckbox = new CheckBox
        {
            Text = "All names", Position = new Vector2(panelX + 370, row2 - 2), Size = new Vector2(100, 30),
            TooltipText = "Label every bone, not just head/face/eye."
        };
        ApplyFont(_showAllBoneNamesCheckbox, 11, TextNormal);
        _showAllBoneNamesCheckbox.Toggled += (p) => { if (_showingBones) RebuildBoneMarkerNodes(); };
        AddChild(_showAllBoneNamesCheckbox);

        _anchorPosLabel = MakeLabel("", new Vector2(panelX + 480, row2 + 3), new Vector2(300, 24), 11, Accent);
        AddChild(_anchorPosLabel);

        // ── Divider ──
        float divY = row2 + 34;
        AddChild(new ColorRect { Color = new Color(0.2f, 0.2f, 0.28f, 0.6f), Position = new Vector2(panelX + 10, divY), Size = new Vector2(panelWidth - 20, 1) });

        // ── Segment header ──
        float segHdrY = divY + 6;
        AddChild(MakeLabel("Per-animation bone switching", new Vector2(panelX + 10, segHdrY), new Vector2(220, 16), 11, AccentWarm));
        AddChild(MakeLabel("Only needed if an eye must follow different bones at different times during one animation.",
            new Vector2(panelX + 240, segHdrY), new Vector2(panelWidth - 260, 16), 10, TextDim));

        // ── Segment buttons ──
        float btnY = segHdrY + 20;

        _splitHereButton = MakeButton("Split here", new Vector2(panelX + 10, btnY), new Vector2(100, 26), 11);
        _splitHereButton.TooltipText = "Split the timeline at the current scrub position, creating a new bone segment.";
        _splitHereButton.Pressed += SplitAtScrubPosition;
        AddChild(_splitHereButton);

        _clearSegmentsButton = MakeButton("Clear segments", new Vector2(panelX + 120, btnY), new Vector2(115, 26), 11);
        _clearSegmentsButton.TooltipText = "Remove all segments for this animation. The eye goes back to using its default bone.";
        _clearSegmentsButton.Pressed += ClearSegmentsForSelected;
        AddChild(_clearSegmentsButton);

        _activeSegmentLabel = MakeLabel("", new Vector2(panelX + 250, btnY + 3), new Vector2(panelWidth - 270, 20), 11, TextDim);
        AddChild(_activeSegmentLabel);

        // ── Scrollable segment list ──
        float listY = btnY + 30;
        float listHeight = panelY + panelHeight - listY - 4;

        _segmentScroll = new ScrollContainer
        {
            Position = new Vector2(panelX + 4, listY),
            Size = new Vector2(panelWidth - 8, listHeight),
            HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled
        };
        AddChild(_segmentScroll);

        _segmentList = new VBoxContainer { CustomMinimumSize = new Vector2(panelWidth - 24, 0) };
        _segmentList.AddThemeConstantOverride("separation", 2);
        _segmentScroll.AddChild(_segmentList);
    }

    // ════════════════════════════════════════════════
    //  SEGMENT UI LOGIC
    // ════════════════════════════════════════════════

    private void RefreshSegmentUI()
    {
        // Clear old rows
        foreach (var child in _segmentList.GetChildren())
            if (child is Node n && IsInstanceValid(n))
                n.QueueFree();

        if (_selectedEye == null)
        {
            _segmentList.AddChild(MakeLabel("Select an eye first.", Vector2.Zero, new Vector2(600, 20), 11, TextDim));
            return;
        }

        if (_currentAnimName == "idle_loop")
        {
            _segmentList.AddChild(MakeLabel("Switch to a non-idle animation to set up bone segments.",
                Vector2.Zero, new Vector2(600, 20), 11, TextDim));
            return;
        }

        if (!_selectedEye.BoneTimelines.TryGetValue(_currentAnimName, out var segments) || segments.Count == 0)
        {
            string hintText;
            if (_selectedEye.HiddenByDefault)
            {
                hintText = "This eye is hidden by default — it won't appear during this animation yet.\n"
                    + "To make it appear partway through: scrub to where it should show up, click 'Split here', "
                    + "then toggle 'Visible' on the segment where it should appear.";
            }
            else
            {
                hintText = "No segments — eye follows default bone ('" + _selectedEye.AnchorBone + "') for the whole animation.\n"
                    + "To switch bones mid-animation: scrub to the switch point, click 'Split here'.";
            }
            var hint = MakeLabel(hintText, Vector2.Zero, new Vector2(600, 50), 11, TextDim);
            hint.AutowrapMode = TextServer.AutowrapMode.WordSmart;
            _segmentList.AddChild(hint);
            return;
        }

        for (int i = 0; i < segments.Count; i++)
        {
            var seg = segments[i];
            int idx = i;
            bool isEditing = i == _editingSegmentIndex;

            var row = new HBoxContainer();
            row.AddThemeConstantOverride("separation", 4);

            // Background highlight for the row being edited
            if (isEditing)
            {
                var rowBg = new ColorRect { Color = SegmentActiveBg, CustomMinimumSize = new Vector2(4, 0) };
                row.AddChild(rowBg);
            }

            // Index
            var idxLbl = new Label { Text = (i + 1) + ".", CustomMinimumSize = new Vector2(20, 0) };
            ApplyFont(idxLbl, 12, isEditing ? AccentWarm : TextDim);
            row.AddChild(idxLbl);

            // Time range (read-only display)
            var timeLbl = new Label
            {
                Text = seg.StartTime.ToString("F2") + "s – " + seg.EndTime.ToString("F2") + "s",
                CustomMinimumSize = new Vector2(120, 0)
            };
            ApplyFont(timeLbl, 11, TextNormal);
            row.AddChild(timeLbl);

            // Bone name (dimmed if hidden)
            var boneEdit = new LineEdit
            {
                Text = seg.BoneName,
                CustomMinimumSize = new Vector2(100, 0),
                Editable = isEditing && !seg.Hidden,
                TooltipText = seg.Hidden ? "Eye is hidden during this segment." : (isEditing ? "Type the bone name for this segment." : "Click 'Edit' to change.")
            };
            if (_font != null) boneEdit.AddThemeFontOverride("font", _font);
            boneEdit.AddThemeFontSizeOverride("font_size", 11);
            if (isEditing && !seg.Hidden)
            {
                boneEdit.TextChanged += (text) => { seg.BoneName = text; };
            }
            if (seg.Hidden) boneEdit.Modulate = new Color(1, 1, 1, 0.4f);
            row.AddChild(boneEdit);

            // Visible toggle (inverted from Hidden — more intuitive)
            var visCheck = new CheckBox
            {
                Text = "Visible",
                ButtonPressed = !seg.Hidden,
                CustomMinimumSize = new Vector2(72, 0),
                TooltipText = seg.Hidden
                    ? "Eye is hidden during this segment. Check to make it appear."
                    : "Eye is visible during this segment. Uncheck to hide it."
            };
            ApplyFont(visCheck, 10, seg.Hidden ? TextDim : AccentGreen);
            visCheck.Toggled += (pressed) =>
            {
                seg.Hidden = !pressed;
                RefreshSegmentUI();
            };
            row.AddChild(visCheck);

            // Edit / Done button (only relevant for visible segments)
            if (!seg.Hidden)
            {
                if (isEditing)
                {
                    var doneBtn = new Button { Text = "Done editing", CustomMinimumSize = new Vector2(95, 0), TooltipText = "Finish editing this segment." };
                    if (_font != null) doneBtn.AddThemeFontOverride("font", _font);
                    doneBtn.AddThemeFontSizeOverride("font_size", 11);
                    doneBtn.Pressed += () => { StopEditingSegment(); };
                    row.AddChild(doneBtn);
                }
                else
                {
                    var editBtn = new Button { Text = "Edit", CustomMinimumSize = new Vector2(45, 0), TooltipText = "Activate this segment: change its bone and drag the eye to set its offset." };
                    if (_font != null) editBtn.AddThemeFontOverride("font", _font);
                    editBtn.AddThemeFontSizeOverride("font_size", 11);
                    editBtn.Pressed += () => { StartEditingSegment(idx); };
                    row.AddChild(editBtn);
                }
            }

            // Remove button
            var removeBtn = new Button { Text = "X", CustomMinimumSize = new Vector2(26, 0), TooltipText = "Remove this segment." };
            if (_font != null) removeBtn.AddThemeFontOverride("font", _font);
            removeBtn.AddThemeFontSizeOverride("font_size", 11);
            removeBtn.Pressed += () => { RemoveSegment(idx); };
            row.AddChild(removeBtn);

            // Offset indicator (only for visible segments)
            if (!seg.Hidden)
            {
                var offsetLbl = new Label
                {
                    Text = seg.HasSpineOffset ? "offset set" : "no offset",
                    CustomMinimumSize = new Vector2(70, 0)
                };
                ApplyFont(offsetLbl, 10, seg.HasSpineOffset ? AccentGreen : TextDim);
                row.AddChild(offsetLbl);
            }

            _segmentList.AddChild(row);
        }
    }

    private void SplitAtScrubPosition()
    {
        if (_selectedEye == null)
        {
            _outputLabel.Text = "Select an eye first.";
            return;
        }
        if (_currentAnimName == "idle_loop")
        {
            _outputLabel.Text = "Switch to a non-idle animation first.";
            return;
        }

        float duration = GetCurrentAnimDuration();
        if (duration <= 0f) return;

        float splitTime = GetCurrentScrubTime();

        if (!_selectedEye.BoneTimelines.ContainsKey(_currentAnimName))
            _selectedEye.BoneTimelines[_currentAnimName] = new List<BoneSegment>();

        var segments = _selectedEye.BoneTimelines[_currentAnimName];

        if (segments.Count == 0)
        {
            // First split: creates two segments
            if (splitTime <= 0.01f || splitTime >= duration - 0.01f)
            {
                _outputLabel.Text = "Scrub to a point between the start and end of the animation before splitting.";
                return;
            }

            bool inheritHidden = _selectedEye.HiddenByDefault;

            segments.Add(new BoneSegment
            {
                StartTime = 0f,
                EndTime = splitTime,
                BoneName = _selectedEye.AnchorBone,
                Hidden = inheritHidden
            });
            segments.Add(new BoneSegment
            {
                StartTime = splitTime,
                EndTime = duration,
                BoneName = _selectedEye.AnchorBone,
                Hidden = inheritHidden
            });

            // Cache default offsets for both
            CacheSegmentOffset(segments[0]);
            CacheSegmentOffset(segments[1]);

            if (inheritHidden)
                _outputLabel.Text = "Split at " + splitTime.ToString("F2") + "s — two hidden segments created. Toggle 'Visible' on the segment where this eye should appear.";
            else
                _outputLabel.Text = "Split at " + splitTime.ToString("F2") + "s — two segments created. Click 'Edit' to set bone and drag to position.";
        }
        else
        {
            // Split an existing segment at the scrub point
            for (int i = 0; i < segments.Count; i++)
            {
                var seg = segments[i];
                if (splitTime > seg.StartTime + 0.01f && splitTime < seg.EndTime - 0.01f)
                {
                    var newSeg = new BoneSegment
                    {
                        StartTime = splitTime,
                        EndTime = seg.EndTime,
                        BoneName = seg.BoneName,
                        Hidden = seg.Hidden
                    };
                    seg.EndTime = splitTime;

                    segments.Insert(i + 1, newSeg);
                    CacheSegmentOffset(seg);
                    CacheSegmentOffset(newSeg);

                    _outputLabel.Text = "Split segment " + (i + 1) + " at " + splitTime.ToString("F2") + "s.";
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
        if (!_selectedEye.BoneTimelines.TryGetValue(_currentAnimName, out var segments)) return;
        if (index < 0 || index >= segments.Count) return;

        _editingSegmentIndex = index;

        // Scrub to the midpoint of this segment so the user sees the pose
        var seg = segments[index];
        float mid = (seg.StartTime + seg.EndTime) / 2f;
        float duration = GetCurrentAnimDuration();
        if (duration > 0f)
        {
            _isPlaying = false;
            _playPauseButton.Text = "Play";
            SetAnimationPaused(_currentAnimName, mid / duration);
        }

        _infoLabel.Text = "EDITING segment " + (index + 1) + ": drag the eye to set its offset from '" + seg.BoneName + "', then click 'Done editing'.";

        RefreshSegmentUI();
    }

    private void StopEditingSegment()
    {
        if (_selectedEye != null && _editingSegmentIndex >= 0)
        {
            // Cache the current offset
            if (_selectedEye.BoneTimelines.TryGetValue(_currentAnimName, out var segments)
                && _editingSegmentIndex < segments.Count)
            {
                CacheSegmentOffsetFromCurrentPose(segments[_editingSegmentIndex]);
            }
        }

        _editingSegmentIndex = -1;
        _infoLabel.Text = _placements.Count + " eye(s) placed.";
        RefreshSegmentUI();
    }

    private void RemoveSegment(int index)
    {
        if (_selectedEye == null) return;
        if (!_selectedEye.BoneTimelines.TryGetValue(_currentAnimName, out var segments)) return;
        if (index < 0 || index >= segments.Count) return;

        if (_editingSegmentIndex == index) _editingSegmentIndex = -1;
        else if (_editingSegmentIndex > index) _editingSegmentIndex--;

        // If removing creates a gap, extend the neighbor
        if (segments.Count > 1)
        {
            if (index == 0)
                segments[1].StartTime = segments[0].StartTime;
            else if (index == segments.Count - 1)
                segments[index - 1].EndTime = segments[index].EndTime;
            else
                segments[index - 1].EndTime = segments[index].EndTime;
        }

        segments.RemoveAt(index);

        if (segments.Count == 1)
        {
            // Only one segment left — remove the timeline entirely, back to default
            _selectedEye.BoneTimelines.Remove(_currentAnimName);
            _editingSegmentIndex = -1;
            _outputLabel.Text = "Only one segment remained — removed it. Eye uses default bone.";
        }
        else if (segments.Count == 0)
        {
            _selectedEye.BoneTimelines.Remove(_currentAnimName);
            _editingSegmentIndex = -1;
        }

        RefreshSegmentUI();
    }

    private void ClearSegmentsForSelected()
    {
        if (_selectedEye == null) return;
        _selectedEye.BoneTimelines.Remove(_currentAnimName);
        _editingSegmentIndex = -1;
        RefreshSegmentUI();
        _outputLabel.Text = "Cleared segments for " + _currentAnimName + ". Eye uses default bone.";
    }

    /// <summary>
    /// Cache the spine offset for a segment by posing to its midpoint and reading the bone position.
    /// Uses the eye's current preview position.
    /// </summary>
    private void CacheSegmentOffset(BoneSegment seg)
    {
        if (_selectedEye == null || _skeletonGodot == null || _spineNode == null) return;

        float mid = (seg.StartTime + seg.EndTime) / 2f;
        float dur = GetCurrentAnimDuration();
        if (dur <= 0f) return;

        var savedAnim = _currentAnimName;
        var savedSlider = _frameSlider.Value;

        PoseSkeletonAt(_currentAnimName, mid);

        var bone = _skeletonGodot.Call("find_bone", seg.BoneName).AsGodotObject();
        if (bone != null)
        {
            var bx = (float)bone.Call("get_world_x");
            var by = (float)bone.Call("get_world_y");
            var bonePreview = SpineToPreview(new Vector2(bx, by));
            var worldOffset = (_selectedEye.Position - bonePreview) / _spineNode.Scale;
            seg.SpineOffset = UnrotateByBone(worldOffset, bone);
            seg.HasSpineOffset = true;
        }
        else
        {
            seg.HasSpineOffset = false;
        }

        SetAnimationPaused(savedAnim, (float)(savedSlider / 100.0));
    }

    /// <summary>
    /// Cache offset from the current pose (used when user clicks "Done editing" after dragging).
    /// Doesn't restore pose because we're already at the right frame.
    /// </summary>
    private void CacheSegmentOffsetFromCurrentPose(BoneSegment seg)
    {
        if (_selectedEye == null || _skeletonGodot == null || _spineNode == null) return;

        var bone = _skeletonGodot.Call("find_bone", seg.BoneName).AsGodotObject();
        if (bone != null)
        {
            var bx = (float)bone.Call("get_world_x");
            var by = (float)bone.Call("get_world_y");
            var bonePreview = SpineToPreview(new Vector2(bx, by));
            var worldOffset = (_selectedEye.Position - bonePreview) / _spineNode.Scale;
            seg.SpineOffset = UnrotateByBone(worldOffset, bone);
            seg.HasSpineOffset = true;
        }
        else
        {
            seg.HasSpineOffset = false;
        }
    }

    /// <summary>Pose the skeleton at an absolute time (not normalized).</summary>
    private void PoseSkeletonAt(string animName, float time)
    {
        if (_animController == null) return;
        var animState = _animController.GetAnimationState();
        var entry = animState.SetAnimation(animName, false);
        if (entry == null) return;
        entry.SetTimeScale(0f);
        entry.SetTrackTime(time);
        animState.Update(0f);
        animState.Apply(_animController.GetSkeleton());
    }

    private float GetCurrentAnimDuration()
    {
        if (_animController == null) return 0f;
        var animState = _animController.GetAnimationState();
        var current = animState.GetCurrent(0);
        return current?.GetAnimationEnd() ?? 0f;
    }

    private float GetCurrentScrubTime()
    {
        float normalized = (float)(_frameSlider.Value / 100.0);
        return normalized * GetCurrentAnimDuration();
    }

    // ════════════════════════════════════════════════
    //  BACK BUTTON
    // ════════════════════════════════════════════════

    private void BuildBackButton()
    {
        var backButtonScene = ResourceLoader.Load<PackedScene>("res://scenes/ui/back_button.tscn");
        var backButton = backButtonScene.Instantiate<NBackButton>();
        backButton.Name = "BackButton";
        AddChild(backButton);
        ConnectSignals();
        var backButtonField = typeof(NSubmenu).GetField("_backButton", BindingFlags.NonPublic | BindingFlags.Instance);
        (backButtonField?.GetValue(this) as NBackButton)?.Enable();
    }

    // ════════════════════════════════════════════════
    //  MONSTER LIST
    // ════════════════════════════════════════════════

    private void BuildMonsterList()
    {
        foreach (MonsterModel monster in ModelDb.Monsters.OrderBy(m => m.Id.Entry))
        {
            var button = new Button
            {
                Text = monster.Id.Entry, Flat = true,
                Alignment = HorizontalAlignment.Left,
                CustomMinimumSize = new Vector2(0, 32),
                SizeFlagsHorizontal = SizeFlags.ExpandFill
            };
            ApplyFont(button, 14, TextNormal);
            button.AddThemeColorOverride("font_hover_color", TextBright);
            var monsterId = monster.Id.Entry;
            var monsterRef = monster;
            button.Pressed += () => SelectMonster(monsterRef, monsterId);
            _monsterList.AddChild(button);
        }
    }

    // ════════════════════════════════════════════════
    //  MONSTER SELECTION
    // ════════════════════════════════════════════════

    private void SelectMonster(MonsterModel monster, string monsterId)
    {
        ClearAllPlacements();
        ClearBoneMarkers();
        _showingBones = false;
        _showBonesButton.Text = "Show Bones";
        _skeletonGodot = null;
        _spineNode = null;
        _currentMonsterId = monsterId;
        _isPlaying = false;
        _playPauseButton.Text = "Play";
        _zoom = 1f;
        _panOffset = Vector2.Zero;
        _editingSegmentIndex = -1;
        ApplyZoomPan();

        if (_currentVisuals != null && IsInstanceValid(_currentVisuals))
        {
            _currentVisuals.QueueFree();
            _currentVisuals = null;
        }

        _currentVisuals = monster.CreateVisuals();
        _previewRoot.AddChild(_currentVisuals);
        _currentVisuals.Position = new Vector2(
            _previewArea.Size.X / 2,
            _previewArea.Size.Y / 2 + _currentVisuals.Bounds.Size.Y * 0.25f
        );

        _availableAnims.Clear();
        _animDropdown.Clear();

        if (_currentVisuals.HasSpineAnimation)
        {
            _animController = _currentVisuals.SpineBody;
            var skeleton = _animController.GetSkeleton();
            if (skeleton != null)
            {
                _skeletonGodot = skeleton.BoundObject as GodotObject;
                _spineNode = _animController.BoundObject as Node2D;
            }
            PopulateAnimationList();
            SetAnimationPaused("idle_loop", 0f);
        }

        _infoLabel.Text = monsterId + " loaded — click the preview to place eyes!";
        UpdateWorkflowStep(WorkflowStep.PlaceEyes);

        // Load existing config from registry if available
        if (LoadPresetsForMonster(monsterId))
        {
            _infoLabel.Text = monsterId + " loaded with " + _placements.Count + " preset eye(s) from registry. Adjust as needed.";
            if (_placements.Count > 0)
                UpdateWorkflowStep(WorkflowStep.AdjustEyes);
        }

        RefreshSegmentUI();
    }

    // ════════════════════════════════════════════════
    //  PRESET LOADING FROM REGISTRY
    // ════════════════════════════════════════════════

    /// <summary>
    /// If GooglyEyesRegistry has configs for this monster, recreate all
    /// EyePlacements from the stored data. Returns true if presets were loaded.
    /// </summary>
    private bool LoadPresetsForMonster(string monsterId)
    {
        if (!GooglyEyesRegistry.Configs.TryGetValue(monsterId, out var configs)) return false;
        if (configs.Length == 0) return false;
        if (_skeletonGodot == null || _spineNode == null) return false;

        // Make sure we're in idle pose so bone positions are consistent with how
        // the default offsets were originally authored
        SetAnimationPaused("idle_loop", 0f);

        float spineScale = _spineNode.Scale.X;

        foreach (var config in configs)
        {
            // Find the anchor bone to compute preview position
            var anchorBone = _skeletonGodot.Call("find_bone", config.AnchorBone).AsGodotObject();
            if (anchorBone == null)
            {
                GD.PrintErr("[GooglyEyes] Preset load: bone '" + config.AnchorBone + "' not found, skipping eye.");
                continue;
            }

            var bx = (float)anchorBone.Call("get_world_x");
            var by = (float)anchorBone.Call("get_world_y");
            var bonePreviewPos = SpineToPreview(new Vector2(bx, by));

            // Convert bone-local offset back to preview position (rotating by bone's world rotation)
            Vector2 rotatedOffset = RotateByBone(config.Offset, anchorBone);
            var previewPos = bonePreviewPos + rotatedOffset * _spineNode.Scale;

            // Create the eye visuals
            var container = new Node2D { Position = previewPos };
            container.AddChild(new Sprite2D { Texture = _eyeTexture, Name = "Eye" });
            container.AddChild(new Sprite2D { Texture = _irisTexture, Name = "Iris", ZIndex = 1 });
            _previewRoot.AddChild(container);

            // Scale: config stores scale divided by spineScale, so reverse that
            float editorScale = config.Scale * spineScale;
            container.Scale = Vector2.One * editorScale;

            var placement = new EyePlacement
            {
                Position = previewPos,
                Scale = editorScale,
                AnchorBone = config.AnchorBone,
                EyeSprite = container.GetChild<Sprite2D>(0),
                IrisSprite = container.GetChild<Sprite2D>(1),
                Container = container,
                HiddenByDefault = config.HiddenByDefault,
                SpineOffset = config.Offset,
                HasSpineOffset = true
            };

            // If hidden by default, ghost it in the editor
            if (config.HiddenByDefault)
                container.Modulate = new Color(1, 1, 1, 0.25f);

            // Convert BoneSegments to editor BoneTimelines
            if (config.BoneSegments != null)
            {
                foreach (var kvp in config.BoneSegments)
                {
                    if (kvp.Value == null || kvp.Value.Length == 0) continue;

                    var editorSegments = new List<BoneSegment>();
                    foreach (var src in kvp.Value)
                    {
                        var editorSeg = new BoneSegment
                        {
                            StartTime = src.StartTime,
                            EndTime = src.EndTime,
                            BoneName = src.BoneName ?? config.AnchorBone,
                            Hidden = src.Hidden
                        };

                        // Compute the preview-space offset for this segment
                        // by posing to the segment's midpoint and reading the bone
                        if (!src.Hidden)
                        {
                            editorSeg.SpineOffset = src.Offset;
                            editorSeg.HasSpineOffset = true;
                        }

                        editorSegments.Add(editorSeg);
                    }

                    placement.BoneTimelines[kvp.Key] = editorSegments;
                }
            }

            _placements.Add(placement);
        }

        return _placements.Count > 0;
    }

    // ════════════════════════════════════════════════
    //  ANIMATION HELPERS
    // ════════════════════════════════════════════════

    private void PopulateAnimationList()
    {
        _animDropdown.Clear();
        _availableAnims.Clear();
        string[] commonAnims = { "idle_loop", "hurt", "attack", "cast", "dead", "die", "revive", "idle", "hit", "spawn", "entrance", "taunt", "buff", "debuff", "charge_up", "attack_heavy" };
        foreach (var animName in commonAnims)
        {
            if (_animController.HasAnimation(animName))
            {
                _availableAnims.Add(animName);
                _animDropdown.AddItem(animName);
            }
        }
        if (_availableAnims.Count > 0) _animDropdown.Selected = 0;
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

    private void SetAnimationPaused(string animName, float normalizedTime)
    {
        if (_animController == null) return;
        _currentAnimName = animName;
        var animState = _animController.GetAnimationState();
        var entry = animState.SetAnimation(animName, false);
        if (entry == null) return;
        entry.SetTimeScale(0f);
        float duration = entry.GetAnimationEnd();
        float time = normalizedTime * duration;
        entry.SetTrackTime(time);
        animState.Update(0f);
        animState.Apply(_animController.GetSkeleton());
        _frameLabel.Text = $"{time:F2}s / {duration:F2}s";
        if (_showingBones) RefreshBonePositions();
    }

    private void CacheSpineOffset(EyePlacement p)
    {
        if (_skeletonGodot == null || _spineNode == null || string.IsNullOrEmpty(p.AnchorBone))
        { p.HasSpineOffset = false; return; }
        var anchorBone = _skeletonGodot.Call("find_bone", p.AnchorBone).AsGodotObject();
        if (anchorBone == null) { p.HasSpineOffset = false; return; }
        var ax = (float)anchorBone.Call("get_world_x");
        var ay = (float)anchorBone.Call("get_world_y");
        var anchorPreviewPos = SpineToPreview(new Vector2(ax, ay));
        var worldOffset = (p.Position - anchorPreviewPos) / _spineNode.Scale;
        p.SpineOffset = UnrotateByBone(worldOffset, anchorBone);
        p.HasSpineOffset = true;
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
        var animState = _animController.GetAnimationState();
        var current = animState.GetCurrent(0);
        if (current == null) return;
        if (_isPlaying) { current.SetTimeScale(1f); current.SetLoop(true); }
        else { current.SetTimeScale(0f); }
    }

    // ════════════════════════════════════════════════
    //  BONE MARKERS
    // ════════════════════════════════════════════════

    private void ToggleBoneMarkers()
    {
        if (_showingBones) { ClearBoneMarkers(); _showBonesButton.Text = "Show Bones"; _showingBones = false; }
        else { ShowBoneMarkers(); _showBonesButton.Text = "Hide Bones"; _showingBones = true; }
    }

    private void ShowBoneMarkers()
    {
        ClearBoneMarkers();
        if (_skeletonGodot == null && _animController != null)
        {
            var skeleton = _animController.GetSkeleton();
            if (skeleton != null) { _skeletonGodot = skeleton.BoundObject as GodotObject; _spineNode = _animController.BoundObject as Node2D; }
        }
        if (_skeletonGodot == null) return;
        RefreshBonePositions();
        RebuildBoneMarkerNodes();
    }

    private void RefreshBonePositions()
    {
        _boneData.Clear();
        if (_skeletonGodot == null) return;
        var allBones = _skeletonGodot.Call("get_bones").AsGodotArray();
        foreach (var bv in allBones)
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

    private void RebuildBoneMarkerNodes()
    {
        foreach (var m in _boneMarkers) if (IsInstanceValid(m)) m.QueueFree();
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
                || info.Name.Contains("head") || info.Name.Contains("neck") || info.Name.Contains("eye")
                || info.Name.Contains("face") || info.Name.Contains("jaw") || info.Name == _selectedAnchorBone)
            {
                marker.AddChild(MakeLabel(info.Name, new Vector2(6, -8), new Vector2(150, 16), 10,
                    isAnchor ? new Color(1f, 0.3f, 0.3f) : TextDim));
            }
            _previewArea.AddChild(marker);
            _boneMarkers.Add(marker);
        }
    }

    private void ClearBoneMarkers()
    {
        foreach (var m in _boneMarkers) if (IsInstanceValid(m)) m.QueueFree();
        _boneMarkers.Clear();
        _boneData.Clear();
    }

    // ════════════════════════════════════════════════
    //  COORDINATE TRANSFORMS
    // ════════════════════════════════════════════════

    private Vector2 SpineToPreview(Vector2 spinePos)
    {
        if (_currentVisuals == null || _spineNode == null) return spinePos;
        return _currentVisuals.Position + _spineNode.Position + spinePos * _spineNode.Scale;
    }

    private void ApplyZoomPan()
    {
        if (_previewRoot == null) return;
        _previewRoot.Scale = Vector2.One * _zoom;
        _previewRoot.Position = _panOffset;
    }

    private void ZoomAt(Vector2 localPos, float factor)
    {
        float oldZoom = _zoom;
        _zoom = Mathf.Clamp(_zoom * factor, 0.2f, 10f);
        _panOffset = localPos - (_zoom / oldZoom) * (localPos - _panOffset);
        ApplyZoomPan();
    }

    private Vector2 ScreenToContent(Vector2 localPos) => (localPos - _panOffset) / _zoom;

    /// <summary>
    /// Gets the world rotation angle (in radians) of a Spine bone.
    /// Uses the bone's a/c matrix components (first column = X axis direction).
    /// </summary>
    private float GetBoneWorldRotation(GodotObject bone)
    {
        float a = (float)bone.Call("get_a");
        float c = (float)bone.Call("get_c");
        return Mathf.Atan2(c, a);
    }

    /// <summary>
    /// Rotates a bone-local offset by the bone's world rotation.
    /// Used when applying offsets (cache → preview).
    /// </summary>
    private Vector2 RotateByBone(Vector2 offset, GodotObject bone)
    {
        float rot = GetBoneWorldRotation(bone);
        float cos = Mathf.Cos(rot);
        float sin = Mathf.Sin(rot);
        return new Vector2(
            offset.X * cos - offset.Y * sin,
            offset.X * sin + offset.Y * cos
        );
    }

    /// <summary>
    /// Inverse-rotates a world-space offset back to bone-local space.
    /// Used when caching offsets (preview → cache).
    /// </summary>
    private Vector2 UnrotateByBone(Vector2 offset, GodotObject bone)
    {
        float rot = GetBoneWorldRotation(bone);
        float cos = Mathf.Cos(-rot);
        float sin = Mathf.Sin(-rot);
        return new Vector2(
            offset.X * cos - offset.Y * sin,
            offset.X * sin + offset.Y * cos
        );
    }

    // ════════════════════════════════════════════════
    //  INPUT HANDLING
    // ════════════════════════════════════════════════

    public override void _Input(InputEvent @event)
    {
        if (_helpVisible)
        {
            if (@event is InputEventMouseButton mb && mb.Pressed) return;
        }
        if (_currentVisuals == null) return;

        var mousePos = GetGlobalMousePosition();
        var previewRect = new Rect2(_previewArea.GlobalPosition, _previewArea.Size);
        if (!previewRect.HasPoint(mousePos)) return;

        var localPos = mousePos - _previewArea.GlobalPosition;
        var contentPos = ScreenToContent(localPos);

        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
            {
                var hit = FindEyeAt(contentPos);
                if (hit != null)
                {
                    SelectEye(hit);
                    _dragging = hit;
                    _isDragging = true;
                    UpdateWorkflowStep(WorkflowStep.AdjustEyes);
                }
                else
                {
                    DeselectEye();
                    PlaceEye(contentPos);
                    SelectEye(_placements[^1]);
                    if (_currentStep == WorkflowStep.PlaceEyes && _placements.Count >= 2)
                        UpdateWorkflowStep(WorkflowStep.AdjustEyes);
                }
                GetViewport().SetInputAsHandled();
            }
            else if (mouseButton.ButtonIndex == MouseButton.Left && !mouseButton.Pressed)
            {
                if (_isDragging && _dragging != null)
                {
                    // If editing a segment, cache offset for that segment
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
            }
            else if (mouseButton.ButtonIndex == MouseButton.Middle && mouseButton.Pressed)
            {
                _isPanning = true;
                _panStart = localPos - _panOffset;
                GetViewport().SetInputAsHandled();
            }
            else if (mouseButton.ButtonIndex == MouseButton.Middle && !mouseButton.Pressed)
            {
                _isPanning = false;
            }
            else if (mouseButton.ButtonIndex == MouseButton.Right && mouseButton.Pressed)
            {
                var hit = FindEyeAt(contentPos);
                if (hit != null) { RemoveEye(hit); GetViewport().SetInputAsHandled(); }
            }
            else if (mouseButton.ButtonIndex == MouseButton.WheelUp)
            {
                var hit = FindEyeAt(contentPos);
                if (hit != null) ResizeEye(hit, 0.1f); else ZoomAt(localPos, 1.1f);
                GetViewport().SetInputAsHandled();
            }
            else if (mouseButton.ButtonIndex == MouseButton.WheelDown)
            {
                var hit = FindEyeAt(contentPos);
                if (hit != null) ResizeEye(hit, -0.1f); else ZoomAt(localPos, 1f / 1.1f);
                GetViewport().SetInputAsHandled();
            }
        }
        else if (@event is InputEventMouseMotion)
        {
            if (_isPanning)
            {
                _panOffset = localPos - _panStart;
                ApplyZoomPan();
                GetViewport().SetInputAsHandled();
            }
            else if (_isDragging && _dragging != null)
            {
                _dragging.Position = contentPos;
                _dragging.Container.Position = contentPos;
                GetViewport().SetInputAsHandled();
            }
        }
    }

    // ════════════════════════════════════════════════
    //  EYE PLACEMENT / SELECTION
    // ════════════════════════════════════════════════

    private EyePlacement FindEyeAt(Vector2 pos)
    {
        float bestDist = float.MaxValue;
        EyePlacement best = null;
        foreach (var p in _placements)
        {
            if (!p.Container.Visible) continue;
            float radius = (_eyeTexture.GetWidth() / 2f) * p.Scale;
            float dist = p.Position.DistanceTo(pos);
            if (dist < radius && dist < bestDist) { bestDist = dist; best = p; }
        }
        return best;
    }

    private void PlaceEye(Vector2 contentPos)
    {
        var container = new Node2D { Position = contentPos };
        container.AddChild(new Sprite2D { Texture = _eyeTexture, Name = "Eye" });
        container.AddChild(new Sprite2D { Texture = _irisTexture, Name = "Iris", ZIndex = 1 });
        _previewRoot.AddChild(container);

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
        _infoLabel.Text = _placements.Count + " eye(s) placed.";
    }

    private void RemoveEye(EyePlacement p)
    {
        if (p == _selectedEye) DeselectEye();
        if (IsInstanceValid(p.Container)) p.Container.QueueFree();
        _placements.Remove(p);
        _infoLabel.Text = _placements.Count + " eye(s) remaining.";
    }

    private void ResizeEye(EyePlacement p, float delta)
    {
        p.Scale = Mathf.Max(0.01f, p.Scale + delta * 0.5f);
        p.Container.Scale = Vector2.One * p.Scale;
        if (p == _selectedEye) _scaleInput.SetValueNoSignal(p.Scale);
    }

    private void UndoLastPlacement()
    {
        if (_placements.Count == 0) return;
        RemoveEye(_placements[^1]);
    }

    private void ClearAllPlacements()
    {
        foreach (var p in _placements) if (IsInstanceValid(p.Container)) p.Container.QueueFree();
        _placements.Clear();
        DeselectEye();
    }

    private void OnScaleInputChanged(double value)
    {
        if (_selectedEye == null) return;
        _selectedEye.Scale = (float)value;
        _selectedEye.Container.Scale = Vector2.One * _selectedEye.Scale;
    }

    private void DuplicateSelectedEye()
    {
        if (_selectedEye == null) return;
        PlaceEye(_selectedEye.Position + new Vector2(30, 30));
        var newEye = _placements[^1];
        newEye.Scale = _selectedEye.Scale;
        newEye.AnchorBone = _selectedEye.AnchorBone;
        newEye.HiddenByDefault = _selectedEye.HiddenByDefault;
        newEye.Container.Scale = Vector2.One * newEye.Scale;
        // Deep-copy bone timelines
        foreach (var kvp in _selectedEye.BoneTimelines)
        {
            newEye.BoneTimelines[kvp.Key] = kvp.Value.Select(s => new BoneSegment
            {
                StartTime = s.StartTime, EndTime = s.EndTime,
                BoneName = s.BoneName, SpineOffset = s.SpineOffset, HasSpineOffset = s.HasSpineOffset,
                Hidden = s.Hidden
            }).ToList();
        }
        SelectEye(newEye);
    }

    private void SelectEye(EyePlacement eye)
    {
        _selectedEye = eye;
        _scaleInput.SetValueNoSignal(eye.Scale);
        _anchorBoneInput.Text = eye.AnchorBone;
        _hiddenByDefaultCheckbox.SetPressedNoSignal(eye.HiddenByDefault);

        int idx = _placements.IndexOf(eye) + 1;
        string hiddenTag = eye.HiddenByDefault ? " (hidden by default)" : "";
        _selectedLabel.Text = "Eye #" + idx + hiddenTag;
        ApplyFont(_selectedLabel, 13, Accent);

        int totalSegs = eye.BoneTimelines.Values.Sum(list => list.Count);
        _selectionPanelHint.Text = totalSegs > 0
            ? "This eye has bone segments on " + eye.BoneTimelines.Count + " animation(s)."
            : "Drag to reposition. Use Scale to resize.";

        _editingSegmentIndex = -1;
        RefreshSegmentUI();
    }

    private void DeselectEye()
    {
        _selectedEye = null;
        _selectedLabel.Text = "No eye selected";
        ApplyFont(_selectedLabel, 13, TextDim);
        _scaleInput.SetValueNoSignal(1.0);
        _hiddenByDefaultCheckbox.SetPressedNoSignal(false);
        _selectionPanelHint.Text = "Click an eye to select it.";
        _editingSegmentIndex = -1;
        RefreshSegmentUI();
    }

    // ════════════════════════════════════════════════
    //  EXPORT
    // ════════════════════════════════════════════════

    private void ExportConfig()
    {
        if (_placements.Count == 0 || string.IsNullOrEmpty(_currentMonsterId))
        {
            _outputLabel.Text = "Nothing to export.";
            return;
        }

        UpdateWorkflowStep(WorkflowStep.Export);

        if (_skeletonGodot == null && _animController != null)
        {
            var skeleton = _animController.GetSkeleton();
            if (skeleton != null) { _skeletonGodot = skeleton.BoundObject as GodotObject; _spineNode = _animController.BoundObject as Node2D; }
        }

        float spineScale = _spineNode != null ? _spineNode.Scale.X : 1f;

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
                            var segOffset = seg.HasSpineOffset ? seg.SpineOffset : Vector2.Zero;
                            sb.Append("BoneName = \"" + seg.BoneName + "\", ");
                            sb.Append("Offset = new Vector2(" + segOffset.X.ToString("F1") + "f, " + segOffset.Y.ToString("F1") + "f)");
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
        _outputLabel.Text = "Exported " + _placements.Count + " eye(s), " + totalSegs + " segment(s) — check console!";
    }

    private Vector2? ComputeSpineOffset(EyePlacement p)
    {
        if (_skeletonGodot == null || string.IsNullOrEmpty(p.AnchorBone)) return Vector2.Zero;
        var anchorBone = _skeletonGodot.Call("find_bone", p.AnchorBone).AsGodotObject();
        if (anchorBone == null)
        {
            _outputLabel.Text = "Bone '" + p.AnchorBone + "' not found!";
            return null;
        }
        var ax = (float)anchorBone.Call("get_world_x");
        var ay = (float)anchorBone.Call("get_world_y");
        var anchorPreviewPos = SpineToPreview(new Vector2(ax, ay));
        var worldOffset = (p.Position - anchorPreviewPos) / _spineNode.Scale;
        return UnrotateByBone(worldOffset, anchorBone);
    }

    // ════════════════════════════════════════════════
    //  CLEANUP
    // ════════════════════════════════════════════════

    public override void OnSubmenuClosed()
    {
        base.OnSubmenuClosed();
        ClearAllPlacements();
        ClearBoneMarkers();
        if (_currentVisuals != null && IsInstanceValid(_currentVisuals))
        {
            _currentVisuals.QueueFree();
            _currentVisuals = null;
        }
    }

    protected override void ConnectSignals() => base.ConnectSignals();
}