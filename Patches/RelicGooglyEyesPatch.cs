using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Nodes.Screens.InspectScreens;
using MegaCrit.Sts2.Core.Saves;

namespace GooglyEyes;

[HarmonyPatch(typeof(NRelic))]
public static class RelicGooglyEyesPatch
{
    private static Texture2D _eyeTexture;
    private static Texture2D _irisTexture;
    private static Vector2 _eyeSize;
    private static Vector2 _irisSize;
 
    private static void EnsureTextures()
    {
        if (_eyeTexture == null)
        {
            _eyeTexture = ResourceLoader.Load<Texture2D>("res://GooglyEyes/googly_eye.png");
            if (_eyeTexture != null) _eyeSize = _eyeTexture.GetSize();
        }
        if (_irisTexture == null)
        {
            _irisTexture = ResourceLoader.Load<Texture2D>("res://GooglyEyes/googly_iris.png");
            if (_irisTexture != null) _irisSize = _irisTexture.GetSize();
        }
    }
 
    [HarmonyPatch("Reload")]
    [HarmonyPostfix]
    static void Reload_Postfix(NRelic __instance)
    {
        if (RelicEditorTab.SuppressRelicPatch) return;
        if (__instance.HasMeta("googly_editor_preview")) return;
 
        try
        {
            RemoveDriver(__instance);
 
            var model = __instance.Model;
            if (model == null) return;
 
            var relicId = model.Id.Entry;
            if (!RelicGooglyEyesRegistry.Configs.TryGetValue(relicId, out var configs)) return;
            if (configs.Length == 0) return;
 
            var icon = __instance.Icon;
            if (icon == null) return;
 
            EnsureTextures();
            if (_eyeTexture == null || _irisTexture == null) return;
 
            Vector2 iconCenter = icon.Size / 2f;
 
            float maxRadius = (_eyeSize.X / 2f) - (_irisSize.X / 2f);
            if (maxRadius < 1f) maxRadius = 1f;
 
            var driver = new RelicEyeDriver
            {
                Name = "GooglyEyeDriver",
                Relic = __instance,
                OriginalIconSize = icon.Size,
                Configs = configs
            };
 
            int eyeIndex = 0;
            foreach (var config in configs)
            {
                var container = new Control
                {
                    Name = $"GooglyEye_{eyeIndex}",
                    Size = Vector2.Zero,
                    Position = iconCenter + config.Offset,
                    Scale = Vector2.One * config.Scale,
                    Modulate = new Color(1f, 1f, 1f, config.Opacity),
                    MouseFilter = Control.MouseFilterEnum.Ignore
                };
 
                var eyeRect = new TextureRect
                {
                    Texture = _eyeTexture,
                    Name = "EyeBacking",
                    Size = _eyeSize,
                    Position = -_eyeSize / 2f,
                    UseParentMaterial = true,
                    MouseFilter = Control.MouseFilterEnum.Ignore
                };
 
                var irisWrapper = new Control
                {
                    Name = "IrisWrapper",
                    Size = Vector2.Zero,
                    Position = Vector2.Zero,
                    MouseFilter = Control.MouseFilterEnum.Ignore
                };
 
                var irisRect = new TextureRect
                {
                    Texture = _irisTexture,
                    Name = "Iris",
                    Size = _irisSize,
                    Position = -_irisSize / 2f,
                    UseParentMaterial = true,
                    MouseFilter = Control.MouseFilterEnum.Ignore
                };
 
                irisWrapper.AddChild(irisRect);
                container.AddChild(eyeRect);
                container.AddChild(irisWrapper);
                icon.AddChild(container);
 
                driver.Eyes.Add(new RelicEyeDriver.EyeData
                {
                    Container = container,
                    IrisWrapper = irisWrapper,
                    IrisOffset = Vector2.Down * maxRadius,
                    IrisVelocity = Vector2.Zero,
                    MaxRadius = maxRadius
                });
 
                eyeIndex++;
            }
 
            icon.AddChild(driver);
        }
        catch (Exception e)
        {
            GD.PrintErr("[GooglyEyes] Relic Reload error: " + e);
        }
    }
 
    private static void RemoveDriver(NRelic relic)
    {
        var icon = relic.Icon;
        if (icon == null) return;
 
        for (int i = icon.GetChildCount() - 1; i >= 0; i--)
        {
            var child = icon.GetChild(i);
            var name = child.Name.ToString();
            if (child is RelicEyeDriver || name.Contains("Googly"))
            {
                icon.RemoveChild(child);
                child.Free();
            }
        }
    }
}
 
