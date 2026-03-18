using Godot;

namespace GooglyEyes;

public static class CreatureGooglyEyesRegistry
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
        { "ARCHITECT", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(5.8f, 11.0f), Scale = 0.20f, AnchorBone = "face" },
            new EyeConfig { Offset = new Vector2(7.7f, -35.9f), Scale = 0.20f, AnchorBone = "face" },
        }},
        { "ASSASSIN_RUBY_RAIDER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-21.5f, -1.7f), Scale = 0.25f, AnchorBone = "eyes" },
            new EyeConfig { Offset = new Vector2(29.1f, -0.9f), Scale = 0.25f, AnchorBone = "eyes" },
        }},
        { "AXE_RUBY_RAIDER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-37.0f, 0.7f), Scale = 0.27f, AnchorBone = "eyes" },
            new EyeConfig { Offset = new Vector2(18.8f, 0.0f), Scale = 0.27f, AnchorBone = "eyes" },
        }},
        { "BATTLE_FRIEND_V1", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-0.0f, -0.0f), Scale = 0.40f, AnchorBone = "eye1" },
            new EyeConfig { Offset = new Vector2(12.4f, 6.6f), Scale = 0.40f, AnchorBone = "eye2" },
        }},
        { "BATTLE_FRIEND_V2", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-0.0f, -0.0f), Scale = 0.40f, AnchorBone = "eye1" },
            new EyeConfig { Offset = new Vector2(12.4f, 6.6f), Scale = 0.40f, AnchorBone = "eye2" },
        }},
        { "BATTLE_FRIEND_V3", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-0.0f, -0.0f), Scale = 0.40f, AnchorBone = "eye1" },
            new EyeConfig { Offset = new Vector2(12.4f, 6.6f), Scale = 0.40f, AnchorBone = "eye2" },
        }},

        { "BRUTE_RUBY_RAIDER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-12.4f, -3.9f), Scale = 0.32f, AnchorBone = "eye_l" },
            new EyeConfig { Offset = new Vector2(4.8f, -0.3f), Scale = 0.32f, AnchorBone = "eye_r" },
        }},
        { "CROSSBOW_RUBY_RAIDER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(3.9f, -26.0f), Scale = 0.33f, AnchorBone = "face_twist" },
            new EyeConfig { Offset = new Vector2(7.0f, 41.0f), Scale = 0.33f, AnchorBone = "face_twist" },
        }},
        { "BYRDPIP", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-0.0f, 0.0f), Scale = 0.24f, AnchorBone = "eye" },
        }},
        { "BYRDONIS", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(0.0f, -0.0f), Scale = 0.19f, AnchorBone = "eye_small" },
            new EyeConfig { Offset = new Vector2(-1.1f, -0.3f), Scale = 0.26f, AnchorBone = "eye_big" },
        }},
        { "CEREMONIAL_BEAST", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(0.0f, 0.0f), Scale = 0.25f, AnchorBone = "eye" },
        }},
        { "DEFECT", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-30.0f, 5.7f), Scale = 1.12f, AnchorBone = "eye_orb" },
            new EyeConfig { Offset = new Vector2(107.7f, 4.4f), Scale = 0.40f, AnchorBone = "head_adjust" },
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
        { "EYE_WITH_TEETH", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(29.4f, 2.3f), Scale = 1.14f, AnchorBone = "iris",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 0.90f, BoneName = "iris", Offset = new Vector2(29.4f, 2.3f) },
                        new BoneSegment { StartTime = 0.90f, EndTime = 1.32f, BoneName = "iris", Offset = new Vector2(29.4f, 2.3f), OpacityStart = 1.00f, OpacityEnd = 0.00f },
                        new BoneSegment { StartTime = 1.32f, EndTime = 1.93f, Hidden = true },
                    } },
                } },
        }},
        { "FABRICATOR", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(173.4f, -102.6f), Scale = 0.54f, AnchorBone = "guy_head" },
        }},
        { "FAKE_MERCHANT_MONSTER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-39.0f, -38.1f), Scale = 0.28f, AnchorBone = "head_twist",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 0.10f, BoneName = "head_twist", Offset = new Vector2(-39.0f, -38.1f) },
                        new BoneSegment { StartTime = 0.10f, EndTime = 0.33f, BoneName = "head_twist", Offset = new Vector2(-39.0f, -38.1f), OpacityStart = 1.00f, OpacityEnd = 0.00f },
                        new BoneSegment { StartTime = 0.33f, EndTime = 1.53f, Hidden = true },
                    } },
                } },
            new EyeConfig { Offset = new Vector2(27.1f, -37.3f), Scale = 0.28f, AnchorBone = "head_twist",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 0.10f, BoneName = "head_twist", Offset = new Vector2(27.1f, -37.3f) },
                        new BoneSegment { StartTime = 0.10f, EndTime = 0.33f, BoneName = "head_twist", Offset = new Vector2(27.1f, -37.3f), OpacityStart = 1.00f, OpacityEnd = 0.00f },
                        new BoneSegment { StartTime = 0.33f, EndTime = 1.53f, Hidden = true },
                    } },
                } },
        }},
        { "FAT_GREMLIN", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(0.0f, 0.0f), Scale = 0.21f, AnchorBone = "eye_l" },
            new EyeConfig { Offset = new Vector2(1.7f, -2.1f), Scale = 0.21f, AnchorBone = "eye_r" },
        }},
        { "FLAIL_KNIGHT", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-0.7f, -50.3f), Scale = 0.64f, AnchorBone = "head_twist" },
            new EyeConfig { Offset = new Vector2(6.2f, 101.0f), Scale = 0.64f, AnchorBone = "head_twist" },
        }},
        { "FOGMOG", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-17.1f, -5.1f), Scale = 0.40f, AnchorBone = "eye_l" },
            new EyeConfig { Offset = new Vector2(7.9f, -7.1f), Scale = 0.40f, AnchorBone = "eye_r" },
        }},
        { "FOSSIL_STALKER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-137.7f, -93.7f), Scale = 0.72f, AnchorBone = "body-twist" },
            new EyeConfig { Offset = new Vector2(32.5f, -91.1f), Scale = 0.72f, AnchorBone = "body-twist" },
        }},
        { "FROG_KNIGHT", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(392.5f, 114.3f), Scale = 0.67f, AnchorBone = "face_twist2",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 1.10f, BoneName = "face_twist2", Offset = new Vector2(0.0f, 0.0f) },
                        new BoneSegment { StartTime = 1.10f, EndTime = 2.33f, Hidden = true },
                    } },
                } },
        }},
        { "FUZZY_WURM_CRAWLER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(12.2f, 7.5f), Scale = 0.40f, AnchorBone = "head" },
        }},
        { "IRONCLAD", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(10.7f, -2.9f), Scale = 0.36f, AnchorBone = "squint",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 1.56f, BoneName = "squint", Offset = new Vector2(10.7f, -2.9f) },
                        new BoneSegment { StartTime = 1.56f, EndTime = 2.33f, Hidden = true },
                    } },
                } },
        }},
        { "KNOWLEDGE_DEMON", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-39.9f, 2.5f), Scale = 0.25f, AnchorBone = "bug_leg_l1" },
            new EyeConfig { Offset = new Vector2(-91.3f, -10.6f), Scale = 0.25f, AnchorBone = "bug_leg_l1" },
            new EyeConfig { Offset = new Vector2(9.1f, 1.0f), Scale = 0.79f, AnchorBone = "face",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 3.32f, BoneName = "face", Offset = new Vector2(9.1f, 1.0f) },
                        new BoneSegment { StartTime = 3.32f, EndTime = 4.73f, Hidden = true },
                    } },
                } },
        }},
        { "MYTE", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-120.6f, 154.7f), Scale = 0.83f, AnchorBone = "back_plate" },
            new EyeConfig { Offset = new Vector2(-226.7f, 18.1f), Scale = 0.83f, AnchorBone = "back_plate" },
        }},
        { "NECROBINDER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(0.0f, 0.0f), Scale = 0.19f, AnchorBone = "eye_l",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 1.09f, BoneName = "eye_l", Offset = new Vector2(0.0f, 0.0f) },
                        new BoneSegment { StartTime = 1.09f, EndTime = 2.33f, Hidden = true },
                    } },
                } },
            new EyeConfig { Offset = new Vector2(8.0f, -2.0f), Scale = 0.15f, AnchorBone = "eye_r",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 1.09f, BoneName = "eye_r", Offset = new Vector2(8.0f, -2.0f) },
                        new BoneSegment { StartTime = 1.09f, EndTime = 2.33f, Hidden = true },
                    } },
                } },
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
        { "REGENT", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(0.0f, 0.0f), Scale = 0.31f, AnchorBone = "eye",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 2.34f, BoneName = "eye", Offset = new Vector2(0.0f, 0.0f) },
                        new BoneSegment { StartTime = 2.34f, EndTime = 2.69f, BoneName = "eye", Offset = new Vector2(0.0f, 0.0f), OpacityStart = 1.00f, OpacityEnd = 0.00f },
                        new BoneSegment { StartTime = 2.69f, EndTime = 4.34f, Hidden = true },
                    } },
                } },
            new EyeConfig { Offset = new Vector2(-6.1f, -19.5f), Scale = 0.24f, AnchorBone = "minion_r_arm_1",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 3.47f, BoneName = "minion_r_arm_1", Offset = new Vector2(-6.1f, -19.5f) },
                        new BoneSegment { StartTime = 3.47f, EndTime = 4.34f, Hidden = true },
                    } },
                } },
            new EyeConfig { Offset = new Vector2(-15.9f, 44.4f), Scale = 0.24f, AnchorBone = "minion_shoulder_r",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 3.47f, BoneName = "minion_shoulder_r", Offset = new Vector2(-15.9f, 44.4f) },
                        new BoneSegment { StartTime = 3.47f, EndTime = 4.34f, Hidden = true },
                    } },
                } },
        }},
        { "SILENT", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1.3f, 2.1f), Scale = 0.21f, AnchorBone = "eye",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 0.64f, BoneName = "eye", Offset = new Vector2(0.0f, 0.0f) },
                        new BoneSegment { StartTime = 0.64f, EndTime = 1.07f, BoneName = "eye", Offset = new Vector2(0.0f, 0.0f), OpacityStart = 1.00f, OpacityEnd = 0.00f },
                        new BoneSegment { StartTime = 1.07f, EndTime = 1.87f, Hidden = true },
                    } },
                } },
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
        }},
    };
}