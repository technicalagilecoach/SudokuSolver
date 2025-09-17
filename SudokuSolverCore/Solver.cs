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
            
            Execute(HiddenSingles);
            Execute(NakedPairs);
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

    private static string Difference(string puzzle1, string puzzle2)
    {
        var difference = new char[puzzle1.Length];
                    
        for (var i = 0; i < puzzle1.Length; i++)
        {
            difference[i] = ' ';
            if (puzzle1[i] == '\n')
            {
                difference[i] = '\n';
            }
            if (puzzle1[i] != puzzle2[i])
                difference[i] = puzzle2[i];
        }

        return new string(difference);
    }
    
    public void Execute(Func<bool> fun)
    {
        if (!_valueModified)
        {
            var before = IsInconsistent();
            var puzzleBefore = PrintCells(Cells);
            
            _valueModified = fun();

            var gotWorse = (!before) && IsInconsistent();
            if (PerformChecks && gotWorse)
            {
                var puzzleAfter = PrintCells(Cells);
                var diff = Difference(puzzleBefore, puzzleAfter);

                //PrintDebugOutput(puzzle);
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
}