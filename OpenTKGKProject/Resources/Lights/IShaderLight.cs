namespace OpenTKGKProject.Resources.Lights;

public interface IShaderLight
{
    public Light GetShaderLightData();
    public void SetShaderLightSpaceMatrix(Matrix4 matrix);
    public void SetShaderLightShaderMapIndex(int index);
}