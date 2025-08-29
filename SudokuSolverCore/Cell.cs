using static SudokuSolverCore.Grid;

namespace SudokuSolverCore
{
    internal class Cell
    {
        private Cell(int value)
        {
            Value = value;
            PotentialValues = [];
            InitializePotentialValues();
        }

        public void InitializePotentialValues()
        {
            if (Value == Undefined)
            {
                for (var i = 1; i <= GridSize; i++)
                    PotentialValues[i] = true;
            }
            else
            {
                for (var i = 1; i <= GridSize; i++)
                    PotentialValues[i] = false;
                PotentialValues[Value] = true;
            }
        }

        public int Value { get; set; }
        public Dictionary<int, bool> PotentialValues { get; set; }

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
