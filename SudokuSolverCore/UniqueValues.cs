using System.Collections;
using static SudokuSolverCore.Puzzle;

namespace SudokuSolverCore;

internal class UniqueValues(int[,] cells, BitArray[,] possibleValues)
{
    public bool SetUniqueValues()
    {
        var valueModified = false;
            
        ForEachCell(position =>
        {
            SelectUniqueValueForCell(ref valueModified, position);
        });

        return valueModified;
    }
    
    private void SelectUniqueValueForCell(ref bool valueModified, Position position)
    {
        var currentCell = cells[position.Row, position.Column];
            
        var valueFixed = currentCell != Undefined;
        if (valueFixed) 
            return;

        if (CountPotentialValues(possibleValues, position) != 1)
            return;
            
        SetValue(out valueModified, position);            
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
                    if (cells[row, column]==Undefined && values[i] == 1 && possibleValues[row,column][i])
                    {
                        cells[row, column] = i + 1;
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
                    if (cells[row, column]==Undefined && values[i] == 1 && possibleValues[row,column][i])
                    {
                        cells[row, column] = i + 1;
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
                var value = possibleValues[index.Row, index.Row];

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
                    if (cells[index.Row, index.Row]==Undefined && values[i] == 1 && possibleValues[index.Row, index.Row][i])
                    {
                        cells[index.Row, index.Row] = i + 1;
                        valueModified = true;
                        break;
                    }
                }
            }
        }

        return valueModified;
    }
    
    // private void SelectHiddenUniqueValueForCell(ref bool valueModified, int row, int column)
    // {
    //     var currentCell = cells[row, column];
    //         
    //     var valueFixed = currentCell.Value != Undefined;
    //     if (valueFixed) 
    //         return;
    //
    //     if (CountPotentialValues(possibleValues, row, column) != 1)
    //         return;
    //         
    //     SetValue(out valueModified, row, column);            
    // }
    
    private void SetValue(out bool valueModified, Position position)
    {
        valueModified = false;

        var index = 1;
        foreach (var pv in possibleValues[position.Row,position.Column])
        {
            if ((bool)pv) 
                break;
            index++;
        }
        
        cells[position.Row,position.Column] = index;
        valueModified = true;
    }
}