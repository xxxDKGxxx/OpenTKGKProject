namespace OpenTKGKProject.Resources.Lights;

public class DirectionalLight(Vector3 color, Vector3 direction) : IShaderLight
{
    private Light _light = new()
    {
        Type = LightType.Directional,
        Color = color,
        Direction = direction,
    };

    public Light GetShaderLightData()
    {
        return _light;
    }

    public void SetShaderLightSpaceMatrix(Matrix4 matrix)
    {
        _light.LightSpaceMatrix = matrix;
    }

    public void SetShaderLightShaderMapIndex(int index)
    {
        _light.ShadowMapLayerIndex = index;
    }
}