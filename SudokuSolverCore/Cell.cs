using System.Collections;

using static SudokuSolverCore.Grid;

namespace SudokuSolverCore
{
    internal class Cell
    {
        private Cell(int value)
        {
            Value = value;
            PotentialValues = InitializePotentialValues();
        }
        
        public int Value { get; set; }
        public BitArray PotentialValues { get; set; }
        
        private BitArray InitializePotentialValues()
        {
            var potentialValues = new BitArray(GridSize);
            
            if (Value == Undefined)
            {
                potentialValues.SetAll(true);
            }
            else
            {
                potentialValues.SetAll(false);
                potentialValues[Value-1] = true;
            }
            
            return potentialValues;
        }

        public static implicit operator Cell(string value)
        {
            return new Cell(value == " " ? Undefined : int.Parse(value));
        }

        public static implicit operator string(Cell tile)
        {
            return tile.Value == Undefined ? " " : tile.Value.ToString();
        }

        public int CountPotentialValues()
        {
            var count = 0;
            
            foreach (bool bit in PotentialValues)
            {
                if (bit)
                    count++;
            }

            return count;
        }

        public bool IsEqualTo(Cell cell)
        {
            bool equals = true;
            
            foreach (var i in AllDigits)
            {
                if (PotentialValues[i] == cell.PotentialValues[i]) 
                    continue;
                
                equals = false;
                break;
            }

            return equals;
        }
    }
}
