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
        { "BING_BONG", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(-9.5f, -15.6f), Scale = 0.11f },
            new RelicEyeConfig { Offset = new Vector2(23.2f, -0.4f), Scale = 0.10f },
        }},
        { "BONE_TEA", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(-0.5f, 6.3f), Scale = 0.03f },
            new RelicEyeConfig { Offset = new Vector2(-11.1f, 6.2f), Scale = 0.03f },
        }},
        { "BYRDPIP", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(6.9f, 1.0f), Scale = 0.05f },
        }},
        { "DAUGHTER_OF_THE_WIND", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(3.5f, -15.7f), Scale = 0.05f },
            new RelicEyeConfig { Offset = new Vector2(13.5f, -6.5f), Scale = 0.05f },
        }},
        { "ELECTRIC_SHRYMP", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(-12.8f, -9.8f), Scale = 0.06f },
            new RelicEyeConfig { Offset = new Vector2(-18.3f, 0.8f), Scale = 0.03f },
        }},
        { "FAKE_SNECKO_EYE", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(-5.2f, -2.2f), Scale = 0.20f },
        }},
        { "FAKE_STRIKE_DUMMY", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(0.4f, -11.4f), Scale = 0.03f },
            new RelicEyeConfig { Offset = new Vector2(-6.7f, -10.5f), Scale = 0.03f },
        }},
        { "FORGOTTEN_SOUL", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(-4.7f, -19.4f), Scale = 0.04f },
            new RelicEyeConfig { Offset = new Vector2(-7.9f, -6.6f), Scale = 0.04f },
        }},
        { "FUNERARY_MASK", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(-9.9f, -8.8f), Scale = 0.07f },
            new RelicEyeConfig { Offset = new Vector2(13.7f, 5.4f), Scale = 0.07f },
        }},
        { "GAME_PIECE", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(-6.4f, -12.9f), Scale = 0.03f },
            new RelicEyeConfig { Offset = new Vector2(0.3f, -16.3f), Scale = 0.03f },
        }},
        { "GIRYA", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(-8.4f, 7.9f), Scale = 0.05f },
            new RelicEyeConfig { Offset = new Vector2(5.1f, 8.2f), Scale = 0.06f },
        }},
        { "GLASS_EYE", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(0.2f, -0.7f), Scale = 0.25f },
        }},
        { "HAPPY_FLOWER", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(4.5f, -19.4f), Scale = 0.03f },
            new RelicEyeConfig { Offset = new Vector2(-2.5f, -19.0f), Scale = 0.03f },
        }},
        { "INTIMIDATING_HELMET", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(1.7f, 3.5f), Scale = 0.10f },
            new RelicEyeConfig { Offset = new Vector2(23.9f, 6.5f), Scale = 0.06f },
        }},
        { "JEWELED_MASK", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(15.4f, -7.9f), Scale = 0.10f },
            new RelicEyeConfig { Offset = new Vector2(-4.9f, 7.7f), Scale = 0.10f },
        }},
        { "LUCKY_FYSH", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(-12.0f, -16.0f), Scale = 0.04f },
            new RelicEyeConfig { Offset = new Vector2(-0.4f, -15.9f), Scale = 0.04f },
        }},
        { "MAW_BANK", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(-20.8f, -6.9f), Scale = 0.05f },
            new RelicEyeConfig { Offset = new Vector2(-8.7f, -6.0f), Scale = 0.05f },
        }},
        { "MEAL_TICKET", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(-1.8f, -5.2f), Scale = 0.02f },
            new RelicEyeConfig { Offset = new Vector2(-5.9f, -3.1f), Scale = 0.02f },
        }},
        { "MINI_REGENT", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(-11.5f, -13.3f), Scale = 0.03f },
        }},
        { "MR_STRUGGLES", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(-15.8f, -7.7f), Scale = 0.06f },
            new RelicEyeConfig { Offset = new Vector2(-3.6f, -18.2f), Scale = 0.06f },
        }},
        { "PAELS_LEGION", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(-2.6f, -8.0f), Scale = 0.07f },
            new RelicEyeConfig { Offset = new Vector2(20.2f, -9.5f), Scale = 0.07f },
        }},
        { "PAPER_KRANE", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(-12.3f, -13.0f), Scale = 0.03f },
        }},
        { "PETRIFIED_TOAD", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(-16.1f, 9.7f), Scale = 0.06f },
        }},
        { "RED_MASK", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(3.1f, 12.2f), Scale = 0.06f },
            new RelicEyeConfig { Offset = new Vector2(-13.4f, 8.0f), Scale = 0.05f },
        }},
        { "RED_SKULL", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(-2.6f, -1.5f), Scale = 0.07f },
            new RelicEyeConfig { Offset = new Vector2(-17.4f, -2.3f), Scale = 0.07f },
        }},
        { "REPTILE_TRINKET", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(6.4f, -18.9f), Scale = 0.05f },
            new RelicEyeConfig { Offset = new Vector2(-5.2f, -12.0f), Scale = 0.05f },
        }},
        { "RING_OF_THE_DRAKE", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(-16.7f, -6.7f), Scale = 0.04f },
        }},
        { "RING_OF_THE_SNAKE", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(-4.9f, -15.9f), Scale = 0.05f },
        }},
        { "RUINED_HELMET", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(-4.4f, 7.0f), Scale = 0.10f },
            new RelicEyeConfig { Offset = new Vector2(18.2f, 6.2f), Scale = 0.06f },
        }},
        { "SNECKO_EYE", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(1.0f, -1.0f), Scale = 0.29f },
        }},
        { "SNECKO_SKULL", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(2.0f, -8.0f), Scale = 0.07f },
        }},
        { "THE_COURIER", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(-15.0f, -14.9f), Scale = 0.05f },
        }},
        { "TOY_BOX", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(15.1f, -16.8f), Scale = 0.02f },
            new RelicEyeConfig { Offset = new Vector2(10.7f, -17.4f), Scale = 0.02f },
        }},
        { "WHITE_BEAST_STATUE", new RelicEyeConfig[] {
            new RelicEyeConfig { Offset = new Vector2(-16.4f, 1.8f), Scale = 0.03f },
        }},
    };
}