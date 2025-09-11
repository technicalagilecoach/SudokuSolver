using System.Collections;

namespace SudokuSolverCore;

public class Strategy(Puzzle puzzle)
{
    protected int[,] Cells => puzzle.Cells;
    protected BitArray[,] Candidates => puzzle.Candidates;
    
    protected bool IsUndefined(int row, int column)
    {
        return Cells[row, column] == Puzzle.Undefined;
    }
    
    protected bool IsUndefined(Position position)
    {
        return Cells[position.Row, position.Column]==Puzzle.Undefined;
    }

    protected void SetValue(Position position, int digit)
    {
        Cells[position.Row,position.Column] = digit;
        Candidates[position.Row,position.Column].SetAll(false);
    }
    
    protected BitArray GetCandidates(Position position)
    {
        return Candidates[position.Row, position.Column];
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
    
    protected int GetDigitOfFirstCandidate(Position position)
    {
        var index = 1;
        foreach (var pv in Candidates[position.Row, position.Column])
        {
            if ((bool)pv)
            {
                Candidates[position.Row, position.Column][index-1] = false;
                break;
            }

            index++;
        }

        return index;
    }
}