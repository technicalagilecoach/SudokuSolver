using SudokuSolver;

namespace SudokuSolverTests;

[TestClass]
public class IndicesAndPositionsTests
{
    [TestMethod]
    [DataRow(1, 1, 0)]
    [DataRow(2, 4, 1)]
    [DataRow(8, 8, 8)]
    public void GetBoxIndexTest(int row, int column, int expectedBoxIndex)
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
    public void GetBoxCoordinatesTest(int box, int expectedRow, int expectedColumn)
    {
        var result = IndicesAndIterators.GetBoxCoordinates(box);

        Assert.AreEqual(expectedRow, result.Row);
        Assert.AreEqual(expectedColumn, result.Column);
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
        var result2 = NakedTuples.Combinations<int>(list, 2);
        var result3 = NakedTuples.Combinations<int>(list, 3);
        var result4 = NakedTuples.Combinations<int>(list, 4);

        var positions = IndicesAndIterators.GetIndicesForRow(0);
        var result5 = NakedTuples.Combinations<Position>(positions, 2);
    }
}