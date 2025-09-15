using System.Collections;
using static SudokuSolverCore.IndicesAndIterators;
using static SudokuSolverCore.Puzzle;

namespace SudokuSolverCore;

internal class HiddenPairs(Puzzle puzzle) : Strategy(puzzle)
{
    public bool Handle()
    {
        var valueModified = false;

        foreach (var row in AllRows)
        {
            var allCellsOfInterest = GetIndicesForRow(row);
            var allPairsOfCells = GetIndicesForDistinctPairs(0, row, allCellsOfInterest);

            valueModified = FindTwinsAndEliminateThemFromPotentialValues(allPairsOfCells, allCellsOfInterest, valueModified);
        }
        
        foreach (var column in AllColumns)
        {
            var allCellsOfInterest = GetIndicesForColumn(column);
            var allPairsOfCells = GetIndicesForDistinctPairs(1, column, allCellsOfInterest);
            
            valueModified = FindTwinsAndEliminateThemFromPotentialValues(allPairsOfCells, allCellsOfInterest, valueModified);
        }
        
        foreach (var box in AllBoxes)
        {
            var allCellsOfInterest = GetIndicesForBox(box);
            var allPairsOfCells = GetIndicesForDistinctPairs(2, box, allCellsOfInterest);
            
            valueModified = FindTwinsAndEliminateThemFromPotentialValues(allPairsOfCells, allCellsOfInterest, valueModified);
        }
        
        return valueModified;
    }

    private bool FindTwinsAndEliminateThemFromPotentialValues(List<(Position, Position)> allPairsOfCells,
        List<Position> allCellsOfInterest, bool valueModified)
    {
        var digits = new int[GridSize];

        foreach (var cell in allCellsOfInterest)
        {
            if (IsUndefined(cell))
            {
                foreach (var digit in AllDigits)
                {
                    if (Candidates[cell.Row, cell.Column][digit])
                        digits[digit]++;
                }
            }
        }

        var candidatesForPairs = new BitArray(digits.Select(digit => digits[digit] == 2).ToArray());

        var foundTwins = new List<(Position, Position, BitArray)>();
        
        foreach (var pair in allPairsOfCells)
        {
            BitArray result = new BitArray(GridSize,true);
            var c1 = Candidates[pair.Item1.Row, pair.Item1.Column];
            var c2 = Candidates[pair.Item2.Row, pair.Item2.Column];
            
            result.And(c1).And(c2).And(candidatesForPairs);

            var count = 0;
            for (var index = 0; index < result.Count; index++)
            {
                if (result[index])
                    count++;
            }

            if (count == 2)
            {
                foundTwins.Add((pair.Item1, pair.Item2, result));
            }
        }

        foreach (var twin in foundTwins)
        {
            var filter = twin.Item3.Not();
            
            foreach (var cell in allCellsOfInterest)
            {
                if (IsUndefined(cell) && cell != twin.Item1 && cell != twin.Item2)
                {
                    var old = Candidates[cell.Row, cell.Column];
                    Candidates[cell.Row, cell.Column].And(filter);
                    valueModified = !Candidates[cell.Row, cell.Column].Equals(old);
                }
            }
        }

        return valueModified;
    }
}
