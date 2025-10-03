using static SudokuSolver.IndicesAndIterators;
using static SudokuSolver.Puzzle;

namespace SudokuSolver;

internal class HiddenSingles(Puzzle puzzle) : Strategy(puzzle)
{
    private int _numberOfNewFixedCells;
    public bool Handle()
    {
        _numberOfNewFixedCells = 0;
            
        foreach (var row in AllRows)
        {
            var positions = GetIndicesForRow(row);
            var distribution = CountDigitDistributionInArea(positions);
            ForEachCellInArea(positions, position => FixSingularValue(distribution, position));
        }     
        
        foreach (var column in AllColumns)
        {
            var positions = GetIndicesForColumn(column);
            var distribution = CountDigitDistributionInArea(positions);
            ForEachCellInArea(positions, position => FixSingularValue(distribution, position));
        } 
        
        foreach (var box in AllBoxes)
        {
            var positions = GetIndicesForBox(box);
            var distribution = CountDigitDistributionInArea(positions);
            ForEachCellInArea(positions, position => FixSingularValue(distribution, position));
        }

        return _numberOfNewFixedCells>0;
    }

    private void FixSingularValue(int[] values, Position position)
    {
        foreach (var digit in AllDigits)
        {
            var isSingularDigit = values[digit] == 1;
            if (IsUndefined(position) && isSingularDigit && IsCandidate(position, digit))
            {
                SetValue(position, digit);
                _numberOfNewFixedCells++;
                break;
            }
        }
    }
}