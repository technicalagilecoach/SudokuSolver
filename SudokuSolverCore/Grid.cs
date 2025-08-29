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
            for (var i = 0; i < puzzle.Length; i++)
            {
                Cells[i / GridSize, i % GridSize] = puzzle[i].ToString();
            }
        }

        public string Print()
        {
            if (Cells == null)
                throw new NullReferenceException();

            var result = "";
            for (var i = 0; i < Cells.Length; i++)
            {
                result += Cells[i / GridSize, i % GridSize];
            }
            return result;
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
