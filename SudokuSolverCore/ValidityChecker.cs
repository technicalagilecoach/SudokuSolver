using static SudokuSolverCore.IndicesAndIterators;
using static SudokuSolverCore.Puzzle;

namespace SudokuSolverCore;

internal static class ValidityChecker
{
    public static bool Check(int[,] cells)
    {
        return IsPuzzleSolved(cells) && IsSolutionCorrect(cells);
    }

    private static bool IsPuzzleSolved(int[,] cells)
    {
        return CountUndefinedCells(cells)==0;
    }

    private static bool IsSolutionCorrect(int[,] cells)
    {
        if (!DistinctValuesInRows(cells))
            return false;
        
        if (!DistinctValuesInColumns(cells))
            return false;
        
        if (!DistinctValuesInRegions(cells))
            return false;
        
        return true;
    }

    internal static int CountUndefinedCells(int[,] cells)
    {
        var count = 0;
        
        foreach (var pos in GetIndicesForAllCells())
        {
            if (cells[pos.Row, pos.Column] == Undefined)
                count++;
        }

        return count;
    }
    
    private static bool DistinctValuesInRows(int[,] cells)
    {
        foreach (var row in AllDigits)
        {
            var values = new SortedSet<int>();
            
            foreach (var column in AllDigits)
            {
                var value = cells[row, column];
                var isNewValue = values.Add(value);
                if (!isNewValue)
                    return false;
            }
        }        
        return true;
    }
    
    private static bool DistinctValuesInColumns(int[,] cells)
    {
        foreach (var column in AllDigits)
        {
            var values = new SortedSet<int>();
            
            foreach (var row in AllDigits)
            {
                var value = cells[row, column];
                var isNewValue = values.Add(value);
                if (!isNewValue)
                    return false;
            }
        }        
        return true;
    }

    private static bool DistinctValuesInRegions(int[,] cells)
    {
        foreach (var region in AllDigits)
        {
            var indices = GetIndicesForRegion(region);

            var values = new SortedSet<int>();
            
            foreach (var index in indices)
            {
                var value = cells[index.Row, index.Column];
                var isNewValue = values.Add(value);
                if (!isNewValue)
                    return false;
            }
        }

        return true;
    }
}