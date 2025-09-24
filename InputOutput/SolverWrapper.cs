namespace SudokuSolver;

public class SolverWrapper(string undefinedSymbol, Input.FileType fileType)
{
    private string UndefinedSymbol { get; } = undefinedSymbol;
    private Input.FileType FileType { get; } = fileType;
    
    public List<string> SolvePuzzles(int number, List<string> allPuzzles, 
        ref string output, ref List<bool> solvedPuzzles)
    {
        List<string> results;
        if (FileType == Input.FileType.SinglePuzzle || number > 0)
            results = SolveOnePuzzle(allPuzzles, number, ref output);
        else
            results = SolveMultiplePuzzles(allPuzzles, ref output, out solvedPuzzles);
        return results;
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

    public string Solve(string puzzle, out bool solved, out int numberOfUnsolvedCells)
    {
        var sudokuPuzzle = new Puzzle();
        sudokuPuzzle.Init(puzzle);
        Solver solver = new(sudokuPuzzle);
        solved = solver.Solve();
        
        numberOfUnsolvedCells = 0;
        if (!solved)
            numberOfUnsolvedCells = sudokuPuzzle.CountUndefinedCells();
        
        var result = sudokuPuzzle.PrintCells(UndefinedSymbol);
        return result;
    }
    
    public List<string> SolveMultiplePuzzles(List<string> allPuzzles, ref string output,
        out List<bool> solvedPuzzles)
    {
        var numberOfUnsolvedPuzzles = SolveMultiplePuzzles(allPuzzles, out var results, out solvedPuzzles);
        output = numberOfUnsolvedPuzzles + " of " + allPuzzles.Count + " puzzles have not been solved.";
        return results;
    }

    public List<string> SolveOnePuzzle(List<string> allPuzzles, int index, ref string output)
    {
        var result = new List<string>();
        
        if (index>0)
            index = index - 1;
        if (index >= 0 && index < allPuzzles.Count)
        {
            var res = SolveOnePuzzle(allPuzzles, index, out var count);
            result.Add(res);
            output = count == 0
                ? "Puzzle has been solved."
                : "Puzzle has not been solved. " + count + " cells are still unsolved.";
        }

        return result;
    }

}