using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SudokuSolver;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;
using System.Linq;

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
    
    private bool _showCandidates = true;

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
                cellViewModel.GlobalShowCandidates = _showCandidates;
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
        if (SelectedCell == cell)
        {
            // Unmark if clicking the same cell again
            SelectedCell = null;
            cell.IsSelected = false;
            
            // Clear all highlights
            foreach (var c in Cells.SelectMany(row => row))
            {
                c.SetHighlight(false);
            }
        }
        else
        {
            if (SelectedCell != null)
            {
                SelectedCell.IsSelected = false;
            }

            SelectedCell = cell;
            cell.IsSelected = true;
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
                // Don't update grid from solver immediately - we want to show only initial state for playback
                // UpdateGridFromSolver();
                
                // Reset grid to initial puzzle state for proper playback
                UpdateAllCellsFromPuzzle();
                
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
    
    public void SetShowCandidates(bool showCandidates)
    {
        _showCandidates = showCandidates;
        foreach (var row in Cells)
        {
            foreach (var cell in row)
            {
                cell.GlobalShowCandidates = showCandidates;
            }
        }
    }

    

    private void PrunePuzzleCandidates()
    {
        // Remove candidates based on existing values in the puzzle
        for (int row = 0; row < Puzzle.GridSize; row++)
        {
            for (int col = 0; col < Puzzle.GridSize; col++)
            {
                var position = new Position(row + 1, col + 1);
                var value = _puzzle.GetCellValue(position);
                
                if (value != 0)
                {
                    // Remove this value from candidates in the same row
                    for (int c = 0; c < Puzzle.GridSize; c++)
                    {
                        if (c != col)
                        {
                            var rowPos = new Position(row + 1, c + 1);
                            var candidates = _puzzle.GetCandidates(rowPos);
                            candidates[value - 1] = false;
                        }
                    }
                    
                    // Remove this value from candidates in the same column
                    for (int r = 0; r < Puzzle.GridSize; r++)
                    {
                        if (r != row)
                        {
                            var colPos = new Position(r + 1, col + 1);
                            var candidates = _puzzle.GetCandidates(colPos);
                            candidates[value - 1] = false;
                        }
                    }
                    
                    // Remove this value from candidates in the same box
                    var boxRow = (row / 3) * 3;
                    var boxCol = (col / 3) * 3;
                    for (int r = boxRow; r < boxRow + 3; r++)
                    {
                        for (int c = boxCol; c < boxCol + 3; c++)
                        {
                            if (r != row || c != col)
                            {
                                var boxPos = new Position(r + 1, c + 1);
                                var candidates = _puzzle.GetCandidates(boxPos);
                                candidates[value - 1] = false;
                            }
                        }
                    }
                }
            }
        }
    }

    [RelayCommand]
    public void LoadPuzzle(string puzzleString)
    {
        try
        {
            _puzzle.Init(puzzleString);
            PrunePuzzleCandidates();
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