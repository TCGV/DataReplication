using System;
using System.Linq;
using Tcgv.DataReplication.Builders;

namespace Tcgv.DataReplicationApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            var n = (int)1e4;
            var k = 8;
            int iterations = (int)Math.Ceiling(Math.Log(n, k - 1));

            Console.WriteLine($"Disruption\tIterations");

            for (double perc = 0; perc < 0.5; perc += 0.05)
            {
                var reached = -1;
                int disabled = (int)(perc * n);

                for (; iterations < n; iterations++)
                {
                    var graph = new RandomRegularGraphBuilder().Build(n, k);

                    var sourceVertex = 50; // Any
                    var item = Guid.NewGuid().GetHashCode();
                    graph.Vertices[sourceVertex].AddItem(item);

                    graph.DisableRandomVertices(disabled, sourceVertex);
                    graph.Propagate(iterations);

                    reached = graph.Vertices.Count(n => n.Items.Contains(item));
                    if (reached == n)
                        break;
                }

                Console.WriteLine($"{perc.ToString("0.00")}\t{iterations}");
            }

            Console.ReadKey();
        }
    }
}
