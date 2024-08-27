
using System.Data.Common;

namespace SudokuSolverCore
{
    public class SudokuPuzzle
    {
        public const int PUZZLE_SIZE = 9;
        public const int SQUARE_SIZE = 3;
        public const int UNDEFINED = -1;

        public SudokuPuzzle()
        {
            puzzle = new Cell[PUZZLE_SIZE, PUZZLE_SIZE];
        }

        readonly Cell[,] puzzle;

        internal Cell[,] Puzzle => puzzle;

        public void Init(string puzzleAsString)
        {
            for (int i = 0; i < puzzleAsString.Length; i++)
            {
                Puzzle[i / PUZZLE_SIZE, i % PUZZLE_SIZE] = puzzleAsString[i].ToString();
            }
        }

        public string Print()
        {
            if (Puzzle == null)
                throw new NullReferenceException();

            string result = "";
            for (int i = 0; i < Puzzle.Length; i++)
            {
                result += Puzzle[i / PUZZLE_SIZE, i % PUZZLE_SIZE];
            }
            return result;
        }
    }
}
