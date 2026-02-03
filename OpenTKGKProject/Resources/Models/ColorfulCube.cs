using System.Runtime.InteropServices;
using ObjectOrientedOpenGL.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTKGKProject.Resources.Models;

public sealed class ColorfulCube : IDisposable, IModel
{
    private readonly VertexBuffer _vertexBuffer;
    private readonly Mesh _mesh;
    
    public Matrix4 Transform = Matrix4.Identity;

    public ColorfulCube(Vector3 position)
    {
        Vertex[] vertices =
        [
            // bottom (z: -0.5, normal: 0, 0, -1)
            new(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, -1.0f)),
            new(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, -1.0f)),
            new(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, -1.0f)),
            new(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, -1.0f)),
            new(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, -1.0f)),
            new(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, -1.0f)),

            // top (z: 0.5, normal: 0, 0, 1)
            new(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f)),
            new(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f)),
            new(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f)),
            new(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f)),
            new(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f)),
            new(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f)),

            // left (x: -0.5, normal: -1, 0, 0)
            new(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(-1.0f, 0.0f, 0.0f)),
            new(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(-1.0f, 0.0f, 0.0f)),
            new(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(-1.0f, 0.0f, 0.0f)),
            new(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(-1.0f, 0.0f, 0.0f)),
            new(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(-1.0f, 0.0f, 0.0f)),
            new(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(-1.0f, 0.0f, 0.0f)),

            // right (x: 0.5, normal: 1, 0, 0)
            new(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1.0f, 1.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f)),
            new(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(1.0f, 1.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f)),
            new(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1.0f, 1.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f)),
            new(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1.0f, 1.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f)),
            new(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(1.0f, 1.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f)),
            new(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1.0f, 1.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f)),

            // front (y: -0.5, normal: 0, -1, 0)
            new(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(1.0f, 0.0f, 1.0f), new Vector3(0.0f, -1.0f, 0.0f)),
            new(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1.0f, 0.0f, 1.0f), new Vector3(0.0f, -1.0f, 0.0f)),
            new(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(1.0f, 0.0f, 1.0f), new Vector3(0.0f, -1.0f, 0.0f)),
            new(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(1.0f, 0.0f, 1.0f), new Vector3(0.0f, -1.0f, 0.0f)),
            new(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(1.0f, 0.0f, 1.0f), new Vector3(0.0f, -1.0f, 0.0f)),
            new(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(1.0f, 0.0f, 1.0f), new Vector3(0.0f, -1.0f, 0.0f)),

            // back (y: 0.5, normal: 0, 1, 0)
            new(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0.0f, 1.0f, 1.0f), new Vector3(0.0f, 1.0f, 0.0f)),
            new(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0.0f, 1.0f, 1.0f), new Vector3(0.0f, 1.0f, 0.0f)),
            new(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.0f, 1.0f, 1.0f), new Vector3(0.0f, 1.0f, 0.0f)),
            new(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.0f, 1.0f, 1.0f), new Vector3(0.0f, 1.0f, 0.0f)),
            new(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0.0f, 1.0f, 1.0f), new Vector3(0.0f, 1.0f, 0.0f)),
            new(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0.0f, 1.0f, 1.0f), new Vector3(0.0f, 1.0f, 0.0f))
        ];

        _vertexBuffer = new VertexBuffer(vertices, vertices.Length * Marshal.SizeOf<Vertex>(),
            vertices.Length, BufferUsageHint.StaticDraw,
            new VertexBuffer.Attribute(0, 3), // positions
            new VertexBuffer.Attribute(1, 3), // color
            new VertexBuffer.Attribute(2, 3) // normal
        );

        _mesh = new Mesh("ColorfulCube", PrimitiveType.Triangles, null, _vertexBuffer);
        
        Transform = Matrix4.CreateTranslation(position);
    }
    
    public void Render(Shader shader)
    {
        shader.Use();
        shader.LoadMatrix4("model", Transform);
        
        _mesh.Bind();
        _mesh.Render(0, _vertexBuffer.Count);
        _mesh.Unbind();
    }

    public void Dispose()
    {
        _vertexBuffer.Dispose();
        _mesh.Dispose();
    }

    public Matrix4 ModelMatrix { get => Transform; set => Transform = value; }
}