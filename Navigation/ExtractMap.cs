using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pot.Navigation
{
    public static class ExtractMap
    {
        public static byte[] ReduceMapToReachableTiles(byte[] fullMap, int bytesPerRow, (int c, int r) startPosition)
        {
            int rows = fullMap.Length / bytesPerRow;
            int columns = fullMap.Length / rows * 2;
            bool[,] tileChecked = new bool[columns, rows];

            Stack<(int c, int r)> tilesToCheck = new Stack<(int c, int r)>();
            tilesToCheck.Push(startPosition);

            while (tilesToCheck.Count > 0)
            {
                var tileToCheck = tilesToCheck.Pop();
                CheckAdjacent(ref tilesToCheck, ref tileChecked, fullMap, columns, rows, tileToCheck);
            }


            LoopMap(fullMap, bytesPerRow, (c, r) =>
            {
                if (tileChecked[c, r]) return;
                var offset = MapOffset(c, r, bytesPerRow);
                // column == even -> change first 4 bit, odd -> change last 4 bit
                fullMap[offset] = ChangeHalfByteToZero(fullMap[offset], (c & 1) == 0);
            });
            return fullMap;
        }

        public static void LoopMap (byte[] fullMap, int bytesPerRow, Action<int, int> action, int columnStepSize = 1, int rowStepSize = 1)
        {
            int rows = fullMap.Length / bytesPerRow;
            int columns = fullMap.Length / rows * 2;

            for (var c = 0; c < columns; c += columnStepSize)
            {
                for (var r = 0; r < rows; r += rowStepSize)
                {
                    action.Invoke(c, r);
                }
            }
        }

        internal static void CheckAdjacent(ref Stack<(int c, int r)> tilesToCheck, ref bool[,] tileChecked, byte[] fullMap, int columns, int rows, (int c, int r) position)
        {
            if (tileChecked[position.c, position.r]) return;
            if (!IsWalkable(fullMap, columns / 2, (long)position.c, (long)position.r)) return;
            tileChecked[position.c, position.r] = true;

            // check all adjacent fields if they exist
            if (position.c > 0)                
                tilesToCheck.Push((position.c - 1, position.r));
            if (position.c < columns - 1)
                tilesToCheck.Push((position.c + 1, position.r));
            if (position.r > 0)
                tilesToCheck.Push((position.c, position.r - 1));
            if (position.r < rows - 1)
                tilesToCheck.Push((position.c, position.r + 1));
            if (position.c > 0 && position.r > 0)
                tilesToCheck.Push((position.c - 1, position.r - 1));
            if (position.c < columns - 1 && position.r < rows - 1)
                tilesToCheck.Push((position.c + 1, position.r + 1));
            if (position.c > 0 && position.r < rows - 1)
                tilesToCheck.Push((position.c - 1, position.r + 1));
            if (position.c < columns - 1 && position.r > 0)
                tilesToCheck.Push((position.c + 1, position.r - 1));
        }
       

        public static byte ChangeHalfByteToZero(byte b, bool firstHalf)
        {
            if (firstHalf)
            {
                return b &= 0x0F;
            }
            else
            {
                return b &= 0xF0;
            }
        }


        public static bool IsWalkable(byte[] fullMap, int bytesPerRow, long c, long r)
        {
            var byteValue = GetWalkableStateByte(fullMap, bytesPerRow, c, r);
            return (int)byteValue >= 2;
        }

        public static byte GetWalkableStateByte(byte[] fullMap, int bytesPerRow, long c, long r)
        {
            var offset = MapOffset((int)c, (int)r, bytesPerRow);
            if (offset < 0 || offset >= fullMap.Length)
            {
                return 0x0;
                throw new Exception(string.Format($"WalkableValue failed: ({c}, {r}) [{bytesPerRow}] => {offset}"));
            }

            byte b;
            if ((c & 1) == 0)
            {
                // even
                // take first 4 bit
                b = (byte)(fullMap[offset] >> 4);

            }
            else
            {
                // odd
                // take last 4 bit
                b = (byte)(fullMap[offset] & 0xF);
            }
            return b;
        }
        
        public static int MapOffset(int column, int row, int bytesPerRow)
        {
            return row * bytesPerRow + column / 2;
        }
    }
}
