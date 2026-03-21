using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace GooglyEyes;
[HarmonyPatch(typeof(NCreature))]
public static class GooglyEyesPatch
{
    private class EyePhysics
    {
        public GodotObject AnchorBone;
        public string AnchorBoneName;
        public Vector2 ConfigOffset;
        public float ConfigScale;
        public Node2D Container;
        public Sprite2D Iris;
        public Vector2 IrisOffset;
        public Vector2 IrisVelocity;
        public Vector2 PrevWorldPos;
        public bool Initialized;
        public float MaxRadius;
        public float LastResolvedOpacity = 1f;
        public EyeConfig SourceConfig;
        public bool HiddenByDefault;
        public bool CurrentlyHidden;
    }

    private class CreatureEyeState
    {
        public List<EyePhysics> Eyes;
        public GodotObject SkeletonGodot;
        public Node2D SpineNode;
        public MegaSprite AnimController;
    }

    private static readonly Dictionary<ulong, CreatureEyeState> CreatureStates = new();
    private static readonly Random Rng = new();

    private const float Gravity = 300f;
    private const float Damping = 0.99f;
    private const float Bounciness = 0.9f;
    private const float BoneForceMultiplier = 3f;
    private const float ShakeImpulseMin = 1500f;
    private const float ShakeImpulseMax = 3000f;

    [HarmonyPatch("_Ready")]
    [HarmonyPostfix]
    static void Ready_Postfix(NCreature __instance)
    {
        try
        {
            var creature = __instance.Entity;
            if (creature == null) return;

            var creatureId = creature.ModelId.Entry;
            if (!CreatureGooglyEyesRegistry.Configs.TryGetValue(creatureId, out var configs)) return;

            var spineBody = __instance.Visuals?.SpineBody;
            if (spineBody == null) return;

            var skeleton = spineBody.GetSkeleton();
            if (skeleton == null) return;

            var skeletonGodot = skeleton.BoundObject as GodotObject;
            if (skeletonGodot == null) return;

            var spineNode = spineBody.BoundObject as Node2D;
            if (spineNode == null) return;

            var eyeTexture = ResourceLoader.Load<Texture2D>("res://GooglyEyes/googly_eye.png");
            var irisTexture = ResourceLoader.Load<Texture2D>("res://GooglyEyes/googly_iris.png");
            if (eyeTexture == null || irisTexture == null) return;

            float eyeRadius = eyeTexture.GetWidth() / 2f;
            float irisRadius = irisTexture.GetWidth() / 2f;

            var localEyes = new List<EyePhysics>();

            int eyeIndex = 0;
            foreach (var config in configs)
            {
                var anchorBone = skeletonGodot.Call("find_bone", config.AnchorBone).AsGodotObject();
                if (anchorBone == null) continue;

                var eyeContainer = new Node2D();
                eyeContainer.Name = $"GooglyEye_{eyeIndex}";
                eyeContainer.Scale = Vector2.One * config.Scale;

                var eyeSprite = new Sprite2D { Texture = eyeTexture, Name = "EyeBacking" };
                var irisSprite = new Sprite2D { Texture = irisTexture, Name = "Iris" };

                eyeSprite.UseParentMaterial = true;
                irisSprite.UseParentMaterial = true;

                eyeContainer.AddChild(eyeSprite);
                eyeContainer.AddChild(irisSprite);
                spineNode.AddChild(eyeContainer);

                float maxRadius = eyeRadius - irisRadius;
                if (maxRadius < 1f) maxRadius = 1f;

                bool hidden = config.HiddenByDefault;
                eyeContainer.Visible = !hidden;

                localEyes.Add(new EyePhysics
                {
                    AnchorBone = anchorBone,
                    AnchorBoneName = config.AnchorBone,
                    ConfigOffset = config.Offset,
                    ConfigScale = config.Scale,
                    Container = eyeContainer,
                    Iris = irisSprite,
                    IrisOffset = Vector2.Down * maxRadius,
                    IrisVelocity = Vector2.Zero,
                    PrevWorldPos = Vector2.Zero,
                    Initialized = false,
                    MaxRadius = maxRadius,
                    SourceConfig = config,
                    HiddenByDefault = hidden,
                    CurrentlyHidden = hidden
                });

                eyeIndex++;
            }

            if (localEyes.Count > 0)
            {
                var state = new CreatureEyeState
                {
                    Eyes = localEyes,
                    SkeletonGodot = skeletonGodot,
                    SpineNode = spineNode,
                    AnimController = spineBody
                };

                CreatureStates[__instance.GetInstanceId()] = state;

                spineNode.Connect("world_transforms_changed", Callable.From<Variant>((arg) =>
                {
                    if (!GodotObject.IsInstanceValid(state.SpineNode)) return;
                    UpdateEyes(state);
                }));

                GD.Print("[GooglyEyes] Applied " + configs.Length + " eyes to " + creatureId);
                
                // Resolve initial visibility/opacity immediately to avoid a
                // single frame of full-opacity eyes before the signal fires.
                UpdateEyes(state);
            }
        }
        catch (Exception e)
        {
            GD.PrintErr("[GooglyEyes] Error: " + e);
        }
    }

