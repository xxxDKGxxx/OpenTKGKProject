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
using OpenTKGKProject.Resources.Lights;
using OpenTKGKProject.Resources.Models;
using OpenTKGKProject.Resources.Models.Sphere;
using Ground = OpenTKGKProject.Resources.Ground;
using ShaderType = OpenTK.Graphics.OpenGL4.ShaderType;

namespace OpenTKGKProject;

public enum TimeOfDay
{
    Day,
    Night,
}

public class Program(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
    : ImGuiGameWindow(gameWindowSettings, nativeWindowSettings)
{
    private Shader GeometryPassShader { get; set; } = null!;
    private Shader LightningPassShader { get; set; } = null!;
    private Shader LightCubeShader { get; set; } = null!;
    private ColorfulCube Cube { get; set; } = null!;
    private ColorfulTetrahedron ColorfulTetrahedron { get; set; } = null!;
    private Sphere Sphere { get; set; } = null!;
    private CircleTrajectoryFollower CircleTrajectoryFollower = null!;
    private RustyCar Car = null!;
    
    private Camera Camera { get; set; } = null!;
    private LookAtObjectControl LookAtObjectControl { get; set; } = null!;
    private FollowObjectControl FollowObjectControl { get; set; } = null!;
    
    private Sky Sky { get; set; } = null!;
    private Overlay Overlay { get; set; } = null!;
    private FpsCounter FpsCounter { get; set; } = null!;
    private Stopwatch Stopwatch { get; } = new();
    private GBuffer GBuffer { get; set; } = null!;
    private Ground Ground { get; set; } = null!;
    private PointLight PointLight { get; set; } = null!;
    private Spotlight StaticSpotLight { get; set; } = null!;
    private Spotlight Spotlight { get; set; } = null!;
    private Spotlight Spotlight2 { get; set; } = null!;
    private Spotlight Taillight { get; set; } = null!;
    private Spotlight Taillight2 { get; set; } = null!;
    private LightCubeModel LightCubeModel { get; set; } = null!;
    private DirectionalLight SunLight = null!;
    private TimeOfDay TimeOfDay = TimeOfDay.Day;
    

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

        GeometryPassShader = new Shader(
            ("OpenTKGKProject.Resources.Shaders.g_buffer.vert", ShaderType.VertexShader), 
            ("OpenTKGKProject.Resources.Shaders.g_buffer.frag", ShaderType.FragmentShader));

        LightningPassShader = new Shader(
            ("OpenTKGKProject.Resources.Shaders.lightning_pass.vert", ShaderType.VertexShader),
            ("OpenTKGKProject.Resources.Shaders.lightning_pass.frag", ShaderType.FragmentShader));

        LightCubeShader = new Shader(
            ("OpenTKGKProject.Resources.Shaders.light_cube.vert", ShaderType.VertexShader),
            ("OpenTKGKProject.Resources.Shaders.light_cube.frag", ShaderType.FragmentShader));

        Camera = new Camera(
            new EditorControl((Vector3.UnitY + Vector3.UnitZ) * 2, Vector3.UnitY * 2),
            new PerspectiveProjection());

        LookAtObjectControl = new LookAtObjectControl(Camera.Control);

        FollowObjectControl = new FollowObjectControl(Camera.Control);

        Cube = new ColorfulCube(new Vector3(0, 2, 0));

        ColorfulTetrahedron = new ColorfulTetrahedron(new Vector3(-2, 2, 2));

        CircleTrajectoryFollower = new CircleTrajectoryFollower(
            new Vector3(0,
                0,
                0),
            10.0f,
            MathHelper.PiOver4,
            MathHelper.PiOver2);

        Sphere = new Sphere(new Vector3(3, 3, 3));
        
        Overlay = new Overlay(new Vector2(0, 10), () => ImGui.Text($"{DateTime.Now:HH:mm:ss}"), Anchor.TopCenter);

        FpsCounter = new FpsCounter();

        Car = new RustyCar(new Vector3(0, 0, 0), new Vector3(1, 0, 0));

        Matrix4 scale = Matrix4.CreateScale(0.01f); 
        Matrix4 translation = Matrix4.CreateTranslation(0.0f, 1f, 0);
        Matrix4 transform = scale * translation;

        Car.Transform = transform;

        GBuffer = new GBuffer(ClientSize.X, ClientSize.Y);

        Ground = new Ground(100f, new Vector3(0.5f, 0.5f, 0.5f));

        PointLight = new PointLight(new Vector3(1.0f), new Vector3(1.0f, 3.0f, 0.0f));

        Spotlight = new Spotlight(
            new Vector3(1.0f),
            new Vector3(-1.0f, 1.0f, 0.0f),
            new Vector3(0.0f, 0.0f, 1.0f),
            MathF.Cos((float)MathHelper.DegreesToRadians(12.5)),
            MathF.Cos((float)MathHelper.DegreesToRadians(17.5)))
            .AttachedTo(Car, new Vector3(100, 100, 300));

        Spotlight2 = new Spotlight(
                new Vector3(1.0f),
            new Vector3(-1.0f, 1.0f, 0.0f),
            new Vector3(0.0f, 0.0f, 1.0f),
            MathF.Cos((float)MathHelper.DegreesToRadians(12.5)),
            MathF.Cos((float)MathHelper.DegreesToRadians(17.5)))
            .AttachedTo(Car, new Vector3(-100, 100, 300));
        
        Taillight = new Spotlight(
            new Vector3(1.0f, 0.0f, 0.0f),
            new Vector3(-1.0f, 1.0f, 0.0f),
            new Vector3(0.0f, 0.0f, -1.0f),
            MathF.Cos((float)MathHelper.DegreesToRadians(35.0f)),
            MathF.Cos((float)MathHelper.DegreesToRadians(50.0f)))
            .AttachedTo(Car, new Vector3(-100, 90, -350));
        
        Taillight2 = new Spotlight(
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(-1.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 0.0f, -1.0f),
                MathF.Cos((float)MathHelper.DegreesToRadians(35.0f)),
                MathF.Cos((float)MathHelper.DegreesToRadians(50.0f)))
            .AttachedTo(Car, new Vector3(100, 90, -350));

        StaticSpotLight = new Spotlight(new Vector3(1.0f, 1.0f, 0.0f),
            new Vector3(-1.0f, 1.0f, 0.0f),
            new Vector3(0.0f, 0.0f, 1.0f),
            MathF.Cos((float)MathHelper.DegreesToRadians(12.5)),
            MathF.Cos((float)MathHelper.DegreesToRadians(17.5)));

        SunLight = new DirectionalLight(
            new Vector3(1.0f, 0.95f, 0.85f), 
            new Vector3(0, -1, -1));

        LightCubeModel = new LightCubeModel();
        
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
        GeometryPassShader.Dispose();
        LightningPassShader.Dispose();
        Sphere.Dispose();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        
        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        
        Camera.Aspect = (float)ClientSize.X / ClientSize.Y;
        GBuffer.Resize(ClientSize.X, ClientSize.Y);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        
        FpsCounter.Update(args.Time);
        Camera.Update((float)args.Time);
        
        if (ImGui.GetIO().WantCaptureMouse) return;

        var keyboard = KeyboardState.GetSnapshot();
        var mouse = MouseState.GetSnapshot();

        Camera.HandleInput((float)args.Time, keyboard, mouse);

        if (keyboard.IsKeyDown(Keys.Escape)) Close();
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        // Geometry Pass
        
        GBuffer.Bind();
        
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.Enable(EnableCap.DepthTest);
        
        CircleTrajectoryFollower.Update(Car, (float)args.Time);
        LookAtObjectControl.UpdateObjectMatrix(Car.Transform);
        FollowObjectControl.UpdateObjectMatrix(Car.Transform);
        
        GeometryPassShader.LoadMatrix4("view", Camera.ViewMatrix);
        GeometryPassShader.LoadMatrix4("projection", Camera.ProjectionMatrix);
        
        Cube.Render(GeometryPassShader);
        ColorfulTetrahedron.Render(GeometryPassShader);
        Sphere.Render(GeometryPassShader);
        Car.Render(GeometryPassShader);
        Ground.Render(GeometryPassShader);
        
        GBuffer.Unbind();
        
        // Lightning pass
        GL.Disable(EnableCap.DepthTest);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        var lights = GetSceneLights()
            .Select(l => l.GetShaderLightData())
            .ToArray();
        
        SetupLights(lights);
        
        LightningPassShader.LoadFloat3("fogColor", new Vector3(0.5f, 0.5f, 0.5f));
        LightningPassShader.LoadFloat("fogStart", 20f);
        LightningPassShader.LoadFloat("fogEnd", 100f);
        
        LightningPassShader.LoadMatrix4("invView", Camera.ViewMatrix.Inverted());
        LightningPassShader.LoadMatrix4("invProj", Camera.ProjectionMatrix.Inverted());
        LightningPassShader.LoadFloat3("viewPos", Camera.Position);
        LightningPassShader.LoadInteger("isPerspective", Camera.Projection is PerspectiveProjection ? 1 : 0);
        
        GBuffer.Draw(LightningPassShader);
        
        // Forward pass
        
        GBuffer.CopyDepthToScreen(ClientSize.X, ClientSize.Y);
        
        GL.Enable(EnableCap.DepthTest);

        if (_debugLights)
        {
            RenderLights(lights);
        }
        
        FpsCounter.Render();
        
        RenderGui();
        
        Context.SwapBuffers();
    }

    private IShaderLight[] GetSceneLights()
    {
        var lights = new List<IShaderLight>
        {
            PointLight,
            Spotlight,
            Spotlight2,
            Taillight,
            Taillight2,
            StaticSpotLight
        };

        if (TimeOfDay == TimeOfDay.Day)
        {
            lights.Add(SunLight);
        }
        
        return lights.ToArray();
    }

    private void RenderLights(Light[] lights)
    {
        LightCubeShader.Use();
        LightCubeShader.LoadMatrix4("view", Camera.ViewMatrix);
        LightCubeShader.LoadMatrix4("projection", Camera.ProjectionMatrix);
        LightCubeModel.Bind();
        
        foreach (var light in lights)
        {
            var lightDir = light.Direction;
            var up = Vector3.UnitY;

            if (Math.Abs(Vector3.Dot(lightDir, up)) > 0.99f)
            {
                up = Vector3.UnitZ;
            }

            var viewMatrix = Matrix4.LookAt(Vector3.Zero, lightDir, up);
            var rotationMatrix = viewMatrix.Inverted();
            var modelMatrix = Matrix4.CreateScale(0.2f) * rotationMatrix * Matrix4.CreateTranslation(light.Position);
            LightCubeShader.LoadMatrix4("model", modelMatrix);
            LightCubeShader.LoadFloat3("lightColor", light.Color);
            LightCubeModel.Render();
        }
        
        LightCubeModel.Unbind();
    }

    private void SetupLights(Light[] lights)
    {
        for (var i = 0; i < lights.Length; i++)
        {
            var name = $"lights[{i}]";
            
            var light = lights[i];
            
            // Wysyłamy pola jedno po drugim
            LightningPassShader.LoadInteger($"{name}.type", (int)light.Type);
            LightningPassShader.LoadFloat3($"{name}.position", light.Position);
            LightningPassShader.LoadFloat3($"{name}.color", light.Color);
    
            // Attenuation
            LightningPassShader.LoadFloat($"{name}.constant", light.Constant);
            LightningPassShader.LoadFloat($"{name}.linear", light.Linear);
            LightningPassShader.LoadFloat($"{name}.quadratic", light.Quadratic);
    
            // Reflektor (Spot Light)
            LightningPassShader.LoadFloat3($"{name}.direction", light.Direction);
            LightningPassShader.LoadFloat($"{name}.cutOff", light.CutOff);
            LightningPassShader.LoadFloat($"{name}.outerCutOff", light.OuterCutOff);
        }
        
        LightningPassShader.LoadInteger("lightCount", lights.Length);
    }

    private static int _control = 3;
    private static int _projection;
    private static int _renderMode = 0;
    private static int _timeOfDay = 0;
    private static float _cutOff = MathHelper.DegreesToRadians(12.5f);
    private static float _outerCutOff = MathHelper.DegreesToRadians(17.5f);
    private static float _spotlightX = 0.0f;
    private static float _spotLightY = 0.0f;
    private static float _spotLightZ = 1.0f;
    private static bool _debugLights = false;
    
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

            if (ImGui.RadioButton("Follow object", ref _control, 5))
            {
                FollowObjectControl = new FollowObjectControl(Camera.Control);
                Camera.Control = FollowObjectControl;
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
        
        ImGui.Begin("Render mode");
        
        if (ImGui.RadioButton("Full", ref _renderMode, 0))
        {
            GBuffer.RenderMode = RenderMode.Full;
        }

        if (ImGui.RadioButton("Color", ref _renderMode, 1))
        {
            GBuffer.RenderMode = RenderMode.Color;
        }

        if (ImGui.RadioButton("Depth", ref _renderMode, 2))
        {
            GBuffer.RenderMode = RenderMode.Depth;
        }

        if (ImGui.RadioButton("Normals", ref _renderMode, 3))
        {
            GBuffer.RenderMode = RenderMode.Normals;
        }

        ImGui.Checkbox("Debug lights", ref _debugLights);
        
        ImGui.End();

        ImGui.Begin("Spotlights");

        if (ImGui.InputFloat("SpotlightX", ref _spotlightX, 0f))
        {
            foreach (var shaderLight in GetSceneLights())
            {
                if (shaderLight is Spotlight s && s != Taillight && s != Taillight2)
                {
                    s.SetDirection(new Vector3(_spotlightX, _spotLightY, _spotLightZ));
                }
            }
        }

        if (ImGui.InputFloat("SpotlightY", ref _spotLightY, 0f))
        {
            foreach (var shaderLight in GetSceneLights())
            {
                if (shaderLight is Spotlight s && s != Taillight && s != Taillight2)
                {
                    s.SetDirection(new Vector3(_spotlightX, _spotLightY, _spotLightZ));
                }
            }
        }

        if (ImGui.InputFloat("SpotlightZ", ref _spotLightZ, 0f))
        {
            foreach (var shaderLight in GetSceneLights())
            {
                if (shaderLight is Spotlight s && s != Taillight && s != Taillight2)
                {
                    s.SetDirection(new Vector3(_spotlightX, _spotLightY, _spotLightZ));
                }
            }
        }

        if (ImGui.SliderAngle("Cutoff Angle", ref _cutOff, 0, 90))
        {
            foreach (var shaderLight in GetSceneLights())
            {
                if (shaderLight is Spotlight s && s != Taillight && s != Taillight2)
                {
                    s.SetCutoff(MathF.Cos(_cutOff));
                }
            }
        }

        if (ImGui.SliderAngle("Outer Cutoff Angle", ref _outerCutOff, _cutOff, 90))
        {
            foreach (var shaderLight in GetSceneLights())
            {
                if (shaderLight is Spotlight s && s != Taillight && s != Taillight2)
                {
                    s.SetOuterCutOff(MathF.Cos(_outerCutOff));
                }
            }
        }
        
        ImGui.End();
        
        ImGui.Begin("Time of Day");

        if (ImGui.RadioButton("Day", ref _timeOfDay, 0))
        {
            TimeOfDay = TimeOfDay.Day;
        }

        if (ImGui.RadioButton("Night", ref _timeOfDay, 1))
        {
            TimeOfDay = TimeOfDay.Night;
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