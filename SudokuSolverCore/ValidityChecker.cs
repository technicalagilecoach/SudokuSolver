using static SudokuSolverCore.Grid;

namespace SudokuSolverCore;

internal static class ValidityChecker
{
    public static bool Check(Cell[,] cells)
    {
        return IsPuzzleSolved(cells) && IsSolutionCorrect(cells);
    }

    private static bool IsPuzzleSolved(Cell[,] cells)
    {
        return CountUndefinedCells(cells)==0;
    }

    private static bool IsSolutionCorrect(Cell[,] cells)
    {
        if (!DistinctValuesInRows(cells))
            return false;
        
        if (!DistinctValuesInColumns(cells))
            return false;
        
        if (!DistinctValuesInRegions(cells))
            return false;
        
        return true;
    }

    private static int CountUndefinedCells(Cell[,] cells)
    {
        var count = 0;
        
        foreach (var row in AllRows)
        {
            foreach (var column in AllColumns)
            {
                if (cells[row, column].Value == Undefined)
                    count++;
            }
        }

        return count;
    }
    
    private static bool DistinctValuesInRows(Cell[,] cells)
    {
        foreach (var row in AllRows)
        {
            var values = new SortedSet<int>();
            
            foreach (var column in AllColumns)
            {
                var value = cells[row, column].Value;
                var isNewValue = values.Add(value);
                if (!isNewValue)
                    return false;
            }
        }        
        return true;
    }
    
    private static bool DistinctValuesInColumns(Cell[,] cells)
    {
        foreach (var column in AllColumns)
        {
            var values = new SortedSet<int>();
            
            foreach (var row in AllRows)
            {
                var value = cells[row, column].Value;
                var isNewValue = values.Add(value);
                if (!isNewValue)
                    return false;
            }
        }        
        return true;
    }

    private static bool DistinctValuesInRegions(Cell[,] cells)
    {
        var regions = new List<(int row, int column)> { (1,1),(1,4),(1,7),(4,1),(4,4),(4,7),(7,1),(7,4),(7,7)};

        foreach (var region in regions)
        {
            var indices = GetIndicesForRegion(region.row, region.column);

            var values = new SortedSet<int>();
            
            foreach (var index in indices)
            {
                var value = cells[index.Item1, index.Item2].Value;
                var isNewValue = values.Add(value);
                if (!isNewValue)
                    return false;
            }
        }

        return true;
    }
}