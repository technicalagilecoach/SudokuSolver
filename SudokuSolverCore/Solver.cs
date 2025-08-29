using static SudokuSolverCore.Grid;

namespace SudokuSolverCore
{
    public class Solver(Grid grid)
    {
        private Cell[,] Cells => grid.Cells;
        private Cell GetCell(int line, int column)
        {
            return Cells[line, column];
        }

        public void Solve()
        {
            bool valueModified;
            do
            {
                PropagateValues();
                valueModified = SetUniqueValues();

                var currentState = grid.Print();
                var spaces = currentState.Count(c => c == ' ');
                var potentialValues = grid.PrintPotentialValues();
                
            } while (valueModified);
        }

        private bool SetUniqueValues()
        {
            var valueModified = false;
            
            ForEachCell((line, column) =>
            {
                SelectUniqueValueForCell(ref valueModified, line, column);
            });

            return valueModified;
        }

        private void SelectUniqueValueForCell(ref bool valueModified, int line, int column)
        {
            var currentCell = GetCell(line, column);
            
            var valueFixed = currentCell.Value != Undefined;
            if (valueFixed) 
                return;

            int count = 0;
            foreach (bool bit in currentCell.PotentialValues)
            {
                if (bit)
                    count++;
            }

            if (count != 1)
                return;
            
            for (var i = 1; i <= GridSize; i++)
            {
                if (currentCell.PotentialValues[i - 1])
                {
                    currentCell.Value=i;
                    valueModified = true;
                    PropagateUsedValuesForOneCell(line, column);
                    return;
                }
            }            
        }

        private void PropagateValues()
        {
            ForEachCell(PropagateUsedValuesForOneCell);
        }

        private void PropagateUsedValuesForOneCell(int line, int column)
        {
            var currentCell = GetCell(line, column);

            var valueIsUndefined = currentCell.Value == Undefined;
            if (valueIsUndefined) 
                return;
            
            foreach (var c in AllColumns)
            {
                if (c!=column)
                    GetCell(line, c).PotentialValues[currentCell.Value-1] = false;
            }
            
            foreach (var l in AllLines)
            {
                if (l!=line)
                    GetCell(l, column).PotentialValues[currentCell.Value-1] = false;
            }
            
            int regionLine = line / RegionSize;
            int regionColumn = column / RegionSize;
            
            var lineOffset = regionLine * RegionSize;
            var columnOffset = regionColumn * RegionSize;

            var indices = Enumerable.Range(0, RegionSize).ToList();
            foreach (var l in indices)
            {
                foreach (var c in indices)
                {
                    if (l!=line||c!=column)
                        GetCell(lineOffset + l, columnOffset + c).PotentialValues[currentCell.Value-1] = false;
                }
            }
        }
    }
}
