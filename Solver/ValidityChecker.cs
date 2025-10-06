using static SudokuSolver.IndicesAndIterators;
using static SudokuSolver.Puzzle;

namespace SudokuSolver;

public static class ValidityChecker
{
    public static bool Check(Puzzle puzzle)
    {
        return IsPuzzleSolved(puzzle) && IsPuzzleConsistent(puzzle);
    }

    private static bool IsPuzzleSolved(Puzzle puzzle)
    {
        return CountUndefinedCells(puzzle)==0;
    }

    public static bool IsPuzzleConsistent(Puzzle puzzle)
    {
        return AreFixedValuesConsistent(puzzle) && AreCandidatesConsistent(puzzle);
    }

    public static bool AreFixedValuesConsistent(Puzzle cells)
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
            if (puzzle.IsUndefined(pos))
                count++;
        }

        return count;
    }
    
    private static bool DistinctValuesInRows(Puzzle puzzle)
    {
        foreach (var row in AllRows)
        {
            var positions = GetIndicesForRow(row);
            if (!DistinctValuesInArea(puzzle, positions)) 
                return false;
        }        
        return true;
    }

    private static bool DistinctValuesInColumns(Puzzle puzzle)
    {
        foreach (var column in AllColumns)
        {
            var positions = GetIndicesForColumn(column);
            if (!DistinctValuesInArea(puzzle, positions)) 
                return false;
        }        
        return true;
    }

    private static bool DistinctValuesInBoxes(Puzzle puzzle)
    {
        foreach (var box in AllBoxes)
        {
            var positions = GetIndicesForBox(box);
            if (!DistinctValuesInArea(puzzle, positions)) 
                return false;
        }
        return true;
    }
    
    private static bool DistinctValuesInArea(Puzzle puzzle, List<Position> positions)
    {
        var values = new SortedSet<int>();
        foreach (var position in positions)
        {
            if (!puzzle.IsUndefined(position)) {
                var value = puzzle.GetCellValue(position);
                var isNewValue = values.Add(value);
                if (!isNewValue)
                    return false;
            }
        }

        return true;
    }

    public static bool AreCandidatesConsistent(Puzzle puzzle)
    {
        foreach (var row in AllRows)
        {
            var positions = GetIndicesForRow(row);
            if (!AreCandidatesConsistentInArea(puzzle, positions))
                return false;
        } 
        
        foreach (var column in AllColumns)
        {
            var positions = GetIndicesForColumn(column);
            if (!AreCandidatesConsistentInArea(puzzle, positions))
                return false;
        } 
        
        foreach (var box in AllBoxes)
        {
            var positions = GetIndicesForBox(box);
            if (!AreCandidatesConsistentInArea(puzzle, positions))
                return false;
        } 

        return true;
    }

    private static bool AreCandidatesConsistentInArea(Puzzle puzzle, List<Position> positions)
    {
        var (fixedDigits,candidateDigits) = CollectCandidatesAndFixedDigits(puzzle, positions);

        var haveCommonDigits = fixedDigits.Overlaps(candidateDigits);
        var allDigitsCovered = AllDigits.ToHashSet().SetEquals(fixedDigits.Union(candidateDigits));
        var consistent = !haveCommonDigits && allDigitsCovered;
        
        return consistent;
    }

    private static (HashSet<int>,HashSet<int>) CollectCandidatesAndFixedDigits(Puzzle puzzle, List<Position> positions)
    {
        var fixedDigits = new HashSet<int>();
        var candidateDigits = new HashSet<int>();
            
        foreach (var position in positions)
        {
            if (puzzle.IsUndefined(position))
            {
                var candidates = puzzle.GetCandidates(position);
                foreach (var digit in AllDigits)
                {
                    if (candidates[digit-1])
                        candidateDigits.Add(digit);
                }
            }
            else
            {
                fixedDigits.Add(puzzle.GetCellValue(position));
            }
        }

        return (fixedDigits,candidateDigits);
    }
}