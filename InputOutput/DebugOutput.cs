namespace SudokuSolver;

public static class DebugOutput
{
    public static string PrintCells(int[,] cells)
    {
        var buffer = new StringWriter();
            
        foreach (var row in Puzzle.AllRows) {
            foreach (var column in Puzzle.AllColumns)
            {
                var position = new Position(row, column);
                var v = cells[position.Row-1, position.Column-1];
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
            if (puzzle.GetCellValue(position) == Puzzle.Undefined)
            {
                var pValues = puzzle.GetCandidates(position);

                var values = "";
                foreach (var digit in Puzzle.AllDigits)
                {
                    var kv = pValues[digit-1];
                    if (kv)
                        values += " " + (digit);
                }

                buffer.Write(position.Row + " " + position.Column + ":" + values + "\n");
            }
        });
            
        return buffer.ToString();
    }

    public static void PrintDebugOutput(Puzzle puzzle)
    {
        var currentState = puzzle.PrintCells();
        Console.WriteLine(currentState);
            
        var spaces = ValidityChecker.CountUndefinedCells(puzzle);
        Console.WriteLine(spaces);
            
        var potentialValues = PrintPotentialValues(puzzle);
        Console.WriteLine(potentialValues);
    }
    
    public static string PrintSpecial(Puzzle puzzle)
    {
        var data = new List<string>();
        IndicesAndIterators.ForEachCell(position =>
            {
                
                if (puzzle.GetCellValue(position) == Puzzle.Undefined)
                {
                    var pValues = puzzle.GetCandidates(position);
                    var values = new List<int>();
                    foreach (var digit in Puzzle.AllDigits)
                    {
                        var kv = pValues[digit-1];
                        if (kv)
                            values.Add(digit);
                    }
                    data.Add("C:"+string.Join("", values));
                }
                else
                {
                    var v = puzzle.GetCellValue(position);
                    data.Add("V:"+v.ToString());
                }
            } 
        );
        
        var buffer = new StringWriter();
        for (int i = 0; i < data.Count; i++)
        {
            buffer.Write(data[i]);
            if ((i+1)%9==0)
                buffer.WriteLine();
            else
                buffer.Write(",");
        }
        return buffer.ToString();
    }
}