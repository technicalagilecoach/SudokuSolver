using static SudokuSolverCore.SudokuPuzzle;

internal static class CellHelpers
{ 
    internal static string IntToString(int value)
    {
        string s;
        if (value == UNDEFINED)
            s = " ";
        else
            s = value.ToString();
        return s;
    }
    internal static int StringToInt(string simplePuzzle1)
    {
        int result = UNDEFINED;
        var ch = simplePuzzle1.ToString();

        if (ch != " ")
            result = Int32.Parse(ch);
        return result;
    }
}