using System.Collections;

using static SudokuSolverCore.Grid;

namespace SudokuSolverCore
{
    internal class Cell
    {
        private Cell(int value)
        {
            Value = value;
            PotentialValues = new BitArray(9);
            InitializePotentialValues();
        }

        public void InitializePotentialValues()
        {
            if (Value == Undefined)
            {
                for (var i = 1; i <= GridSize; i++)
                    PotentialValues[i-1] = true;
            }
            else
            {
                for (var i = 1; i <= GridSize; i++)
                    PotentialValues[i-1] = false;
                PotentialValues[Value-1] = true;
            }
        }

        public int Value { get; set; }
        public BitArray PotentialValues { get; set; }

        public static implicit operator Cell(string value)
        {
            return new Cell(CellHelpers.StringToInt(value));
        }

        public static implicit operator string(Cell tile)
        {
            return CellHelpers.IntToString(tile.Value);
        }
    }
}
