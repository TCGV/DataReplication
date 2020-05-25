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
                    vertices[i].Neighbors.Add(vertices[e]);
            }
        }

        public Vertex[] Vertices { get; private set; }

        public int GetDiameter()
        {
            return Vertices.Max(
                v => v.GetMaxShortestPathLength(Vertices.Length)
            );
        }

        public Vertex[] GetShortestPath(Vertex from, Vertex to)
        {
            var queue = new Queue<dynamic>();
            var visited = new HashSet<Vertex>();

            dynamic q = new { V = from, Length = 1 };
            queue.Enqueue(q);
            visited.Add(from);

            while (queue.Count > 0 && !visited.Contains(to))
            {
                var x = queue.Dequeue();
                foreach (var c in x.V.Neighbors)
                {
                    if (!visited.Contains(c))
                    {
                        q = new { V = c, P = x, Length = x.Length + 1 };
                        queue.Enqueue(q);
                        visited.Add(c);
                        if (c == to)
                            break;
                    }
                }
            }

            if (queue.Count > 0)
            {
                var path = new Vertex[q.Length];
                int i = path.Length;
                while (--i >= 0)
                {
                    path[i] = q.V;
                    if (i > 0)
                        q = q.P;
                }
                return path;
            }
            return null;
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
