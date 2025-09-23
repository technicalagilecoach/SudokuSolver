namespace SudokuSolver;

public static class SolverUtil
{
    public static int SolveMultiplePuzzles(List<string> allPuzzles, out List<string> solutions,
        out List<bool> solvedPuzzles, string undefinedSymbol)
    {
        var numberOfUnsolvedCells = new int[allPuzzles.Count];
        solutions = [];

        for (var index = 0; index < allPuzzles.Count; index++)
        {
            var result = SolveOnePuzzle(allPuzzles, index, out var count, undefinedSymbol);
            numberOfUnsolvedCells[index] = count;
            solutions.Add(result);
        }
        
        var numberOfUnsolvedPuzzles = numberOfUnsolvedCells.Count(c => c!=0);
        solvedPuzzles = numberOfUnsolvedCells.Select(n => n == 0).ToList();

        return numberOfUnsolvedPuzzles;
    }

    public static string SolveOnePuzzle(List<string> allPuzzles, int index, out int numberOfUnsolvedCells, string undefinedSymbol)
    {
        numberOfUnsolvedCells = 0;
        
        if (index < 0 || index >= allPuzzles.Count) 
            return ""; //ToDo exception handling
        
        var puzzle = allPuzzles[index];
        var result = Solve(puzzle, out var solved, out numberOfUnsolvedCells, undefinedSymbol);

        return result;
    }

    public static string Solve(string puzzle, out bool solved, out int unsolvedCells, string undefinedSymbol)
    {
        var sudokuPuzzle = new Puzzle();
        sudokuPuzzle.Init(puzzle);
        Solver solver = new(sudokuPuzzle);
        solved = solver.Solve();
        
        unsolvedCells = 0;
        if (!solved)
            unsolvedCells = sudokuPuzzle.CountUndefinedCells();
        
        var result = sudokuPuzzle.PrintCells(undefinedSymbol);
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