using Godot;

namespace GooglyEyes;

public static class CreatureGooglyEyesRegistry
{
    public static readonly Dictionary<string, EyeConfig[]> Configs = new()
    {
{ "TRACKER_RUBY_RAIDER", new EyeConfig[] {
    new EyeConfig { Offset = new Vector2(2.4f, 3.4f), Scale = 0.30f, AnchorBone = "b_eye",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "die", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.61f, BoneName = "b_eye", Offset = new Vector2(2.4f, 3.4f) },
                new BoneSegment { StartTime = 0.61f, EndTime = 0.77f, Hidden = true },
                new BoneSegment { StartTime = 0.77f, EndTime = 1.97f, BoneName = "b_eye", Offset = new Vector2(2.4f, 3.4f) },
            } },
        } },
    new EyeConfig { Offset = new Vector2(184.5f, 22.8f), Scale = 0.40f, AnchorBone = "t_head",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "hurt", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.10f, BoneName = "t_head", Offset = new Vector2(184.5f, 22.8f) },
                new BoneSegment { StartTime = 0.10f, EndTime = 0.57f, Hidden = true },
                new BoneSegment { StartTime = 0.57f, EndTime = 1.00f, BoneName = "t_head", Offset = new Vector2(184.5f, 22.8f) },
            } },
        } },
    new EyeConfig { Offset = new Vector2(109.3f, -17.0f), Scale = 0.40f, AnchorBone = "t_head",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "hurt", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.10f, BoneName = "t_head", Offset = new Vector2(109.3f, -17.0f) },
                new BoneSegment { StartTime = 0.10f, EndTime = 0.57f, Hidden = true },
                new BoneSegment { StartTime = 0.57f, EndTime = 1.00f, BoneName = "t_head", Offset = new Vector2(109.3f, -17.0f) },
            } },
        } },
    new EyeConfig { Offset = new Vector2(6.0f, -50.1f), Scale = 0.40f, AnchorBone = "t_facing_head",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "hurt", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.10f, Hidden = true },
                new BoneSegment { StartTime = 0.10f, EndTime = 0.57f, BoneName = "t_facing_head", Offset = new Vector2(6.0f, -50.1f) },
                new BoneSegment { StartTime = 0.57f, EndTime = 1.00f, Hidden = true },
            } },
            { "attack", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 2.00f, Hidden = true },
            } },
            { "cast", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 2.00f, Hidden = true },
            } },
            { "die", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.97f, Hidden = true },
            } },
            { "idle_loop", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 5.33f, Hidden = true },
            } },
        } },
    new EyeConfig { Offset = new Vector2(3.4f, 40.0f), Scale = 0.40f, AnchorBone = "t_facing_head",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "hurt", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.10f, Hidden = true },
                new BoneSegment { StartTime = 0.10f, EndTime = 0.57f, BoneName = "t_facing_head", Offset = new Vector2(3.4f, 40.0f) },
                new BoneSegment { StartTime = 0.57f, EndTime = 1.00f, Hidden = true },
            } },
            { "attack", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 2.00f, Hidden = true },
            } },
            { "cast", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 2.00f, Hidden = true },
            } },
            { "die", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.97f, Hidden = true },
            } },
            { "idle_loop", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 5.33f, Hidden = true },
            } },
        } },
}},
        { "ARCHITECT", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(5.8f, 11.0f), Scale = 0.20f, AnchorBone = "face" },
            new EyeConfig { Offset = new Vector2(7.7f, -35.9f), Scale = 0.20f, AnchorBone = "face" },
        }},
        { "ASSASSIN_RUBY_RAIDER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(67.8f, -37.0f), Scale = 0.29f, AnchorBone = "head" },
            new EyeConfig { Offset = new Vector2(58.0f, 28.0f), Scale = 0.29f, AnchorBone = "head" },
        }},
        { "AXE_RUBY_RAIDER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-52.9f, -96.2f), Scale = 0.27f, AnchorBone = "face_twist" },
            new EyeConfig { Offset = new Vector2(4.9f, -83.1f), Scale = 0.27f, AnchorBone = "face_twist" },
        }},
        { "AXEBOT", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(114.4f, -2.2f), Scale = 1.54f, AnchorBone = "head_twist" },
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
        { "BOWLBUG_EGG", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-42.9f, 69.6f), Scale = 0.62f, AnchorBone = "head_twist" },
        }},
        { "BOWLBUG_NECTAR", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-42.9f, 69.6f), Scale = 0.62f, AnchorBone = "head_twist" },
        }},
        { "BOWLBUG_ROCK", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-42.9f, 69.6f), Scale = 0.62f, AnchorBone = "head_twist" },
        }},
        { "BOWLBUG_SILK", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-42.9f, 69.6f), Scale = 0.62f, AnchorBone = "head_twist" },
        }},
        { "BRUTE_RUBY_RAIDER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-12.4f, -3.9f), Scale = 0.32f, AnchorBone = "eye_l" },
            new EyeConfig { Offset = new Vector2(4.8f, -0.3f), Scale = 0.32f, AnchorBone = "eye_r" },
        }},
        { "BYGONE_EFFIGY", new EyeConfig[] {
    new EyeConfig { Offset = new Vector2(-63.0f, -28.6f), Scale = 0.22f, AnchorBone = "cheek",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "attack", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.07f, BoneName = "cheek", Offset = new Vector2(-63.0f, -28.6f) },
                new BoneSegment { StartTime = 0.07f, EndTime = 1.00f, Hidden = true },
                new BoneSegment { StartTime = 1.00f, EndTime = 1.33f, BoneName = "cheek", Offset = new Vector2(-63.0f, -28.6f) },
            } },
        } },
    new EyeConfig { Offset = new Vector2(-10.4f, -32.0f), Scale = 0.22f, AnchorBone = "cheek",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "attack", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.07f, BoneName = "cheek", Offset = new Vector2(-10.4f, -32.0f) },
                new BoneSegment { StartTime = 0.07f, EndTime = 1.00f, Hidden = true },
                new BoneSegment { StartTime = 1.00f, EndTime = 1.33f, BoneName = "cheek", Offset = new Vector2(-10.4f, -32.0f) },
            } },
        } },
    new EyeConfig { Offset = new Vector2(3.0f, 6.0f), Scale = 0.22f, AnchorBone = "attack-eye",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "idle_loop", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.67f, Hidden = true },
            } },
            { "die", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.07f, Hidden = true },
            } },
            { "hurt", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.17f, Hidden = true },
            } },
            { "attack", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.15f, Hidden = true },
                new BoneSegment { StartTime = 0.15f, EndTime = 0.92f, BoneName = "attack-eye", Offset = new Vector2(3.0f, 6.0f) },
                new BoneSegment { StartTime = 0.92f, EndTime = 1.33f, Hidden = true },
            } },
        } },
}},
        { "CALCIFIED_CULTIST", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(71.2f, -53.9f), Scale = 0.32f, AnchorBone = "head" },
        }},
        { "CORPSE_SLUG", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(28.4f, 5.1f), Scale = 0.39f, AnchorBone = "antenna3f" },
            new EyeConfig { Offset = new Vector2(16.1f, 8.2f), Scale = 0.39f, AnchorBone = "antenna3b" },
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
        { "CHOMPER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(0.9f, 5.8f), Scale = 0.59f, AnchorBone = "head" },
        }},
        { "CUBEX_CONSTRUCT", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(61.1f, -30.2f), Scale = 0.70f, AnchorBone = "body_top" },
        }},
        { "DAMP_CULTIST", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(71.2f, -53.9f), Scale = 0.32f, AnchorBone = "head" },
        }},
        { "DECIMILLIPEDE_SEGMENT_BACK", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1.2f, -80.8f), Scale = -0.71f, AnchorBone = "seg_1_spine16" },
            new EyeConfig { Offset = new Vector2(-154.4f, -24.9f), Scale = -0.71f, AnchorBone = "seg_1_spine16" },
        }},
        { "DECIMILLIPEDE_SEGMENT_FRONT", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-119.9f, 32.6f), Scale = -0.67f, AnchorBone = "seg_3_spine6" },
            new EyeConfig { Offset = new Vector2(34.2f, 51.4f), Scale = -0.67f, AnchorBone = "seg_3_spine6" },
        }},
        { "DECIMILLIPEDE_SEGMENT_MIDDLE", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(96.0f, 29.3f), Scale = -0.53f, AnchorBone = "seg_2_spine_26" },
            new EyeConfig { Offset = new Vector2(-24.1f, -7.4f), Scale = -0.53f, AnchorBone = "seg_2_spine_26" },
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
        { "FLYCONID", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1.4f, -32.4f), Scale = 0.30f, AnchorBone = "cap1b" },
            new EyeConfig { Offset = new Vector2(6.0f, 34.9f), Scale = 0.30f, AnchorBone = "cap1b" },
            new EyeConfig { Offset = new Vector2(41.5f, -12.5f), Scale = 0.20f, AnchorBone = "cap2b" },
            new EyeConfig { Offset = new Vector2(30.3f, 28.2f), Scale = 0.20f, AnchorBone = "cap2b" },
            new EyeConfig { Offset = new Vector2(-0.7f, -22.0f), Scale = 0.23f, AnchorBone = "cap3b" },
            new EyeConfig { Offset = new Vector2(4.1f, 25.0f), Scale = 0.23f, AnchorBone = "cap3b" },
            new EyeConfig { Offset = new Vector2(-26.9f, -39.7f), Scale = 0.25f, AnchorBone = "cap4b" },
            new EyeConfig { Offset = new Vector2(-34.0f, 26.3f), Scale = 0.25f, AnchorBone = "cap4b" },
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
            new EyeConfig { Offset = new Vector2(5.3f, -3.9f), Scale = 0.67f, AnchorBone = "face_twist2",
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
        { "GAS_BOMB", new EyeConfig[] {
    new EyeConfig { Offset = new Vector2(-92.7f, -43.3f), Scale = 0.22f, AnchorBone = "tail1",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "spawn", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.08f, Hidden = true },
                new BoneSegment { StartTime = 0.08f, EndTime = 1.00f, BoneName = "tail1", Offset = new Vector2(-92.7f, -43.3f) },
            } },
            { "explode", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.09f, BoneName = "tail1", Offset = new Vector2(-92.7f, -43.3f) },
                new BoneSegment { StartTime = 0.09f, EndTime = 1.17f, Hidden = true },
            } },
            { "die", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.27f, BoneName = "tail1", Offset = new Vector2(-92.7f, -43.3f), OpacityStart = 1.00f, OpacityEnd = 0.00f },
                new BoneSegment { StartTime = 0.27f, EndTime = 2.00f, Hidden = true },
            } },
        } },
    new EyeConfig { Offset = new Vector2(-51.6f, -15.4f), Scale = 0.22f, AnchorBone = "tail1",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "spawn", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.08f, Hidden = true },
                new BoneSegment { StartTime = 0.08f, EndTime = 1.00f, BoneName = "tail1", Offset = new Vector2(-51.6f, -15.4f) },
            } },
            { "explode", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.09f, BoneName = "tail1", Offset = new Vector2(-51.6f, -15.4f) },
                new BoneSegment { StartTime = 0.09f, EndTime = 1.17f, Hidden = true },
            } },
            { "die", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.27f, BoneName = "tail1", Offset = new Vector2(-51.6f, -15.4f), OpacityStart = 1.00f, OpacityEnd = 0.00f },
                new BoneSegment { StartTime = 0.27f, EndTime = 2.00f, Hidden = true },
            } },
        } },
}},
        { "GLOBE_HEAD", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(47.1f, -210.5f), Scale = 0.93f, AnchorBone = "head" },
            new EyeConfig { Offset = new Vector2(156.9f, -52.5f), Scale = 0.93f, AnchorBone = "head" },
        }},
        { "GREMLIN_MERC", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-54.2f, -24.8f), Scale = 0.62f, AnchorBone = "helmet_twist" },
            new EyeConfig { Offset = new Vector2(85.9f, -13.4f), Scale = 0.62f, AnchorBone = "helmet_twist" },
        }},
        { "GUARDBOT", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-120.8f, -156.5f), Scale = 0.69f, AnchorBone = "sphere_twist" },
            new EyeConfig { Offset = new Vector2(34.6f, -131.8f), Scale = 0.69f, AnchorBone = "sphere_twist" },
        }},
        { "HAUNTED_SHIP", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(798.6f, 79.8f), Scale = 1.00f, AnchorBone = "body_twist" },
            new EyeConfig { Offset = new Vector2(578.7f, 112.9f), Scale = 1.00f, AnchorBone = "body_twist" },
        }},
        { "HUNTER_KILLER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(9.4f, -22.7f), Scale = 0.44f, AnchorBone = "eye_r2" },
            new EyeConfig { Offset = new Vector2(-6.9f, -12.8f), Scale = 0.44f, AnchorBone = "eye_r" },
        }},
        { "INFESTED_PRISM", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(0.0f, 0.0f), Scale = 0.69f, AnchorBone = "actual eye",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 1.13f, BoneName = "actual eye", Offset = new Vector2(0.0f, 0.0f) },
                        new BoneSegment { StartTime = 1.13f, EndTime = 1.87f, BoneName = "actual eye", Offset = new Vector2(0.0f, 0.0f), OpacityStart = 1.00f, OpacityEnd = 0.00f },
                    } },
                } },
        }},
        { "INKLET", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(0.0f, 0.0f), Scale = 0.68f, AnchorBone = "eye",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 1.35f, BoneName = "eye", Offset = new Vector2(0.0f, 0.0f) },
                        new BoneSegment { StartTime = 1.35f, EndTime = 2.17f, BoneName = "", Offset = new Vector2(0.0f, 0.0f) },
                    } },
                } },
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
        { "KIN_FOLLOWER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-119.4f, 44.9f), Scale = 0.76f, AnchorBone = "grass_3_flap_l" },
            new EyeConfig { Offset = new Vector2(-190.0f, -99.6f), Scale = 0.76f, AnchorBone = "grass_3_flap_l" },
        }},
        { "KIN_PRIEST", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(0.0f, -0.0f), Scale = 0.33f, AnchorBone = "eye" },
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
        { "LAGAVULIN_MATRIARCH", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(33.0f, -15.0f), Scale = 1.43f, AnchorBone = "eye_look" },
            new EyeConfig { Offset = new Vector2(2.6f, 1.9f), Scale = 0.27f, AnchorBone = "butt_eye7" },
            new EyeConfig { Offset = new Vector2(-82.1f, 6.2f), Scale = 0.47f, AnchorBone = "butt_eyes1" },
            new EyeConfig { Offset = new Vector2(73.9f, -7.1f), Scale = 0.47f, AnchorBone = "butt_eyes1" },
            new EyeConfig { Offset = new Vector2(-9.3f, 2.4f), Scale = 0.37f, AnchorBone = "butt_eye5" },
            new EyeConfig { Offset = new Vector2(3.8f, -4.1f), Scale = 0.43f, AnchorBone = "butt_eye8" },
            new EyeConfig { Offset = new Vector2(-40.6f, -3.2f), Scale = 0.23f, AnchorBone = "butt_eyes3" },
            new EyeConfig { Offset = new Vector2(45.8f, -3.5f), Scale = 0.23f, AnchorBone = "butt_eyes3" },
            new EyeConfig { Offset = new Vector2(4.9f, 1.0f), Scale = 0.37f, AnchorBone = "butt_eye9" },
            new EyeConfig { Offset = new Vector2(3.9f, 0.7f), Scale = 0.27f, AnchorBone = "butt_eye10" },
            new EyeConfig { Offset = new Vector2(0.5f, 0.2f), Scale = 0.33f, AnchorBone = "butt_eye6" },
            new EyeConfig { Offset = new Vector2(-49.6f, 0.2f), Scale = 0.37f, AnchorBone = "butt_eyes4" },
            new EyeConfig { Offset = new Vector2(46.2f, -17.7f), Scale = 0.37f, AnchorBone = "butt_eyes4" },
            new EyeConfig { Offset = new Vector2(-66.4f, 0.3f), Scale = 0.37f, AnchorBone = "butt_eyes2" },
            new EyeConfig { Offset = new Vector2(69.1f, -2.8f), Scale = 0.37f, AnchorBone = "butt_eyes2" },
        }},
        { "LEAF_SLIME_M", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(0.0f, 0.0f), Scale = 0.56f, AnchorBone = "eye_base" },
        }},
        { "LEAF_SLIME_S", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(4.7f, 4.4f), Scale = 0.51f, AnchorBone = "iris" },
        }},
        { "LIVING_FOG", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-45.5f, -118.1f), Scale = 0.33f, AnchorBone = "body_1",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 1.04f, BoneName = "body_1", Offset = new Vector2(-45.5f, -118.1f) },
                        new BoneSegment { StartTime = 1.04f, EndTime = 2.17f, BoneName = "body_1", Offset = new Vector2(-45.5f, -118.1f), OpacityStart = 1.00f, OpacityEnd = 0.00f },
                    } },
                } },
            new EyeConfig { Offset = new Vector2(1.0f, -62.8f), Scale = 0.33f, AnchorBone = "body_1",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 1.04f, BoneName = "body_1", Offset = new Vector2(1.0f, -62.8f) },
                        new BoneSegment { StartTime = 1.04f, EndTime = 2.17f, BoneName = "body_1", Offset = new Vector2(1.0f, -62.8f), OpacityStart = 1.00f, OpacityEnd = 0.00f },
                    } },
                } },
        }},
        { "LIVING_SHIELD", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(19.2f, -75.4f), Scale = 0.67f, AnchorBone = "face_twist",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 0.04f, BoneName = "face_twist", Offset = new Vector2(19.2f, -75.4f) },
                        new BoneSegment { StartTime = 0.04f, EndTime = 1.67f, Hidden = true },
                    } },
                } },
            new EyeConfig { Offset = new Vector2(8.1f, 71.8f), Scale = 0.67f, AnchorBone = "face_twist",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 0.04f, BoneName = "face_twist", Offset = new Vector2(8.1f, 71.8f) },
                        new BoneSegment { StartTime = 0.04f, EndTime = 1.67f, Hidden = true },
                    } },
                } },
        }},
        { "LOUSE_PROGENITOR", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-168.5f, -104.7f), Scale = 1.34f, AnchorBone = "head_twist" },
        }},
        { "MAGI_KNIGHT", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(77.7f, -80.9f), Scale = 0.63f, AnchorBone = "face" },
            new EyeConfig { Offset = new Vector2(75.8f, 59.2f), Scale = 0.63f, AnchorBone = "face" },
        }},
        { "MAWLER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(34.1f, -13.8f), Scale = 0.40f, AnchorBone = "brow" },
        }},
        { "MECHA_KNIGHT", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(35.9f, -111.8f), Scale = 0.27f, AnchorBone = "dude_head" },
        }},
        { "MYTE", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-120.6f, 154.7f), Scale = 0.83f, AnchorBone = "back_plate" },
            new EyeConfig { Offset = new Vector2(-226.7f, 18.1f), Scale = 0.83f, AnchorBone = "back_plate" },
        }},
        { "MYSTERIOUS_KNIGHT", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-1.1f, -34.6f), Scale = 0.59f, AnchorBone = "head_twist" },
            new EyeConfig { Offset = new Vector2(-7.3f, 106.4f), Scale = 0.59f, AnchorBone = "head_twist" },
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
        { "NIBBIT", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(0.4f, -2.8f), Scale = 0.25f, AnchorBone = "eye_adjust" },
        }},
        { "NOISEBOT", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-81.0f, -154.0f), Scale = 0.93f, AnchorBone = "sphere_twist" },
            new EyeConfig { Offset = new Vector2(104.5f, -102.6f), Scale = 0.93f, AnchorBone = "sphere_twist" },
        }},
        { "OVICOPTER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(0.0f, 0.0f), Scale = 0.37f, AnchorBone = "face3" },
            new EyeConfig { Offset = new Vector2(-2.6f, -0.3f), Scale = 0.33f, AnchorBone = "face2" },
            new EyeConfig { Offset = new Vector2(3.4f, -6.5f), Scale = 0.53f, AnchorBone = "eye_large_r" },
            new EyeConfig { Offset = new Vector2(5.4f, 2.6f), Scale = 0.37f, AnchorBone = "eye_small_r" },
        }},
        { "OWL_MAGISTRATE", new EyeConfig[] {
    new EyeConfig { Offset = new Vector2(3.0f, -11.2f), Scale = 1.23f, AnchorBone = "eye_r",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "attack_dive", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.96f, Hidden = true },
                new BoneSegment { StartTime = 0.96f, EndTime = 1.40f, BoneName = "eye_r", Offset = new Vector2(3.0f, -11.2f) },
            } },
            { "attack_peck", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.06f, BoneName = "eye_r", Offset = new Vector2(3.0f, -11.2f) },
                new BoneSegment { StartTime = 0.06f, EndTime = 0.36f, Hidden = true },
                new BoneSegment { StartTime = 0.36f, EndTime = 0.57f, BoneName = "eye_r", Offset = new Vector2(3.0f, -11.2f) },
                new BoneSegment { StartTime = 0.57f, EndTime = 0.93f, Hidden = true },
                new BoneSegment { StartTime = 0.93f, EndTime = 1.13f, BoneName = "eye_r", Offset = new Vector2(3.0f, -11.2f) },
                new BoneSegment { StartTime = 1.13f, EndTime = 1.33f, Hidden = true },
                new BoneSegment { StartTime = 1.33f, EndTime = 1.53f, BoneName = "eye_r", Offset = new Vector2(3.0f, -11.2f) },
            } },
            { "die", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.96f, Hidden = true },
                new BoneSegment { StartTime = 0.96f, EndTime = 1.83f, BoneName = "eye_r", Offset = new Vector2(3.0f, -11.2f) },
            } },
            { "die_flying", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.03f, Hidden = true },
                new BoneSegment { StartTime = 1.03f, EndTime = 1.90f, BoneName = "eye_r", Offset = new Vector2(3.0f, -11.2f) },
            } },
            { "fly_loop", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.67f, Hidden = true },
            } },
            { "hurt", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.06f, BoneName = "eye_r", Offset = new Vector2(3.0f, -11.2f) },
                new BoneSegment { StartTime = 0.06f, EndTime = 0.62f, Hidden = true },
                new BoneSegment { StartTime = 0.62f, EndTime = 0.83f, BoneName = "eye_r", Offset = new Vector2(3.0f, -11.2f) },
            } },
            { "hurt_flying", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.40f, Hidden = true },
            } },
            { "take_off", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.16f, BoneName = "eye_r", Offset = new Vector2(3.0f, -11.2f) },
                new BoneSegment { StartTime = 0.16f, EndTime = 0.67f, Hidden = true },
            } },
        } },
    new EyeConfig { Offset = new Vector2(-5.6f, -57.0f), Scale = 1.23f, AnchorBone = "eye_l",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "attack_dive", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.96f, Hidden = true },
                new BoneSegment { StartTime = 0.96f, EndTime = 1.40f, BoneName = "eye_l", Offset = new Vector2(-5.6f, -57.0f) },
            } },
            { "attack_peck", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.06f, BoneName = "eye_l", Offset = new Vector2(-5.6f, -57.0f) },
                new BoneSegment { StartTime = 0.06f, EndTime = 0.36f, Hidden = true },
                new BoneSegment { StartTime = 0.36f, EndTime = 0.57f, BoneName = "eye_l", Offset = new Vector2(-5.6f, -57.0f) },
                new BoneSegment { StartTime = 0.57f, EndTime = 0.93f, Hidden = true },
                new BoneSegment { StartTime = 0.93f, EndTime = 1.13f, BoneName = "eye_l", Offset = new Vector2(-5.6f, -57.0f) },
                new BoneSegment { StartTime = 1.13f, EndTime = 1.33f, Hidden = true },
                new BoneSegment { StartTime = 1.33f, EndTime = 1.53f, BoneName = "eye_l", Offset = new Vector2(-5.6f, -57.0f) },
            } },
            { "die", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.96f, Hidden = true },
                new BoneSegment { StartTime = 0.96f, EndTime = 1.83f, BoneName = "eye_l", Offset = new Vector2(-5.6f, -57.0f) },
            } },
            { "die_flying", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.03f, Hidden = true },
                new BoneSegment { StartTime = 1.03f, EndTime = 1.90f, BoneName = "eye_l", Offset = new Vector2(-5.6f, -57.0f) },
            } },
            { "fly_loop", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.67f, Hidden = true },
            } },
            { "hurt", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.06f, BoneName = "eye_l", Offset = new Vector2(-5.6f, -57.0f) },
                new BoneSegment { StartTime = 0.06f, EndTime = 0.62f, Hidden = true },
                new BoneSegment { StartTime = 0.62f, EndTime = 0.83f, BoneName = "eye_l", Offset = new Vector2(-5.6f, -57.0f) },
            } },
            { "hurt_flying", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.40f, Hidden = true },
            } },
            { "take_off", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.16f, BoneName = "eye_l", Offset = new Vector2(-5.6f, -57.0f) },
                new BoneSegment { StartTime = 0.16f, EndTime = 0.67f, Hidden = true },
            } },
        } },
    new EyeConfig { Offset = new Vector2(-1.6f, -1.3f), Scale = 1.23f, AnchorBone = "profile_eye",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "attack_dive", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.96f, BoneName = "profile_eye", Offset = new Vector2(-1.6f, -1.3f) },
                new BoneSegment { StartTime = 0.96f, EndTime = 1.40f, Hidden = true },
            } },
            { "attack_peck", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.07f, Hidden = true },
                new BoneSegment { StartTime = 0.07f, EndTime = 0.36f, BoneName = "profile_eye", Offset = new Vector2(-1.6f, -1.3f) },
                new BoneSegment { StartTime = 0.36f, EndTime = 0.57f, Hidden = true },
                new BoneSegment { StartTime = 0.57f, EndTime = 0.93f, BoneName = "profile_eye", Offset = new Vector2(-1.6f, -1.3f) },
                new BoneSegment { StartTime = 0.93f, EndTime = 1.13f, Hidden = true },
                new BoneSegment { StartTime = 1.13f, EndTime = 1.33f, BoneName = "profile_eye", Offset = new Vector2(-1.6f, -1.3f) },
                new BoneSegment { StartTime = 1.33f, EndTime = 1.53f, Hidden = true },
            } },
            { "die", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.96f, BoneName = "profile_eye", Offset = new Vector2(-1.6f, -1.3f) },
                new BoneSegment { StartTime = 0.96f, EndTime = 1.83f, Hidden = true },
            } },
            { "die_flying", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.03f, BoneName = "profile_eye", Offset = new Vector2(-1.6f, -1.3f) },
                new BoneSegment { StartTime = 1.03f, EndTime = 1.90f, Hidden = true },
            } },
            { "hurt", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.06f, Hidden = true },
                new BoneSegment { StartTime = 0.06f, EndTime = 0.63f, BoneName = "profile_eye", Offset = new Vector2(-1.6f, -1.3f) },
                new BoneSegment { StartTime = 0.63f, EndTime = 0.83f, Hidden = true },
            } },
            { "idle_loop", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 5.33f, Hidden = true },
            } },
            { "take_off", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.16f, Hidden = true },
                new BoneSegment { StartTime = 0.16f, EndTime = 0.67f, BoneName = "profile_eye", Offset = new Vector2(-1.6f, -1.3f) },
            } },
        } },
}},
        { "PAELS_LEGION", new EyeConfig[] {
    new EyeConfig { Offset = new Vector2(2.0f, -8.5f), Scale = 0.40f, AnchorBone = "face",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "block", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.46f, BoneName = "face", Offset = new Vector2(2.0f, -8.5f) },
                new BoneSegment { StartTime = 0.46f, EndTime = 1.83f, Hidden = true },
            } },
            { "block_dive", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.15f, BoneName = "face", Offset = new Vector2(2.0f, -8.5f) },
                new BoneSegment { StartTime = 1.15f, EndTime = 2.00f, Hidden = true },
            } },
            { "block_loop", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.00f, Hidden = true },
            } },
            { "sleep", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.35f, Hidden = true },
            } },
            { "sleep_loop", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.00f, Hidden = true },
            } },
            { "wake_up", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.86f, Hidden = true },
                new BoneSegment { StartTime = 0.86f, EndTime = 1.50f, BoneName = "face", Offset = new Vector2(2.0f, -8.5f) },
            } },
        } },
    new EyeConfig { Offset = new Vector2(-162.8f, -15.1f), Scale = 0.40f, AnchorBone = "face",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "block", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.46f, BoneName = "face", Offset = new Vector2(-162.8f, -15.1f) },
                new BoneSegment { StartTime = 0.46f, EndTime = 1.83f, Hidden = true },
            } },
            { "block_dive", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.15f, BoneName = "face", Offset = new Vector2(-162.8f, -15.1f) },
                new BoneSegment { StartTime = 1.15f, EndTime = 2.00f, Hidden = true },
            } },
            { "block_loop", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.00f, Hidden = true },
            } },
            { "sleep", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.35f, Hidden = true },
            } },
            { "sleep_loop", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.00f, Hidden = true },
            } },
            { "wake_up", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.86f, Hidden = true },
                new BoneSegment { StartTime = 0.86f, EndTime = 1.50f, BoneName = "face", Offset = new Vector2(-162.8f, -15.1f) },
            } },
        } },
}},
        { "PARAFRIGHT", new EyeConfig[] {
    new EyeConfig { Offset = new Vector2(2.1f, -59.2f), Scale = 0.67f, AnchorBone = "head_twist",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "die", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.31f, BoneName = "head_twist", Offset = new Vector2(2.1f, -59.2f) },
                new BoneSegment { StartTime = 0.31f, EndTime = 1.00f, BoneName = "head_twist", Offset = new Vector2(2.1f, -59.2f), OpacityStart = 1.00f, OpacityEnd = 0.00f },
            } },
            { "hurt_stunned", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.57f, BoneName = "head_twist", Offset = new Vector2(2.1f, -59.2f), OpacityStart = 0.50f, OpacityEnd = 0.50f },
            } },
            { "wake_up", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.46f, BoneName = "head_twist", Offset = new Vector2(2.1f, -59.2f), OpacityStart = 0.50f, OpacityEnd = 1.00f },
                new BoneSegment { StartTime = 0.46f, EndTime = 0.67f, BoneName = "head_twist", Offset = new Vector2(2.1f, -59.2f) },
            } },
            { "stunned_loop", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 4.00f, BoneName = "head_twist", Offset = new Vector2(2.1f, -59.2f), OpacityStart = 0.50f, OpacityEnd = 0.50f },
            } },
            { "stun", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.65f, BoneName = "head_twist", Offset = new Vector2(2.1f, -59.2f), OpacityStart = 1.00f, OpacityEnd = 0.50f },
                new BoneSegment { StartTime = 0.65f, EndTime = 0.77f, BoneName = "head_twist", Offset = new Vector2(2.1f, -59.2f), OpacityStart = 0.50f, OpacityEnd = 0.50f },
            } },
            { "spawn", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.50f, BoneName = "head_twist", Offset = new Vector2(2.1f, -59.2f), OpacityStart = 0.00f, OpacityEnd = 1.00f },
                new BoneSegment { StartTime = 0.50f, EndTime = 1.17f, BoneName = "head_twist", Offset = new Vector2(2.1f, -59.2f) },
            } },
        } },
    new EyeConfig { Offset = new Vector2(141.6f, -45.0f), Scale = 0.67f, AnchorBone = "head_twist",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "die", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.31f, BoneName = "head_twist", Offset = new Vector2(141.6f, -45.0f) },
                new BoneSegment { StartTime = 0.31f, EndTime = 1.00f, BoneName = "head_twist", Offset = new Vector2(141.6f, -45.0f), OpacityStart = 1.00f, OpacityEnd = 0.00f },
            } },
            { "hurt_stunned", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.57f, BoneName = "head_twist", Offset = new Vector2(141.6f, -45.0f), OpacityStart = 0.50f, OpacityEnd = 0.50f },
            } },
            { "wake_up", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.46f, BoneName = "head_twist", Offset = new Vector2(141.6f, -45.0f), OpacityStart = 0.50f, OpacityEnd = 1.00f },
                new BoneSegment { StartTime = 0.46f, EndTime = 0.67f, BoneName = "head_twist", Offset = new Vector2(141.6f, -45.0f) },
            } },
            { "stunned_loop", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 4.00f, BoneName = "head_twist", Offset = new Vector2(141.6f, -45.0f), OpacityStart = 0.50f, OpacityEnd = 0.50f },
            } },
            { "stun", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.65f, BoneName = "head_twist", Offset = new Vector2(141.6f, -45.0f), OpacityStart = 1.00f, OpacityEnd = 0.50f },
                new BoneSegment { StartTime = 0.65f, EndTime = 0.77f, BoneName = "head_twist", Offset = new Vector2(141.6f, -45.0f), OpacityStart = 0.50f, OpacityEnd = 0.50f },
            } },
            { "spawn", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.50f, BoneName = "head_twist", Offset = new Vector2(141.6f, -45.0f), OpacityStart = 0.00f, OpacityEnd = 1.00f },
                new BoneSegment { StartTime = 0.50f, EndTime = 1.17f, BoneName = "head_twist", Offset = new Vector2(141.6f, -45.0f) },
            } },
        } },
}},
        { "PHANTASMAL_GARDENER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-0.0f, 0.0f), Scale = 0.40f, AnchorBone = "mouth_upper2",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 0.75f, BoneName = "mouth_upper2", Offset = new Vector2(-0.0f, 0.0f) },
                        new BoneSegment { StartTime = 0.75f, EndTime = 1.07f, Hidden = true },
                    } },
                } },
        }},
        { "PHROG_PARASITE", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(101.0f, -328.9f), Scale = 1.50f, AnchorBone = "bubble_d",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 1.69f, BoneName = "bubble_d", Offset = new Vector2(101.0f, -328.9f) },
                        new BoneSegment { StartTime = 1.69f, EndTime = 2.50f, Hidden = true },
                    } },
                } },
            new EyeConfig { Offset = new Vector2(25.8f, 155.3f), Scale = 1.50f, AnchorBone = "bubble_d",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 1.69f, BoneName = "bubble_d", Offset = new Vector2(25.8f, 155.3f) },
                        new BoneSegment { StartTime = 1.69f, EndTime = 2.50f, Hidden = true },
                    } },
                } },
        }},
        { "PUNCH_CONSTRUCT", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-6.9f, 28.6f), Scale = 0.41f, AnchorBone = "head" },
            new EyeConfig { Offset = new Vector2(-0.7f, -63.4f), Scale = 0.41f, AnchorBone = "head" },
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
        { "QUEEN", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-70.7f, -57.4f), Scale = 0.71f, AnchorBone = "face",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 1.12f, BoneName = "face", Offset = new Vector2(-70.7f, -57.4f) },
                        new BoneSegment { StartTime = 1.12f, EndTime = 1.26f, BoneName = "face", Offset = new Vector2(-70.7f, -57.4f), OpacityStart = 1.00f, OpacityEnd = 0.00f },
                        new BoneSegment { StartTime = 1.26f, EndTime = 2.22f, Hidden = true },
                    } },
                } },
            new EyeConfig { Offset = new Vector2(73.2f, -60.5f), Scale = 0.71f, AnchorBone = "face",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 1.12f, BoneName = "face", Offset = new Vector2(73.2f, -60.5f) },
                        new BoneSegment { StartTime = 1.12f, EndTime = 1.26f, BoneName = "face", Offset = new Vector2(73.2f, -60.5f), OpacityStart = 1.00f, OpacityEnd = 0.00f },
                        new BoneSegment { StartTime = 1.26f, EndTime = 2.22f, Hidden = true },
                    } },
                } },
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
        { "SCROLL_OF_BITING", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(4.9f, -76.0f), Scale = 0.48f, AnchorBone = "top_scroll_base" },
            new EyeConfig { Offset = new Vector2(1.4f, 25.9f), Scale = 0.48f, AnchorBone = "top_scroll_base" },
        }},
        { "SHRINKER_BEETLE", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(0.0f, -0.0f), Scale = 0.48f, AnchorBone = "eye_f" },
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
        
        { "SLIMED_BERSERKER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(0.0f, 0.0f), Scale = 0.73f, AnchorBone = "eye_2_pupil" },
            new EyeConfig { Offset = new Vector2(6.2f, -1.8f), Scale = 0.60f, AnchorBone = "eye_7_pupil" },
            new EyeConfig { Offset = new Vector2(6.4f, -5.8f), Scale = 0.67f, AnchorBone = "eye_8_pupil" },
            new EyeConfig { Offset = new Vector2(32.0f, 7.4f), Scale = 1.20f, AnchorBone = "eye_5_pupil" },
            new EyeConfig { Offset = new Vector2(-29.3f, -6.5f), Scale = 1.00f, AnchorBone = "eye_3_pupil" },
            new EyeConfig { Offset = new Vector2(21.8f, -4.5f), Scale = 0.93f, AnchorBone = "eye_4_pupil" },
            new EyeConfig { Offset = new Vector2(13.0f, 3.1f), Scale = 1.07f, AnchorBone = "pupil_1" },
            new EyeConfig { Offset = new Vector2(-15.8f, 3.2f), Scale = 0.67f, AnchorBone = "eye_10_pupil" },
            new EyeConfig { Offset = new Vector2(2.1f, 4.7f), Scale = 0.60f, AnchorBone = "eye_6_pupil" },
            new EyeConfig { Offset = new Vector2(3.0f, -1.5f), Scale = 0.40f, AnchorBone = "eye_9_pupil" },
        }},
        { "SLUMBERING_BEETLE", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(127.3f, 134.5f), Scale = 1.10f, AnchorBone = "head_persp",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "roll", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 0.27f, BoneName = "head_persp", Offset = new Vector2(127.3f, 134.5f) },
                        new BoneSegment { StartTime = 0.27f, EndTime = 1.44f, Hidden = true },
                        new BoneSegment { StartTime = 1.44f, EndTime = 1.90f, BoneName = "head_persp", Offset = new Vector2(127.3f, 134.5f) },
                    } },
                } },
        }},
        { "SOUL_NEXUS", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(0.0f, 0.0f), Scale = 2.00f, AnchorBone = "eye_ball",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 0.54f, BoneName = "eye_ball", Offset = new Vector2(0.0f, 0.0f) },
                        new BoneSegment { StartTime = 0.54f, EndTime = 3.00f, BoneName = "eye_ball", Offset = new Vector2(0.0f, 0.0f), OpacityStart = 1.00f, OpacityEnd = 0.00f },
                    } },
                } },
        }},
        { "SLITHERING_STRANGLER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(88.3f, 8.5f), Scale = 0.47f, AnchorBone = "head_twist" },
            new EyeConfig { Offset = new Vector2(-13.0f, -24.1f), Scale = 0.47f, AnchorBone = "head_twist" },
        }},
        { "SLUDGE_SPINNER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(1.5f, -114.2f), Scale = 0.40f, AnchorBone = "cog" },
            new EyeConfig { Offset = new Vector2(-104.6f, -111.6f), Scale = 0.40f, AnchorBone = "cog" },
        }},
        { "SPECTRAL_KNIGHT", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-36.3f, -72.0f), Scale = 0.45f, AnchorBone = "face_twist",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 2.90f, BoneName = "face_twist", Offset = new Vector2(-36.3f, -72.0f) },
                        new BoneSegment { StartTime = 2.90f, EndTime = 3.25f, BoneName = "face_twist", Offset = new Vector2(-36.3f, -72.0f), OpacityStart = 1.00f, OpacityEnd = 0.00f },
                        new BoneSegment { StartTime = 3.25f, EndTime = 4.86f, Hidden = true },
                    } },
                } },
            new EyeConfig { Offset = new Vector2(67.7f, -66.2f), Scale = 0.45f, AnchorBone = "face_twist",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 2.90f, BoneName = "face_twist", Offset = new Vector2(67.7f, -66.2f) },
                        new BoneSegment { StartTime = 2.90f, EndTime = 3.25f, BoneName = "face_twist", Offset = new Vector2(67.7f, -66.2f), OpacityStart = 1.00f, OpacityEnd = 0.00f },
                        new BoneSegment { StartTime = 3.25f, EndTime = 4.86f, Hidden = true },
                    } },
                } },
        }},
        { "TERROR_EEL", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(2.9f, 31.5f), Scale = 1.00f, AnchorBone = "eye_1" },
            new EyeConfig { Offset = new Vector2(13.4f, 32.5f), Scale = 1.25f, AnchorBone = "eye_2" },
            new EyeConfig { Offset = new Vector2(-12.9f, 80.5f), Scale = 1.25f, AnchorBone = "eye_3" },
            new EyeConfig { Offset = new Vector2(14.9f, -11.8f), Scale = 1.90f, AnchorBone = "eye_rotate_4-5" },
            new EyeConfig { Offset = new Vector2(-6.1f, 7.1f), Scale = 0.70f, AnchorBone = "eye_6" },
            new EyeConfig { Offset = new Vector2(-5.0f, -6.8f), Scale = 0.90f, AnchorBone = "eye_7" },
            new EyeConfig { Offset = new Vector2(-3.8f, -47.9f), Scale = 1.30f, AnchorBone = "eye_8" },
            new EyeConfig { Offset = new Vector2(-11.3f, -58.8f), Scale = 1.30f, AnchorBone = "eye_9-10" },
            new EyeConfig { Offset = new Vector2(28.4f, -6.3f), Scale = 0.90f, AnchorBone = "eye_16" },
            new EyeConfig { Offset = new Vector2(-5.3f, -23.6f), Scale = 0.80f, AnchorBone = "eye_12" },
            new EyeConfig { Offset = new Vector2(0.6f, -15.3f), Scale = 0.55f, AnchorBone = "eye_11" },
            new EyeConfig { Offset = new Vector2(13.2f, -16.6f), Scale = 1.55f, AnchorBone = "eye_13" },
            new EyeConfig { Offset = new Vector2(5.5f, -6.6f), Scale = 0.55f, AnchorBone = "eye_15" },
            new EyeConfig { Offset = new Vector2(0.0f, 0.0f), Scale = 0.55f, AnchorBone = "tail9" },
        }},
        { "TOUGH_EGG", new EyeConfig[] {
    new EyeConfig { Offset = new Vector2(-7.6f, 14.6f), Scale = 0.44f, AnchorBone = "eye_l",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "egg_die", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.33f, Hidden = true },
            } },
            { "egg_hatch", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.51f, Hidden = true },
                new BoneSegment { StartTime = 0.51f, EndTime = 1.13f, BoneName = "eye_l", Offset = new Vector2(-7.6f, 14.6f) },
            } },
            { "egg_hurt", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.53f, Hidden = true },
            } },
            { "egg_idle_loop", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.33f, Hidden = true },
            } },
            { "egg_spawn", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.53f, Hidden = true },
            } },
        } },
    new EyeConfig { Offset = new Vector2(9.1f, 13.0f), Scale = 0.44f, AnchorBone = "eye_r",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "egg_die", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.33f, Hidden = true },
            } },
            { "egg_hatch", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.51f, Hidden = true },
                new BoneSegment { StartTime = 0.51f, EndTime = 1.13f, BoneName = "eye_r", Offset = new Vector2(9.1f, 13.0f) },
            } },
            { "egg_hurt", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.53f, Hidden = true },
            } },
            { "egg_idle_loop", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.33f, Hidden = true },
            } },
            { "egg_spawn", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.53f, Hidden = true },
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
        { "SNAPPING_JAXFRUIT", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(246.6f, -103.2f), Scale = 0.67f, AnchorBone = "body" },
            new EyeConfig { Offset = new Vector2(260.6f, 90.4f), Scale = 0.67f, AnchorBone = "body" },
            new EyeConfig { Offset = new Vector2(22.2f, 17.3f), Scale = 0.07f, AnchorBone = "chomper3" },
            new EyeConfig { Offset = new Vector2(23.5f, 18.0f), Scale = 0.07f, AnchorBone = "chomper2" },
            new EyeConfig { Offset = new Vector2(21.8f, 18.2f), Scale = 0.07f, AnchorBone = "chomper1" },
        }},
        { "SOUL_FYSH", new EyeConfig[] {
    new EyeConfig { Offset = new Vector2(121.9f, -7.1f), Scale = 1.16f, AnchorBone = "eye",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "attack_debuff", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.40f, BoneName = "eye", Offset = new Vector2(121.9f, -7.1f), OpacityStart = 0.50f, OpacityEnd = 0.50f },
            } },
            { "die", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.18f, BoneName = "eye", Offset = new Vector2(121.9f, -7.1f) },
                new BoneSegment { StartTime = 1.18f, EndTime = 2.42f, BoneName = "eye", Offset = new Vector2(121.9f, -7.1f), OpacityStart = 1.00f, OpacityEnd = 0.00f },
                new BoneSegment { StartTime = 2.42f, EndTime = 4.20f, Hidden = true },
            } },
            { "die_intangible", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.64f, BoneName = "eye", Offset = new Vector2(121.9f, -7.1f), OpacityStart = 0.50f, OpacityEnd = 0.50f },
                new BoneSegment { StartTime = 1.64f, EndTime = 2.47f, BoneName = "eye", Offset = new Vector2(121.9f, -7.1f), OpacityStart = 0.50f, OpacityEnd = 0.00f },
                new BoneSegment { StartTime = 2.47f, EndTime = 4.20f, Hidden = true },
            } },
            { "hurt_intangible", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.90f, BoneName = "eye", Offset = new Vector2(121.9f, -7.1f), OpacityStart = 0.50f, OpacityEnd = 0.50f },
            } },
            { "intangible_end", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.48f, BoneName = "eye", Offset = new Vector2(121.9f, -7.1f), OpacityStart = 0.50f, OpacityEnd = 0.50f },
                new BoneSegment { StartTime = 0.48f, EndTime = 1.00f, BoneName = "eye", Offset = new Vector2(121.9f, -7.1f), OpacityStart = 0.50f, OpacityEnd = 1.00f },
            } },
            { "intangible_loop", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 10.00f, BoneName = "eye", Offset = new Vector2(121.9f, -7.1f), OpacityStart = 0.50f, OpacityEnd = 0.50f },
            } },
            { "intangible_start", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.27f, BoneName = "eye", Offset = new Vector2(121.9f, -7.1f) },
                new BoneSegment { StartTime = 0.27f, EndTime = 1.13f, BoneName = "eye", Offset = new Vector2(121.9f, -7.1f), OpacityStart = 1.00f, OpacityEnd = 0.50f },
            } },
        } },
}},
        { "THE_FORGOTTEN", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(147.3f, 38.0f), Scale = 1.00f, AnchorBone = "face",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 1.24f, BoneName = "face", Offset = new Vector2(145.7f, 36.0f) },
                        new BoneSegment { StartTime = 1.24f, EndTime = 2.00f, BoneName = "", Offset = new Vector2(0.0f, 0.0f) },
                    } },
                } },
            new EyeConfig { Offset = new Vector2(-141.6f, 22.8f), Scale = 1.00f, AnchorBone = "face",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 1.24f, BoneName = "face", Offset = new Vector2(-148.2f, 26.9f) },
                        new BoneSegment { StartTime = 1.24f, EndTime = 2.00f, BoneName = "", Offset = new Vector2(0.0f, 0.0f) },
                    } },
                } },
        }},
        { "THE_INSATIABLE", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(0.0f, -0.0f), Scale = 0.97f, AnchorBone = "rim_01",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 1.44f, BoneName = "rim_01", Offset = new Vector2(0.0f, -0.0f) },
                        new BoneSegment { StartTime = 1.44f, EndTime = 2.50f, Hidden = true },
                    } },
                    { "eat_player", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 0.40f, BoneName = "rim_01", Offset = new Vector2(0.0f, -0.0f) },
                        new BoneSegment { StartTime = 0.40f, EndTime = 2.58f, Hidden = true },
                        new BoneSegment { StartTime = 2.58f, EndTime = 3.17f, BoneName = "rim_01", Offset = new Vector2(0.0f, -0.0f) },
                    } },
                } },
            new EyeConfig { Offset = new Vector2(-17.9f, -51.1f), Scale = 0.97f, AnchorBone = "rim_11",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 1.54f, BoneName = "rim_11", Offset = new Vector2(-17.9f, -51.1f) },
                        new BoneSegment { StartTime = 1.54f, EndTime = 2.50f, Hidden = true },
                    } },
                    { "eat_player", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 0.40f, BoneName = "rim_11", Offset = new Vector2(-17.9f, -51.1f) },
                        new BoneSegment { StartTime = 0.40f, EndTime = 2.58f, Hidden = true },
                        new BoneSegment { StartTime = 2.58f, EndTime = 3.17f, BoneName = "rim_11", Offset = new Vector2(-17.9f, -51.1f) },
                    } },
                } },
        }},
        { "THE_LOST", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(74.9f, 18.7f), Scale = 0.57f, AnchorBone = "face",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 1.27f, BoneName = "face", Offset = new Vector2(79.5f, 14.5f) },
                        new BoneSegment { StartTime = 1.27f, EndTime = 2.00f, BoneName = "", Offset = new Vector2(79.5f, 14.5f) },
                    } },
                } },
            new EyeConfig { Offset = new Vector2(-53.9f, 33.5f), Scale = 0.57f, AnchorBone = "face",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 1.28f, BoneName = "face", Offset = new Vector2(-60.9f, 18.0f) },
                        new BoneSegment { StartTime = 1.28f, EndTime = 2.00f, BoneName = "", Offset = new Vector2(-60.9f, 18.0f) },
                    } },
                } },
        }},
        { "THIEVING_HOPPER", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(74.6f, -38.5f), Scale = 0.64f, AnchorBone = "face" },
            new EyeConfig { Offset = new Vector2(-61.1f, -40.2f), Scale = 0.64f, AnchorBone = "face" },
        }},
        { "TOADPOLE", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(0.0f, -0.0f), Scale = 0.43f, AnchorBone = "pupil_front" },
        }},
        { "TORCH_HEAD_AMALGAM", new EyeConfig[] {
    new EyeConfig { Offset = new Vector2(-108.7f, 29.5f), Scale = 0.67f, AnchorBone = "torch_2_attach",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "die", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.06f, BoneName = "torch_2_attach", Offset = new Vector2(-108.7f, 29.5f) },
                new BoneSegment { StartTime = 1.06f, EndTime = 3.47f, Hidden = true },
            } },
        } },
    new EyeConfig { Offset = new Vector2(72.4f, -66.3f), Scale = 0.67f, AnchorBone = "torch_2_attach",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "die", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.06f, BoneName = "torch_2_attach", Offset = new Vector2(72.4f, -66.3f) },
                new BoneSegment { StartTime = 1.06f, EndTime = 3.47f, Hidden = true },
            } },
        } },
    new EyeConfig { Offset = new Vector2(-79.1f, 47.2f), Scale = 0.67f, AnchorBone = "torch_3_attach",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "die", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.06f, BoneName = "torch_3_attach", Offset = new Vector2(-79.1f, 47.2f) },
                new BoneSegment { StartTime = 1.06f, EndTime = 3.47f, Hidden = true },
            } },
        } },
    new EyeConfig { Offset = new Vector2(36.6f, -42.2f), Scale = 0.67f, AnchorBone = "torch_3_attach",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "die", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.06f, BoneName = "torch_3_attach", Offset = new Vector2(36.6f, -42.2f) },
                new BoneSegment { StartTime = 1.06f, EndTime = 3.47f, Hidden = true },
            } },
        } },
    new EyeConfig { Offset = new Vector2(-74.7f, 22.0f), Scale = 0.67f, AnchorBone = "torch_1_attach",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "die", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.06f, BoneName = "torch_1_attach", Offset = new Vector2(-74.7f, 22.0f) },
                new BoneSegment { StartTime = 1.06f, EndTime = 3.47f, Hidden = true },
            } },
        } },
    new EyeConfig { Offset = new Vector2(52.4f, -56.6f), Scale = 0.67f, AnchorBone = "torch_1_attach",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "die", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.06f, BoneName = "torch_1_attach", Offset = new Vector2(52.4f, -56.6f) },
                new BoneSegment { StartTime = 1.06f, EndTime = 3.47f, Hidden = true },
            } },
        } },
}},
        { "TUNNELER", new EyeConfig[] {
    new EyeConfig { Offset = new Vector2(1.5f, -0.6f), Scale = 0.61f, AnchorBone = "eye_r",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "burrow", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.74f, BoneName = "eye_r", Offset = new Vector2(1.5f, -0.6f) },
                new BoneSegment { StartTime = 0.74f, EndTime = 0.77f, BoneName = "eye_r", Offset = new Vector2(1.5f, -0.6f), OpacityStart = 1.00f, OpacityEnd = 0.00f },
                new BoneSegment { StartTime = 0.77f, EndTime = 1.47f, Hidden = true },
            } },
            { "hidden_attack", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.57f, Hidden = true },
            } },
            { "hidden_die", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.04f, Hidden = true },
                new BoneSegment { StartTime = 0.04f, EndTime = 1.40f, BoneName = "eye_r", Offset = new Vector2(1.5f, -0.6f) },
            } },
            { "hidden_loop", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.33f, Hidden = true },
            } },
            { "unburrow_attack", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.39f, Hidden = true },
                new BoneSegment { StartTime = 0.39f, EndTime = 1.07f, BoneName = "eye_r", Offset = new Vector2(1.5f, -0.6f) },
            } },
        } },
    new EyeConfig { Offset = new Vector2(-6.8f, 3.1f), Scale = 0.61f, AnchorBone = "eye_l",
        BoneSegments = new Dictionary<string, BoneSegment[]> {
            { "burrow", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.75f, BoneName = "eye_l", Offset = new Vector2(-6.8f, 3.1f) },
                new BoneSegment { StartTime = 0.75f, EndTime = 0.77f, BoneName = "eye_l", Offset = new Vector2(-6.8f, 3.1f), OpacityStart = 1.00f, OpacityEnd = 0.00f },
                new BoneSegment { StartTime = 0.77f, EndTime = 1.47f, Hidden = true },
            } },
            { "hidden_attack", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 1.57f, Hidden = true },
            } },
            { "hidden_die", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.04f, Hidden = true },
                new BoneSegment { StartTime = 0.04f, EndTime = 1.40f, BoneName = "eye_l", Offset = new Vector2(-6.8f, 3.1f) },
            } },
            { "hidden_loop", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.33f, Hidden = true },
            } },
            { "unburrow_attack", new BoneSegment[] {
                new BoneSegment { StartTime = 0.00f, EndTime = 0.39f, Hidden = true },
                new BoneSegment { StartTime = 0.39f, EndTime = 1.07f, BoneName = "eye_l", Offset = new Vector2(-6.8f, 3.1f) },
            } },
        } },
}},
        { "VANTOM", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(-256.5f, -184.9f), Scale = 1.20f, AnchorBone = "head_twist",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 2.25f, BoneName = "head_twist", Offset = new Vector2(-256.5f, -184.9f) },
                        new BoneSegment { StartTime = 2.25f, EndTime = 2.36f, BoneName = "head_twist", Offset = new Vector2(-256.5f, -184.9f), OpacityStart = 1.00f, OpacityEnd = 0.00f },
                        new BoneSegment { StartTime = 2.36f, EndTime = 3.47f, Hidden = true },
                    } },
                } },
            new EyeConfig { Offset = new Vector2(-3.4f, -70.8f), Scale = 1.20f, AnchorBone = "head_twist",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "die", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 2.25f, BoneName = "head_twist", Offset = new Vector2(-3.4f, -70.8f) },
                        new BoneSegment { StartTime = 2.25f, EndTime = 2.36f, BoneName = "head_twist", Offset = new Vector2(-3.4f, -70.8f), OpacityStart = 1.00f, OpacityEnd = 0.00f },
                        new BoneSegment { StartTime = 2.36f, EndTime = 3.47f, Hidden = true },
                    } },
                } },
        }},
        { "WATERFALL_GIANT", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(128.6f, 142.8f), Scale = 1.16f, AnchorBone = "head_twist_layered",
                BoneSegments = new Dictionary<string, BoneSegment[]> {
                    { "erupt", new BoneSegment[] {
                        new BoneSegment { StartTime = 0.00f, EndTime = 0.04f, BoneName = "head_twist_layered", Offset = new Vector2(128.6f, 142.8f) },
                        new BoneSegment { StartTime = 0.04f, EndTime = 0.10f, BoneName = "head_twist_layered", Offset = new Vector2(128.6f, 142.8f), OpacityStart = 1.00f, OpacityEnd = 0.00f },
                        new BoneSegment { StartTime = 0.10f, EndTime = 1.13f, Hidden = true },
                    } },
                } },
        }},
    };
}