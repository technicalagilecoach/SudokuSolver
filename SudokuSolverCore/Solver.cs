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

        private void PropagateUsedValuesForOneCell(Position position)
        {
            if (Cells[position.Row, position.Column].Value == Undefined) 
                return;

            ForEachCellInRowExcept(position.Column, c => {
                PossibleValues[position.Row, c][Cells[position.Row, position.Column].Value-1] = false;
            });

            ForEachCellInColumnExcept(position.Row, r => {
                PossibleValues[r, position.Column][Cells[position.Row, position.Column].Value-1] = false;
            });

            ForEachCellInRegionExcept(position.Row, position.Column, tuple => {
                PossibleValues[tuple.Item1, tuple.Item2][Cells[position.Row, position.Column].Value-1] = false;
            });
        }
    }
}
