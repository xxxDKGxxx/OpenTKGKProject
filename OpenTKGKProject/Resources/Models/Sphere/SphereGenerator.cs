using OpenTK.Mathematics;
using OpenTKGKProject.Resources;

public static class SphereGenerator
{
    public static (Vertex[] vertices, int[] indices) Generate(float radius, int widthSegments, int heightSegments)
    {
        List<Vertex> vertices = new List<Vertex>();
        List<int> indices = new List<int>();

        for (int y = 0; y <= heightSegments; y++)
        {
            for (int x = 0; x <= widthSegments; x++)
            {
                // 1. Obliczanie pozycji (x, y, z) na sferze
                float xSegment = (float)x / widthSegments;
                float ySegment = (float)y / heightSegments;
                
                // Kąty w radianach
                float xPos = (float)(Math.Cos(xSegment * 2.0f * Math.PI) * Math.Sin(ySegment * Math.PI));
                float yPos = (float)Math.Cos(ySegment * Math.PI);
                float zPos = (float)(Math.Sin(xSegment * 2.0f * Math.PI) * Math.Sin(ySegment * Math.PI));

                // 2. Tworzenie wierzchołka
                Vertex v = new Vertex();
                
                // Pozycja
                v.Position = new Vector3(xPos, yPos, zPos) * radius;
                
                // Normalna - dla kuli to po prostu znormalizowana pozycja!
                // To właśnie to zapewnia "gładkie cieniowanie" (Smooth Shading)
                v.Normal = new Vector3(xPos, yPos, zPos).Normalized();
                v.Color = new Vector3(1.0f, 0.0f, 0.0f);

                vertices.Add(v);
            }
        }

        // 3. Generowanie indeksów (trójkątów)
        // Łączymy punkty w kwadraty, a kwadraty tniemy na dwa trójkąty
        for (int y = 0; y < heightSegments; y++)
        {
            for (int x = 0; x < widthSegments; x++)
            {
                int current = y * (widthSegments + 1) + x;
                int next = current + widthSegments + 1;

                // Pierwszy trójkąt
                indices.Add(current);
                indices.Add(next);
                indices.Add(current + 1);

                // Drugi trójkąt
                indices.Add(current + 1);
                indices.Add(next);
                indices.Add(next + 1);
            }
        }

        return (vertices.ToArray(), indices.ToArray());
    }
}