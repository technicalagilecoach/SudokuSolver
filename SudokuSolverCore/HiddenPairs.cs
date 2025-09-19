using System.Collections;
using static SudokuSolverCore.IndicesAndIterators;
using static SudokuSolverCore.Puzzle;

namespace SudokuSolverCore;

internal class HiddenPairs(Puzzle puzzle) : Strategy(puzzle)
{
    private int _numberOfRemovedCandidates = 0;
    
    public bool Handle()
    {
        _numberOfRemovedCandidates = 0;

        foreach (var row in AllRows)
        {
            var allCellsOfInterest = GetIndicesForRow(row);
            var allPairsOfCells = GetIndicesForDistinctPairs(0, row);

            FindTwinsAndEliminateThemFromPotentialValues(allPairsOfCells, allCellsOfInterest);
        }
        
        foreach (var column in AllColumns)
        {
            var allCellsOfInterest = GetIndicesForColumn(column);
            var allPairsOfCells = GetIndicesForDistinctPairs(1, column);
            
            FindTwinsAndEliminateThemFromPotentialValues(allPairsOfCells, allCellsOfInterest);
        }
        
        foreach (var box in AllBoxes)
        {
            var allCellsOfInterest = GetIndicesForBox(box);
            var allPairsOfCells = GetIndicesForDistinctPairs(2, box);
            
            FindTwinsAndEliminateThemFromPotentialValues(allPairsOfCells, allCellsOfInterest);
        }
        
        return _numberOfRemovedCandidates>0;
    }

    private void FindTwinsAndEliminateThemFromPotentialValues(List<(Position, Position)> allPairsOfCells,
        List<Position> allCellsOfInterest)
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

        var foundPairs = new List<(Position, Position, BitArray)>();
        
        foreach (var pair in allPairsOfCells)
        {
            var result = new BitArray(GridSize,true);
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
                foundPairs.Add((pair.Item1, pair.Item2, result));
            }
        }

        foreach (var pair in foundPairs)
        {
            var filter = pair.Item3.Not();
            
            foreach (var cell in allCellsOfInterest)
            {
                if (IsUndefined(cell) && cell != pair.Item1 && cell != pair.Item2)
                {
                    var old = Candidates[cell.Row, cell.Column];
                    Candidates[cell.Row, cell.Column].And(filter); 
                    
                    //ToDo: more than one candidate can be removed here -> all of them should be counted
                    if (!Candidates[cell.Row, cell.Column].Equals(old))
                        _numberOfRemovedCandidates++; 
                }
            }
        }
    }
}
