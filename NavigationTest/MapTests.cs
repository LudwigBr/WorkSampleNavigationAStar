using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pot.Navigation;

namespace PotTest
{
    [TestClass]
    public class MapTests
    {
        [TestMethod]
        public void ChangeHalfByteToZeroFirstHalf()
        {
            byte byteToChange = 0x55;
            bool firstHalf = true;

            var result = ExtractMap.ChangeHalfByteToZero(byteToChange, firstHalf);

            byte expectedResult = 0x05;
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void ChangeHalfByteToZeroSecondHalf()
        {
            byte byteToChange = 0x55;
            bool firstHalf = false;

            var result = ExtractMap.ChangeHalfByteToZero(byteToChange, firstHalf);

            byte expectedResult = 0x50;
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void ChangeHalfByteToZeroUnchanged()
        {
            byte byteToChange = 0x00;
            bool firstHalf = true;

            var result = ExtractMap.ChangeHalfByteToZero(byteToChange, firstHalf);

            byte expectedResult = 0x00;
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void IsWalkableTrue()
        {
            byte[] fullMap = new byte[] {
                0x55,
            }; 
            int bytesPerRow = 1;

            Assert.AreEqual(true, ExtractMap.IsWalkable(fullMap, bytesPerRow, 0, 0));
            Assert.AreEqual(true, ExtractMap.IsWalkable(fullMap, bytesPerRow, 1, 0));
        }

        [TestMethod]
        public void IsWalkableFalse()
        {
            byte[] fullMap = new byte[] {
                0x00,
            };
            int bytesPerRow = 1;

            Assert.AreEqual(false, ExtractMap.IsWalkable(fullMap, bytesPerRow, 0, 0));
            Assert.AreEqual(false, ExtractMap.IsWalkable(fullMap, bytesPerRow, 1, 0));
        }

        [TestMethod]
        public void IsWalkableHalfByteFirst()
        {
            byte[] fullMap = new byte[] {
                0x50,
            };
            int bytesPerRow = 1;

            Assert.AreEqual(true, ExtractMap.IsWalkable(fullMap, bytesPerRow, 0, 0));
            Assert.AreEqual(false, ExtractMap.IsWalkable(fullMap, bytesPerRow, 1, 0));
        }

        [TestMethod]
        public void IsWalkableHalfByteLast()
        {
            byte[] fullMap = new byte[] {
                0x05,
            };
            int bytesPerRow = 1;

            Assert.AreEqual(false, ExtractMap.IsWalkable(fullMap, bytesPerRow, 0, 0));
            Assert.AreEqual(true, ExtractMap.IsWalkable(fullMap, bytesPerRow, 1, 0));
        }

        [TestMethod]
        public void IsWalkableBigger()
        {
            byte[] fullMap = new byte[] {
                0x15, 0x11, 0x15,
                0x51, 0x55, 0x55,
            };
            int bytesPerRow = 3;

            Assert.AreEqual(false, ExtractMap.IsWalkable(fullMap, bytesPerRow, 0, 0));
            Assert.AreEqual(true, ExtractMap.IsWalkable(fullMap, bytesPerRow, 1, 0));
            Assert.AreEqual(false, ExtractMap.IsWalkable(fullMap, bytesPerRow, 2, 0));
            Assert.AreEqual(false, ExtractMap.IsWalkable(fullMap, bytesPerRow, 3, 0));
            Assert.AreEqual(false, ExtractMap.IsWalkable(fullMap, bytesPerRow, 4, 0));
            Assert.AreEqual(true, ExtractMap.IsWalkable(fullMap, bytesPerRow, 5, 0));
            Assert.AreEqual(true, ExtractMap.IsWalkable(fullMap, bytesPerRow, 0, 1));
            Assert.AreEqual(false, ExtractMap.IsWalkable(fullMap, bytesPerRow, 1, 1));
            Assert.AreEqual(true, ExtractMap.IsWalkable(fullMap, bytesPerRow, 2, 1));
            Assert.AreEqual(true, ExtractMap.IsWalkable(fullMap, bytesPerRow, 3, 1));
            Assert.AreEqual(true, ExtractMap.IsWalkable(fullMap, bytesPerRow, 4, 1));
            Assert.AreEqual(true, ExtractMap.IsWalkable(fullMap, bytesPerRow, 5, 1));
        }


        [TestMethod]
        public void ReduceMapToReachableTilesNoChange()
        {
            byte[] input = new byte[] {
                0x55, 0x55,
                0x55, 0x00,
            };
            int bytesPerRow = 2;
            (int x, int y) startPosition = (0, 0);

            var result = ExtractMap.ReduceMapToReachableTiles(input, bytesPerRow, startPosition);

            byte[] expectedResult = new byte[] {
                0x55, 0x55,
                0x55, 0x00,
            };
            CollectionAssert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void ReduceMapToReachableTilesHalfByte()
        {
            byte[] input = new byte[] {
                0x55, 0x05, 
                0x00, 0x05, 
            };
            int bytesPerRow = 2;
            (int x, int y) startPosition = (0, 0);

            var result = ExtractMap.ReduceMapToReachableTiles(input, bytesPerRow, startPosition);

            byte[] expectedResult = new byte[] {
                0x55, 0x00,
                0x00, 0x00,
            };
            CollectionAssert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void ReduceMapToReachableTilesDifferentStart()
        {
            byte[] input = new byte[] {
                0x55, 0x55,
                0x00, 0x00,
                0x55, 0x55
            };
            int bytesPerRow = 2;
            (int x, int y) startPosition = (3, 2);

            var result = ExtractMap.ReduceMapToReachableTiles(input, bytesPerRow, startPosition);

            byte[] expectedResult = new byte[] {
                0x00, 0x00,
                0x00, 0x00,
                0x55, 0x55
            };
            CollectionAssert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void ReduceMapToReachableTilesDifferentStartHalfByteConnected()
        {
            byte[] input = new byte[] {
                0x55, 0x55,
                0x50, 0x00,
                0x55, 0x55
            };
            int bytesPerRow = 2;
            (int x, int y) startPosition = (3, 2);

            var result = ExtractMap.ReduceMapToReachableTiles(input, bytesPerRow, startPosition);

            byte[] expectedResult = new byte[] {
                0x55, 0x55,
                0x50, 0x00,
                0x55, 0x55
            };
            CollectionAssert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void ReduceMapToReachableTilesDiagonal()
        {
            byte[] input = new byte[] {
                0x55, 0x00,
                0x00, 0x55,
                0x55, 0x55
            };
            int bytesPerRow = 2;
            (int x, int y) startPosition = (0, 0);

            var result = ExtractMap.ReduceMapToReachableTiles(input, bytesPerRow, startPosition);

            byte[] expectedResult = new byte[] {
                0x55, 0x00,
                0x00, 0x55,
                0x55, 0x55
            };
            CollectionAssert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void ReduceMapToReachableTilesStartInMiddle()
        {
            byte[] input = new byte[] {
                0x55, 0x55, 0x55, 0x55, 0x55, 0x55,
                0x55, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x55, 0x00, 0x55, 0x00, 0x55, 0x55,
                0x55, 0x00, 0x55, 0x00, 0x55, 0x55,
                0x55, 0x00, 0x55, 0x55, 0x00, 0x55,
                0x55, 0x00, 0x55, 0x00, 0x55, 0x55,
                0x55, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x55, 0x55, 0x55, 0x55, 0x55, 0x55,

            };
            int bytesPerRow = 6;
            (int x, int y) startPosition = (7, 4);

            var result = ExtractMap.ReduceMapToReachableTiles(input, bytesPerRow, startPosition);

            byte[] expectedResult = new byte[] {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x55, 0x00, 0x55, 0x55,
                0x00, 0x00, 0x55, 0x00, 0x55, 0x55,
                0x00, 0x00, 0x55, 0x55, 0x00, 0x55,
                0x00, 0x00, 0x55, 0x00, 0x55, 0x55,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            };
            CollectionAssert.AreEqual(expectedResult, result);
        }
    }
}
