using SudokuSolverCore;
    
using static SudokuSolverCore.Grid;

namespace SudokuSolverTests
{
    [TestClass]
    public class ConversionTests
    {
        [TestMethod]
        [DataRow(1, 1, 1)]
        [DataRow(2, 4, 2)]
        [DataRow(8, 8, 9)]
        public void RegionIndexForCoordinatesTest(int row, int column, int expectedRegionIndex)
        {
            var result = GetRegionIndex(row, column);
            Assert.AreEqual(expectedRegionIndex, result);
        }
        
        [TestMethod]
        [DataRow(1, 1, 1)]
        [DataRow(2, 1, 4)]
        [DataRow(3, 1, 7)]
        [DataRow(4, 4, 1)]
        [DataRow(5, 4, 4)]
        [DataRow(6, 4, 7)]
        [DataRow(7, 7, 1)]
        [DataRow(8, 7, 4)]
        [DataRow(9, 7, 7)]
        public void CoordinatesForRegionIndexTest(int region, int expectedRow, int expectedColumn)
        {
            var result = GetRegionCoordinates(region);

            Assert.AreEqual(expectedRow, result.row);
            Assert.AreEqual(expectedColumn, result.column);
        }
    }
}