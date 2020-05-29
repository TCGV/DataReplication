using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Tcgv.DataReplication.Builders;

namespace Tcgv.DataReplication.DataModel.Tests
{
    [TestClass()]
    public class GraphTests
    {
        [TestMethod]
        public void GraphItem_ZeroDisruption_PropagationTest()
        {
            var n = (int)1e3;
            var k = 4;

            var sourceVertex = 50;
            var iterations = (int)Math.Ceiling(Math.Log(n, k - 1)) + 1;

            var graph = new RandomRegularGraphBuilder().Build(n, k);

            var item = Guid.NewGuid().GetHashCode();
            graph.Vertices[sourceVertex].AddItem(item);

            graph.Propagate(iterations);

            Assert.AreEqual(graph.Vertices.Length, graph.Vertices.Count(n => n.Items.Contains(item)));
        }

        [TestMethod]
        public void GraphItem_MinorDisruption_PropagationTest()
        {
            var n = (int)1e3;
            var k = 4;

            var sourceVertex = 50;
            var disabled = (int)(0.05 * n);
            var iterations = (int)Math.Ceiling(Math.Log(n, k - 1)) + 1;

            var graph = new RandomRegularGraphBuilder().Build(n, k);

            var item = Guid.NewGuid().GetHashCode();
            graph.Vertices[sourceVertex].AddItem(item);

            graph.DisableRandomVertices(disabled, sourceVertex);
            graph.Propagate(iterations);

            Assert.AreEqual(graph.Vertices.Length, graph.Vertices.Count(n => n.Items.Contains(item)));
        }

        [TestMethod]
        public void GraphItem_MajorDisruption_PropagationTest()
        {
            var n = (int)1e3;
            var k = 4;

            var sourceVertex = 50;
            var disabled = (int)(0.50 * n);
            var iterations = (int)Math.Ceiling(Math.Log(n, k - 1)) + 1;

            var graph = new RandomRegularGraphBuilder().Build(n, k);

            var item = Guid.NewGuid().GetHashCode();
            graph.Vertices[sourceVertex].AddItem(item);

            graph.DisableRandomVertices(disabled, sourceVertex);
            graph.Propagate(iterations);

            Assert.IsTrue(graph.Vertices.Count(n => n.Items.Contains(item)) < 500);
        }

        [TestMethod()]
        public void GetShortestPathTest()
        {
            var g = BuildSampleGraph();

            Assert.AreEqual(3, g.GetShortestPath(g.Vertices[5], g.Vertices[7]).Length);
            Assert.AreEqual(4, g.GetShortestPath(g.Vertices[6], g.Vertices[5]).Length);
            Assert.AreEqual(5, g.GetShortestPath(g.Vertices[0], g.Vertices[10]).Length);
        }

        [TestMethod()]
        public void GetConnectivityTest()
        {
            var g = BuildSampleGraph();

            Assert.AreEqual(2, g.GetConnectivity(g.Vertices[5], g.Vertices[7]));
            Assert.AreEqual(2, g.GetConnectivity(g.Vertices[6], g.Vertices[5]));
            Assert.AreEqual(3, g.GetConnectivity(g.Vertices[0], g.Vertices[2]));
            Assert.AreEqual(3, g.GetConnectivity(g.Vertices[0], g.Vertices[9]));
        }

        private static Graph BuildSampleGraph()
        {
            var vertices = new Vertex[11];
            for (int i = 0; i < vertices.Length; i++)
                vertices[i] = new Vertex();

            vertices[0].AddNeighbors(vertices[1], vertices[2], vertices[3]);
            vertices[1].AddNeighbors(vertices[4]);
            vertices[2].AddNeighbors(vertices[5]);
            vertices[3].AddNeighbors(vertices[6]);
            vertices[4].AddNeighbors(vertices[9]);
            vertices[5].AddNeighbors(vertices[8]);
            vertices[6].AddNeighbors(vertices[7]);
            vertices[7].AddNeighbors(vertices[2], vertices[8], vertices[9]);
            vertices[8].AddNeighbors(vertices[9]);
            vertices[9].AddNeighbors(vertices[10]);

            var g = new Graph(vertices);
            return g;
        }
    }
}