using static SudokuSolverCore.Grid;

namespace SudokuSolverCore
{
    public class Solver(Grid grid)
    {
        private Cell[,] Cells => grid.Cells;

        public void Solve()
        {
            var valueModified = true;
            const bool debug = false;
            
            do
            {
                PropagateValues();
                valueModified = FindUniqueValues();

                if (!valueModified)
                    valueModified = FindHiddenUniqueValues();
                
                if (!valueModified)
                    valueModified = FindDoublePairs();
                
                if (debug)
                    GenerateDebugOutput();
            } while (valueModified);
            
            if (!ValidityChecker.Check(Cells))
                GenerateDebugOutput();
        }

        private void GenerateDebugOutput()
        {
            var currentState = grid.Print();
            var spaces = currentState.Count(c => c == ' ');
            var potentialValues = grid.PrintPotentialValues();
        }

        private bool FindUniqueValues()
        {
            var uniqueValues = new UniqueValues(Cells);
            return uniqueValues.SetUniqueValues();
        }

        private bool FindHiddenUniqueValues()
        {
            var uniqueValues = new UniqueValues(Cells);
            return uniqueValues.SetHiddenUniqueValues();
        }

        
        private bool FindDoublePairs()
        {
            var doublePairs = new DoublePairs(Cells);
            return doublePairs.Handle();
        }
        
        private void PropagateValues()
        {
            ForEachCell(PropagateUsedValuesForOneCell);
        }

        private void PropagateUsedValuesForOneCell(int row, int column)
        {
            if (Cells[row, column].Value == Undefined) 
                return;

            ForEachCellInRowExcept(column, c => {
                Cells[row, c].PotentialValues[Cells[row, column].Value-1] = false;
            });

            ForEachCellInColumnExcept(row, r => {
                Cells[r, column].PotentialValues[Cells[row, column].Value-1] = false;
            });

            ForEachCellInRegionExcept(row, column, tuple => {
                Cells[tuple.Item1, tuple.Item2].PotentialValues[Cells[row, column].Value-1] = false;    
            });
        }
    }
}
