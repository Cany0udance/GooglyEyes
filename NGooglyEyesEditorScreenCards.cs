using System.Text;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace GooglyEyes;

/// <summary>
/// Card-mode functionality for the googly eyes editor.
/// Partial class extension of NGooglyEyesEditorScreen.
///
/// Cards don't have skeletons, bones, or animations — just a fixed 300x422
/// Control with a portrait. Eye placement is a simple offset from card center.
/// </summary>
public partial class NGooglyEyesEditorScreen
{
    // ════════════════════════════════════════════════
    //  CARD MODE STATE
    // ════════════════════════════════════════════════
 
    private enum EditorMode { Monsters, Cards }
    private EditorMode _editorMode = EditorMode.Monsters;
 
    private static readonly Vector2 CardSize = NCard.defaultSize; // (300, 422)
    private static readonly Vector2 CardCenter = CardSize / 2f;   // (150, 211)
    public static bool SuppressCardPatch;
 
    // Tab bar
    private Button _monsterTabButton;
    private Button _cardTabButton;
 
    // Card-specific UI
    private VBoxContainer _cardList;
    private ScrollContainer _cardSidebarScroll;
    private NCard _previewCard;
    private string _currentCardId;
    private CardModel _currentCardModel;
 
    // Card opacity control
    private SpinBox _cardOpacityInput;
    private Label _cardOpacityLabel;
 
    // Card eye placements (separate from monster _placements)
    private readonly List<CardEyePlacement> _cardPlacements = new();
    private CardEyePlacement _selectedCardEye;
    private CardEyePlacement _draggingCardEye;
    private bool _isDraggingCardEye;
 
    private class CardEyePlacement
    {
        /// <summary>Position in preview-root content space.</summary>
        public Vector2 Position;
        public float Scale = 1f;
        public float Opacity = 1f;
        public Sprite2D EyeSprite;
        public Sprite2D IrisSprite;
        public Node2D Container;
        /// <summary>Offset from card center in card-local pixels.</summary>
        public Vector2 CardOffset;
    }
 
    // ════════════════════════════════════════════════
    //  TAB BAR (called from main _Ready)
    // ════════════════════════════════════════════════
 
    /// <summary>
    /// Call this from the main _Ready(), after BuildSidebar but before BuildMonsterList.
    /// Inserts a tab bar at the top of the sidebar to switch between Monsters and Cards.
    /// </summary>
    private void BuildTabBar()
    {
        var tabContainer = new HBoxContainer
        {
            Position = new Vector2(Padding, Padding),
            Size = new Vector2(SidebarWidth - Padding * 2, 30)
        };
        tabContainer.AddThemeConstantOverride("separation", 4);
        AddChild(tabContainer);
 
        _monsterTabButton = new Button
        {
            Text = "Monsters",
            ToggleMode = true,
            ButtonPressed = true,
            CustomMinimumSize = new Vector2((SidebarWidth - Padding * 2 - 4) / 2f, 28),
            TooltipText = "Place googly eyes on monster sprites."
        };
        if (_font != null) _monsterTabButton.AddThemeFontOverride("font", _font);
        _monsterTabButton.AddThemeFontSizeOverride("font_size", 13);
        _monsterTabButton.Pressed += () => SwitchToMode(EditorMode.Monsters);
        tabContainer.AddChild(_monsterTabButton);
 
        _cardTabButton = new Button
        {
            Text = "Cards",
            ToggleMode = true,
            ButtonPressed = false,
            CustomMinimumSize = new Vector2((SidebarWidth - Padding * 2 - 4) / 2f, 28),
            TooltipText = "Place googly eyes on card art."
        };
        if (_font != null) _cardTabButton.AddThemeFontOverride("font", _font);
        _cardTabButton.AddThemeFontSizeOverride("font_size", 13);
        _cardTabButton.Pressed += () => SwitchToMode(EditorMode.Cards);
        tabContainer.AddChild(_cardTabButton);
 
        BuildCardSidebar();

    }
 
    private void BuildCardSidebar()
    {
        _cardSidebarScroll = new ScrollContainer
        {
            Position = new Vector2(0, 62),
            Size = new Vector2(SidebarWidth, GetViewportRect().Size.Y - 62),
            HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled,
            Visible = false
        };
        AddChild(_cardSidebarScroll);
 
        _cardList = new VBoxContainer { CustomMinimumSize = new Vector2(SidebarWidth - 20, 0) };
        _cardList.AddThemeConstantOverride("separation", 2);
        _cardSidebarScroll.AddChild(_cardList);
    }
 
