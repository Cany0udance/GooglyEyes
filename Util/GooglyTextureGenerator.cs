using Godot;

namespace GooglyEyes.Util;

public static class GooglyTextureGenerator
{
    public const int EyeSize = 205;
    public const int IrisSize = 105;

    private static Texture2D _placeholderEye;
    private static Texture2D _placeholderIris;
    private static ShaderMaterial _eyeMaterial;
    private static ShaderMaterial _irisMaterial;

    private static readonly Shader EyeShader = new()
    {
        Code = @"
            shader_type canvas_item;
            uniform float outline_ratio : hint_range(0.0, 0.2) = 0.029;
            uniform vec4 outline_color : source_color = vec4(0.15, 0.15, 0.15, 1.0);
void fragment() {
    vec2 uv = UV * 2.0 - 1.0;
    float dist = length(uv);
    float aa = fwidth(dist);
    float alpha = smoothstep(1.0, 1.0 - aa, dist);
    float inner = 1.0 - outline_ratio;
    float outline_mix = smoothstep(inner - aa, inner, dist);
    float outline_fade = smoothstep(1.0 - aa, inner, dist);
    vec4 circle = mix(vec4(1.0, 1.0, 1.0, 1.0), outline_color, outline_mix);
    circle.a *= alpha;
circle.a *= mix(1.0, outline_fade * 0.5 + 0.5, outline_mix);
    COLOR = circle * COLOR;
}"
    };

    private static readonly Shader IrisShader = new()
    {
        Code = @"
            shader_type canvas_item;
void fragment() {
    vec2 uv = UV * 2.0 - 1.0;
    float dist = length(uv);
    float aa = fwidth(dist);
    COLOR = vec4(0.0, 0.0, 0.0, smoothstep(1.0, 1.0 - aa, dist)) * COLOR;
}"
    };

    public static Texture2D EyeTexture => _placeholderEye ??= CreatePlaceholder(EyeSize);
    public static Texture2D IrisTexture => _placeholderIris ??= CreatePlaceholder(IrisSize);

    public static ShaderMaterial GetEyeMaterial()
    {
        _eyeMaterial ??= new ShaderMaterial { Shader = EyeShader };
        return (ShaderMaterial)_eyeMaterial.Duplicate();
    }

    public static ShaderMaterial GetIrisMaterial()
    {
        _irisMaterial ??= new ShaderMaterial { Shader = IrisShader };
        return (ShaderMaterial)_irisMaterial.Duplicate();
    }

    private static Texture2D CreatePlaceholder(int size)
    {
        var image = Image.CreateEmpty(size, size, false, Image.Format.Rgba8);
        image.Fill(Colors.White);
        return ImageTexture.CreateFromImage(image);
    }
}