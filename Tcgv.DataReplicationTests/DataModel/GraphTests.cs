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
        public void GetShortestPathToTest()
        {
            var vertices = new Vertex[11];
            for (int i = 0; i < vertices.Length; i++)
                vertices[i] = new Vertex();

            vertices[0].Connect(vertices[1], vertices[2], vertices[3]);
            vertices[1].Connect(vertices[4]);
            vertices[2].Connect(vertices[5]);
            vertices[3].Connect(vertices[6]);
            vertices[4].Connect(vertices[9]);
            vertices[5].Connect(vertices[8]);
            vertices[6].Connect(vertices[7]);
            vertices[7].Connect(vertices[2], vertices[8]);
            vertices[8].Connect(vertices[9]);
            vertices[9].Connect(vertices[10]);

            var g = new Graph(vertices);
            Assert.AreEqual(null, g.GetShortestPath(vertices[5], vertices[7]));
            Assert.AreEqual(4, g.GetShortestPath(vertices[6], vertices[5]).Length);
            Assert.AreEqual(5, g.GetShortestPath(vertices[0], vertices[10]).Length);
        }
    }
}