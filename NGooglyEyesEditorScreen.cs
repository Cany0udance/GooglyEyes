using System.Reflection;
using System.Text;
using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

namespace GooglyEyes;

public enum WorkflowStep { SelectMonster, PlaceEyes, AdjustEyes, Export }

/// <summary>
/// Main googly-eyes editor screen. Owns shared layout (sidebar shell, preview area,
/// toolbar, workflow bar, help overlay) and delegates entity-specific behavior to
/// the active <see cref="EditorTab"/>.
///
/// To add a new tab: create a class that extends EditorTab, instantiate it in
/// RegisterTabs(), done.
/// </summary>
public partial class NGooglyEyesEditorScreen : NSubmenu
{
    // ── Shared resources (exposed to tabs) ──
    private Font _font;
    private Texture2D _eyeTexture;
    private Texture2D _irisTexture;

    public Node2D PreviewRoot { get; private set; }
    public Control PreviewArea { get; private set; }
    public float Zoom { get; set; } = 1f;
    public Vector2 PanOffset { get; set; } = Vector2.Zero;
    public WorkflowStep CurrentStep { get; private set; } = WorkflowStep.SelectMonster;

    // ── Tab system ──
    private readonly List<EditorTab> _tabs = new();
    private readonly List<Button> _tabButtons = new();
    private EditorTab _activeTab;

    // ── Shared UI elements ──
    private Label _infoLabel;
    private Label _outputLabel;
    private Label _selectedLabel;
    private Label _selectionHintLabel;
    private SpinBox _scaleInput;
    private Button _duplicateButton;
    private Button _clearButton;
    private Button _undoButton;
    private Button _exportButton;

    // ── Workflow bar ──
    private readonly Label[] _stepLabels = new Label[4];
    private readonly ColorRect[] _stepBgs = new ColorRect[4];
    private Label _stepHintLabel;

    // ── Help overlay ──
    private Control _helpOverlay;
    private bool _helpVisible;
    private Button _helpToggleButton;

    // ── Pan state ──
    private bool _isPanning;
    private Vector2 _panStart;

    protected override Control InitialFocusedControl => null;

    // ═══════════════════════════════════════════════
    //  LIFECYCLE
    // ═══════════════════════════════════════════════

    public override void _Ready()
    {
        try
        {
            SetAnchorsPreset(LayoutPreset.FullRect);
            var screen = GetViewportRect().Size;

            _font = ResourceLoader.Load<Font>("res://fonts/kreon_regular.ttf");
            _eyeTexture = ResourceLoader.Load<Texture2D>("res://GooglyEyes/googly_eye.png");
            _irisTexture = ResourceLoader.Load<Texture2D>("res://GooglyEyes/googly_iris.png");

            // Background
            var bg = new ColorRect { Color = EditorTheme.BgColor };
            bg.SetAnchorsPreset(LayoutPreset.FullRect);
            AddChild(bg);

            BuildSidebarShell(screen);
            BuildPreviewPanel(screen);
            BuildWorkflowBar(screen);
            BuildToolbar(screen);
            BuildSelectionPanel(screen);

            RegisterTabs();
            BuildTabBar(screen);

            // These must be added after tabs so they sit on top in the scene tree
            BuildHelpToggle(screen);
            BuildHelpOverlay(screen);
            BuildBackButton();

            // Activate the first tab
            SwitchToTab(0);
        }
        catch (Exception e)
        {
            GD.PrintErr("[GooglyEyes] Editor _Ready error: " + e);
        }
    }

    public override void _Process(double delta) => _activeTab?.Process(delta);

    public override void OnSubmenuClosed()
    {
        foreach (var tab in _tabs) tab.Cleanup();
        base.OnSubmenuClosed();
    }

    protected override void ConnectSignals() => base.ConnectSignals();

    // ═══════════════════════════════════════════════
    //  TAB REGISTRATION
    // ═══════════════════════════════════════════════

