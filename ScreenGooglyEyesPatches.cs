using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Events;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;

namespace GooglyEyes;

[HarmonyPatch(typeof(NCharacterSelectScreen))]
public static class CharSelectBgGooglyEyesPatch
{
    [HarmonyPatch("SelectCharacter")]
    [HarmonyPostfix]
    static void SelectCharacter_Postfix(NCharacterSelectScreen __instance, CharacterModel characterModel, Control ____bgContainer)
    {
        try
        {
            if (characterModel == null || ____bgContainer == null) return;
            var charPath = characterModel.CharacterSelectBg;
            if (!ScreenGooglyEyesRegistry.Configs.ContainsKey(charPath)) return;
            // The bg scene was just added as the last child of _bgContainer
            if (____bgContainer.GetChildCount() == 0) return;
            var bgNode = ____bgContainer.GetChild(____bgContainer.GetChildCount() - 1);
            ScreenGooglyEyesHelper.ApplyEyesToScene(bgNode, charPath);
        }
        catch (Exception e)
        {
            GD.PrintErr("[GooglyEyes] CharSelectBg patch error: " + e);
        }
    }
}

/// <summary>
/// Shared logic for applying googly eyes to screen scenes that contain SpineSprites.
/// Used by both character select and ancient event background patches.
/// </summary>
public static class ScreenGooglyEyesHelper
{
    private static Texture2D _eyeTexture;
    private static Texture2D _irisTexture;

    private static void EnsureTextures()
    {
        _eyeTexture ??= ResourceLoader.Load<Texture2D>("res://GooglyEyes/googly_eye.png");
        _irisTexture ??= ResourceLoader.Load<Texture2D>("res://GooglyEyes/googly_iris.png");
    }

    public static void ApplyEyesToScene(Node sceneRoot, string scenePath)
    {
        if (!ScreenGooglyEyesRegistry.Configs.TryGetValue(scenePath, out var configs)) return;
        if (configs.Length == 0) return;

        EnsureTextures();
        if (_eyeTexture == null || _irisTexture == null) return;

        // Find the SpineSprite
        var spineNode = FindSpineSprite(sceneRoot);

        if (spineNode != null)
        {
            ApplySpineEyes(sceneRoot, scenePath, configs, spineNode);
        }
        else
        {
            ApplyStaticEyes(sceneRoot, scenePath, configs);
        }
    }

    private static void ApplyStaticEyes(Node sceneRoot, string scenePath, EyeConfig[] configs)
    {
        // Static scene: use Control-based nodes so modulate inherits correctly
        // when the game darkens parent containers via overlays/dialogs.
        Control parent = sceneRoot as Control;
        if (parent == null)
        {
            // If root is Node2D, try adding to it anyway — modulate works within Node2D trees
            if (sceneRoot is not Node2D parentNode) return;
            ApplyStaticEyesNode2D(parentNode, scenePath, configs);
            return;
        }

        Vector2 eyeSize = _eyeTexture.GetSize();
        Vector2 irisSize = _irisTexture.GetSize();
        float maxRadius = (eyeSize.X / 2f) - (irisSize.X / 2f);
        if (maxRadius < 1f) maxRadius = 1f;

        var driver = new StaticScreenEyeDriver
        {
            Name = "GooglyEyeDriver",
            Target = sceneRoot
        };

        foreach (var config in configs)
        {
            var container = new Control
            {
                Name = "GooglyEye",
                Size = Vector2.Zero,
                Position = config.Offset,
                Scale = Vector2.One * config.Scale,
                MouseFilter = Control.MouseFilterEnum.Ignore
            };
            if (config.Opacity < 0.99f)
                container.Modulate = new Color(1f, 1f, 1f, config.Opacity);

            var eyeRect = new TextureRect
            {
                Texture = _eyeTexture, Name = "EyeBacking",
                Size = eyeSize, Position = -eyeSize / 2f,
                MouseFilter = Control.MouseFilterEnum.Ignore
            };

            var irisWrapper = new Control
            {
                Name = "IrisWrapper", Size = Vector2.Zero,
                Position = Vector2.Zero,
                MouseFilter = Control.MouseFilterEnum.Ignore
            };

            var irisRect = new TextureRect
            {
                Texture = _irisTexture, Name = "Iris",
                Size = irisSize, Position = -irisSize / 2f,
                MouseFilter = Control.MouseFilterEnum.Ignore
            };

            irisWrapper.AddChild(irisRect);
            container.AddChild(eyeRect);
            container.AddChild(irisWrapper);
            parent.AddChild(container);

            driver.Eyes.Add(new StaticScreenEyeDriver.EyeData
            {
                IrisWrapper = irisWrapper,
                IrisOffset = Vector2.Zero,
                IrisVelocity = Vector2.Zero,
                MaxRadius = maxRadius
            });
        }

        parent.AddChild(driver);
        GD.Print("[GooglyEyes] Applied " + configs.Length + " static eyes to screen: " + scenePath);
    }

