using System.Diagnostics;
using System.Runtime.InteropServices;
using ImGuiNET;
using ObjectOrientedOpenGL.Core;
using ObjectOrientedOpenGL.Extra;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTKGKProject.Resources;
using OpenTKGKProject.Resources.Models;
using OpenTKGKProject.Resources.Models.Sphere;
using ShaderType = OpenTK.Graphics.OpenGL4.ShaderType;

namespace OpenTKGKProject;

public class Program(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
    : ImGuiGameWindow(gameWindowSettings, nativeWindowSettings)
{
    private Shader VertexWithColorsShader { get; set; } = null!;
    private ColorfulCube Cube { get; set; } = null!;
    private Sphere Sphere { get; set; } = null!;
    private CircleTrajectoryFollower CircleTrajectoryFollower = null!;
    private Plane Plane = null!;
    private RustyCar Car = null!;
    
    private Camera Camera { get; set; } = null!;
    private LookAtObjectControl LookAtObjectControl { get; set; } = null!;
    
    private Sky Sky { get; set; } = null!;
    private Overlay Overlay { get; set; } = null!;
    private FpsCounter FpsCounter { get; set; } = null!;
    // private Canvas<Color4, OpenTkColor4Converter> Canvas { get; set; } = null!;
    private Stopwatch Stopwatch { get; } = new();
    
    
    private Vector3 _lightPosition = new(5, 5, 5);
    private Vector3 _lightColor = new(1.0f, 1.0f, 1.0f);

    private DebugProc DebugProcCallback { get; } = OnDebugMessage;

    public static void Main(string[] args)
    {
        var gwSettings = GameWindowSettings.Default;
        var nwSettings = NativeWindowSettings.Default;
        nwSettings.NumberOfSamples = 16;

#if DEBUG
        nwSettings.Flags |= ContextFlags.Debug;
#endif

        using var program = new Program(gwSettings, nwSettings);
        program.Title = "OpenTKGKProject";
        program.ClientSize = new Vector2i(1280, 800);
        program.Run();
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.DebugMessageCallback(DebugProcCallback, IntPtr.Zero);
        GL.Enable(EnableCap.DebugOutput);

#if DEBUG
        GL.Enable(EnableCap.DebugOutputSynchronous);
#endif

        VertexWithColorsShader = new Shader(
            ("OpenTKGKProject.Resources.Shaders.shader.vert", ShaderType.VertexShader), 
            ("OpenTKGKProject.Resources.Shaders.shader.frag", ShaderType.FragmentShader));

        Camera = new Camera(
            new EditorControl((Vector3.UnitY + Vector3.UnitZ) * 2, Vector3.UnitY * 2),
            new PerspectiveProjection());

        LookAtObjectControl = new LookAtObjectControl(Camera.Control);

        Cube = new ColorfulCube(new Vector3(0, 2, 0));

        CircleTrajectoryFollower = new CircleTrajectoryFollower(
            new Vector3(0,
                0,
                0),
            10.0f,
            MathHelper.PiOver4,
            MathHelper.PiOver2);

        Sphere = new Sphere(new Vector3(3, 3, 3));
        
        Overlay = new Overlay(new Vector2(0, 10), () => ImGui.Text($"{DateTime.Now:HH:mm:ss}"), Anchor.TopCenter);

        // Canvas = new Canvas<Color4, OpenTkColor4Converter>(ClientSize.X, ClientSize.Y);

        FpsCounter = new FpsCounter();

        Car = new RustyCar(new Vector3(0, 0, 0), new Vector3(1, 0, 0));

        Matrix4 scale = Matrix4.CreateScale(0.01f); 
        Matrix4 translation = Matrix4.CreateTranslation(0.0f, 1f, 0);
        Matrix4 transform = scale * translation;

        Car.Transform = transform;
        
        Stopwatch.Start();

        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
        GL.Disable(EnableCap.CullFace);
        GL.Enable(EnableCap.DepthTest);
        GL.DepthFunc(DepthFunction.Lequal);
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        
        Cube.Dispose();
        VertexWithColorsShader.Dispose();
        Plane.Dispose();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        // Canvas.Resize(ClientSize.X, ClientSize.Y);
        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        Camera.Aspect = (float)ClientSize.X / ClientSize.Y;
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        
        FpsCounter.Update(args.Time);

        Camera.Update((float)args.Time);
        // Canvas.SetColor(Random.Shared.Next(ClientSize.X), Random.Shared.Next(ClientSize.Y), Color.OrangeRed);
        // Canvas.Update();
        
        if (ImGui.GetIO().WantCaptureMouse) return;

        var keyboard = KeyboardState.GetSnapshot();
        var mouse = MouseState.GetSnapshot();

        Camera.HandleInput((float)args.Time, keyboard, mouse);

        if (keyboard.IsKeyDown(Keys.Escape)) Close();
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        FpsCounter.Render();
        
        VertexWithColorsShader.Use();
        VertexWithColorsShader.LoadMatrix4("view", Camera.ViewMatrix);
        VertexWithColorsShader.LoadMatrix4("projection", Camera.ProjectionMatrix);
        VertexWithColorsShader.LoadFloat3("lightPos", _lightPosition);
        VertexWithColorsShader.LoadFloat3("lightColor", _lightColor);
        VertexWithColorsShader.LoadFloat3("viewPos", Camera.Position);
        
        CircleTrajectoryFollower.Update(Car, (float)args.Time);
        
        Cube.Render(VertexWithColorsShader);
        
        Sphere.Render(VertexWithColorsShader);
        
        Car.Render(VertexWithColorsShader);
        
        LookAtObjectControl.UpdateObjectMatrix(Car.Transform);
        
        RenderGui();

        Context.SwapBuffers();
    }

    private static int _control = 3;
    private static int _projection;
    protected override void BuildGuiLayout()
    {
        ImGui.Begin("Camera");
        if (ImGui.CollapsingHeader("Control"))
        {
            using var indent = new ImGuiIndent(10.0f);
            if (ImGui.RadioButton("No Control", ref _control, 0))
                Camera.Control = new NoControl(Camera.Control);
            if (ImGui.RadioButton("Orbital Control", ref _control, 1))
                Camera.Control = new OrbitalControl(Camera.Control);
            if (ImGui.RadioButton("FlyBy Control", ref _control, 2))
                Camera.Control = new FlyByControl(Camera.Control);
            if (ImGui.RadioButton("Editor Control", ref _control, 3))
                Camera.Control = new EditorControl(Camera.Control);
            if (ImGui.RadioButton("Look At Object Control", ref _control, 4))
            {
                LookAtObjectControl = new LookAtObjectControl(Camera.Control);
                
                Camera.Control = LookAtObjectControl;
            }
        }

        if (ImGui.CollapsingHeader("Projection"))
        {
            using var indent = new ImGuiIndent(10.0f);
            if (ImGui.RadioButton("Perspective", ref _projection, 0))
                Camera.Projection = new PerspectiveProjection { Aspect = Camera.Aspect };
            if (ImGui.RadioButton("Orthographic", ref _projection, 1))
                Camera.Projection = new OrthographicProjection { Aspect = Camera.Aspect };
        }

        ImGui.End();

        // ImGui.ShowDemoWindow();
        
        Overlay.Render();
        // Canvas.Render();
    }

    private static void OnDebugMessage(
        DebugSource source,     // Source of the debugging message.
        DebugType type,         // Type of the debugging message.
        int id,                 // ID associated with the message.
        DebugSeverity severity, // Severity of the message.
        int length,             // Length of the string in pMessage.
        IntPtr pMessage,        // Pointer to message string.
        IntPtr pUserParam)      // The pointer you gave to OpenGL.
    {
        var message = Marshal.PtrToStringAnsi(pMessage, length);

        var log = $"[{severity} source={source} type={type} id={id}] {message}";

        Console.WriteLine(log);
    }
}