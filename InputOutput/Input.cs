namespace SudokuSolver;

public static class Input
{
    public enum FileType
    {
        SinglePuzzle,
        MultiplePuzzlesOneLineEach,
        MultiplePuzzlesWithName,
        Unknown
    }
    
    public static List<string> ReadPuzzlesFromFile(string filename)
    {
        var allLines = GetAllLinesFromFile(filename);
        
        var fileType = DetermineFileType(filename);

        switch (fileType)
        {
            case FileType.Unknown:
                return [];
            case FileType.MultiplePuzzlesOneLineEach:
                return allLines;
            case FileType.MultiplePuzzlesWithName:
                return ReadMultilinePuzzles(allLines);
        }

        var sw = new StringWriter();
        foreach (var line in allLines)
            sw.Write(line);
        
        return [sw.ToString()];
    }

    private static List<string> GetAllLinesFromFile(string filename)
    {
        using StreamReader reader = new(filename);
        var text = reader.ReadToEnd();
        char[] separators = ['\n', '\r'];
        var allLines = text.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToList();
        return allLines;
    }
    
    public static FileType DetermineFileType(string filename)
    {
        using StreamReader reader = new(filename);
        var firstLine = reader.ReadLine();

        if (firstLine == null)
            return FileType.Unknown; // Todo: throw exception instead?
        
        if (firstLine.Length == Puzzle.GridSize * Puzzle.GridSize)
            return FileType.MultiplePuzzlesOneLineEach; // 81 numbers/spaces/zeroes in first line

        bool IsValidPuzzleLine(string line)
        {
            if (line.Length != Puzzle.GridSize)
                return false;
            
            var b = true;
            for (var i = 0; i < line.Length; i++)
            {
                char symbol = line[i];
                if (char.IsDigit(symbol)||char.IsWhiteSpace(symbol)||symbol=='.')
                    continue;
                b = false;
                break;
            }

            return b;
        }

        if (IsValidPuzzleLine(firstLine))
            return FileType.SinglePuzzle; // 9 numbers/spaces/zeroes in first line
        
        var secondLine = reader.ReadLine();
        if (secondLine != null && IsValidPuzzleLine(secondLine))
            return FileType.MultiplePuzzlesWithName; // text in first line, 9 numbers/spaces/zeroes in second line

        return FileType.Unknown;
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