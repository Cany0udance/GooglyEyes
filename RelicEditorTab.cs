using System.Text;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Relics;

namespace GooglyEyes;

/// <summary>
/// Editor tab for placing googly eyes on relic icons.
/// Relics are static images — no skeleton or animation.
/// Eye placement is a simple offset from the icon center.
/// </summary>
public class RelicEditorTab : EditorTab
{
    public override string TabName => "Relics";
    public override string TabTooltip => "Place googly eyes on relic icons.";
 
    public static bool SuppressRelicPatch;
 
    // ── Sidebar ──
    private ScrollContainer _sidebarScroll;
    private VBoxContainer _relicList;
    private Button _selectedButton;
    private bool _listBuilt;
    private LineEdit _searchInput;
    private OptionButton _filterDropdown;
    private enum FilterMode { All, Configured, Unconfigured }
    private FilterMode _filterMode = FilterMode.All;
 
    // ── Preview ──
    private NRelic _previewRelic;
    private string _currentRelicId;
    private RelicModel _currentRelicModel;
    private Vector2 _relicSize;
    private Vector2 _relicCenter;
 
    // ── Placements ──
    private readonly List<RelicEyePlacement> _placements = new();
    private RelicEyePlacement _selectedEye;
    private RelicEyePlacement _dragging;
    private bool _isDragging;
 
    // ── Relic-specific controls ──
    private SpinBox _opacityInput;
    private Label _opacityLabel;
 
    private class RelicEyePlacement
    {
        public Vector2 Position;
        public float Scale = 1f;
        public float Opacity = 1f;
        public Sprite2D EyeSprite;
        public Sprite2D IrisSprite;
        public Node2D Container;
        public Vector2 RelicOffset;
    }
 
    // ═══════════════════════════════════════════════
    //  LIFECYCLE
    // ═══════════════════════════════════════════════
 
    protected override void OnRegister()
    {
        var screenSize = Screen.GetViewportRect().Size;
        BuildSidebarContainer(screenSize);
        BuildOpacityControl(screenSize);
    }
 
    public override void BuildSidebarItems()
    {
        if (_listBuilt) return;
        _listBuilt = true;
 
        foreach (var relic in ModelDb.AllRelics.OrderBy(r => r.Id.Entry))
        {
            var id = relic.Id.Entry;
            bool hasConfig = RelicGooglyEyesRegistry.Configs.ContainsKey(id);
 
            var button = new Button
            {
                Text = id, Flat = true,
                Alignment = HorizontalAlignment.Left,
                CustomMinimumSize = new Vector2(0, 28),
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
            };
            button.SetMeta("relic_id", id);
            ApplyFont(button, 12, hasConfig ? EditorTheme.AccentGreen : EditorTheme.TextNormal);
            button.AddThemeColorOverride("font_hover_color", EditorTheme.TextBright);
 
            var relicRef = relic;
            button.Pressed += () => SelectRelic(relicRef, id);
            _relicList.AddChild(button);
        }
    }
 
    public override void Activate()
    {
        _sidebarScroll.Visible = true;
        _searchInput.Visible = true;
        _filterDropdown.Visible = true;
        _opacityLabel.Visible = true;
        _opacityInput.Visible = true;
        BuildSidebarItems();
    }
 
    public override void Deactivate()
    {
        _sidebarScroll.Visible = false;
        _searchInput.Visible = false;
        _filterDropdown.Visible = false;
        _opacityLabel.Visible = false;
        _opacityInput.Visible = false;
        ClearPreview();
        _selectedButton = null;
    }
 
    public override void Process(double delta) { /* Relics are static — nothing to update per frame. */ }
 
    public override void Cleanup() => ClearPreview();
 
    // ═══════════════════════════════════════════════
    //  SELECTION STATE
    // ═══════════════════════════════════════════════
 
    public override bool HasSelection => _selectedEye != null;
 
    public override string SelectionLabel
    {
        get
        {
            if (_selectedEye == null) return "No eye selected";
            return "Eye #" + (_placements.IndexOf(_selectedEye) + 1);
        }
    }
 
    public override float SelectionScale => _selectedEye?.Scale ?? 1f;
    public override string SelectionHint => _selectedEye != null
        ? "Drag to reposition. Scroll to resize."
        : "Click an eye to select it.";
 
    // ═══════════════════════════════════════════════
    //  UI BUILDING
    // ═══════════════════════════════════════════════
 
    private void BuildSidebarContainer(Vector2 screenSize)
    {
        float controlsY = 62f;
 
        // Search input
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
 
        // Filter dropdown
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
 
        _relicList = new VBoxContainer { CustomMinimumSize = new Vector2(EditorTheme.SidebarWidth - 20, 0) };
        _relicList.AddThemeConstantOverride("separation", 2);
        _sidebarScroll.AddChild(_relicList);
    }
 
