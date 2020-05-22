using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Tcgv.DataReplication.Builders;

namespace Tcgv.DataReplication.DataModel.Tests
{
    [TestClass()]
    public class GraphTests
    {
        [TestMethod()]
        public void CreateGraphTest()
        {
            var k = 4;
            var n = (int)1e4;
            var graph = new RandomRegularGraphBuilder().Build(n, k);

            var item = 123456789;
            graph.Vertices[50].AddItem(item);

            var iterations =
                (int)Math.Ceiling(Math.Log(graph.Vertices.Length, k - 1)) + 1;
            graph.Propagate(iterations);

            Assert.AreEqual(graph.Vertices.Length, graph.Vertices.Count(n => n.Items.Contains(item)));
        }
    }
}