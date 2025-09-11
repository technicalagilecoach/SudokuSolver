using System.Collections;

namespace SudokuSolverCore;

public class Strategy(Puzzle puzzle)
{
    protected int[,] Cells => puzzle.Cells;
    protected BitArray[,] Candidates => puzzle.Candidates;
    
    protected bool IsUndefined(Position position)
    {
        return Cells[position.Row, position.Column]==Puzzle.Undefined;
    }
}