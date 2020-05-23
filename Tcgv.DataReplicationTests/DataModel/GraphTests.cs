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
    }
}