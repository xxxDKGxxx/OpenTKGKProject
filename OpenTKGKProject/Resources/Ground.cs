using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace OpenTKGKProject.Resources;

public class Ground : IModel
{
    private readonly Mesh _mesh;

    public Ground(float scale, Vector3 groundColor)
    {
        var vertices = new Vertex[]
        {
            new(new Vector3(-1, 0, -1), groundColor, new Vector3(0.0f, 1.0f, 0.0f)),
            new(new Vector3(-1, 0, 1), groundColor, new Vector3(0.0f, 1.0f, 0.0f)),
            new(new Vector3(1, 0, -1), groundColor, new Vector3(0.0f, 1.0f, 0.0f)),
            new(new Vector3(1, 0, 1), groundColor, new Vector3(0.0f, 1.0f, 0.0f)),
        };

        var indices = new[]
        {
            0u, 1u, 2u,
            1u, 3u, 2u
        };

        var vertexBuffer = new VertexBuffer(
            vertices,
            vertices.Length * Marshal.SizeOf<Vertex>(),
            vertices.Length,
            BufferUsageHint.StaticDraw,
            new VertexBuffer.Attribute(0, 3), // pos
            new VertexBuffer.Attribute(1, 3), // color
            new VertexBuffer.Attribute(2, 3)); // norm

        var indexBuffer = new IndexBuffer(
            indices,
            indices.Length * Marshal.SizeOf<uint>(),
            DrawElementsType.UnsignedInt,
            indices.Length);

        _mesh = new Mesh("ground", PrimitiveType.Triangles, indexBuffer, vertexBuffer);

        ModelMatrix = Matrix4.CreateScale(scale);
    }

    public void Render(Shader shader)
    {
        shader.Use();
        shader.LoadMatrix4("model", ModelMatrix);

        _mesh.Bind();
        _mesh.RenderIndexed();
        _mesh.Unbind();
    }

    public Matrix4 ModelMatrix { get; set; }
}