    private void BuildCardOpacityInput()
    {
        var screenSize = GetViewportRect().Size;
        float panelX = SidebarWidth + Padding;
        float panelY = screenSize.Y - 278;
        float row = panelY + 24;
 
        _cardOpacityLabel = MakeLabel("Opacity:", new Vector2(panelX + 600, row + 3), new Vector2(60, 24), 13, TextNormal);
        _cardOpacityLabel.Visible = false;
        AddChild(_cardOpacityLabel);
 
        _cardOpacityInput = new SpinBox
        {
            MinValue = 0.0, MaxValue = 1.0, Step = 0.05, Value = 1.0,
            Position = new Vector2(panelX + 660, row - 2), Size = new Vector2(90, 30),
            Editable = true, TooltipText = "Eye opacity. 1.0 = fully visible, 0.0 = invisible.",
            Visible = false
        };
        if (_font != null) _cardOpacityInput.AddThemeFontOverride("font", _font);
        _cardOpacityInput.AddThemeFontSizeOverride("font_size", 12);
        _cardOpacityInput.ValueChanged += OnCardOpacityChanged;
        AddChild(_cardOpacityInput);
    }
 
    /// <summary>
    /// Populates the card sidebar with all cards from the game.
    /// Called lazily on first switch to Cards mode.
    /// </summary>
    private bool _cardListBuilt;
    private void BuildCardList()
    {
        if (_cardListBuilt) return;
        _cardListBuilt = true;

        foreach (var card in ModelDb.AllCards.OrderBy(c => c.Id.Entry))
        {
            var cardId = card.Id.Entry;
            bool hasConfig = CardGooglyEyesRegistry.Configs.ContainsKey(cardId);

            var button = new Button
            {
                Text = cardId,
                Flat = true,
                Alignment = HorizontalAlignment.Left,
                CustomMinimumSize = new Vector2(0, 28),
                SizeFlagsHorizontal = SizeFlags.ExpandFill
            };
            ApplyFont(button, 12, hasConfig ? AccentGreen : TextNormal);
            button.AddThemeColorOverride("font_hover_color", TextBright);

            var cardRef = card;
            var id = cardId;
            button.Pressed += () => SelectCard(cardRef, id);
            _cardList.AddChild(button);
        }
    }
 
    // ════════════════════════════════════════════════
    //  MODE SWITCHING
    // ════════════════════════════════════════════════
 
    private void SwitchToMode(EditorMode mode)
    {
        if (_editorMode == mode) return;
        _editorMode = mode;
 
        _monsterTabButton.SetPressedNoSignal(mode == EditorMode.Monsters);
        _cardTabButton.SetPressedNoSignal(mode == EditorMode.Cards);
        
        if (_selectedSidebarButton != null && IsInstanceValid(_selectedSidebarButton))
        {
            bool isMonster = _editorMode == EditorMode.Monsters;
            bool prevHasConfig = isMonster
                ? GooglyEyesRegistry.Configs.ContainsKey(_selectedSidebarButton.Text)
                : CardGooglyEyesRegistry.Configs.ContainsKey(_selectedSidebarButton.Text);
            ApplyFont(_selectedSidebarButton, isMonster ? 14 : 12, prevHasConfig ? AccentGreen : TextNormal);
            _selectedSidebarButton = null;
        }
 
        if (mode == EditorMode.Cards)
        {
            _sidebarScroll.Visible = false;
            _cardSidebarScroll.Visible = true;
            _cardOpacityLabel.Visible = true;
            _cardOpacityInput.Visible = true;
            BuildCardList();
 
            SetMonsterPanelsVisible(false);
 
            if (_currentVisuals != null && IsInstanceValid(_currentVisuals))
            {
                ClearAllPlacements();
                ClearBoneMarkers();
                _currentVisuals.QueueFree();
                _currentVisuals = null;
            }
            _currentMonsterId = null;
 
            _infoLabel.Text = "Select a card from the sidebar to begin.";
            UpdateWorkflowStep(WorkflowStep.SelectMonster);
        }
        else
        {
            _cardSidebarScroll.Visible = false;
            _sidebarScroll.Visible = true;
            _cardOpacityLabel.Visible = false;
            _cardOpacityInput.Visible = false;
 
            SetMonsterPanelsVisible(true);
 
            ClearCardPreview();
 
            _infoLabel.Text = "Select a monster from the sidebar to get started.";
            UpdateWorkflowStep(WorkflowStep.SelectMonster);
        }
    }
 
