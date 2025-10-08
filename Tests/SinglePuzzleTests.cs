using SudokuSolver;

namespace SudokuSolverTests;

[TestClass]
public class SolveSudokuPuzzle
{
    private static void CheckForSolvability(string puzzle)
    {
        puzzle = puzzle.Replace("\n", "");
        SolverWrapper.Solve(puzzle, " ", out var unsolvedCells, out var strategyStats);
        Assert.IsTrue(unsolvedCells==0);
    }

    [TestMethod]
    public void SolveGrid01()
    {
        // Grid 01 from https://projecteuler.info/resources/documents/0096_sudoku.txt
        const string puzzle = "003020600\n"
                            + "900305001\n"
                            + "001806400\n"
                            + "008102900\n"
                            + "700000008\n"
                            + "006708200\n"
                            + "002609500\n"
                            + "800203009\n"
                            + "005010300\n";
        
        // NakedSingles
        CheckForSolvability(puzzle);
    }
    
    [TestMethod]
    public void SolveGrid03()
    {
        // Grid 03 from https://projecteuler.info/resources/documents/0096_sudoku.txt
        const string puzzle = "000000907\n"
                            + "000420180\n"
                            + "000705026\n"
                            + "100904000\n"
                            + "050000040\n"
                            + "000507009\n"
                            + "920108000\n"
                            + "034059000\n"
                            + "507000000\n";
       
        // NakedSingles, HiddenSingles
        CheckForSolvability(puzzle);
    }

    [TestMethod]
    public void SolveGrid06()
    {
        // Grid 06 from https://projecteuler.info/resources/documents/0096_sudoku.txt
        const string puzzle = "100920000\n"
                            + "524010000\n"
                            + "000000070\n"
                            + "050008102\n"
                            + "000000000\n"
                            + "402700090\n"
                            + "060000000\n"
                            + "000030945\n"
                            + "000071006\n";

        // NakedSingles, HiddenSingles, NakedPairs, HiddenPairs, PointingTuples
        CheckForSolvability(puzzle);
    }

    [TestMethod]
    public void SolveGrid42()
    {
        // Grid 42 from https://projecteuler.info/resources/documents/0096_sudoku.txt
        const string puzzle = "380000000\n"
                            + "000400785\n"
                            + "009020300\n"
                            + "060090000\n"
                            + "800302009\n"
                            + "000040070\n"
                            + "001070500\n"
                            + "495006000\n"
                            + "000000092\n";

        // NakedSingles, HiddenSingles, NakedPairs, NakedTriplets, PointingTuples
        CheckForSolvability(puzzle);
    }
    
    [TestMethod]
    public void SolveGrid07()
    {
        // Grid 07 from https://projecteuler.info/resources/documents/0096_sudoku.txt
        const string puzzle =   "043080250\n"
                              + "600000000\n"
                              + "000001094\n"
                              + "900004070\n"
                              + "000608000\n"
                              + "010200003\n"
                              + "820500000\n"
                              + "000000005\n"
                              + "034090710\n";

        // NakedSingles, HiddenSingles, NakedPairs, HiddenPairs, Pointing Tuples, BoxLineReduction, XWing
        CheckForSolvability(puzzle);
    }
}