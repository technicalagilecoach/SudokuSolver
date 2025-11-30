using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using SudokuSolver.GUI.ViewModels;

namespace SudokuSolver.GUI.Views;

public partial class SudokuGridControl : UserControl
{
    public SudokuGridControl()
    {
        InitializeComponent();
    }

    private void OnCellTapped(object? sender, TappedEventArgs e)
    {
        if (sender is Border border && border.DataContext is SudokuCellViewModel cell)
        {
            var viewModel = DataContext as SudokuGridViewModel;
            viewModel?.SelectCellCommand.Execute(cell);
            UpdateRelatedCellHighlights(cell);
        }
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        UpdateCellSelection();
    }

    private void UpdateCellSelection()
    {
        // This will be called to update visual selection
    }

    private void UpdateRelatedCellHighlights(SudokuCellViewModel selectedCell)
    {
        if (DataContext is not SudokuGridViewModel viewModel) return;

        // Clear all highlights first
        foreach (var cell in viewModel.Cells.SelectMany(row => row))
        {
            cell.SetHighlight(false);
        }

        // Highlight selected cell's row, column, and box
        foreach (var cell in viewModel.Cells.SelectMany(row => row))
        {
            bool shouldHighlight = 
                cell.Position.Row == selectedCell.Position.Row ||
                cell.Position.Column == selectedCell.Position.Column ||
                GetBoxNumber(cell.Position) == GetBoxNumber(selectedCell.Position);

            cell.SetHighlight(shouldHighlight);
        }
    }

    private int GetBoxNumber(Position position)
    {
        return ((position.Row - 1) / 3) * 3 + ((position.Column - 1) / 3) + 1;
    }
}