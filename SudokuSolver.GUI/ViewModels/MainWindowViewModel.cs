using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SudokuSolver.GUI.Services;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace SudokuSolver.GUI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private SudokuGridViewModel _sudokuGrid = new();
    
    [ObservableProperty]
    private SolvingStepsViewModel _solvingSteps;

    private readonly IFileDialogService _fileDialogService;

    public MainWindowViewModel(IFileDialogService fileDialogService)
    {
        _fileDialogService = fileDialogService;
        SolvingSteps = new SolvingStepsViewModel(_sudokuGrid);
        _sudokuGrid.OnSolvingCompleted += OnSolvingCompleted;
    }
    
    private void OnSolvingCompleted(Solver solver)
    {
        _solvingSteps.LoadStepsFromSolver(solver);
    }
    
    private string NormalizePuzzleFormat(string puzzleText, string fileExtension)
    {
        // Remove any whitespace first
        puzzleText = string.Concat(puzzleText.Where(c => !char.IsWhiteSpace(c)));
        
        return fileExtension switch
        {
            ".csv" => NormalizeCsvFormat(puzzleText),
            ".txt" => NormalizeTextFormat(puzzleText),
            _ => NormalizeTextFormat(puzzleText) // Default to text format
        };
    }
    
    private string NormalizeTextFormat(string puzzleText)
    {
        // Handle different text formats:
        // 1. "530070000600195000098000060800060003400803001700020006060000280000419005000080079" (single line)
        // 2. Grid format with spaces and separators
        // 3. Line-by-line format
        
        var digits = new List<char>();
        
        // Extract all digits from the text
        foreach (char c in puzzleText)
        {
            if (char.IsDigit(c))
            {
                digits.Add(c);
            }
        }
        
        // If we have exactly 81 digits, use them directly
        if (digits.Count == 81)
        {
            return new string(digits.ToArray());
        }
        
        // If we have fewer digits, try to parse grid format
        return ParseGridFormat(puzzleText);
    }
    
    private string NormalizeCsvFormat(string puzzleText)
    {
        // Handle CSV format (if any)
        var lines = puzzleText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var digits = new List<char>();
        
        foreach (var line in lines)
        {
            var values = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var value in values)
            {
                if (int.TryParse(value.Trim(), out int digit) && digit >= 0 && digit <= 9)
                {
                    digits.Add(digit.ToString()[0]);
                }
            }
        }
        
        return digits.Count == 81 ? new string(digits.ToArray()) : ParseGridFormat(puzzleText);
    }
    
    private string ParseGridFormat(string puzzleText)
    {
        // Try to parse as a 9x9 grid with various separators
        var lines = puzzleText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        var grid = new int[9, 9];
        var row = 0;
        
        foreach (var line in lines)
        {
            if (row >= 9) break;
            
            var cleanLine = line.Trim();
            if (string.IsNullOrEmpty(cleanLine)) continue;
            
            // Split by various separators (space, tab, comma, etc.)
            var parts = cleanLine.Split(new[] { ' ', '\t', ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
            var col = 0;
            
            foreach (var part in parts)
            {
                if (col >= 9) break;
                
                if (int.TryParse(part.Trim(), out int digit) && digit >= 0 && digit <= 9)
                {
                    grid[row, col] = digit;
                }
                else
                {
                    grid[row, col] = 0; // Default to empty
                }
                col++;
            }
            row++;
        }
        
        // Convert grid to string
        var result = "";
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                result += grid[r, c].ToString();
            }
        }
        
        return result;
    }

    [RelayCommand]
    private async Task LoadPuzzleFromFile()
    {
        var fileTypes = new[]
        {
            new Avalonia.Platform.Storage.FilePickerFileType("Sudoku Files") { Patterns = new[] { "*.txt", "*.sudoku", "*.csv" } },
            new Avalonia.Platform.Storage.FilePickerFileType("All Files") { Patterns = new[] { "*.*" } }
        };

        var filePath = await _fileDialogService.OpenFileAsync("Load Sudoku Puzzle", fileTypes);
        if (filePath != null)
        {
            try
            {
                var puzzleText = await File.ReadAllTextAsync(filePath);
                var normalizedPuzzle = NormalizePuzzleFormat(puzzleText, Path.GetExtension(filePath).ToLower());
                SudokuGrid.LoadPuzzle(normalizedPuzzle);
            }
            catch (Exception ex)
            {
                SudokuGrid.Status = $"Error loading file: {ex.Message}";
            }
        }
    }

    [RelayCommand]
    private async Task SavePuzzleToFile()
    {
        var fileTypes = new[]
        {
            new Avalonia.Platform.Storage.FilePickerFileType("Sudoku Files") { Patterns = new[] { "*.txt", "*.sudoku" } },
            new Avalonia.Platform.Storage.FilePickerFileType("All Files") { Patterns = new[] { "*.*" } }
        };

        var filePath = await _fileDialogService.SaveFileAsync("Save Sudoku Puzzle", fileTypes);
        if (filePath != null)
        {
            try
            {
                var puzzleText = SudokuGrid.GetPuzzleString();
                await File.WriteAllTextAsync(filePath, puzzleText);
                SudokuGrid.Status = "Puzzle saved successfully";
            }
            catch (Exception ex)
            {
                SudokuGrid.Status = $"Error saving file: {ex.Message}";
            }
        }
    }
}
