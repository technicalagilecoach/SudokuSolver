using System.Collections;
using System.Diagnostics;
using static SudokuSolverCore.IndicesAndIterators;
using static SudokuSolverCore.Printers;
using static SudokuSolverCore.ValidityChecker;

namespace SudokuSolverCore;

public class Solver(Puzzle puzzle)
{
    private int[,] Cells => puzzle.Cells;
    private BitArray[,] Candidates => puzzle.Candidates; 

    public void Solve()
    {
        bool valueModified;
            
        do
        {
            PropagateValues();
            valueModified = FindUniqueValues();

            if (!valueModified)
                valueModified = FindHiddenUniqueValues();
                
            if (!valueModified)
                valueModified = FindDoublePairs();
            
            //if (!valueModified)
            //    valueModified = FindHiddenDoublePairs();
            
            if (!valueModified)
                valueModified = FindPointingPairs();
            
            if (!IsSolutionCorrect(puzzle.Cells))
                PrintDebugOutput(puzzle);
            
        } while (valueModified);

        if (!Check(puzzle.Cells))
            PrintDebugOutput(puzzle);
    }

    private bool FindUniqueValues()
    {
        var uniqueValues = new UniqueValues(puzzle);
        return uniqueValues.SetUniqueValues();
    }

    private bool FindHiddenUniqueValues()
    {
        var uniqueValues = new UniqueValues(puzzle);
        return uniqueValues.SetHiddenUniqueValues();
    }
        
    private bool FindDoublePairs()
    {
        var doublePairs = new DoublePairs(Cells, Candidates);
        return doublePairs.Handle();
    }
     
    private bool FindHiddenDoublePairs()
    {
        var doublePairs = new HiddenDoublePairs(Cells, Candidates);
        return doublePairs.Handle();
    }
    
    private bool FindPointingPairs()
    {
        var pointingPairs = new PointingPairs(Cells, Candidates);
        return pointingPairs.Handle();
    }
    
    private void PropagateValues()
    {
        ForEachCell(PropagateUsedValuesForOneCell);
    }

    private void PropagateUsedValuesForOneCell(Position position)
    {
        if (puzzle.IsUndefined(position)) 
            return;

        ForEachCellInRowExcept(position.Column, column =>
        {
            RemoveCandidate(position, Candidates[position.Row, column]);
        });
        ForEachCellInColumnExcept(position.Row, row =>
        {
            RemoveCandidate(position, Candidates[row, position.Column]);
        });
        ForEachCellInRegionExcept(position, tuple =>
        {
            RemoveCandidate(position, Candidates[tuple.Row,tuple.Column]);
        });
    }

    private void RemoveCandidate(Position position, BitArray digits)
    {
        var digit = Cells[position.Row, position.Column];  
        digits[digit-1] = false;
    }
}