using System.Text;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace GooglyEyes;

/// <summary>
/// Editor tab for placing googly eyes on card art.
/// Cards have no skeleton or animation — just a fixed-size Control with a portrait.
/// Eye placement is a simple offset from card center.
/// </summary>
public class CardEditorTab : EditorTab
{
    public override string TabName => "Cards";
    public override string TabTooltip => "Place googly eyes on card art.";
 
    private static readonly Vector2 CardSize = NCard.defaultSize;  // (300, 422)
    private static readonly Vector2 CardCenter = CardSize / 2f;
 
    public static bool SuppressCardPatch;
 
    // ── Sidebar ──
    private ScrollContainer _sidebarScroll;
    private VBoxContainer _cardList;
    private Button _selectedButton;
    private bool _listBuilt;
 
    // ── Preview ──
    private NCard _previewCard;
    private string _currentCardId;
    private CardModel _currentCardModel;
 
    // ── Placements ──
    private readonly List<CardEyePlacement> _placements = new();
    private CardEyePlacement _selectedEye;
    private CardEyePlacement _dragging;
    private bool _isDragging;
 
    // ── Card-specific controls ──
    private SpinBox _opacityInput;
    private Label _opacityLabel;
 
    private class CardEyePlacement
    {
        public Vector2 Position;
        public float Scale = 1f;
        public float Opacity = 1f;
        public Sprite2D EyeSprite;
        public Sprite2D IrisSprite;
        public Node2D Container;
        public Vector2 CardOffset;
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
 
        foreach (var card in ModelDb.AllCards.OrderBy(c => c.Id.Entry))
        {
            var id = card.Id.Entry;
            bool hasConfig = CardGooglyEyesRegistry.Configs.ContainsKey(id);
 
            var button = new Button
            {
                Text = id, Flat = true,
                Alignment = HorizontalAlignment.Left,
                CustomMinimumSize = new Vector2(0, 28),
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
            };
            ApplyFont(button, 12, hasConfig ? EditorTheme.AccentGreen : EditorTheme.TextNormal);
            button.AddThemeColorOverride("font_hover_color", EditorTheme.TextBright);
 
            var cardRef = card;
            button.Pressed += () => SelectCard(cardRef, id);
            _cardList.AddChild(button);
        }
    }
 
    public override void Activate()
    {
        _sidebarScroll.Visible = true;
        _opacityLabel.Visible = true;
        _opacityInput.Visible = true;
        BuildSidebarItems(); // lazy build
    }
 
    public override void Deactivate()
    {
        _sidebarScroll.Visible = false;
        _opacityLabel.Visible = false;
        _opacityInput.Visible = false;
        ClearPreview();
        _selectedButton = null;
    }
 
    public override void Process(double delta) { /* Cards are static — nothing to update per frame. */ }
 
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
        _sidebarScroll = new ScrollContainer
        {
            Position = new Vector2(0, 62),
            Size = new Vector2(EditorTheme.SidebarWidth, screenSize.Y - 62),
            HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled,
            Visible = false
        };
        Screen.AddChild(_sidebarScroll);
 
        _cardList = new VBoxContainer { CustomMinimumSize = new Vector2(EditorTheme.SidebarWidth - 20, 0) };
        _cardList.AddThemeConstantOverride("separation", 2);
        _sidebarScroll.AddChild(_cardList);
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
    //  CARD SELECTION
    // ═══════════════════════════════════════════════
 
    private void SelectCard(CardModel card, string cardId)
    {
        ClearPreview();
        HighlightButton(cardId);
 
        _currentCardId = cardId;
        _currentCardModel = card;
        Screen.ResetZoomPan();
 
        _previewCard = NCard.Create(card);
        if (_previewCard == null)
        {
            SetInfo("Failed to create card preview for " + cardId);
            return;
        }
 
        _previewCard.SetMeta("googly_editor_preview", true);
        StripExistingGooglyEyes(_previewCard);
 
        PreviewRoot.AddChild(_previewCard);
        _previewCard.Position = new Vector2(
            (PreviewArea.Size.X - CardSize.X) / 2f,
            (PreviewArea.Size.Y - CardSize.Y) / 2f + 180f
        );
        _previewCard.UpdateVisuals(PileType.None, CardPreviewMode.Normal);
 
        SetInfo(cardId + " loaded — click the card to place eyes!");
        AdvanceWorkflow(WorkflowStep.PlaceEyes);
 
        if (LoadPresets(cardId))
        {
            SetInfo(cardId + " loaded with " + _placements.Count + " preset eye(s).");
            if (_placements.Count > 0) AdvanceWorkflow(WorkflowStep.AdjustEyes);
        }
    }
 
    private static void StripExistingGooglyEyes(NCard card)
    {
        var body = card.Body;
        if (body == null) return;
 
        for (int i = body.GetChildCount() - 1; i >= 0; i--)
        {
            var child = body.GetChild(i);
            var name = child.Name.ToString();
            if (child is CardEyeDriver || name.Contains("Googly") || name.StartsWith("@Control@"))
            {
                body.RemoveChild(child);
                child.Free();
            }
        }
    }
 
