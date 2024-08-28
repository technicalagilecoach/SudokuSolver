using System;
using System.Collections.Generic;
using System.Data.Common;
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

        internal static IEnumerable<int> allLines = Enumerable.Range(0, GRID_SIZE);
        internal static IEnumerable<int> allColumns = Enumerable.Range(0, GRID_SIZE);

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
            foreach (var line in allLines)
            {
                foreach (var column in allColumns)
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

        static void ForEachCell(Action<int, int> action)
        {
            foreach (var line in allLines)
            {
                foreach (var column in allColumns)
                {
                    action(line, column);
                }
            }
        }

        private bool UpdateValues()
        {
            bool valueModified = false;

            ForEachCell((line, column) =>
            {
                UpdateOneCell(ref valueModified, line, column);
            });

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

        private void AddValuesFromLine(HashSet<int> collectedValues, int line)
        {
            foreach (var column in allColumns)
            {
                CollectValues(collectedValues, line, column);
            }
        }

        private void AddValuesFromColumn(HashSet<int> collectedValues, int column)
        {
            foreach (var line in allLines)
            {
                CollectValues(collectedValues, line, column);
            }
        }

        private void AddValuesFromRegion(HashSet<int> collectedValues, int regionLine, int regionColumn)
        {
            int lineOffset = regionLine * REGION_SIZE;
            int columnOffset = regionColumn * REGION_SIZE;

            var indices = Enumerable.Range(0, REGION_SIZE);
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
            ForEachCell((line, column) =>
            {
                IterateOneCell(line, column);
            });
        }

        private void IterateOneCell(int line, int column)
        {
            Cell currentCell = Grid[line, column];

            bool valueNotFixed = currentCell.Value == UNDEFINED;
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
