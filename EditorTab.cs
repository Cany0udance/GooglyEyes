using Godot;

namespace GooglyEyes;

/// <summary>
/// Base class for an editor tab (Monsters, Cards, or any future entity type).
/// Each tab owns its own sidebar content, mode-specific panels, placement list,
/// input handling, and export logic.
/// </summary>
public abstract class EditorTab
{
    // ── References set by the screen on registration ──
    protected NGooglyEyesEditorScreen Screen;
    protected Font Font;
    protected Texture2D EyeTexture;
    protected Texture2D IrisTexture;
 
    // ── Convenience accessors ──
    protected Node2D PreviewRoot => Screen.PreviewRoot;
    protected Control PreviewArea => Screen.PreviewArea;
 
    /// <summary>Label shown in the tab bar button.</summary>
    public abstract string TabName { get; }
 
    /// <summary>Tooltip for the tab bar button.</summary>
    public abstract string TabTooltip { get; }
 
    // ────────────────────────────────────────────────
    //  Lifecycle — called by the screen
    // ────────────────────────────────────────────────
 
    /// <summary>
    /// Called once during _Ready. Store references and build any persistent UI
    /// (sidebar scroll container, mode-specific panels).
    /// </summary>
    public void Register(NGooglyEyesEditorScreen screen, Font font, Texture2D eye, Texture2D iris)
    {
        Screen = screen;
        Font = font;
        EyeTexture = eye;
        IrisTexture = iris;
        OnRegister();
    }
 
    protected abstract void OnRegister();
 
    /// <summary>Build sidebar items (buttons for each entity). May be called lazily.</summary>
    public abstract void BuildSidebarItems();
 
    /// <summary>Show this tab's sidebar and panels.</summary>
    public abstract void Activate();
 
    /// <summary>Hide this tab's sidebar and panels.</summary>
    public abstract void Deactivate();
 
    /// <summary>Called every frame while this tab is active.</summary>
    public abstract void Process(double delta);
 
    /// <summary>Full cleanup on screen close.</summary>
    public abstract void Cleanup();
 
    // ────────────────────────────────────────────────
    //  Input
    // ────────────────────────────────────────────────
 
    /// <summary>
    /// Handle input within the preview area. Return true if consumed.
    /// Zoom/pan is handled by the screen; this receives placement-related input.
    /// </summary>
    public abstract bool HandleInput(InputEvent @event, Vector2 localPos, Vector2 contentPos);
 
    /// <summary>
    /// Return the eye placement under <paramref name="contentPos"/>, or null.
    /// Used by the screen to decide whether scroll should resize an eye vs zoom.
    /// </summary>
    public abstract bool HasEyeAt(Vector2 contentPos);
 
    // ────────────────────────────────────────────────
    //  Shared toolbar actions (delegated from screen)
    // ────────────────────────────────────────────────
 
    public abstract void Export();
    public abstract void ClearAll();
    public abstract void UndoLast();
    public abstract void DuplicateSelected();
    public abstract void OnScaleChanged(double value);
    public abstract void DeselectCurrent();
 
    // ────────────────────────────────────────────────
    //  Selection state (read by screen for shared panel)
    // ────────────────────────────────────────────────
 
    public abstract bool HasSelection { get; }
    public abstract string SelectionLabel { get; }
    public abstract float SelectionScale { get; }
    public abstract string SelectionHint { get; }
 
    // ────────────────────────────────────────────────
    //  Helpers available to subclasses
    // ────────────────────────────────────────────────
 
    protected Node2D CreateEyeContainer(Vector2 position, float scale = 1f)
    {
        var container = new Node2D { Position = position };
        container.AddChild(new Sprite2D { Texture = EyeTexture, Name = "Eye" });
        container.AddChild(new Sprite2D { Texture = IrisTexture, Name = "Iris", ZIndex = 1 });
        container.Scale = Vector2.One * scale;
        PreviewRoot.AddChild(container);
        return container;
    }
 
    /// <summary>Hit-test a point against a circular eye region.</summary>
    protected bool IsWithinEye(Vector2 testPoint, Vector2 eyeCenter, float eyeScale)
    {
        float radius = (EyeTexture.GetWidth() / 2f) * eyeScale;
        return testPoint.DistanceTo(eyeCenter) < radius;
    }
 
    // ── UI shorthand (delegates to screen) ──
 
    protected void ApplyFont(Control c, int size, Color color) => Screen.ApplyFont(c, size, color);
    protected Label MakeLabel(string t, Vector2 p, Vector2 s, int fs, Color c) => Screen.MakeLabel(t, p, s, fs, c);
    protected Button MakeButton(string t, Vector2 p, Vector2 s, int fs = 14) => Screen.MakeButton(t, p, s, fs);
    protected void SetInfo(string t) => Screen.SetInfoText(t);
    protected void SetOutput(string t) => Screen.SetOutputText(t);
    protected void AdvanceWorkflow(WorkflowStep s) => Screen.UpdateWorkflowStep(s);
}