using System.Collections;
using static SudokuSolverCore.IndicesAndIterators;

namespace SudokuSolverCore;

public class PruneCandidates(Puzzle puzzle) : Strategy(puzzle)
{
    public bool Handle()
    {
        ForEachCell(RemoveCandidatesBasedOnSingle);
        return false;
    }

    private void RemoveCandidatesBasedOnSingle(Position position)
    {
        if (IsUndefined(position)) 
            return;

        ForEachCellInRowExcept(position.Column, column =>
        {
            RemoveCandidate(position, Candidates[position.Row, column]);
        });
        ForEachCellInColumnExcept(position.Row, row =>
        {
            RemoveCandidate(position, Candidates[row, position.Column]);
        });
        ForEachCellInBoxExcept(position, tuple =>
        {
            RemoveCandidate(position, Candidates[tuple.Row,tuple.Column]);
        });
    }

    private void RemoveCandidate(Position position, BitArray digits)
    {
        var digit = Cells[position.Row, position.Column];  
        digits[digit-1] = false;
    }
    
}