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
        { "VANTOM", new EyeConfig[] {
            new EyeConfig { Offset = new Vector2(0.0f, 0.0f), Scale = 0.44f, AnchorBone = "track_blade" },
        }},
    };
}