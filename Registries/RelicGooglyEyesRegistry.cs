using Godot;

namespace GooglyEyes;

public static class RelicGooglyEyesRegistry
{
    public static readonly Dictionary<string, RelicEyeConfig[]> Configs = new()
    {
        { "AKABEKO", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(-12.9f, -2.6f), Scale = 0.04f },
            new RelicEyeConfig { Offset = new Vector2(-20.9f, -5.0f), Scale = 0.04f },
        }},
        { "BAG_OF_PREPARATION", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(-16.3f, -12.2f), Scale = 0.05f },
            new RelicEyeConfig { Offset = new Vector2(-4.8f, -10.5f), Scale = 0.05f },
        }},
        { "BIG_MUSHROOM", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(7.6f, -14.5f), Scale = 0.08f },
            new RelicEyeConfig { Offset = new Vector2(-9.8f, -6.6f), Scale = 0.08f },
        }},
        { "BIIIG_HUG", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(7.6f, 1.7f), Scale = 0.08f },
            new RelicEyeConfig { Offset = new Vector2(-9.0f, 1.6f), Scale = 0.08f },
        }},
    };
}