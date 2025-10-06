namespace SudokuSolver;

using static IndicesAndIterators;

public class NakedSingles(Puzzle puzzle) : Strategy(puzzle)
{
    private int _numberOfNewFixedCells;
    
    public bool Handle()
    {
        _numberOfNewFixedCells = 0;
        
        ForEachCell(SelectUniqueValueForCell);

        return _numberOfNewFixedCells > 0;
    }
    
    private void SelectUniqueValueForCell(Position position)
    {
        if (IsFixed(position)) 
            return;

        if (CountCandidates(position) != 1)
            return;

        SetValue(position, GetValueOfFirstCandidate(position));

        _numberOfNewFixedCells++;
    }
}