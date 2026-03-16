using Godot;

namespace GooglyEyes;

public class EyeConfig
{
    public Vector2 Offset;
    public float Scale;
    public string AnchorBone;
 
    /// <summary>
    /// If true, this eye is hidden by default and only appears during
    /// non-hidden BoneSegments in specific animations.
    /// </summary>
    public bool HiddenByDefault;
 
    /// <summary>
    /// Per-animation timed bone segments, keyed by spine animation name.
    /// Each segment defines a time range, which bone to follow, the offset,
    /// and whether the eye is hidden during that range.
    /// If an animation has no entry here, the eye uses its default bone/offset.
    /// </summary>
    public Dictionary<string, BoneSegment[]> BoneSegments;
}
 
public class BoneSegment
{
    public float StartTime;
    public float EndTime;
    public string BoneName;
    public Vector2 Offset;
 
    /// <summary>
    /// When true, the eye is invisible during this time range.
    /// BoneName and Offset are ignored.
    /// </summary>
    public bool Hidden;
}
