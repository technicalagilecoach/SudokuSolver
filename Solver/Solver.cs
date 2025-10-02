using System.Collections;
using static SudokuSolver.ValidityChecker;

namespace SudokuSolver;

public class Solver(Puzzle puzzle)
{
    private int[,] Cells => puzzle.Cells;
    private BitArray[,] Candidates => puzzle.Candidates;
    
    private bool _puzzleModified = false;

    private const bool PerformChecks = true;
    
    public Dictionary<string,int> StrategyStats { get; } = new();
    public List<string> StrategyProtocol { get; } = new();

    public string LastConsistentState { get; private set; } = "";
    
    public bool Solve()
    {
        LastConsistentState = puzzle.PrintCells();

        try
        {
            do
            {
                _puzzleModified = false;

                Execute(PruneCandidates); //removes candidates

                Execute(NakedSingles); //fixes values
                Execute(HiddenSingles); //fixes values

                Execute(NakedPairs); //removes candidates
                Execute(HiddenPairs); //removes candidates

                Execute(NakedTriplets); //removes candidates
                Execute(HiddenTriplets); //removes candidates

                Execute(NakedQuadruplets); //removes candidates
                Execute(HiddenQuadruplets); //removes candidates

                Execute(PointingTuples); //removes candidates
                Execute(BoxLineReduction); //removes candidates
            } while (_puzzleModified);
        }
        catch (Exception e)
        {
            // ignored
        }

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
        if (!_puzzleModified)
        {
            var before = IsInconsistent();
            var puzzleBefore = puzzle.PrintCells();
            LastConsistentState = puzzleBefore;
            
            string strategy = fun.Method.Name;
            if (!StrategyStats.ContainsKey(strategy))
                StrategyStats[strategy] = 0;
            
            _puzzleModified = fun();

            if (_puzzleModified)
            {
                StrategyStats[strategy]++;
                StrategyProtocol.Add(strategy);
            }

            var candidatesOk = ValidityChecker.CheckCandidates(puzzle);
            
            var gotWorse = (!before) && IsInconsistent();
            if (PerformChecks && gotWorse)
            {
                var puzzleAfter = puzzle.PrintCells();
                var diff = Puzzle.Difference(puzzleBefore, puzzleAfter);
                throw new Exception();
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

    private bool NakedTriplets()
    {
        return new NakedTriplets(puzzle).Handle();
    }
    
    private bool HiddenTriplets()
    {
        return new HiddenTriplets(puzzle).Handle();
    }
   
    private bool NakedQuadruplets()
    {
        return new NakedQuadruplets(puzzle).Handle();
    }
    
    private bool HiddenQuadruplets()
    {
        return new HiddenQuadruplets(puzzle).Handle();
    }
    
    private bool PointingTuples()
    {
        return new PointingTuples(puzzle).Handle();
    }

    private bool BoxLineReduction()
    {
        return new BoxLineReduction(puzzle).Handle();
    }
}