    /// <summary>
    /// Hides or shows the monster-only panels (animation controls, bone panel).
    /// </summary>
    private void SetMonsterPanelsVisible(bool visible)
    {
        _animDropdown.Visible = visible;
        _frameSlider.Visible = visible;
        _frameLabel.Visible = visible;
        _playPauseButton.Visible = visible;
        _animPanelHint.Visible = visible;
        _track1Dropdown.Visible = visible;
 
        _anchorBoneInput.Visible = visible;
        _showBonesButton.Visible = visible;
        _showAllBoneNamesCheckbox.Visible = visible;
        _anchorPosLabel.Visible = visible;
        _anchorPanelHint.Visible = visible;
        _splitHereButton.Visible = visible;
        _clearSegmentsButton.Visible = visible;
        _activeSegmentLabel.Visible = visible;
        _segmentScroll.Visible = visible;
        _hiddenByDefaultCheckbox.Visible = visible;
    }
 
    // ════════════════════════════════════════════════
    //  CARD SELECTION
    // ════════════════════════════════════════════════
 
    private void SelectCard(CardModel card, string cardId)
    {
        ClearCardPreview();
 
        foreach (var child in _cardList.GetChildren())
        {
            if (child is Button btn && btn.Text == cardId)
            {
                HighlightSidebarButton(btn, cardId, false);
                break;
            }
        }
        
        _currentCardId = cardId;
        _currentCardModel = card;
 
        _zoom = 1f;
        _panOffset = Vector2.Zero;
        ApplyZoomPan();
 
        _previewCard = NCard.Create(card);
        if (_previewCard != null)
        {
            _previewCard.SetMeta("googly_editor_preview", true);

            var body = _previewCard.Body;
            if (body != null)
            {
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
        }
        if (_previewCard == null)
        {
            _infoLabel.Text = "Failed to create card preview for " + cardId;
            return;
        }
 
        _previewRoot.AddChild(_previewCard);
 
        _previewCard.Position = new Vector2(
            (_previewArea.Size.X - CardSize.X) / 2f,
            (_previewArea.Size.Y - CardSize.Y) / 2f + 180f
        );
        _previewCard.UpdateVisuals(PileType.None, CardPreviewMode.Normal);
 
        _infoLabel.Text = cardId + " loaded — click the card to place eyes!";
        UpdateWorkflowStep(WorkflowStep.PlaceEyes);
 
        if (LoadCardPresets(cardId))
        {
            _infoLabel.Text = cardId + " loaded with " + _cardPlacements.Count + " preset eye(s). Adjust as needed.";
            if (_cardPlacements.Count > 0)
                UpdateWorkflowStep(WorkflowStep.AdjustEyes);
        }
    }
 
    private void ClearCardPreview()
    {
        ClearAllCardPlacements();
        DeselectCardEye();
 
        if (_previewCard != null && IsInstanceValid(_previewCard))
        {
            _previewCard.QueueFree();
            _previewCard = null;
        }
        _currentCardId = null;
        _currentCardModel = null;
    }
 
    // ════════════════════════════════════════════════
    //  CARD PRESET LOADING
    // ════════════════════════════════════════════════
 
    private bool LoadCardPresets(string cardId)
    {
        if (!CardGooglyEyesRegistry.Configs.TryGetValue(cardId, out var configs)) return false;
        if (configs.Length == 0) return false;
        if (_previewCard == null) return false;
 
        foreach (var config in configs)
        {
            var contentPos = CardToContentPos(config.Offset);
            PlaceCardEye(contentPos, config.Scale, config.Opacity);
        }
 
        return _cardPlacements.Count > 0;
    }
 
    // ════════════════════════════════════════════════
    //  CARD COORDINATE HELPERS
    // ════════════════════════════════════════════════
 
    /// <summary>
    /// Converts a card-local offset (from card center) to preview content-space position.
    /// </summary>
    private Vector2 CardToContentPos(Vector2 cardOffset)
    {
        if (_previewCard == null) return Vector2.Zero;
        return _previewCard.Position + CardCenter + cardOffset;
    }
 
    /// <summary>
    /// Converts a content-space position back to a card-local offset from center.
    /// </summary>
    private Vector2 ContentPosToCardOffset(Vector2 contentPos)
    {
        if (_previewCard == null) return Vector2.Zero;
        return contentPos - _previewCard.Position - CardCenter;
    }
 
    // ════════════════════════════════════════════════
    //  CARD EYE PLACEMENT
    // ════════════════════════════════════════════════
 
    private void PlaceCardEye(Vector2 contentPos, float scale = 1f, float opacity = 1f)
    {
        var container = new Node2D { Position = contentPos };
        container.AddChild(new Sprite2D { Texture = _eyeTexture, Name = "Eye" });
        container.AddChild(new Sprite2D { Texture = _irisTexture, Name = "Iris", ZIndex = 1 });
        container.Scale = Vector2.One * scale;
        container.Modulate = new Color(1f, 1f, 1f, opacity);
        _previewRoot.AddChild(container);
 
        var placement = new CardEyePlacement
        {
            Position = contentPos,
            Scale = scale,
            Opacity = opacity,
            EyeSprite = container.GetChild<Sprite2D>(0),
            IrisSprite = container.GetChild<Sprite2D>(1),
            Container = container,
            CardOffset = ContentPosToCardOffset(contentPos)
        };
 
        _cardPlacements.Add(placement);
        _infoLabel.Text = _cardPlacements.Count + " eye(s) placed.";
    }
 
    private void RemoveCardEye(CardEyePlacement p)
    {
        if (p == _selectedCardEye) DeselectCardEye();
        if (IsInstanceValid(p.Container)) p.Container.QueueFree();
        _cardPlacements.Remove(p);
        _infoLabel.Text = _cardPlacements.Count + " eye(s) remaining.";
    }
 
    private void ClearAllCardPlacements()
    {
        foreach (var p in _cardPlacements)
            if (IsInstanceValid(p.Container)) p.Container.QueueFree();
        _cardPlacements.Clear();
        DeselectCardEye();
    }
 
    private void ResizeCardEye(CardEyePlacement p, float delta)
    {
        p.Scale = Mathf.Max(0.01f, p.Scale + delta * 0.5f);
        p.Container.Scale = Vector2.One * p.Scale;
        if (p == _selectedCardEye) _scaleInput.SetValueNoSignal(p.Scale);
    }
 
    private CardEyePlacement FindCardEyeAt(Vector2 pos)
    {
        float bestDist = float.MaxValue;
        CardEyePlacement best = null;
        foreach (var p in _cardPlacements)
        {
            float radius = (_eyeTexture.GetWidth() / 2f) * p.Scale;
            float dist = p.Position.DistanceTo(pos);
            if (dist < radius && dist < bestDist) { bestDist = dist; best = p; }
        }
        return best;
    }
 
    private void SelectCardEye(CardEyePlacement eye)
    {
        _selectedCardEye = eye;
        _scaleInput.SetValueNoSignal(eye.Scale);
        _cardOpacityInput.SetValueNoSignal(eye.Opacity);
 
        int idx = _cardPlacements.IndexOf(eye) + 1;
        _selectedLabel.Text = "Eye #" + idx;
        ApplyFont(_selectedLabel, 13, Accent);
        _selectionPanelHint.Text = "Drag to reposition. Scroll to resize.";
    }
 
    private void DeselectCardEye()
    {
        _selectedCardEye = null;
        if (_editorMode == EditorMode.Cards)
        {
            _selectedLabel.Text = "No eye selected";
            ApplyFont(_selectedLabel, 13, TextDim);
            _scaleInput.SetValueNoSignal(1.0);
            _cardOpacityInput.SetValueNoSignal(1.0);
            _selectionPanelHint.Text = "Click an eye to select it.";
        }
    }
 
    // ════════════════════════════════════════════════
    //  CARD INPUT HANDLING
    // ════════════════════════════════════════════════
 
    /// <summary>
    /// Handles input for card mode. Returns true if input was consumed.
    /// </summary>
    private bool HandleCardInput(InputEvent @event, Vector2 localPos, Vector2 contentPos)
    {
        if (_previewCard == null) return false;
 
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
            {
                var hit = FindCardEyeAt(contentPos);
                if (hit != null)
                {
                    SelectCardEye(hit);
                    _draggingCardEye = hit;
                    _isDraggingCardEye = true;
                    UpdateWorkflowStep(WorkflowStep.AdjustEyes);
                }
                else
                {
                    DeselectCardEye();
                    PlaceCardEye(contentPos);
                    SelectCardEye(_cardPlacements[^1]);
                    if (_currentStep == WorkflowStep.PlaceEyes && _cardPlacements.Count >= 2)
                        UpdateWorkflowStep(WorkflowStep.AdjustEyes);
                }
                return true;
            }
            else if (mouseButton.ButtonIndex == MouseButton.Left && !mouseButton.Pressed)
            {
                if (_isDraggingCardEye && _draggingCardEye != null)
                {
                    _draggingCardEye.CardOffset = ContentPosToCardOffset(_draggingCardEye.Position);
                }
                _isDraggingCardEye = false;
                _draggingCardEye = null;
                return true;
            }
            else if (mouseButton.ButtonIndex == MouseButton.Right && mouseButton.Pressed)
            {
                var hit = FindCardEyeAt(contentPos);
                if (hit != null) { RemoveCardEye(hit); return true; }
            }
            else if (mouseButton.ButtonIndex == MouseButton.WheelUp)
            {
                var hit = FindCardEyeAt(contentPos);
                if (hit != null) { ResizeCardEye(hit, 0.1f); return true; }
            }
            else if (mouseButton.ButtonIndex == MouseButton.WheelDown)
            {
                var hit = FindCardEyeAt(contentPos);
                if (hit != null) { ResizeCardEye(hit, -0.1f); return true; }
            }
        }
        else if (@event is InputEventMouseMotion)
        {
            if (_isDraggingCardEye && _draggingCardEye != null)
            {
                _draggingCardEye.Position = contentPos;
                _draggingCardEye.Container.Position = contentPos;
                return true;
            }
        }
 
        return false;
    }
 
