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

        public int GetConnectivity()
        {
            var min = -1;
            for (int i = 1; i < Vertices.Length; i++)
            {
                var c = GetConnectivity(Vertices[0], Vertices[i]);
                if (min < 0 || c < min)
                    min = c;
            }
            return min;
        }

        public int GetConnectivity(Vertex from, Vertex to)
        {
            var avoid = new HashSet<Point>();
            return GetConnectivity(from, to, avoid);
        }

        public Vertex[] GetShortestPath(Vertex from, Vertex to)
        {
            return GetShortestPath(from, to, new HashSet<Point>());
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

        private int GetConnectivity(Vertex from, Vertex to, HashSet<Point> avoid)
        {
            int i = 0;
            Vertex[] path = new Vertex[0];

            while (path != null)
            {
                path = GetShortestPath(from, to, avoid);
                if (path != null)
                {
                    i++;
                    for (int j = 1; j < path.Length; j++)
                    {
                        avoid.Add(new Point(path[j - 1].Id, path[j].Id));
                        avoid.Add(new Point(path[j].Id, path[j - 1].Id));
                    }
                }
            }

            return i;
        }

        private Vertex[] GetShortestPath(Vertex from, Vertex to, HashSet<Point> avoid)
        {
            var q = DFS(from, to, avoid);

            if (q.Vertex == to)
                return ExtractPath(q);

            return null;
        }

        private static VertexDFSData DFS(Vertex from, Vertex to, HashSet<Point> avoid)
        {
            var queue = new Queue<VertexDFSData>();
            var visited = new HashSet<Point>(avoid);
            var q = new VertexDFSData { Vertex = from, Length = 1 };

            queue.Enqueue(q);

            while (queue.Count > 0 && q.Vertex != to)
            {
                var x = queue.Dequeue();
                foreach (var c in x.Vertex.Neighbors)
                {
                    var p = new Point(x.Vertex.Id, c.Id);
                    if (!visited.Contains(p))
                    {
                        q = new VertexDFSData { Vertex = c, Previous = x, Length = x.Length + 1 };
                        queue.Enqueue(q);
                        visited.Add(p);
                        if (c == to)
                            break;
                    }
                }
            }

            return q;
        }

        private static Vertex[] ExtractPath(VertexDFSData q)
        {
            var path = new Vertex[q.Length];
            int i = path.Length;
            while (--i >= 0)
            {
                path[i] = q.Vertex;
                if (i > 0)
                    q = q.Previous;
            }
            return path;
        }
    }
}
