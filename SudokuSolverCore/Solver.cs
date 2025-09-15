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

    private readonly bool _generateProtocol = false;
    private List<(string,bool)> _executedStrategies = [];
    private List<List<(string,bool)>> _executionProtocol = [];
    private int _round = 0;

    private readonly bool _generateDebugOutput = false;
    private readonly bool _performChecks = false;
    private List<string> _puzzleStates = new List<string>();
    
    
    public bool Solve()
    {
        do
        {
            _valueModified = false;

            if (_generateDebugOutput)
                _puzzleStates.Add(Print(puzzle));
            
            if (_performChecks&&IsInconsistent())
            {
                
            }

            Execute(PruneCandidates);
            Execute(NakedSingles);
            
            //Execute(HiddenSingles);
            //Execute(NakedPairs);
            //Execute(HiddenPairs);
            Execute(PointingPairs);
    
            UpdateProtocol();
        } while (_valueModified);

        var isCorrect = Check(Cells);
        
        if (_generateDebugOutput&&!isCorrect)
            PrintDebugOutput(puzzle);
        
        return isCorrect;
    }

    private void UpdateProtocol()
    {
        if (!_generateProtocol) return;
        
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
            var before = false;
            var gotWorse = false;
            
            if (_generateProtocol)
                before = IsInconsistent();
            
            _valueModified = fun();

            if (_generateProtocol)
            {
                var after  = IsInconsistent();
                gotWorse = (!before) && after;
                if (gotWorse)
                    PrintDebugOutput(puzzle);
            }

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