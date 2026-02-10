using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace OpenTKGKProject.Resources;

public enum RenderMode
{
    Full = 0,
    Depth = 1,
    Normals = 2,
    Color = 3
}

public sealed class GBuffer : IBindable, IDisposable
{
    public RenderMode RenderMode = RenderMode.Full;

    public const int DepthUnit = 0;
    public const int NormalUnit = 1;
    public const int ColorUnit = 2;

    private struct ScreenVertex(Vector2 position, Vector2 texCoord)
    {
        public Vector2 APos = position;
        public Vector2 ATexCoord = texCoord;
    }

    private Framebuffer _framebuffer;
    private Texture _depthColorBuffer;
    private Texture _normalColorBuffer;
    private Texture _colorColorBuffer;
    private readonly Mesh _screenMesh;

    private readonly ScreenVertex[] _screenVerticies =
    [
        new ScreenVertex(new Vector2(-1, -1), new Vector2(0, 0)),
        new ScreenVertex(new Vector2(-1, 1), new Vector2(0, 1)),
        new ScreenVertex(new Vector2(1, 1), new Vector2(1, 1)),
        new ScreenVertex(new Vector2(1, -1), new Vector2(1, 0))
    ];

    private readonly uint[] _screenIndicies =
    [
        0u, 1u, 2u,
        0u, 2u, 3u
    ];

    private VertexBuffer ScreenVertexBuffer { get; set; }
    private IndexBuffer ScreenIndexBuffer { get; set; }


    public GBuffer(int width, int height)
    {
        InitFramebuffer(width, height);

        ScreenVertexBuffer = new VertexBuffer(
            _screenVerticies,
            _screenVerticies.Length * Marshal.SizeOf<ScreenVertex>(),
            _screenVerticies.Length,
            BufferUsageHint.StaticDraw,
            new VertexBuffer.Attribute(0, 2),
            new VertexBuffer.Attribute(1, 2));

        ScreenIndexBuffer = new IndexBuffer(
            _screenIndicies,
            _screenIndicies.Length * Marshal.SizeOf<uint>(),
            DrawElementsType.UnsignedInt,
            _screenIndicies.Length);

        _screenMesh = new Mesh("screen", PrimitiveType.Triangles, ScreenIndexBuffer, ScreenVertexBuffer);
    }
    
    private void InitFramebuffer(int width, int height)
    {
        _framebuffer = new Framebuffer();
        
        var options = new Texture.Options(
            new Texture.EnumParameter(TextureParameterName.TextureMinFilter, TextureMinFilter.Nearest),
            new Texture.EnumParameter(TextureParameterName.TextureMagFilter, TextureMagFilter.Nearest));
        
        _depthColorBuffer = new Texture();
        _depthColorBuffer.Allocate(width, height, SizedInternalFormat.DepthComponent32);
        _depthColorBuffer.ApplyOptions(options);
        
        _normalColorBuffer = new Texture();
        _normalColorBuffer.Allocate(width, height, SizedInternalFormat.Rgba16f);
        _normalColorBuffer.ApplyOptions(options);
        
        _colorColorBuffer = new Texture();
        _colorColorBuffer.Allocate(width, height, SizedInternalFormat.Rgba16f);
        _colorColorBuffer.ApplyOptions(options);

        _framebuffer.AttachTexture(FramebufferAttachment.DepthAttachment, _depthColorBuffer);
        _framebuffer.AttachTexture(FramebufferAttachment.ColorAttachment0, _normalColorBuffer);
        _framebuffer.AttachTexture(FramebufferAttachment.ColorAttachment1, _colorColorBuffer);

        GL.NamedFramebufferDrawBuffers(
            _framebuffer.Handle,
            2,
            [
                DrawBuffersEnum.ColorAttachment0,
                DrawBuffersEnum.ColorAttachment1]);
    }

    public void Resize(int width, int height)
    {
        DisposeFramebuffer();
        InitFramebuffer(width, height);
    }

    public void CopyDepthToScreen(int width, int height)
    {
        _framebuffer.BindRead();

        GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0); // Piszemy na Ekran (0)

        GL.BlitFramebuffer(
            0, 0, width, height, // Źródło (Cały FBO)
            0, 0, width, height, // Cel (Całe okno)
            ClearBufferMask.DepthBufferBit, // Kopiujemy TYLKO głębię
            BlitFramebufferFilter.Nearest // Dla głębi musi być Nearest
        );

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0); // Wracamy do domyślnego
    }

    public void Bind()
    {
        _framebuffer.Bind();
    }

    public void BindTextures()
    {
        _depthColorBuffer.ActivateUnit(DepthUnit);
        _normalColorBuffer.ActivateUnit(NormalUnit);
        _colorColorBuffer.ActivateUnit(ColorUnit);
    }

    public void Draw(Shader shader)
    {
        BindTextures();

        shader.Use();
        
        shader.LoadInteger("renderMode", (int)RenderMode);
        shader.LoadInteger("gDepth", DepthUnit);
        shader.LoadInteger("gNormal", NormalUnit);
        shader.LoadInteger("gColor", ColorUnit);
        
        _screenMesh.Bind();
        _screenMesh.RenderIndexed();
        _screenMesh.Unbind();
    }

    public void Unbind()
    {
        _framebuffer.Unbind();
    }

    public void Dispose()
    {
        DisposeFramebuffer();
        _screenMesh.Dispose();
    }

    private void DisposeFramebuffer()
    {
        _framebuffer.Dispose();
        _depthColorBuffer.Dispose();
        _normalColorBuffer.Dispose();
        _colorColorBuffer.Dispose();
    }
}