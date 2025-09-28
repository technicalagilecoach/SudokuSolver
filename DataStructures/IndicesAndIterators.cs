namespace SudokuSolver;

public static class IndicesAndIterators
{
    private static readonly Dictionary<int,List<Position>> IndicesForRow = new();
    private static readonly Dictionary<int,List<Position>> IndicesForColumn = new();
    private static readonly Dictionary<int,List<Position>> IndicesForBox = new();
    
    private static readonly List<Position> IndicesForAllCells = [];
    
    private static readonly List<List<(Position, Position)>> IndicesForDistinctPairsInRows = [];
    private static readonly List<List<(Position, Position)>> IndicesForDistinctPairsInColumns = [];
    private static readonly List<List<(Position, Position)>> IndicesForDistinctPairsInBoxes = [];
    
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
        var row = box / 3 * 3;
        var column = box % 3 * 3;
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
        switch (areaType)
        {
            case 0:
                return GetIndicesForDistinctPairsInRows(index);
            case 1:
                return GetIndicesForDistinctPairsInColumns(index);
            case 2:
                return GetIndicesForDistinctPairsInBoxes(index);
        }

        return [];
    }

    public static List<(Position, Position)> GetIndicesForDistinctPairsInRows(int index)
    {
        if (IndicesForDistinctPairsInRows.Count!=0)
            return IndicesForDistinctPairsInRows[index];
 
        for (var row = 0; row < Puzzle.GridSize; row++)
        {
            var allCellsOfInterest = GetIndicesForRow(row);
            IndicesForDistinctPairsInRows.Add(GetAllDistinctPairs(allCellsOfInterest));
        }
        
        return IndicesForDistinctPairsInRows[index];
    }
    
    public static List<(Position, Position)> GetIndicesForDistinctPairsInColumns(int index)
    {
        if (IndicesForDistinctPairsInColumns.Count!=0)
            return IndicesForDistinctPairsInColumns[index];
        
        for (var column = 0; column < Puzzle.GridSize; column++)
        {
            var allCellsOfInterest = GetIndicesForColumn(column);
            IndicesForDistinctPairsInColumns.Add(GetAllDistinctPairs(allCellsOfInterest));
        }
        
        return IndicesForDistinctPairsInColumns[index];
    }
    
    public static List<(Position, Position)> GetIndicesForDistinctPairsInBoxes(int index)
    {
        if (IndicesForDistinctPairsInBoxes.Count!=0)
            return IndicesForDistinctPairsInBoxes[index];
        
        for (var box = 0; box < Puzzle.GridSize; box++)
        {
            var allCellsOfInterest = GetIndicesForBox(box);
            IndicesForDistinctPairsInBoxes.Add(GetAllDistinctPairs(allCellsOfInterest));
        }
        
        return IndicesForDistinctPairsInBoxes[index];
    }

    private static List<(Position, Position)> GetAllDistinctPairs(List<Position> allCellsOfInterest)
    {
        var allDistinctPairs = new List<(Position, Position)>();

        for (var i = 0; i < allCellsOfInterest.Count - 1; i++)
        {
            for (var j = i; j < allCellsOfInterest.Count; j++)
            {
                if (i != j)
                    allDistinctPairs.Add((allCellsOfInterest[i], allCellsOfInterest[j]));
            }
        }

        return allDistinctPairs;
    }

    public static List<List<T>> Combinations<T>(List<T> list, int n)
    {
        if (n == 0)
            return [[]];
        if (list.Count == 0)
            return [];

        var head = list[0];
        var tail = list.Count>1?list.GetRange(1,list.Count - 1):[];

        var result = new List<List<T>>();
        
        //head is part of the combination
        var res1 = Combinations(tail, n-1);
        foreach (var item in res1)
        {
            item.Insert(0, head);
            result.Add(item);
        }

        //head is not part of the combination
        var res2 = Combinations(tail, n);
        foreach (var item in res2)
        {
            result.Add(item);
        }
        
        return result;
    }
}
