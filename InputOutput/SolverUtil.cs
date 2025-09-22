namespace SudokuSolver;

public static class SolverUtil
{
    public static int SolveMultiplePuzzles(List<string> allPuzzles, out List<string> solutions)
    {
        var numberOfUnsolvedCells = new int[allPuzzles.Count];
        solutions = [];

        for (var index = 0; index < allPuzzles.Count; index++)
        {
            var puzzle = allPuzzles[index];
            var result = Solve(puzzle, out var solved);

            var count = 0;
            if (!solved)
                count = result.Count(c => c == ' ');

            numberOfUnsolvedCells[index] = count;
            solutions.Add(result);
        }
        
        var numberOfUnsolvedPuzzles = numberOfUnsolvedCells.Count(c => c!=0);
        return numberOfUnsolvedPuzzles;
    }

    public static string Solve(string puzzle, out bool solved)
    {
        var sudokuPuzzle = new Puzzle();
        sudokuPuzzle.Init(puzzle);
        Solver solver = new(sudokuPuzzle);
        solved = solver.Solve();
        var result = sudokuPuzzle.PrintCells();
        return result;
    }


    public static string OneSolverStep(string puzzle, out bool hasBeenChanged)
    {
        var sudokuPuzzle = new Puzzle();
        sudokuPuzzle.Init(puzzle.Replace("\n", ""));
        Solver solver = new(sudokuPuzzle);

        solver.Execute(solver.NakedSingles);

        var result = sudokuPuzzle.PrintCells();
        
        var diff = Puzzle.Difference(puzzle, result);

        hasBeenChanged = false;
        
        return result;
    }
}