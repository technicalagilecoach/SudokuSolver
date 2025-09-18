namespace SudokuSolverCore;

public class Position(int row, int column)
{
    public int Row { get; } = row;
    public int Column { get; } = column;

    private bool Equals(Position? other) => other is not null && other.Row == Row && other.Column == Column;

    public override bool Equals(object? obj) => Equals(obj as Position);
    
    public override int GetHashCode() => Row*10+Column;

    public static bool operator ==(Position position1, Position position2)
    {
        return position1.Equals(position2);
    }

    public static bool operator !=(Position position1, Position position2)
    {
        return !position1.Equals(position2);
    }
}