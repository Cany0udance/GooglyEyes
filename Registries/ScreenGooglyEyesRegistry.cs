using Godot;

namespace GooglyEyes;

public static class ScreenGooglyEyesRegistry
{
    public static readonly Dictionary<string, EyeConfig[]> Configs = new()
    {
        { "res://scenes/screens/char_select/char_select_bg_ironclad.tscn", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(200.5f, -70.2f), Scale = 1.30f, AnchorBone = "bod4" },
            new EyeConfig { Offset = new Vector2(209.4f, -345.6f), Scale = 1.30f, AnchorBone = "bod4" },
        }},
        { "res://scenes/screens/char_select/char_select_bg_defect.tscn", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(476.5f, 23.7f), Scale = 0.65f, AnchorBone = "head" },
            new EyeConfig { Offset = new Vector2(222.6f, 25.6f), Scale = 1.57f, AnchorBone = "head" },
        }},
        { "res://scenes/screens/char_select/char_select_bg_necrobinder.tscn", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(123.7f, -64.6f), Scale = 0.85f, AnchorBone = "head" },
            new EyeConfig { Offset = new Vector2(115.6f, -251.8f), Scale = 0.78f, AnchorBone = "head" },
        }},
        { "res://scenes/screens/char_select/char_select_bg_regent.tscn", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(11.7f, -7.8f), Scale = 0.87f, AnchorBone = "eye" },
        }},
        { "res://scenes/screens/char_select/char_select_bg_silent.tscn", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-423.4f, -428.1f), Scale = 0.87f, AnchorBone = "shoulder_r" },
        }},
        { "res://scenes/events/background_scenes/darv.tscn", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(473.4f, 237.4f), Scale = 0.30f },
            new EyeConfig { Offset = new Vector2(866.6f, 331.7f), Scale = 0.18f },
            new EyeConfig { Offset = new Vector2(809.6f, 330.4f), Scale = 0.18f },
        }},
        { "res://scenes/events/background_scenes/neow.tscn", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-7.2f, -7.1f), Scale = 0.55f, AnchorBone = "eye a 1" },
            new EyeConfig { Offset = new Vector2(3.2f, -3.1f), Scale = 0.55f, AnchorBone = "eye b 1" },
            new EyeConfig { Offset = new Vector2(2.1f, -3.2f), Scale = 0.55f, AnchorBone = "eye c 1" },
        }},
        { "res://scenes/events/background_scenes/orobas.tscn", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(869.0f, 382.5f), Scale = 1.83f },
        }},
        { "res://scenes/events/background_scenes/pael.tscn", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1065.1f, 426.7f), Scale = 0.50f },
        }},
        { "res://scenes/events/background_scenes/tanx.tscn", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(874.4f, 225.8f), Scale = 0.20f },
            new EyeConfig { Offset = new Vector2(1238.8f, 300.6f), Scale = 0.10f },
            new EyeConfig { Offset = new Vector2(1276.1f, 400.7f), Scale = 0.13f },
        }},
        { "res://scenes/events/background_scenes/tezcatara.tscn", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(4.7f, 9.7f), Scale = 0.99f, AnchorBone = "eye_l" },
            new EyeConfig { Offset = new Vector2(1.2f, -5.0f), Scale = 0.99f, AnchorBone = "eye_r" },
        }},
    };
}