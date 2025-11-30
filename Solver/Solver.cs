using System.Collections;
using System.Diagnostics;
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
    public List<SolvingStepData> SolvingSteps { get; } = new();

    public string LastConsistentState { get; private set; } = "";
    
    public Puzzle Puzzle => puzzle;
    private int _stepCounter = 0;
    
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
                Execute(HiddenQuadruplets); //removes candidates

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
        var puzzleBefore = "";
        
        if (PerformChecks)
        {
            before = AreFixedValuesConsistent(puzzle);
            beforeCandidates = AreCandidatesConsistent(puzzle);
            puzzleBefore = puzzle.PrintCells();
            if (!before)
                throw new SolverException(puzzleBefore);
            LastConsistentState = puzzleBefore;
        }

        var strategy = fun.Method.Name;
        StrategyStats.TryAdd(strategy, 0);
        
        var stopwatch = Stopwatch.StartNew();
        _puzzleModified = fun();
        stopwatch.Stop();
        
        if (strategy is "NakedSingles" or "HiddenSingles")
            PruneCandidates();

        if (_puzzleModified)
        {
            StrategyStats[strategy]++;
            StrategyProtocol.Add(strategy);
            
            // Capture solving step data
            var stepData = new SolvingStepData(++_stepCounter, strategy, puzzleBefore)
            {
                PuzzleStateAfter = puzzle.PrintCells(),
                ExecutionTime = stopwatch.Elapsed
            };
            
            // Track cell modifications (simplified - could be enhanced)
            TrackCellModifications(puzzleBefore, stepData);
            
            SolvingSteps.Add(stepData);
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
    
    private void TrackCellModifications(string beforeState, SolvingStepData stepData)
    {
        var afterState = puzzle.PrintCells();
        
        // Simple comparison - could be enhanced for better tracking
        if (beforeState != afterState)
        {
            // This is a simplified approach - a full implementation would
            // track individual cell changes and candidate removals
            stepData.ModifiedCells.Add(new CellModification
            {
                Position = new Position(1, 1), // Placeholder
                OldValue = 0,
                NewValue = 1
            });
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