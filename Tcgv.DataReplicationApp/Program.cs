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
            MeasureDiameters();

            RunSimulations();

            Console.ReadKey();
        }

        private static void MeasureDiameters()
        {
            for (int r = 3; r <= 9; r += 2)
            {
                Console.WriteLine($"N\tDiameter\tVariance\t[r={r}]");
                for (int n = 10; n <= 1e3; n += 99)
                {
                    var list = new List<double>();
                    for (int i = 0; i < 100; i++)
                    {
                        var graph = new RandomRegularGraphBuilder().Build(n, r);
                        list.Add(graph.GetDiameter());
                    }
                    Console.WriteLine($"{n}\t{list.Average().ToString("0.00")}\t{list.StdDev().ToString("0.00")}");
                }
            }
        }

        private static void RunSimulations()
        {
            var n = (int)1e3;
            for (int r = 4; r <= 12; r += 4)
                SimulateReplications(n, r);
        }

        private static void SimulateReplications(int n, int r)
        {
            Console.WriteLine($"Disruption\tIterations");

            for (double perc = 0; perc < 0.5; perc += 0.05)
            {
                var reached = -1;
                int iterations = 1;
                int disabled = (int)(perc * n);

                for (; iterations < n; iterations++)
                {
                    var graph = new RandomRegularGraphBuilder().Build(n, r);

                    var sourceVertex = 50; // Any
                    var item = Guid.NewGuid().GetHashCode();
                    graph.Vertices[sourceVertex].AddItem(item);

                    graph.DisableRandomVertices(disabled, sourceVertex);
                    graph.Propagate(iterations);

                    reached = graph.Vertices.Count(n => n.Items.Contains(item));
                    if (reached == n || iterations > n)
                        break;
                }

                Console.WriteLine($"{perc.ToString("0.00")}\t{iterations}");
            }
        }
    }
}
