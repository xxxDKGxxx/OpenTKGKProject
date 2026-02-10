using OpenTK.Graphics.OpenGL4;

namespace OpenTKGKProject.Resources;

public sealed class ShadowBuffer : IBindable, IDisposable
{
    public const int ShadowWidth = 4096;
    public const int ShadowHeight = 4096;
    public const int MaxShadowingLights = 8;

    public const int ShadowUnit = 3;

    private Texture _shadowColorBuffer;
    private Framebuffer _shadowFramebuffer;
    
    public ShadowBuffer()
    {
        InitShadowFramebuffer();
    }
    
    private void InitShadowFramebuffer()
    {
        _shadowFramebuffer = new Framebuffer();
        
        var options = new Texture.Options(
            new Texture.EnumParameter(TextureParameterName.TextureMinFilter, TextureMinFilter.Nearest),
            new Texture.EnumParameter(TextureParameterName.TextureMagFilter, TextureMagFilter.Nearest),
            new Texture.EnumParameter(TextureParameterName.TextureWrapS, TextureWrapMode.ClampToBorder),
            new Texture.EnumParameter(TextureParameterName.TextureWrapT, TextureWrapMode.ClampToBorder),
            new Texture.FloatParameter(TextureParameterName.TextureBorderColor, 1.0f, 1.0f, 1.0f, 1.0f));

        _shadowColorBuffer = new Texture(TextureTarget.Texture2DArray);
        _shadowColorBuffer.Allocate3d(
            ShadowWidth, 
            ShadowHeight, 
            MaxShadowingLights, 
            SizedInternalFormat.DepthComponent32);
        _shadowColorBuffer.ApplyOptions(options);
        
        GL.NamedFramebufferDrawBuffer(_shadowFramebuffer.Handle, DrawBufferMode.None);
        GL.NamedFramebufferReadBuffer(_shadowFramebuffer.Handle, ReadBufferMode.None);
    }

    public void BindTextureLayer(int layer)
    {
       _shadowFramebuffer.AttachTextureLayer(FramebufferAttachment.DepthAttachment, _shadowColorBuffer, layer);
    }

    public void BindTextures()
    {
        _shadowColorBuffer.ActivateUnit(ShadowUnit);
    }
    
    public void Bind()
    {
        _shadowFramebuffer.Bind();
    }

    public void Unbind()
    {
        _shadowFramebuffer.Unbind();
    }

    public void Dispose()
    {
        _shadowColorBuffer.Dispose();
        _shadowFramebuffer.Dispose();
    }
}