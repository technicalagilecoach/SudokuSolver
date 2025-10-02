using static SudokuSolver.IndicesAndIterators;
using static SudokuSolver.Puzzle;

namespace SudokuSolver;

public class XWing(Puzzle puzzle) : Strategy(puzzle) 
{
    private int _numberOfRemovedCandidates;
    
    public bool Handle()
    {
        _numberOfRemovedCandidates = 0;

        foreach (var digit in AllDigits)
        {
            var filteredCandidates = FilterCandidates(digit);
            
            //find X-Wings for rows
            // - consider only rows where the candidate occurs exactly twice
            // - check if two of these rows have the candidates in the same columns
            //eliminate candidates
            // - in the remainder of the columns where the two rows have the candidate 

            XWingRows(filteredCandidates, digit);

            //find X-Wings for columns
            //eliminate candidates
            
            XWingColumns(filteredCandidates, digit);
        }

        return _numberOfRemovedCandidates>0;
    }

    private void XWingRows(bool[,] filteredCandidates, int digit)
    {
        List<int> candidateRows = [];
        foreach (var row in AllRows)
        {
            var count = 0;
            foreach (var column in AllColumns)
            {
                if (filteredCandidates[row,column])
                    count++;
            }
            if (count == 2)
                candidateRows.Add(row);
        }

        var rowCombinations = Combinations<int>(candidateRows,2);

        var xWingRows = new List<List<int>>();
            
        foreach (var rowCombination in rowCombinations)
        {
            bool areEqual = true;
            foreach (var column in AllColumns)
            {
                if (filteredCandidates[rowCombination[0], column] != filteredCandidates[rowCombination[1], column])
                {
                    areEqual = false;
                    break;
                }
            }
            if (areEqual)
                xWingRows.Add(rowCombination);
        }

        foreach (var pairOfRows in xWingRows)
        {
            var xWingColumns = new List<int>();
            foreach (var column in AllColumns)
            {
                if (filteredCandidates[pairOfRows[0],column] == true)
                    xWingColumns.Add(column);
            }
                
            foreach (var row in AllRows)
            {
                if (pairOfRows[0] != row && pairOfRows[1] != row)
                {
                    if (Candidates[row, xWingColumns[0]][digit])
                    {
                        Candidates[row, xWingColumns[0]][digit] = false;
                        _numberOfRemovedCandidates++;
                    }
                    if (Candidates[row, xWingColumns[1]][digit])
                    {
                        Candidates[row, xWingColumns[1]][digit] = false;
                        _numberOfRemovedCandidates++;
                    }
                }
            }
        }
    }

    private void XWingColumns(bool[,] filteredCandidates, int digit)
    {
        List<int> candidateColumns = [];
        foreach (var column in AllColumns)
        {
            var count = 0;
            foreach (var row in AllRows)
            {
                if (filteredCandidates[row,column])
                    count++;
            }
            if (count == 2)
                candidateColumns.Add(column);
        }

        var columnCombinations = Combinations<int>(candidateColumns,2);

        var xWingColumns = new List<List<int>>();
            
        foreach (var columnCombination in columnCombinations)
        {
            bool areEqual = true;
            foreach (var row in AllRows)
            {
                if (filteredCandidates[row, columnCombination[0]] != filteredCandidates[row,columnCombination[1]])
                {
                    areEqual = false;
                    break;
                }
            }
            if (areEqual)
                xWingColumns.Add(columnCombination);
        }

        foreach (var pairOfColumns in xWingColumns)
        {
            var xWingRows = new List<int>();
            foreach (var row in AllRows)
            {
                if (filteredCandidates[row,pairOfColumns[0]] == true)
                    xWingRows.Add(row);
            }
                
            foreach (var column in AllColumns)
            {
                if (pairOfColumns[0] != column && pairOfColumns[1] != column)
                {
                    if (Candidates[xWingRows[0],column][digit])
                    {
                        Candidates[xWingRows[0],column][digit] = false;
                        _numberOfRemovedCandidates++;
                    }
                    if (Candidates[xWingRows[1],column][digit])
                    {
                        Candidates[xWingRows[1],column][digit] = false;
                        _numberOfRemovedCandidates++;
                    }
                }
            }
        }
    }

    
    private bool[,] FilterCandidates(int digit)
    {
        var filteredCandidates = new bool[GridSize, GridSize];
        foreach (var row in AllRows)
        {
            foreach (var column in AllColumns)
            {
                if (GetCandidates(new Position(row,column))[digit])
                    filteredCandidates[row,column] = true;
                else
                    filteredCandidates[row,column] = false;
            }
        }

        return filteredCandidates;
    }
}