namespace OpenTKGKProject.Resources.Models.Sphere;

public static class SphereGenerator
{
    public static (Vertex[] vertices, int[] indices) Generate(float radius, int widthSegments, int heightSegments)
    {
        var vertices = new List<Vertex>();
        var indices = new List<int>();

        for (var y = 0; y <= heightSegments; y++)
        {
            for (var x = 0; x <= widthSegments; x++)
            {
                // 1. Obliczanie pozycji (x, y, z) na sferze
                var xSegment = (float)x / widthSegments;
                var ySegment = (float)y / heightSegments;

                // Kąty w radianach
                var xPos = (float)(Math.Cos(xSegment * 2.0f * Math.PI) * Math.Sin(ySegment * Math.PI));
                var yPos = (float)Math.Cos(ySegment * Math.PI);
                var zPos = (float)(Math.Sin(xSegment * 2.0f * Math.PI) * Math.Sin(ySegment * Math.PI));

                // 2. Tworzenie wierzchołka
                Vertex v = new()
                {
                    // Pozycja
                    Position = new Vector3(xPos, yPos, zPos) * radius,

                    // Normalna - dla kuli to po prostu znormalizowana pozycja!
                    // To właśnie to zapewnia "gładkie cieniowanie" (Smooth Shading)
                    Normal = new Vector3(xPos, yPos, zPos).Normalized(),
                    Color = new Vector3(1.0f, 0.0f, 0.0f)
                };

                vertices.Add(v);
            }
        }

        // 3. Generowanie indeksów (trójkątów)
        // Łączymy punkty w kwadraty, a kwadraty tniemy na dwa trójkąty
        for (var y = 0; y < heightSegments; y++)
        {
            for (var x = 0; x < widthSegments; x++)
            {
                var current = y * (widthSegments + 1) + x;
                var next = current + widthSegments + 1;

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