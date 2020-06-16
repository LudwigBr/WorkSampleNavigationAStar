using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pot.Navigation.Nodes
{
    public class Connection
    {
        public Node Start { get; }
        public Node End { get; }
        public int Length { get; }

        public Connection(Node start, Node end)
        {
            Start = start ?? throw new ArgumentNullException(nameof(start));
            End = end ?? throw new ArgumentNullException(nameof(end));
            Length = (int) Math.Round(CalculateLength());
        }

        private double CalculateLength()
        {
            var x = Math.Pow(End.Position.x - Start.Position.x, 2);
            var y = Math.Pow(End.Position.y - Start.Position.y, 2);
            return Math.Sqrt(x + y);
        }

        public Node OtherNode(Node node)
        {
            if (node == Start) return End;
            if (node == End) return Start;
            return null;
        }
    }
}
