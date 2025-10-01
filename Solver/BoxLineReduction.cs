using static SudokuSolver.IndicesAndIterators;
using static SudokuSolver.Puzzle;

namespace SudokuSolver;

public class BoxLineReduction(Puzzle puzzle) : Strategy(puzzle)
{
    int _removedCandidates = 0;
    
    public bool Handle()
    {
        _removedCandidates = 0;
        
        foreach (var box in AllBoxes)
        {
            var (rowsInBox, columnsInBox) = GetRowsAndColumnsOfBox(GetIndicesForBox(box));

            foreach (var row in rowsInBox)
            {
                var possibleDigits = FindExclusiveDigitsForBox(columnsInBox, GetIndicesForRow(row),position => position.Column);
                RemoveCandidatesInRestOfBoxRows(possibleDigits, rowsInBox, row, columnsInBox);
            }

            foreach (var column in columnsInBox)
            {
                var possibleDigits = FindExclusiveDigitsForBox(rowsInBox, GetIndicesForColumn(column), position => position.Row);
                RemoveCandidatesInRestOfBoxColumns(possibleDigits, columnsInBox, column, rowsInBox);
            }
        }
        
        return _removedCandidates>0;
    }

    private SortedSet<int> FindExclusiveDigitsForBox(SortedSet<int> indicesInBox, List<Position> positions, Func<Position, int> projection)
    {
        var possibleDigits = new SortedSet<int>(AllDigits);
        foreach (var cell in positions)
        {
            bool isOutsideOfBox = !indicesInBox.Contains(projection(cell));
            
            if (IsUndefined(cell))
            {
                if (isOutsideOfBox)
                {
                    var candidates = GetCandidates(cell);
                    for (var index = 0; index < candidates.Count; index++)
                    {
                        if (candidates[index])
                            possibleDigits.Remove(index);
                    }
                }
            }
            else
            {
                possibleDigits.Remove(GetValue(cell));
            }
        }
        return possibleDigits;
    }

    private void RemoveCandidatesInRestOfBoxColumns(SortedSet<int> possibleDigits, SortedSet<int> columnsInBox,
        int column,
        SortedSet<int> rowsInBox)
    {
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
                            RemoveCandidate(cell, digit);
                        }
                    }
                }
            }
        }
    }

    private void RemoveCandidatesInRestOfBoxRows(SortedSet<int> possibleDigits, SortedSet<int> rowsInBox, int row,
        SortedSet<int> columnsInBox)
    {
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
                            RemoveCandidate(cell, digit);
                        }
                    }
                }
            }
        }
    }

    private void RemoveCandidate(Position cell, int digit)
    {
        var candidates = GetCandidates(cell);
        if (candidates[digit])
        {
            candidates[digit] = false;
            _removedCandidates++;
        }
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