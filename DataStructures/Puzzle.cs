using System.Collections;
using static SudokuSolver.IndicesAndIterators;

namespace SudokuSolver;

public class Puzzle
{
    public const int GridSize = 9;
    public const int BoxSize = 3;
    public const int Undefined = 0;

    public static readonly IEnumerable<int> AllColumns = Enumerable.Range(1, GridSize);
    public static readonly IEnumerable<int> AllRows = Enumerable.Range(1, GridSize);
    public static readonly IEnumerable<int> AllBoxes = Enumerable.Range(1, GridSize);
    public static readonly IEnumerable<int> AllDigits = Enumerable.Range(1, GridSize);

    private int[,] Cells { get; } = new int[GridSize, GridSize];
    private BitArray[,] Candidates { get; } = new BitArray[GridSize, GridSize];

    public bool IsUndefined(Position position)
    {
        return GetCellValue(position) == Undefined;
    }

    public int GetCellValue(Position position)
    {
        return Cells[position.Row-1, position.Column-1];
    }
    
    public void SetCellValue(Position position, int value)
    {
        Cells[position.Row-1, position.Column-1]=value;
    }

    public BitArray GetCandidates(Position position)
    {
        return Candidates[position.Row-1, position.Column-1];
    }

    public void Init(string puzzle)
    {
        var rows = StringToRows(puzzle);

        ForEachCell(position =>
        {
            var symbol = rows[position.Row-1][position.Column-1].ToString();
            var digit = symbol is " " or "." ? Undefined : int.Parse(symbol);
            SetCellValue(position,digit);
        });

        ForEachCell(position =>
        {
            BitArray candidates = InitializeCandidates(position);
            Candidates[position.Row-1, position.Column-1] = candidates;
        });
    }

    private static List<string> StringToRows(string puzzle)
    {
        var rows = new List<string>();
        for (var row = 0; row < GridSize; row++)
        {
            rows.Add(puzzle.Substring(GridSize * row, GridSize));
        }

        return rows;
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
            candidates[GetCellValue(position) - 1] = true;
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
    
    public string PrintCells(string undefinedSymbol = " ")
    {
        var buffer = new StringWriter();
            
        foreach (var row in Puzzle.AllRows) 
        {
            foreach (var column in Puzzle.AllColumns)
            {
                Position position = new Position(row, column);
                var v = GetCellValue(position);
                buffer.Write(v == Puzzle.Undefined ? undefinedSymbol : v);
            } 

            buffer.WriteLine();
        }  
            
        return buffer.ToString();
    }

    public int CountUndefinedCells()
    {
        var count = 0;

        foreach (var row in Puzzle.AllRows)
        {
            foreach (var column in Puzzle.AllColumns)
            {
                if (IsUndefined(new Position(row, column)))
                    count++;
            }
        }

        return count;
    }

    public static Puzzle Copy(Puzzle puzzle)
    {
        var puzzleString = puzzle.PrintCells();
        puzzleString = puzzleString.Replace("\n", "");
        var newPuzzle = new Puzzle();
        newPuzzle.Init(puzzleString);
        return newPuzzle;
    }
}