using System.Collections;
using static SudokuSolverCore.IndicesAndIterators;

namespace SudokuSolverCore;

public class Puzzle
{
    public const int GridSize = 9;
    public const int RegionSize = 3;
    public const int Undefined = 0;

    internal static readonly IEnumerable<int> AllColumns = Enumerable.Range(0, GridSize);
    internal static readonly IEnumerable<int> AllRows = Enumerable.Range(0, GridSize);
    internal static readonly IEnumerable<int> AllRegions = Enumerable.Range(0, GridSize);
    internal static readonly IEnumerable<int> AllDigits = Enumerable.Range(0, GridSize);

    public int[,] Cells { get; } = new int[GridSize, GridSize];
    public BitArray[,] Candidates { get; } = new BitArray[GridSize, GridSize];

    public bool IsUndefined(Position position)
    {
        return GetValue(position) == Undefined;
    }

    public int GetValue(Position position)
    {
        return Cells[position.Row, position.Column];
    }

    public void SetCandidates(Position position, BitArray candidates)
    {
        Candidates[position.Row, position.Column] = candidates;
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
            SetCandidates(position, InitializeCandidates(position));
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