using System;
using System.Collections.Generic;
using System.Linq;

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

        public int GetDiameter()
        {
            return Vertices.Max(
                v => v.GetMaxShortestPathLength(Vertices.Length)
            );
        }

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

        public void DisableRandomVertices(int count, int exclusiveIndex)
        {
            var rd = new Random(Guid.NewGuid().GetHashCode());
            while (count > 0)
            {
                var i = rd.Next(Vertices.Length);
                if (Vertices[i].IsEnabled && i != exclusiveIndex)
                {
                    Vertices[i].Disable();
                    count--;
                }
            }
        }
    }
}