    /// <summary>
    /// Instantiate and register all editor tabs here.
    /// Adding a new tab is a one-liner.
    /// </summary>
    private void RegisterTabs()
    {
        RegisterTab(new MonsterEditorTab());
        RegisterTab(new CardEditorTab());
        RegisterTab(new RelicEditorTab());
        RegisterTab(new ScreenEditorTab());
    }

    private void RegisterTab(EditorTab tab)
    {
        tab.Register(this, _font, _eyeTexture, _irisTexture);
        _tabs.Add(tab);
    }

    // ═══════════════════════════════════════════════
    //  TAB BAR
    // ═══════════════════════════════════════════════

    private void BuildTabBar(Vector2 screenSize)
    {
        var container = new HBoxContainer
        {
            Position = new Vector2(EditorTheme.Padding, EditorTheme.Padding),
            Size = new Vector2(EditorTheme.SidebarWidth - EditorTheme.Padding * 2, 30)
        };
        container.AddThemeConstantOverride("separation", 4);
        AddChild(container);

        float btnWidth = (EditorTheme.SidebarWidth - EditorTheme.Padding * 2 - 4 * (_tabs.Count - 1)) / _tabs.Count;

        for (int i = 0; i < _tabs.Count; i++)
        {
            var tab = _tabs[i];
            int idx = i;
            var btn = new Button
            {
                Text = tab.TabName,
                ToggleMode = true,
                ButtonPressed = i == 0,
                CustomMinimumSize = new Vector2(btnWidth, 28),
                TooltipText = tab.TabTooltip
            };
            if (_font != null) btn.AddThemeFontOverride("font", _font);
            btn.AddThemeFontSizeOverride("font_size", 13);
            btn.Pressed += () => SwitchToTab(idx);
            container.AddChild(btn);
            _tabButtons.Add(btn);
        }
    }

    private void SwitchToTab(int index)
    {
        if (index < 0 || index >= _tabs.Count) return;
        var newTab = _tabs[index];
        if (newTab == _activeTab) return;

        _activeTab?.Deactivate();

        for (int i = 0; i < _tabButtons.Count; i++)
            _tabButtons[i].SetPressedNoSignal(i == index);

        _activeTab = newTab;
        _activeTab.Activate();

        SetInfoText("Select an entry from the sidebar to begin.");
        UpdateWorkflowStep(WorkflowStep.SelectMonster);
        RefreshSelectionPanel();
    }

    // ═══════════════════════════════════════════════
    //  SIDEBAR SHELL (just the background + title)
    // ═══════════════════════════════════════════════

    private void BuildSidebarShell(Vector2 screenSize)
    {
        AddChild(new ColorRect
        {
            Color = EditorTheme.PanelBg,
            Position = Vector2.Zero,
            Size = new Vector2(EditorTheme.SidebarWidth, screenSize.Y)
        });
        AddChild(MakeLabel("Googly Eyes Editor",
            new Vector2(EditorTheme.Padding, EditorTheme.Padding),
            new Vector2(EditorTheme.SidebarWidth - EditorTheme.Padding * 2, 30), 18, EditorTheme.TextBright));
        AddChild(MakeLabel("Pick an entry to begin.",
            new Vector2(EditorTheme.Padding, 42),
            new Vector2(EditorTheme.SidebarWidth - EditorTheme.Padding * 2, 16), 11, EditorTheme.TextDim));
    }

    // ═══════════════════════════════════════════════
    //  PREVIEW PANEL
    // ═══════════════════════════════════════════════

    private void BuildPreviewPanel(Vector2 screenSize)
    {
        float px = EditorTheme.SidebarWidth + EditorTheme.Padding;
        float py = EditorTheme.Padding + 58f;
        float pw = screenSize.X - EditorTheme.SidebarWidth - EditorTheme.Padding * 2;
        float ph = screenSize.Y - 447 - 58f;

        AddChild(new ColorRect { Color = EditorTheme.PreviewBg, Position = new Vector2(px, py), Size = new Vector2(pw, ph) });

        PreviewArea = new Control
        {
            Position = new Vector2(px, py),
            Size = new Vector2(pw, ph),
            ClipContents = true
        };
        AddChild(PreviewArea);

        PreviewRoot = new Node2D();
        PreviewArea.AddChild(PreviewRoot);

        _infoLabel = MakeLabel("Select an entry from the sidebar to get started.",
            new Vector2(px + 10, py + 5), new Vector2(pw - 20, 30), 13, EditorTheme.TextDim);
        AddChild(_infoLabel);
    }

