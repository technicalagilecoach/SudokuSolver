using System.Collections;
using static SudokuSolver.IndicesAndIterators;
using static SudokuSolver.Puzzle;

namespace SudokuSolver;

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

            FindHiddenPairsAndEliminateCandidates(allPairsOfCells, allCellsOfInterest);
        }
        
        foreach (var column in AllColumns)
        {
            var allCellsOfInterest = GetIndicesForColumn(column);
            var allPairsOfCells = GetIndicesForDistinctPairs(1, column);
            
            FindHiddenPairsAndEliminateCandidates(allPairsOfCells, allCellsOfInterest);
        }
        
        foreach (var box in AllBoxes)
        {
            var allCellsOfInterest = GetIndicesForBox(box);
            var allPairsOfCells = GetIndicesForDistinctPairs(2, box);
            
            FindHiddenPairsAndEliminateCandidates(allPairsOfCells, allCellsOfInterest);
        }
        
        return _numberOfRemovedCandidates>0;
    }

    private void FindHiddenPairsAndEliminateCandidates(List<(Position, Position)> allPairsOfCells,
        List<Position> allCellsOfInterest)
    {
        var distribution = CountDigitDistributionInArea(allCellsOfInterest);
        
        var candidatesForPairs = new BitArray(GridSize);
        var count = 0;
        for (var digit = 0; digit < GridSize; digit++)
        {
            if (distribution[digit] == 2)
            {
                candidatesForPairs[digit] = true;
                count++;
            }
        }
        
        if (count < 2)
            return;

        if (count > 2)
            return; // To Do (more than one hidden pair in area)

        var hiddenPair = new List<Position>();
        foreach (var cell in allCellsOfInterest)
        {
            if (HasMatchingCandidates(cell,candidatesForPairs))
                hiddenPair.Add(cell);
        }

        if (hiddenPair.Count != 2)
            return;
        
        var foundPair = new List<(Position, Position, BitArray)>();
        
        foundPair.Add((hiddenPair[0], hiddenPair[1], candidatesForPairs));
        
        RemoveCandidates(allCellsOfInterest, foundPair);
    }

    private bool HasMatchingCandidates(Position cell, BitArray candidates) 
    {
        var cellCandidates = Candidates[cell.Row, cell.Column];
        
        var cellsAreEqual = true;
        
        foreach (var digit in AllDigits)
        {
            if (!candidates[digit])
                continue;
            
            if (GetCandidates(cell)[digit] == candidates[digit]) 
                continue;
                
            cellsAreEqual = false;
            break;
        }
        
        return cellsAreEqual;
    }
    
    private void RemoveCandidates(List<Position> allCellsOfInterest, List<(Position, Position, BitArray)> foundPairs)
    {
        foreach (var pair in foundPairs)
        {
            var filter = pair.Item3; //.Not();
            
            foreach (var cell in allCellsOfInterest)
            {
                if (IsUndefined(cell) && (cell == pair.Item1 || cell == pair.Item2))
                {
                    var old = (BitArray)Candidates[cell.Row, cell.Column].Clone();
                    Candidates[cell.Row, cell.Column].And(filter); 
                    
                    //ToDo: more than one candidate can be removed here -> all of them should be counted
                    if (!HasMatchingCandidates(cell, old))
                        _numberOfRemovedCandidates++; 
                }
            }
        }
    }
}
