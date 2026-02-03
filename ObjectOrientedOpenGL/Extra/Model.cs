using ObjectOrientedOpenGL.Core;
using OpenTK.Mathematics;

namespace ObjectOrientedOpenGL.Extra;

public class Model(string path, List<Mesh> meshes, Model.Node root) : IDisposable
{
    public string Path { get; } = path;
    private List<Mesh> Meshes { get; } = meshes;
    public Node Root { get; set; } = root;

    public class Node(string name, Matrix4tk transform, List<Mesh> meshes, List<Node> children)
    {
        public string Name { get; } = name;
        public Matrix4tk Transform { get; } = transform;
        public List<Mesh> Meshes { get; } = meshes;
        public List<Node> Children { get; } = children;
    }
    
    public void Draw(Shader shader, Matrix4tk parentTransform)
    {
        DrawNode(Root, parentTransform, shader);
    }

    private static void DrawNode(Node node, Matrix4tk parentTransform, Shader shader)
    {
        var globalTransform = node.Transform * parentTransform;
        
        shader.LoadMatrix4("model", globalTransform);

        foreach (var mesh in node.Meshes)
        {
            mesh.Bind();
            mesh.RenderIndexed();
            mesh.Unbind();
        }
        
        foreach (var child in node.Children)
        {
            DrawNode(child, globalTransform, shader);
        }
    }

    public void Dispose()
    {
        foreach (var mesh in Meshes)
        {
            mesh.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}