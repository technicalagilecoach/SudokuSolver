namespace SudokuSolverCore;

public static class Printers
{
    public static string Print(Puzzle puzzle)
    {
        var buffer = new StringWriter();
            
        foreach (var row in Puzzle.AllDigits) {
            foreach (var column in Puzzle.AllDigits)
            {
                var v = puzzle.GetValue(new Position(row, column));
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
            if (puzzle.GetValue(position) == Puzzle.Undefined)
            {
                var pValues = puzzle.PossibleValues[position.Row,position.Column];

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
            
        var spaces = ValidityChecker.CountUndefinedCells(puzzle.GetCells());
        Console.WriteLine(spaces);
            
        var potentialValues = PrintPotentialValues(puzzle);
        Console.WriteLine(potentialValues);
    }
}