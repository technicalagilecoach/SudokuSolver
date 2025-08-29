using static SudokuSolverCore.Grid;

namespace SudokuSolverCore;

internal static class CellHelpers
{ 
    internal static string IntToString(int value)
    {
        return value == Undefined ? " " : value.ToString();
    }
    internal static int StringToInt(string value)
    {
        return value == " " ? Undefined : int.Parse(value);
    }
}