using System.Collections.Generic;
using static SudokuSolver.IndicesAndIterators;
using static SudokuSolver.Puzzle;

namespace SudokuSolver;

public static class ValidityChecker
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
        
        if (!DistinctValuesInBoxes(cells))
            return false;
        
        return true;
    }

    public static int CountUndefinedCells(int[,] cells)
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

    private static bool DistinctValuesInBoxes(int[,] cells)
    {
        foreach (var box in AllBoxes)
        {
            var indices = GetIndicesForBox(box);

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