using static SudokuSolverCore.Grid;

internal static class CellHelpers
{ 
    internal static string IntToString(int value)
    {
        if (value == UNDEFINED)
            return " ";

        return value.ToString();
    }
    internal static int StringToInt(string value)
    {
        if (value == " ")
            return UNDEFINED;

        return Int32.Parse(value);
    }
}