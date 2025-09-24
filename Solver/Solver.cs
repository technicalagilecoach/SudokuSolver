using System.Collections;
using static SudokuSolver.ValidityChecker;

namespace SudokuSolver;

public class Solver(Puzzle puzzle)
{
    private int[,] Cells => puzzle.Cells;
    private BitArray[,] Candidates => puzzle.Candidates;
    
    private bool _valueModified = false;

    private const bool PerformChecks = true;

    public bool Solve()
    {
        do
        {
            _valueModified = false;

            Execute(PruneCandidates); //removes candidates
            
            Execute(NakedSingles); //fixes values
            Execute(HiddenSingles); //fixes values
            
            Execute(NakedPairs); //removes candidates
            Execute(HiddenPairs); //removes candidates?
            
            Execute(PointingPairs); //removes candidates
            Execute(BoxLineReduction); //removes candidates
        } while (_valueModified);

        var isCorrect = Check(Cells);

        if (PerformChecks && !isCorrect)
        {
            var isInconsistent = IsInconsistent();
            var state = puzzle.PrintCells();
        }

        return isCorrect;
    }

    private bool IsInconsistent()
    {
        return PerformChecks && !IsSolutionCorrect(puzzle.Cells);
    }

    public void Execute(Func<bool> fun)
    {
        if (!_valueModified)
        {
            var before = IsInconsistent();
            var puzzleBefore = puzzle.PrintCells();
            
            _valueModified = fun();

            var gotWorse = (!before) && IsInconsistent();
            if (PerformChecks && gotWorse)
            {
                var puzzleAfter = puzzle.PrintCells();
                var diff = Puzzle.Difference(puzzleBefore, puzzleAfter);
            }
        }
    }

    private bool PruneCandidates()
    {
        return new PruneCandidates(puzzle).Handle();
    }

    private bool NakedSingles()
    {
        return new NakedSingles(puzzle).Handle();
    }

    private bool HiddenSingles()
    {
        return new HiddenSingles(puzzle).Handle();
    }

    private bool NakedPairs()
    {
        return new NakedPairs(puzzle).Handle();
    }

    private bool HiddenPairs()
    {
        return new HiddenPairs(puzzle).Handle();
    }

    private bool PointingPairs()
    {
        return new PointingPairs(puzzle).Handle();
    }

    private bool BoxLineReduction()
    {
        return new BoxLineReduction(puzzle).Handle();
    }
}