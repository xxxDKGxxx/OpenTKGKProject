using System.Runtime.InteropServices;
using ObjectOrientedOpenGL.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTKGKProject.Resources.Lights;

public sealed class LightCubeModel : IDisposable
{
    private readonly Mesh _mesh;
    
    public LightCubeModel()
    {
        // 8 narożników sześcianu (Kolor ustawiony na Biały, żeby mnożyć go przez kolor światła w shaderze)
        var vertices = new LightSourceVertex[]
        {
            // Przednia ściana (Z = 0.5)
            new(new Vector3(-0.5f, -0.5f,  0.5f)), // 0: Lewy-Dół-Przód
            new(new Vector3( 0.5f, -0.5f,  0.5f)), // 1: Prawy-Dół-Przód
            new(new Vector3( 0.5f,  0.5f,  0.5f)), // 2: Prawy-Góra-Przód
            new(new Vector3(-0.5f,  0.5f,  0.5f)), // 3: Lewy-Góra-Przód

            // Tylna ściana (Z = -0.5)
            new(new Vector3(-0.5f, -0.5f, -0.5f)), // 4: Lewy-Dół-Tył
            new(new Vector3( 0.5f, -0.5f, -0.5f)), // 5: Prawy-Dół-Tył
            new(new Vector3( 0.5f,  0.5f, -0.5f)), // 6: Prawy-Góra-Tył
            new(new Vector3(-0.5f,  0.5f, -0.5f))  // 7: Lewy-Góra-Tył
        };

        // 36 indeksów tworzących 12 trójkątów (kolejność CCW - przeciwna do zegara)
        var indices = new uint[]
        {
            // Przód
            0, 1, 2,
            2, 3, 0,

            // Prawo
            1, 5, 6,
            6, 2, 1,

            // Tył
            7, 6, 5,
            5, 4, 7,

            // Lewo
            4, 0, 3,
            3, 7, 4,

            // Góra
            3, 2, 6,
            6, 7, 3,

            // Dół
            4, 5, 1,
            1, 0, 4
        };

        var vertexBuffer = new VertexBuffer(
            vertices,
            vertices.Length * Marshal.SizeOf<LightSourceVertex>(),
            vertices.Length,
            BufferUsageHint.StaticDraw,
            new VertexBuffer.Attribute(0, 3));
        
        var indexBuffer = new IndexBuffer(
            indices, 
            indices.Length * Marshal.SizeOf<uint>(), 
            DrawElementsType.UnsignedInt, 
            indices.Length);

        _mesh = new Mesh("lightcube", PrimitiveType.Triangles, indexBuffer, vertexBuffer);
    }

    public void Bind()
    {
        _mesh.Bind();
    }

    public void Unbind()
    {
        _mesh.Unbind();
    }

    public void Render()
    {
        _mesh.RenderIndexed();
    }

    public void Dispose()
    {
        _mesh.Dispose();
    }
}