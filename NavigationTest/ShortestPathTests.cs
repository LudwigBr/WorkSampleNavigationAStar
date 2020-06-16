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
    public class ShortestPathTests
    {
        [TestMethod]
        public void GetShortestPathSimple()
        {
            var nodes = new Node[1, 3];
            nodes[0, 0] = new Node(5, 5);
            nodes[0, 1] = new Node(5, 10);
            nodes[0, 2] = new Node(5, 15);
            nodes[0, 0].AddConnection(nodes[0, 1]);
            nodes[0, 1].AddConnection(nodes[0, 2]);
            var network = new NavigationNetwork(nodes, 5);

            var actual = network.GetShortestPath(nodes[0, 0], nodes[0, 2]);

            var expected = new Stack<Node>();
            expected.Push(nodes[0, 2]);
            expected.Push(nodes[0, 1]);
            expected.Push(nodes[0, 0]);

            CollectionAssert.AreEqual(expected, actual.ToPreviousNodesStack());
        }

        [TestMethod]
        public void GetShortestPathWithObstacle()
        {
            var nodes = new Node[2, 3];
            nodes[0, 0] = new Node(5, 5);
            nodes[1, 0] = new Node(10, 5);
            nodes[1, 1] = new Node(10, 10);
            nodes[1, 2] = new Node(10, 15);
            nodes[0, 2] = new Node(5, 15);

            nodes[0, 0].AddConnection(nodes[1, 0]);
            nodes[0, 0].AddConnection(nodes[1, 1]);
            nodes[0, 2].AddConnection(nodes[1, 1]);
            nodes[0, 2].AddConnection(nodes[1, 2]);
            nodes[1, 0].AddConnection(nodes[1, 1]);
            nodes[1, 1].AddConnection(nodes[1, 2]);

            var network = new NavigationNetwork(nodes, 5);

            var actual = network.GetShortestPath(nodes[0, 0], nodes[0, 2]);

            var expected = new Stack<Node>();
            expected.Push(nodes[0, 2]);
            expected.Push(nodes[1, 1]);
            expected.Push(nodes[0, 0]);

            CollectionAssert.AreEqual(expected, actual.ToPreviousNodesStack());
        }

        [TestMethod]
        public void GetShortestPathWithObstacleBigger()
        {
            var nodes = new Node[3, 3];
            nodes[0, 0] = new Node(5, 5);
            nodes[0, 2] = new Node(5, 15);
            nodes[1, 0] = new Node(10, 5);
            nodes[1, 2] = new Node(10, 15);
            nodes[2, 0] = new Node(15, 5);
            nodes[2, 1] = new Node(15, 10);
            nodes[2, 2] = new Node(15, 15);

            nodes[0, 0].AddConnection(nodes[1, 0]);
            nodes[1, 0].AddConnection(nodes[2, 0]);
            nodes[1, 0].AddConnection(nodes[2, 1]);
            nodes[2, 0].AddConnection(nodes[2, 1]);
            nodes[2, 1].AddConnection(nodes[2, 2]);
            nodes[2, 1].AddConnection(nodes[1, 2]);
            nodes[1, 2].AddConnection(nodes[0, 2]);

            var network = new NavigationNetwork(nodes, 5);

            var actual = network.GetShortestPath(nodes[0, 0], nodes[0, 2]);

            var expected = new Stack<Node>();
            expected.Push(nodes[0, 2]);
            expected.Push(nodes[1, 2]);
            expected.Push(nodes[2, 1]);
            expected.Push(nodes[1, 0]);
            expected.Push(nodes[0, 0]);

            CollectionAssert.AreEqual(expected, actual.ToPreviousNodesStack());
        }
    }
}
