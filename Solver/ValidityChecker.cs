using static SudokuSolver.IndicesAndIterators;
using static SudokuSolver.Puzzle;

namespace SudokuSolver;

public static class ValidityChecker
{
    public static bool Check(Puzzle puzzle)
    {
        return IsPuzzleSolved(puzzle) && IsSolutionCorrect(puzzle);
    }

    private static bool IsPuzzleSolved(Puzzle puzzle)
    {
        return CountUndefinedCells(puzzle)==0;
    }

    public static bool IsSolutionCorrect(Puzzle cells)
    {
        if (!DistinctValuesInRows(cells))
            return false;
        
        if (!DistinctValuesInColumns(cells))
            return false;
        
        if (!DistinctValuesInBoxes(cells))
            return false;
        
        return true;
    }

    public static int CountUndefinedCells(Puzzle puzzle)
    {
        var count = 0;
        
        foreach (var pos in GetIndicesForAllCells())
        {
            if (puzzle.Cells[pos.Row, pos.Column] == Undefined)
                count++;
        }

        return count;
    }
    
    private static bool DistinctValuesInRows(Puzzle puzzle)
    {
        foreach (var row in AllRows)
        {
            var indices = GetIndicesForRow(row);
            if (!DistinctValuesInArea(puzzle, indices)) 
                return false;
        }        
        return true;
    }

    private static bool DistinctValuesInColumns(Puzzle puzzle)
    {
        foreach (var column in AllColumns)
        {
            var indices = GetIndicesForColumn(column);
            if (!DistinctValuesInArea(puzzle, indices)) 
                return false;
        }        
        return true;
    }

    private static bool DistinctValuesInBoxes(Puzzle puzzle)
    {
        foreach (var box in AllBoxes)
        {
            var indices = GetIndicesForBox(box);
            if (!DistinctValuesInArea(puzzle, indices)) 
                return false;
        }
        return true;
    }
    
    private static bool DistinctValuesInArea(Puzzle puzzle, List<Position> indices)
    {
        var values = new SortedSet<int>();
        foreach (var index in indices)
        {
            var value = puzzle.Cells[index.Row, index.Column];
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