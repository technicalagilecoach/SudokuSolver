using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static SudokuSolverCore.Grid;

namespace SudokuSolverCore
{
    public class Solver(Grid grid)
    {
        private readonly Grid grid = grid;

        internal Cell[,] Grid => grid.grid;

        public void Solve()
        {
            bool valueModified;
            do
            {
                ResetPotentialValues();
                OneIteration();
                valueModified = UpdateValues();

            } while (valueModified);
        }

        private void ResetPotentialValues()
        {
            var indices = Enumerable.Range(0,GRID_SIZE);           
            foreach (var line in indices)
            {
                foreach (var column in indices)
                {
                    ResetOneCell(line, column);
                }
            }
        }

        private void ResetOneCell(int line, int column)
        {
            Cell currentCell = Grid[line, column];
            currentCell.InitializePotentialValues();
        }

        private bool UpdateValues()
        {
            bool valueModified = false;
            var indices = Enumerable.Range(0, GRID_SIZE);
            foreach (var line in indices)
            {
                foreach (var column in indices)
                {
                    UpdateOneCell(ref valueModified, line, column);
                }
            }

            return valueModified;
        }

        private void UpdateOneCell(ref bool valueModified, int line, int column)
        {          
            Cell currentCell = Grid[line, column];         
            if (currentCell.Value == UNDEFINED)
            {
                var possibleValues = currentCell.PotentialValues.Where(x => x.Value == true);
                if (possibleValues.Count() == 1)
                {
                    valueModified = true;
                    currentCell.Value = possibleValues.First().Key;
                }
            }      
        }

        private void OneIteration()
        {
            var indices = Enumerable.Range(0, GRID_SIZE);
            foreach (var line in indices)
            {
                foreach (var column in indices)
                {
                    IterateOneCell(line, column);
                }
            }

            void AddValuesFromLine(HashSet<int> collectedValues, int line)
            {
                var indices = Enumerable.Range(0, GRID_SIZE);
                foreach (var column in indices)
                {
                    CollectValues(collectedValues, line, column);
                }
            }

            void AddValuesFromColumn(HashSet<int> collectedValues, int column)
            {
                var indices = Enumerable.Range(0, GRID_SIZE);
                foreach (var line in indices)
                {
                    CollectValues(collectedValues, line, column);
                }
            }

            void AddValuesFromRegion(HashSet<int> collectedValues, int regionLine, int regionColumn)
            {
                int lineOffset = regionLine * REGION_SIZE;
                int columnOffset = regionColumn * REGION_SIZE;

                var indices = Enumerable.Range(0, REGION_SIZE);
                foreach (var line in indices) {
                    foreach (var column in indices) {
                        CollectValues(collectedValues, lineOffset + line, columnOffset + column);
                    }
                }
            }

            void IterateOneCell(int line, int column)
            {
                Cell currentCell = Grid[line, column];

                bool valueNotFixed = currentCell.Value == -1;
                if (valueNotFixed)
                {
                    var existingValues = new HashSet<int>();
                    AddValuesFromLine(existingValues, line);
                    AddValuesFromColumn(existingValues, column);
                    AddValuesFromRegion(existingValues, line / REGION_SIZE, column / REGION_SIZE);

                    foreach (var value in existingValues)
                    {
                        currentCell.PotentialValues[value] = false;
                    }
                }
            }
        }

        private void CollectValues(HashSet<int> collectedValues, int line, int column)
        {
            int value = Grid[line, column].Value;
            if (value != UNDEFINED)
            {
                collectedValues.Add(value);
            }
        }
    }
}
