using System.Collections;
using System.Collections.Generic;
using static SudokuSolver.Puzzle;

namespace SudokuSolver;

public class Strategy(Puzzle puzzle)
{
    protected int[,] Cells => puzzle.Cells;
    protected BitArray[,] Candidates => puzzle.Candidates;

    protected bool IsUndefined(int row, int column)
    {
        return Cells[row, column] == Undefined;
    }

    protected bool IsUndefined(Position position)
    {
        return Cells[position.Row, position.Column] == Undefined;
    }

    protected bool IsFixed(Position position)
    {
        return !IsUndefined(position);
    }

    protected void SetValue(Position position, int digit)
    { 
        Cells[position.Row,position.Column] = digit+1;
        //Candidates[position.Row,position.Column].SetAll(false);
    }
    
    protected BitArray GetCandidates(Position position)
    {
        return Candidates[position.Row, position.Column];
    }
    
    protected bool IsCandidate(Position position, int digit)
    {
        return GetCandidates(position)[digit];
    }

    protected int CountCandidates(Position position)
    {
        var count = 0;
            
        foreach (bool bit in Candidates[position.Row, position.Column])
        {
            if (bit)
                count++;
        }

        return count;
    }
    
    protected int GetIndexOfFirstCandidate(Position position)
    {
        var index = 0;
        foreach (var pv in Candidates[position.Row, position.Column])
        {
            if ((bool)pv)
            {
                Candidates[position.Row, position.Column][index] = false;
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
                if (candidates[digit])
                    distribution[digit]++;
            }
        }
        
        return distribution;
    }
}