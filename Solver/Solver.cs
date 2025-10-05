using System.Collections;
using static SudokuSolver.ValidityChecker;

namespace SudokuSolver;

internal class SolverException(string s) : Exception(s);

public class Solver(Puzzle puzzle)
{
    //private int[,] Cells => puzzle.Cells;
   // private BitArray[,] Candidates => puzzle.Candidates;
    
    private bool _puzzleModified = false;

    private bool PerformChecks = true;
    
    public Dictionary<string,int> StrategyStats { get; } = new();
    public List<string> StrategyProtocol { get; } = new();

    public string LastConsistentState { get; private set; } = "";
    
    public bool Solve()
    {
        LastConsistentState = puzzle.PrintCells();

        try
        {
            PruneCandidates(); //call it once after Init
            
            do
            {
                _puzzleModified = false;

                Execute(NakedSingles); //fixes values
                Execute(HiddenSingles); //fixes values

                Execute(NakedPairs); //removes candidates
                Execute(HiddenPairs); //removes candidates

                Execute(NakedTriplets); //removes candidates
                Execute(HiddenTriplets); //removes candidates

                Execute(NakedQuadruplets); //removes candidates
                //Execute(HiddenQuadruplets); //removes candidates

                Execute(PointingTuples); //removes candidates
                Execute(BoxLineReduction); //removes candidates
                
                Execute(XWing); //removes candidates
            } while (_puzzleModified);
        }
        catch (SolverException e)
        {
            // ignored
        }

        return Check(puzzle);
    }

    private void Execute(Func<bool> fun)
    {
        if (_puzzleModified) 
            return;
        
        var before = true;
        var beforeCandidates = true;
        if (PerformChecks)
        {
            before = AreFixedValuesConsistent(puzzle);
            beforeCandidates = AreCandidatesConsistent(puzzle);
            var puzzleBefore = puzzle.PrintCells();
            if (!before)
                throw new SolverException(puzzleBefore);
            LastConsistentState = puzzleBefore;
        }

        var strategy = fun.Method.Name;
        StrategyStats.TryAdd(strategy, 0);
            
        _puzzleModified = fun();
        if (strategy is "NakedSingles" or "HiddenSingles")
            PruneCandidates();

        if (_puzzleModified)
        {
            StrategyStats[strategy]++;
            StrategyProtocol.Add(strategy);
        }

        if (PerformChecks)
        {
            var gotWorse = before && !AreFixedValuesConsistent(puzzle);
            var candidatesGotWorse= beforeCandidates && !AreCandidatesConsistent(puzzle);
            if (gotWorse||candidatesGotWorse)
            {
                var puzzleAfter = puzzle.PrintCells();
                throw new SolverException(puzzleAfter);
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
    
    private bool XWing()
    {
        return new XWing(puzzle).Handle();
    }
}