    // ═══════════════════════════════════════════════
    //  WORKFLOW BAR
    // ═══════════════════════════════════════════════

    private static readonly string[] StepNames = { "1. Pick Entry", "2. Place Eyes", "3. Adjust", "4. Export" };
    private static readonly string[] StepHints =
    {
        "Start by choosing an entry from the sidebar on the left.",
        "Click anywhere on the preview to place a googly eye. Scroll over an eye to resize. Right-click to remove.",
        "Fine-tune: select an eye to change its scale or other properties.",
        "Happy with the placement? Hit 'Export to Console' to generate the config code."
    };

    private void BuildWorkflowBar(Vector2 screenSize)
    {
        float bx = EditorTheme.SidebarWidth + EditorTheme.Padding;
        float by = EditorTheme.Padding;
        float bw = screenSize.X - EditorTheme.SidebarWidth - EditorTheme.Padding * 2;
        float sw = bw / 4f;

        for (int i = 0; i < 4; i++)
        {
            var bg = new ColorRect
            {
                Color = EditorTheme.StepInactiveBg,
                Position = new Vector2(bx + sw * i + 2, by),
                Size = new Vector2(sw - 4, 28)
            };
            AddChild(bg);
            _stepBgs[i] = bg;

            var lbl = MakeLabel(StepNames[i],
                new Vector2(bx + sw * i + 10, by + 4),
                new Vector2(sw - 20, 20), 12, EditorTheme.TextDim);
            AddChild(lbl);
            _stepLabels[i] = lbl;
        }

        _stepHintLabel = MakeLabel(StepHints[0],
            new Vector2(bx, by + 32),
            new Vector2(bw, 20), 12, EditorTheme.AccentWarm);
        _stepHintLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        AddChild(_stepHintLabel);
    }

    public void UpdateWorkflowStep(WorkflowStep step)
    {
        CurrentStep = step;
        int idx = (int)step;
        for (int i = 0; i < 4; i++)
        {
            if (i < idx)
            {
                _stepBgs[i].Color = EditorTheme.StepCompleteBg;
                ApplyFont(_stepLabels[i], 12, EditorTheme.StepCompleteText);
            }
            else if (i == idx)
            {
                _stepBgs[i].Color = EditorTheme.StepActiveBg;
                ApplyFont(_stepLabels[i], 12, EditorTheme.TextBright);
            }
            else
            {
                _stepBgs[i].Color = EditorTheme.StepInactiveBg;
                ApplyFont(_stepLabels[i], 12, EditorTheme.TextDim);
            }
        }
        _stepHintLabel.Text = StepHints[idx];
    }

    // ═══════════════════════════════════════════════
    //  TOOLBAR
    // ═══════════════════════════════════════════════

    private void BuildToolbar(Vector2 screenSize)
    {
        float y = screenSize.Y - 323;
        float x = EditorTheme.SidebarWidth + EditorTheme.Padding;

        _clearButton = MakeButton("Clear All", new Vector2(x, y), new Vector2(100, 34), 12);
        _clearButton.TooltipText = "Remove every placed eye.";
        _clearButton.Pressed += () => _activeTab?.ClearAll();
        AddChild(_clearButton);

        _undoButton = MakeButton("Undo", new Vector2(x + 110, y), new Vector2(70, 34), 12);
        _undoButton.TooltipText = "Remove the last placed eye.";
        _undoButton.Pressed += () => _activeTab?.UndoLast();
        AddChild(_undoButton);

        _exportButton = MakeButton("Export to Console", new Vector2(x + 190, y), new Vector2(150, 34), 12);
        _exportButton.TooltipText = "Print config code to Godot's output console.";
        _exportButton.Pressed += () => _activeTab?.Export();
        AddChild(_exportButton);

        _outputLabel = MakeLabel("", new Vector2(x + 360, y + 5), new Vector2(400, 28), 12, EditorTheme.Accent);
        AddChild(_outputLabel);
    }

