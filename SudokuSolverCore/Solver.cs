using System.Collections;
using static SudokuSolverCore.Puzzle;

namespace SudokuSolverCore
{
    public class Solver(Puzzle puzzle)
    {
        private Cell[,] Cells => puzzle.Cells;
        private BitArray[,] PossibleValues => puzzle.PossibleValues; 

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
            var currentState = puzzle.Print();
            var spaces = currentState.Count(c => c == ' ');
            var potentialValues = puzzle.PrintPotentialValues();
        }

        private bool FindUniqueValues()
        {
            var uniqueValues = new UniqueValues(Cells, PossibleValues);
            return uniqueValues.SetUniqueValues();
        }

        private bool FindHiddenUniqueValues()
        {
            var uniqueValues = new UniqueValues(Cells, PossibleValues);
            return uniqueValues.SetHiddenUniqueValues();
        }

        
        private bool FindDoublePairs()
        {
            var doublePairs = new DoublePairs(Cells, PossibleValues);
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
                PossibleValues[row, c][Cells[row, column].Value-1] = false;
            });

            ForEachCellInColumnExcept(row, r => {
                PossibleValues[r, column][Cells[row, column].Value-1] = false;
            });

            ForEachCellInRegionExcept(row, column, tuple => {
                PossibleValues[tuple.Item1, tuple.Item2][Cells[row, column].Value-1] = false;
            });
        }
    }
}
