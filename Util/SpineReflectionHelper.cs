using System.Reflection;

namespace GooglyEyes.Util;

internal static class SpineReflectionHelper
{
    private static MethodInfo _getAnimationMethod;
    private static MethodInfo _getNameMethod;

    public static string GetAnimationName(object trackEntry, string fallback = "idle_loop")
    {
        if (trackEntry == null) return fallback;

        _getAnimationMethod ??= trackEntry.GetType()
            .GetMethod("GetAnimation",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        var animation = _getAnimationMethod?.Invoke(trackEntry, null);
        if (animation == null) return fallback;

        _getNameMethod ??= animation.GetType()
            .GetMethod("GetName",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        return _getNameMethod?.Invoke(animation, null) as string ?? fallback;
    }
}