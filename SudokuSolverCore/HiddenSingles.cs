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
            var distribution = CountDigitDistributionInArea(positions);
            ForEachCellInArea(positions, position => FixSingularValue(distribution, out valueModified, position));
        }     
        
        foreach (var column in AllColumns)
        {
            var positions = GetIndicesForColumn(column);
            var distribution = CountDigitDistributionInArea(positions);
            ForEachCellInArea(positions, position => FixSingularValue(distribution, out valueModified, position));
        } 
        
        foreach (var box in AllBoxes)
        {
            var positions = GetIndicesForBox(box);
            var distribution = CountDigitDistributionInArea(positions);
            ForEachCellInArea(positions, position => FixSingularValue(distribution, out valueModified, position));
        }

        return valueModified;
    }

    private void FixSingularValue(int[] values, out bool valueModified, Position position)
    {
        foreach (var digit in AllDigits)
        {
            var isSingularDigit = values[digit] == 1;
            if (IsUndefined(position) && isSingularDigit && IsCandidate(position, digit))
            {
                SetValue(position, digit);
                valueModified = true;
                break;
            }
        }

        valueModified = false;
    }
}