    // ═══════════════════════════════════════════════
    //  SELECTION PANEL
    // ═══════════════════════════════════════════════

    private void BuildSelectionPanel(Vector2 screenSize)
    {
        float px = EditorTheme.SidebarWidth + EditorTheme.Padding;
        float py = screenSize.Y - 278;
        float pw = screenSize.X - EditorTheme.SidebarWidth - EditorTheme.Padding * 2;

        AddChild(new ColorRect { Color = EditorTheme.PanelBg, Position = new Vector2(px, py), Size = new Vector2(pw, 55) });
        AddChild(MakeLabel("Selected Eye", new Vector2(px + 10, py + 4), new Vector2(100, 16), 11, EditorTheme.Accent));

        _selectionHintLabel = MakeLabel("Click an eye to select it.",
            new Vector2(px + 120, py + 4), new Vector2(pw - 140, 16), 10, EditorTheme.TextDim);
        AddChild(_selectionHintLabel);

        float row = py + 24;

        _selectedLabel = MakeLabel("No eye selected", new Vector2(px + 10, row + 3), new Vector2(120, 24), 13, EditorTheme.TextDim);
        AddChild(_selectedLabel);

        AddChild(MakeLabel("Scale:", new Vector2(px + 130, row + 3), new Vector2(50, 24), 13, EditorTheme.TextNormal));

        _scaleInput = new SpinBox
        {
            MinValue = 0.01, MaxValue = 5.0, Step = 0.01, Value = 1.0,
            Position = new Vector2(px + 180, row - 2), Size = new Vector2(100, 30),
            Editable = true, TooltipText = "Eye size multiplier."
        };
        if (_font != null) _scaleInput.AddThemeFontOverride("font", _font);
        _scaleInput.AddThemeFontSizeOverride("font_size", 12);
        _scaleInput.ValueChanged += v => _activeTab?.OnScaleChanged(v);
        AddChild(_scaleInput);

        _duplicateButton = MakeButton("Duplicate", new Vector2(px + 300, row - 2), new Vector2(100, 30), 12);
        _duplicateButton.TooltipText = "Copy this eye.";
        _duplicateButton.Pressed += () => _activeTab?.DuplicateSelected();
        AddChild(_duplicateButton);
    }

    /// <summary>
    /// Refreshes the shared selection panel from the active tab's state.
    /// Called by tabs after selection changes.
    /// </summary>
    public void RefreshSelectionPanel()
    {
        if (_activeTab == null) return;

        if (_activeTab.HasSelection)
        {
            _selectedLabel.Text = _activeTab.SelectionLabel;
            ApplyFont(_selectedLabel, 13, EditorTheme.Accent);
            _scaleInput.SetValueNoSignal(_activeTab.SelectionScale);
        }
        else
        {
            _selectedLabel.Text = "No eye selected";
            ApplyFont(_selectedLabel, 13, EditorTheme.TextDim);
            _scaleInput.SetValueNoSignal(1.0);
        }
        _selectionHintLabel.Text = _activeTab.SelectionHint;
    }

    public void SetScaleValueNoSignal(double value) => _scaleInput.SetValueNoSignal(value);

    // ═══════════════════════════════════════════════
    //  HELP OVERLAY
    // ═══════════════════════════════════════════════

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
        float ow = 480f, oh = 560f;
        float ox = (screenSize.X - ow) / 2f;
        float oy = (screenSize.Y - oh) / 2f;

