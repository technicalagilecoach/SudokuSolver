using System.IO;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using SudokuSolverTests;

namespace SudokuSolver;

[Command(Description = "A simple command line Sudoku solver.")]
public class InputCommand : ICommand
{
    [CommandParameter(0, Description = "A file containing one or more Sudoku puzzles to solve.")]
    public required FileInfo FileName { get; set; }

    [CommandOption("output", 'o', Description = "File to store the results.")]
    public FileInfo? OutputFile { get; set; }
    
    public ValueTask ExecuteAsync(IConsole console)
    {
        var allPuzzles = TestHelpers.ReadPuzzlesFromFile(FileName.FullName);
        var numberOfUnsolvedPuzzles = TestHelpers.SolveMultiplePuzzles(allPuzzles);

        string output = numberOfUnsolvedPuzzles + " of " + allPuzzles.Count + " puzzles have not been solved.";
        
        if (OutputFile is not null)
        {
            using FileStream fs = File.Create(OutputFile.FullName);
            using var sr = new StreamWriter(fs);

            sr.WriteLine(output);
        }
        else 
        {
            console.Output.WriteLine(output);
        }

        // If the execution is not meant to be asynchronous,
        // return an empty task at the end of the method.
        return default;
    }
}