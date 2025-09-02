using System.Collections;
using static SudokuSolverCore.Puzzle;

namespace SudokuSolverCore;

internal class DoublePairs(Cell[,] cells, BitArray[,] possibleValues)
{
    public bool Handle()
    {
        var valueModified = false;
        
        var undefinedCells = MarkUndefinedCells();
        var potentialTwins = MarkPotentialTwins(undefinedCells);

        foreach (var row in AllDigits)
        {
            var allCellsOfInterest = GetIndicesForRow(row);
            var allPairsOfCells = GetIndicesForDistinctPairs(allCellsOfInterest);

            valueModified = FindTwinsAndEliminateThemFromPotentialValues(allPairsOfCells, potentialTwins, valueModified, allCellsOfInterest, undefinedCells);
        }
        
        foreach (var column in AllDigits)
        {
            var allCellsOfInterest = GetIndicesForColumn(column);
            var allPairsOfCells = GetIndicesForDistinctPairs(allCellsOfInterest);
            
            valueModified = FindTwinsAndEliminateThemFromPotentialValues(allPairsOfCells, potentialTwins, valueModified, allCellsOfInterest, undefinedCells);
        }
        
        foreach (var region in AllDigits)
        {
            var allCellsOfInterest = GetIndicesForRegion(region);
            var allPairsOfCells = GetIndicesForDistinctPairs(allCellsOfInterest);
            
            valueModified = FindTwinsAndEliminateThemFromPotentialValues(allPairsOfCells, potentialTwins, valueModified, allCellsOfInterest, undefinedCells);
        }
        
        return valueModified;
    }

    private bool FindTwinsAndEliminateThemFromPotentialValues(List<((int row, int column), (int row, int column))> allPairsOfCells, bool[,] potentialTwins,
        bool valueModified, List<(int row, int column)> allCellsOfInterest, bool[,] undefinedCells)
    {
        foreach (var pairOfCells in allPairsOfCells)
        {
            if (ArePotentialTwins(potentialTwins, pairOfCells) && CellsAreEqual(pairOfCells))
            {
                valueModified = EliminatePotentialValuesFromOtherCells(allCellsOfInterest, undefinedCells, pairOfCells);
                break;
            }
        }

        return valueModified;
    }

    private bool EliminatePotentialValuesFromOtherCells(List<(int row, int column)> allCellsOfInterest, bool[,] undefinedCells,
        ((int row, int column), (int row, int column)) pairOfCells)
    {
        var actualChange = false;
        
        foreach (var ele in allCellsOfInterest)
        {
            if (UndefinedAndDifferent(undefinedCells, ele, pairOfCells))
                actualChange = EliminatePotentialValuesFromOtherCells(ele.row, ele.column, pairOfCells.Item1.row, pairOfCells.Item1.column, actualChange);
        }

        return actualChange;
    }

    private static bool UndefinedAndDifferent(bool[,] undefinedCells, (int row, int column) ele, ((int row, int column), (int row, int column)) pairOfCells)
    {
        return undefinedCells[ele.row, ele.column] && ele != pairOfCells.Item1 && ele != pairOfCells.Item2;
    }

    private static bool ArePotentialTwins(bool[,] potentialTwins, ((int row, int column), (int row, int column)) pair)
    {
        return potentialTwins[pair.Item1.row, pair.Item1.column] && potentialTwins[pair.Item2.row, pair.Item2.column];
    }

    private bool CellsAreEqual(((int row, int column), (int row, int column)) pair)
    {
        bool cellsAreEqual = true;
            
        foreach (var i in AllDigits)
        {
            if (possibleValues[pair.Item1.row, pair.Item1.column][i] == possibleValues[pair.Item2.row, pair.Item2.column][i]) 
                continue;
                
            cellsAreEqual = false;
            break;
        }
        
        return cellsAreEqual;
    }
    
    private static List<((int row, int column), (int row, int column))>  GetIndicesForDistinctPairs(List<(int, int)> indices)
    {
        var pairs = new List<((int row, int column), (int row, int column))>();

        foreach (var r1 in AllDigits.SkipLast(1))
        {
            foreach (var r2 in AllDigits.Skip(1))
            {
                pairs.Add(((indices[r1].Item1, indices[r1].Item2), (indices[r2].Item1, indices[r2].Item2)));
            }
        }

        return pairs;
    }

    private bool EliminatePotentialValuesFromOtherCells(int row3, int column3, int row1, int column1, bool actualChange)
    {
        foreach (var i in AllDigits)
        {
            if (possibleValues[row1, column1][i] && possibleValues[row3, column3][i])
            {
                possibleValues[row3, column3][i] = false;
                actualChange = true;
            }
        }

        return actualChange;
    }

    private bool[,] MarkPotentialTwins(bool[,] undefinedCells)
    {
        var potentialTwins = new bool[GridSize, GridSize];
        ForEachCell((row, column) =>
        {
            if (!undefinedCells[row, column]) 
                return;
            
            if (CountPotentialValues(possibleValues, row, column) == 2)
                potentialTwins[row, column] = true;
            else
                potentialTwins[row, column] = false;
        });
        return potentialTwins;
    }

    private bool[,] MarkUndefinedCells()
    {
        var undefinedCells = new bool[GridSize, GridSize];
        
        ForEachCell((row, column) =>
        {
            if (cells[row, column].Value == Undefined)
                undefinedCells[row, column] = true;
            else
                undefinedCells[row, column] = false;
        });
        
        return undefinedCells;
    }
}
