namespace SudokuSolver;

public class GuessAndTry(Puzzle puzzle) : Strategy(puzzle)
{
    private readonly Puzzle _puzzle = puzzle;

    public bool Handle()
    {
        var cellWithTwoCandidates = FindCellWithTwoCandidates(out var firstCandidate, out var secondCandidate);
        if (cellWithTwoCandidates is null)
            return false;
        
        Puzzle puzzleCopy = Puzzle.Copy(_puzzle);
        
        puzzleCopy.SetCellValue(cellWithTwoCandidates, firstCandidate);
        Solver solver = new Solver(puzzleCopy);
        bool isConsistent = solver.Solve();
        
        if (isConsistent)
            _puzzle.SetCellValue(cellWithTwoCandidates, firstCandidate);
        else
            _puzzle.SetCellValue(cellWithTwoCandidates, secondCandidate);
        
        return true;
    }

    private Position? FindCellWithTwoCandidates(out int firstCandidate, out int secondCandidate)
    {
        Position? cellWithTwoCandidates = null;
        firstCandidate = 0;
        secondCandidate = 0;
        
        foreach (var row in Puzzle.AllRows)
        {
            foreach (var column in Puzzle.AllColumns)
            {
                Position position = new Position(row, column);
                var candidates = _puzzle.GetCandidates(position);

                List<int> numbers = [];
                for (int index = 0; index < candidates.Count; index++)
                {
                    if (candidates[index])
                    {
                        numbers.Add(index+1);
                    }
                }

                if (numbers.Count == 2)
                {
                    cellWithTwoCandidates = position;
                    firstCandidate = numbers[0];
                    secondCandidate = numbers[1];
                    return cellWithTwoCandidates;
                }
            }
        }

        return null;
    }
}
