namespace SudokuSolver;

public class SolverWrapper(string undefinedSymbol, Input.FileType fileType, List<string> allPuzzles)
{
    private string UndefinedSymbol { get; } = undefinedSymbol;
    private Input.FileType FileType { get; } = fileType;
    private List<string> AllPuzzles { get; } = allPuzzles;

    public List<string> SolvePuzzles(int number, ref string output, ref List<bool> solvedPuzzles)
    {
        List<string> results;
        if (FileType == Input.FileType.SinglePuzzle || number > 0)
            results = SolveOnePuzzle(number, ref output);
        else
            results = SolveMultiplePuzzles(ref output, out solvedPuzzles);
        return results;
    }

    private string SolveOnePuzzle(int index, out int numberOfUnsolvedCells)
    {
        numberOfUnsolvedCells = 0;
        
        if (index < 0 || index >= AllPuzzles.Count) 
            return "";
        
        var puzzle = AllPuzzles[index];
        var result = Solve(puzzle, out var solved, out numberOfUnsolvedCells);

        return result;
    }

    private List<string> SolveOnePuzzle(int index, ref string output)
    {
        var result = new List<string>();
        
        if (index>0)
            index = index - 1;
        if (index >= 0 && index < AllPuzzles.Count)
        {
            var res = SolveOnePuzzle(index, out var count);
            result.Add(res);
            output = count == 0
                ? "Puzzle has been solved."
                : "Puzzle has not been solved. " + count + " cells are still unsolved.";
        }

        return result;
    }

    private int SolveMultiplePuzzles(out List<string> solutions,
        out List<bool> solvedPuzzles)
    {
        var numberOfUnsolvedCells = new int[AllPuzzles.Count];
        solutions = [];

        for (var index = 0; index < AllPuzzles.Count; index++)
        {
            var result = SolveOnePuzzle(index, out var count);
            numberOfUnsolvedCells[index] = count;
            solutions.Add(result);
        }
        
        var numberOfUnsolvedPuzzles = numberOfUnsolvedCells.Count(c => c!=0);
        solvedPuzzles = numberOfUnsolvedCells.Select(n => n == 0).ToList();

        return numberOfUnsolvedPuzzles;
    }

    private List<string> SolveMultiplePuzzles(ref string output, out List<bool> solvedPuzzles)
    {
        var numberOfUnsolvedPuzzles = SolveMultiplePuzzles(out var results, out solvedPuzzles);
        output = numberOfUnsolvedPuzzles + " of " + AllPuzzles.Count + " puzzles have not been solved.";
        return results;
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
}