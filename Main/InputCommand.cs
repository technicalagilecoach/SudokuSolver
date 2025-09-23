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
    
    public ValueTask ExecuteAsync(IConsole console)
    {
        CheckIfFileIsMissing(FileName);
        
        var fileType = Input.DetermineFileType(FileName.FullName);
        
        if (fileType == Input.FileType.Unknown)
            throw new CommandException("Invalid file type.");
        
        var allPuzzles = Input.ReadPuzzlesFromFile(FileName.FullName);

        List<string> results;
        var output = "";

        if (fileType == Input.FileType.SinglePuzzle || Number > 0)
            results = SolveOnePuzzle(allPuzzles, Number, ref output);
        else
            results = SolveMultiplePuzzles(allPuzzles, ref output);

        if (OutputFile is not null)
        {
            using var fs = TryToCreateFile(OutputFile);
            using var sr = new StreamWriter(fs);

            foreach (var puzzle in results)
            {
                sr.WriteLine(puzzle);
            }

            sr.WriteLine(output);
        }
        else 
        {
            foreach (var puzzle in results)
            {
                console.Output.WriteLine(puzzle);
            }
            
            console.Output.WriteLine(output);
        }

        // If the execution is not meant to be asynchronous,
        // return an empty task at the end of the method.
        return default;
    }

    private static List<string> SolveMultiplePuzzles(List<string> allPuzzles, ref string output)
    {
        var numberOfUnsolvedPuzzles = SolverUtil.SolveMultiplePuzzles(allPuzzles, out var results);
        output = numberOfUnsolvedPuzzles + " of " + allPuzzles.Count + " puzzles have not been solved.";
        return results;
    }

    private List<string> SolveOnePuzzle(List<string> allPuzzles, int index, ref string output)
    {
        var result = new List<string>();
        
        if (index<0)
            index = Number - 1;
        if (index >= 0 && index < allPuzzles.Count)
        {
            var res = SolverUtil.SolveOnePuzzle(allPuzzles, index, out var count);
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