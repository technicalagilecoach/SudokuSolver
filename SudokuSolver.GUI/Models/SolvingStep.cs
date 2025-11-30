using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using SudokuSolver;

namespace SudokuSolver.GUI.Models;

public partial class SolvingStep : ObservableObject
{
    [ObservableProperty]
    private bool _isSelected;
    
    public int StepNumber { get; set; }
    public string StrategyName { get; set; } = "";
    public string Description { get; set; } = "";
    public List<Position> AffectedCells { get; set; } = new();
    public List<CellChange> CellChanges { get; set; } = new();
    public TimeSpan ExecutionTime { get; set; }
    public string PuzzleStateBefore { get; set; } = "";
    public string PuzzleStateAfter { get; set; } = "";
    public StepType StepType { get; set; }
    
    public SolvingStep(int stepNumber, string strategyName, string description = "")
    {
        StepNumber = stepNumber;
        StrategyName = strategyName;
        Description = description;
        StepType = DetermineStepType(strategyName);
    }
    
    private StepType DetermineStepType(string strategyName)
    {
        return strategyName switch
        {
            "NakedSingles" or "HiddenSingles" => StepType.ValuePlacement,
            "NakedPairs" or "HiddenPairs" or "NakedTriplets" or "HiddenTriplets" or 
            "NakedQuadruplets" or "HiddenQuadruplets" => StepType.CandidateElimination,
            "PointingTuples" or "BoxLineReduction" or "XWing" => StepType.AdvancedTechnique,
            "PruneCandidates" => StepType.CandidatePruning,
            _ => StepType.Other
        };
    }
    
    public void AddCellChange(Position position, int oldValue, int newValue, List<int>? removedCandidates = null)
    {
        var change = new CellChange
        {
            Position = position,
            OldValue = oldValue,
            NewValue = newValue,
            RemovedCandidates = removedCandidates ?? new List<int>()
        };
        
        CellChanges.Add(change);
        
        if (!AffectedCells.Contains(position))
        {
            AffectedCells.Add(position);
        }
    }
    
    public string GetFormattedDescription()
    {
        if (!string.IsNullOrEmpty(Description))
            return Description;
            
        return StrategyName switch
        {
            "NakedSingles" => $"Placed {CellChanges.Count} value(s) using Naked Singles",
            "HiddenSingles" => $"Placed {CellChanges.Count} value(s) using Hidden Singles",
            "NakedPairs" => $"Eliminated candidates using Naked Pairs",
            "HiddenPairs" => $"Eliminated candidates using Hidden Pairs",
            "NakedTriplets" => $"Eliminated candidates using Naked Triplets",
            "HiddenTriplets" => $"Eliminated candidates using Hidden Triplets",
            "NakedQuadruplets" => $"Eliminated candidates using Naked Quadruplets",
            "HiddenQuadruplets" => $"Eliminated candidates using Hidden Quadruplets",
            "PointingTuples" => $"Eliminated candidates using Pointing Tuples",
            "BoxLineReduction" => $"Eliminated candidates using Box-Line Reduction",
            "XWing" => $"Eliminated candidates using X-Wing",
            "PruneCandidates" => "Pruned invalid candidates",
            _ => $"Applied {StrategyName} strategy"
        };
    }
}

public class CellChange
{
    public Position Position { get; set; } = new Position(1, 1);
    public int OldValue { get; set; }
    public int NewValue { get; set; }
    public List<int> RemovedCandidates { get; set; } = new();
    
    public bool IsValueChange => OldValue != NewValue;
    public bool IsCandidateRemoval => RemovedCandidates.Count > 0;
}

public enum StepType
{
    ValuePlacement,
    CandidateElimination,
    CandidatePruning,
    AdvancedTechnique,
    Other
}