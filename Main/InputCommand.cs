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

    [CommandOption("statistics", 's', Description = "Show statistics about the solver strategies which were used for each puzzle.")]
    public bool Statistics { get; set; } = false;
    
    [CommandOption("pdf", 'p', Description = "Create a pdf version of the unsolved puzzle. If the input contains multiple puzzles it only returns the first puzzle.")]
    public FileInfo? PdfFile { get; set; }
    
    public ValueTask ExecuteAsync(IConsole console)
    {
        CheckIfFileIsMissing(FileName);
        var fileStream = TryToInitializeFileStream();
        var fileType = CheckFileType(FileName.FullName);

        if (PdfFile is not null)
        {
            var allPuzzles = Input.ReadPuzzlesFromFile(FileName.FullName, out var puzzleNames);
            var puzzle = new Puzzle();
            puzzle.Init(allPuzzles[0]);
            PdfWriter.WritePdf(puzzleNames[0], puzzle, puzzle, false, PdfFile.FullName);
        }
        else
        {
            RunSolver(console, fileType, fileStream);
        }

        // If the execution is not meant to be asynchronous,
        // return an empty task at the end of the method.
        return default;
    }

    private void RunSolver(IConsole console, Input.FileType fileType, FileStream? fileStream)
    {
        var allPuzzles = Input.ReadPuzzlesFromFile(FileName.FullName, out var puzzleNames);
        var undefinedSymbol = Input.DetermineUndefinedSymbol(allPuzzles[0]);

        var solvedPuzzles = new List<bool>();
        var output = "";

        var solver = new SolverWrapper(undefinedSymbol, fileType, allPuzzles);
        var results = solver.SolvePuzzles(Number, ref output, ref solvedPuzzles);
       
        var writer = new Writer(Unsolved, undefinedSymbol, fileType, puzzleNames);
        writer.WriteResults(console, fileStream, results, solvedPuzzles, allPuzzles);
        
        console.Output.WriteLine(output);

        if (Statistics)
            WriteStatistics(console, puzzleNames, solver.StrategyStatistics);
    }

    private static void WriteStatistics(IConsole console, List<string> puzzleNames, List<Dictionary<string,int>> stats)
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

    private FileStream? TryToInitializeFileStream()
    {
        FileStream? fileStream = null;
        if (OutputFile is not null)
        {
            fileStream = TryToCreateFile(OutputFile);
        }

        return fileStream;
    }

    private static Input.FileType CheckFileType(string fileName)
    {
        var fileType = Input.DetermineFileType(fileName);
        if (fileType == Input.FileType.Unknown)
            throw new CommandException("Invalid file type.");
        return fileType;
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