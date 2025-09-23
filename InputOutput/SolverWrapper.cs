namespace SudokuSolver;

public class SolverWrapper
{
    private string _undefinedSymbol;

    public SolverWrapper(string undefinedSymbol)
    {
        this._undefinedSymbol = undefinedSymbol;
    }

    public int SolveMultiplePuzzles(List<string> allPuzzles, out List<string> solutions,
        out List<bool> solvedPuzzles)
    {
        var numberOfUnsolvedCells = new int[allPuzzles.Count];
        solutions = [];

        for (var index = 0; index < allPuzzles.Count; index++)
        {
            var result = SolveOnePuzzle(allPuzzles, index, out var count);
            numberOfUnsolvedCells[index] = count;
            solutions.Add(result);
        }
        
        var numberOfUnsolvedPuzzles = numberOfUnsolvedCells.Count(c => c!=0);
        solvedPuzzles = numberOfUnsolvedCells.Select(n => n == 0).ToList();

        return numberOfUnsolvedPuzzles;
    }

    public string SolveOnePuzzle(List<string> allPuzzles, int index, out int numberOfUnsolvedCells)
    {
        numberOfUnsolvedCells = 0;
        
        if (index < 0 || index >= allPuzzles.Count) 
            return ""; //ToDo exception handling
        
        var puzzle = allPuzzles[index];
        var result = Solve(puzzle, out var solved, out numberOfUnsolvedCells);

        return result;
    }

    public string Solve(string puzzle, out bool solved, out int unsolvedCells)
    {
        var sudokuPuzzle = new Puzzle();
        sudokuPuzzle.Init(puzzle);
        Solver solver = new(sudokuPuzzle);
        solved = solver.Solve();
        
        unsolvedCells = 0;
        if (!solved)
            unsolvedCells = sudokuPuzzle.CountUndefinedCells();
        
        var result = sudokuPuzzle.PrintCells(_undefinedSymbol);
        return result;
    }
}