using System.Collections;
using static SudokuSolverCore.IndicesAndIterators;
using static SudokuSolverCore.Printers;
using static SudokuSolverCore.ValidityChecker;

namespace SudokuSolverCore;

public class Solver(Puzzle puzzle)
{
    private int[,] Cells => puzzle.Cells;
    private BitArray[,] Candidates => puzzle.Candidates;
    
    private bool _valueModified = false;

    private List<(string,bool)> _executedStrategies = [];
    private List<List<(string,bool)>> _executionProtocol = [];
    private readonly bool _performChecks = true;
    private int _round = 0;
    
    public void Solve()
    {
        do
        {
            _valueModified = false;
            
            PruneCandidates();
            
            Execute(NakedSingles);
            Execute(HiddenSingles);
            Execute(NakedPairs);
            //Execute(HiddenPairs);
            Execute(PointingPairs);
    
            UpdateProtocol();
        } while (_valueModified);

        if (!Check(puzzle.Cells))
            PrintDebugOutput(puzzle);
    }

    private void UpdateProtocol()
    {
        _executionProtocol.Add(_executedStrategies);
        _executedStrategies = [];
        _round++;
    }

    private void CheckConsistency()
    {
        if (IsInConsistent())
            PrintDebugOutput(puzzle);
    }

    private bool IsInConsistent()
    {
        return _performChecks && !IsSolutionCorrect(puzzle.Cells);
    }

    public void Execute(Func<bool> fun)
    {
        if (!_valueModified)
        {
            var before = IsInConsistent();
            _valueModified = fun();
            var after = IsInConsistent();
            var gotWorse = (!before)&&after;
            if (gotWorse)
                PrintDebugOutput(puzzle);
            _executedStrategies.Add((fun.Method.Name, gotWorse));
        }
    }
    
    private bool NakedSingles()
    {
        return new Singles(puzzle).HandleNakedSingles();
    }

    private bool HiddenSingles()
    {
        return new Singles(puzzle).HandleHiddenSingles();
    }
        
    private bool NakedPairs()
    {
        return new NakedPairs(puzzle).Handle();
    }
     
    private bool HiddenPairs()
    {
        return new HiddenPairs(puzzle).Handle();
    }
    
    private bool PointingPairs()
    {
        return new PointingPairs(puzzle).Handle();
    }
    
    private void PruneCandidates()
    {
        ForEachCell(RemoveCandidatesBasedOnSingle);
    }

    private void RemoveCandidatesBasedOnSingle(Position position)
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