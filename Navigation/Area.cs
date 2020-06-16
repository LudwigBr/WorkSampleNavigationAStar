using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pot.Navigation
{
    public class Area
    {
        private int BytesPerRow { get; }
        private (int c, int r) StartGridPosition { get; }
        private int SeenRectangleSideLength => 75;

        private byte[] FullMap { get; }
        private byte[] ReachableMap { get; set; }
        private (int columns, int rows) MapSize { get; set; }
        private bool[,] GridSeen { get; set; }
       /* private Route Route { get; set; }*/

        public Area(byte[] fullMap, int bytesPerRow, (int c, int r) currentGridPosition)
        {
            FullMap = fullMap;
            BytesPerRow = bytesPerRow;
            StartGridPosition = currentGridPosition;
            Initialize();
        }

        private void Initialize()
        {
            var rows = FullMap.Length / BytesPerRow;
            var columns = BytesPerRow * 2;
            MapSize = (columns, rows);

            GridSeen = new bool[columns, rows];

            ReachableMap = ExtractMap.ReduceMapToReachableTiles(FullMap, BytesPerRow, StartGridPosition);
        }


        public void UpdateGridSeen((int x, int y) CurrentGridPos)
        {
            int columnStart = Math.Max(0, (int) CurrentGridPos.x - SeenRectangleSideLength);
            int columnEnd = Math.Min((int) CurrentGridPos.x + SeenRectangleSideLength, MapSize.columns);
            int rowStart = Math.Max(0, (int) CurrentGridPos.y - SeenRectangleSideLength);
            int rowEnd = Math.Min((int) CurrentGridPos.y + SeenRectangleSideLength, MapSize.rows);
            for (var column = columnStart; column < columnEnd; column++)
            {
                for (var row = rowStart; row < rowEnd; row++)
                {
                    GridSeen[column, row] = true;
                }
            }
        }


        public Bitmap ToBitmap()
        {
            var bitmap = new Bitmap(MapSize.columns, MapSize.rows);

            for (var r = 0; r < MapSize.rows; r++)
            {
                for (var c = 0; c < MapSize.columns; c++)
                {
                    bitmap.SetPixel(c, r, GetColor(c, r));
                }
            }

            return bitmap;
        }

        private Color GetColor(int c, int r)
        {
            if (StartGridPosition.c == c && StartGridPosition.r == r) 
                return Color.Red;

/*            if (Route != null && Route.IsWaypoint(c, r)) 
                return Color.Blue;*/

            var isWalkable = ExtractMap.IsWalkable(ReachableMap, BytesPerRow, c, r);
            if (isWalkable)
            {
                if (GridSeen[c, r]) 
                    return Color.Orange;
                else
                    return Color.Green;
            }

            return Color.Transparent;
        }

        public Area FromBitmap()
        {
            throw new NotImplementedException();
        }
    }
}
