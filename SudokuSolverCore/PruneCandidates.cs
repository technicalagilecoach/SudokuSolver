using System.Collections;
using static SudokuSolverCore.IndicesAndIterators;

namespace SudokuSolverCore;

public class PruneCandidates(Puzzle puzzle) : Strategy(puzzle)
{
    private int _numberOfRemovedCandidates = 0;
    
    public bool Handle()
    {
        _numberOfRemovedCandidates = 0;
        
        ForEachCell(RemoveCandidatesBasedOnFixedValues);
        
        return _numberOfRemovedCandidates>0;
    }

    private void RemoveCandidatesBasedOnFixedValues(Position position)
    {
        if (IsUndefined(position)) 
            return;
        
        ForEachCellInAreaExcept(GetIndicesForRow(position.Row), position, pos =>
        {
            RemoveCandidate(position, Candidates[pos.Row, pos.Column]);
        });
        ForEachCellInAreaExcept(GetIndicesForColumn(position.Column), position, pos =>
        {
            RemoveCandidate(position, Candidates[pos.Row, pos.Column]);
        });
        ForEachCellInAreaExcept(GetIndicesForBox(GetBoxIndex(position)), position, tuple  =>
        {
            RemoveCandidate(position, Candidates[tuple.Row,tuple.Column]);
        });
    }

    private void RemoveCandidate(Position position, BitArray digits)
    {
        var digit = Cells[position.Row, position.Column];
        
        if (digits[digit - 1])
        {
            digits[digit - 1] = false;
            _numberOfRemovedCandidates++;
        }
    }

    private static void ForEachCellInAreaExcept(List<Position> positions, Position position, Action<Position> action)
    {
        foreach (var pos in positions)
        {
            if (pos!=position)
                action(pos);
        }
    }
}