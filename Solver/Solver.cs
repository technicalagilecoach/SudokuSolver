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
                
                Execute(GuessAndTry);
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
        if (strategy is "NakedSingles" or "HiddenSingles" or "GuessAndTry")
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
            
            // Track cell modifications with enhanced tracking
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
        var beforePuzzle = CreatePuzzleFromState(beforeState);
        var afterPuzzle = Puzzle.Copy(puzzle);
        
        // Track value changes
        IndicesAndIterators.ForEachCell(position =>
        {
            var beforeValue = beforePuzzle.GetCellValue(position);
            var afterValue = afterPuzzle.GetCellValue(position);
            
            if (beforeValue != afterValue)
            {
                var modification = new CellModification
                {
                    Position = position,
                    OldValue = beforeValue,
                    NewValue = afterValue,
                    Type = afterValue != 0 ? ModificationType.ValuePlacement : ModificationType.CandidateElimination
                };
                
                // Track candidate changes
                var beforeCandidates = GetCandidatesList(beforePuzzle.GetCandidates(position));
                var afterCandidates = GetCandidatesList(afterPuzzle.GetCandidates(position));
                var removedCandidates = beforeCandidates.Except(afterCandidates).ToList();
                
                modification.RemovedCandidates = removedCandidates;
                stepData.ModifiedCells.Add(modification);
                stepData.AffectedCells.Add(position);
            }
            else if (beforeValue == 0)
            {
                // Track candidate eliminations in empty cells
                var beforeCandidates = GetCandidatesList(beforePuzzle.GetCandidates(position));
                var afterCandidates = GetCandidatesList(afterPuzzle.GetCandidates(position));
                var removedCandidates = beforeCandidates.Except(afterCandidates).ToList();
                
                if (removedCandidates.Count > 0)
                {
                    var elimination = new CandidateElimination
                    {
                        Position = position,
                        EliminatedCandidates = removedCandidates,
                        Reason = stepData.StrategyName
                    };
                    
                    stepData.CandidateEliminations.Add(elimination);
                    stepData.AffectedCells.Add(position);
                    
                    // Also add to modified cells for consistency
                    var modification = new CellModification
                    {
                        Position = position,
                        OldValue = beforeValue,
                        NewValue = afterValue,
                        RemovedCandidates = removedCandidates,
                        Type = ModificationType.CandidateElimination
                    };
                    
                    stepData.ModifiedCells.Add(modification);
                }
            }
        });
        
        // Determine strategy pattern based on strategy name and affected cells
        stepData.Pattern = DetermineStrategyPattern(stepData.StrategyName, stepData.AffectedCells);
    }
    
    private Puzzle CreatePuzzleFromState(string state)
    {
        var tempPuzzle = new Puzzle();
        var cleanState = state.Replace("\n", "").Replace(" ", "0");
        tempPuzzle.Init(cleanState);
        return tempPuzzle;
    }
    
    private List<int> GetCandidatesList(BitArray candidates)
    {
        var list = new List<int>();
        for (int i = 0; i < candidates.Length; i++)
        {
            if (candidates[i])
                list.Add(i + 1);
        }
        return list;
    }
    
    private StrategyPattern DetermineStrategyPattern(string strategyName, List<Position> affectedCells)
    {
        var pattern = new StrategyPattern();
        
        pattern.PatternType = strategyName switch
        {
            "NakedSingles" or "HiddenSingles" => PatternType.Single,
            "NakedPairs" or "HiddenPairs" => PatternType.Pair,
            "NakedTriplets" or "HiddenTriplets" => PatternType.Triplet,
            "NakedQuadruplets" or "HiddenQuadruplets" => PatternType.Quadruplet,
            "XWing" => PatternType.XWing,
            "PointingTuples" => PatternType.PointingTuple,
            "BoxLineReduction" => PatternType.BoxLineReduction,
            _ => PatternType.None
        };
        
        // Categorize cells based on their relationships
        if (affectedCells.Count > 0)
        {
            // Group cells by rows, columns, and boxes
            var rows = affectedCells.GroupBy(c => c.Row).ToList();
            var columns = affectedCells.GroupBy(c => c.Column).ToList();
            var boxes = affectedCells.GroupBy(c => GetBoxNumber(c)).ToList();
            
            // Primary cells are those directly involved in the pattern
            pattern.PrimaryCells = affectedCells.Take(GetPrimaryCellCount(pattern.PatternType)).ToList();
            
            // Secondary cells are affected by the pattern
            pattern.SecondaryCells = affectedCells.Skip(pattern.PrimaryCells.Count).ToList();
            
            // Track affected rows, columns, and boxes
            pattern.AffectedRows = rows.Select(g => new Position(g.Key, 1)).ToList();
            pattern.AffectedColumns = columns.Select(g => new Position(1, g.Key)).ToList();
            pattern.AffectedBoxes = boxes.Select(g => new Position((g.Key - 1) / 3 * 3 + 1, (g.Key - 1) % 3 * 3 + 1)).ToList();
        }
        
        return pattern;
    }
    
    private int GetPrimaryCellCount(PatternType patternType)
    {
        return patternType switch
        {
            PatternType.Single => 1,
            PatternType.Pair => 2,
            PatternType.Triplet => 3,
            PatternType.Quadruplet => 4,
            PatternType.XWing => 4,
            _ => 0
        };
    }
    
    private int GetBoxNumber(Position position)
    {
        return ((position.Row - 1) / 3) * 3 + ((position.Column - 1) / 3) + 1;
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

    private bool GuessAndTry()
    {
        return new GuessAndTry(puzzle).Handle();
    }
}