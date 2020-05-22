using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Tcgv.DataReplication.Builders;
using Tcgv.DataReplication.Exceptions;

namespace Tcgv.DataReplication.Builder.Tests
{
    [TestClass()]
    public class RandomRegularGraphBuilderTests
    {
        [TestMethod]
        [ExpectedException(typeof(PositiveParameterException))]
        public void Build_N_Zero_Test()
        {
            new RandomRegularGraphBuilder().Build(0, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(PositiveParameterException))]
        public void Build_K_Zero_Test()
        {
            new RandomRegularGraphBuilder().Build(5, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(GraphOrderException))]
        public void Build_Invalid_Order_Test()
        {
            new RandomRegularGraphBuilder().Build(5, 5);
        }

        [TestMethod]
        public void Build_N5_K2_Test()
        {
            BuildAndAssert(5, 2);
        }

        [TestMethod]
        public void Build_N1000_K4_Test()
        {
            BuildAndAssert(1000, 4);
        }

        private static void BuildAndAssert(int n, int k)
        {
            var b = new RandomRegularGraphBuilder();
            var g = b.Build(n, k);

            Assert.AreEqual(n, g.Vertices.Length);
            Assert.IsTrue(g.Vertices.All(v => v.Edges.Count == k));
        }
    }
}