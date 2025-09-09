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

    public static bool IsSolutionCorrect(int[,] cells)
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
        foreach (var row in AllRows)
        {
            var values = new SortedSet<int>();
            
            foreach (var column in AllColumns)
            {
                var value = cells[row, column];
                if (value != Undefined)
                {
                    var isNewValue = values.Add(value);
                    if (!isNewValue)
                        return false;
                }
            }
        }        
        return true;
    }
    
    private static bool DistinctValuesInColumns(int[,] cells)
    {
        foreach (var column in AllColumns)
        {
            var values = new SortedSet<int>();
            
            foreach (var row in AllRows)
            {
                var value = cells[row, column];
                if (value != Undefined)
                {
                    var isNewValue = values.Add(value);
                    if (!isNewValue)
                        return false;
                }
            }
        }        
        return true;
    }

    private static bool DistinctValuesInRegions(int[,] cells)
    {
        foreach (var region in AllRegions)
        {
            var indices = GetIndicesForRegion(region);

            var values = new SortedSet<int>();
            
            foreach (var index in indices)
            {
                var value = cells[index.Row, index.Column];
                if (value != Undefined)
                {
                    var isNewValue = values.Add(value);
                    if (!isNewValue)
                        return false;
                }
            }
        }

        return true;
    }
}