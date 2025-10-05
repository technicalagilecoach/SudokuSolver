using static SudokuSolver.IndicesAndIterators;
using static SudokuSolver.Puzzle;

namespace SudokuSolver;

public class PointingTuples(Puzzle puzzle) : Strategy(puzzle) {
    private int _numberOfRemovedCandidates;
    
    public bool Handle()
    {
        _numberOfRemovedCandidates = 0;

        foreach (var box in AllBoxes)
        {
            var allCellsInBox = GetIndicesForBox(box);
            
            HandlePointingTuplesInRows(allCellsInBox, box);
            HandlePointingTuplesInColumns(allCellsInBox, box);
        }

        return _numberOfRemovedCandidates>0;
    }

    private void HandlePointingTuplesInRows(List<Position> allCellsInBox, int box)
    {
        var candidatesInRelevantRows = CountCandidatesInRelevantArea(allCellsInBox, position => position.Row, out var candidatesFound);
        if (!candidatesFound) 
            return;
        var pointingTuplesInRows = FindPointingTuples(GetBoxCoordinates(box).Row, candidatesInRelevantRows);
        var columnsWithUndefinedCells = allCellsInBox.Where(IsUndefined).Select(pos => pos.Column).ToHashSet();
        RemoveCandidatesInRows(pointingTuplesInRows, columnsWithUndefinedCells);
    }

    private void HandlePointingTuplesInColumns(List<Position> allCellsInBox, int box)
    {
        var candidatesInRelevantColumns = CountCandidatesInRelevantArea(allCellsInBox, position => position.Column, out var candidatesFound);
        if (!candidatesFound) 
            return;
        var pointingTuplesInColumns = FindPointingTuples(GetBoxCoordinates(box).Column, candidatesInRelevantColumns);
        var rowsWithUndefinedCells = allCellsInBox.Where(IsUndefined).Select(pos => pos.Row).ToHashSet();
        RemoveCandidatesInColumns(pointingTuplesInColumns, rowsWithUndefinedCells);
    }
    
    private int[,] CountCandidatesInRelevantArea(List<Position> allCellsInBox, Func<Position, int> projection,
        out bool candidatesFound)
    {
        var candidatesInRelevantAreas = new int[BoxSize,GridSize];
        
        candidatesFound = false;

        foreach (var position in allCellsInBox.Where(IsUndefined))
        {
            var candidates = GetCandidates(position);
            foreach (var digit in AllDigits)
            {
                if (candidates[digit])
                {
                    candidatesInRelevantAreas[(projection(position)-1) % 3, digit]++; //count how many times the candidate occurs in this row
                    candidatesFound = true;
                }
            }
        }
        
        return candidatesInRelevantAreas;
    }
    
    private static List<(int, int)> FindPointingTuples(int baseIndex, int[,] candidateCountPerArea)
    {
        var pointingTuples = new List<(int, int)>();
        
        foreach (var digit in AllDigits)
        {
            int offset = GetIndexOfPositiveComponent(candidateCountPerArea[0,digit], candidateCountPerArea[1,digit], candidateCountPerArea[2,digit]);
            if (offset >= 0)
            {
                pointingTuples.Add((digit, baseIndex+offset));                    
            }
        }

        return pointingTuples;
    }
    
    private void RemoveCandidatesInRows(List<(int, int)> pointingTuplesInRows, HashSet<int> columnsWithUndefinedCells)
    {
        foreach (var (digit, row) in pointingTuplesInRows)
        {
            foreach (var column in AllColumns.Where(column => !columnsWithUndefinedCells.Contains(column)))
            {
                RemoveCandidate(new Position(row, column), digit, ref _numberOfRemovedCandidates);
            }
        }
    }

    private void RemoveCandidatesInColumns(List<(int, int)> pointingTuplesInColumns, HashSet<int> rowsWithUndefinedCells)
    {
        foreach (var (digit, column) in pointingTuplesInColumns)
        {
            foreach (var row in AllRows.Where(row => !rowsWithUndefinedCells.Contains(row)))
            {
                RemoveCandidate(new Position(row, column), digit, ref _numberOfRemovedCandidates);
            }
        }
    }
    
    private static int GetIndexOfPositiveComponent(int a, int b, int c)
    {
        int indexOfPositive = 0;
        int positive = 0;

        if (a > 0)
        {
            indexOfPositive=0;
            positive++;
        }

        if (b > 0)
        {
            indexOfPositive=1;
            positive++;
        }

        if (c > 0)
        {
            indexOfPositive=2;
            positive++;
        }

        bool onlyOnePositive = positive==1;
        
        if (!onlyOnePositive)
            indexOfPositive = -1;
        
        return indexOfPositive;
    }
}