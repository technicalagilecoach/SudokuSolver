namespace SudokuSolver;

using static IndicesAndIterators;
using static Puzzle;
public class NakedSingles(Puzzle puzzle) : Strategy(puzzle)
{
    public bool Handle()
    {
        var numberOfNewFixedCells = 0;
        
        ForEachCell(position =>
        {
            SelectUniqueValueForCell(position, ref numberOfNewFixedCells);
        });

        return numberOfNewFixedCells > 0;
    }
    
    private void SelectUniqueValueForCell(Position position, ref int count)
    {
        if (IsFixed(position)) 
            return;

        if (CountCandidates(position) != 1)
            return;

        SetValue(position, GetIndexOfFirstCandidate(position));

        count++;
    }
}