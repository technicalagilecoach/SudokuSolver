using static SudokuSolverCore.Grid;

namespace SudokuSolverCore
{
    public class Solver(Grid grid)
    {
        private readonly Grid _grid = grid;

        private Cell[,] Cells => _grid.Cells;
        private Cell GetCell(int line, int column)
        {
            return Cells[line, column];
        }

        public void Solve()
        {
            bool valueModified;
            do
            {
                ResetPotentialValues();
                OneIteration();
                valueModified = UpdateValues();

                string currentState = _grid.Print();
                int spaces = currentState.Count(c => c == ' ');
                string potentialValues = _grid.PrintPotentialValues();
            } while (valueModified);
        }

        private void ResetPotentialValues()
        {
            ForEachCell((line, column) =>
            {
                GetCell(line, column).InitializePotentialValues();
            });
        }

        private bool UpdateValues()
        {
            var valueModified = false;
            
            ForEachCell((line, column) =>
            {
                UpdateOneCell(ref valueModified, line, column);
            });

            return valueModified;
        }

        private void UpdateOneCell(ref bool valueModified, int line, int column)
        {
            var currentCell = GetCell(line, column);
            var valueNotFixed = currentCell.Value == Undefined;

            if (!valueNotFixed) 
                return;
            
            var possibleValues = currentCell.PotentialValues.Where(x => x.Value == true).ToList();
            
            if (possibleValues.Count != 1) 
                return;
            
            valueModified = true;
            currentCell.Value = possibleValues.First().Key;
        }

        private void AddValuesFromLine(HashSet<int> collectedValues, int line)
        {
            foreach (var column in AllColumns)
            {
                CollectValues(collectedValues, line, column);
            }
        }

        private void AddValuesFromColumn(HashSet<int> collectedValues, int column)
        {
            foreach (var line in AllLines)
            {
                CollectValues(collectedValues, line, column);
            }
        }

        private void AddValuesFromRegion(HashSet<int> collectedValues, int regionLine, int regionColumn)
        {
            var lineOffset = regionLine * RegionSize;
            var columnOffset = regionColumn * RegionSize;

            var indices = Enumerable.Range(0, RegionSize).ToList();
            foreach (var line in indices)
            {
                foreach (var column in indices)
                {
                    CollectValues(collectedValues, lineOffset + line, columnOffset + column);
                }
            }
        }

        private void OneIteration()
        {
            ForEachCell(IterateOneCell);
        }

        private void IterateOneCell(int line, int column)
        {
            var currentCell = GetCell(line, column);

            var valueNotFixed = currentCell.Value == Undefined;
            if (!valueNotFixed) 
                return;
            
            var existingValues = new HashSet<int>();
            AddValuesFromLine(existingValues, line);
            AddValuesFromColumn(existingValues, column);
            AddValuesFromRegion(existingValues, line / RegionSize, column / RegionSize);

            foreach (var value in existingValues)
            {
                currentCell.PotentialValues[value] = false;
            }
        }

        private void CollectValues(HashSet<int> collectedValues, int line, int column)
        {
            var value = GetCell(line, column).Value;
            if (value != Undefined)
            {
                collectedValues.Add(value);
            }
        }
    }
}
