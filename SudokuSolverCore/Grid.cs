using System.Data.Common;

namespace SudokuSolverCore
{
    public class Grid
    {
        public const int GRID_SIZE = 9;
        public const int REGION_SIZE = 3;
        public const int UNDEFINED = -1;

        public Grid()
        {
            _grid = new Cell[GRID_SIZE, GRID_SIZE];
        }

        readonly Cell[,] _grid;

        internal static IEnumerable<int> allLines = Enumerable.Range(0, GRID_SIZE);
        internal static IEnumerable<int> allColumns = Enumerable.Range(0, GRID_SIZE);

        internal Cell[,] grid => _grid;

        public void Init(string puzzle)
        {
            for (int i = 0; i < puzzle.Length; i++)
            {
                grid[i / GRID_SIZE, i % GRID_SIZE] = puzzle[i].ToString();
            }
        }

        public string Print()
        {
            if (grid == null)
                throw new NullReferenceException();

            string result = "";
            for (int i = 0; i < grid.Length; i++)
            {
                result += grid[i / GRID_SIZE, i % GRID_SIZE];
            }
            return result;
        }
        public static void ForEachCell(Action<int, int> action)
        {
            foreach (var line in allLines)
            {
                foreach (var column in allColumns)
                {
                    action(line, column);
                }
            }
        }
    }
}
