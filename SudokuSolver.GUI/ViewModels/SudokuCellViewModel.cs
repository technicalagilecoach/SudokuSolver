using CommunityToolkit.Mvvm.ComponentModel;
using SudokuSolver;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media;
using Avalonia;

namespace SudokuSolver.GUI.ViewModels;

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
    public bool ShowCandidates => Value == 0 && !string.IsNullOrEmpty(Candidates);
    public string FontWeight => IsGiven ? "Bold" : "Normal";
    public SolidColorBrush TextColor => IsGiven ? new SolidColorBrush(Color.FromArgb(255, 100, 100, 100)) : 
                                        HasConflict ? new SolidColorBrush(Color.FromArgb(255, 230, 120, 120)) :
                                        new SolidColorBrush(Color.FromArgb(255, 120, 120, 120));
    
    public SolidColorBrush BackgroundColor => IsSelected ? new SolidColorBrush(Color.FromArgb(255, 240, 248, 255)) : // Lighter blue
                                               IsStepHighlighted ? new SolidColorBrush(Color.FromArgb(255, 255, 252, 240)) : // Lighter yellow
                                               IsHighlighted ? new SolidColorBrush(Color.FromArgb(255, 248, 248, 255)) : // Lighter lavender
                                               IsGiven ? new SolidColorBrush(Color.FromArgb(255, 252, 252, 252)) : // Even lighter gray
                                               HasConflict ? new SolidColorBrush(Color.FromArgb(255, 255, 245, 245)) : // Lighter red
                                               new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)); // Pure white
    
public Thickness BorderThickness => GetBorderThickness();
    
    private Thickness GetBorderThickness()
    {
        var left = 0.5;
        var top = 0.5;
        var right = 0.5;
        var bottom = 0.5;
        
        // Thicker borders for 3x3 box divisions
        // Right border after columns 3 and 6
        if (Position.Column == 3 || Position.Column == 6)
            right = 2.0;
            
        // Bottom border after rows 3 and 6  
        if (Position.Row == 3 || Position.Row == 6)
            bottom = 2.0;
            
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

    private void UpdateCandidateGrid()
    {
        if (Value == 0 && !string.IsNullOrEmpty(Candidates))
        {
            var candidateList = Candidates.Split(',');
            for (int i = 0; i < 9; i++)
            {
                CandidateGrid[i] = candidateList.Contains((i + 1).ToString()) ? (i + 1).ToString() : "";
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
        
        OnPropertyChanged(nameof(HasValue));
        OnPropertyChanged(nameof(ShowCandidates));
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
}