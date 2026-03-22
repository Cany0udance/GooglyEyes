using Godot;

namespace GooglyEyes;

/// <summary>
/// Shared color palette and layout constants for the googly eyes editor.
/// </summary>
public static class EditorTheme
{
    public const float Padding = 16f;
    public const float SidebarWidth = 280f;
 
    // ── Backgrounds ──
    public static readonly Color BgColor        = new(0.07f, 0.07f, 0.1f, 1f);
    public static readonly Color PanelBg        = new(0.1f, 0.1f, 0.15f, 1f);
    public static readonly Color PreviewBg      = new(0.05f, 0.05f, 0.07f, 1f);
    public static readonly Color HelpBg         = new(0.12f, 0.14f, 0.22f, 0.95f);
 
    // ── Accents ──
    public static readonly Color Accent         = new(0.4f, 0.55f, 0.95f, 1f);
    public static readonly Color AccentWarm     = new(0.95f, 0.6f, 0.3f, 1f);
    public static readonly Color AccentGreen    = new(0.3f, 0.75f, 0.45f, 1f);
 
    // ── Text ──
    public static readonly Color TextBright     = new(0.9f, 0.9f, 0.93f, 1f);
    public static readonly Color TextNormal     = new(0.72f, 0.72f, 0.78f, 1f);
    public static readonly Color TextDim        = new(0.45f, 0.45f, 0.52f, 1f);
 
    // ── Workflow steps ──
    public static readonly Color StepActiveBg   = new(0.4f, 0.55f, 0.95f, 0.25f);
    public static readonly Color StepInactiveBg = new(0.1f, 0.1f, 0.15f, 0.5f);
    public static readonly Color StepCompleteBg = new(0.3f, 0.7f, 0.4f, 0.25f);
    public static readonly Color StepCompleteText = new(0.5f, 0.8f, 0.55f, 1f);
 
    // ── Segments ──
    public static readonly Color SegmentActiveBg   = new(0.4f, 0.55f, 0.95f, 0.3f);
    public static readonly Color SegmentInactiveBg = new(0.12f, 0.12f, 0.18f, 0.5f);
 
    // ── Misc ──
    public static readonly Color SelectedEntryColor = new(0.95f, 0.85f, 0.3f, 1f);
    public static readonly Color Divider            = new(0.2f, 0.2f, 0.28f, 0.6f);
}