    private static void UpdateEyes(CreatureEyeState state)
    {
        if (!GodotObject.IsInstanceValid(state.SpineNode)) return;

        float currentTime = 0f;
        string currentAnimName = "idle_loop";
        try
        {
            var animState = state.AnimController.GetAnimationState();
            var currentEntry = animState.GetCurrent(0);
            if (currentEntry != null)
            {
                float duration = currentEntry.GetAnimationEnd();
                if (duration > 0f)
                {
                    float rawTime = currentEntry.GetTrackTime();
                    currentTime = rawTime >= duration ? duration : rawTime % duration;
                }
                currentAnimName = currentEntry.GetAnimation().GetName();
            }
        }
        catch (ObjectDisposedException)
        {
            return;
        }

        foreach (var eye in state.Eyes)
        {
            if (!GodotObject.IsInstanceValid(eye.Container)) continue;

            string activeBone;
            Vector2 activeOffset;
            bool shouldHide;
            ResolveSegmentState(eye, currentAnimName, currentTime, state.SkeletonGodot,
                out activeBone, out activeOffset, out shouldHide, out float resolvedOpacity);

            if (shouldHide)
            {
                if (!eye.CurrentlyHidden)
                {
                    eye.CurrentlyHidden = true;
                    eye.Container.Visible = false;
                }
                continue;
            }

            if (eye.CurrentlyHidden)
            {
                eye.CurrentlyHidden = false;
                eye.Container.Visible = true;
                eye.Initialized = false;
                eye.IrisOffset = Vector2.Down * eye.MaxRadius;
                eye.IrisVelocity = Vector2.Zero;
                eye.Iris.Position = eye.IrisOffset;
            }

            eye.Container.Modulate = new Color(1f, 1f, 1f, resolvedOpacity);
            eye.LastResolvedOpacity = resolvedOpacity;

            if (activeBone != eye.AnchorBoneName)
            {
                var newBone = state.SkeletonGodot.Call("find_bone", activeBone).AsGodotObject();
                if (newBone != null)
                {
                    eye.AnchorBone = newBone;
                    eye.AnchorBoneName = activeBone;
                    eye.Initialized = false;
                    eye.IrisOffset = Vector2.Down * eye.MaxRadius;
                    eye.IrisVelocity = Vector2.Zero;
                    eye.Iris.Position = eye.IrisOffset;
                }
            }

            eye.ConfigOffset = activeOffset;

            var wx = (float)eye.AnchorBone.Call("get_world_x");
            var wy = (float)eye.AnchorBone.Call("get_world_y");
            var bonePos = new Vector2(wx, wy);

            var rotatedOffset = RotateOffsetByBone(eye.ConfigOffset, eye.AnchorBone);
            eye.Container.Position = bonePos + rotatedOffset;

            if (!eye.Initialized)
            {
                eye.PrevWorldPos = bonePos;
                eye.Initialized = true;
                continue;
            }

            var boneDelta = bonePos - eye.PrevWorldPos;
            eye.PrevWorldPos = bonePos;

            eye.IrisVelocity -= boneDelta * BoneForceMultiplier;

            float dt = 1f / 60f;
            eye.IrisVelocity += Vector2.Down * Gravity * dt;
            eye.IrisVelocity *= Damping;
            eye.IrisOffset += eye.IrisVelocity * dt;

            float dist = eye.IrisOffset.Length();
            if (dist > eye.MaxRadius)
            {
                var normal = eye.IrisOffset.Normalized();
                eye.IrisOffset = normal * eye.MaxRadius;
                var dot = eye.IrisVelocity.Dot(normal);
                if (dot > 0)
                    eye.IrisVelocity -= normal * dot * (1f + Bounciness);
            }

            eye.Iris.Position = eye.IrisOffset;
        }
    }

