using ObjectOrientedOpenGL.Core;
using ObjectOrientedOpenGL.Extra;
using OpenTK.Mathematics;

namespace OpenTKGKProject.Resources.Models;

public class RustyCar : IModel
{
    public Matrix4 Transform { get; set; }
    
    private readonly Model _carModel;

    public RustyCar(OpenTK.Mathematics.Vector3 position, OpenTK.Mathematics.Vector3 color)
    {
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
        
        Transform = Matrix4.CreateTranslation(position);
    }

    public void Render(Shader shader)
    {
        _carModel.Draw(shader, Transform);
    }

    public Matrix4 ModelMatrix { get => Transform; set => Transform = value; }
}