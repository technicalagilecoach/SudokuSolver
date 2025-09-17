using System.Collections;
using static SudokuSolverCore.IndicesAndIterators;
using static SudokuSolverCore.Printers;
using static SudokuSolverCore.ValidityChecker;

namespace SudokuSolverCore;

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

            Execute(PruneCandidates);
            Execute(NakedSingles);
            
            //Execute(HiddenSingles);
            //Execute(NakedPairs);
            //Execute(HiddenPairs);
            Execute(PointingPairs);
        } while (_valueModified);

        var isCorrect = Check(Cells);
        
        if (PerformChecks&&!isCorrect)
            PrintDebugOutput(puzzle);
        
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
            var puzzleBefore = PrintCells(Cells);
            
            _valueModified = fun();

            var gotWorse = true;// (!before) && IsInconsistent();
            if (PerformChecks && gotWorse)
            {
                var puzzleAfter = PrintCells(Cells);
                var diff = Puzzle.Difference(puzzleBefore, puzzleAfter);

                //PrintDebugOutput(puzzle);
            }
        }
    }

    public bool PruneCandidates()
    {
        return new PruneCandidates(puzzle).Handle();
    }

    public bool NakedSingles()
    {
        return new NakedSingles(puzzle).Handle();
    }

    public bool HiddenSingles()
    {
        return new HiddenSingles(puzzle).Handle();
    }
        
    public bool NakedPairs()
    {
        return new NakedPairs(puzzle).Handle();
    }
     
    public bool HiddenPairs()
    {
        return new HiddenPairs(puzzle).Handle();
    }

    public bool PointingPairs()
    {
        return new PointingPairs(puzzle).Handle();
    }
}