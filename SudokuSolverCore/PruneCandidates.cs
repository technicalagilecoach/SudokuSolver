using System.Collections;
using static SudokuSolverCore.IndicesAndIterators;

namespace SudokuSolverCore;

public class PruneCandidates(Puzzle puzzle) : Strategy(puzzle)
{
    public bool Handle()
    {
        var candidatesModified = false;
        
        ForEachCell(position => 
            RemoveCandidatesBasedOnFixedValues(position, ref candidatesModified)
        );
        
        return candidatesModified;
    }

    private void RemoveCandidatesBasedOnFixedValues(Position position, ref bool candidatesModified)
    {
        if (IsUndefined(position)) 
            return;

        bool modified = candidatesModified;
        
        ForEachCellInRowExcept(position.Column, column =>
        {
            RemoveCandidate(position, Candidates[position.Row, column], out modified);
        });
        ForEachCellInColumnExcept(position.Row, row =>
        {
            RemoveCandidate(position, Candidates[row, position.Column], out modified);
        });
        ForEachCellInBoxExcept(position, tuple =>
        {
            RemoveCandidate(position, Candidates[tuple.Row,tuple.Column], out modified);
        });
        
        candidatesModified = candidatesModified || modified;
    }

    private void RemoveCandidate(Position position, BitArray digits, out bool candidatesModified)
    {
        var digit = Cells[position.Row, position.Column];

        candidatesModified = false;
        if (digits[digit - 1])
        {
            digits[digit - 1] = false;
            candidatesModified = true;
        }
    }
    
}