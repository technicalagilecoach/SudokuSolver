using static SudokuSolver.IndicesAndIterators;
using static SudokuSolver.Puzzle;

namespace SudokuSolver;

public class PointingPairs(Puzzle puzzle) : Strategy(puzzle) {
    private int _numberOfRemovedCandidates;
    
    public bool Handle()
    {
        _numberOfRemovedCandidates = 0;

        foreach (var box in AllBoxes)
        {
            var allCellsInBox = GetIndicesForBox(box);
            
            HandlePointingPairsInRows(allCellsInBox, box);
            HandlePointingPairsInColumns(allCellsInBox, box);
        }

        return _numberOfRemovedCandidates>0;
    }

    private void HandlePointingPairsInRows(List<Position> allCellsInBox, int box)
    {
        var candidatesInRelevantRows = CreateValuesRows(allCellsInBox);
        var pointingPairsInRows = FindPointingPairsInRows(box, candidatesInRelevantRows);
        var columnsWithUndefinedCells = allCellsInBox.Where(IsUndefined).Select(pos => pos.Column).ToHashSet();
        RemoveCandidatesBasedOnPointingPairsInRows(pointingPairsInRows, columnsWithUndefinedCells);
    }

    private void HandlePointingPairsInColumns(List<Position> allCellsInBox, int box)
    {
        var candidatesInRelevantColumns = CreateValuesColumns(allCellsInBox);
        var pointingPairsInColumns = FindPointingPairsInColumns(box, candidatesInRelevantColumns);
        var rowsWithUndefinedCells = allCellsInBox.Where(IsUndefined).Select(pos => pos.Row).ToHashSet();
        RemoveCandidatesBasedOnPointingPairsInColumns(pointingPairsInColumns, rowsWithUndefinedCells);
    }
    
    private int[,] CreateValuesRows(List<Position> allCellsInBox)
    {
        var candidatesInRelevantRows = new int[BoxSize,GridSize];
        
        //var found = false;

        foreach (var position in allCellsInBox.Where(IsUndefined))
        {
            var candidates = GetCandidates(position);
            foreach (var digit in AllDigits)
            {
                if (candidates[digit])
                {
                    candidatesInRelevantRows[position.Row % 3, digit]++; //count how many times the candidate occurs in this row
                    //found = true;
                }
            }
        }
        
        //ToDo: if valuesRows only contains 0s then the rest can be skipped
        
        return candidatesInRelevantRows;
    }

    private static List<(int, int)> FindPointingPairsInRows(int box, int[,] valuesRows)
    {
        var pointingPairsInRows = new List<(int, int)>();
        
        foreach (var digit in AllDigits)
        {
            int offset = GetIndexOfPositiveComponent(valuesRows[0,digit], valuesRows[1,digit], valuesRows[2,digit]);
            if (offset >= 0)
            {
                var pos = GetBoxCoordinates(box);
                pointingPairsInRows.Add((digit, pos.Row+offset));                    
            }
        }

        return pointingPairsInRows;
    }
    
    private void RemoveCandidatesBasedOnPointingPairsInRows(List<(int, int)> pointingPairsInRows, HashSet<int> columnsWithUndefinedCells)
    {
        foreach (var (digit, row) in pointingPairsInRows)
        {
            foreach (var column in AllColumns)
            {
                if (IsUndefined(row,column) && !columnsWithUndefinedCells.Contains(column) && Candidates[row,column][digit])
                {
                    Candidates[row,column][digit] = false;
                    _numberOfRemovedCandidates++;
                }
            }
        }
    }

    private int[,] CreateValuesColumns(List<Position> allCellsInBox)
    {
        var candidatesInRelevantColumns = new int[BoxSize,GridSize];
        
        //var found = false;

        foreach (var position in allCellsInBox.Where(IsUndefined))
        {
            var candidates = GetCandidates(position);
            foreach (var digit in AllDigits)
            {
                if (candidates[digit])
                {
                    candidatesInRelevantColumns[position.Column % 3, digit]++; //count how many times the candidate occurs in this row
                    //found = true;
                }
            }
        }
        
        //ToDo: if valuesColumns only contains 0s then the rest can be skipped
        
        return candidatesInRelevantColumns;
    }

    private static List<(int, int)> FindPointingPairsInColumns(int box, int[,] valuesColumns)
    {
        var pointingPairsInColumns = new List<(int, int)>();
        
        foreach (var digit in AllDigits)
        {
            int offset = GetIndexOfPositiveComponent(valuesColumns[0,digit], valuesColumns[1,digit], valuesColumns[2,digit]);
            if (offset >= 0)
            {
                var pos = GetBoxCoordinates(box);
                pointingPairsInColumns.Add((digit, pos.Column+offset));                    
            }
        }

        return pointingPairsInColumns;
    }
    
    private void RemoveCandidatesBasedOnPointingPairsInColumns(List<(int, int)> pointingPairsInColumns, HashSet<int> rowsWithUndefinedCells)
    {
        foreach (var (digit, column) in pointingPairsInColumns)
        {
            foreach (var row in AllColumns)
            {
                if (IsUndefined(row,column) && !rowsWithUndefinedCells.Contains(row) && Candidates[row,column][digit])
                {
                    Candidates[row,column][digit] = false;
                    _numberOfRemovedCandidates++;
                }
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