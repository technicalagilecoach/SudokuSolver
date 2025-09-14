using SudokuSolverCore;

namespace SudokuSolverTests;

[TestClass]
public class SolveSudokuPuzzle
{
    private static string Solve(string puzzle)
    {
        var sudokuPuzzle = new Puzzle();
        sudokuPuzzle.Init(puzzle);
        Solver solver = new(sudokuPuzzle);
        solver.Solve();
        return Printers.Print(sudokuPuzzle);
    }

    [TestMethod]
    public void SolveEasyPuzzle()
    {
        //https://sudoku-puzzles.net/sudoku-easy
        //unit propagation is sufficient for solution
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
        //unit propagation is sufficient for solution
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
    public void SolveHardPuzzle()
    {
        //https://sudoku-puzzles.net/sudoku-hard/580/
        //double pairs needed for solution
        const string puzzle =   "   9 2  7\n"
                                + "  467 8  \n"
                                + " 3  8 5  \n"
                                + "   853 1 \n"
                                + "84   72  \n"
                                + "  1    7 \n"
                                + "  9 2    \n"
                                + " 75 6  2 \n"
                                + "2  5   8 \n";

        const string expectedSolution =   "518932647\n"
                                          + "924675831\n"
                                          + "637481592\n"
                                          + "792853416\n"
                                          + "846197253\n"
                                          + "351246978\n"
                                          + "189724365\n"
                                          + "475368129\n"
                                          + "263519784\n";
            
        var result = Solve(puzzle);
        Assert.IsFalse(result.Contains(' '));
        Assert.AreEqual(expectedSolution, result);
    }

    [TestMethod]
    [Ignore]
    public void SolveExtremePuzzle()
    {
        //puzzle from https://sudoku.com/extreme/
        const string puzzle =         " 62 1  8 \n"
                                      + "       13\n"
                                      + "  1 94   \n"
                                      + "5 74    2\n"
                                      + "9   81   \n"
                                      + "   7    6\n"
                                      + "       6 \n"
                                      + "     84  \n"
                                      + "  5 2    \n";
           
        var result = Solve(puzzle);
        Assert.IsFalse(result.Contains(' '));
    }
        
        
    [TestMethod]
    [Ignore]
    public void SolveAIEscargot2006Puzzle()
    {
        const string puzzle =       "1    7 9 \n"
                                    + " 3  2   8\n"
                                    + "  96  5  \n"
                                    + "  53  9  \n"
                                    + " 1  8   2\n"
                                    + "6    4   \n"
                                    + "3      1 \n"
                                    + " 4      7\n"
                                    + "  7   3  \n";
           
        var result = Solve(puzzle);
        Assert.IsFalse(result.Contains(' '));
    }
        
    [TestMethod]
    [Ignore]
    public void SolveArtoInkala2012Puzzle()
    {
        const string puzzle =     "  53     \n"
                                  + "8      2 \n"
                                  + " 7  1 5  \n"
                                  + "4    53  \n"
                                  + " 1  7   6\n"
                                  + "  32   8 \n"
                                  + " 6 5    9\n"
                                  + "  4    3 \n"
                                  + "     97  \n";
           
        var result = Solve(puzzle);
        Assert.IsFalse(result.Contains(' '));
    }

    [TestMethod]
    //[Ignore]
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
        
        const string expectedSolution =   "462831957\n"
                                        + "795426183\n"
                                        + "381795426\n"
                                        + "173984265\n"
                                        + "659312748\n"
                                        + "248567319\n"
                                        + "926178534\n"
                                        + "834259671\n"
                                        + "517643892\n";
            
        var result = Solve(puzzle);
        Assert.IsFalse(result.Contains(' '));
        Assert.AreEqual(expectedSolution, result);
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
        var result = Solve(puzzle);
        Assert.IsFalse(result.Contains(' '));
    }

    [TestMethod]
    [Ignore]
    public void SolvePuzzlesFromFile()
    {
        const string filename = "/home/armin/src/SudokuSolver/0096_sudoku.txt";
        
        List<List<string>> allPuzzles = ReadPuzzlesFromFile(filename);

        var solvablePuzzles = new int[allPuzzles.Count];
        
        for (var index = 0; index < allPuzzles.Count; index++)
        {
            var puzzle = allPuzzles[index];
            var puzzleAsString = string.Join("\n", puzzle);
            var result = Solve(puzzleAsString);

            var count = 0;
            foreach (var c in result) {
                if (c==' ')
                    count++;
            }

            solvablePuzzles[index] = count; //!result.Contains(' ');
        }
    }

    private static List<List<string>> ReadPuzzlesFromFile(string filename)
    {
        List<List<string>> allPuzzles = new List<List<string>>();
        
        using StreamReader reader = new(filename);

        string text = reader.ReadToEnd();
        List<string> allLines = text.Split("\n").ToList();
        
        int numOfPuzzles = allLines.Count/10;

        for (int i = 0; i < numOfPuzzles; i++)
        {
            int offset = i*10;
            List<string> puzzle = new List<string>();
            
            for (int j = offset; j < 10+offset; j++)
            {
                if (j%10!=0) {
                    puzzle.Add(allLines[j]);
                }
            }
            
            allPuzzles.Add(puzzle);
        }

        return allPuzzles;
    }
}