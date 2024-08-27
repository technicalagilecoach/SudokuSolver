
using System.Data.Common;

namespace SudokuSolverCore
{
    public class SudokuPuzzle
    {
        public SudokuPuzzle() { 
            puzzle = new Tile[9,9]; 
        }

        Tile[,] puzzle;

        public void Init(string puzzleAsString)
        {
            for (int i = 0;i< puzzleAsString.Length;i++)
            {
                puzzle[i / 9, i % 9] = puzzleAsString[i].ToString();
            }
        }

        public string Print()
        {
            if (puzzle == null)
                throw new NullReferenceException();

            string result = "";
            for (int i = 0; i < puzzle.Length; i++)
            {
                result += puzzle[i / 9, i % 9];
            }
            return result;
        }

        public void Solve()
        {
            bool valueModified = false;
            do
            {
                ResetPotentialValues();
                OneIteration();
                valueModified = UpdateValues();

            } while (valueModified);
        }

        private void ResetPotentialValues()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Tile currentTile = puzzle[i, j];
                    currentTile.InitializePotentialValues();
                }
            }
        }

        private bool UpdateValues()
        {
            bool valueModified = false;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Tile currentTile = puzzle[i, j];
                    if (currentTile.GetValue() == -1)
                    {
                        int numberOfPotentialValues = 0;
                        for (int k = 1; k <= 9; k++)
                        {
                            if (currentTile.potentialValues[k] == true)
                                numberOfPotentialValues++;
                        }
                        if (numberOfPotentialValues == 1)
                        {
                            for (int k = 1; k <= 9; k++)
                            {
                                if (currentTile.potentialValues[k] == true)
                                {
                                    valueModified = true;
                                    currentTile.SetValue(k);
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
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Tile currentTile = puzzle[i, j];

                    bool valueNotFixed = currentTile.GetValue() == -1;
                    if (valueNotFixed)
                    {
                        bool useOldMethod = false;
                        if (useOldMethod)
                        {
                            CurrentLine(currentTile, i);
                            CurrentColumn(currentTile, j);
                            CurrentSquare(currentTile, i / 3, j / 3);
                        }
                        else
                        {
                            var valuesInLine = GetValuesFromLine(i);
                            var valuesInColumn = GetValuesFromColumn(j);
                            var valuesInSquare = GetValuesFromSquare(i / 3, j / 3);

                            var existingValues = new HashSet<int>();
                            existingValues.UnionWith(valuesInLine);
                            existingValues.UnionWith(valuesInColumn);
                            existingValues.UnionWith(valuesInSquare);

                            foreach (var value in existingValues) {
                                currentTile.potentialValues[value] = false;
                            }
                        }

                    }
                }
            }

            HashSet<int> GetValuesFromLine(int line) { 
                HashSet<int> values = new HashSet<int>();

                for (int i = 0; i < 9; i++)
                {
                    int w = puzzle[line, i].GetValue();
                    if (w != -1)
                    {
                        values.Add(w);
                    }
                }
                return values;
            }

            void CurrentLine(Tile currentTile, int line)
            {
                for (int i = 0; i < 9; i++)
                {
                    int w = puzzle[line, i].GetValue();
                    if (w != -1 && w != currentTile.GetValue())
                    {
                        currentTile.potentialValues[w] = false;
                    }
                }
            }

            HashSet<int> GetValuesFromColumn(int column)
            {
                HashSet<int> values = new HashSet<int>();

                for (int i = 0; i < 9; i++)
                {
                    int w = puzzle[i,column].GetValue();
                    if (w != -1)
                    {
                        values.Add(w);
                    }
                }
                return values;
            }


            void CurrentColumn(Tile currentTile, int column)
            {
                for (int i = 0; i < 9; i++)
                {
                    int w = puzzle[i, column].GetValue();
                    if (w != -1 && w != currentTile.GetValue())
                    {
                        currentTile.potentialValues[w] = false;
                    }
                }
            }

            HashSet<int> GetValuesFromSquare(int v1, int v2)
            {
                HashSet<int> values = new HashSet<int>();

                int rowOffset = v1 * 3;
                int columnOffset = v2 * 3;

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        int w = puzzle[rowOffset + i, columnOffset + j].GetValue();
                        if (w != -1)
                        {
                            values.Add(w);
                        }
                    }
                }

                return values;
            }


            void CurrentSquare(Tile currentTile, int v1, int v2)
            {
                int rowOffset = v1 * 3;
                int columnOffset = v2 * 3;

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        int w = puzzle[rowOffset + i, columnOffset + j].GetValue();
                        if (w != -1 && w != currentTile.GetValue())
                        {
                            currentTile.potentialValues[w] = false;
                        }
                    }
                }
            }
        }
    }
}
