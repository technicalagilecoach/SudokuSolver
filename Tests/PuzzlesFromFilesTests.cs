using SudokuSolver;

namespace SudokuSolverTests;

[TestClass]
public class PuzzlesFromFilesTests
{
    [TestMethod]
    [Ignore]
    public void SolvePuzzlesFromFile()
    {
        //const string filename = "/home/armin/src/SudokuSolver/SudokuSolverTests/puzzles/0096_sudoku.txt";
        //const string filename = "/home/armin/src/SudokuSolver/SudokuSolverTests/puzzles/Just17.txt";
        const string filename = "/home/tac/src/SudokuSolver/SudokuSolverTests/puzzles/top50000.txt";

        var allPuzzles = Input.ReadPuzzlesFromFile(filename, out var puzzleNames);
        List<bool> solvedPuzzles;
        SolverWrapper solver = new SolverWrapper(".");
        var numberOfUnsolvedPuzzles = solver.SolveMultiplePuzzles(allPuzzles,out var results, out solvedPuzzles); //check undefined symbol
        Console.WriteLine(numberOfUnsolvedPuzzles + " of " + allPuzzles.Count + " puzzles have not been solved.");
        Assert.AreEqual(0, numberOfUnsolvedPuzzles);
    }
}