    private void BuildOpacityControl(Vector2 screenSize)
    {
        float px = EditorTheme.SidebarWidth + EditorTheme.Padding;
        float py = screenSize.Y - 278;
        float row = py + 24;
 
        _opacityLabel = MakeLabel("Opacity:", new Vector2(px + 420, row + 3), new Vector2(60, 24), 13, EditorTheme.TextNormal);
        _opacityLabel.Visible = false;
        Screen.AddChild(_opacityLabel);
 
        _opacityInput = new SpinBox
        {
            MinValue = 0.0, MaxValue = 1.0, Step = 0.05, Value = 1.0,
            Position = new Vector2(px + 480, row - 2), Size = new Vector2(90, 30),
            Editable = true, TooltipText = "Eye opacity. 1.0 = fully visible.",
            Visible = false
        };
        if (Font != null) _opacityInput.AddThemeFontOverride("font", Font);
        _opacityInput.AddThemeFontSizeOverride("font_size", 12);
        _opacityInput.ValueChanged += OnOpacityChanged;
        Screen.AddChild(_opacityInput);
    }
 
    // ═══════════════════════════════════════════════
    //  SIDEBAR FILTER
    // ═══════════════════════════════════════════════
 
    private void RefreshSidebarFilter()
    {
        string search = _searchInput.Text?.Trim() ?? "";
        bool hasSearch = search.Length > 0;
 
        foreach (var child in _relicList.GetChildren())
        {
            if (child is not Button btn || !btn.HasMeta("relic_id")) continue;
 
            string id = btn.GetMeta("relic_id").AsString();
            bool hasConfig = RelicGooglyEyesRegistry.Configs.ContainsKey(id);
 
            bool matchesSearch = !hasSearch || id.Contains(search, StringComparison.OrdinalIgnoreCase);
            bool matchesFilter = _filterMode switch
            {
                FilterMode.Configured => hasConfig,
                FilterMode.Unconfigured => !hasConfig,
                _ => true
            };
 
            btn.Visible = matchesSearch && matchesFilter;
        }
    }
 
    // ═══════════════════════════════════════════════
    //  RELIC SELECTION
    // ═══════════════════════════════════════════════
 
    private void SelectRelic(RelicModel relic, string relicId)
    {
        ClearPreview();
        HighlightButton(relicId);
 
        _currentRelicId = relicId;
        _currentRelicModel = relic;
        Screen.ResetZoomPan();
 
        _previewRelic = NRelic.Create(relic, NRelic.IconSize.Large);
        if (_previewRelic == null)
        {
            SetInfo("Failed to create relic preview for " + relicId);
            return;
        }
 
        _previewRelic.SetMeta("googly_editor_preview", true);
        PreviewRoot.AddChild(_previewRelic);
 
        // Use the Icon TextureRect's actual rendered size, not the BigIcon texture size.
        // The NRelic scene renders the icon at a fixed size (e.g. 60x60) regardless of texture resolution.
        var iconNode = _previewRelic.Icon;
        if (iconNode != null)
        {
            _relicSize = iconNode.Size;
            _relicCenter = iconNode.Position + iconNode.Size / 2f;
        }
        else
        {
            _relicSize = new Vector2(60, 60);
            _relicCenter = _relicSize / 2f;
        }
 
        _previewRelic.Position = new Vector2(
            (PreviewArea.Size.X - _relicSize.X) / 2f,
            (PreviewArea.Size.Y - _relicSize.Y) / 2f
        );
 
        SetInfo(relicId + " loaded — click the relic to place eyes!");
        AdvanceWorkflow(WorkflowStep.PlaceEyes);
 
        if (LoadPresets(relicId))
        {
            SetInfo(relicId + " loaded with " + _placements.Count + " preset eye(s).");
            if (_placements.Count > 0) AdvanceWorkflow(WorkflowStep.AdjustEyes);
        }
    }
 
    private void HighlightButton(string relicId)
    {
        if (_selectedButton != null && GodotObject.IsInstanceValid(_selectedButton))
        {
            bool prev = RelicGooglyEyesRegistry.Configs.ContainsKey(_selectedButton.Text);
            ApplyFont(_selectedButton, 12, prev ? EditorTheme.AccentGreen : EditorTheme.TextNormal);
        }
        foreach (var child in _relicList.GetChildren())
        {
            if (child is Button btn && btn.Text == relicId)
            {
                _selectedButton = btn;
                ApplyFont(btn, 12, EditorTheme.SelectedEntryColor);
                break;
            }
        }
    }
 
    private void ClearPreview()
    {
        ClearAll();
        DeselectCurrent();
        if (_previewRelic != null && GodotObject.IsInstanceValid(_previewRelic))
        {
            _previewRelic.QueueFree();
            _previewRelic = null;
        }
        _currentRelicId = null;
        _currentRelicModel = null;
    }
 
    // ═══════════════════════════════════════════════
    //  PRESET LOADING
    // ═══════════════════════════════════════════════
 
    private bool LoadPresets(string relicId)
    {
        if (!RelicGooglyEyesRegistry.Configs.TryGetValue(relicId, out var configs)) return false;
        if (configs.Length == 0 || _previewRelic == null) return false;
 
        foreach (var c in configs)
            PlaceEye(RelicToContent(c.Offset), c.Scale, c.Opacity);
 
        return _placements.Count > 0;
    }
 
