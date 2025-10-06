using static SudokuSolver.IndicesAndIterators;
using static SudokuSolver.Puzzle;

namespace SudokuSolver;

public class BoxLineReduction(Puzzle puzzle) : Strategy(puzzle)
{
    private int _removedCandidates;
    
    public bool Handle()
    {
        _removedCandidates = 0;
        
        foreach (var box in AllBoxes)
        {
            var allCellsInBox = GetIndicesForBox(box);
            var (rowsInBox, columnsInBox) = GetRowsAndColumnsOfBox(allCellsInBox);

            foreach (var row in rowsInBox)
            {
                var possibleNumbers = FindDigitsExclusiveInBox(columnsInBox, GetIndicesForRow(row),position => position.Column);
                
                var remainingCells = allCellsInBox.Where(position => position.Row != row);
                foreach (var cell in remainingCells)
                {
                    RemoveCandidates(cell, possibleNumbers.ToList(), ref _removedCandidates);
                }
            }

            foreach (var column in columnsInBox)
            {
                var possibleNumbers = FindDigitsExclusiveInBox(rowsInBox, GetIndicesForColumn(column), position => position.Row);
                
                var remainingCells = allCellsInBox.Where(position => position.Column != column);
                foreach (var cell in remainingCells)
                {
                    RemoveCandidates(cell, possibleNumbers.ToList(), ref _removedCandidates);
                }
            }
        }
        
        return _removedCandidates>0;
    }

    private SortedSet<int> FindDigitsExclusiveInBox(SortedSet<int> indicesInBox, List<Position> positions, Func<Position, int> projection)
    {
        var possibleNumbers = new SortedSet<int>(AllDigits);
        foreach (var cell in positions)
        {
            bool isOutsideOfBox = !indicesInBox.Contains(projection(cell));
            
            if (IsUndefined(cell))
            {
                if (isOutsideOfBox)
                {
                    var candidates = GetCandidates(cell);
                    foreach (var number in AllDigits) 
                    {
                        if (candidates[number-1])
                            possibleNumbers.Remove(number);
                    }
                }
            }
            else
            {
                possibleNumbers.Remove(GetValue(cell));
            }
        }
        return possibleNumbers;
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
}