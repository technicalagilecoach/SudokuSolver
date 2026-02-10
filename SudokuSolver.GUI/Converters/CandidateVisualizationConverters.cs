using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using SudokuSolver.GUI.ViewModels;

namespace SudokuSolver.GUI.Converters;

public class CandidateIndexToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is SudokuCellViewModel cellViewModel && parameter is string candidateStr)
        {
            if (int.TryParse(candidateStr, out int candidate) && !string.IsNullOrEmpty(candidateStr))
            {
                return cellViewModel.CandidateColor(candidate);
            }
        }
        return new Avalonia.Media.SolidColorBrush(0xFFBBBBBB); // Default gray
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingOperations.DoNothing;
    }
}

public class BooleanToFontWeightConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isHighlighted && isHighlighted)
            return "Bold";
        return "Normal";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingOperations.DoNothing;
    }
}

public class BooleanToStrikethroughConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isEliminated && isEliminated)
        {
            var decoration = new TextDecoration { Location = TextDecorationLocation.Strikethrough };
            return new TextDecorationCollection { decoration };
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingOperations.DoNothing;
    }
}

public class HighlightTypeToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is HighlightType highlightType)
        {
            return highlightType switch
            {
                HighlightType.ValuePlacement => new Avalonia.Media.SolidColorBrush(0xFFE8F5E8), // Light green
                HighlightType.CandidateElimination => new Avalonia.Media.SolidColorBrush(0xFFFFF0F0), // Light red
                HighlightType.PrimaryPattern => new Avalonia.Media.SolidColorBrush(0xFFF0F8FF), // Light blue
                HighlightType.SecondaryPattern => new Avalonia.Media.SolidColorBrush(0xFFF8F0FF), // Light purple
                HighlightType.AffectedRow => new Avalonia.Media.SolidColorBrush(0xFFFFFAF0), // Light orange
                HighlightType.AffectedColumn => new Avalonia.Media.SolidColorBrush(0xFFF0FFFF), // Light cyan
                HighlightType.AffectedBox => new Avalonia.Media.SolidColorBrush(0xFFFAF0FF), // Light pink
                HighlightType.Conflict => new Avalonia.Media.SolidColorBrush(0xFFFFE6E6), // Light red
                _ => new Avalonia.Media.SolidColorBrush(0xFFFFFFFF) // White
            };
        }
        return new Avalonia.Media.SolidColorBrush(0xFFFFFFFF); // Default white
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingOperations.DoNothing;
    }
}