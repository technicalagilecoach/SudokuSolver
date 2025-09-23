using CliFx.Infrastructure;

namespace SudokuSolver;

public class Writer(bool unsolved, FileInfo? outputFile)
{
    public FileInfo? OutputFile = outputFile;

    public void WritePuzzlesToFileStream(FileStream fileStream, List<string> results, List<bool> solvedPuzzles,
        Input.FileType fileType,
        List<string> puzzleNames, string output)
    {
        
        using var sr = new StreamWriter(fileStream);

        int index = 0;
        for (var i=0; i<results.Count; i++)
        {
            if (unsolved && solvedPuzzles.Count==results.Count && solvedPuzzles[i])
                continue;
                
            var res = results[i];

            if (fileType == Input.FileType.MultiplePuzzlesWithName)
            {
                sr.WriteLine(puzzleNames[index]);
                sr.Write(res);
                index++;
            }

            if (fileType == Input.FileType.MultiplePuzzlesOneLineEach)
            {
                res = res.Replace("\n", "");
                sr.WriteLine(res);
            }
        }

        sr.WriteLine(output);
    }
    
    public void WriteSinglePuzzleToConsole(IConsole console, string solution, string puzzle, string undefinedSymbol)
    {
        var puzzleIndex = 0;
        for (var index = 0; index < solution.Length; index++)
        {
            if (index%10!=0)
                puzzleIndex++;
                    
            string nextCell = solution[index].ToString().Replace(" ", undefinedSymbol);
                    
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

    public void WriteMultiplePuzzlesToConsole(IConsole console, List<string> results, List<bool> solvedPuzzles, Input.FileType fileType,
        List<string> puzzleNames)
    {
        int index = 0;
        for (var i=0; i<results.Count; i++)
        {
            if (unsolved && solvedPuzzles.Count==results.Count && solvedPuzzles[i])
                continue;
                    
            var res = results[i];
                    
            if (fileType == Input.FileType.MultiplePuzzlesWithName)
            {
                console.Output.WriteLine(puzzleNames[index]);
                console.Output.Write(res);    
                index++;
            }

            if (fileType == Input.FileType.MultiplePuzzlesOneLineEach)
            {
                res = res.Replace("\n", "");
                console.Output.WriteLine(res);
            }
        }
    }
}