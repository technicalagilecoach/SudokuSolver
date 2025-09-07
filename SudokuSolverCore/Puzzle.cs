using System.Collections;

namespace SudokuSolverCore
{
    public class Puzzle
    {
        public const int GridSize = 9;
        public const int HighestNumber = GridSize;
        private const int RegionSize = 3;
        public const int Undefined = 0;

        internal static readonly IEnumerable<int> AllDigits = Enumerable.Range(0, GridSize);

        internal Cell[,] Cells { get; } = new Cell[GridSize, GridSize];

        public BitArray[,] PossibleValues { get; } = new BitArray[GridSize, GridSize];
        
        public void Init(string puzzle)
        {
            var rows = puzzle.Split('\n');
            
            ForEachCell(position =>
            {
                Cells[position.Row, position.Column] = (rows[position.Row][position.Column]).ToString();
            });
            
            ForEachCell(position =>
            {
                PossibleValues[position.Row, position.Column] = InitializePossibleValues(position);
            });
        }

        private BitArray InitializePossibleValues(Position position)
        {
            var potentialValues = new BitArray(GridSize);
            
            if (Cells[position.Row, position.Column].Value == Undefined)
            {
                potentialValues.SetAll(true);
            }
            else
            {
                potentialValues.SetAll(false);
                potentialValues[Cells[position.Row, position.Column].Value-1] = true;
            }
            
            return potentialValues;
        }
        
        public static int CountPotentialValues(BitArray[,] possibleValues, Position position)
        {
            var count = 0;
            
            foreach (bool bit in possibleValues[position.Row, position.Column])
            {
                if (bit)
                    count++;
            }

            return count;
        }
        
        public string Print()
        {
            if (Cells == null)
                throw new NullReferenceException();

            var buffer = new StringWriter();
            
            foreach (var row in AllDigits) {
                foreach (var column in AllDigits) {
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

            ForEachCell(position =>
            {
                var cell = Cells[position.Row, position.Column];
                if (cell.Value == Undefined)
                {
                    var pValues = PossibleValues[position.Row,position.Column];

                    var values = "";
                    foreach (var index in AllDigits)
                    {
                        var kv = pValues[index];
                        if (kv)
                            values += " " + (index + 1);
                    }

                    buffer.Write(position.Row + " " + position.Column + ":" + values + "\n");
                }
            });
            
            return buffer.ToString();
        }
       
        public static void ForEachCell(Action<Position> action)
        {
            foreach (var row in AllDigits)
            {
                foreach (var column in AllDigits)
                {
                    action(new Position(row, column));
                }
            }
        }
        
        public static void ForEachCellInColumnExcept(int row, Action<int> action)
        {
            foreach (var r in AllDigits)
            {
                if (r!=row)
                    action(r);
            }
        }
        
        public static void ForEachCellInRowExcept(int column, Action<int> action)
        {
            foreach (var c in AllDigits)
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
            var regionLine = position.Row / RegionSize;
            var regionColumn = position.Column / RegionSize;
            
            return regionLine * RegionSize + regionColumn + 1;
        }

        public static Position GetRegionCoordinates(int region)
        {
            var row = ((region-1)/3)*3+1;
            var column = ((region-1)%3)*3+1;
            return new Position(row, column);
        }
        
        public static List<Position> GetIndicesForRegion(int index)
        {
            var coordinates = GetRegionCoordinates(index);
            
            var region = new List<Position>();
            
            var regionLine = coordinates.Row / RegionSize;
            var regionColumn = coordinates.Column / RegionSize;
            
            var lineOffset = regionLine * RegionSize;
            var columnOffset = regionColumn * RegionSize;

            var indices = Enumerable.Range(0, RegionSize).ToList();
            foreach (var l in indices)
            {
                foreach (var c in indices)
                {
                    region.Add(new Position(lineOffset + l, columnOffset + c));
                }
            }

            return region;
        }

        public static List<Position> GetIndicesForColumn(int column)
        {
            var columnIndices = new List<Position>();
            foreach (var r in AllDigits)
            {
                columnIndices.Add(new Position(r, column));
            }

            return columnIndices;
        }

        public static List<Position> GetIndicesForRow(int row)
        {
            var rowIndices = new List<Position>();
            foreach (var c in AllDigits)
            {
                rowIndices.Add(new Position(row, c));
            }

            return rowIndices;
        }
    }
}