    /// <summary>
    /// Handles scale input changes when in card mode.
    /// </summary>
    private void OnCardScaleChanged(double value)
    {
        if (_selectedCardEye == null) return;
        _selectedCardEye.Scale = (float)value;
        _selectedCardEye.Container.Scale = Vector2.One * _selectedCardEye.Scale;
    }
 
    private void OnCardOpacityChanged(double value)
    {
        if (_selectedCardEye == null) return;
        _selectedCardEye.Opacity = (float)value;
        _selectedCardEye.Container.Modulate = new Color(1f, 1f, 1f, _selectedCardEye.Opacity);
    }
 
    // ════════════════════════════════════════════════
    //  CARD EXPORT
    // ════════════════════════════════════════════════
 
    private void ExportCardConfig()
    {
        if (_cardPlacements.Count == 0 || string.IsNullOrEmpty(_currentCardId))
        {
            _outputLabel.Text = "Nothing to export.";
            return;
        }
 
        UpdateWorkflowStep(WorkflowStep.Export);
 
        var sb = new StringBuilder();
        sb.AppendLine("// Googly Eyes config for card: " + _currentCardId);
        sb.AppendLine("{ \"" + _currentCardId + "\", new CardEyeConfig[] {");
 
        foreach (var p in _cardPlacements)
        {
            var offset = ContentPosToCardOffset(p.Position);
            sb.Append("    new CardEyeConfig { ");
            sb.Append("Offset = new Vector2(" + offset.X.ToString("F1") + "f, " + offset.Y.ToString("F1") + "f), ");
            sb.Append("Scale = " + p.Scale.ToString("F2") + "f");
            if (p.Opacity < 0.99f)
                sb.Append(", Opacity = " + p.Opacity.ToString("F2") + "f");
            sb.AppendLine(" },");
        }
 
        sb.AppendLine("}},");
 
        GD.Print("[GooglyEyes] === CARD EXPORT ===");
        GD.Print(sb.ToString());
        GD.Print("[GooglyEyes] === END CARD EXPORT ===");
 
        _outputLabel.Text = "Exported " + _cardPlacements.Count + " card eye(s) — check console!";
    }
 
    // ════════════════════════════════════════════════
    //  CARD CLEANUP
    // ════════════════════════════════════════════════
 
    private void CleanupCardMode()
    {
        ClearCardPreview();
    }
}