using System.Collections;
using static SudokuSolverCore.IndicesAndIterators;
using static SudokuSolverCore.Printers;
using static SudokuSolverCore.Puzzle;
using static SudokuSolverCore.ValidityChecker;

namespace SudokuSolverCore
{
    public class Solver(Puzzle puzzle)
    {
        private int[,] Cells => puzzle.GetCells();
        private BitArray[,] PossibleValues => puzzle.PossibleValues; 

        public void Solve()
        {
            bool valueModified;
            
            do
            {
                PropagateValues();
                valueModified = FindUniqueValues();

                if (!valueModified)
                    valueModified = FindHiddenUniqueValues();
                
                if (!valueModified)
                    valueModified = FindDoublePairs();
            } while (valueModified);

            if (!Check(Cells))
            {
                GenerateDebugOutput();
            }

            GenerateDebugOutput();
        }

        private void GenerateDebugOutput()
        {
            var currentState = Print(puzzle);
            Console.WriteLine(currentState);
            
            var spaces = CountUndefinedCells(puzzle.GetCells());
            Console.WriteLine(spaces);
            
            var potentialValues = PrintPotentialValues(puzzle);
            Console.WriteLine(potentialValues);
        }

        private bool FindUniqueValues()
        {
            var uniqueValues = new UniqueValues(puzzle);
            return uniqueValues.SetUniqueValues();
        }

        private bool FindHiddenUniqueValues()
        {
            var uniqueValues = new UniqueValues(puzzle);
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
            if (Cells[position.Row, position.Column] == Undefined) 
                return;

            ForEachCellInRowExcept(position.Column, c => {
                PossibleValues[position.Row, c][Cells[position.Row, position.Column]-1] = false;
            });

            ForEachCellInColumnExcept(position.Row, r => {
                PossibleValues[r, position.Column][Cells[position.Row, position.Column]-1] = false;
            });

            ForEachCellInRegionExcept(position, tuple => {
                PossibleValues[tuple.Row, tuple.Column][Cells[position.Row, position.Column]-1] = false;
            });
        }
    }
}
