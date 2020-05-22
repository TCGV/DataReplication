using System.Collections.Generic;

namespace Tcgv.DataReplication.DataModel
{
    public class Graph
    {
        public Graph()
        {
            Vertices = new Vertex[0];
        }

        public Graph(Vertex[] vertices)
        {
            Vertices = vertices;
        }

        public Graph(Vertex[] vertices, HashSet<int>[] edges)
        {
            Vertices = vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                foreach (var e in edges[i])
                    vertices[i].Edges.Add(vertices[e]);
            }
        }

        public Vertex[] Vertices { get; private set; }

        public void Propagate(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                foreach (var v in Vertices)
                    v.Propagate();
                foreach (var v in Vertices)
                    v.Commit();
            }
        }
    }
}
