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
        int originalValue;
        public Dictionary<int, bool> potentialValues;

        public Tile(int value)
        {
            this.originalValue = value;
            potentialValues = [];
            InitializePotentialValues();
        }

        public void InitializePotentialValues()
        {
            int value = this.originalValue;

            if (value == UNDEFINED)
            {
                for (int i = 1; i <= PUZZLE_SIZE; i++)
                    potentialValues[i] = true;
            }
            else
            {
                for (int i = 1; i <= PUZZLE_SIZE; i++)
                    potentialValues[i] = false;
                potentialValues[value] = true;
            }
        }

        public int GetValue() { return this.originalValue; }

        public void SetValue(int v) { this.originalValue = v; }

        public static int StringToInt(string simplePuzzle1)
        {
            int result = UNDEFINED;
            var ch = simplePuzzle1.ToString();

            if (ch != " ")
                result = Int32.Parse(ch);
            return result;
        }

        public static string IntToString(int value)
        {
            string s;
            if (value == UNDEFINED)
                s = " ";
            else
                s = value.ToString();
            return s;
        }

        public static implicit operator Tile(string v)
        {
            return new Tile(StringToInt(v));
        }

        public static implicit operator string(Tile v)
        {
            return IntToString(v.originalValue);
        }
    }
}