    /// <summary>Fallback for Node2D-rooted static scenes.</summary>
    private static void ApplyStaticEyesNode2D(Node2D parent, string scenePath, EyeConfig[] configs)
    {
        float eyeRadius = _eyeTexture.GetWidth() / 2f;
        float irisRadius = _irisTexture.GetWidth() / 2f;
        float maxRadius = eyeRadius - irisRadius;
        if (maxRadius < 1f) maxRadius = 1f;

        var driver = new StaticScreenEyeDriver
        {
            Name = "GooglyEyeDriver",
            Target = parent
        };

        foreach (var config in configs)
        {
            var eyeContainer = new Node2D
            {
                Name = "GooglyEye", ZIndex = 2,
                Position = config.Offset,
                Scale = Vector2.One * config.Scale
            };
            if (config.Opacity < 0.99f)
                eyeContainer.Modulate = new Color(1f, 1f, 1f, config.Opacity);

            var eyeSprite = new Sprite2D { Texture = _eyeTexture, Name = "EyeBacking" };
            var irisSprite = new Sprite2D { Texture = _irisTexture, Name = "Iris", ZIndex = 1 };

            eyeContainer.AddChild(eyeSprite);
            eyeContainer.AddChild(irisSprite);
            parent.AddChild(eyeContainer);

            driver.Eyes.Add(new StaticScreenEyeDriver.EyeData
            {
                IrisWrapper = irisSprite,
                IrisOffset = Vector2.Zero,
                IrisVelocity = Vector2.Zero,
                MaxRadius = maxRadius
            });
        }

        parent.AddChild(driver);
        GD.Print("[GooglyEyes] Applied " + configs.Length + " static eyes (Node2D) to screen: " + scenePath);
    }

