using System.Collections;
using System.IO.Enumeration;
using static SudokuSolverCore.Grid;

namespace SudokuSolverCore
{
    public class Solver(Grid grid)
    {
        private Cell[,] Cells => grid.Cells;

        public void Solve()
        {
            var valueModified = true;
            
            do
            {
                PropagateValues();
                valueModified = SetUniqueValues();
                
                if (!valueModified)
                    valueModified = HandleConnectedTwins();
                
                //for debugging
                var currentState = grid.Print();
                var spaces = currentState.Count(c => c == ' ');
                var potentialValues = grid.PrintPotentialValues();
                
            } while (valueModified);
        }

        private bool HandleConnectedTwins()
        {
            var valueModified = false;
            
            var undefinedCells = new bool[GridSize, GridSize];
            ForEachCell((row, column) =>
            {
                if (Cells[row, column].Value == Undefined)
                    undefinedCells[row, column] = true;
                else
                    undefinedCells[row, column] = false;
            });
            
            var potentialTwins = new bool[GridSize, GridSize];
            ForEachCell((row, column) =>
            {
                if (!undefinedCells[row, column]) 
                    return;
                
                if (Cells[row, column].CountPotentialValues() == 2)
                    potentialTwins[row, column] = true;
                else
                    potentialTwins[row, column] = false;
            });

            //twins in a row
            foreach (var row in AllRows)
            {
                for (var c1 = 0; c1<GridSize-1; c1++)
                {
                    for (var c2 = c1+1; c2<GridSize; c2++)
                    {
                        if (potentialTwins[row, c1] && potentialTwins[row, c2])
                        {
                            var cell1 = Cells[row, c1];
                            var cell2 = Cells[row, c2];

                            if (cell1.IsEqualTo(cell2))
                            {
                                //elminate potential values from other cells
                                for (var c3 = 0; c3 < GridSize; c3++)
                                {
                                    if (undefinedCells[row, c3] && c3 != c1 && c3 != c2)
                                    {
                                        var cell3 = Cells[row, c3];

                                        for (var i = 0; i < HighestNumber; i++)
                                        {
                                            if (cell1.PotentialValues[i])
                                                cell3.PotentialValues[i] = false;
                                        }
                                    }
                                }

                                //exit the loop
                                valueModified = true;
                                break;
                                
                                //if there are more twins in one row they will be found in later repetitions
                            }
                        }
                    }
                }
            }
            
            //twins in a column
            foreach (var column in AllColumns)
            {
                for (var r1 = 0; r1<GridSize-1; r1++)
                {
                    for (var r2 = r1+1; r2<GridSize; r2++)
                    {
                        if (potentialTwins[r1,column] && potentialTwins[r2,column])
                        {
                            var cell1 = Cells[r1, column];
                            var cell2 = Cells[r2, column];

                            if (cell1.IsEqualTo(cell2))
                            {
                                //elminate potential values from other cells
                                for (var r3 = 0; r3 < GridSize; r3++)
                                {
                                    if (undefinedCells[r3,column] && r3 != r1 && r3 != r2)
                                    {
                                        var cell3 = Cells[r3, column];

                                        for (var i = 0; i < HighestNumber; i++)
                                        {
                                            if (cell1.PotentialValues[i])
                                                cell3.PotentialValues[i] = false;
                                        }
                                    }
                                }

                                //exit the loop
                                valueModified = true;
                                break;
                                
                                //if there are more twins in one row they will be found in later repetitions
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
            
            ForEachCell((row, column) =>
            {
                SelectUniqueValueForCell(ref valueModified, row, column);
            });

            return valueModified;
        }

        private void SelectUniqueValueForCell(ref bool valueModified, int row, int column)
        {
            var currentCell = Cells[row, column];
            
            var valueFixed = currentCell.Value != Undefined;
            if (valueFixed) 
                return;

            if (currentCell.CountPotentialValues() != 1)
                return;
            
            for (var i = 0; i < HighestNumber; i++)
            {
                if (!currentCell.PotentialValues[i]) continue;
                currentCell.Value=i+1;
                valueModified = true;
                PropagateUsedValuesForOneCell(row, column);
                return;
            }            
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
