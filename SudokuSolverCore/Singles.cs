using System.Collections;
using System.Diagnostics;
using static SudokuSolverCore.IndicesAndIterators;
using static SudokuSolverCore.Puzzle;

namespace SudokuSolverCore;

internal class Singles(Puzzle puzzle) : Strategy(puzzle)
{
    public bool HandleNakedSingles()
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
        if (!IsUndefined(position)) 
            return;

        if (CountCandidates(position, Candidates) != 1)
            return;
            
        SetValue(position, out valueModified);            
    }

    public bool HandleHiddenSingles()
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
            var positions = GetIndicesForRegion(region);
            
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
                if (IsUndefined(position) && values[digit] == 1 && GetCandidates(position)[digit])
                {
                    Cells[position.Row, position.Column] = digit + 1;
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
            var value = GetCandidates(position);
            foreach (var digit in AllDigits)
            {
                if (value[digit])
                    values[digit]++;
            }
        }
        
        return values;
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