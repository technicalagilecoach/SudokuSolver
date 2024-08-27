using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static SudokuSolverCore.SudokuPuzzle;

namespace SudokuSolverCore
{
    internal class Tile
    {
        public Tile(int value)
        {
            Value = value;
            PotentialValues = [];
            InitializePotentialValues();
        }

        public void InitializePotentialValues()
        {
            if (Value == UNDEFINED)
            {
                for (int i = 1; i <= PUZZLE_SIZE; i++)
                    PotentialValues[i] = true;
            }
            else
            {
                for (int i = 1; i <= PUZZLE_SIZE; i++)
                    PotentialValues[i] = false;
                PotentialValues[Value] = true;
            }
        }

        public int Value { get; set; }
        public Dictionary<int, bool> PotentialValues { get; set; }

        public static implicit operator Tile(string value)
        {
            return new Tile(TileHelpers.StringToInt(value));
        }

        public static implicit operator string(Tile tile)
        {
            return TileHelpers.IntToString(tile.Value);
        }
    }
}
