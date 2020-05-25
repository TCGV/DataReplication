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
            BuildRRG(0, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(PositiveParameterException))]
        public void Build_K_Zero_Test()
        {
            BuildRRG(5, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(GraphOrderException))]
        public void Build_Invalid_Order_Test()
        {
            BuildRRG(5, 5);
        }

        [TestMethod]
        public void Build_N5_K2_Test()
        {
            BuildAndAssertDegree(5, 2);
        }

        [TestMethod]
        public void Build_N11_K3_Test()
        {
            BuildAndAssertDegree(11, 3);
        }

        [TestMethod]
        public void Build_N1000_K4_Test()
        {
            BuildAndAssertDegree(1000, 4);
        }

        [TestMethod]
        public void RRG_MaxDiameter_Test()
        {
            var n = (int)1e3;
            Assert.IsTrue(BuildRRG(n, 3).GetDiameter() <= 500);
            Assert.IsTrue(BuildRRG(n, 3).GetDiameter() <= 13);
            Assert.IsTrue(BuildRRG(n, 4).GetDiameter() <= 9);
            Assert.IsTrue(BuildRRG(n, 5).GetDiameter() <= 7);
            Assert.IsTrue(BuildRRG(n, 6).GetDiameter() <= 6);
            Assert.IsTrue(BuildRRG(n, 7).GetDiameter() <= 6);
            Assert.IsTrue(BuildRRG(n, 8).GetDiameter() <= 5);
        }

        private static void BuildAndAssertDegree(int n, int k)
        {
            DataModel.Graph g = BuildRRG(n, k);

            Assert.AreEqual(n, g.Vertices.Length);
            Assert.IsTrue(g.Vertices.All(v => v.Neighbors.Count == k));
        }

        private static DataModel.Graph BuildRRG(int n, int k)
        {
            var b = new RandomRegularGraphBuilder();
            var g = b.Build(n, k);
            return g;
        }
    }
}