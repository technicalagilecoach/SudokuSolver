using CliFx.Infrastructure;
using static SudokuSolver.SolverWrapper;

namespace SudokuSolver;

public class Writer(bool unsolved, string undefinedSymbol, Input.FileType fileType)
{
    private bool Unsolved { get; } = unsolved;
    private string UndefinedSymbol { get; } = undefinedSymbol;
    private Input.FileType FileType { get; } = fileType;

    public void WriteResults(IConsole console, FileStream? fileStream,
        List<string> results, List<bool> solvedPuzzles, List<string> puzzleNames, List<string> allPuzzles)
    {
        if (fileStream is not null)
        {
            WritePuzzlesToFileStream(fileStream, results, solvedPuzzles, puzzleNames);
        }
        else 
        {
            if (FileType == Input.FileType.SinglePuzzle)
                WriteSinglePuzzleToConsole(console, results[0], allPuzzles[0]);
            else
                WriteMultiplePuzzlesToConsole(console, results, solvedPuzzles, puzzleNames);
        }
    }

    private void WritePuzzlesToFileStream(FileStream fileStream, List<string> results, List<bool> solvedPuzzles,
        List<string> puzzleNames)
    {
        using var sr = new StreamWriter(fileStream);

        int index = 0;
        for (var i=0; i<results.Count; i++)
        {
            if (Unsolved && solvedPuzzles.Count==results.Count && solvedPuzzles[i])
                continue;
                
            var res = results[i];

            if (FileType == Input.FileType.MultiplePuzzlesWithName)
            {
                sr.WriteLine(puzzleNames[index]);
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

    private void WriteSinglePuzzleToConsole(IConsole console, string solution, string puzzle)
    {
        var puzzleIndex = 0;
        for (var index = 0; index < solution.Length; index++)
        {
            if (index%10!=0)
                puzzleIndex++;
                    
            string nextCell = solution[index].ToString().Replace(" ", UndefinedSymbol);
                    
            if (puzzleIndex<puzzle.Length && solution[index] == puzzle[puzzleIndex])
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

    private void WriteMultiplePuzzlesToConsole(IConsole console, List<string> results, List<bool> solvedPuzzles, 
        List<string> puzzleNames)
    {
        int index = 0;
        for (var i=0; i<results.Count; i++)
        {
            if (Unsolved && solvedPuzzles.Count==results.Count && solvedPuzzles[i])
                continue;
                    
            var res = results[i];
                    
            if (FileType == Input.FileType.MultiplePuzzlesWithName)
            {
                console.Output.WriteLine(puzzleNames[index]);
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