public partial class RelicEyeDriver : Node
{
    public NRelic Relic;
    public List<EyeData> Eyes = new();
    public Vector2 PrevGlobalPos;
    public bool Initialized;
    public Vector2 OriginalIconSize;
    public RelicEyeConfig[] Configs;
    private int _sizeCheckFrames = 5;
    private bool _sizeChecked;

    private const float Gravity = 300f;
    private const float Damping = 0.99f;
    private const float Bounciness = 0.9f;
    private const float MoveForceMultiplier = 3f;

    public class EyeData
    {
        public Control Container;
        public Control IrisWrapper;
        public Vector2 IrisOffset;
        public Vector2 IrisVelocity;
        public float MaxRadius;
    }

    public override void _Process(double delta)
    {
        if (!GodotObject.IsInstanceValid(Relic)) { QueueFree(); return; }

        if (_sizeCheckFrames > 0)
        {
            _sizeCheckFrames--;
            var icon = Relic.Icon;
            if (icon != null && Configs != null && OriginalIconSize.X > 0)
            {
                var currentSize = icon.Size;
                if (currentSize != OriginalIconSize)
                {
                    float sizeRatio = currentSize.X / OriginalIconSize.X;
                    Vector2 newCenter = currentSize / 2f;
                    for (int i = 0; i < Eyes.Count && i < Configs.Length; i++)
                    {
                        Eyes[i].Container.Position = newCenter + Configs[i].Offset * sizeRatio;
                        Eyes[i].Container.Scale = Vector2.One * Configs[i].Scale * sizeRatio;
                    }
                    _sizeCheckFrames = 0;
                }
            }
        }

        float dt = (float)delta;
        if (dt <= 0f) return;

        var currentGlobalPos = Relic.GlobalPosition;

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
            if (!GodotObject.IsInstanceValid(eye.Container)) continue;

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

            eye.IrisWrapper.Position = eye.IrisOffset;
        }
    }
}
 
// ─────────────────────────────────────────────────────────────────────
// Inspect screen patches
// ─────────────────────────────────────────────────────────────────────
 
[HarmonyPatch(typeof(NInspectRelicScreen))]
public static class RelicInspectGooglyEyesPatch
{
    private static Texture2D _eyeTexture;
    private static Texture2D _irisTexture;
    private static Vector2 _eyeSize;
    private static Vector2 _irisSize;
 
    private static void EnsureTextures()
    {
        if (_eyeTexture == null)
        {
            _eyeTexture = ResourceLoader.Load<Texture2D>("res://GooglyEyes/googly_eye.png");
            if (_eyeTexture != null) _eyeSize = _eyeTexture.GetSize();
        }
        if (_irisTexture == null)
        {
            _irisTexture = ResourceLoader.Load<Texture2D>("res://GooglyEyes/googly_iris.png");
            if (_irisTexture != null) _irisSize = _irisTexture.GetSize();
        }
    }
 
    private static void CleanupRelicImage(TextureRect relicImage)
    {
        if (relicImage == null) return;
 
        for (int i = relicImage.GetChildCount() - 1; i >= 0; i--)
        {
            var child = relicImage.GetChild(i);
            var name = child.Name.ToString();
            if (child is RelicInspectEyeDriver || name.Contains("Googly"))
            {
                relicImage.RemoveChild(child);
                child.QueueFree();
            }
        }
    }
 
    [HarmonyPatch("Open")]
    [HarmonyPrefix]
    static void Open_Prefix(NInspectRelicScreen __instance)
    {
        try
        {
            var relicImage = __instance.GetNode<TextureRect>("%RelicImage");
            CleanupRelicImage(relicImage);
        }
        catch (Exception e)
        {
            GD.PrintErr("[GooglyEyes] Open cleanup error: " + e);
        }
    }
 
