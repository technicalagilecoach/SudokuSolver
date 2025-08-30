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
    }
}
