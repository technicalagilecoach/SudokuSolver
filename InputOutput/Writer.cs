using CliFx.Infrastructure;
using static SudokuSolver.SolverWrapper;

namespace SudokuSolver;

public class Writer(bool unsolved, string undefinedSymbol, Input.FileType fileType, List<string> puzzleNames)
{
    private bool Unsolved { get; } = unsolved;
    private string UndefinedSymbol { get; } = undefinedSymbol;
    private Input.FileType FileType { get; } = fileType;
    private List<string> PuzzleNames { get; } = puzzleNames;
    
    public void WriteResults(IConsole console, FileStream? fileStream, List<string> results, List<bool> solvedPuzzles, List<string> originalPuzzles)
    {
        if (fileStream is not null)
        {
            WritePuzzlesToFileStream(fileStream, results, solvedPuzzles);
        }
        else 
        {
            if (FileType == Input.FileType.SinglePuzzle)
                WriteSinglePuzzleToConsole(console, results[0], originalPuzzles[0]);
            else
                WriteMultiplePuzzlesToConsole(console, results, solvedPuzzles);
        }
    }

    private void WritePuzzlesToFileStream(FileStream fileStream, List<string> results, List<bool> solvedPuzzles)
    {
        using var sr = new StreamWriter(fileStream);

        for (var i=0; i<results.Count; i++)
        {
           
            if (Unsolved && solvedPuzzles.Count==results.Count && solvedPuzzles[i])
                continue;
                
            var res = results[i];

            if (FileType == Input.FileType.MultiplePuzzlesWithName)
            {
                sr.WriteLine(PuzzleNames[i]);
                sr.Write(res);
            }

            if (FileType == Input.FileType.MultiplePuzzlesOneLineEach)
            {
                res = res.Replace("\n", "");
                sr.WriteLine(res);
            }
        }
    }

    private void WriteSinglePuzzleToConsole(IConsole console, string result, string originalPuzzle)
    {
        var puzzleIndex = 0;
        for (var index = 0; index < result.Length; index++)
        {
            if (index%10!=0)
                puzzleIndex++;
                    
            string nextCell = result[index].ToString().Replace(" ", UndefinedSymbol);
                    
            if (puzzleIndex<originalPuzzle.Length && result[index] == originalPuzzle[puzzleIndex])
            {
                console.WithForegroundColor(ConsoleColor.DarkRed);
                console.Output.Write(nextCell);
                console.ResetColor();
            }
            else
            {
                console.Output.Write(nextCell);
            }
        }
    }

    private void WriteMultiplePuzzlesToConsole(IConsole console, List<string> results, List<bool> solvedPuzzles)
    {
        for (var i=0; i<results.Count; i++)
        {
            if (Unsolved && solvedPuzzles.Count==results.Count && solvedPuzzles[i])
                continue;
                    
            var res = results[i];
                    
            if (FileType == Input.FileType.MultiplePuzzlesWithName)
            {
                console.Output.WriteLine(PuzzleNames[i]);
                console.Output.Write(res);    
            }

            if (FileType == Input.FileType.MultiplePuzzlesOneLineEach)
            {
                res = res.Replace("\n", "");
                console.Output.WriteLine(res);
            }
        }
    }
    
    public void WriteStatistics(IConsole console, List<string> puzzleNames, List<Dictionary<string,int>> stats)
    {
        var summary = SumUpStatistics();

        for (var i = 0; i < stats.Count; i++)
        {
            WriteStatsLine(puzzleNames[i], stats[i]);
        }
        
        console.Output.WriteLine();
        WriteStatsLine("Summary", summary);
        return;

        void WriteStatsLine(string puzzleName, Dictionary<string, int> stat)
        {
            console.Output.Write(puzzleName + " - ");
            
            foreach (var strategy in stat)
            {
                string formatString = "{0,4}";
                console.Output.Write(strategy.Key + ": " + String.Format(formatString, strategy.Value) + " ");
            }
            console.Output.WriteLine();
        }

        Dictionary<string, int> SumUpStatistics()
        {
            var dictionary = new Dictionary<string, int>();
            foreach (var stat in stats)
            {
                foreach (var strategy in stat)
                {
                    dictionary.TryAdd(strategy.Key, 0);
                    dictionary[strategy.Key] += strategy.Value;
                }
            }

            return dictionary;
        }
    }
}