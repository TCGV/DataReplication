using System.Collections.Generic;
using Tcgv.DataReplication.DataModel;

namespace Tcgv.DataReplication.Builders
{
    public class RegularGraphBuilder : GraphBuilder
    {
        public override Graph Build(List<Vertex> vertices, int k)
        {
            ValidateParameters(vertices.Count, k);

            var g = new Graph(vertices.ToArray());

            var groups = new Queue<Vertex>[k + 1];
            for (int i = 1; i < groups.Length; i++)
                groups[i] = new Queue<Vertex>();
            groups[0] = new Queue<Vertex>(g.Vertices);

            var idx = 0;
            foreach (var a in g.Vertices)
            {
                for (int i = 0; a.Edges.Count < k && groups[k].Count < g.Vertices.Length; i++)
                {
                    while (groups[idx].Count == 0)
                        idx++;

                    Vertex b = null;
                    do
                    {
                        b = groups[idx].Dequeue();
                        if (b.Edges.Count != idx || b == a || a.Edges.Contains(b))
                        {
                            Enqueue(groups, b);
                            while (groups[idx].Count == 0)
                                idx++;
                            b = null;
                        }
                    } while (b == null);

                    if (b.Edges.Count < k)
                    {
                        b.Add(a);
                        a.Add(b);
                        Enqueue(groups, b);
                    }
                }
            }

            return g;
        }

        private static void Enqueue(Queue<Vertex>[] groups, Vertex b)
        {
            groups[b.Edges.Count].Enqueue(b);
        }
    }
}
