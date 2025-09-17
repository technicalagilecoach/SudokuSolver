using SudokuSolverCore;

namespace SudokuSolverTests;

public static class TestHelpers
{
    public static int SolveMultiplePuzzles(List<List<string>> allPuzzles)
    {
        var unsolvedCells = new int[allPuzzles.Count];

        for (var index = 0; index < allPuzzles.Count; index++)
        {
            var puzzle = allPuzzles[index];
            var puzzleAsString = string.Join("", puzzle);
            unsolvedCells[index] = TryToSolve(puzzleAsString);
        }
        
        var numberOfUnsolvedPuzzles = unsolvedCells.Count(c => c!=0);
        return numberOfUnsolvedPuzzles;
    }

    public static string Solve(string puzzle, out bool solved)
    {
        var sudokuPuzzle = new Puzzle();
        sudokuPuzzle.Init(puzzle);
        Solver solver = new(sudokuPuzzle);
        solved = solver.Solve();
        var result = Printers.Print(sudokuPuzzle);
        return result;
    }

    private static int TryToSolve(string puzzle)
    {
        var result = Solve(puzzle, out var solved);

        var count = 0;
        if (!solved)
            count = result.Count(c => c == ' ');

        return count;
    }

    public static List<List<string>> ReadPuzzlesFromFile(string filename)
    {
        var allPuzzles = new List<List<string>>();
        
        using StreamReader reader = new(filename);

        var text = reader.ReadToEnd();
        
        char[] separators = ['\n', '\r'];
        var allLines = text.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToList();

        if (allLines[0].Length == 81)
        {
            foreach (var line in allLines) {
                var puzzle = new List<string>();

                for (var lineIndex = 0; lineIndex < 9; lineIndex++)
                    puzzle.Add(line.Substring(9 * lineIndex, 9));
                
                allPuzzles.Add(puzzle);
            }
        }
        else
        {
            var numOfPuzzles = allLines.Count / 10;

            for (var puzzleIndex = 0; puzzleIndex < numOfPuzzles; puzzleIndex++)
            {
                var offset = puzzleIndex * 10;
                var puzzle = new List<string>();

                for (var lineIndex = offset; lineIndex < 10 + offset; lineIndex++)
                {
                    if (lineIndex % 10 != 0)
                    {
                        puzzle.Add(allLines[lineIndex]);
                    }
                }

                allPuzzles.Add(puzzle);
            }
        }

        return allPuzzles;
    }
}