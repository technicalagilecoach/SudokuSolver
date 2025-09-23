using CliFx;
using CliFx.Attributes;
using CliFx.Exceptions;
using CliFx.Infrastructure;

namespace SudokuSolver;

[Command(Description = "A simple command line Sudoku solver.")]
public class InputCommand : ICommand
{
    [CommandParameter(0, Description = "A file containing one or more Sudoku puzzles to solve.")]
    public required FileInfo FileName { get; set; }

    [CommandOption("output", 'o', Description = "File to store the results.")]
    public FileInfo? OutputFile { get; set; }
    
    [CommandOption("number", 'n', Description = "Number of the puzzle to be solved from a file with multiple puzzles.")]
    public int Number { get; set; }

    [CommandOption("unsolved", 'u', Description = "Output the unsolved puzzles only. This option only applies to input files with multiple puzzles.")]
    public bool Unsolved { get; set; } = false;
    
    public ValueTask ExecuteAsync(IConsole console)
    {
        CheckIfFileIsMissing(FileName);
        
        var fileType = Input.DetermineFileType(FileName.FullName);
        
        if (fileType == Input.FileType.Unknown)
            throw new CommandException("Invalid file type.");

        var allPuzzles = Input.ReadPuzzlesFromFile(FileName.FullName, out var puzzleNames);

        var undefinedSymbol = DetermineUndefinedSymbol(allPuzzles[0]);
        
        List<string> results;
        List<bool> solvedPuzzles = new List<bool>();
        
        var output = "";

        if (fileType == Input.FileType.SinglePuzzle || Number > 0)
            results = SolveOnePuzzle(allPuzzles, Number, ref output, undefinedSymbol);
        else
            results = SolveMultiplePuzzles(allPuzzles, ref output, out solvedPuzzles, undefinedSymbol);

        if (OutputFile is not null)
        {
            WritePuzzlesToFile(results, solvedPuzzles, fileType, puzzleNames, output);
        }
        else 
        {
            if (fileType == Input.FileType.SinglePuzzle)
                WriteSinglePuzzleToConsole(console, results[0], allPuzzles[0], undefinedSymbol);
            else
                WriteMultiplePuzzlesToConsole(console, results, solvedPuzzles, fileType, puzzleNames);
            
            console.Output.WriteLine(output);
        }

        // If the execution is not meant to be asynchronous,
        // return an empty task at the end of the method.
        return default;
    }

    private void WritePuzzlesToFile(List<string> results, List<bool> solvedPuzzles, Input.FileType fileType, List<string> puzzleNames, string output)
    {
        using var fs = TryToCreateFile(OutputFile);
        using var sr = new StreamWriter(fs);

        int index = 0;
        for (var i=0; i<results.Count; i++)
        {
            if (Unsolved && solvedPuzzles.Count==results.Count && solvedPuzzles[i])
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

    private void WriteMultiplePuzzlesToConsole(IConsole console, List<string> results, List<bool> solvedPuzzles, Input.FileType fileType,
        List<string> puzzleNames)
    {
        int index = 0;
        for (var i=0; i<results.Count; i++)
        {
            if (Unsolved && solvedPuzzles.Count==results.Count && solvedPuzzles[i])
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

    private static void WriteSinglePuzzleToConsole(IConsole console, string solution, string puzzle, string undefinedSymbol)
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

    private static string DetermineUndefinedSymbol(string firstPuzzle)
    {
        string undefinedSymbol="";
        
        if (firstPuzzle.Contains(' '))
        {
            undefinedSymbol = " ";
        }
        else if (firstPuzzle.Contains('.'))
        {
            undefinedSymbol = ".";
        }
        else if (firstPuzzle.Contains('0'))
        {
            undefinedSymbol = "0";
        }

        return undefinedSymbol;
    }

    private static List<string> SolveMultiplePuzzles(List<string> allPuzzles, ref string output,
        out List<bool> solvedPuzzles, string undefinedSymbol)
    {
        var solver = new SolverWrapper(undefinedSymbol);
        var numberOfUnsolvedPuzzles = solver.SolveMultiplePuzzles(allPuzzles, out var results, out solvedPuzzles);
        output = numberOfUnsolvedPuzzles + " of " + allPuzzles.Count + " puzzles have not been solved.";
        return results;
    }

    private List<string> SolveOnePuzzle(List<string> allPuzzles, int index, ref string output, string undefinedSymbol)
    {
        var solver = new SolverWrapper(undefinedSymbol);
        var result = new List<string>();
        
        if (index<0)
            index = Number - 1;
        if (index >= 0 && index < allPuzzles.Count)
        {
            var res = solver.SolveOnePuzzle(allPuzzles, index, out var count);
            result.Add(res);
            output = count == 0
                ? "Puzzle has been solved."
                : "Puzzle has not been solved. " + count + " cells are still unsolved.";
        }

        return result;
    }

    private static void CheckIfFileIsMissing(FileInfo filename)
    {
        if (!filename.Exists)
            throw new CommandException("Sudoku file not found.", 1);
    }

    private static FileStream TryToCreateFile(FileInfo fileInfo)
    {
        FileStream? fs = null;
        try
        {
            if (fileInfo.Exists)
                throw new CommandException("File already exists.", 1);

            var directory = fileInfo.Directory;

            if (directory is not null && !directory.Exists)
            {
                try
                {
                    directory.Create();
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw new CommandException("Target directory does not exist and cannot be created.", 1);
                }
                catch (IOException ex)
                {
                    throw new CommandException("Target directory does not exist and cannot be created.", 1);
                }
            }

            try
            {
                fs = File.Create(fileInfo.FullName);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new CommandException("Target file does not exist and cannot be created.", 1);
            }

            return fs;
        }
        catch
        {
            fs?.Dispose();
            throw;
        }
    }
}