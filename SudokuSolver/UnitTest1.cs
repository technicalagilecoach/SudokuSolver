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
            const string easyPuzzle = " 7 583 2 " +
                                      " 592  3  " +
                                      "34   65 7" +
                                      "795   632" +
                                      "  36971  " +
                                      "68   27  " +
                                      "914835 76" +
                                      " 3 7 1495" +
                                      "567429 13";

            const string expectedSolution = "176583924" +
                                            "859274361" +
                                            "342916587" +
                                            "795148632" +
                                            "423697158" +
                                            "681352749" +
                                            "914835276" +
                                            "238761495" +
                                            "567429813";

            var result = Solve(easyPuzzle);
            Assert.IsFalse(result.Contains(' '));
            Assert.AreEqual(expectedSolution, result);
        }



        [TestMethod]
        public void SolveMediumPuzzle()
        {
            //https://sudoku-puzzles.net/sudoku-medium/620/
            const string mediumPuzzle = " 92 3 1 4" +
                                        " 4   1  9" +
                                        "  8 947  " +
                                        "3 96  8 1" +
                                        "87 3  69 " +
                                        " 1   8  7" +
                                        "     3 1 " +
                                        "2  78 435" +
                                        "  51     ";

            const string expectedSolution = "792536184"+
                                            "643871529"+
                                            "158294763"+
                                            "329647851"+
                                            "874315692"+
                                            "516928347"+
                                            "987453216"+
                                            "261789435"+
                                            "435162978";

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
            const string hardPuzzle = "   9 2  7"
                                      + "  467 8  "
                                      + " 3  8 5  "
                                      + "   853 1 "
                                      + "84   72  "
                                      + "  1    7 "
                                      + "  9 2    "
                                      + " 75 6  2 "
                                      + "2  5   8 ";
            var result = Solve(hardPuzzle);
            Assert.IsFalse(result.Contains(' '));
        }
    }
}