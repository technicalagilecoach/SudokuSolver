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

        var index = 0;
        for (var i=0; i<results.Count; i++)
        {
            if (Unsolved && solvedPuzzles.Count==results.Count && solvedPuzzles[i])
                continue;
                
            var res = results[i];

            if (FileType == Input.FileType.MultiplePuzzlesWithName)
            {
                sr.WriteLine(PuzzleNames[index]);
                sr.Write(res);
                index++;
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
        int index = 0;
        for (var i=0; i<results.Count; i++)
        {
            if (Unsolved && solvedPuzzles.Count==results.Count && solvedPuzzles[i])
                continue;
                    
            var res = results[i];
                    
            if (FileType == Input.FileType.MultiplePuzzlesWithName)
            {
                console.Output.WriteLine(PuzzleNames[index]);
                console.Output.Write(res);    
                index++;
            }

            if (FileType == Input.FileType.MultiplePuzzlesOneLineEach)
            {
                res = res.Replace("\n", "");
                console.Output.WriteLine(res);
            }
        }
    }
}