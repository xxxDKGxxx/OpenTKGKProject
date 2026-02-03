using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using ObjectOrientedOpenGL.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTKGKProject.Resources;

public sealed class GBuffer : IBindable, IDisposable
{
    private struct ScreenVertex(Vector2 position, Vector2 texCoord)
    {
        public Vector2 APos = position;
        public Vector2 ATexCoord = texCoord;
    }
    
    private readonly Framebuffer _framebuffer;
    private readonly Texture _positionColorBuffer;
    private readonly Texture _normalColorBuffer;
    private readonly Texture _colorAndSpecularColorBuffer;
    private readonly Renderbuffer _depthBuffer;
    private readonly Mesh _screenMesh;

    public GBuffer(int width, int height)
    {
        _framebuffer = new Framebuffer();

        _positionColorBuffer = new Texture();
        _positionColorBuffer.Allocate(width, height, SizedInternalFormat.Rgba16f);
        
        _normalColorBuffer = new Texture();
        _normalColorBuffer.Allocate(width, height, SizedInternalFormat.Rgba16f);
        
        _colorAndSpecularColorBuffer = new Texture();
        _colorAndSpecularColorBuffer.Allocate(width, height, SizedInternalFormat.Rgba16f);
        
        _framebuffer.AttachTexture(FramebufferAttachment.ColorAttachment0, _positionColorBuffer);
        _framebuffer.AttachTexture(FramebufferAttachment.ColorAttachment1, _normalColorBuffer);
        _framebuffer.AttachTexture(FramebufferAttachment.ColorAttachment2, _colorAndSpecularColorBuffer);
        
        _depthBuffer = new Renderbuffer();
        _depthBuffer.Allocate(width, height, RenderbufferStorage.DepthComponent24);
        
        _framebuffer.AttachRenderBuffer(FramebufferAttachment.DepthAttachment, _depthBuffer);
        
        GL.NamedFramebufferDrawBuffers(_framebuffer.Handle,
            3,
            [
                DrawBuffersEnum.ColorAttachment0,
                DrawBuffersEnum.ColorAttachment1,
                DrawBuffersEnum.ColorAttachment2
            ]);

        var screenVerticies = new[]
        {
            new ScreenVertex(new Vector2(-1, -1), new Vector2(0, 0)),
            new ScreenVertex(new Vector2(-1, 1), new Vector2(0, 1)),
            new ScreenVertex(new Vector2(1, 1), new Vector2(1, 1)),
            new ScreenVertex(new Vector2(1, -1), new Vector2(1, 0))
        };
        
        var screenVertexBuffer = new VertexBuffer(
        screenVerticies,
            Marshal.SizeOf<ScreenVertex>(),
            screenVerticies.Length,
            BufferUsageHint.StaticDraw,
            new VertexBuffer.Attribute(0,2),
            new VertexBuffer.Attribute(1, 2));
    }

    public void Bind()
    {
        _framebuffer.Bind();
    }

    public void BindTextures()
    {
        _positionColorBuffer.ActivateUnit();
        _normalColorBuffer.ActivateUnit(1);
        _colorAndSpecularColorBuffer.ActivateUnit(2);
    }

    public void Draw(Shader shader)
    {
        BindTextures();
        
        shader.LoadInteger("gPosition", 0);
        shader.LoadInteger("gNormal", 1);
        shader.LoadInteger("gColor", 2);
        
        
    }

    public void Unbind()
    {
        _framebuffer.Unbind();
    }

    public void Dispose()
    {
        _framebuffer.Dispose();
        _positionColorBuffer.Dispose();
        _normalColorBuffer.Dispose();
        _colorAndSpecularColorBuffer.Dispose();
        _depthBuffer.Dispose();
    }
}