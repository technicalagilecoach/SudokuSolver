using System.Collections;

using static SudokuSolverCore.Puzzle;

namespace SudokuSolverCore
{
    public class Cell
    {
        private Cell(int value)
        {
            Value = value;
        }
        
        public int Value { get; set; }
        
        public static implicit operator Cell(string value)
        {
            return new Cell(value == " " ? Undefined : int.Parse(value));
        }

        public static implicit operator string(Cell tile)
        {
            return tile.Value == Undefined ? " " : tile.Value.ToString();
        }
    }
}
