using Godot;

namespace GooglyEyes;

public class CardEyeConfig
{
    /// <summary>
    /// Offset from the card's center (150, 211) in card-local pixels.
    /// </summary>
    public Vector2 Offset;

    public float Scale;

    /// <summary>
    /// 1.0 = fully opaque, 0.0 = invisible. Default is 1.0.
    /// </summary>
    public float Opacity = 1f;
}