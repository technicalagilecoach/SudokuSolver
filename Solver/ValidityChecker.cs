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
            var indices = GetIndicesForRow(row);
            if (!DistinctValuesInArea(cells, indices)) 
                return false;
        }        
        return true;
    }

    private static bool DistinctValuesInColumns(int[,] cells)
    {
        foreach (var column in AllColumns)
        {
            var indices = GetIndicesForColumn(column);
            if (!DistinctValuesInArea(cells, indices)) 
                return false;
        }        
        return true;
    }

    private static bool DistinctValuesInBoxes(int[,] cells)
    {
        foreach (var box in AllBoxes)
        {
            var indices = GetIndicesForBox(box);
            if (!DistinctValuesInArea(cells, indices)) 
                return false;
        }
        return true;
    }
    
    private static bool DistinctValuesInArea(int[,] cells, List<Position> indices)
    {
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

        return true;
    }

    public static bool CheckCandidates(Puzzle puzzle)
    {
        bool candidatesOk = true;
        
        foreach (var row in AllRows)
        {
            var positions = GetIndicesForRow(row);
            candidatesOk = candidatesOk && CheckCandidatesForArea(puzzle, positions);
        } 
        
        foreach (var column in AllColumns)
        {
            var positions = GetIndicesForColumn(column);
            candidatesOk = candidatesOk && CheckCandidatesForArea(puzzle, positions);
        } 
        
        foreach (var box in AllBoxes)
        {
            var positions = GetIndicesForBox(box);
            candidatesOk = candidatesOk && CheckCandidatesForArea(puzzle, positions);
        } 

        return candidatesOk;
    }

    private static bool CheckCandidatesForArea(Puzzle puzzle, List<Position> positions)
    {
        SortedSet<int> fixedDigits = new SortedSet<int>();
        SortedSet<int> candidateDigits = new SortedSet<int>();
            
        foreach (var pos in positions)
        {
            if (puzzle.IsUndefined(pos))
            {
                var candidates = puzzle.Candidates[pos.Row, pos.Column];
                foreach (var digit in AllDigits)
                {
                    if (candidates[digit])
                        candidateDigits.Add(digit);
                }
            }
            else
            {
                fixedDigits.Add(puzzle.Cells[pos.Row, pos.Column]-1);
            }
        }
            
        var allDigits = AllDigits.ToHashSet();
        bool haveCommonDigits = fixedDigits.Overlaps(candidateDigits);
        var encounteredDigits = fixedDigits.Union(candidateDigits).ToHashSet();
        bool allDigitsCovered = allDigits.SetEquals(encounteredDigits);
        bool error = haveCommonDigits || !allDigitsCovered;
        return error;
    }
}