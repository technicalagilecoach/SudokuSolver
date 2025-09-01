namespace SudokuSolverCore
{
    public class Grid
    {
        public const int GridSize = 9;
        public const int HighestNumber = GridSize;
        public const int RegionSize = 3;
        public const int Undefined = 0;

        internal static readonly IEnumerable<int> AllRows = Enumerable.Range(0, GridSize);
        internal static readonly IEnumerable<int> AllColumns = Enumerable.Range(0, GridSize);
        //internal static readonly IEnumerable<(int row, int column)> AllRegions = [(1, 1), (1, 4), (1, 7), (4, 1), (4, 4), (4, 7), (7, 1), (7, 4), (7, 7)];
        internal static readonly IEnumerable<int> AllRegions = Enumerable.Range(0, GridSize);

        internal Cell[,] Cells { get; } = new Cell[GridSize, GridSize];

        public void Init(string puzzle)
        {
            var rows = puzzle.Split('\n');
            
            ForEachCell((row, column) =>
            {
                Cells[row, column] = (rows[row][column]).ToString();
            });
        }

        public string Print()
        {
            if (Cells == null)
                throw new NullReferenceException();

            var buffer = new StringWriter();
            
            foreach (var row in AllRows) {
                foreach (var column in AllColumns) {
                    buffer.Write((string)Cells[row, column]);
                } 

                buffer.WriteLine();
            }  
            
            return buffer.ToString();
        }
        
        public string PrintPotentialValues()
        {
            if (Cells == null)
                throw new NullReferenceException();

            var buffer = new StringWriter();

            ForEachCell((row, column) =>
            {
                var cell = Cells[row, column];
                if (cell.Value == Undefined)
                {
                    var pValues = cell.PotentialValues;

                    var values = "";
                    for (var index = 0; index < pValues.Count; index++)
                    {
                        var kv = pValues[index];
                        if (kv)
                            values += " " + (index + 1);
                    }

                    buffer.Write(row + " " + column + ":" + values + "\n");
                }
            });
            
            return buffer.ToString();
        }
        
        public static void ForEachCell(Action<int, int> action)
        {
            foreach (var row in AllRows)
            {
                foreach (var column in AllColumns)
                {
                    action(row, column);
                }
            }
        }
        
        public static void ForEachCellInColumnExcept(int row, Action<int> action)
        {
            foreach (var r in AllRows)
            {
                if (r!=row)
                    action(r);
            }
        }
        
        public static void ForEachCellInRowExcept(int column, Action<int> action)
        {
            foreach (var c in AllColumns)
            {
                if (c != column)
                    action(c);
            }
        }

        public static void ForEachCellInRegionExcept(int row, int column, Action<(int, int)> action)
        {
            foreach (var p in GetIndicesForRegion(GetRegionIndex(row,column)))
            {
                if (p.row != row || p.column != column)
                    action(p);
            }
        }

        public static int GetRegionIndex(int row, int column)
        {
            var regionLine = row / RegionSize;
            var regionColumn = column / RegionSize;
            
            return regionLine * RegionSize + regionColumn + 1;
        }

        public static (int row,int column) GetRegionCoordinates(int region)
        {
            var row = ((region-1)/3)*3+1;
            var column = ((region-1)%3)*3+1;
            return (row, column);
        }
        
        public static List<(int row, int column)> GetIndicesForRegion(int index)
        {
            var coordinates = GetRegionCoordinates(index);
            
            var region = new List<(int, int)>();
            
            var regionLine = coordinates.row / RegionSize;
            var regionColumn = coordinates.column / RegionSize;
            
            var lineOffset = regionLine * RegionSize;
            var columnOffset = regionColumn * RegionSize;

            var indices = Enumerable.Range(0, RegionSize).ToList();
            foreach (var l in indices)
            {
                foreach (var c in indices)
                {
                    region.Add((lineOffset + l, columnOffset + c));
                }
            }

            return region;
        }

        public static List<(int row, int column)> GetIndicesForColumn(int column)
        {
            var columnIndices = new List<(int row, int column)>();
            for (var r = 0; r < GridSize; r++)
            {
                columnIndices.Add((r, column));
            }

            return columnIndices;
        }

        public static List<(int row, int column)> GetIndicesForRow(int row)
        {
            var rowIndices = new List<(int row, int column)>();
            for (var c = 0; c < GridSize; c++)
            {
                rowIndices.Add((row, c));
            }

            return rowIndices;
        }
    }
}
