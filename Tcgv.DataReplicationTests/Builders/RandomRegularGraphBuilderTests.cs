using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Tcgv.DataReplication.Builders;
using Tcgv.DataReplication.DataModel;
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
        public void RRG_Connectivity_Test()
        {
            var n = (int)1e3;
            Assert.AreEqual(3, BuildRRG(n, 3).GetConnectivity());
            Assert.AreEqual(4, BuildRRG(n, 4).GetConnectivity());
            Assert.AreEqual(5, BuildRRG(n, 5).GetConnectivity());
            Assert.AreEqual(6, BuildRRG(n, 6).GetConnectivity());
            Assert.AreEqual(7, BuildRRG(n, 7).GetConnectivity());
            Assert.AreEqual(8, BuildRRG(n, 8).GetConnectivity());
        }

        [TestMethod]
        public void RRG_MaxDiameter_Test()
        {
            var n = (int)1e3;
            Assert.IsTrue(BuildRRG(n, 3).GetDiameter() <= 13);
            Assert.IsTrue(BuildRRG(n, 4).GetDiameter() <= 9);
            Assert.IsTrue(BuildRRG(n, 5).GetDiameter() <= 7);
            Assert.IsTrue(BuildRRG(n, 6).GetDiameter() <= 6);
            Assert.IsTrue(BuildRRG(n, 7).GetDiameter() <= 6);
            Assert.IsTrue(BuildRRG(n, 8).GetDiameter() <= 5);
        }

        [TestMethod]
        public void RRG_InterGraph_Connectivity_Test()
        {
            var n = (int)1e2;

            var g1 = BuildRRG(n, 15);
            var g2 = BuildRRG(n, 15);
            Assert.AreEqual(15, g1.GetConnectivity());
            Assert.AreEqual(15, g2.GetConnectivity());

            g1.Vertices[50].AddNeighbors(g2.Vertices[51]);
            g1.Vertices[51].AddNeighbors(g2.Vertices[52]);
            g1.Vertices[53].AddNeighbors(g2.Vertices[52]);

            var g = new Graph(g1.Vertices.Union(g2.Vertices).ToArray());
            Assert.AreEqual(3, g.GetConnectivity());
        }

        private static void BuildAndAssertDegree(int n, int k)
        {
            var g = BuildRRG(n, k);

            Assert.AreEqual(n, g.Vertices.Length);
            if (g.Vertices.Length % 2 == 0)
                Assert.IsTrue(g.Vertices.All(v => v.Neighbors.Count == k));
            else
                Assert.IsTrue(g.Vertices.Count(v => v.Neighbors.Count == k) >= g.Vertices.Length - 1);
        }

        private static Graph BuildRRG(int n, int k)
        {
            var b = new RandomRegularGraphBuilder();
            var g = b.Build(n, k);
            return g;
        }
    }
}