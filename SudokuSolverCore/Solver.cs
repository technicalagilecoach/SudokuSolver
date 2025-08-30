using System.Collections;
using System.IO.Enumeration;
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
                
                if (!valueModified)
                {
                    valueModified = HandleConnectedTwins();
                }
                
                var currentState = grid.Print();
                var spaces = currentState.Count(c => c == ' ');
                var potentialValues = grid.PrintPotentialValues();
                
            } while (valueModified);
        }

        private bool HandleConnectedTwins()
        {
            bool valueModified = false;
            
            var undefinedCells = new bool[GridSize, GridSize];
            foreach (var l in AllLines)
            {
                foreach (var c in AllColumns)
                {
                    if (Cells[l,c].Value == Undefined)
                        undefinedCells[l, c] = true;
                    else 
                        undefinedCells[l, c] = false;
                }
            }
            
            var potentialTwins = new bool[GridSize, GridSize];
            foreach (var l in AllLines)
            {
                foreach (var c in AllColumns)
                {
                    if (undefinedCells[l, c])
                    {
                        if (CountPotentialValues(Cells[l, c]) == 2)
                            potentialTwins[l, c] = true;
                        else
                            potentialTwins[l, c] = false;
                    }
                }
            }

            //twins in a line
            foreach (var l in AllLines)
            {
                for (var c1 = 0; c1<GridSize-1; c1++)
                {
                    for (var c2 = c1+1; c2<GridSize; c2++)
                    {
                        if (potentialTwins[l, c1] && potentialTwins[l, c2])
                        {
                            Cell cell1 = GetCell(l, c1);
                            Cell cell2 = GetCell(l, c2);

                            bool equals = true;
                            for (int i = 0; i < GridSize; i++)
                            {
                                if (cell1.PotentialValues[i] != cell2.PotentialValues[i])
                                {
                                    equals = false;
                                    break;
                                }
                            }

                            if (equals)
                            {
                                for (var c3 = 0; c3 < GridSize; c3++)
                                {
                                    if (undefinedCells[l, c3] && c3 != c1 && c3 != c2)
                                    {
                                        Cell cell3 = GetCell(l, c3);

                                        for (var i = 0; i < GridSize; i++)
                                        {
                                            if (cell1.PotentialValues[i])
                                                cell3.PotentialValues[i] = false;
                                        }
                                    }
                                }

                                valueModified = true;
                                break;
                                //elminate potential values from other cells
                                //exit the loop
                                //if there are more twins in one line they will be found in later repetitions
                            }
                        }
                    }
                }
            }
            
            //twins in a column
            foreach (var c in AllColumns)
            {
                for (var l1 = 0; l1<GridSize-1; l1++)
                {
                    for (var l2 = l1+1; l2<GridSize; l2++)
                    {
                        if (potentialTwins[l1,c] && potentialTwins[l2,c])
                        {
                            Cell cell1 = GetCell( l1,c);
                            Cell cell2 = GetCell( l2,c);

                            bool equals = true;
                            for (int i = 0; i < GridSize; i++)
                            {
                                if (cell1.PotentialValues[i] != cell2.PotentialValues[i])
                                {
                                    equals = false;
                                    break;
                                }
                            }

                            if (equals)
                            {
                                for (var l3 = 0; l3 < GridSize; l3++)
                                {
                                    if (undefinedCells[l3,c] && l3 != l1 && l3 != l2)
                                    {
                                        Cell cell3 = GetCell( l3,c);

                                        for (var i = 0; i < GridSize; i++)
                                        {
                                            if (cell1.PotentialValues[i])
                                                cell3.PotentialValues[i] = false;
                                        }
                                    }
                                }

                                valueModified = true;
                                break;
                                //elminate potential values from other cells
                                //exit the loop
                                //if there are more twins in one line they will be found in later repetitions
                            }
                        }
                    }
                }
            }

            
            //twins in a region

            return valueModified;
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

            if (CountPotentialValues(currentCell) != 1)
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

        private static int CountPotentialValues(Cell currentCell)
        {
            int count = 0;
            
            foreach (bool bit in currentCell.PotentialValues)
            {
                if (bit)
                    count++;
            }

            return count;
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
