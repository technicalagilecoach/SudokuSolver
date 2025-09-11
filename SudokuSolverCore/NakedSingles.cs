namespace SudokuSolverCore;

using static IndicesAndIterators;
using static Puzzle;
public class NakedSingles(Puzzle puzzle) : Strategy(puzzle)
{
    public bool Handle()
    {
        var valueModified = false;
            
        ForEachCell(position =>
        {
            SelectUniqueValueForCell(position, ref valueModified);
        });

        return valueModified;
    }
    
    private void SelectUniqueValueForCell(Position position, ref bool valueModified)
    {
        if (!IsUndefined(position)) 
            return;

        if (CountCandidates(position) != 1)
            return;
            
        SetValue(position, out valueModified);            
    }
    
    private void SetValue(Position position, out bool valueModified)
    {
        valueModified = false;

        var index = 1;
        foreach (var pv in Candidates[position.Row, position.Column])
        {
            if ((bool)pv)
            {
                Candidates[position.Row, position.Column][index-1] = false;
                break;
            }

            index++;
        }
        
        Cells[position.Row,position.Column] = index;
        valueModified = true;
    }
}