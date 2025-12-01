using CommunityToolkit.Mvvm.ComponentModel;
using SudokuSolver;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media;
using Avalonia;

namespace SudokuSolver.GUI.ViewModels;

public enum HighlightType
{
    None,
    ValuePlacement,
    CandidateElimination,
    PrimaryPattern,
    SecondaryPattern,
    AffectedRow,
    AffectedColumn,
    AffectedBox,
    Conflict
}

public partial class SudokuCellViewModel : ViewModelBase
{
    [ObservableProperty]
    private int _value;
    
    [ObservableProperty]
    private bool _isGiven;
    
    [ObservableProperty]
    private bool _isSelected;
    
    [ObservableProperty]
    private bool _isHighlighted;
    
    [ObservableProperty]
    private bool _hasConflict;
    
    [ObservableProperty]
    private bool _isStepHighlighted;
    
    [ObservableProperty]
    private string _candidates = "";
    
    [ObservableProperty]
    private HighlightType _highlightType = HighlightType.None;
    
    [ObservableProperty]
    private List<int> _highlightedCandidates = new();
    
    [ObservableProperty]
    private List<int> _eliminatedCandidates = new();
    
    [ObservableProperty]
    private bool _isPrimaryPatternCell;
    
    [ObservableProperty]
    private bool _isSecondaryPatternCell;

    public Position Position { get; }
    
    public ObservableCollection<string> CandidateGrid { get; } = new();

    public SudokuCellViewModel(Position position, int value = 0)
    {
        Position = position;
        Value = value;
        IsGiven = value != 0;
        InitializeCandidateGrid();
    }
    
    // Visual properties for styling
    public bool HasValue => Value != 0;
    private bool _globalShowCandidates = true;
    
    public bool GlobalShowCandidates
    {
        get => _globalShowCandidates;
        set
        {
            _globalShowCandidates = value;
            OnPropertyChanged(nameof(ShowCandidates));
        }
    }
    
    public bool ShowCandidates => Value == 0 && !string.IsNullOrEmpty(Candidates) && GlobalShowCandidates;
    public string FontWeight => IsGiven ? "Bold" : "Normal";
    public SolidColorBrush TextColor => GetTextColor();
    public SolidColorBrush BackgroundColor => GetBackgroundColor();
    public SolidColorBrush CandidateColor(int candidate) => GetCandidateColor(candidate);
    public bool IsCandidateHighlighted(int candidate) => HighlightedCandidates.Contains(candidate);
    public bool IsCandidateEliminated(int candidate) => EliminatedCandidates.Contains(candidate);
    
public Thickness BorderThickness => GetBorderThickness();
    
    private Thickness GetBorderThickness()
    {
        var left = 0.5;
        var top = 0.5;
        var right = 0.5;
        var bottom = 0.5;
        
        // Thicker borders for 3x3 box divisions
        // Right border after columns 3 and 6 - extend to center in gap
        if (Position.Column == 3 || Position.Column == 6)
            right = 4.0;
            
        // Bottom border after rows 3 and 6 - extend to center in gap
        if (Position.Row == 3 || Position.Row == 6)
            bottom = 4.0;
            
        return new Thickness(left, top, right, bottom);
    }

    private void InitializeCandidateGrid()
    {
        CandidateGrid.Clear();
        for (int i = 0; i < 9; i++)
        {
            CandidateGrid.Add("");
        }
    }

    partial void OnValueChanged(int value)
    {
        if (value != 0 && !IsGiven)
        {
            Candidates = "";
            UpdateCandidateGrid();
        }
        OnPropertyChanged(nameof(HasValue));
        OnPropertyChanged(nameof(ShowCandidates));
        OnPropertyChanged(nameof(TextColor));
        OnPropertyChanged(nameof(BackgroundColor));
    }

    partial void OnIsSelectedChanged(bool value)
    {
        OnPropertyChanged(nameof(BackgroundColor));
    }

    partial void OnIsHighlightedChanged(bool value)
    {
        OnPropertyChanged(nameof(BackgroundColor));
    }

    partial void OnHasConflictChanged(bool value)
    {
        OnPropertyChanged(nameof(TextColor));
        OnPropertyChanged(nameof(BackgroundColor));
    }

    partial void OnIsStepHighlightedChanged(bool value)
    {
        OnPropertyChanged(nameof(BackgroundColor));
    }

    partial void OnCandidatesChanged(string value)
    {
        UpdateCandidateGrid();
        OnPropertyChanged(nameof(ShowCandidates));
    }
    
    partial void OnHighlightedCandidatesChanged(List<int> value)
    {
        OnPropertyChanged(nameof(IsCandidateHighlighted));
        OnPropertyChanged(nameof(CandidateColor));
    }
    
    partial void OnEliminatedCandidatesChanged(List<int> value)
    {
        OnPropertyChanged(nameof(IsCandidateEliminated));
        OnPropertyChanged(nameof(CandidateColor));
    }
    
    partial void OnHighlightTypeChanged(HighlightType value)
    {
        OnPropertyChanged(nameof(TextColor));
        OnPropertyChanged(nameof(BackgroundColor));
        OnPropertyChanged(nameof(IsPrimaryPatternCell));
        OnPropertyChanged(nameof(IsSecondaryPatternCell));
    }

    private void UpdateCandidateGrid()
    {
        if (Value == 0 && !string.IsNullOrEmpty(Candidates))
        {
            var candidateList = Candidates.Split(',');
            CandidateGrid.Clear();
            for (int i = 0; i < 9; i++)
            {
                CandidateGrid.Add(candidateList.Contains((i + 1).ToString()) ? (i + 1).ToString() : "");
            }
        }
        else
        {
            InitializeCandidateGrid();
        }
    }

