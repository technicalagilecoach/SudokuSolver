using static SudokuSolver.IndicesAndIterators;
using static SudokuSolver.Puzzle;

namespace SudokuSolver;

public class XWing(Puzzle puzzle) : Strategy(puzzle) 
{
    private int _numberOfRemovedCandidates;

    private bool[,] FilteredCandidates { get; set; } = null!;

    private bool GetFilteredCandidate(Position position)
    {
        return FilteredCandidates[position.Row-1, position.Column-1];
    }

    private void SetFilteredCandidate(Position position, bool value)
    {
        FilteredCandidates[position.Row-1, position.Column-1] = value;
    }
    
    public bool Handle()
    {
        _numberOfRemovedCandidates = 0;

        foreach (var number in AllDigits)
        {
            FilterCandidates(number);
            XWingRows(number);
            XWingColumns(number);
        }

        return _numberOfRemovedCandidates>0;
    }

    private void XWingRows(int number)
    {
        var rowCombinations = FindCandidatesForXWingRows();
        var xWingRows = FindXWingRows(rowCombinations);
        RemoveCandidatesBasedOnXWingRows(number, xWingRows);
    }

    private void XWingColumns(int number)
    {
        var columnCombinations = FindCandidatesForXWingColumns();
        var xWingColumns = FindXWingColumns(columnCombinations);
        RemoveCandidatesBasedOnXWingColumns(number, xWingColumns);
    }

    private List<List<int>> FindXWingRows(List<List<int>> rowCombinations)
    {
        var xWingRows = new List<List<int>>();
        foreach (var rowCombination in rowCombinations)
        {
            var areEqual = true;
            foreach (var column in AllColumns)
            {
                if (GetFilteredCandidate(new Position(rowCombination[0], column)) != GetFilteredCandidate(new Position(rowCombination[1], column)))
                {
                    areEqual = false;
                    break;
                }
            }
            if (areEqual)
                xWingRows.Add(rowCombination);
        }

        return xWingRows;
    }
    
    private List<List<int>> FindXWingColumns(List<List<int>> columnCombinations)
    {
        var xWingColumns = new List<List<int>>();
        foreach (var columnCombination in columnCombinations)
        {
            var areEqual = true;
            foreach (var row in AllRows)
            {
                if (GetFilteredCandidate(new Position(row, columnCombination[0])) != GetFilteredCandidate(new Position(row, columnCombination[1])))
                {
                    areEqual = false;
                    break;
                }
            }
            if (areEqual)
                xWingColumns.Add(columnCombination);
        }

        return xWingColumns;
    }

    private void RemoveCandidatesBasedOnXWingRows(int number, List<List<int>> xWingRows)
    {
        foreach (var pairOfRows in xWingRows)
        {
            var xWingColumns = new List<int>();
            foreach (var column in AllColumns)
            {
                if (GetFilteredCandidate(new Position(pairOfRows[0],column)))
                    xWingColumns.Add(column);
            }
                
            foreach (var row in AllRows)
            {
                if (pairOfRows[0] != row && pairOfRows[1] != row)
                {
                    var pos1 = new Position(row, xWingColumns[0]);
                    var pos2 = new Position(row, xWingColumns[1]);
                    var index = number - 1;
                    
                    if (GetCandidates(pos1)[index])
                    {
                        GetCandidates(pos1)[index] = false;
                        _numberOfRemovedCandidates++;
                    }
                    if (GetCandidates(pos2)[index])
                    {
                        GetCandidates(pos2)[index] = false;
                        _numberOfRemovedCandidates++;
                    }
                }
            }
        }
    }
    
    private List<List<int>> FindCandidatesForXWingColumns()
    {
        List<int> candidateColumns = [];
        foreach (var column in AllColumns)
        {
            var count = 0;
            foreach (var row in AllRows)
            {
                if (GetFilteredCandidate(new Position(row, column)))
                    count++;
            }
            if (count == 2)
                candidateColumns.Add(column);
        }

        var columnCombinations = Combinations<int>(candidateColumns,2);
        return columnCombinations;
    }

    private List<List<int>> FindCandidatesForXWingRows()
    {
        List<int> candidateRows = [];
        foreach (var row in AllRows)
        {
            var count = 0;
            foreach (var column in AllColumns)
            {
                if (GetFilteredCandidate(new Position(row, column)))
                    count++;
            }
            if (count == 2)
                candidateRows.Add(row);
        }

        var rowCombinations = Combinations<int>(candidateRows,2);
        return rowCombinations;
    }
    
    private void RemoveCandidatesBasedOnXWingColumns(int number, List<List<int>> xWingColumns)
    {
        foreach (var pairOfColumns in xWingColumns)
        {
            var xWingRows = new List<int>();
            foreach (var row in AllRows)
            {
                if (GetFilteredCandidate(new Position(row,pairOfColumns[0])))
                    xWingRows.Add(row);
            }
                
            foreach (var column in AllColumns)
            {
                if (pairOfColumns[0] != column && pairOfColumns[1] != column)
                {
                    var pos1 = new Position(xWingRows[0],column);
                    var pos2 = new Position(xWingRows[1],column);
                    var index = number - 1;
                    
                    if (GetCandidates(pos1)[index])
                    {
                        GetCandidates(pos1)[index] = false;
                        _numberOfRemovedCandidates++;
                    }
                    if (GetCandidates(pos2)[index])
                    {
                        GetCandidates(pos2)[index] = false;
                        _numberOfRemovedCandidates++;
                    }
                }
            }
        }
    }

    private void FilterCandidates(int number)
    {
        FilteredCandidates = new bool[GridSize, GridSize];
        foreach (var row in AllRows)
        {
            foreach (var column in AllColumns)
            {
                var position = new Position(row, column);
                if (GetCandidates(position)[number-1])
                    SetFilteredCandidate(position, true);
                else
                    SetFilteredCandidate(position, false);
            }
        }
    }
}