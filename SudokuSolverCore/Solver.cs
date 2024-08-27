using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static SudokuSolverCore.SudokuPuzzle;

namespace SudokuSolverCore
{
    public class Solver(SudokuPuzzle puzzle)
    {
        private readonly SudokuPuzzle puzzle = puzzle;

        internal Cell[,] Puzzle => puzzle.Puzzle;

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
            for (int line = 0; line < PUZZLE_SIZE; line++)
            {
                for (int column = 0; column < PUZZLE_SIZE; column++)
                {
                    Cell currentCell = Puzzle[line, column];
                    currentCell.InitializePotentialValues();
                }
            }
        }

        private bool UpdateValues()
        {
            bool valueModified = false;
            for (int line = 0; line < PUZZLE_SIZE; line++)
            {
                for (int column = 0; column < PUZZLE_SIZE; column++)
                {
                    Cell currentCell = Puzzle[line, column];
                    if (currentCell.Value == -1)
                    {
                        int numberOfPotentialValues = 0;
                        for (int digit = 1; digit <= PUZZLE_SIZE; digit++)
                        {
                            if (currentCell.PotentialValues[digit] == true)
                                numberOfPotentialValues++;
                        }
                        if (numberOfPotentialValues == 1)
                        {
                            for (int digit = 1; digit <= PUZZLE_SIZE; digit++)
                            {
                                if (currentCell.PotentialValues[digit] == true)
                                {
                                    valueModified = true;
                                    currentCell.Value = digit;
                                }
                            }
                        }
                    }
                }
            }

            return valueModified;
        }

        private void OneIteration()
        {
            for (int line = 0; line < PUZZLE_SIZE; line++)
            {
                for (int column = 0; column < PUZZLE_SIZE; column++)
                {
                    Cell currentCell = Puzzle[line, column];

                    bool valueNotFixed = currentCell.Value == -1;
                    if (valueNotFixed)
                    {
                        var existingValues = new HashSet<int>();
                        AddValuesFromLine(existingValues, line);
                        AddValuesFromColumn(existingValues, column);
                        AddValuesFromSquare(existingValues, line / SQUARE_SIZE, column / SQUARE_SIZE);

                        foreach (var value in existingValues)
                        {
                            currentCell.PotentialValues[value] = false;
                        }
                    }
                }
            }

            void AddValuesFromLine(HashSet<int> collectedValues, int line)
            {
                for (int column = 0; column < PUZZLE_SIZE; column++)
                {
                    int value = Puzzle[line, column].Value;
                    if (value != UNDEFINED)
                    {
                        collectedValues.Add(value);
                    }
                }
            }

            void AddValuesFromColumn(HashSet<int> collectedValues, int column)
            {
                for (int line = 0; line < PUZZLE_SIZE; line++)
                {
                    int value = Puzzle[line, column].Value;
                    if (value != UNDEFINED)
                    {
                        collectedValues.Add(value);
                    }
                }
            }

            void AddValuesFromSquare(HashSet<int> collectedValues, int squareLine, int squareColumn)
            {
                int rowOffset = squareLine * SQUARE_SIZE;
                int columnOffset = squareColumn * SQUARE_SIZE;

                for (int line = 0; line < SQUARE_SIZE; line++)
                {
                    for (int column = 0; column < SQUARE_SIZE; column++)
                    {
                        int value = Puzzle[rowOffset + line, columnOffset + column].Value;
                        if (value != UNDEFINED)
                        {
                            collectedValues.Add(value);
                        }
                    }
                }
            }
        }
    }
}
