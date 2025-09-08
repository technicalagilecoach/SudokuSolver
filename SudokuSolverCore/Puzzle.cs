using System.Collections;
using static SudokuSolverCore.IndicesAndIterators;

namespace SudokuSolverCore;

public class Puzzle
{
    public const int GridSize = 9;
    public const int RegionSize = 3;
    public const int Undefined = 0;

    internal static readonly IEnumerable<int> AllDigits = Enumerable.Range(0, GridSize);

    public readonly int[,] Cells = new int[GridSize, GridSize];
    public BitArray[,] Candidates { get; } = new BitArray[GridSize, GridSize];

    public bool IsUndefined(Position position)
    {
        return Cells[position.Row, position.Column] == Undefined;
    }
    
    public void Init(string puzzle)
    {
        var rows = puzzle.Split('\n');

        ForEachCell(position =>
        {
            var value = rows[position.Row][position.Column].ToString();
            var v = value == " " ? Undefined : int.Parse(value);
            Cells[position.Row, position.Column] = v;
        });

        ForEachCell(position =>
        {
            Candidates[position.Row, position.Column] = InitializeCandidates(position);
        });
    }

    private BitArray InitializeCandidates(Position position)
    {
        var potentialValues = new BitArray(GridSize);
            
        if (Cells[position.Row, position.Column] == Undefined)
        {
            potentialValues.SetAll(true);
        }
        else
        {
            potentialValues.SetAll(false);
            potentialValues[Cells[position.Row, position.Column] -1] = true;
        }
            
        return potentialValues;
    }
        
    public static int CountCandidates(BitArray[,] candidates, Position position)
    {
        var count = 0;
            
        foreach (bool bit in candidates[position.Row, position.Column])
        {
            if (bit)
                count++;
        }

        return count;
    }
}