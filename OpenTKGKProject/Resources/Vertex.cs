using OpenTK.Mathematics;

namespace OpenTKGKProject.Resources;

public struct Vertex(Vector3 position, Vector3 color, Vector3 normal)
{
    public Vector3 Position = position;
    public Vector3 Color = color;
    public Vector3 Normal = normal;
}