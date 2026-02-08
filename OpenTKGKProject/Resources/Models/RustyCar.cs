using ObjectOrientedOpenGL.Extra;

namespace OpenTKGKProject.Resources.Models;

public class RustyCar : IModel, IDisposable
{
    public Matrix4 ModelMatrix { get; set; }

    private readonly Model _carModel;
    
    public RustyCar(OpenTK.Mathematics.Vector3 position, OpenTK.Mathematics.Vector3 color)
    {
        ModelMatrix = Matrix4.CreateTranslation(position);
        
        _carModel = ModelLoader.Load(
            "OpenTKGKProject.Resources.Models.RustyCar.source.oldcar.FBX",
            (mesh, i) => new Vertex(
                mesh.Vertices[i].AsOpenTkVector(),
                color,
                mesh.Normals[i].AsOpenTkVector()
                ),
            [
                new VertexBuffer.Attribute(0, 3),
                new VertexBuffer.Attribute(1, 3),
                new VertexBuffer.Attribute(2, 3)
            ],
            node => !node.Name.Contains("Plane", StringComparison.OrdinalIgnoreCase));
    }

    public void Render(Shader shader)
    {
        _carModel.Draw(shader, ModelMatrix);
    }

    public void Dispose()
    {
        _carModel.Dispose();
    }
}