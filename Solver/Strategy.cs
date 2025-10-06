using System.Collections;
using static SudokuSolver.Puzzle;

namespace SudokuSolver;

public class Strategy(Puzzle puzzle)
{
    //protected int[,] Cells => puzzle.Cells;
    //protected BitArray[,] Candidates => puzzle.Candidates;

    protected bool IsUndefined(int row, int column)
    {
        return puzzle.GetCellValue(new Position(row,column))== Undefined;
    }

    protected bool IsUndefined(Position position)
    {
        return puzzle.GetCellValue(position) == Undefined;
    }

    protected bool IsFixed(Position position)
    {
        return !IsUndefined(position);
    }
    
    protected void SetValue(Position position, int value)
    { 
        puzzle.SetCellValue(position, value);
    }
    
    protected int GetValue(Position position)
    { 
        return puzzle.GetCellValue(position);
    }
    
    protected BitArray GetCandidates(Position position)
    {
        return puzzle.GetCandidates(position);
    }
    
    protected void RemoveCandidates(Position position, List<int> valuesToRemove, ref int numberOfRemovedCandidates)
    {
        if (IsUndefined(position))
        {
            var candidates = GetCandidates(position);
            foreach (var digit in valuesToRemove)
            {
                if (candidates[digit-1])
                {
                    candidates[digit-1] = false;
                    numberOfRemovedCandidates++;
                }
            }
        }
    }
    
    protected void RemoveCandidate(Position position, int value, ref int numberOfRemovedCandidates)
    {
        if (IsUndefined(position))
        {
            var candidates = GetCandidates(position);
            if (candidates[value-1])
            {
                candidates[value-1] = false;
                numberOfRemovedCandidates++;
            }
        }
    }
    
    protected bool IsCandidate(Position position, int digit)
    {
        return GetCandidates(position)[digit-1];
    }

    protected int CountCandidates(Position position)
    {
        var count = 0;
            
        foreach (bool bit in GetCandidates(position))
        {
            if (bit)
                count++;
        }

        return count;
    }
    
    protected int GetValueOfFirstCandidate(Position position)
    {
        var index = 1;
        foreach (var pv in GetCandidates(position))
        {
            if ((bool)pv)
            {
                //GetCandidates(position)[index] = false;
                break;
            }

            index++;
        }

        return index;
    }
    
    protected int[] CountDigitDistributionInArea(List<Position> positions)
    {
        var distribution = new int[GridSize];
        
        foreach (var position in positions)
        {
            var candidates = GetCandidates(position);
            foreach (var digit in AllDigits)
            {
                if (candidates[digit-1])
                    distribution[digit-1]++;
            }
        }
        
        return distribution;
    }
}