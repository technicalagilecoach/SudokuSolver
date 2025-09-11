using static SudokuSolverCore.IndicesAndIterators;
using static SudokuSolverCore.Puzzle;

namespace SudokuSolverCore;

internal class HiddenSingles(Puzzle puzzle) : Strategy(puzzle)
{
    public bool Handle()
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
}