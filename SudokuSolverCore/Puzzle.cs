using System.Collections;
using static SudokuSolverCore.IndicesAndIterators;

namespace SudokuSolverCore;

public class Puzzle
{
    public const int GridSize = 9;
    public const int RegionSize = 3;
    public const int Undefined = 0;

    internal static readonly IEnumerable<int> AllDigits = Enumerable.Range(0, GridSize);

    private readonly int[,] _cells = new int[GridSize, GridSize];
    public BitArray[,] PossibleValues { get; } = new BitArray[GridSize, GridSize];
        
        
    public int[,] GetCells()
    {
        return _cells;
    }

    public int GetValue(Position position)
    {
        return _cells[position.Row, position.Column];
    }

    private void SetValue(Position position, int value)
    {
        _cells[position.Row, position.Column] = value;
    }
        
        
    public void Init(string puzzle)
    {
        var rows = puzzle.Split('\n');

        ForEachCell(position =>
        {
            var value = rows[position.Row][position.Column].ToString();
            var v = value == " " ? Undefined : int.Parse(value);
            SetValue(position, v);
        });

        ForEachCell(position =>
        {
            PossibleValues[position.Row, position.Column] = InitializePossibleValues(position);
        });
    }

    private BitArray InitializePossibleValues(Position position)
    {
        var potentialValues = new BitArray(GridSize);
            
        if (GetValue(position) == Undefined)
        {
            potentialValues.SetAll(true);
        }
        else
        {
            potentialValues.SetAll(false);
            potentialValues[GetValue(position) -1] = true;
        }
            
        return potentialValues;
    }
        
    public static int CountPossibleValues(BitArray[,] possibleValues, Position position)
    {
        var count = 0;
            
        foreach (bool bit in possibleValues[position.Row, position.Column])
        {
            if (bit)
                count++;
        }

        return count;
    }
}