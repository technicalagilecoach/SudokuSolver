using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SudokuSolver;
using SudokuSolver.GUI.Models;
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
    private bool _isPlaying = false;
    
    [ObservableProperty]
    private bool _canStepForward = false;
    
    [ObservableProperty]
    private bool _canStepBackward = false;
    
    [ObservableProperty]
    private int _playbackSpeed = 1000; // milliseconds
    
    [ObservableProperty]
    private string _status = "No steps available";

    private SudokuGridViewModel _sudokuGrid;
    private System.Timers.Timer? _playbackTimer;

    public SolvingStepsViewModel(SudokuGridViewModel sudokuGrid)
    {
        _sudokuGrid = sudokuGrid;
        InitializePlaybackTimer();
    }

    private void InitializePlaybackTimer()
    {
        _playbackTimer = new System.Timers.Timer
        {
            Interval = PlaybackSpeed
        };
        _playbackTimer.Elapsed += OnPlaybackTimerTick;
    }

    [RelayCommand]
    private void PlayPause()
    {
        if (IsPlaying)
        {
            PausePlayback();
        }
        else
        {
            StartPlayback();
        }
    }

    [RelayCommand]
    private void StepForward()
    {
        if (CanStepForward && CurrentStepIndex < Steps.Count - 1)
        {
            CurrentStepIndex++;
            ApplyStep(Steps[CurrentStepIndex]);
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
    private void Reset()
    {
        PausePlayback();
        CurrentStepIndex = -1;
        _sudokuGrid.InitializeEmptyPuzzle();
        UpdateNavigationState();
        Status = "Reset to initial state";
    }

    [RelayCommand]
    private void GoToEnd()
    {
        PausePlayback();
        // Apply all steps
        for (int i = 0; i < Steps.Count; i++)
        {
            ApplyStep(Steps[i]);
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

            Steps.Add(step);
        }

        UpdateNavigationState();
        Status = Steps.Count > 0 ? $"Loaded {Steps.Count} solving steps" : "No steps available";
    }

    private void StartPlayback()
    {
        if (Steps.Count == 0) return;

        IsPlaying = true;
        _playbackTimer!.Interval = PlaybackSpeed;
        _playbackTimer.Start();
        Status = "Playing steps...";
    }

    private void PausePlayback()
    {
        IsPlaying = false;
        _playbackTimer?.Stop();
        Status = "Playback paused";
    }

    private void OnPlaybackTimerTick(object? sender, System.Timers.ElapsedEventArgs e)
    {
        if (CurrentStepIndex < Steps.Count - 1)
        {
            StepForward();
        }
        else
        {
            PausePlayback();
        }
    }

    private void ApplyStep(SolvingStep step)
    {
        // Highlight affected cells
        ClearHighlights();
        
        foreach (var cell in _sudokuGrid.Cells.SelectMany(row => row))
        {
            cell.SetStepHighlight(step.AffectedCells.Contains(cell.Position));
        }

        // Apply cell changes
        foreach (var change in step.CellChanges)
        {
            var cell = _sudokuGrid.Cells[change.Position.Row - 1][change.Position.Column - 1];
            cell.SetValue(change.NewValue);
        }

        Status = $"Step {step.StepNumber}: {step.GetFormattedDescription()}";
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
            cell.SetStepHighlight(false);
        }
    }

    private void UpdateNavigationState()
    {
        CanStepForward = CurrentStepIndex < Steps.Count - 1;
        CanStepBackward = CurrentStepIndex > 0;
        
        if (CurrentStepIndex >= 0 && CurrentStepIndex < Steps.Count)
        {
            SelectedStep = Steps[CurrentStepIndex];
        }
    }

    partial void OnPlaybackSpeedChanged(int value)
    {
        if (_playbackTimer != null)
        {
            _playbackTimer.Interval = value;
        }
    }

    partial void OnSelectedStepChanged(SolvingStep? value)
    {
        if (value != null)
        {
            CurrentStepIndex = Steps.IndexOf(value);
            UpdateNavigationState();
        }
    }
}