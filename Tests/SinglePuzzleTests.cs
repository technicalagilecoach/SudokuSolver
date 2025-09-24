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
    public void SolveEasyPuzzle()
    {
        //https://sudoku-puzzles.net/sudoku-easy
        //unit propagation is sufficient for solution
        const string puzzle = " 7 583 2 \n" +
                              " 592  3  \n" +
                              "34   65 7\n" +
                              "795   632\n" +
                              "  36971  \n" +
                              "68   27  \n" +
                              "914835 76\n" +
                              " 3 7 1495\n" +
                              "567429 13\n";
        
        CheckForSolvability(puzzle);
    }

    [TestMethod]
    public void SolveMediumPuzzle()
    {
        //https://sudoku-puzzles.net/sudoku-medium/620/
        //unit propagation is sufficient for solution
        const string puzzle = " 92 3 1 4\n" +
                              " 4   1  9\n" +
                              "  8 947  \n" +
                              "3 96  8 1\n" +
                              "87 3  69 \n" +
                              " 1   8  7\n" +
                              "     3 1 \n" +
                              "2  78 435\n" +
                              "  51     \n";

        CheckForSolvability(puzzle);
    }

    [TestMethod]
    public void SolveHardPuzzle()
    {
        //https://sudoku-puzzles.net/sudoku-hard/580/
        //double pairs needed for solution
        const string puzzle =     "   9 2  7\n"
                                + "  467 8  \n"
                                + " 3  8 5  \n"
                                + "   853 1 \n"
                                + "84   72  \n"
                                + "  1    7 \n"
                                + "  9 2    \n"
                                + " 75 6  2 \n"
                                + "2  5   8 \n";
           
        CheckForSolvability(puzzle);
    }

    [TestMethod]
    public void SolveExtremePuzzle()
    {
        //puzzle from https://sudoku.com/extreme/
        const string puzzle =   " 62 1  8 \n"
                              + "       13\n"
                              + "  1 94   \n"
                              + "5 74    2\n"
                              + "9   81   \n"
                              + "   7    6\n"
                              + "       6 \n"
                              + "     84  \n"
                              + "  5 2    \n";

        CheckForSolvability(puzzle);
    }
 
    [TestMethod]
    [Ignore]
    public void SolveAIEscargot2006Puzzle()
    {
        const string puzzle =   "1    7 9 \n"
                              + " 3  2   8\n"
                              + "  96  5  \n"
                              + "  53  9  \n"
                              + " 1  8   2\n"
                              + "6    4   \n"
                              + "3      1 \n"
                              + " 4      7\n"
                              + "  7   3  \n";

        CheckForSolvability(puzzle);
    }
        
    [TestMethod]
    [Ignore]
    public void SolveArtoInkala2012Puzzle()
    {
        const string puzzle = "  53     \n"
                              + "8      2 \n"
                              + " 7  1 5  \n"
                              + "4    53  \n"
                              + " 1  7   6\n"
                              + "  32   8 \n"
                              + " 6 5    9\n"
                              + "  4    3 \n"
                              + "     97  \n";

        CheckForSolvability(puzzle);
    }

    [TestMethod]
    public void SolveGrid03()
    {
        const string puzzle =   "000000907\n"
                              + "000420180\n"
                              + "000705026\n"
                              + "100904000\n"
                              + "050000040\n"
                              + "000507009\n"
                              + "920108000\n"
                              + "034059000\n"
                              + "507000000\n";
       
        CheckForSolvability(puzzle);
    }
    
    [TestMethod]
    [Ignore]
    public void SolveGrid07()
    {
        const string puzzle =   "043080250\n"
                              + "600000000\n"
                              + "000001094\n"
                              + "900004070\n"
                              + "000608000\n"
                              + "010200003\n"
                              + "820500000\n"
                              + "000000005\n"
                              + "034090710\n";

        CheckForSolvability(puzzle);
    }
}