        _helpOverlay = new Control { Position = Vector2.Zero, Size = screenSize, Visible = false };
        AddChild(_helpOverlay);

        var dim = new ColorRect { Color = new Color(0f, 0f, 0f, 0.6f) };
        dim.SetAnchorsPreset(LayoutPreset.FullRect);
        _helpOverlay.AddChild(dim);

        _helpOverlay.AddChild(new ColorRect { Color = EditorTheme.HelpBg, Position = new Vector2(ox, oy), Size = new Vector2(ow, oh) });

        float cx = ox + 20f, cy = oy + 16f;
        _helpOverlay.AddChild(MakeLabel("Googly Eyes Editor — Quick Reference",
            new Vector2(cx, cy), new Vector2(ow - 40, 28), 17, EditorTheme.TextBright));
        cy += 36f;

        string[] lines =
        {
            "BASICS",
            "1. Pick an entry from the sidebar.",
            "2. Click the preview to place googly eyes.",
            "3. Scroll over an eye to resize, right-click to remove.",
            "4. Middle-click + drag to pan, scroll empty area to zoom.",
            "",
            "BONE ANCHORING (Monsters only)",
            "Each eye follows a bone (default: 'head'). Click an eye,",
            "then change the bone in the Bone Anchoring panel.",
            "Use 'Show Bones' to see available bones.",
            "",
            "SWITCHING BONES MID-ANIMATION",
            "1. Select an eye and pick a non-idle animation.",
            "2. Scrub to the switch point, click 'Split here'.",
            "3. Click 'Edit' on a segment to activate it.",
            "4. Change the bone, drag the eye to position.",
            "5. Click 'Done editing' when finished.",
            "",
            "HIDING EYES (Monsters only)",
            "Tick 'Hidden by default' to make an eye invisible",
            "during idle. Add segments on other animations and",
            "toggle visibility per segment.",
            "",
            "CARDS",
            "Cards are simpler — no bones or animations.",
            "Just place, resize, and adjust opacity.",
            "",
            "EXPORT",
            "Hit 'Export to Console' to print the config code."
        };

        foreach (var line in lines)
        {
            bool header = line.Length > 0 && line == line.ToUpper();
            _helpOverlay.AddChild(MakeLabel(line,
                new Vector2(cx, cy), new Vector2(ow - 40, 18),
                header ? 13 : 12, header ? EditorTheme.Accent : EditorTheme.TextNormal));
            cy += header ? 22f : 16f;
        }

