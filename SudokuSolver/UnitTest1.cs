using SudokuSolverCore;

namespace SudokuSolver
{
    [TestClass]
    public class SolveSudokuPuzzle
    {
        private static string Solve(string puzzle)
        {
            SudokuPuzzle solver = new SudokuPuzzle();
            solver.Init(puzzle);
            solver.Solve();
            return solver.Print();
        }

        [TestMethod]
        public void SolveEasyPuzzle()
        {
            //https://sudoku-puzzles.net/sudoku-easy
            String easyPuzzle =
                                 " 7 583 2 "
                                + " 592  3  "
                                + "34   65 7"
                                + "795   632"
                                + "  36971  "
                                + "68   27  "
                                + "914835 76"
                                + " 3 7 1495"
                                + "567429 13";
            string result = Solve(easyPuzzle);
            Assert.IsFalse(result.Contains(' '));
        }



        [TestMethod]
        public void SolveMediumPuzzle()
        {
            //https://sudoku-puzzles.net/sudoku-medium/620/
            String mediumPuzzle =
                                  " 92 3 1 4"
                                + " 4   1  9"
                                + "  8 947  "
                                + "3 96  8 1"
                                + "87 3  69 "
                                + " 1   8  7"
                                + "     3 1 "
                                + "2  78 435"
                                + "  51     ";
            string result = Solve(mediumPuzzle);
            Assert.IsFalse(result.Contains(' '));
        }

        [TestMethod]
        public void SolveHardPuzzle()
        {
            //https://sudoku-puzzles.net/sudoku-hard/580/
            //no unique solution?
            String hardPuzzle =
                                  "   9 2  7"
                                + "  467 8  "
                                + " 3  8 5  "
                                + "   853 1 "
                                + "84   72  "
                                + "  1    7 "
                                + "  9 2    "
                                + " 75 6  2 "
                                + "2  5   8 ";
            string result = Solve(hardPuzzle);
            Assert.IsFalse(result.Contains(' '));
        }
    }
}