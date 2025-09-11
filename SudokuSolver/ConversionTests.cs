using SudokuSolverCore;

namespace SudokuSolverTests;

[TestClass]
public class ConversionTests
{
    [TestMethod]
    [DataRow(1, 1, 0)]
    [DataRow(2, 4, 1)]
    [DataRow(8, 8, 8)]
    public void BoxIndexForCoordinatesTest(int row, int column, int expectedBoxIndex)
    {
        var result = IndicesAndIterators.GetBoxIndex(new Position(row, column));
        Assert.AreEqual(expectedBoxIndex, result);
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
    public void CoordinatesForBoxIndexTest(int box, int expectedRow, int expectedColumn)
    {
        var result = IndicesAndIterators.GetBoxCoordinates(box);

        Assert.AreEqual(expectedRow, result.Row);
        Assert.AreEqual(expectedColumn, result.Column);
    }
}