        var closeBtn = MakeButton("Close", new Vector2(ox + ow / 2f - 40f, oy + oh - 46f), new Vector2(80, 32), 14);
        closeBtn.Pressed += ToggleHelp;
        _helpOverlay.AddChild(closeBtn);
    }

    private void ToggleHelp()
    {
        _helpVisible = !_helpVisible;
        _helpOverlay.Visible = _helpVisible;
    }

    // ═══════════════════════════════════════════════
    //  BACK BUTTON
    // ═══════════════════════════════════════════════

    private void BuildBackButton()
    {
        var scene = ResourceLoader.Load<PackedScene>("res://scenes/ui/back_button.tscn");
        var btn = scene.Instantiate<NBackButton>();
        btn.Name = "BackButton";
        AddChild(btn);
        ConnectSignals();
        var field = typeof(NSubmenu).GetField("_backButton", BindingFlags.NonPublic | BindingFlags.Instance);
        (field?.GetValue(this) as NBackButton)?.Enable();
    }

    // ═══════════════════════════════════════════════
    //  ZOOM / PAN
    // ═══════════════════════════════════════════════

    public void ResetZoomPan()
    {
        Zoom = 1f;
        PanOffset = Vector2.Zero;
        ApplyZoomPan();
    }

    private void ApplyZoomPan()
    {
        if (PreviewRoot == null) return;
        PreviewRoot.Scale = Vector2.One * Zoom;
        PreviewRoot.Position = PanOffset;
    }

    private void ZoomAt(Vector2 localPos, float factor)
    {
        float old = Zoom;
        Zoom = Mathf.Clamp(Zoom * factor, 0.2f, 10f);
        PanOffset = localPos - (Zoom / old) * (localPos - PanOffset);
        ApplyZoomPan();
    }

    public Vector2 ScreenToContent(Vector2 localPos) => (localPos - PanOffset) / Zoom;

    // ═══════════════════════════════════════════════
    //  INPUT
    // ═══════════════════════════════════════════════

    public override void _Input(InputEvent @event)
    {
        if (!IsVisibleInTree()) return;
        // ── De-focus spinboxes / line edits on outside click ──
        var focus = GetViewport().GuiGetFocusOwner();
        if (focus is SpinBox or LineEdit)
        {
            if (@event is InputEventMouseButton { Pressed: true } mb)
            {
                if (!new Rect2(focus.GlobalPosition, focus.Size).HasPoint(GetGlobalMousePosition()))
                    focus.ReleaseFocus();
                else
                    return;
            }
            else return;
        }

        // ── Help overlay eats clicks ──
        if (_helpVisible && @event is InputEventMouseButton { Pressed: true }) return;

        // ── Bounds check ──
        var mouse = GetGlobalMousePosition();
        if (!new Rect2(PreviewArea.GlobalPosition, PreviewArea.Size).HasPoint(mouse)) return;

        var localPos = mouse - PreviewArea.GlobalPosition;
        var contentPos = ScreenToContent(localPos);

        // ── Pan (middle mouse — shared across all tabs) ──
        if (@event is InputEventMouseButton mmb)
        {
            if (mmb.ButtonIndex == MouseButton.Middle && mmb.Pressed)
            {
                _isPanning = true;
                _panStart = localPos - PanOffset;
                GetViewport().SetInputAsHandled();
                return;
            }
            if (mmb.ButtonIndex == MouseButton.Middle && !mmb.Pressed)
            {
                _isPanning = false;
                return;
            }

            // ── Zoom on empty space (no eye under cursor) ──
            bool eyeUnder = _activeTab?.HasEyeAt(contentPos) ?? false;
            if (!eyeUnder && mmb.ButtonIndex == MouseButton.WheelUp)
            {
                ZoomAt(localPos, 1.1f);
                GetViewport().SetInputAsHandled();
                return;
            }
            if (!eyeUnder && mmb.ButtonIndex == MouseButton.WheelDown)
            {
                ZoomAt(localPos, 1f / 1.1f);
                GetViewport().SetInputAsHandled();
                return;
            }
        }

        if (@event is InputEventMouseMotion && _isPanning)
        {
            PanOffset = localPos - _panStart;
            ApplyZoomPan();
            GetViewport().SetInputAsHandled();
            return;
        }

        // ── Delegate to active tab ──
        if (_activeTab != null && _activeTab.HandleInput(@event, localPos, contentPos))
            GetViewport().SetInputAsHandled();
    }

    // ═══════════════════════════════════════════════
    //  PUBLIC HELPERS (used by tabs)
    // ═══════════════════════════════════════════════

    public void SetInfoText(string text) => _infoLabel.Text = text;
    public void SetOutputText(string text) => _outputLabel.Text = text;

    public void ApplyFont(Control control, int size, Color color)
    {
        if (_font != null) control.AddThemeFontOverride("font", _font);
        control.AddThemeFontSizeOverride("font_size", size);
        control.AddThemeColorOverride("font_color", color);
    }

    public Label MakeLabel(string text, Vector2 pos, Vector2 size, int fontSize, Color color)
    {
        var label = new Label { Text = text, Position = pos, Size = size };
        ApplyFont(label, fontSize, color);
        return label;
    }

    public Button MakeButton(string text, Vector2 pos, Vector2 size, int fontSize = 14)
    {
        var button = new Button { Text = text, Position = pos, Size = size };
        if (_font != null) button.AddThemeFontOverride("font", _font);
        button.AddThemeFontSizeOverride("font_size", fontSize);
        return button;
    }
}