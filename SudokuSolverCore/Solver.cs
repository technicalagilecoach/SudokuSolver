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
            
            Execute(PruneCandidates);
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
        if (IsInconsistent())
            PrintDebugOutput(puzzle);
    }

    private bool IsInconsistent()
    {
        return _performChecks && !IsSolutionCorrect(puzzle.Cells);
    }

    public void Execute(Func<bool> fun)
    {
        if (!_valueModified)
        {
            var before = IsInconsistent();
            _valueModified = fun();
            var after = IsInconsistent();
            var gotWorse = (!before)&&after;
            if (gotWorse)
                PrintDebugOutput(puzzle);
            _executedStrategies.Add((fun.Method.Name, gotWorse));
        }
    }
    
    private bool PruneCandidates()
    {
        return new PruneCandidates(puzzle).Handle();
    }
    
    private bool NakedSingles()
    {
        return new NakedSingles(puzzle).Handle();
    }

    private bool HiddenSingles()
    {
        return new HiddenSingles(puzzle).Handle();
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
}