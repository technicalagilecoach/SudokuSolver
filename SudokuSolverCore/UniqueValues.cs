using System.Collections;
using static SudokuSolverCore.Puzzle;

namespace SudokuSolverCore;

internal class UniqueValues(Cell[,] cells, BitArray[,] possibleValues)
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

        if (CountPotentialValues(possibleValues, row, column) != 1)
            return;
            
        SetValue(out valueModified, row, column);            
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
                var value = possibleValues[row, column];

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
                    if (cells[row, column].Value==Undefined && values[i] == 1 && possibleValues[row,column][i])
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
                var value = possibleValues[row, column];

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
                    if (cells[row, column].Value==Undefined && values[i] == 1 && possibleValues[row,column][i])
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
                var value = possibleValues[index.Item1, index.Item2];

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
                    if (cells[index.Item1, index.Item2].Value==Undefined && values[i] == 1 && possibleValues[index.Item1,index.Item2][i])
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

        if (CountPotentialValues(possibleValues, row, column) != 1)
            return;
            
        SetValue(out valueModified, row, column);            
    }
    
    private void SetValue(out bool valueModified,  int row, int column)
    {
        valueModified = false;

        var index = 1;
        foreach (var pv in possibleValues[row,column])
        {
            if ((bool)pv) 
                break;
            index++;
        }
        
        cells[row,column].Value = index;
        valueModified = true;
    }
}