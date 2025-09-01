using static SudokuSolverCore.Grid;

namespace SudokuSolverCore;

internal class UniqueValues(Cell[,] cells)
{
    public bool SetUniqueValues()
    {
        var valueModified = false;
            
        ForEachCell((row, column) =>
        {
            SelectUniqueValueForCell(ref valueModified, row, column);
        });

        return valueModified;
    }

    private void SelectUniqueValueForCell(ref bool valueModified, int row, int column)
    {
        var currentCell = cells[row, column];
            
        var valueFixed = currentCell.Value != Undefined;
        if (valueFixed) 
            return;

        if (currentCell.CountPotentialValues() != 1)
            return;
            
        SetValue(out valueModified, currentCell);            
    }

    private static void SetValue(out bool valueModified, Cell currentCell)
    {
        valueModified = false;

        var i = 0;
        for (; i < HighestNumber; i++)
        {
            if (currentCell.PotentialValues[i]) 
                break;
        }
        
        currentCell.Value = i+1;
        valueModified = true;
    }
}