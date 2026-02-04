using OpenTK.Mathematics;

namespace OpenTKGKProject.Resources.Lights;

public class Spotlight(
    Vector3 color,
    Vector3 position,
    Vector3 direction,
    float cutOff,
    float outerCutOff,
    float constant = 1.0f,
    float linear = 0.045f,
    float quadratic = 0.0075f)
    : IShaderLight, IAttachableToModel<Spotlight>
{
    private IModel? _attachedModel = null;
    private Vector3? _attachedOffset = null!;
    
    private Light _light = new()
    {
        Type = LightType.Spotlight,
        Position = position,
        Color = color,
        Direction = direction,
        CutOff = cutOff,
        OuterCutOff = outerCutOff,
        Constant = constant,
        Linear = linear,
        Quadratic = quadratic
    };

    public Light GetShaderLightData()
    {
        var resultLight = _light;

        if (_attachedModel is null || !_attachedOffset.HasValue) return resultLight;
        
        var localPos = new Vector4(_attachedOffset.Value, 1.0f);
        var worldPos = localPos * _attachedModel.ModelMatrix;
        
        resultLight.Position = worldPos.Xyz;
            
        var localDir = new Vector4(_light.Direction, 0.0f); 
        var worldDir = localDir * _attachedModel.ModelMatrix;

        resultLight.Direction = worldDir.Xyz.Normalized();

        return resultLight;
    }

    public void SetDirection(Vector3 dir)
    {
        _light.Direction = dir.Normalized();
    }

    public void SetCutoff(float value)
    {
        _light.CutOff = value;
    }

    public void SetOuterCutOff(float value)
    {
        _light.OuterCutOff = value;
    }

    public Spotlight AttachedTo(IModel model, Vector3 offset)
    {
        _attachedModel = model;
        _attachedOffset = offset;
        
        return this;
    }
}