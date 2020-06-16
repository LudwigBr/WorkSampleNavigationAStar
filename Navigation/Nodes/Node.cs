using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pot.Navigation.Nodes
{
    public enum NodeState
    {
        Open,
        Closed,
        Untested
    }

    public class Node : FastPriorityQueueNode
    {
        public (int x, int y) Position { get; }
        public List<Connection> Connections { get; }

        public int LengthFromStart { get; set; }
        public Node PreviousNode { get; set; }
        public NodeState State { get; set; }

        public Node((int x, int y) position)
        {
            Position = position;
            Connections = new List<Connection>();
            State = NodeState.Untested;
        }
        public Node(int x, int y) : this((x, y)) { }

        public void AddConnection(Node target)
        {
            var connection = new Connection(this, target);
            Connections.Add(connection);
            target.Connections.Add(connection);
        }

        public Stack<Node> ToPreviousNodesStack()
        {
            var result = new Stack<Node>();
            var node = this;
            while (node != null)
            {
                result.Push(node);
                node = node.PreviousNode;
            }
            return result;
        }
    }
}
