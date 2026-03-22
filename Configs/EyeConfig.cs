using Godot;

namespace GooglyEyes;

public class EyeConfig
{
    public Vector2 Offset;
    public float Scale;
    public string AnchorBone;
    public bool HiddenByDefault;
    public float Opacity = 1f;
    public Dictionary<string, BoneSegment[]> BoneSegments;
}

public class BoneSegment
{
    public float StartTime;
    public float EndTime;
    public string BoneName;
    public Vector2 Offset;
    public bool Hidden;
    public float OpacityStart = 1f;
    public float OpacityEnd = 1f;
}