    // ═══════════════════════════════════════════════
    //  COORDINATE HELPERS
    // ═══════════════════════════════════════════════
 
    private Vector2 RelicToContent(Vector2 relicOffset) =>
        _previewRelic != null ? _previewRelic.Position + _relicCenter + relicOffset : Vector2.Zero;
 
    private Vector2 ContentToRelic(Vector2 contentPos) =>
        _previewRelic != null ? contentPos - _previewRelic.Position - _relicCenter : Vector2.Zero;
 
    // ═══════════════════════════════════════════════
    //  INPUT
    // ═══════════════════════════════════════════════
 
    public override bool HasEyeAt(Vector2 contentPos) => FindEyeAt(contentPos) != null;
 
    public override bool HandleInput(InputEvent @event, Vector2 localPos, Vector2 contentPos)
    {
        if (_previewRelic == null) return false;
 
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
                    _dragging.RelicOffset = ContentToRelic(_dragging.Position);
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
        if (_placements.Count == 0 || string.IsNullOrEmpty(_currentRelicId))
        {
            SetOutput("Nothing to export.");
            return;
        }
        AdvanceWorkflow(WorkflowStep.Export);
 
        var sb = new StringBuilder();
        sb.AppendLine("// Googly Eyes config for relic: " + _currentRelicId);
        sb.AppendLine("{ \"" + _currentRelicId + "\", new RelicEyeConfig[] {");
 
        foreach (var p in _placements)
        {
            var offset = ContentToRelic(p.Position);
            sb.Append("    new RelicEyeConfig { ");
            sb.Append("Offset = new Vector2(" + offset.X.ToString("F1") + "f, " + offset.Y.ToString("F1") + "f), ");
            sb.Append("Scale = " + p.Scale.ToString("F2") + "f");
            if (p.Opacity < 0.99f) sb.Append(", Opacity = " + p.Opacity.ToString("F2") + "f");
            sb.AppendLine(" },");
        }
        sb.AppendLine("}},");
 
        GD.Print("[GooglyEyes] === RELIC EXPORT ===");
        GD.Print(sb.ToString());
        GD.Print("[GooglyEyes] === END RELIC EXPORT ===");
 
        SetOutput("Exported " + _placements.Count + " relic eye(s) — check console!");
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
        PlaceEye(_selectedEye.Position + new Vector2(15, 15), _selectedEye.Scale, _selectedEye.Opacity);
        SelectEye(_placements[^1]);
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
        _opacityInput.SetValueNoSignal(1.0);
        Screen.RefreshSelectionPanel();
    }
 
    // ═══════════════════════════════════════════════
    //  EYE MANAGEMENT
    // ═══════════════════════════════════════════════
 
    private RelicEyePlacement FindEyeAt(Vector2 pos)
    {
        float bestDist = float.MaxValue;
        RelicEyePlacement best = null;
        foreach (var p in _placements)
        {
            if (!IsWithinEye(pos, p.Position, p.Scale)) continue;
            float dist = p.Position.DistanceTo(pos);
            if (dist < bestDist) { bestDist = dist; best = p; }
        }
        return best;
    }
 
    private void PlaceEye(Vector2 contentPos, float scale = 1f, float opacity = 1f)
    {
        var container = CreateEyeContainer(contentPos, scale);
        container.Modulate = new Color(1f, 1f, 1f, opacity);
 
        _placements.Add(new RelicEyePlacement
        {
            Position = contentPos, Scale = scale, Opacity = opacity,
            EyeSprite = container.GetChild<Sprite2D>(0),
            IrisSprite = container.GetChild<Sprite2D>(1),
            Container = container,
            RelicOffset = ContentToRelic(contentPos)
        });
        SetInfo(_placements.Count + " eye(s) placed.");
    }
 
    private void RemoveEye(RelicEyePlacement p)
    {
        if (p == _selectedEye) DeselectCurrent();
        if (GodotObject.IsInstanceValid(p.Container)) p.Container.QueueFree();
        _placements.Remove(p);
        SetInfo(_placements.Count + " eye(s) remaining.");
    }
 
    private void ResizeEye(RelicEyePlacement p, float delta)
    {
        p.Scale = Mathf.Max(0.01f, p.Scale + delta * 0.5f);
        p.Container.Scale = Vector2.One * p.Scale;
        if (p == _selectedEye) Screen.SetScaleValueNoSignal(p.Scale);
    }
 
    private void SelectEye(RelicEyePlacement eye)
    {
        _selectedEye = eye;
        _opacityInput.SetValueNoSignal(eye.Opacity);
        Screen.RefreshSelectionPanel();
    }
 
    private void OnOpacityChanged(double value)
    {
        if (_selectedEye == null) return;
        _selectedEye.Opacity = (float)value;
        _selectedEye.Container.Modulate = new Color(1f, 1f, 1f, _selectedEye.Opacity);
    }
}
 