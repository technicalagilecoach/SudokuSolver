// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using System.CommandLine.Parsing;
using SudokuSolverTests;

namespace SudokuSolverMain;

class Program
{
    static int Main(string[] args)
    {
        Option<FileInfo> fileOption = new("--file")
        {
            Description = "A file containing one or more Sudoku puzzles to solve."
        };

        RootCommand rootCommand = new("Simple command line Sudoku solver.");
        rootCommand.Options.Add(fileOption);

        ParseResult parseResult = rootCommand.Parse(args);
        if (parseResult.Errors.Count == 0 && parseResult.GetValue(fileOption) is FileInfo parsedFile)
        {
            var puzzles = TestHelpers.ReadPuzzlesFromFile(parsedFile.FullName);
            var numberOfUnsolvedPuzzles = TestHelpers.SolveMultiplePuzzles(puzzles);
            Console.WriteLine(numberOfUnsolvedPuzzles + " of " + puzzles.Count + " puzzles have not been solved.");

            return 0;
        }
        foreach (ParseError parseError in parseResult.Errors)
        {
            Console.Error.WriteLine(parseError.Message);
        }
        return 1;
    }
}