    public void UpdateFromPuzzle(Puzzle puzzle)
    {
        Value = puzzle.GetCellValue(Position);
        IsGiven = Value != 0;
        
        UpdateCandidatesFromPuzzle(puzzle);
        
        OnPropertyChanged(nameof(HasValue));
        OnPropertyChanged(nameof(FontWeight));
        OnPropertyChanged(nameof(TextColor));
        OnPropertyChanged(nameof(BackgroundColor));
    }

    public void SetValue(int newValue)
    {
        if (!IsGiven)
        {
            Value = newValue;
        }
    }

    public void Clear()
    {
        if (!IsGiven)
        {
            Value = 0;
            Candidates = "";
            UpdateCandidateGrid();
        }
    }

    public void SetHighlight(bool highlighted)
    {
        IsHighlighted = highlighted;
    }

    public void SetStepHighlight(bool highlighted)
    {
        IsStepHighlighted = highlighted;
    }

    public void SetConflict(bool hasConflict)
    {
        HasConflict = hasConflict;
    }
    
    private SolidColorBrush GetTextColor()
    {
        return IsGiven ? new SolidColorBrush(Color.FromArgb(255, 100, 100, 100)) : 
               HasConflict ? new SolidColorBrush(Color.FromArgb(255, 230, 120, 120)) :
               HighlightType switch
               {
                   HighlightType.ValuePlacement => new SolidColorBrush(Color.FromArgb(255, 0, 150, 0)), // Dark green
                   HighlightType.PrimaryPattern => new SolidColorBrush(Color.FromArgb(255, 0, 0, 200)), // Blue
                   HighlightType.SecondaryPattern => new SolidColorBrush(Color.FromArgb(255, 200, 0, 200)), // Purple
                   _ => new SolidColorBrush(Color.FromArgb(255, 120, 120, 120)) // Gray
               };
    }
    
    private SolidColorBrush GetBackgroundColor()
    {
        if (IsSelected)
            return new SolidColorBrush(Color.FromArgb(255, 240, 248, 255)); // Light blue
        
        return HighlightType switch
        {
            HighlightType.ValuePlacement => new SolidColorBrush(Color.FromArgb(255, 240, 255, 240)), // Light green
            HighlightType.CandidateElimination => new SolidColorBrush(Color.FromArgb(255, 255, 245, 240)), // Light red
            HighlightType.PrimaryPattern => new SolidColorBrush(Color.FromArgb(255, 240, 248, 255)), // Light blue
            HighlightType.SecondaryPattern => new SolidColorBrush(Color.FromArgb(255, 248, 240, 255)), // Light purple
            HighlightType.AffectedRow => new SolidColorBrush(Color.FromArgb(255, 255, 250, 240)), // Light orange
            HighlightType.AffectedColumn => new SolidColorBrush(Color.FromArgb(255, 240, 255, 250)), // Light cyan
            HighlightType.AffectedBox => new SolidColorBrush(Color.FromArgb(255, 250, 240, 255)), // Light pink
            HighlightType.Conflict => new SolidColorBrush(Color.FromArgb(255, 255, 230, 230)), // Light red
            HighlightType.None when IsStepHighlighted => new SolidColorBrush(Color.FromArgb(255, 255, 252, 240)), // Light yellow
            HighlightType.None when IsHighlighted => new SolidColorBrush(Color.FromArgb(255, 248, 248, 255)), // Light lavender
            HighlightType.None when IsGiven => new SolidColorBrush(Color.FromArgb(255, 252, 252, 252)), // Light gray
            _ => new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)) // White
        };
    }
    
    private SolidColorBrush GetCandidateColor(int candidate)
    {
        if (IsCandidateEliminated(candidate))
            return new SolidColorBrush(Color.FromArgb(255, 200, 100, 100)); // Red for eliminated
        
        if (IsCandidateHighlighted(candidate))
            return new SolidColorBrush(Color.FromArgb(255, 100, 100, 200)); // Blue for highlighted
        
        return new SolidColorBrush(Color.FromArgb(255, 180, 180, 180)); // Default gray
    }
    
    public void SetHighlightType(HighlightType highlightType, List<int>? highlightedCandidates = null, List<int>? eliminatedCandidates = null)
    {
        HighlightType = highlightType;
        HighlightedCandidates = highlightedCandidates ?? new List<int>();
        EliminatedCandidates = eliminatedCandidates ?? new List<int>();
        
        // Update pattern cell flags
        IsPrimaryPatternCell = highlightType == HighlightType.PrimaryPattern;
        IsSecondaryPatternCell = highlightType == HighlightType.SecondaryPattern;
        
        // Notify property changes
        OnPropertyChanged(nameof(TextColor));
        OnPropertyChanged(nameof(BackgroundColor));
        OnPropertyChanged(nameof(IsCandidateHighlighted));
        OnPropertyChanged(nameof(IsCandidateEliminated));
        OnPropertyChanged(nameof(CandidateColor));
    }
    
    public void ClearHighlights()
    {
        SetHighlightType(HighlightType.None);
    }
    
    public void UpdateCandidatesFromPuzzle(Puzzle puzzle)
    {
        if (Value == 0)
        {
            var candidates = puzzle.GetCandidates(Position);
            var candidateList = new List<string>();
            for (int i = 0; i < candidates.Length; i++)
            {
                if (candidates[i])
                    candidateList.Add((i + 1).ToString());
            }
            Candidates = string.Join(",", candidateList);
        }
        else
        {
            Candidates = "";
        }
        
        OnPropertyChanged(nameof(ShowCandidates));
    }
}