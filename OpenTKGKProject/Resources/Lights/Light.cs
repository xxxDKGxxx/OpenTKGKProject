using OpenTK.Mathematics;

namespace OpenTKGKProject.Resources.Lights;

public enum LightType
{
    Point = 0,
    Spotlight = 1,
    Directional = 2,
}

public struct Light {
    public LightType Type; // 0 point, 1 reflector, 2 directional
    public Vector3 Position;
    public Vector3 Direction;
    public Vector3 Color;
    
    // attenuation
    public float Constant;
    public float Linear;
    public float Quadratic;
    
    // reflector
    public float CutOff;
    public float OuterCutOff;
};