    private static void ApplySpineEyes(Node sceneRoot, string scenePath, EyeConfig[] configs, Node spineNode)
    {
        MegaSprite animController;
        GodotObject skeletonGodot;
        Node2D spineNode2D;
        try
        {
            animController = new MegaSprite((Variant)spineNode);
            var skeleton = animController.GetSkeleton();
            if (skeleton == null) return;
            skeletonGodot = skeleton.BoundObject as GodotObject;
            spineNode2D = animController.BoundObject as Node2D;
            if (skeletonGodot == null || spineNode2D == null) return;
        }
        catch (Exception e)
        {
            GD.PrintErr("[GooglyEyes] Failed to init spine for screen: " + e);
            return;
        }

        float eyeRadius = _eyeTexture.GetWidth() / 2f;
        float irisRadius = _irisTexture.GetWidth() / 2f;
        float maxRadius = eyeRadius - irisRadius;
        if (maxRadius < 1f) maxRadius = 1f;

        var localEyes = new List<ScreenEyePhysics>();

        foreach (var config in configs)
        {
            var anchorBone = skeletonGodot.Call("find_bone", config.AnchorBone).AsGodotObject();
            if (anchorBone == null) continue;

            var eyeContainer = new Node2D();
            eyeContainer.Name = "GooglyEye";
            eyeContainer.ZIndex = 2;
            eyeContainer.Scale = Vector2.One * config.Scale;

            if (config.Opacity < 0.99f)
                eyeContainer.Modulate = new Color(1f, 1f, 1f, config.Opacity);

            var eyeSprite = new Sprite2D { Texture = _eyeTexture, Name = "EyeBacking" };
            var irisSprite = new Sprite2D { Texture = _irisTexture, Name = "Iris", ZIndex = 1 };

            eyeContainer.AddChild(eyeSprite);
            eyeContainer.AddChild(irisSprite);
            spineNode2D.AddChild(eyeContainer);

            bool hidden = config.HiddenByDefault;
            eyeContainer.Visible = !hidden;

            localEyes.Add(new ScreenEyePhysics
            {
                AnchorBone = anchorBone,
                AnchorBoneName = config.AnchorBone,
                ConfigOffset = config.Offset,
                ConfigScale = config.Scale,
                Container = eyeContainer,
                Iris = irisSprite,
                MaxRadius = maxRadius,
                SourceConfig = config,
                HiddenByDefault = hidden,
                CurrentlyHidden = hidden,
                LastResolvedOpacity = config.Opacity
            });
        }

        if (localEyes.Count == 0) return;

        var state = new ScreenEyeState
        {
            Eyes = localEyes,
            SkeletonGodot = skeletonGodot,
            SpineNode = spineNode2D,
            AnimController = animController
        };

        // Connect to world_transforms_changed for per-frame updates
        spineNode2D.Connect("world_transforms_changed", Callable.From<Variant>(arg =>
        {
            if (!GodotObject.IsInstanceValid(state.SpineNode)) return;
            UpdateScreenEyes(state);
        }));

        GD.Print("[GooglyEyes] Applied " + configs.Length + " eyes to screen: " + scenePath);
    }

    private static Node FindSpineSprite(Node root)
    {
        if (root.GetClass() == "SpineSprite") return root;
        foreach (var child in root.GetChildren())
        {
            var found = FindSpineSprite(child);
            if (found != null) return found;
        }
        return null;
    }

    private static void UpdateScreenEyes(ScreenEyeState state)
    {
        if (!GodotObject.IsInstanceValid(state.SpineNode)) return;

        // Get current animation name and time
        float currentTime = 0f;
        string currentAnimName = "animation";
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
        catch (ObjectDisposedException) { return; }
        catch (NullReferenceException) { return; }

        foreach (var eye in state.Eyes)
        {
            if (!GodotObject.IsInstanceValid(eye.Container)) continue;

            // Resolve from segments
            ResolveSegmentState(eye, currentAnimName, currentTime,
                out string activeBone, out Vector2 activeOffset,
                out bool shouldHide, out float resolvedOpacity);

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
                eye.IrisOffset = Vector2.Zero;
                eye.IrisVelocity = Vector2.Zero;
                eye.Iris.Position = Vector2.Zero;
            }

            // Apply opacity
            eye.Container.Modulate = new Color(1f, 1f, 1f, resolvedOpacity);
            eye.LastResolvedOpacity = resolvedOpacity;

            // Switch bone if needed
            if (activeBone != eye.AnchorBoneName)
            {
                var newBone = state.SkeletonGodot.Call("find_bone", activeBone).AsGodotObject();
                if (newBone != null)
                {
                    eye.AnchorBone = newBone;
                    eye.AnchorBoneName = activeBone;
                    eye.Initialized = false;
                    eye.IrisOffset = Vector2.Zero;
                    eye.IrisVelocity = Vector2.Zero;
                    eye.Iris.Position = Vector2.Zero;
                }
            }

            eye.ConfigOffset = activeOffset;

            // Position the eye
            var wx = (float)eye.AnchorBone.Call("get_world_x");
            var wy = (float)eye.AnchorBone.Call("get_world_y");
            var bonePos = new Vector2(wx, wy);
            var rotatedOffset = RotateOffsetByBone(eye.ConfigOffset, eye.AnchorBone);
            eye.Container.Position = bonePos + rotatedOffset;

            // Iris physics
            if (!eye.Initialized)
            {
                eye.PrevWorldPos = bonePos;
                eye.Initialized = true;
                continue;
            }

            var boneDelta = bonePos - eye.PrevWorldPos;
            eye.PrevWorldPos = bonePos;

            eye.IrisVelocity -= boneDelta * 3f;

            float dt = 1f / 60f;
            eye.IrisVelocity += Vector2.Down * 300f * dt;
            eye.IrisVelocity *= 0.99f;
            eye.IrisOffset += eye.IrisVelocity * dt;

            float dist = eye.IrisOffset.Length();
            if (dist > eye.MaxRadius)
            {
                var normal = eye.IrisOffset.Normalized();
                eye.IrisOffset = normal * eye.MaxRadius;
                var dot = eye.IrisVelocity.Dot(normal);
                if (dot > 0)
                    eye.IrisVelocity -= normal * dot * (1f + 0.9f);
            }

            eye.Iris.Position = eye.IrisOffset;
        }
    }

    private static void ResolveSegmentState(
        ScreenEyePhysics eye, string animName, float time,
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

    private class ScreenEyePhysics
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

    private class ScreenEyeState
    {
        public List<ScreenEyePhysics> Eyes;
        public GodotObject SkeletonGodot;
        public Node2D SpineNode;
        public MegaSprite AnimController;
    }
}

