using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace OpenTKGKProject.Resources.Models.Sphere;

public class Sphere : IDisposable
{
    private readonly VertexBuffer _vertexBuffer;
    private readonly IndexBuffer _indexBuffer;
    private readonly Mesh _mesh;
    private readonly Matrix4 _transform;

    public Sphere(Vector3 position)
    {
        var (sphereVerticies, sphereIndicies) = SphereGenerator.Generate(1, 200, 200);

        _vertexBuffer = new VertexBuffer(sphereVerticies,
            sphereVerticies.Length * Marshal.SizeOf<Resources.Vertex>(), sphereVerticies.Length,
            BufferUsageHint.StaticDraw, new VertexBuffer.Attribute(0, 3), new VertexBuffer.Attribute(1, 3),
            new VertexBuffer.Attribute(2, 3));

        _indexBuffer = new IndexBuffer(sphereIndicies,
            sphereIndicies.Length * sizeof(int),
            DrawElementsType.UnsignedInt,
            sphereIndicies.Length,
            BufferUsageHint.StaticDraw);

        _mesh = new Mesh("Sphere", PrimitiveType.Triangles, _indexBuffer, _vertexBuffer);

        _transform = Matrix4.CreateTranslation(position);
    }

    public void Render(Shader shader)
    {
        shader.Use();
        shader.LoadMatrix4("model", _transform);

        _mesh.Bind();
        _mesh.RenderIndexed(0, _indexBuffer.Count);
        _mesh.Unbind();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _vertexBuffer.Dispose();
        _indexBuffer.Dispose();
        _mesh.Dispose();
    }
}