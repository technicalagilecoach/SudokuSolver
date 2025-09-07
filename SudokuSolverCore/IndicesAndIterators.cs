namespace SudokuSolverCore;

public class IndicesAndIterators
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
        foreach (var r in Puzzle.AllDigits)
        {
            if (r!=row)
                action(r);
        }
    }

    public static void ForEachCellInRowExcept(int column, Action<int> action)
    {
        foreach (var c in Puzzle.AllDigits)
        {
            if (c != column)
                action(c);
        }
    }

    public static void ForEachCellInRegionExcept(Position position, Action<Position> action)
    {
        foreach (var p in GetIndicesForRegion(GetRegionIndex(position)))
        {
            if (p.Row != position.Row || p.Column != position.Column)
                action(p);
        }
    }

    public static int GetRegionIndex(Position position)
    {
        var regionLine = position.Row / Puzzle.RegionSize;
        var regionColumn = position.Column / Puzzle.RegionSize;
            
        return regionLine * Puzzle.RegionSize + regionColumn + 1;
    }

    public static Position GetRegionCoordinates(int region)
    {
        var row = ((region-1)/3)*3+1;
        var column = ((region-1)%3)*3+1;
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
    
    public static List<Position> GetIndicesForRegion(int index)
    {
        var coordinates = IndicesAndIterators.GetRegionCoordinates(index);
            
        var region = new List<Position>();
            
        var regionLine = coordinates.Row / Puzzle.RegionSize;
        var regionColumn = coordinates.Column / Puzzle.RegionSize;
            
        var lineOffset = regionLine * Puzzle.RegionSize;
        var columnOffset = regionColumn * Puzzle.RegionSize;

        var indices = Enumerable.Range(0, Puzzle.RegionSize).ToList();
        foreach (var l in indices)
        {
            foreach (var c in indices)
            {
                region.Add(new Position(lineOffset + l, columnOffset + c));
            }
        }

        return region;
    }
}