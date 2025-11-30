using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SudokuSolver;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;

namespace SudokuSolver.GUI.ViewModels;

public partial class SudokuGridViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<ObservableCollection<SudokuCellViewModel>> _cells = new();
    
    [ObservableProperty]
    private SudokuCellViewModel? _selectedCell;
    
    [ObservableProperty]
    private bool _isSolving;
    
    [ObservableProperty]
    private string _status = "Ready";

    private Puzzle _puzzle = new();
    private Solver? _solver;

    public SudokuGridViewModel()
    {
        InitializeGrid();
        InitializeEmptyPuzzle();
        LoadSamplePuzzle();
    }

    private void LoadSamplePuzzle()
    {
        // Sample puzzle (0 = empty)
        var samplePuzzle = "530070000600195000098000060800060003400803001700020006060000280000419005000080079";
        LoadPuzzle(samplePuzzle);
    }

    private void InitializeGrid()
    {
        Cells.Clear();
        for (int row = 0; row < Puzzle.GridSize; row++)
        {
            var rowCells = new ObservableCollection<SudokuCellViewModel>();
            for (int col = 0; col < Puzzle.GridSize; col++)
            {
                var position = new Position(row + 1, col + 1);
                var cellViewModel = new SudokuCellViewModel(position);
                rowCells.Add(cellViewModel);
            }
            Cells.Add(rowCells);
        }
    }

    public void InitializeEmptyPuzzle()
    {
        var emptyPuzzle = new string('0', Puzzle.GridSize * Puzzle.GridSize);
        _puzzle.Init(emptyPuzzle);
        UpdateAllCellsFromPuzzle();
    }

    [RelayCommand]
    private void SelectCell(SudokuCellViewModel cell)
    {
        if (SelectedCell != null)
        {
            SelectedCell.IsSelected = false;
        }

        SelectedCell = cell;
        cell.IsSelected = true;
    }

    

    [RelayCommand]
    private void ClearCell()
    {
        if (SelectedCell != null && !SelectedCell.IsGiven)
        {
            SelectedCell.Clear();
            UpdatePuzzleFromCell(SelectedCell);
        }
    }

    [RelayCommand]
    private async Task SolvePuzzleAsync()
    {
        if (IsSolving) return;

        try
        {
            IsSolving = true;
            Status = "Solving...";

            await Task.Run(() =>
            {
                _solver = new Solver(Puzzle.Copy(_puzzle));
                var solved = _solver.Solve();
                
                if (solved)
                {
                    Status = "Solved!";
                }
                else
                {
                    Status = "Cannot solve this puzzle";
                }
            });

            if (_solver != null)
            {
                UpdateGridFromSolver();
                // Notify parent view model about solving steps
                OnSolvingCompleted?.Invoke(_solver);
            }
        }
        catch (Exception ex)
        {
            Status = $"Error: {ex.Message}";
        }
        finally
        {
            IsSolving = false;
        }
    }
    
    public Action<Solver>? OnSolvingCompleted;

    [RelayCommand]
    private void ClearPuzzle()
    {
        InitializeEmptyPuzzle();
        Status = "Ready";
    }

    [RelayCommand]
    public void LoadPuzzle(string puzzleString)
    {
        try
        {
            _puzzle.Init(puzzleString);
            UpdateAllCellsFromPuzzle();
            Status = "Puzzle loaded";
        }
        catch (Exception ex)
        {
            Status = $"Error loading puzzle: {ex.Message}";
        }
    }

    private void UpdatePuzzleFromCell(SudokuCellViewModel cell)
    {
        _puzzle.SetCellValue(cell.Position, cell.Value);
    }

    private void UpdateAllCellsFromPuzzle()
    {
        for (int row = 0; row < Puzzle.GridSize; row++)
        {
            for (int col = 0; col < Puzzle.GridSize; col++)
            {
                Cells[row][col].UpdateFromPuzzle(_puzzle);
            }
        }
    }

    private void UpdateGridFromSolver()
    {
        if (_solver == null) return;

        for (int row = 0; row < Puzzle.GridSize; row++)
        {
            for (int col = 0; col < Puzzle.GridSize; col++)
            {
                var position = new Position(row + 1, col + 1);
                var value = _solver.Puzzle.GetCellValue(position);
                Cells[row][col].Value = value;
                Cells[row][col].IsGiven = value != 0;
            }
        }
    }

    public string GetPuzzleString()
    {
        return _puzzle.PrintCells("").Replace("\n", "");
    }
}