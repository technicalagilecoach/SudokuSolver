using System.Collections;
using static SudokuSolverCore.IndicesAndIterators;

namespace SudokuSolverCore;

public class Puzzle
{
    public const int GridSize = 9;
    public const int BoxSize = 3;
    public const int Undefined = 0;

    internal static readonly IEnumerable<int> AllColumns = Enumerable.Range(0, GridSize);
    internal static readonly IEnumerable<int> AllRows = Enumerable.Range(0, GridSize);
    internal static readonly IEnumerable<int> AllBoxes = Enumerable.Range(0, GridSize);
    internal static readonly IEnumerable<int> AllDigits = Enumerable.Range(0, GridSize);

    public int[,] Cells { get; } = new int[GridSize, GridSize];
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
            var symbol = rows[position.Row][position.Column].ToString();
            var digit = symbol == " " ? Undefined : int.Parse(symbol);
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
        
    public static int CountCandidates(Position position, BitArray[,] candidates)
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