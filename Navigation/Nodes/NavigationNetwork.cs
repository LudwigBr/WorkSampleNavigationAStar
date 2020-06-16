using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleToAttribute("NavigationTest")]

namespace Pot.Navigation.Nodes
{
    public class NavigationNetwork
    {
        private byte[] FullMap { get; }
        private int BytesPerRow { get; }

        internal (int columns, int rows) NodesArraySize { get; }
        internal Node[,] Nodes { get; }
        internal int NodeDistance { get; }

        public NavigationNetwork(byte[] fullMap, int bytesPerRow, int nodeDistance = 5)
        {
            FullMap = fullMap ?? throw new ArgumentNullException(nameof(fullMap));
            BytesPerRow = bytesPerRow;
            int rows = FullMap.Length / BytesPerRow;
            int columns = BytesPerRow * 2;
            NodesArraySize = (
                (int) Math.Ceiling(columns / (double)nodeDistance), 
                (int) Math.Ceiling(rows / (double)nodeDistance)
                );
            Nodes = GenerateNodes(nodeDistance);
            NodeDistance = nodeDistance;
        }

        // for test cases
        internal NavigationNetwork(Node[,] nodes, int nodeDistance = 5)
        {
            NodesArraySize = (nodes.GetLength(0), nodes.GetLength(1));
            Nodes = nodes;
            NodeDistance = 5;
        }

        public Node GetShortestPath(Node startNode, Node endNode)
        {
            FastPriorityQueue<Node> queue = new FastPriorityQueue<Node>(1024);
            startNode.LengthFromStart = 0;
            queue.Enqueue(startNode, 1);

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                if (currentNode == endNode)
                {
                    return currentNode;
                }
                currentNode.State = NodeState.Closed;
                ExpandNode(currentNode, queue, endNode);
            }
            return null;
        }

        private void ExpandNode(Node currentNode, FastPriorityQueue<Node> queue, Node endNode)
        {
            foreach (var connection in currentNode.Connections)
            {
                var successorNode = connection.OtherNode(currentNode);
                if (successorNode.State == NodeState.Closed) continue;
                var currentLength = currentNode.LengthFromStart + connection.Length;
                if (queue.Contains(currentNode) && successorNode.LengthFromStart < currentLength) continue;

                successorNode.PreviousNode = currentNode;
                successorNode.LengthFromStart = currentLength;

                var estimatedFullLength = successorNode.LengthFromStart + (float) Distance(successorNode, endNode);
                if (queue.Contains(successorNode))
                {
                    queue.UpdatePriority(successorNode, estimatedFullLength);
                }
                else
                {
                    queue.Enqueue(successorNode, estimatedFullLength);
                }    
            }
        }

        private Node[,] GenerateNodes(int nodeDistance = 5)
        {
            var nodes = new Node[NodesArraySize.columns, NodesArraySize.rows];

            ExtractMap.LoopMap(FullMap, BytesPerRow, (c, r) =>
            {
                if (!ExtractMap.IsWalkable(FullMap, BytesPerRow, c, r)) return;

                var node = new Node(c, r);
                (int c, int r) nodeIndex = (c / nodeDistance, r / nodeDistance);
                foreach (var adjacent in GetAdjacentNodes(nodes, nodeIndex.c, nodeIndex.r))
                {
                    node.AddConnection(adjacent);
                }
                nodes[nodeIndex.c, nodeIndex.r] = node;
            },
            nodeDistance,
            nodeDistance);

            return nodes;
        }

        internal List<Node> GetAdjacentNodes(Node[,] nodes, int cIndex, int rIndex)
        {
            var result = new List<Node>();
            for (var c = -1; c <= 1; c++)
            {
                for (var r = -1; r <= 1; r++)
                {
                    if (c == 0 && r == 0) continue;
                    if (cIndex + c < 0 || rIndex + r < 0) continue;
                    if (cIndex + c > NodesArraySize.columns - 1 || rIndex + r > NodesArraySize.rows -1) continue;
                    var node = nodes[cIndex + c, rIndex + r];
                    if (node == null) continue;
                    result.Add(node);
                }
            }
            return result;
        }

        internal Node GetClosestNode((int x, int y) gridPosition)
        {
            Node result = null;
            double resultDistance = double.MaxValue; 
            foreach (var node in Nodes)
            {
                if (node == null) continue;
                var distance = Distance(node.Position, gridPosition);
                if (distance < resultDistance)
                {
                    result = node;
                    resultDistance = distance;
                }
            }
            return result;
        }

        internal double Distance(Node start, Node end)
        {
            return Distance(start.Position, end.Position);
        }

        internal double Distance((int x, int y) nodePosition, (int x, int y) gridPositon)
        {
            return Math.Sqrt(
                Math.Pow(gridPositon.x - nodePosition.x, 2) +
                Math.Pow(gridPositon.y - nodePosition.y, 2)
            );
        }
    }
}
