using Godot;

namespace GooglyEyes;

public static class GooglyEyesRegistry
{
    public static readonly Dictionary<string, EyeConfig[]> Configs = new()
    {
        { "TRACKER_RUBY_RAIDER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(0.0f, 0.0f), Scale = 0.30f, AnchorBone = "t_eye",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "hurt", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 0.07f, BoneName = "t_eye", Offset = new Vector2(0.0f, 0.0f) },
                        new BoneSegment { StartTime = 0.07f, EndTime = 0.60f, BoneName = "t_facing_eye_l", Offset = new Vector2(0.0f, 0.0f) },
                        new BoneSegment { StartTime = 0.60f, EndTime = 1.00f, BoneName = "t_eye", Offset = new Vector2(0.0f, 0.0f) },
                    } },
                } },
            new EyeConfig { Offset = new Vector2(2.4f, 3.4f), Scale = 0.30f, AnchorBone = "b_eye" },
            new EyeConfig { Offset = new Vector2(238.1f, 832.2f), Scale = 0.30f, AnchorBone = "t_facing_eye_r", HiddenByDefault = true,
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "hurt", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 0.07f, Hidden = true },
                        new BoneSegment { StartTime = 0.07f, EndTime = 0.60f, BoneName = "t_facing_eye_r", Offset = new Vector2(4.5f, -0.4f) },
                        new BoneSegment { StartTime = 0.60f, EndTime = 1.00f, Hidden = true },
                    } },
                } },
        }},
        { "ASSASSIN_RUBY_RAIDER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-21.5f, -1.7f), Scale = 0.25f, AnchorBone = "eyes" },
            new EyeConfig { Offset = new Vector2(29.1f, -0.9f), Scale = 0.25f, AnchorBone = "eyes" },
        }},
        { "AXE_RUBY_RAIDER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-37.0f, 0.7f), Scale = 0.27f, AnchorBone = "eyes" },
            new EyeConfig { Offset = new Vector2(18.8f, 0.0f), Scale = 0.27f, AnchorBone = "eyes" },
        }},
        { "BRUTE_RUBY_RAIDER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-12.4f, -3.9f), Scale = 0.32f, AnchorBone = "eye_l" },
            new EyeConfig { Offset = new Vector2(4.8f, -0.3f), Scale = 0.32f, AnchorBone = "eye_r" },
        }},
        { "CROSSBOW_RUBY_RAIDER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(3.9f, -26.0f), Scale = 0.33f, AnchorBone = "face_twist" },
            new EyeConfig { Offset = new Vector2(7.0f, 41.0f), Scale = 0.33f, AnchorBone = "face_twist" },
        }},
        { "BYRDONIS", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(0.0f, -0.0f), Scale = 0.19f, AnchorBone = "eye_small" },
            new EyeConfig { Offset = new Vector2(-1.1f, -0.3f), Scale = 0.26f, AnchorBone = "eye_big" },
        }},
        { "CEREMONIAL_BEAST", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(0.0f, 0.0f), Scale = 0.25f, AnchorBone = "eye" },
        }},
        { "DEVOTED_SCULPTOR", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(15.8f, -26.6f), Scale = 0.32f, AnchorBone = "face",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 1.44f, BoneName = "face", Offset = new Vector2(16.9f, -31.1f) },
                        new BoneSegment { StartTime = 1.44f, EndTime = 2.83f, Hidden = true },
                    } },
                } },
        }},
        { "ENTOMANCER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(84.5f, -71.6f), Scale = 0.33f, AnchorBone = "head_upper" },
            new EyeConfig { Offset = new Vector2(87.4f, 0.7f), Scale = 0.33f, AnchorBone = "head_upper" },
        }},
        { "EXOSKELETON", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-2.2f, 2.8f), Scale = 0.75f, AnchorBone = "eye_r" },
            new EyeConfig { Offset = new Vector2(-3.7f, 1.2f), Scale = 0.75f, AnchorBone = "eye_l" },
        }},
        { "FABRICATOR", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(173.4f, -102.6f), Scale = 0.54f, AnchorBone = "guy_head" },
        }},
        { "FAT_GREMLIN", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(0.0f, 0.0f), Scale = 0.21f, AnchorBone = "eye_l" },
            new EyeConfig { Offset = new Vector2(1.7f, -2.1f), Scale = 0.21f, AnchorBone = "eye_r" },
        }},
        { "FLAIL_KNIGHT", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-0.7f, -50.3f), Scale = 0.64f, AnchorBone = "head_twist" },
            new EyeConfig { Offset = new Vector2(6.2f, 101.0f), Scale = 0.64f, AnchorBone = "head_twist" },
        }},
        { "ZAPBOT", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-64.7f, 50.8f), Scale = 1.20f, AnchorBone = "sphere" },
        }},
        { "WRIGGLER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-26.6f, -26.3f), Scale = 0.42f, AnchorBone = "lip_upper" },
            new EyeConfig { Offset = new Vector2(72.3f, -19.3f), Scale = 0.42f, AnchorBone = "lip_upper" },
        }},
        { "VINE_SHAMBLER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-78.4f, -78.6f), Scale = 0.77f, AnchorBone = "sneer_top" },
        }},
        { "TWO_TAILED_RAT", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-0.4f, -8.4f), Scale = 0.27f, AnchorBone = "eye_top",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 1.13f, BoneName = "eye_top", Offset = new Vector2(-0.4f, -8.4f) },
                        new BoneSegment { StartTime = 1.13f, EndTime = 1.83f, Hidden = true },
                    } },
                } },
        }},
        { "TWIG_SLIME_S", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-48.4f, -42.1f), Scale = 0.55f, AnchorBone = "eye_holder" },
            new EyeConfig { Offset = new Vector2(84.5f, -61.5f), Scale = 0.55f, AnchorBone = "eye_holder" },
        }},
        { "TWIG_SLIME_M", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(0.0f, 0.0f), Scale = 0.40f, AnchorBone = "twig 5" },
            new EyeConfig { Offset = new Vector2(-19.0f, 13.5f), Scale = 0.40f, AnchorBone = "twig 1" },
        }},
        { "TURRET_OPERATOR", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(106.9f, -117.2f), Scale = 0.53f, AnchorBone = "face" },
            new EyeConfig { Offset = new Vector2(-23.3f, -123.1f), Scale = 0.33f, AnchorBone = "face",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 1.34f, BoneName = "face", Offset = new Vector2(-23.3f, -123.1f) },
                        new BoneSegment { StartTime = 1.34f, EndTime = 2.08f, Hidden = true },
                    } },
                } },
        }},
        { "THE_OBSCURA", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-16.4f, -42.1f), Scale = 1.07f, AnchorBone = "iris1" },
        }},
        { "STABBOT", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-44.5f, 56.7f), Scale = 1.44f, AnchorBone = "sphere" },
        }},
        { "SPINY_TOAD", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-0.0f, 0.0f), Scale = 0.91f, AnchorBone = "eye_l" },
            new EyeConfig { Offset = new Vector2(-3.4f, 0.3f), Scale = 0.91f, AnchorBone = "eye_r" },
        }},
        { "SNEAKY_GREMLIN", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(4.1f, 3.8f), Scale = 0.27f, AnchorBone = "eye_b" },
            new EyeConfig { Offset = new Vector2(5.6f, 1.7f), Scale = 0.27f, AnchorBone = "eye_f" },
        }},
        { "SEWER_CLAM", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(294.5f, 94.9f), Scale = 1.20f, AnchorBone = "top_lips" },
            new EyeConfig { Offset = new Vector2(-124.8f, 139.8f), Scale = 1.20f, AnchorBone = "top_lips" },
        }},
        { "SEAPUNK", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(0.0f, 0.0f), Scale = 0.55f, AnchorBone = "mouth_top_left" },
            new EyeConfig { Offset = new Vector2(-1.6f, -11.2f), Scale = 0.55f, AnchorBone = "mouth_top_right" },
        }}, // TODO AXEBOT PLACEHOLDER!
        { "AXEBOT", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(239.2f, -179.6f), Scale = 1.54f, AnchorBone = "head",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 1.38f, BoneName = "head", Offset = new Vector2(239.2f, -179.6f) },
                        new BoneSegment { StartTime = 1.38f, EndTime = 1.89f, BoneName = "head", Offset = new Vector2(239.2f, -179.6f), OpacityStart = 1.00f, OpacityEnd = 0.00f },
                        new BoneSegment { StartTime = 1.89f, EndTime = 2.15f, BoneName = "head", Offset = new Vector2(239.2f, -179.6f), OpacityStart = 0.00f, OpacityEnd = 0.00f },
                    } },
                } },
        }},
    };
}