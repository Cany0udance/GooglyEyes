using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

namespace GooglyEyes;

[HarmonyPatch(typeof(NSingleplayerSubmenu), "_Ready")]
public class GooglyEyesEditorButtonPatch
{
    private static NSubmenuButton _editorButton;

    public static void Postfix(NSingleplayerSubmenu __instance)
    {
        try
        {
            var customButtonField = typeof(NSingleplayerSubmenu).GetField("_customButton",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var customButton = (NSubmenuButton)customButtonField.GetValue(__instance);

            _editorButton = (NSubmenuButton)customButton.Duplicate();
            _editorButton.Name = "GooglyEyesEditorButton";
            __instance.AddChild(_editorButton);

            _editorButton.Position = new Vector2(
                customButton.Position.X,
                ((Control)customButton).Position.Y + ((Control)customButton).Size.Y + 20f
            );

            var bgPanel = _editorButton.GetNode<Control>((NodePath)"BgPanel");
            bgPanel.Material = (ShaderMaterial)bgPanel.Material.Duplicate();

            var hsvField = typeof(NSubmenuButton).GetField("_hsv",
                BindingFlags.NonPublic | BindingFlags.Instance);
            hsvField?.SetValue(_editorButton, bgPanel.Material);

            var hsvMaterial = (ShaderMaterial)bgPanel.Material;
            hsvMaterial.SetShaderParameter("h", (Variant)0.3f);

            // Skip SetIconAndLocalization — just set the label directly
            var label = _editorButton.GetNode<Label>((NodePath)"Label");
            if (label != null)
                label.Text = "Googly Eyes";

            _editorButton.Connect(
                NClickableControl.SignalName.Released,
                Callable.From(new Action<NButton>(_ => OpenEditor(__instance)))
            );
        }
        catch (Exception e)
        {
            GD.PrintErr("[GooglyEyes] Failed to add editor button: " + e.ToString());
        }
    }

    private static void OpenEditor(NSingleplayerSubmenu instance)
    {
        GD.Print("[GooglyEyes] Opening editor");

        var stackField = typeof(NSubmenu).GetField("_stack",
            BindingFlags.NonPublic | BindingFlags.Instance);
        var stack = stackField?.GetValue(instance);

        if (stack != null)
        {
            var editorScreen = new NGooglyEyesEditorScreen();
            ((Control)stack).AddChild(editorScreen);

            var pushMethod = stack.GetType().GetMethod("Push");
            pushMethod?.Invoke(stack, new object[] { editorScreen });
        }
    }

    public static NSubmenuButton GetEditorButton() => _editorButton;
}

[HarmonyPatch(typeof(NSingleplayerSubmenu), "RefreshButtons")]
public class GooglyEyesRefreshButtonsPatch
{
    public static void Postfix()
    {
        GooglyEyesEditorButtonPatch.GetEditorButton()?.SetEnabled(true);
    }
}