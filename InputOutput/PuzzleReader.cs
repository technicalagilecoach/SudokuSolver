using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SudokuSolver;

public static class PuzzleReader
{
    public static List<string> ReadPuzzlesFromFile(string filename)
    {
        var allLines = GetAllLinesFromFile(filename);
        return ContainsSingleLinePuzzles(allLines) ? ReadSingleLinePuzzles(allLines) : ReadMultilinePuzzles(allLines);
    }

    private static List<string> GetAllLinesFromFile(string filename)
    {
        using StreamReader reader = new(filename);
        var text = reader.ReadToEnd();
        var allLines = text.Split((char[])['\n', '\r'], StringSplitOptions.RemoveEmptyEntries).ToList();
        return allLines;
    }

    private static bool ContainsSingleLinePuzzles(List<string> allLines)
    {
        return allLines[0].Length == (Puzzle.GridSize^2);
    }

    private static List<string> ReadMultilinePuzzles(List<string> allLines)
    {
        var allPuzzles = new List<string>();
        
        const int linesPerPuzzle = Puzzle.GridSize + 1;
        
        var numOfPuzzles = allLines.Count / linesPerPuzzle;

        for (var i = 0; i < numOfPuzzles; i++)
        {
            var offset = i * linesPerPuzzle;
            var puzzle = new List<string>();

            for (var j = offset; j < linesPerPuzzle + offset; j++)
            {
                if (j % linesPerPuzzle != 0)
                {
                    puzzle.Add(allLines[j]);
                }
            }

            var puzzleAsString = string.Join("", puzzle);
            
            allPuzzles.Add(puzzleAsString);
        }
        
        return allPuzzles;
    }

    private static List<string> ReadSingleLinePuzzles(List<string> allLines)
    {
        var allPuzzles = allLines.ToList();
        
        return allPuzzles;
    }
}