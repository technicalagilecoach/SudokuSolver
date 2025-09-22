using static SudokuSolver.IndicesAndIterators;
using static SudokuSolver.Puzzle;

namespace SudokuSolver;

public class BoxLineReduction(Puzzle puzzle) : Strategy(puzzle)
{
    public bool Handle()
    {
        var removedCandidates = 0;
        
        //box after box:
        foreach (var box in AllBoxes)
        {
            var boxIndices = GetIndicesForBox(box);
            var (rowsInBox, columnsInBox) = GetRowsAndColumnsOfBox(boxIndices);

            //for each row (intersecting the box)
            foreach (var row in rowsInBox)
            {
                //find numbers which can only be inside the box (in this row)
                var possibleDigits = new SortedSet<int>(AllDigits);
                foreach (var column in AllColumns)
                {
                    Position cell = new Position(row, column);
                    if (!columnsInBox.Contains(column))
                    {
                        if (IsUndefined(cell))
                        {
                            var candidates = GetCandidates(cell);
                            for (var index = 0; index < candidates.Count; index++)
                            {
                                if (candidates[index])
                                    possibleDigits.Remove(index);
                            }
                        }
                        else
                        {
                            possibleDigits.Remove(GetValue(cell));
                        } 
                    }
                    else
                    {
                        //also remove fixed values from within the box
                        if (!IsUndefined(cell))
                            possibleDigits.Remove(GetValue(cell));
                    }
                }

                //remove these numbers as candidates in the rest of the box (on the other rows)
                foreach (var digit in possibleDigits)
                {
                    foreach (var row2 in rowsInBox)
                    {
                        if (row2 != row)
                        {
                            foreach (var column in columnsInBox)
                            {
                                Position cell = new Position(row2, column);
                                if (IsUndefined(cell))
                                {
                                   var candidates = GetCandidates(cell);
                                   if (candidates[digit])
                                   {
                                       candidates[digit] = false;
                                       removedCandidates++;
                                   }
                                }
                            }
                        }
                    }
                }

            }

            //for each column (intersecting the box)
            foreach (var column in columnsInBox)
            {
                //find numbers which can only be inside the box (in this column)
                var possibleDigits = new SortedSet<int>(AllDigits);
                foreach (var row in AllRows)
                {
                    Position cell = new Position(row, column);
                    if (!rowsInBox.Contains(row))
                    {
                        if (IsUndefined(cell))
                        {
                            var candidates = GetCandidates(cell);
                            for (var index = 0; index < candidates.Count; index++)
                            {
                                if (candidates[index])
                                    possibleDigits.Remove(index);
                            }
                        }
                        else
                        {
                            possibleDigits.Remove(GetValue(cell));
                        }
                    }
                    else
                    {
                        if (!IsUndefined(cell))
                            possibleDigits.Remove(GetValue(cell));
                    }
                }

                //remove these numbers as candidates in the rest of the box (on the other columns)
                foreach (var digit in possibleDigits)
                {
                    foreach (var column2 in columnsInBox)
                    {
                        if (column2 != column)
                        {
                            foreach (var row in rowsInBox)
                            {
                                Position cell = new Position(row, column2);
                                if (IsUndefined(cell))
                                {
                                    var candidates = GetCandidates(cell);
                                    if (candidates[digit])
                                    {
                                        candidates[digit] = false;
                                        removedCandidates++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        return removedCandidates>0;
    }

    private static (SortedSet<int>,SortedSet<int>) GetRowsAndColumnsOfBox(List<Position> boxIndices)
    {
        var rows = new SortedSet<int>();
        var columns = new SortedSet<int>();
    
        foreach (var box in boxIndices)
        {
            rows.Add(box.Row);
            columns.Add(box.Column);
        }

        return (rows,columns);
    }

    private int GetValue(Position position)
    {
        return Cells[position.Row, position.Column]-1;
    }

}