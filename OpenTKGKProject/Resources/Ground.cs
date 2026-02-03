using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using ObjectOrientedOpenGL.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTKGKProject.Resources;

public struct GroundVertex(Vector3 position)
{
    public Vector3 Position = position;
}

public sealed class Ground : IDisposable
{
    private readonly VertexBuffer _vertexBuffer;
    private readonly IndexBuffer _indexBuffer;
    private readonly Mesh _mesh;
    private readonly Vector3 _groundColor;
    
    public Ground(Vector3 groundColor)
    {
        var vertices = new GroundVertex[] 
        {
            new(new Vector3(-1, -1, 0)), // left down
            new(new Vector3(1, 1, 0)), // right up
            new(new Vector3(-1, 1, 0)), // left up
            new(new Vector3(1, -1, 0)) // right down
        };

        _vertexBuffer = new VertexBuffer(vertices,
            vertices.Length * Marshal.SizeOf<GroundVertex>(),
            vertices.Length,
            BufferUsageHint.StaticDraw,
            new VertexBuffer.Attribute(0,
                3));

        var indices = new int[]
        {
            0, 2, 3,
            1, 2, 3
        };
        
        _indexBuffer = new IndexBuffer(indices,
            indices.Length * sizeof(int),
            DrawElementsType.UnsignedInt,
            indices.Length);
        
        _mesh = new Mesh("Ground", PrimitiveType.Triangles, _indexBuffer, _vertexBuffer);
        _groundColor = groundColor;
    }
    
    public void Dispose()
    {
        _vertexBuffer.Dispose();
        _mesh.Dispose();
        _indexBuffer.Dispose();
    }
}