    private static void ResolveSegmentState(
        EyePhysics eye, string animName, float time, GodotObject skeletonGodot,
        out string boneName, out Vector2 offset, out bool hidden, out float opacity)
    {
        var config = eye.SourceConfig;

        if (config.BoneSegments != null
            && config.BoneSegments.TryGetValue(animName, out var segments)
            && segments.Length > 0)
        {
            for (int i = 0; i < segments.Length; i++)
            {
                var seg = segments[i];
                if (time >= seg.StartTime && time < seg.EndTime)
                {
                    hidden = seg.Hidden;
                    boneName = seg.Hidden ? eye.AnchorBoneName : (seg.BoneName ?? config.AnchorBone);
                    offset = seg.Hidden ? eye.ConfigOffset : seg.Offset;
                    float segDur = seg.EndTime - seg.StartTime;
                    float t = segDur > 0f ? (time - seg.StartTime) / segDur : 0f;
                    opacity = Mathf.Lerp(seg.OpacityStart, seg.OpacityEnd, t);
                    return;
                }
            }

            var last = segments[^1];
            hidden = last.Hidden;
            boneName = last.Hidden ? eye.AnchorBoneName : (last.BoneName ?? config.AnchorBone);
            offset = last.Hidden ? eye.ConfigOffset : last.Offset;
            opacity = last.OpacityEnd;
            return;
        }

        hidden = config.HiddenByDefault;
        boneName = config.AnchorBone;
        offset = config.Offset;
        opacity = eye.LastResolvedOpacity;
    }

    private static Vector2 RotateOffsetByBone(Vector2 offset, GodotObject bone)
    {
        float a = (float)bone.Call("get_a");
        float c = (float)bone.Call("get_c");
        float rot = Mathf.Atan2(c, a);
        float cos = Mathf.Cos(rot);
        float sin = Mathf.Sin(rot);
        return new Vector2(
            offset.X * cos - offset.Y * sin,
            offset.X * sin + offset.Y * cos
        );
    }

    [HarmonyPatch("SetAnimationTrigger")]
    [HarmonyPostfix]
    static void SetAnimationTrigger_Postfix(NCreature __instance, string trigger)
    {
        if (trigger != "Hit" && trigger != "Attack") return;
        if (!CreatureStates.TryGetValue(__instance.GetInstanceId(), out var state)) return;

        float impulseMultiplier = trigger == "Hit" ? 1f : 0.5f;

        foreach (var eye in state.Eyes)
        {
            if (eye.CurrentlyHidden) continue;
            float angle = (float)(Rng.NextDouble() * Math.PI * 2);
            float strength = (ShakeImpulseMin + (float)(Rng.NextDouble() * (ShakeImpulseMax - ShakeImpulseMin))) * impulseMultiplier;
            eye.IrisVelocity += new Vector2(
                (float)Math.Cos(angle) * strength,
                (float)Math.Sin(angle) * strength
            );
        }
    }

    [HarmonyPatch("_ExitTree")]
    [HarmonyPostfix]
    static void ExitTree_Postfix(NCreature __instance)
    {
        CreatureStates.Remove(__instance.GetInstanceId());
    }
}