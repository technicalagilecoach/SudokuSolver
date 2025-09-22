namespace SudokuSolver;

public static class Input
{
    public static List<string> ReadPuzzlesFromFile(string filename)
    {
        var allLines = GetAllLinesFromFile(filename);
        return ContainsSingleLinePuzzles(allLines) ? allLines : ReadMultilinePuzzles(allLines);
    }

    private static List<string> GetAllLinesFromFile(string filename)
    {
        using StreamReader reader = new(filename);
        var text = reader.ReadToEnd();
        char[] separators = ['\n', '\r'];
        var allLines = text.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToList();
        return allLines;
    }

    private static bool ContainsSingleLinePuzzles(List<string> allLines)
    {
        return allLines[0].Length == Puzzle.GridSize*Puzzle.GridSize;
    }

    private static List<string> ReadMultilinePuzzles(List<string> allLines)
    {
        var allPuzzles = new List<string>();
        
        const int linesPerPuzzle = Puzzle.GridSize + 1;
        
        var numOfPuzzles = allLines.Count / linesPerPuzzle;

        for (var puzzleIndex = 0; puzzleIndex < numOfPuzzles; puzzleIndex++)
        {
            var offset = puzzleIndex * linesPerPuzzle;
            var puzzle = new List<string>();

            for (var lineIndex = offset; lineIndex < linesPerPuzzle + offset; lineIndex++)
            {
                if (lineIndex % linesPerPuzzle != 0)
                {
                    puzzle.Add(allLines[lineIndex]);
                }
            }

            var puzzleAsString = string.Join("", puzzle);
            
            allPuzzles.Add(puzzleAsString);
        }
        
        return allPuzzles;
    }
}