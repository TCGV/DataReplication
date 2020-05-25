using System;
using System.Collections.Generic;
using System.Linq;
using Tcgv.DataReplication.Builders;
using Tcgv.DataReplication.Extensions;

namespace Tcgv.DataReplicationApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            //var n = (int)1e4;
            //var k = 8;
            //int iterations = (int)Math.Ceiling(Math.Log(n, k - 1));
            //SimulateReplications(n, k, iterations);

            for (int k = 3; k <= 9; k += 2)
            {
                Console.WriteLine($"N\tDiameter\tVariance\t[k={k}]");
                for (int n = 10; n <= 1e3; n += 99)
                {
                    var list = new List<double>();
                    for (int i = 0; i < 100; i++)
                    {
                        var graph = new RandomRegularGraphBuilder().Build(n, k);
                        list.Add(graph.GetDiameter());
                    }
                    Console.WriteLine($"{n}\t{list.Average().ToString("0.00")}\t{list.StdDev().ToString("0.00")}");
                }
            }

            Console.ReadKey();
        }

        private static void SimulateReplications(int n, int k, int iterations)
        {
            Console.WriteLine($"Disruption\tIterations\t[n={n}, k={k}]");

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
        }
    }
}
