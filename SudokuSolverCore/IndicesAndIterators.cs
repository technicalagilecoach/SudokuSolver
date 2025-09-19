namespace SudokuSolverCore;

public static class IndicesAndIterators
{
    private static readonly Dictionary<int,List<Position>> IndicesForRow = new();
    private static readonly Dictionary<int,List<Position>> IndicesForColumn = new();
    private static readonly Dictionary<int,List<Position>> IndicesForBox = new();
    
    private static readonly List<Position> IndicesForAllCells = [];
    
    private static readonly Dictionary<int, List<List<(Position, Position)>>> IndicesForDistinctPairs = new();
    
    public static void ForEachCell(Action<Position> action)
    {
        foreach (var pos in GetIndicesForAllCells())
        {
            action(pos);
        }
    }
    
    public static void ForEachCellInArea(List<Position> positions, Action<Position> action)
    {
        foreach (var position in positions)
        {
            action(position);
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
        if (IndicesForAllCells.Count!=0)
                return IndicesForAllCells;

        foreach (var row in Puzzle.AllDigits)
        {
            foreach (var column in Puzzle.AllDigits)
            {
                IndicesForAllCells.Add(new Position(row, column));
            }
        }
        
        return IndicesForAllCells;
    }

    public static List<Position> GetIndicesForRow(int row)
    {
        if (IndicesForRow.ContainsKey(row))
        {
            return IndicesForRow[row];
        }
        
        var rowIndices = new List<Position>();
        foreach (var c in Puzzle.AllDigits)
        {
            rowIndices.Add(new Position(row, c));
        }

        IndicesForRow.Add(row, rowIndices);
        return rowIndices;
    }
    
    public static List<Position> GetIndicesForColumn(int column)
    {
        if (IndicesForColumn.ContainsKey(column))
        {
            return IndicesForColumn[column];
        }
        
        var columnIndices = new List<Position>();
        foreach (var r in Puzzle.AllDigits)
        {
            columnIndices.Add(new Position(r, column));
        }

        IndicesForColumn.Add(column, columnIndices);
        return columnIndices;
    }
    
    public static List<Position> GetIndicesForBox(int index)
    {
        if (IndicesForBox.ContainsKey(index))
        {
            return IndicesForBox[index];
        }

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
        
        IndicesForBox.Add(index, box);
        return box;
    }
    
    public static List<(Position, Position)> GetIndicesForDistinctPairs(int areaType, int index)
    {
        // if (IndicesForDistinctPairs.Count != 0)
        // {
        //     return IndicesForDistinctPairs[areaType][index];
        // }
        
        var allCellsOfInterest = new List<Position>();

        switch (areaType)
        {
            case 0:
                allCellsOfInterest = GetIndicesForRow(index); break;
            case 1:
                allCellsOfInterest = GetIndicesForColumn(index); break;
            case 2:
                allCellsOfInterest = GetIndicesForBox(index); break;
        }

        for (var area = 0; area < 3; area++)
        {
            IndicesForDistinctPairs[area] = new List<List<(Position, Position)>>();
            
            for (var idx = 0; idx < Puzzle.GridSize; idx++)
            {
                IndicesForDistinctPairs[area].Add(new List<(Position, Position)>());

                for (var i = 0; i < allCellsOfInterest.Count-1; i++)
                {
                    for (var j = i; j < allCellsOfInterest.Count; j++)
                    {
                        if (i!=j)
                            IndicesForDistinctPairs[area][idx].Add((allCellsOfInterest[i], allCellsOfInterest[j]));
                    }
                }
            }
        }
        
        return IndicesForDistinctPairs[areaType][index];
    }
}
