namespace OpenTKGKProject.Resources.Lights;

public class PointLight(
    Vector3 color,
    Vector3 position,
    float constant = 1.0f,
    float linear = 0.09f,
    float quadratic = 0.032f)
    : IShaderLight
{
    private readonly Light _light = new()
    {
        Type = LightType.Point,
        Position = position,
        Color = color,
        Constant = constant,
        Linear = linear,
        Quadratic = quadratic
    };

    public Light GetShaderLightData()
    {
        return _light;
    }
}