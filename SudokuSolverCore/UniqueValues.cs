using System.Collections;
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
        var currentCell = puzzle.Cells[position.Row, position.Column];
            
        var valueFixed = currentCell != Undefined;
        if (valueFixed) 
            return;

        if (CountCandidates(puzzle.Candidates, position) != 1)
            return;
            
        SetValue(out valueModified, position);            
    }

    public bool SetHiddenUniqueValues()
    {
        var valueModified = false;
            
        foreach (var row in AllRows)
        {
            var positions = GetIndicesForRow(row);
          
            var values = CountDigitsInArea(positions);
            valueModified = UpdateValues(positions, values, valueModified);
        }     
        
        foreach (var column in AllColumns)
        {
            var positions = GetIndicesForColumn(column);
            
            var values = CountDigitsInArea(positions);
            valueModified = UpdateValues(positions, values, valueModified);
        } 
        
        foreach (var region in AllRegions)
        {
            var positions = GetIndicesForRegion(region+1);
            
            var values = CountDigitsInArea(positions);
            valueModified = UpdateValues(positions, values, valueModified);
        }

        return valueModified;
    }

    private bool UpdateValues(List<Position> positions, int[] values, bool valueModified)
    {
        foreach (var position in positions)
        {
            foreach (var digit in AllDigits)
            {
                if (IsUndefined(position) && values[digit] == 1 && Candidate(position)[digit])
                {
                    SetValue(position, digit);
                    valueModified = true;
                    break;
                }
            }
        }

        return valueModified;
    }

    private int[] CountDigitsInArea(List<Position> positions)
    {
        var values = new int[GridSize];
        
        foreach (var position in positions)
        {
            var value = Candidate(position);
            foreach (var digit in AllDigits)
            {
                if (value[digit])
                    values[digit]++;
            }
        }
        
        return values;
    }

    private int SetValue(Position position, int digit)
    {
        return puzzle.Cells[position.Row, position.Column] = digit + 1;
    }

    private BitArray Candidate(Position position)
    {
        return puzzle.Candidates[position.Row, position.Column];
    }

    private bool IsUndefined(Position position)
    {
        return puzzle.Cells[position.Row, position.Column]==Undefined;
    }
    
    private void SetValue(out bool valueModified, Position position)
    {
        valueModified = false;

        var index = 1;
        foreach (var pv in puzzle.Candidates[position.Row,position.Column])
        {
            if ((bool)pv) 
                break;
            index++;
        }
        
        puzzle.Cells[position.Row,position.Column] = index;
        valueModified = true;
    }
}