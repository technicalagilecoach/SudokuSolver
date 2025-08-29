namespace SudokuSolverCore
{
    public class Grid
    {
        public const int GridSize = 9;
        public const int RegionSize = 3;
        public const int Undefined = -1;

        internal static readonly IEnumerable<int> AllLines = Enumerable.Range(0, GridSize);
        internal static readonly IEnumerable<int> AllColumns = Enumerable.Range(0, GridSize);

        internal Cell[,] Cells { get; } = new Cell[GridSize, GridSize];

        public void Init(string puzzle)
        {
            var rows = puzzle.Split('\n');

            for (var i = 0; i < GridSize; i++)
            {
                for (var j = 0; j< GridSize; j++)
                {
                    Cells[i, j] = (rows[i][j]).ToString();
                }
            }
        }

        public string Print()
        {
            if (Cells == null)
                throw new NullReferenceException();

            var buffer = new StringWriter();
            
            for (var i = 0; i < Cells.GetLength(0); i++) 
            { 
                for (var j = 0; j < Cells.GetLength(1); j++) 
                { 
                    buffer.Write((string)Cells[i, j]);
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
            
            for (var i = 0; i < Cells.GetLength(0); i++) 
            { 
                for (var j = 0; j < Cells.GetLength(1); j++) 
                { 
                    var cell = Cells[i, j];
                    if (cell.Value == Undefined)
                    {
                        var pValues = cell.PotentialValues;

                        string values = "";
                        foreach (var kv in pValues)
                        {
                            if (kv.Value)
                                values += " "+kv.Key;
                        }

                        buffer.Write(i+" "+j+":"+values+"\n");    
                    }
                } 
            }  
            
            return buffer.ToString();
        }
        
        public static void ForEachCell(Action<int, int> action)
        {
            foreach (var line in AllLines)
            {
                foreach (var column in AllColumns)
                {
                    action(line, column);
                }
            }
        }
    }
}
