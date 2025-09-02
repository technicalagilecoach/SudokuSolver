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

    public bool SetHiddenUniqueValues()
    {
        var valueModified = false;
            
        //rows
        foreach (var row in AllDigits)
        {
            var values = new int[HighestNumber];
            
            foreach (var column in AllDigits)
            {
                var value = cells[row, column].PotentialValues;

                foreach (var i in AllDigits)
                {
                    if (value[i])
                        values[i]++;
                }
            }

            foreach (var column in AllDigits)
            {
                foreach (var i in AllDigits)
                {
                    if (cells[row, column].Value==Undefined && values[i] == 1 && cells[row,column].PotentialValues[i])
                    {
                        cells[row, column].Value = i + 1;
                        valueModified = true;
                        break;
                    }
                }
            }
        }     
        
        //columns
        foreach (var column in AllDigits)
        {
            var values = new int[HighestNumber];
            
            foreach (var row in AllDigits)    
            {
                var value = cells[row, column].PotentialValues;

                foreach (var i in AllDigits)
                {
                    if (value[i])
                        values[i]++;
                }
            }

            foreach (var row in AllDigits)
            {
                foreach (var i in AllDigits)
                {
                    if (cells[row, column].Value==Undefined && values[i] == 1 && cells[row,column].PotentialValues[i])
                    {
                        cells[row, column].Value = i + 1;
                        valueModified = true;
                        break;
                    }
                }
            }
        } 
        
        //regions
        foreach (var region in AllDigits)
        {
            var indices = GetIndicesForRegion(region);
            
            var values = new int[HighestNumber];
            
            foreach (var index in indices)
            {
                var value = cells[index.Item1, index.Item2].PotentialValues;

                foreach (var i in AllDigits)
                {
                    if (value[i])
                        values[i]++;
                }
            }

            foreach (var index in indices)
            {
                foreach (var i in AllDigits)
                {
                    if (cells[index.Item1, index.Item2].Value==Undefined && values[i] == 1 && cells[index.Item1,index.Item2].PotentialValues[i])
                    {
                        cells[index.Item1, index.Item2].Value = i + 1;
                        valueModified = true;
                        break;
                    }
                }
            }
        }

        return valueModified;
    }
    
    private void SelectHiddenUniqueValueForCell(ref bool valueModified, int row, int column)
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