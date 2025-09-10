using System.Collections;
using static SudokuSolverCore.IndicesAndIterators;
using static SudokuSolverCore.Puzzle;

namespace SudokuSolverCore;

internal class HiddenDoublePairs(int[,] cells, BitArray[,] candidates)
{
    public bool Handle()
    {
        var valueModified = false;

        foreach (var row in AllRows)
        {
            var allCellsOfInterest = GetIndicesForRow(row);
            var allPairsOfCells = GetIndicesForDistinctPairs(allCellsOfInterest);

            valueModified = FindTwinsAndEliminateThemFromPotentialValues(allPairsOfCells, valueModified, allCellsOfInterest);
        }
        
        foreach (var column in AllColumns)
        {
            var allCellsOfInterest = GetIndicesForColumn(column);
            var allPairsOfCells = GetIndicesForDistinctPairs(allCellsOfInterest);
            
            valueModified = FindTwinsAndEliminateThemFromPotentialValues(allPairsOfCells, valueModified, allCellsOfInterest);
        }
        
        foreach (var region in AllRegions)
        {
            var allCellsOfInterest = GetIndicesForRegion(region);
            var allPairsOfCells = GetIndicesForDistinctPairs(allCellsOfInterest);
            
            valueModified = FindTwinsAndEliminateThemFromPotentialValues(allPairsOfCells, valueModified, allCellsOfInterest);
        }
        
        return valueModified;
    }

    private bool FindTwinsAndEliminateThemFromPotentialValues(List<(Position, Position)> allPairsOfCells, bool valueModified, List<Position> allCellsOfInterest)
    {
        var digits = new int[GridSize];

        foreach (var cell in allCellsOfInterest)
        {
            if (IsUndefined(cell))
            {
                foreach (var digit in AllDigits)
                {
                    if (candidates[cell.Row, cell.Column][digit])
                        digits[digit]++;
                }
            }
        }

        var candidatesForPairs = new BitArray(digits.Select(digit => digits[digit] == 2).ToArray());

        var foundTwins = new List<(Position, Position)>();
        
        foreach (var pair in allPairsOfCells)
        {
            BitArray result = new BitArray(GridSize,true);
            var c1 = candidates[pair.Item1.Row, pair.Item1.Column];
            var c2 = candidates[pair.Item2.Row, pair.Item2.Column];
            
            result.And(c1).And(c2).And(candidatesForPairs);

            var count = 0;
            for (var index = 0; index < result.Count; index++)
            {
                if (result[index])
                    count++;
            }

            if (count == 2)
            {
                foundTwins.Add(pair);
            }
        }

        //ToDo: Remove Candidates
        
        return valueModified;
    }

    private bool IsUndefined(Position position)
    {
        return cells[position.Row, position.Column] == Undefined;
    }
}
