using static SudokuSolver.IndicesAndIterators;
using static SudokuSolver.Puzzle;

namespace SudokuSolver;

internal class HiddenSingles(Puzzle puzzle) : Strategy(puzzle)
{
    public bool Handle()
    {
        var numberOfNewFixedCells = 0;
            
        foreach (var row in AllRows)
        {
            var positions = GetIndicesForRow(row);
            var distribution = CountDigitDistributionInArea(positions);
            ForEachCellInArea(positions, position => FixSingularValue(distribution, ref numberOfNewFixedCells, position));
        }     
        
        foreach (var column in AllColumns)
        {
            var positions = GetIndicesForColumn(column);
            var distribution = CountDigitDistributionInArea(positions);
            ForEachCellInArea(positions, position => FixSingularValue(distribution, ref numberOfNewFixedCells, position));
        } 
        
        foreach (var box in AllBoxes)
        {
            var positions = GetIndicesForBox(box);
            var distribution = CountDigitDistributionInArea(positions);
            ForEachCellInArea(positions, position => FixSingularValue(distribution, ref numberOfNewFixedCells, position));
        }

        return numberOfNewFixedCells>0;
    }

    private void FixSingularValue(int[] values, ref int numberOfNewFixedCells, Position position)
    {
        foreach (var digit in AllDigits)
        {
            var isSingularDigit = values[digit] == 1;
            if (IsUndefined(position) && isSingularDigit && IsCandidate(position, digit))
            {
                SetValue(position, digit);
                numberOfNewFixedCells++;
                break;
            }
        }
    }
}