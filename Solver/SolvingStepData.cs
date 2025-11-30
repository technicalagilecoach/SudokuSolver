using System;
using System.Collections.Generic;
using System.Linq;
using SudokuSolver;

namespace SudokuSolver;

public class SolvingStepData
{
    public int StepNumber { get; set; }
    public string StrategyName { get; set; } = "";
    public string PuzzleStateBefore { get; set; } = "";
    public string PuzzleStateAfter { get; set; } = "";
    public TimeSpan ExecutionTime { get; set; }
    public List<CellModification> ModifiedCells { get; set; } = new();
    public List<Position> AffectedCells { get; set; } = new();
    public List<CandidateElimination> CandidateEliminations { get; set; } = new();
    public StrategyPattern Pattern { get; set; } = new();
    
    public SolvingStepData(int stepNumber, string strategyName, string puzzleStateBefore)
    {
        StepNumber = stepNumber;
        StrategyName = strategyName;
        PuzzleStateBefore = puzzleStateBefore;
    }
}

public class CellModification
{
    public Position Position { get; set; } = new Position(1, 1);
    public int OldValue { get; set; }
    public int NewValue { get; set; }
    public List<int> RemovedCandidates { get; set; } = new();
    public ModificationType Type { get; set; }
    
    public bool IsValueChange => OldValue != NewValue;
    public bool IsCandidateRemoval => RemovedCandidates.Count > 0;
}

public class CandidateElimination
{
    public Position Position { get; set; } = new Position(1, 1);
    public List<int> EliminatedCandidates { get; set; } = new();
    public string Reason { get; set; } = "";
}

public class StrategyPattern
{
    public List<Position> PrimaryCells { get; set; } = new();
    public List<Position> SecondaryCells { get; set; } = new();
    public List<Position> AffectedRows { get; set; } = new();
    public List<Position> AffectedColumns { get; set; } = new();
    public List<Position> AffectedBoxes { get; set; } = new();
    public PatternType PatternType { get; set; }
}

public enum ModificationType
{
    ValuePlacement,
    CandidateElimination,
    CandidatePruning
}

public enum PatternType
{
    None,
    Single,
    Pair,
    Triplet,
    Quadruplet,
    XWing,
    Swordfish,
    PointingTuple,
    BoxLineReduction
}