namespace OpenTKGKProject.Resources.Lights;

public class DirectionalLight(Vector3 color, Vector3 direction) : IShaderLight
{
    private readonly Light _light = new()
    {
        Type = LightType.Directional,
        Color = color,
        Direction = direction,
    };

    public Light GetShaderLightData()
    {
        return _light;
    }
}