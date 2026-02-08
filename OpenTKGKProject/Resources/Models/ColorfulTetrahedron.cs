using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace OpenTKGKProject.Resources.Models;

public sealed class ColorfulTetrahedron : IDisposable, IModel
{
    private readonly VertexBuffer _vertexBuffer;
    private readonly Mesh _mesh;

    public Matrix4 ModelMatrix { get; set; }

    public ColorfulTetrahedron(Vector3 position)
    {
        const float s = 0.5f;

        var p0 = new Vector3(s, s, s);
        var p1 = new Vector3(s, -s, -s);
        var p2 = new Vector3(-s, s, -s);
        var p3 = new Vector3(-s, -s, s);

        var color1 = new Vector3(1.0f, 0.0f, 0.0f);
        var color2 = new Vector3(0.0f, 1.0f, 0.0f);
        var color3 = new Vector3(0.0f, 0.0f, 1.0f);
        var color4 = new Vector3(1.0f, 1.0f, 0.0f);

        var n1 = CalcNormal(p0, p3, p1);
        var n2 = CalcNormal(p0, p2, p3);
        var n3 = CalcNormal(p0, p1, p2);
        var n4 = CalcNormal(p1, p3, p2);

        Vertex[] vertices =
        [
            // Face 1 (p0, p3, p1)
            new(p0, color1, n1),
            new(p3, color1, n1),
            new(p1, color1, n1),

            // Face 2 (p0, p2, p3)
            new(p0, color2, n2),
            new(p2, color2, n2),
            new(p3, color2, n2),

            // Face 3 (p0, p1, p2)
            new(p0, color3, n3),
            new(p1, color3, n3),
            new(p2, color3, n3),

            // Face 4 (p1, p3, p2) - Podstawa (zależy od orientacji)
            new(p1, color4, n4),
            new(p3, color4, n4),
            new(p2, color4, n4)
        ];

        _vertexBuffer = new VertexBuffer(vertices, vertices.Length * Marshal.SizeOf<Vertex>(),
            vertices.Length, BufferUsageHint.StaticDraw,
            new VertexBuffer.Attribute(0, 3), // positions
            new VertexBuffer.Attribute(1, 3), // color
            new VertexBuffer.Attribute(2, 3)  // normal
        );

        _mesh = new Mesh("ColorfulTetrahedron", PrimitiveType.Triangles, null, _vertexBuffer);

        ModelMatrix = Matrix4.CreateTranslation(position);
    }

    public void Render(Shader shader)
    {
        shader.Use();
        shader.LoadMatrix4("model", ModelMatrix);

        _mesh.Bind();
        _mesh.Render(0, _vertexBuffer.Count);
        _mesh.Unbind();
    }

    public void Dispose()
    {
        _vertexBuffer.Dispose();
        _mesh.Dispose();
    }

    private static Vector3 CalcNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        var dir = Vector3.Cross(b - a, c - a);
        return Vector3.Normalize(dir);
    }
}