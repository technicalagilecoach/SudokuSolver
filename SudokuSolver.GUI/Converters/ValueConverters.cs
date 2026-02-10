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
            System.Diagnostics.Debug.WriteLine($"SelectedToBackgroundConverter: isSelected = {isSelected}");
            return new Avalonia.Media.SolidColorBrush(
                isSelected ? 
                Avalonia.Media.Color.FromArgb(255, 240, 248, 255) : 
                Avalonia.Media.Color.FromArgb(255, 255, 255, 255)
            );
        }
        System.Diagnostics.Debug.WriteLine($"SelectedToBackgroundConverter: value is not bool, it's {value?.GetType().Name}");
        return new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.White);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}