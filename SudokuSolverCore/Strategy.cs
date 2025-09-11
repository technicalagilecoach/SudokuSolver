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
    
    protected BitArray GetCandidates(Position position)
    {
        return Candidates[position.Row, position.Column];
    }
}