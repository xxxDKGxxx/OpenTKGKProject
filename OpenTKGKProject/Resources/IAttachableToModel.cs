using OpenTK.Mathematics;

namespace OpenTKGKProject.Resources;

public interface IAttachableToModel<out T>
{
    public T AttachedTo(IModel model, Vector3 offset);
}