    private void HighlightButton(string cardId)
    {
        if (_selectedButton != null && GodotObject.IsInstanceValid(_selectedButton))
        {
            bool prev = CardGooglyEyesRegistry.Configs.ContainsKey(_selectedButton.Text);
            ApplyFont(_selectedButton, 12, prev ? EditorTheme.AccentGreen : EditorTheme.TextNormal);
        }
        foreach (var child in _cardList.GetChildren())
        {
            if (child is Button btn && btn.Text == cardId)
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
        if (_previewCard != null && GodotObject.IsInstanceValid(_previewCard))
        {
            _previewCard.QueueFree();
            _previewCard = null;
        }
        _currentCardId = null;
        _currentCardModel = null;
    }
 
    // ═══════════════════════════════════════════════
    //  PRESET LOADING
    // ═══════════════════════════════════════════════
 
    private bool LoadPresets(string cardId)
    {
        if (!CardGooglyEyesRegistry.Configs.TryGetValue(cardId, out var configs)) return false;
        if (configs.Length == 0 || _previewCard == null) return false;
 
        foreach (var c in configs)
            PlaceEye(CardToContent(c.Offset), c.Scale, c.Opacity);
 
        return _placements.Count > 0;
    }
 
    // ═══════════════════════════════════════════════
    //  COORDINATE HELPERS
    // ═══════════════════════════════════════════════
 
    private Vector2 CardToContent(Vector2 cardOffset) =>
        _previewCard != null ? _previewCard.Position + CardCenter + cardOffset : Vector2.Zero;
 
    private Vector2 ContentToCard(Vector2 contentPos) =>
        _previewCard != null ? contentPos - _previewCard.Position - CardCenter : Vector2.Zero;
 
    // ═══════════════════════════════════════════════
    //  INPUT
    // ═══════════════════════════════════════════════
 
    public override bool HasEyeAt(Vector2 contentPos) => FindEyeAt(contentPos) != null;
 
    public override bool HandleInput(InputEvent @event, Vector2 localPos, Vector2 contentPos)
    {
        if (_previewCard == null) return false;
 
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
                    _dragging.CardOffset = ContentToCard(_dragging.Position);
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
        if (_placements.Count == 0 || string.IsNullOrEmpty(_currentCardId))
        {
            SetOutput("Nothing to export.");
            return;
        }
        AdvanceWorkflow(WorkflowStep.Export);
 
        var sb = new StringBuilder();
        sb.AppendLine("// Googly Eyes config for card: " + _currentCardId);
        sb.AppendLine("{ \"" + _currentCardId + "\", new CardEyeConfig[] {");
 
        foreach (var p in _placements)
        {
            var offset = ContentToCard(p.Position);
            sb.Append("    new CardEyeConfig { ");
            sb.Append("Offset = new Vector2(" + offset.X.ToString("F1") + "f, " + offset.Y.ToString("F1") + "f), ");
            sb.Append("Scale = " + p.Scale.ToString("F2") + "f");
            if (p.Opacity < 0.99f) sb.Append(", Opacity = " + p.Opacity.ToString("F2") + "f");
            sb.AppendLine(" },");
        }
        sb.AppendLine("}},");
 
        GD.Print("[GooglyEyes] === CARD EXPORT ===");
        GD.Print(sb.ToString());
        GD.Print("[GooglyEyes] === END CARD EXPORT ===");
 
        SetOutput("Exported " + _placements.Count + " card eye(s) — check console!");
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
        PlaceEye(_selectedEye.Position + new Vector2(30, 30), _selectedEye.Scale, _selectedEye.Opacity);
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
 
    private CardEyePlacement FindEyeAt(Vector2 pos)
    {
        float bestDist = float.MaxValue;
        CardEyePlacement best = null;
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
 
        _placements.Add(new CardEyePlacement
        {
            Position = contentPos, Scale = scale, Opacity = opacity,
            EyeSprite = container.GetChild<Sprite2D>(0),
            IrisSprite = container.GetChild<Sprite2D>(1),
            Container = container,
            CardOffset = ContentToCard(contentPos)
        });
        SetInfo(_placements.Count + " eye(s) placed.");
    }
 
    private void RemoveEye(CardEyePlacement p)
    {
        if (p == _selectedEye) DeselectCurrent();
        if (GodotObject.IsInstanceValid(p.Container)) p.Container.QueueFree();
        _placements.Remove(p);
        SetInfo(_placements.Count + " eye(s) remaining.");
    }
 
    private void ResizeEye(CardEyePlacement p, float delta)
    {
        p.Scale = Mathf.Max(0.01f, p.Scale + delta * 0.5f);
        p.Container.Scale = Vector2.One * p.Scale;
        if (p == _selectedEye) Screen.SetScaleValueNoSignal(p.Scale);
    }
 
    private void SelectEye(CardEyePlacement eye)
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