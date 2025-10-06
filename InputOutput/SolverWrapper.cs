namespace SudokuSolver;

public class SolverWrapper(string undefinedSymbol, Input.FileType fileType, List<string> allPuzzles)
{
    private string UndefinedSymbol { get; } = undefinedSymbol;
    private Input.FileType FileType { get; } = fileType;
    private List<string> AllPuzzles { get; } = allPuzzles;
    public List<Dictionary<string, int>> StrategyStatistics { get; } = [];

    public List<string> SolvePuzzles(int number, ref string output, ref List<bool> solvedPuzzles)
    {
        List<string> results = [];
        if (FileType == Input.FileType.SinglePuzzle || number > 0)
        {
            var result = Solve(AllPuzzles[GetIndexFromNumber(number)], UndefinedSymbol, out var count, out var strategyStats);
            results.Add(result);
            StrategyStatistics.Add(strategyStats);
            output = CreateMessageForSinglePuzzle(count);
        }
        else
        {
            var numberOfUnsolvedPuzzles = SolveMultiplePuzzles(out var res, out solvedPuzzles);
            output = CreateMessageForMultiplePuzzles(numberOfUnsolvedPuzzles);
            results = res;
        }

        return results;
    }

    private int SolveMultiplePuzzles(out List<string> results, out List<bool> solvedPuzzles)
    {
        var numberOfUnsolvedCells = new int[AllPuzzles.Count];
        results = [];

        for (var index = 0; index < AllPuzzles.Count; index++)
        {
            var result = Solve(AllPuzzles[index], UndefinedSymbol, out var count, out var strategyStats);
            results.Add(result);
            StrategyStatistics.Add(strategyStats);
            numberOfUnsolvedCells[index] = count;
        }
        
        var numberOfUnsolvedPuzzles = numberOfUnsolvedCells.Count(c => c!=0);
        solvedPuzzles = numberOfUnsolvedCells.Select(n => n == 0).ToList();

        return numberOfUnsolvedPuzzles;
    }
    
    public static string Solve(string puzzleString, string undefinedSymbol, out int numberOfUnsolvedCells,
        out Dictionary<string, int> strategyStats, FileInfo? pdfFile = null)
    {
        var puzzle = new Puzzle();
        puzzle.Init(puzzleString);
        
        Puzzle? origPuzzle = null;
        if (pdfFile != null)
            origPuzzle = Puzzle.Copy(puzzle);
        
        Solver solver = new(puzzle);
        var solved = solver.Solve();

        strategyStats = solver.StrategyStats;
        
        numberOfUnsolvedCells = 0;
        if (!solved)
            numberOfUnsolvedCells = puzzle.CountUndefinedCells();

        if (!solved && origPuzzle!=null && pdfFile != null)
        {
            var lastConsistentState = new Puzzle();
            lastConsistentState.Init(solver.LastConsistentState.Replace("\n", ""));
            var pc = new PruneCandidates(lastConsistentState);
            pc.Handle();
            
            PdfWriter.WritePdfWithCandidates(origPuzzle, lastConsistentState, pdfFile.FullName);
        }

        var result = puzzle.PrintCells(undefinedSymbol);
        return result;
    }
    
    private int GetIndexFromNumber(int number)
    {
        int index = 0;

        if (FileType == Input.FileType.SinglePuzzle)
            index = 0;
        else
            index = number - 1;

        if (index < 0 || index >= AllPuzzles.Count)
            throw new IndexOutOfRangeException();
        
        return index;
    }

    private static string CreateMessageForSinglePuzzle(int count)
    {
        return count == 0
            ? "Puzzle has been solved."
            : "Puzzle has not been solved. " + count + " cells are still unsolved.";
    }

    private string CreateMessageForMultiplePuzzles(int numberOfUnsolvedPuzzles)
    {
        return numberOfUnsolvedPuzzles + " of " + AllPuzzles.Count + " puzzles have not been solved.";
    }
}