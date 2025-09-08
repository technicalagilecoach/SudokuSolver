namespace SudokuSolverCore;

public static class Printers
{
    public static string Print(Puzzle puzzle)
    {
        var buffer = new StringWriter();
            
        foreach (var row in Puzzle.AllDigits) {
            foreach (var column in Puzzle.AllDigits)
            {
                Position position = new Position(row, column);
                var v = puzzle.Cells[position.Row, position.Column];
                buffer.Write(v == Puzzle.Undefined ? " ":v);
            } 

            buffer.WriteLine();
        }  
            
        return buffer.ToString();
    }

    private static string PrintPotentialValues(Puzzle puzzle)
    {
        var buffer = new StringWriter();

        IndicesAndIterators.ForEachCell(position =>
        {
            if (puzzle.Cells[position.Row, position.Column] == Puzzle.Undefined)
            {
                var pValues = puzzle.Candidates[position.Row,position.Column];

                var values = "";
                foreach (var index in Puzzle.AllDigits)
                {
                    var kv = pValues[index];
                    if (kv)
                        values += " " + (index + 1);
                }

                buffer.Write(position.Row + " " + position.Column + ":" + values + "\n");
            }
        });
            
        return buffer.ToString();
    }

    public static void PrintDebugOutput(Puzzle puzzle)
    {
        var currentState = Print(puzzle);
        Console.WriteLine(currentState);
            
        var spaces = ValidityChecker.CountUndefinedCells(puzzle.Cells);
        Console.WriteLine(spaces);
            
        var potentialValues = PrintPotentialValues(puzzle);
        Console.WriteLine(potentialValues);
    }
}