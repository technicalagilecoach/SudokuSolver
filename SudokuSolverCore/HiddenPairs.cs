using System.Collections;
using static SudokuSolverCore.IndicesAndIterators;
using static SudokuSolverCore.Puzzle;

namespace SudokuSolverCore;

internal class HiddenPairs(Puzzle puzzle) : Strategy(puzzle)
{
    public bool Handle()
    {
        var numberOfRemovedCandidates = 0;

        foreach (var row in AllRows)
        {
            var allCellsOfInterest = GetIndicesForRow(row);
            var allPairsOfCells = GetIndicesForDistinctPairs(0, row);

            FindTwinsAndEliminateThemFromPotentialValues(allPairsOfCells, allCellsOfInterest, ref numberOfRemovedCandidates);
        }
        
        foreach (var column in AllColumns)
        {
            var allCellsOfInterest = GetIndicesForColumn(column);
            var allPairsOfCells = GetIndicesForDistinctPairs(1, column);
            
            FindTwinsAndEliminateThemFromPotentialValues(allPairsOfCells, allCellsOfInterest, ref numberOfRemovedCandidates);
        }
        
        foreach (var box in AllBoxes)
        {
            var allCellsOfInterest = GetIndicesForBox(box);
            var allPairsOfCells = GetIndicesForDistinctPairs(2, box);
            
            FindTwinsAndEliminateThemFromPotentialValues(allPairsOfCells, allCellsOfInterest, ref numberOfRemovedCandidates);
        }
        
        return numberOfRemovedCandidates>0;
    }

    private void FindTwinsAndEliminateThemFromPotentialValues(List<(Position, Position)> allPairsOfCells,
        List<Position> allCellsOfInterest, ref int numberOfRemovedCandidates)
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
                    
                    //ToDo: more than one candidate can be removed here -> all of them should be counted
                    if (!Candidates[cell.Row, cell.Column].Equals(old))
                        numberOfRemovedCandidates++; 
                }
            }
        }
    }
}
