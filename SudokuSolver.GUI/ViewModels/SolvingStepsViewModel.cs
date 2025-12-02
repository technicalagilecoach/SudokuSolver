using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SudokuSolver;
using SudokuSolver.GUI.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SudokuSolver.GUI.ViewModels;

public partial class SolvingStepsViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<SolvingStep> _steps = new();
    
    [ObservableProperty]
    private SolvingStep? _selectedStep;
    
    [ObservableProperty]
    private int _currentStepIndex = -1;
    
    [ObservableProperty]
    private bool _canStepForward = false;
    
    [ObservableProperty]
    private bool _canStepBackward = false;
    
    [ObservableProperty]
    private bool _isPlaying = false;
    
    [ObservableProperty]
    private bool _hasSteps = false;
    
    private System.Timers.Timer? _playbackTimer;
    
    [ObservableProperty]
    private bool _enableAnimations = true;
    
    [ObservableProperty]
    private bool _showCandidates = true;
    
    partial void OnShowCandidatesChanged(bool value)
    {
        _sudokuGrid.SetShowCandidates(value);
    }
    
    [ObservableProperty]
    private double _animationDuration = 300; // milliseconds
    
    [ObservableProperty]
    private string _status = "No steps available";
    
    [ObservableProperty]
    private string _strategyExplanation = "Select a step to see strategy explanation";

    private SudokuGridViewModel _sudokuGrid;

    public SolvingStepsViewModel(SudokuGridViewModel sudokuGrid)
    {
        _sudokuGrid = sudokuGrid;
        InitializePlaybackTimer();
    }

    private void InitializePlaybackTimer()
    {
        _playbackTimer = new System.Timers.Timer
        {
            Interval = 1000 // 1 second between steps
        };
        _playbackTimer.Elapsed += OnPlaybackTimerTick;
    }

    

    [RelayCommand]
    private async Task StepForward()
    {
        if (CanStepForward && CurrentStepIndex < Steps.Count - 1)
        {
            CurrentStepIndex++;
            await ApplyStep(Steps[CurrentStepIndex]);
            UpdateNavigationState();
        }
    }

    [RelayCommand]
    private void StepBackward()
    {
        if (CanStepBackward && CurrentStepIndex > 0)
        {
            CurrentStepIndex--;
            // Reset to previous step state
            ResetToStep(Steps[CurrentStepIndex]);
            UpdateNavigationState();
        }
    }

    [RelayCommand]
    private void Pause()
    {
        if (!IsPlaying) return;
        
        IsPlaying = false;
        _playbackTimer?.Stop();
        Status = "Paused";
    }

    private async void OnPlaybackTimerTick(object? sender, System.Timers.ElapsedEventArgs e)
    {
        if (CurrentStepIndex < Steps.Count - 1 && IsPlaying)
        {
            CurrentStepIndex++;
            await ApplyStep(Steps[CurrentStepIndex]);
            UpdateNavigationState();
        }
        else
        {
            // Reached the end or was paused
            IsPlaying = false;
            _playbackTimer?.Stop();
            Status = CurrentStepIndex >= Steps.Count - 1 ? "Completed all steps" : "Playback stopped";
        }
    }

    [RelayCommand]
    private void Reset()
    {
        CurrentStepIndex = -1;
        HasSteps = false;
        _sudokuGrid.InitializeEmptyPuzzle();
        UpdateNavigationState();
        Status = "Reset to initial state";
    }

    [RelayCommand]
    private async Task SolveAsFarAsPossible()
    {
        if (IsPlaying) return;

        IsPlaying = true;
        Status = "Playing steps...";
        _playbackTimer!.Start();
    }

    [RelayCommand]
    private async Task GoToEnd()
    {
        // Apply all steps
        for (int i = 0; i < Steps.Count; i++)
        {
            await ApplyStep(Steps[i]);
        }
        CurrentStepIndex = Steps.Count - 1;
        UpdateNavigationState();
        Status = "Applied all steps";
    }

    public void LoadStepsFromSolver(Solver solver)
    {
        Steps.Clear();
        CurrentStepIndex = -1;

        for (int i = 0; i < solver.SolvingSteps.Count; i++)
        {
            var stepData = solver.SolvingSteps[i];
            var step = new SolvingStep(i + 1, stepData.StrategyName)
            {
                ExecutionTime = stepData.ExecutionTime,
                PuzzleStateBefore = stepData.PuzzleStateBefore,
                PuzzleStateAfter = stepData.PuzzleStateAfter
            };

            // Convert cell modifications to cell changes
            foreach (var modification in stepData.ModifiedCells)
            {
                step.AddCellChange(
                    modification.Position,
                    modification.OldValue,
                    modification.NewValue,
                    modification.RemovedCandidates
                );
            }
            
            // Add enhanced pattern information
            if (stepData.Pattern != null)
            {
                step.AffectedCells.AddRange(stepData.AffectedCells);
                
                // Add pattern-specific cells
                step.AffectedCells.AddRange(stepData.Pattern.PrimaryCells);
                step.AffectedCells.AddRange(stepData.Pattern.SecondaryCells);
                
                // Remove duplicates
                step.AffectedCells = step.AffectedCells.Distinct().ToList();
            }

            Steps.Add(step);
        }

        HasSteps = Steps.Count > 0;
        UpdateNavigationState();
        Status = Steps.Count > 0 ? $"Loaded {Steps.Count} solving steps" : "No steps available";
    }

    

    private async Task ApplyStep(SolvingStep step)
    {
        // Clear all highlights first
        ClearHighlights();
        
        // Apply strategy-specific highlighting with animation
        if (EnableAnimations)
        {
            await ApplyStrategySpecificHighlightsAnimated(step);
        }
        else
        {
            ApplyStrategySpecificHighlights(step);
        }

        // Apply cell changes with animation
        foreach (var change in step.CellChanges)
        {
            var cell = _sudokuGrid.Cells[change.Position.Row - 1][change.Position.Column - 1];
            
            if (EnableAnimations && change.IsValueChange)
            {
                await AnimateValuePlacement(cell, change.NewValue);
            }
            else
            {
                cell.SetValue(change.NewValue);
            }
            
            // Highlight value placement
            if (change.IsValueChange)
            {
                cell.SetHighlightType(HighlightType.ValuePlacement);
            }
        }

        Status = $"Step {step.StepNumber}: {step.GetFormattedDescription()}";
    }
    
    private async Task ApplyStrategySpecificHighlightsAnimated(SolvingStep step)
    {
        // Apply highlights in sequence for better visual effect
        var strategyName = step.StrategyName;
        var allCells = _sudokuGrid.Cells.SelectMany(row => row).ToList();
        
        // First, highlight primary pattern cells
        foreach (var cell in allCells)
        {
            var position = cell.Position;
            if (step.AffectedCells.Contains(position))
            {
                var highlightType = DetermineHighlightType(strategyName, position, step);
                var highlightedCandidates = GetHighlightedCandidates(strategyName, position, step);
                var eliminatedCandidates = GetEliminatedCandidates(strategyName, position, step);
                
                cell.SetHighlightType(highlightType, highlightedCandidates, eliminatedCandidates);
                await Task.Delay((int)(AnimationDuration / 10)); // Small delay between cells
            }
        }
        
        // Then, highlight secondary affected areas
        await Task.Delay((int)(AnimationDuration / 2));
        
        foreach (var cell in allCells)
        {
            var position = cell.Position;
            if (!step.AffectedCells.Contains(position) && IsInAffectedArea(position, step))
            {
                cell.SetHighlightType(HighlightType.SecondaryPattern);
                await Task.Delay((int)(AnimationDuration / 20)); // Smaller delay
            }
        }
    }
    
    private async Task AnimateValuePlacement(SudokuCellViewModel cell, int newValue)
    {
        // Create a subtle animation for value placement
        // This would require more complex animation support in Avalonia
        // For now, we'll use a simple delay
        await Task.Delay((int)(AnimationDuration / 2));
        cell.SetValue(newValue);
    }
    
    private void ApplyStrategySpecificHighlights(SolvingStep step)
    {
        var strategyName = step.StrategyName;
        
        foreach (var cell in _sudokuGrid.Cells.SelectMany(row => row))
        {
            var position = cell.Position;
            var highlightType = DetermineHighlightType(strategyName, position, step);
            var highlightedCandidates = GetHighlightedCandidates(strategyName, position, step);
            var eliminatedCandidates = GetEliminatedCandidates(strategyName, position, step);
            
            cell.SetHighlightType(highlightType, highlightedCandidates, eliminatedCandidates);
        }
    }
    
    private HighlightType DetermineHighlightType(string strategyName, Position position, SolvingStep step)
    {
        // Check if this is a primary pattern cell
        if (step.AffectedCells.Contains(position))
        {
            return strategyName switch
            {
                "NakedSingles" or "HiddenSingles" => HighlightType.ValuePlacement,
                "NakedPairs" or "HiddenPairs" => HighlightType.PrimaryPattern,
                "NakedTriplets" or "HiddenTriplets" => HighlightType.PrimaryPattern,
                "NakedQuadruplets" or "HiddenQuadruplets" => HighlightType.PrimaryPattern,
                "XWing" => HighlightType.PrimaryPattern,
                "PointingTuples" => HighlightType.PrimaryPattern,
                "BoxLineReduction" => HighlightType.PrimaryPattern,
                _ => HighlightType.CandidateElimination
            };
        }
        
        // Check for affected rows, columns, or boxes
        if (IsInAffectedArea(position, step))
        {
            return HighlightType.SecondaryPattern;
        }
        
        return HighlightType.None;
    }
    
    private List<int> GetHighlightedCandidates(string strategyName, Position position, SolvingStep step)
    {
        var highlightedCandidates = new List<int>();
        
        // For pair/triplet strategies, highlight the candidates that form the pattern
        if (step.AffectedCells.Contains(position))
        {
            if (strategyName.Contains("Pair") || strategyName.Contains("Triplet") || strategyName.Contains("Quadruplet"))
            {
                // Get the candidates that should be highlighted for this pattern
                // This would need to be stored in the step data or calculated
                // For now, we'll use a simplified approach
                var cellChanges = step.CellChanges.Where(c => c.Position.Equals(position)).ToList();
                foreach (var change in cellChanges)
                {
                    highlightedCandidates.AddRange(change.RemovedCandidates);
                }
            }
        }
        
        return highlightedCandidates.Distinct().ToList();
    }
    
    private List<int> GetEliminatedCandidates(string strategyName, Position position, SolvingStep step)
    {
        var eliminatedCandidates = new List<int>();
        
        // Get candidates that were eliminated from this cell
        var cellChanges = step.CellChanges.Where(c => c.Position.Equals(position) && c.RemovedCandidates.Count > 0).ToList();
        foreach (var change in cellChanges)
        {
            eliminatedCandidates.AddRange(change.RemovedCandidates);
        }
        
        return eliminatedCandidates.Distinct().ToList();
    }
    
    private bool IsInAffectedArea(Position position, SolvingStep step)
    {
        // Check if position is in the same row, column, or box as any affected cell
        foreach (var affectedCell in step.AffectedCells)
        {
            if (position.Row == affectedCell.Row || 
                position.Column == affectedCell.Column || 
                GetBoxNumber(position) == GetBoxNumber(affectedCell))
            {
                return true;
            }
        }
        
        return false;
    }
    
    private int GetBoxNumber(Position position)
    {
        return ((position.Row - 1) / 3) * 3 + ((position.Column - 1) / 3) + 1;
    }

    private void ResetToStep(SolvingStep step)
    {
        // Load the puzzle state before this step
        if (!string.IsNullOrEmpty(step.PuzzleStateBefore))
        {
            _sudokuGrid.LoadPuzzle(step.PuzzleStateBefore);
        }
        
        ClearHighlights();
        Status = $"Reset to step {step.StepNumber}";
    }

    private void ClearHighlights()
    {
        foreach (var cell in _sudokuGrid.Cells.SelectMany(row => row))
        {
            cell.ClearHighlights();
            cell.SetStepHighlight(false);
        }
    }

    private void UpdateNavigationState()
    {
        CanStepForward = HasSteps && CurrentStepIndex < Steps.Count - 1;
        CanStepBackward = HasSteps && CurrentStepIndex > 0;
        
        // Clear all previous selections
        foreach (var step in Steps)
        {
            step.IsSelected = false;
        }
        
        // Highlight current step
        if (CurrentStepIndex >= 0 && CurrentStepIndex < Steps.Count)
        {
            SelectedStep = Steps[CurrentStepIndex];
            SelectedStep.IsSelected = true;
        }
    }

    

    partial void OnSelectedStepChanged(SolvingStep? value)
    {
        if (value != null)
        {
            CurrentStepIndex = Steps.IndexOf(value);
            UpdateNavigationState();
            UpdateStrategyExplanation(value);
        }
    }
    
    private void UpdateStrategyExplanation(SolvingStep step)
    {
        StrategyExplanation = GetStrategyExplanation(step);
    }
    
    private string GetStrategyExplanation(SolvingStep step)
    {
        return step.StrategyName switch
        {
            "NakedSingles" => GetNakedSinglesExplanation(step),
            "HiddenSingles" => GetHiddenSinglesExplanation(step),
            "NakedPairs" => GetNakedPairsExplanation(step),
            "HiddenPairs" => GetHiddenPairsExplanation(step),
            "NakedTriplets" => GetNakedTripletsExplanation(step),
            "HiddenTriplets" => GetHiddenTripletsExplanation(step),
            "NakedQuadruplets" => GetNakedQuadrupletsExplanation(step),
            "HiddenQuadruplets" => GetHiddenQuadrupletsExplanation(step),
            "PointingTuples" => GetPointingTuplesExplanation(step),
            "BoxLineReduction" => GetBoxLineReductionExplanation(step),
            "XWing" => GetXWingExplanation(step),
            "PruneCandidates" => GetPruneCandidatesExplanation(step),
            _ => $"Applied {step.StrategyName} strategy affecting {step.AffectedCells.Count} cells."
        };
    }
    
    private string GetNakedSinglesExplanation(SolvingStep step)
    {
        return "Naked Singles: When a cell has only one possible candidate, that candidate must be the correct value for that cell. " +
               $"This step placed {step.CellChanges.Count(c => c.IsValueChange)} value(s) by identifying cells with single candidates.";
    }
    
    private string GetHiddenSinglesExplanation(SolvingStep step)
    {
        return "Hidden Singles: When a candidate appears only once in a row, column, or box, it must be placed in that cell, " +
               "even if that cell has other candidates. " +
               $"This step placed {step.CellChanges.Count(c => c.IsValueChange)} value(s) by finding hidden singles.";
    }
    
    private string GetNakedPairsExplanation(SolvingStep step)
    {
        return "Naked Pairs: When two cells in the same area contain exactly the same two candidates, " +
               "those two candidates can be eliminated from all other cells in that area. " +
               $"This step eliminated candidates from {step.AffectedCells.Count} cells.";
    }
    
    private string GetHiddenPairsExplanation(SolvingStep step)
    {
        return "Hidden Pairs: When two candidates appear only in exactly two cells within an area, " +
               "those two cells must contain those candidates, eliminating other candidates from them. " +
               $"This step eliminated candidates from {step.AffectedCells.Count} cells.";
    }
    
    private string GetNakedTripletsExplanation(SolvingStep step)
    {
        return "Naked Triplets: When three cells in the same area contain exactly the same three candidates, " +
               "those three candidates can be eliminated from all other cells in that area. " +
               $"This step eliminated candidates from {step.AffectedCells.Count} cells.";
    }
    
    private string GetHiddenTripletsExplanation(SolvingStep step)
    {
        return "Hidden Triplets: When three candidates appear only in exactly three cells within an area, " +
               "those three cells must contain those candidates, eliminating other candidates from them. " +
               $"This step eliminated candidates from {step.AffectedCells.Count} cells.";
    }
    
    private string GetNakedQuadrupletsExplanation(SolvingStep step)
    {
        return "Naked Quadruplets: When four cells in the same area contain exactly the same four candidates, " +
               "those four candidates can be eliminated from all other cells in that area. " +
               $"This step eliminated candidates from {step.AffectedCells.Count} cells.";
    }
    
    private string GetHiddenQuadrupletsExplanation(SolvingStep step)
    {
        return "Hidden Quadruplets: When four candidates appear only in exactly four cells within an area, " +
               "those four cells must contain those candidates, eliminating other candidates from them. " +
               $"This step eliminated candidates from {step.AffectedCells.Count} cells.";
    }
    
    private string GetPointingTuplesExplanation(SolvingStep step)
    {
        return "Pointing Tuples: When candidates are confined to a single row or column within a box, " +
               "those candidates can be eliminated from the rest of that row or column outside the box. " +
               $"This step eliminated candidates from {step.AffectedCells.Count} cells.";
    }
    
    private string GetBoxLineReductionExplanation(SolvingStep step)
    {
        return "Box-Line Reduction: When candidates are confined to a single box within a row or column, " +
               "those candidates can be eliminated from the rest of that box. " +
               $"This step eliminated candidates from {step.AffectedCells.Count} cells.";
    }
    
    private string GetXWingExplanation(SolvingStep step)
    {
        return "X-Wing: When a candidate appears in exactly two cells in two different rows, " +
               "and those cells are in the same two columns, that candidate can be eliminated from all other cells in those columns. " +
               $"This step eliminated candidates from {step.AffectedCells.Count} cells.";
    }
    
    private string GetPruneCandidatesExplanation(SolvingStep step)
    {
        return "Prune Candidates: Remove candidates that are already placed as values in the same row, column, or box. " +
               "This is a basic housekeeping step to keep the candidate list accurate. " +
               $"This step processed {step.AffectedCells.Count} cells.";
    }
}