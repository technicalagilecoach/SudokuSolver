namespace SudokuSolverCore;

using static IndicesAndIterators;
using static Puzzle;
public class NakedSingles(Puzzle puzzle) : Strategy(puzzle)
{
    public bool Handle()
    {
        var count = 0;
        
        ForEachCell(position =>
        {
            SelectUniqueValueForCell(position, ref count);
        });

        return count > 0;
    }
    
    private void SelectUniqueValueForCell(Position position, ref int count)
    {
        if (!IsUndefined(position)) 
            return;

        if (CountCandidates(position) != 1)
            return;

        SetValue(position, GetDigitOfFirstCandidate(position));

        count++;
    }
}