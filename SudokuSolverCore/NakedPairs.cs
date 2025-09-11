using System.Collections;
using static SudokuSolverCore.IndicesAndIterators;
using static SudokuSolverCore.Puzzle;

namespace SudokuSolverCore;

internal class NakedPairs(int[,] cells, BitArray[,] candidates)
{
    public bool Handle()
    {
        var valueModified = false;
        
        var undefinedCells = MarkUndefinedCells();
        var potentialTwins = MarkPotentialTwins(undefinedCells);

        foreach (var row in AllRows)
        {
            var allCellsOfInterest = GetIndicesForRow(row);
            var allPairsOfCells = GetIndicesForDistinctPairs(allCellsOfInterest);

            valueModified = FindTwinsAndEliminateThemFromPotentialValues(allPairsOfCells, potentialTwins, valueModified, allCellsOfInterest, undefinedCells);
        }
        
        foreach (var column in AllColumns)
        {
            var allCellsOfInterest = GetIndicesForColumn(column);
            var allPairsOfCells = GetIndicesForDistinctPairs(allCellsOfInterest);
            
            valueModified = FindTwinsAndEliminateThemFromPotentialValues(allPairsOfCells, potentialTwins, valueModified, allCellsOfInterest, undefinedCells);
        }
        
        foreach (var region in AllRegions)
        {
            var allCellsOfInterest = GetIndicesForRegion(region);
            var allPairsOfCells = GetIndicesForDistinctPairs(allCellsOfInterest);
            
            valueModified = FindTwinsAndEliminateThemFromPotentialValues(allPairsOfCells, potentialTwins, valueModified, allCellsOfInterest, undefinedCells);
        }
        
        return valueModified;
    }

    private bool FindTwinsAndEliminateThemFromPotentialValues(List<(Position, Position)> allPairsOfCells, bool[,] potentialTwins,
        bool valueModified, List<Position> allCellsOfInterest, bool[,] undefinedCells)
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

    private bool EliminatePotentialValuesFromOtherCells(List<Position> allCellsOfInterest, bool[,] undefinedCells,
        (Position, Position) pairOfCells)
    {
        var actualChange = false;
        
        foreach (var ele in allCellsOfInterest)
        {
            if (UndefinedAndDifferent(undefinedCells, ele, pairOfCells))
                actualChange = EliminatePotentialValuesFromOtherCells(ele, pairOfCells.Item1, actualChange);
        }

        return actualChange;
    }

    private static bool UndefinedAndDifferent(bool[,] undefinedCells, Position ele, (Position, Position) pairOfCells)
    {
        return undefinedCells[ele.Row, ele.Column] && ele != pairOfCells.Item1 && ele != pairOfCells.Item2;
    }

    private static bool ArePotentialTwins(bool[,] potentialTwins, (Position, Position) pair)
    {
        return potentialTwins[pair.Item1.Row, pair.Item1.Column] && potentialTwins[pair.Item2.Row, pair.Item2.Column];
    }

    private bool CellsAreEqual((Position, Position) pair)
    {
        var cellsAreEqual = true;
            
        foreach (var i in AllDigits)
        {
            if (candidates[pair.Item1.Row, pair.Item1.Column][i] == candidates[pair.Item2.Row, pair.Item2.Column][i]) 
                continue;
                
            cellsAreEqual = false;
            break;
        }
        
        return cellsAreEqual;
    }

    private bool EliminatePotentialValuesFromOtherCells(Position pos1, Position pos2, bool actualChange)
    {
        foreach (var i in AllDigits)
        {
            if (candidates[pos2.Row, pos2.Column][i] && candidates[pos1.Row, pos1.Column][i])
            {
                candidates[pos1.Row, pos1.Column][i] = false;
                actualChange = true;
            }
        }

        return actualChange;
    }

    private bool[,] MarkPotentialTwins(bool[,] undefinedCells)
    {
        var potentialTwins = new bool[GridSize, GridSize];
        ForEachCell(position =>
        {
            if (!undefinedCells[position.Row, position.Column]) 
                return;
            
            potentialTwins[position.Row, position.Column] = CountCandidates(position, candidates) == 2;
        });
        return potentialTwins;
    }

    private bool[,] MarkUndefinedCells()
    {
        var undefinedCells = new bool[GridSize, GridSize];
        
        ForEachCell(position =>
        {
            undefinedCells[position.Row, position.Column] = IsUndefined(position);
        });
        
        return undefinedCells;
    }

    private bool IsUndefined(Position position)
    {
        return cells[position.Row, position.Column] == Undefined;
    }
}
