using static SudokuSolverCore.Grid;

namespace SudokuSolverCore;

internal class DoublePairs(Cell[,] cells)
{
    public bool Handle()
    {
        var valueModified = false;
        
        var undefinedCells = new bool[GridSize, GridSize];
        ForEachCell((row, column) =>
        {
            if (cells[row, column].Value == Undefined)
                undefinedCells[row, column] = true;
            else
                undefinedCells[row, column] = false;
        });
        
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

        //twins in a row
        foreach (var row in AllRows)
        {
            for (var c1 = 0; c1<GridSize-1; c1++)
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
                                if (undefinedCells[row, c3] && c3 != c1 && c3 != c2)
                                {
                                    var cell3 = cells[row, c3];

                                    for (var i = 0; i < HighestNumber; i++)
                                    {
                                        if (cell1.PotentialValues[i] && cell3.PotentialValues[i])
                                        {
                                            cell3.PotentialValues[i] = false;
                                            actualChange = true;
                                        }
                                    }
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
                                if (undefinedCells[r3,column] && r3 != r1 && r3 != r2)
                                {
                                    var cell3 = cells[r3, column];

                                    for (var i = 0; i < HighestNumber; i++)
                                    {
                                        if (cell1.PotentialValues[i] && cell3.PotentialValues[i])
                                        {
                                            cell3.PotentialValues[i] = false;
                                            actualChange = true;
                                        }
                                    }
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
        var regions = new List<(int row, int column)> { (1,1),(1,4),(1,7),(4,1),(4,4),(4,7),(7,1),(7,4),(7,7)};

        foreach (var region in regions)
        {
            var indices = GetIndicesForRegion(region.row, region.column).ToArray();
            
            for (var c1 = 0; c1<GridSize-1; c1++)
            {
                for (var c2 = c1+1; c2<GridSize; c2++)
                {
                    if (potentialTwins[indices[c1].Item1,indices[c1].Item2] && potentialTwins[indices[c2].Item1,indices[c2].Item2])
                    {
                        var cell1 = cells[indices[c1].Item1, indices[c1].Item2];
                        var cell2 = cells[indices[c2].Item1, indices[c2].Item2];

                        if (cell1.IsEqualTo(cell2))
                        {
                            bool actualChange = false;
                            
                            //elminate potential values from other cells
                            for (var c3 = 0; c3 < GridSize; c3++)
                            {
                                if (undefinedCells[indices[c3].Item1, indices[c3].Item2] && c3 != c1 && c3 != c2)
                                {
                                    var cell3 = cells[indices[c3].Item1, indices[c3].Item2];

                                    for (var i = 0; i < HighestNumber; i++)
                                    {
                                        if (cell1.PotentialValues[i] && cell3.PotentialValues[i])
                                        {
                                            cell3.PotentialValues[i] = false;
                                            actualChange = true;
                                        }
                                    }
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
}