    [HarmonyPatch("UpdateRelicDisplay")]
    [HarmonyPostfix]
    static void UpdateRelicDisplay_Postfix(NInspectRelicScreen __instance)
    {
        try
        {
            var relicImage = __instance.GetNode<TextureRect>("%RelicImage");
            if (relicImage == null) return;
 
            CleanupRelicImage(relicImage);
 
            var traverse = Traverse.Create(__instance);
            var relics = traverse.Field<IReadOnlyList<RelicModel>>("_relics").Value;
            var index = traverse.Field<int>("_index").Value;
 
            if (relics == null || index < 0 || index >= relics.Count) return;
 
            var relic = relics[index];
            var relicId = relic.Id.Entry;
            if (!RelicGooglyEyesRegistry.Configs.TryGetValue(relicId, out var configs)) return;
            if (configs.Length == 0) return;
            if (!SaveManager.Instance.IsRelicSeen(relic)) return;
 
            EnsureTextures();
            if (_eyeTexture == null || _irisTexture == null) return;
 
            const float ReferenceIconSize = 60f;
            Vector2 renderedSize = relicImage.Size;
            float sizeRatio = renderedSize.X / ReferenceIconSize;
            Vector2 center = renderedSize / 2f;
 
            float maxRadius = (_eyeSize.X / 2f) - (_irisSize.X / 2f);
            if (maxRadius < 1f) maxRadius = 1f;
 
            var driver = new RelicInspectEyeDriver
            {
                Name = "GooglyEyeDriver",
                Target = relicImage
            };
 
            int eyeIndex = 0;
            foreach (var config in configs)
            {
                var container = new Control
                {
                    Name = $"GooglyEye_{eyeIndex}",
                    Size = Vector2.Zero,
                    Position = center + config.Offset * sizeRatio,
                    Scale = Vector2.One * config.Scale * sizeRatio,
                    Modulate = new Color(1f, 1f, 1f, config.Opacity),
                    MouseFilter = Control.MouseFilterEnum.Ignore
                };
 
                var eyeRect = new TextureRect
                {
                    Texture = _eyeTexture,
                    Name = "EyeBacking",
                    Size = _eyeSize,
                    Position = -_eyeSize / 2f,
                    MouseFilter = Control.MouseFilterEnum.Ignore
                };
 
                var irisWrapper = new Control
                {
                    Name = "IrisWrapper",
                    Size = Vector2.Zero,
                    Position = Vector2.Zero,
                    MouseFilter = Control.MouseFilterEnum.Ignore
                };
 
                var irisRect = new TextureRect
                {
                    Texture = _irisTexture,
                    Name = "Iris",
                    Size = _irisSize,
                    Position = -_irisSize / 2f,
                    MouseFilter = Control.MouseFilterEnum.Ignore
                };
 
                irisWrapper.AddChild(irisRect);
                container.AddChild(eyeRect);
                container.AddChild(irisWrapper);
                relicImage.AddChild(container);
 
                driver.Eyes.Add(new RelicInspectEyeDriver.EyeData
                {
                    Container = container,
                    IrisWrapper = irisWrapper,
                    IrisOffset = Vector2.Down * maxRadius,
                    IrisVelocity = Vector2.Zero,
                    MaxRadius = maxRadius
                });
 
                eyeIndex++;
            }
 
            relicImage.AddChild(driver);
        }
        catch (Exception e)
        {
            GD.PrintErr("[GooglyEyes] Relic inspect patch error: " + e);
        }
    }
}
 
/// <summary>
/// Iris physics driver for the relic inspect screen.
/// Separate from RelicEyeDriver because it tracks a TextureRect, not an NRelic.
/// </summary>
public partial class RelicInspectEyeDriver : Node
{
    public TextureRect Target;
    public List<EyeData> Eyes = new();
    public Vector2 PrevGlobalPos;
    public bool Initialized;
 
    private const float Gravity = 300f;
    private const float Damping = 0.99f;
    private const float Bounciness = 0.9f;
    private const float MoveForceMultiplier = 3f;
 
    public class EyeData
    {
        public Control Container;
        public Control IrisWrapper;
        public Vector2 IrisOffset;
        public Vector2 IrisVelocity;
        public float MaxRadius;
    }
 
    public override void _Process(double delta)
    {
        if (!GodotObject.IsInstanceValid(Target))
        {
            CleanupEyes();
            QueueFree();
            return;
        }
 
        float dt = (float)delta;
        if (dt <= 0f) return;
 
        var currentGlobalPos = Target.GlobalPosition;
 
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
            if (!GodotObject.IsInstanceValid(eye.Container)) continue;
 
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
 
            eye.IrisWrapper.Position = eye.IrisOffset;
        }
    }
 
    private void CleanupEyes()
    {
        foreach (var eye in Eyes)
        {
            if (GodotObject.IsInstanceValid(eye.Container))
            {
                eye.Container.GetParent()?.RemoveChild(eye.Container);
                eye.Container.Free();
            }
        }
        Eyes.Clear();
    }
}
 