using System.Collections;
using static SudokuSolver.IndicesAndIterators;

namespace SudokuSolver;

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

        GetCandidates(position).SetAll(false);
        
        var value = GetValue(position);
        
        ForEachCellInAreaExcept(GetIndicesForRow(position.Row), position, pos =>
        {
            RemoveCandidate(pos, value, ref _numberOfRemovedCandidates);
        });
        ForEachCellInAreaExcept(GetIndicesForColumn(position.Column), position, pos =>
        {
            RemoveCandidate(pos, value, ref _numberOfRemovedCandidates);
        });
        ForEachCellInAreaExcept(GetIndicesForBox(GetBoxIndex(position)), position, pos  =>
        {
            RemoveCandidate(pos, value, ref _numberOfRemovedCandidates);
        });
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