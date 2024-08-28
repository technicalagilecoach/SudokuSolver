using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static SudokuSolverCore.Grid;

namespace SudokuSolverCore
{
    internal class Cell
    {
        public Cell(int value)
        {
            Value = value;
            PotentialValues = [];
            InitializePotentialValues();
        }

        public void InitializePotentialValues()
        {
            if (Value == UNDEFINED)
            {
                for (int i = 1; i <= GRID_SIZE; i++)
                    PotentialValues[i] = true;
            }
            else
            {
                for (int i = 1; i <= GRID_SIZE; i++)
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
