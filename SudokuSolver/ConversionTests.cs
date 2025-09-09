using SudokuSolverCore;

namespace SudokuSolverTests;

[TestClass]
public class ConversionTests
{
    [TestMethod]
    [DataRow(1, 1, 0)]
    [DataRow(2, 4, 1)]
    [DataRow(8, 8, 8)]
    public void RegionIndexForCoordinatesTest(int row, int column, int expectedRegionIndex)
    {
        var result = IndicesAndIterators.GetRegionIndex(new Position(row, column));
        Assert.AreEqual(expectedRegionIndex, result);
    }
        
    [TestMethod]
    [DataRow(0, 0, 0)]
    [DataRow(1, 0, 3)]
    [DataRow(2, 0, 6)]
    [DataRow(3, 3, 0)]
    [DataRow(4, 3, 3)]
    [DataRow(5, 3, 6)]
    [DataRow(6, 6, 0)]
    [DataRow(7, 6, 3)]
    [DataRow(8, 6, 6)]
    public void CoordinatesForRegionIndexTest(int region, int expectedRow, int expectedColumn)
    {
        var result = IndicesAndIterators.GetRegionCoordinates(region);

        Assert.AreEqual(expectedRow, result.Row);
        Assert.AreEqual(expectedColumn, result.Column);
    }
}