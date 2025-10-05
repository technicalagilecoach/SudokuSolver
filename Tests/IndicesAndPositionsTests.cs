using SudokuSolver;

namespace SudokuSolverTests;

[TestClass]
public class IndicesAndPositionsTests
{
    [TestMethod]
    [DataRow(1, 1, 1)]
    [DataRow(2, 4, 2)]
    [DataRow(8, 8, 9)]
    public void GetBoxIndexTest(int row, int column, int expectedBoxIndex)
    {
        var result = IndicesAndIterators.GetBoxIndex(new Position(row, column));
        Assert.AreEqual(expectedBoxIndex, result);
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
    public void GetBoxCoordinatesTest(int box, int expectedRow, int expectedColumn)
    {
        var result = IndicesAndIterators.GetBoxCoordinates(box);

        Assert.AreEqual(expectedRow, result.Row);
        Assert.AreEqual(expectedColumn, result.Column);
    }

    [TestMethod]
    [DataRow(1, 1, 1)]
    [DataRow(3, 9, 3)]
    [DataRow(6, 6, 5)]
    [DataRow(9, 9, 9)]
    public void GetIndicesForBoxTest(int row, int column, int expectedBox)
    {
        Position position = new Position(row, column);
        var boxIndex = IndicesAndIterators.GetBoxIndex(position);
        Assert.AreEqual(expectedBox, boxIndex);
    }

    [TestMethod]
    public void IsDisjointFromTest()
    {
        var cell = new Position(0, 0);
        var pairOfCells = (new Position(0, 0), new Position(0, 1));
        
        var isDisjoint = Position.IsDisjointFrom(cell, pairOfCells);
        
        Assert.IsFalse(isDisjoint);
    }

    [TestMethod]
    public void GetIndicesForDistinctPairsTest()
    {
        IndicesAndIterators.GetIndicesForDistinctPairs(0, 0);
        var indices4 = IndicesAndIterators.GetIndicesForDistinctPairs(0, 4);

        Assert.IsTrue(indices4[0].Item1.Row == 4);
    }
    
    [TestMethod]
    public void GenerateCombinationsTest()
    {
        List<int> list = [ 1, 2, 3, 4, 5 ];
        var result2 = IndicesAndIterators.Combinations<int>(list, 2);
        var result3 = IndicesAndIterators.Combinations<int>(list, 3);
        var result4 = IndicesAndIterators.Combinations<int>(list, 4);

        var positions = IndicesAndIterators.GetIndicesForRow(0);
        var result5 = IndicesAndIterators.Combinations<Position>(positions, 2);
    }
}