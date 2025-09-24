namespace SudokuSolver;

public static class DebugOutput
{
    public static string PrintCells(int[,] cells)
    {
        var buffer = new StringWriter();
            
        foreach (var row in Puzzle.AllDigits) {
            foreach (var column in Puzzle.AllDigits)
            {
                Position position = new Position(row, column);
                var v = cells[position.Row, position.Column];
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
        var currentState = puzzle.PrintCells();
        Console.WriteLine(currentState);
            
        var spaces = ValidityChecker.CountUndefinedCells(puzzle.Cells);
        Console.WriteLine(spaces);
            
        var potentialValues = PrintPotentialValues(puzzle);
        Console.WriteLine(potentialValues);
    }
    
    public static string PrintSpecial(Puzzle puzzle)
    {
        var data = new List<string>();
        IndicesAndIterators.ForEachCell(position =>
            {
                
                if (puzzle.Cells[position.Row, position.Column] == Puzzle.Undefined)
                {
                    var pValues = puzzle.Candidates[position.Row,position.Column];
                    var values = new List<int>();
                    foreach (var index in Puzzle.AllDigits)
                    {
                        var kv = pValues[index];
                        if (kv)
                            values.Add(index + 1);
                    }
                    data.Add("C:"+string.Join("", values));
                }
                else
                {
                    var v = puzzle.Cells[position.Row, position.Column];
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