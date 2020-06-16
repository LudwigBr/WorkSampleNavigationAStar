using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pot.Navigation.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PotTest
{
    [TestClass]

    public class NavigationNetworkTests
    {
        [TestMethod]
        public void GetAdjacentNodesSimple()
        {
            var nodes = new Node[2,1];
            nodes[0, 0] = new Node(5, 5);
            nodes[1, 0] = new Node(10, 5);
            var network = new NavigationNetwork(nodes);

            var actual = network.GetAdjacentNodes(nodes, 0, 0);

            Assert.IsTrue(actual.Count == 1);
            Assert.AreEqual(actual.First(), nodes[1, 0]);
        }

        [TestMethod]
        public void GetAdjacentNodes()
        {
            var nodes = new Node[3, 2];
            nodes[0, 0] = new Node(5, 5);
            nodes[0, 1] = new Node(5, 10);
            nodes[1, 0] = new Node(10, 5);
            nodes[1, 1] = new Node(10, 10);
            nodes[2, 0] = new Node(15, 5);
            nodes[2, 1] = new Node(15, 10);
            var network = new NavigationNetwork(nodes);

            var actual = network.GetAdjacentNodes(nodes, 0, 0);

            Assert.IsTrue(actual.Count == 3);
            var expected = new List<Node>() { 
                nodes[1, 0],
                nodes[0, 1], nodes[1, 1], 
            };
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void GetAdjacentNodesAllPossible()
        {
            var nodes = new Node[4, 3];
            nodes[0, 0] = new Node(5, 5);
            nodes[0, 1] = new Node(5, 10);
            nodes[0, 2] = new Node(5, 15);
            nodes[1, 0] = new Node(10, 5);
            nodes[1, 1] = new Node(10, 10);
            nodes[1, 2] = new Node(10, 15);
            nodes[2, 0] = new Node(15, 5);
            nodes[2, 1] = new Node(15, 10);
            nodes[2, 2] = new Node(15, 15);
            nodes[3, 0] = new Node(20, 5);
            nodes[3, 1] = new Node(20, 10);
            nodes[3, 2] = new Node(20, 15);
            var network = new NavigationNetwork(nodes);

            var actual = network.GetAdjacentNodes(nodes, 1, 1);

            Assert.IsTrue(actual.Count == 8);
            var expected = new List<Node>() { 
                nodes[0, 0], nodes[1, 0], nodes[2, 0], 
                nodes[0, 1], nodes[2, 1], 
                nodes[0, 2], nodes[1, 2], nodes[2, 2],
            };
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void GenerateNodesSimple()
        {
            byte[] fullMap = new byte[] {
                0x00,
                0x05,
            };
            int bytesPerRow = 1;
            var network = new NavigationNetwork(fullMap, bytesPerRow, 1);

            Assert.AreEqual((2, 2), network.NodesArraySize);
            Assert.AreEqual((1, 1), network.Nodes[1, 1]?.Position);
        }

        [TestMethod]
        public void GenerateNodesSimpleNonWalkable()
        {
            byte[] fullMap = new byte[] {
                0x00,
                0x00,
            };
            int bytesPerRow = 1;
            var network = new NavigationNetwork(fullMap, bytesPerRow, 1);

            Assert.AreEqual((2, 2), network.NodesArraySize);
            Assert.AreEqual(null, network.Nodes[0, 0]);
            Assert.AreEqual(null, network.Nodes[0, 1]);
            Assert.AreEqual(null, network.Nodes[1, 0]);
            Assert.AreEqual(null, network.Nodes[1, 1]);
        }

        [TestMethod]
        public void GenerateNodes()
        {
            byte[] fullMap = new byte[] {
                0x55, 0x55, 0x55, 
                0x55, 0x55, 0x55, 
                0x55, 0x55, 0x55,
                0x55, 0x55, 0x55,
                0x55, 0x55, 0x55,
            };
            int bytesPerRow = 3;
            var network = new NavigationNetwork(fullMap, bytesPerRow, 2);

            Assert.AreEqual((3, 3), network.NodesArraySize);
            Assert.AreEqual((0, 0), network.Nodes[0, 0].Position);
            Assert.AreEqual((0, 2), network.Nodes[0, 1].Position);
            Assert.AreEqual((0, 4), network.Nodes[0, 2].Position);
            Assert.AreEqual((2, 0), network.Nodes[1, 0].Position);
            Assert.AreEqual((2, 2), network.Nodes[1, 1].Position);
            Assert.AreEqual((2, 4), network.Nodes[1, 2].Position);
            Assert.AreEqual((4, 0), network.Nodes[2, 0].Position);
            Assert.AreEqual((4, 2), network.Nodes[2, 1].Position);
            Assert.AreEqual((4, 4), network.Nodes[2, 2].Position);
        }

        [TestMethod]
        public void GetClosestNodeSimple()
        {
            var nodes = new Node[1, 2];
            nodes[0, 0] = new Node(5, 5);
            nodes[0, 1] = new Node(5, 10);
            var network = new NavigationNetwork(nodes, 5);

            var actual = network.GetClosestNode((5, 5));

            Assert.AreEqual(nodes[0, 0], actual);
        }

        [TestMethod]
        public void GetClosestNodeLongDistance()
        {
            var nodes = new Node[2, 3];
            nodes[0, 0] = new Node(5, 5);
            nodes[0, 1] = new Node(5, 10);
            nodes[0, 2] = new Node(5, 15);
            nodes[1, 0] = new Node(10, 5);
            nodes[1, 1] = new Node(10, 10);
            nodes[1, 2] = new Node(10, 15);
            var network = new NavigationNetwork(nodes, 5);

            var actual = network.GetClosestNode((500, 500));

            Assert.AreEqual(nodes[1, 2], actual);
        }

        [TestMethod]
        public void GetClosestNodeMissingNodes()
        {
            var nodes = new Node[1, 3];
            nodes[0, 0] = new Node(5, 5);
            nodes[0, 1] = new Node(100, 100);
            nodes[0, 2] = new Node(200, 200);
            var network = new NavigationNetwork(nodes, 5);

            var actual = network.GetClosestNode((70, 70));

            Assert.AreEqual(nodes[0, 1], actual);
        }
    }
}
