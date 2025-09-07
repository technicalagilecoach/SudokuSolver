using static SudokuSolverCore.IndicesAndIterators;
using static SudokuSolverCore.Puzzle;

namespace SudokuSolverCore;

internal class UniqueValues(Puzzle puzzle)
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
        var currentCell = puzzle.GetCells()[position.Row, position.Column];
            
        var valueFixed = currentCell != Undefined;
        if (valueFixed) 
            return;

        if (CountPossibleValues(puzzle.PossibleValues, position) != 1)
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
                var value = puzzle.PossibleValues[row, column];

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
                    if (IsUndefined(row, column) && values[i] == 1 && puzzle.PossibleValues[row,column][i])
                    {
                        puzzle.GetCells()[row, column] = i + 1;
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
                var value = puzzle.PossibleValues[row, column];

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
                    if (IsUndefined(row, column) && values[i] == 1 && puzzle.PossibleValues[row,column][i])
                    {
                        puzzle.GetCells()[row, column] = i + 1;
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
                var value = puzzle.PossibleValues[index.Row, index.Row];

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
                    var row = index.Row;
                    var column = index.Column;
                    
                    if (IsUndefined(row, column) && values[i] == 1 && puzzle.PossibleValues[row, column][i])
                    {
                        puzzle.GetCells()[row, column] = i + 1;
                        valueModified = true;
                        break;
                    }
                }
            }
        }

        return valueModified;
    }

    private bool IsUndefined(int row, int column)
    {
        return puzzle.GetCells()[row, column]==Undefined;
    }
    
    private void SetValue(out bool valueModified, Position position)
    {
        valueModified = false;

        var index = 1;
        foreach (var pv in puzzle.PossibleValues[position.Row,position.Column])
        {
            if ((bool)pv) 
                break;
            index++;
        }
        
        puzzle.GetCells()[position.Row,position.Column] = index;
        valueModified = true;
    }
}