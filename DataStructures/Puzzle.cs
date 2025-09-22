using System.Collections;
using static SudokuSolver.IndicesAndIterators;

namespace SudokuSolver;

public class Puzzle
{
    public const int GridSize = 9;
    public const int BoxSize = 3;
    public const int Undefined = 0;

    public static readonly IEnumerable<int> AllColumns = Enumerable.Range(0, GridSize);
    public static readonly IEnumerable<int> AllRows = Enumerable.Range(0, GridSize);
    public static readonly IEnumerable<int> AllBoxes = Enumerable.Range(0, GridSize);
    public static readonly IEnumerable<int> AllDigits = Enumerable.Range(0, GridSize);

    public int[,] Cells { get; } = new int[GridSize, GridSize];
    public BitArray[,] Candidates { get; } = new BitArray[GridSize, GridSize];

    public bool IsUndefined(Position position)
    {
        return Cells[position.Row, position.Column] == Undefined;
    }

    public void Init(string puzzle)
    {
        var rows = new List<string>();
        for (var row = 0; row < GridSize; row++)
        {
            rows.Add(puzzle.Substring(GridSize * row, GridSize));
        }

        ForEachCell(position =>
        {
            var symbol = rows[position.Row][position.Column].ToString();
            var digit = (symbol == " " || symbol== ".") ? Undefined : int.Parse(symbol); // To Do: inefficient logical or?
            Cells[position.Row, position.Column] = digit;
        });

        ForEachCell(position =>
        {
            BitArray candidates = InitializeCandidates(position);
            Candidates[position.Row, position.Column] = candidates;
        });
    }

    private BitArray InitializeCandidates(Position position)
    {
        var candidates = new BitArray(GridSize);
            
        if (IsUndefined(position))
        {
            candidates.SetAll(true);
        }
        else
        {
            candidates.SetAll(false);
            candidates[Cells[position.Row, position.Column] - 1] = true;
        }
            
        return candidates;
    }
    
    public static string Difference(string puzzle1, string puzzle2)
    {
        var difference = new char[puzzle1.Length];
                    
        for (var i = 0; i < puzzle1.Length; i++)
        {
            difference[i] = ' ';
            if (puzzle1[i] == '\n')
            {
                difference[i] = '\n';
            }
            if (puzzle1[i] != puzzle2[i])
                difference[i] = puzzle2[i];
        }

        return new string(difference);
    }
    
    public string PrintCells()
    {
        var buffer = new StringWriter();
            
        foreach (var row in Puzzle.AllDigits) {
            foreach (var column in Puzzle.AllDigits)
            {
                Position position = new Position(row, column);
                var v = Cells[position.Row, position.Column];
                buffer.Write(v == Puzzle.Undefined ? " ":v);
            } 

            buffer.WriteLine();
        }  
            
        return buffer.ToString();
    }
}