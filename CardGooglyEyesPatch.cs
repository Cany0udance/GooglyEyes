using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace GooglyEyes;

[HarmonyPatch(typeof(NCard))]
public static class CardGooglyEyesPatch
{
    private static readonly Vector2 CardCenter = NCard.defaultSize / 2f;

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
    static void Reload_Postfix(NCard __instance)
    {
        if (__instance.HasMeta("googly_editor_preview")) return;
        try
        {
            RemoveDriver(__instance);

            var model = __instance.Model;
            if (model == null) return;

            var cardId = model.Id.Entry;
            if (!CardGooglyEyesRegistry.Configs.TryGetValue(cardId, out var configs)) return;
            if (configs.Length == 0) return;

            var body = __instance.Body;
            if (body == null) return;

            EnsureTextures();
            if (_eyeTexture == null || _irisTexture == null) return;

            float maxRadius = (_eyeSize.X / 2f) - (_irisSize.X / 2f);
            if (maxRadius < 1f) maxRadius = 1f;

            var driver = new CardEyeDriver
            {
                Name = "GooglyEyeDriver",
                Card = __instance
            };

            foreach (var config in configs)
            {
                // Outer container — positioned at the eye center, scaled
                var container = new Control
                {
                    Name = "GooglyEye",
                    Size = Vector2.Zero,
                    Position = CardCenter + config.Offset,
                    Scale = Vector2.One * config.Scale,
                    Modulate = new Color(1f, 1f, 1f, config.Opacity),
                    MouseFilter = Control.MouseFilterEnum.Ignore
                };
                // Eye backing — centered on the container's position
                var eyeRect = new TextureRect
                {
                    Texture = _eyeTexture,
                    Name = "EyeBacking",
                    Size = _eyeSize,
                    Position = -_eyeSize / 2f,
                    UseParentMaterial = true,
                    MouseFilter = Control.MouseFilterEnum.Ignore
                };

                // Iris wrapper — a zero-size Control we can reposition freely.
                // The TextureRect inside is offset so it's centered on the wrapper's origin.
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
                body.AddChild(container);

                driver.Eyes.Add(new CardEyeDriver.EyeData
                {
                    Container = container,
                    IrisWrapper = irisWrapper,
                    IrisOffset = Vector2.Zero,
                    IrisVelocity = Vector2.Zero,
                    MaxRadius = maxRadius
                });
            }

            body.AddChild(driver);
        }
        catch (Exception e)
        {
            GD.PrintErr("[GooglyEyes] Card Reload error: " + e);
        }
    }

    [HarmonyPatch("OnFreedToPool")]
    [HarmonyPostfix]
    static void OnFreedToPool_Postfix(NCard __instance)
    {
        RemoveDriver(__instance);
    }

    private static void RemoveDriver(NCard card)
    {
        var body = card.Body;
        if (body == null) return;

        for (int i = body.GetChildCount() - 1; i >= 0; i--)
        {
            var child = body.GetChild(i);
            var name = child.Name.ToString();
            if (child is CardEyeDriver || name.Contains("Googly") || name.StartsWith("@Control@"))
            {
                body.RemoveChild(child);
                child.Free();
            }
        }
    }
}

public partial class CardEyeDriver : Node
{
    public NCard Card;
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
        if (!GodotObject.IsInstanceValid(Card)) { QueueFree(); return; }

        float dt = (float)delta;
        if (dt <= 0f) return;

        var currentGlobalPos = Card.GlobalPosition;

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