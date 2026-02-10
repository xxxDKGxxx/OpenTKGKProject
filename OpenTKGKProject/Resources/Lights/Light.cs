namespace OpenTKGKProject.Resources.Lights;

public enum LightType
{
    Point = 0,
    Spotlight = 1,
    Directional = 2,
}

public struct Light
{
    public LightType Type; // 0 point, 1 reflector, 2 directional
    public Vector3 Position;
    public Vector3 Direction;
    public Vector3 Color;

    public Matrix4 LightSpaceMatrix;
    public int ShadowMapLayerIndex;

    // attenuation
    public float Constant;
    public float Linear;
    public float Quadratic;

    // reflector
    public float CutOff;
    public float OuterCutOff;

    public float LightRange()
    {
        var threshold = 5.0f / 256.0f; 

        var lightMax = Math.Max(Color.X, Math.Max(Color.Y, Color.Z));

        var constant = Constant;
        var linear = Linear;
        var quadratic = Quadratic;
    
        var cTerm = constant - lightMax / threshold;
        var delta = linear * linear - 4 * quadratic * cTerm;

        if (delta < 0)
        {
            return 1000.0f;
        }
        
        var distance = (-linear + (float)Math.Sqrt(delta)) / (2.0f * quadratic);
    
        return distance;
    }
};