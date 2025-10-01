using static SudokuSolver.IndicesAndIterators;
using static SudokuSolver.Puzzle;

namespace SudokuSolver;

public class BoxLineReduction(Puzzle puzzle) : Strategy(puzzle)
{
    private int _removedCandidates = 0;
    
    public bool Handle()
    {
        _removedCandidates = 0;
        
        foreach (var box in AllBoxes)
        {
            var allCellsInBox = GetIndicesForBox(box);
            var (rowsInBox, columnsInBox) = GetRowsAndColumnsOfBox(allCellsInBox);

            foreach (var row in rowsInBox)
            {
                var possibleDigits = FindDigitsExclusiveInBox(columnsInBox, GetIndicesForRow(row),position => position.Column);
                
                var remainingCells = allCellsInBox.Where(position => position.Row != row);
                foreach (var cell in remainingCells)
                {
                    RemoveCandidate(cell, possibleDigits);
                }
            }

            foreach (var column in columnsInBox)
            {
                var possibleDigits = FindDigitsExclusiveInBox(rowsInBox, GetIndicesForColumn(column), position => position.Row);
                
                var remainingCells = allCellsInBox.Where(position => position.Column != column);
                foreach (var cell in remainingCells)
                {
                    RemoveCandidate(cell, possibleDigits);
                }
            }
        }
        
        return _removedCandidates>0;
    }

    private SortedSet<int> FindDigitsExclusiveInBox(SortedSet<int> indicesInBox, List<Position> positions, Func<Position, int> projection)
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

    private void RemoveCandidate(Position cell, SortedSet<int> digitsToRemove)
    {
        if (IsUndefined(cell))
        {
            var candidates = GetCandidates(cell);
            foreach (var digit in digitsToRemove)
            {
                if (candidates[digit])
                {
                    candidates[digit] = false;
                    _removedCandidates++;
                }
            }
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