/// <summary>
/// Iris physics driver for static screen backgrounds (no Spine skeleton).
/// Tracks the scene root's global position and applies gravity + bounce to irises.
/// </summary>
public partial class StaticScreenEyeDriver : Node
{
    public Node Target;
    public List<EyeData> Eyes = new();
    public Vector2 PrevGlobalPos;
    public bool Initialized;

    private const float Gravity = 300f;
    private const float Damping = 0.99f;
    private const float Bounciness = 0.9f;
    private const float MoveForceMultiplier = 3f;

    public class EyeData
    {
        /// <summary>Either a Control (for Control-based trees) or Node2D (for Node2D trees).</summary>
        public Node IrisWrapper;
        public Vector2 IrisOffset;
        public Vector2 IrisVelocity;
        public float MaxRadius;
    }

    public override void _Process(double delta)
    {
        if (!GodotObject.IsInstanceValid(Target)) { QueueFree(); return; }

        float dt = (float)delta;
        if (dt <= 0f) return;

        Vector2 currentGlobalPos;
        if (Target is Control c) currentGlobalPos = c.GlobalPosition;
        else if (Target is Node2D n) currentGlobalPos = n.GlobalPosition;
        else return;

        if (!Initialized)
        {
            PrevGlobalPos = currentGlobalPos;
            Initialized = true;
            return;
        }

        var moveDelta = currentGlobalPos - PrevGlobalPos;
        PrevGlobalPos = currentGlobalPos;

        foreach (var eye in Eyes)
        {
            if (!GodotObject.IsInstanceValid(eye.IrisWrapper)) continue;

            eye.IrisVelocity -= moveDelta * MoveForceMultiplier;
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

            if (eye.IrisWrapper is Control ctrl) ctrl.Position = eye.IrisOffset;
            else if (eye.IrisWrapper is Node2D n2d) n2d.Position = eye.IrisOffset;
        }
    }
}

[HarmonyPatch(typeof(NAncientEventLayout))]
public static class AncientBgGooglyEyesPatch
{
    [HarmonyPatch("InitializeVisuals")]
    [HarmonyPostfix]
    static void InitializeVisuals_Postfix(NAncientEventLayout __instance, AncientEventModel ____ancientEvent, Control ____ancientBgContainer)
    {
        try
        {
            if (____ancientEvent == null || ____ancientBgContainer == null) return;

            var path = SceneHelper.GetScenePath("events/background_scenes/" + ____ancientEvent.Id.Entry.ToLowerInvariant());

            if (____ancientBgContainer.GetChildCount() == 0) return;
            var bgNode = ____ancientBgContainer.GetChild(____ancientBgContainer.GetChildCount() - 1);

            ScreenGooglyEyesHelper.ApplyEyesToScene(bgNode, path);
        }
        catch (Exception e)
        {
            GD.PrintErr("[GooglyEyes] Ancient bg patch error: " + e);
        }
    }
}