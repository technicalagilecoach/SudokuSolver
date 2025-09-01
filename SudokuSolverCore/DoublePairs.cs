using System.Collections;
using static SudokuSolverCore.Grid;

namespace SudokuSolverCore;

internal class DoublePairs(Cell[,] cells)
{
    public bool Handle()
    {
        var valueModified = false;
        
        var undefinedCells = MarkUndefinedCells();
        var potentialTwins = MarkPotentialTwins(undefinedCells);

        //twins in a row
        foreach (var row in AllRows)
        {
            var AllCellsInRow = GetIndicesForCurrentRow(row);
            var pairs = GetIndicesForDistinctPairs(row);

            foreach (var pair in pairs)
            {
                if (potentialTwins[pair.Item1.row, pair.Item1.column] && potentialTwins[pair.Item2.row, pair.Item2.column])
                {
                    if (CellsAreEqual(pair))
                    {
                        bool actualChange = false;

                        //elminate potential values from other cells
                        foreach (var ele in AllCellsInRow)
                        {
                            if (undefinedCells[ele.row, ele.column] && ele.column != pair.Item1.column && ele.column != pair.Item2.column)
                            {
                                actualChange = EliminatePotentialValuesFromOtherCells(ele.row, ele.column, cells[pair.Item1.row, pair.Item1.column], actualChange);
                            }
                        }

                        //exit the loop
                        valueModified = actualChange;
                        break;

                        //if there are more twins in one row they will be found in later repetitions
                    }
                }
            }


            /*for (var c1 = 0; c1<GridSize-1; c1++)
            {
                for (var c2 = c1+1; c2<GridSize; c2++)
                {
                    if (potentialTwins[row, c1] && potentialTwins[row, c2])
                    {
                        var cell1 = cells[row, c1];
                        var cell2 = cells[row, c2];

                        if (cell1.IsEqualTo(cell2))
                        {
                            bool actualChange = false;

                            //elminate potential values from other cells
                            for (var c3 = 0; c3 < GridSize; c3++)
                            {
                                var row3 = row;
                                var column3 = c3;

                                if (undefinedCells[row3, column3] && column3 != c1 && column3 != c2)
                                {
                                    actualChange = EliminatePotentialValuesFromOtherCells(row3, column3, cell1, actualChange);
                                }
                            }

                            //exit the loop
                            valueModified = actualChange;
                            break;

                            //if there are more twins in one row they will be found in later repetitions
                        }
                    }
                }
            }*/
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
                        var cell1 = cells[r1, column];
                        var cell2 = cells[r2, column];

                        if (cell1.IsEqualTo(cell2))
                        {
                            bool actualChange = false;
                            
                            //elminate potential values from other cells
                            for (var r3 = 0; r3 < GridSize; r3++)
                            {
                                var row3 = r3;
                                var column3 = column;
                                
                                if (undefinedCells[row3,column3] && row3 != r1 && row3 != r2)
                                {
                                    actualChange = EliminatePotentialValuesFromOtherCells(row3, column3, cell1, actualChange);
                                }
                            }

                            //exit the loop
                            valueModified = actualChange;
                            break;
                            
                            //if there are more twins in one row they will be found in later repetitions
                        }
                    }
                }
            }
        }
        
        //twins in a region
        foreach (var region in AllRegions)
        {
            var indices = GetIndicesForRegion(region.row, region.column).ToArray();
            
            for (var c1 = 0; c1<GridSize-1; c1++)
            {
                for (var c2 = c1+1; c2<GridSize; c2++)
                {
                    var row1 = indices[c1].Item1;
                    var column1 = indices[c1].Item2;
                    var row2 = indices[c2].Item1;
                    var column2 = indices[c2].Item2;
                    
                    if (potentialTwins[row1,column1] && potentialTwins[row2,column2])
                    {
                        var cell1 = cells[row1, column1];
                        var cell2 = cells[row2, column2];

                        if (cell1.IsEqualTo(cell2))
                        {
                            bool actualChange = false;
                            
                            //elminate potential values from other cells
                            for (var c3 = 0; c3 < GridSize; c3++)
                            {
                                var row3 = indices[c3].Item1;
                                var column3 = indices[c3].Item2;
                                
                                if (undefinedCells[row3, column3] && c3 != c1 && c3 != c2)
                                {
                                    actualChange = EliminatePotentialValuesFromOtherCells(row3, column3, cell1, actualChange);
                                }
                            }

                            //exit the loop
                            valueModified = actualChange;
                            break;
                            
                            //if there are more twins in one row they will be found in later repetitions
                        }
                    }
                }
            }
        }
        
        return valueModified;
    }

    private bool CellsAreEqual(((int row, int column), (int row, int column)) pair)
    {
        var cell1 = cells[pair.Item1.row, pair.Item1.column];
        var cell2 = cells[pair.Item2.row, pair.Item2.column];
        var cellsAreEqual = cell1.IsEqualTo(cell2);
        return cellsAreEqual;
    }

    private static List<((int row, int column), (int row, int column))> GetIndicesForDistinctPairs(int row)
    {
        var pairs = new List<((int row, int column), (int row, int column))>();
        for (var c1 = 0; c1 < GridSize - 1; c1++)
        {
            for (var c2 = c1 + 1; c2 < GridSize; c2++)
            {
                pairs.Add(((row,c1), (row,c2)));
            }
        }

        return pairs;
    }

    private static List<(int row, int column)> GetIndicesForCurrentRow(int row)
    {
        var rowIndices = new List<(int row, int column)>();
        for (var c = 0; c < GridSize; c++)
        {
            rowIndices.Add((row, c));
        }

        return rowIndices;
    }

    private bool EliminatePotentialValuesFromOtherCells(int row3, int column3, Cell cell1, bool actualChange)
    {
        var cell3 = cells[row3, column3];

        for (var i = 0; i < HighestNumber; i++)
        {
            if (cell1.PotentialValues[i] && cell3.PotentialValues[i])
            {
                cell3.PotentialValues[i] = false;
                actualChange = true;
            }
        }

        return actualChange;
    }

    private bool[,] MarkPotentialTwins(bool[,] undefinedCells)
    {
        var potentialTwins = new bool[GridSize, GridSize];
        ForEachCell((row, column) =>
        {
            if (!undefinedCells[row, column]) 
                return;
            
            if (cells[row, column].CountPotentialValues() == 2)
                potentialTwins[row, column] = true;
            else
                potentialTwins[row, column] = false;
        });
        return potentialTwins;
    }

    private bool[,] MarkUndefinedCells()
    {
        var undefinedCells = new bool[GridSize, GridSize];
        
        ForEachCell((row, column) =>
        {
            if (cells[row, column].Value == Undefined)
                undefinedCells[row, column] = true;
            else
                undefinedCells[row, column] = false;
        });
        
        return undefinedCells;
    }
}
