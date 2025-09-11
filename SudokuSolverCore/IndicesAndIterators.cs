namespace SudokuSolverCore;

public static class IndicesAndIterators
{
    public static void ForEachCell(Action<Position> action)
    {
        foreach (var pos in GetIndicesForAllCells())
        {
            action(pos);
        }
    }

    public static void ForEachCellInColumnExcept(int row, Action<int> action)
    {
        foreach (var r in Puzzle.AllRows)
        {
            if (r!=row)
                action(r);
        }
    }

    public static void ForEachCellInRowExcept(int column, Action<int> action)
    {
        foreach (var c in Puzzle.AllColumns)
        {
            if (c != column)
                action(c);
        }
    }
    
    public static void ForEachCellInBoxExcept(Position position, Action<Position> action)
    {
        foreach (var p in GetIndicesForBox(GetBoxIndex(position)))
        {
            if (p.Row != position.Row || p.Column != position.Column)
                action(p);
        }
    }

    public static int GetBoxIndex(Position position)
    {
        var boxLine = position.Row / Puzzle.BoxSize;
        var boxColumn = position.Column / Puzzle.BoxSize;
            
        return boxLine * Puzzle.BoxSize + boxColumn;
    }

    public static Position GetBoxCoordinates(int box)
    {
        var row = box / 3 * 3;//+1;
        var column = box % 3 * 3;//+1;
        return new Position(row, column);
    }

    public static List<Position> GetIndicesForAllCells()
    {
        var indices = new List<Position>();
        foreach (var row in Puzzle.AllDigits)
        {
            foreach (var column in Puzzle.AllDigits)
            {
                indices.Add(new Position(row, column));
            }
        }
        return indices;
    }

    public static List<Position> GetIndicesForRow(int row)
    {
        var rowIndices = new List<Position>();
        foreach (var c in Puzzle.AllDigits)
        {
            rowIndices.Add(new Position(row, c));
        }

        return rowIndices;
    }
    
    public static List<Position> GetIndicesForColumn(int column)
    {
        var columnIndices = new List<Position>();
        foreach (var r in Puzzle.AllDigits)
        {
            columnIndices.Add(new Position(r, column));
        }

        return columnIndices;
    }
    
    public static List<Position> GetIndicesForBox(int index)
    {
        var coordinates = GetBoxCoordinates(index);
            
        var box = new List<Position>();
            
        var boxLine = coordinates.Row / Puzzle.BoxSize;
        var boxColumn = coordinates.Column / Puzzle.BoxSize;
            
        var lineOffset = boxLine * Puzzle.BoxSize;
        var columnOffset = boxColumn * Puzzle.BoxSize;

        var indices = Enumerable.Range(0, Puzzle.BoxSize).ToList();
        foreach (var l in indices)
        {
            foreach (var c in indices)
            {
                box.Add(new Position(lineOffset + l, columnOffset + c));
            }
        }

        return box;
    }

    public static List<(Position, Position)>  GetIndicesForDistinctPairs(List<Position> indices)
    {
        var pairs = new List<(Position, Position)>();

        for (var i = 0; i < indices.Count-1; i++)
        {
            for (var j = i; j < indices.Count; j++)
            {
                if (i!=j)
                    pairs.Add((indices[i], indices[j]));
            }
        }

        return pairs;
    }
}