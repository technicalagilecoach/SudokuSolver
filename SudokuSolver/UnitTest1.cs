using SudokuSolverCore;

namespace SudokuSolverTests
{
    [TestClass]
    public class SolveSudokuPuzzle
    {
        private static string Solve(string puzzle)
        {
            Grid sudokuPuzzle = new();
            sudokuPuzzle.Init(puzzle);
            Solver solver = new(sudokuPuzzle);
            solver.Solve();
            return sudokuPuzzle.Print();
        }

        [TestMethod]
        public void SolveEasyPuzzle()
        {
            //https://sudoku-puzzles.net/sudoku-easy
            const string easyPuzzle = " 7 583 2 \n" +
                                      " 592  3  \n" +
                                      "34   65 7\n" +
                                      "795   632\n" +
                                      "  36971  \n" +
                                      "68   27  \n" +
                                      "914835 76\n" +
                                      " 3 7 1495\n" +
                                      "567429 13\n";

            const string expectedSolution = "176583924\n" +
                                            "859274361\n" +
                                            "342916587\n" +
                                            "795148632\n" +
                                            "423697158\n" +
                                            "681352749\n" +
                                            "914835276\n" +
                                            "238761495\n" +
                                            "567429813\n";

            var result = Solve(easyPuzzle);
            Assert.IsFalse(result.Contains(' '));
            Assert.AreEqual(expectedSolution, result);
        }



        [TestMethod]
        public void SolveMediumPuzzle()
        {
            //https://sudoku-puzzles.net/sudoku-medium/620/
            const string mediumPuzzle = " 92 3 1 4\n" +
                                        " 4   1  9\n" +
                                        "  8 947  \n" +
                                        "3 96  8 1\n" +
                                        "87 3  69 \n" +
                                        " 1   8  7\n" +
                                        "     3 1 \n" +
                                        "2  78 435\n" +
                                        "  51     \n";

            const string expectedSolution = "792536184\n"+
                                            "643871529\n"+
                                            "158294763\n"+
                                            "329647851\n"+
                                            "874315692\n"+
                                            "516928347\n"+
                                            "987453216\n"+
                                            "261789435\n"+
                                            "435162978\n";

            var result = Solve(mediumPuzzle);
            Assert.IsFalse(result.Contains(' '));
            Assert.AreEqual(expectedSolution, result);
        }

        [TestMethod]
        [Ignore]
        public void SolveHardPuzzle()
        {
            //https://sudoku-puzzles.net/sudoku-hard/580/
            //no unique solution?
            const string hardPuzzle =   "   9 2  7\n"
                                      + "  467 8  \n"
                                      + " 3  8 5  \n"
                                      + "   853 1 \n"
                                      + "84   72  \n"
                                      + "  1    7 \n"
                                      + "  9 2    \n"
                                      + " 75 6  2 \n"
                                      + "2  5   8 \n";
            var result = Solve(hardPuzzle);
            Assert.IsFalse(result.Contains(' '));
        }
    }
}