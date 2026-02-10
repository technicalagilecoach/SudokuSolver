using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using SudokuSolver.GUI.ViewModels;

namespace SudokuSolver.GUI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Add key handler for Ctrl+Q
        KeyDown += OnKeyDown;
    }
    
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        // Check for Ctrl+Q
        if (e.Key == Key.Q && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            // Trigger the exit command
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.ExitCommand.Execute(null);
            }
        }
    }
}