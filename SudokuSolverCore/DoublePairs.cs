using System.Collections;
using static SudokuSolverCore.Grid;

namespace SudokuSolverCore;

internal class DoublePairs(Cell[,] cells)
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
        var cell1 = cells[pair.Item1.row, pair.Item1.column];
        var cell2 = cells[pair.Item2.row, pair.Item2.column];
        var cellsAreEqual = cell1.IsEqualTo(cell2);
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
        var cell1 = cells[row1, column1];
        var cell3 = cells[row3, column3];

        foreach (var i in AllDigits)
        {
            if (cell1.PotentialValues[i] && cell3.PotentialValues[i])
            {
                cell3.PotentialValues[i] = false;
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
            
            if (cells[row, column].CountPotentialValues() == 2)
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
