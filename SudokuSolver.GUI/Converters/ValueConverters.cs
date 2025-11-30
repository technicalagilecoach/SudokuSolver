using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace SudokuSolver.GUI.Converters;

public class BooleanToPlayPauseStringConverter : IValueConverter
{
    public static readonly BooleanToPlayPauseStringConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isPlaying)
        {
            return isPlaying ? "⏸" : "▶";
        }
        return "▶";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class SelectedToBackgroundConverter : IValueConverter
{
    public static readonly SelectedToBackgroundConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSelected)
        {
            return isSelected ? Avalonia.Media.Color.FromArgb(255, 173, 216, 230) : Avalonia.Media.Colors.White;
        }
        return Avalonia.Media.Colors.White;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}