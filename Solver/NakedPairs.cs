using static SudokuSolver.IndicesAndIterators;
using static SudokuSolver.Puzzle;

namespace SudokuSolver;

public class NakedPairs(Puzzle puzzle) : Strategy(puzzle){
    
    private bool GetCandidate(Position position, int digit)
    {
        return Candidates[position.Row, position.Column][digit];
    }

    private bool SetCandidate(Position position, int digit, bool value)
    {
        return Candidates[position.Row, position.Column][digit] = value;
    }

    private bool[,] _undefinedCells = new bool[GridSize, GridSize];
    private bool[,] _potentialTwins = new bool[GridSize, GridSize];
    
    private List<string> _removedCandidates = [];
    
    public bool Handle()
    {
        var numberOfRemovedCandidates = 0;
        
        MarkUndefinedCells();
        MarkPotentialTwins();
        
        foreach (var row in AllRows)
        {
            var allCellsOfInterest = GetIndicesForRow(row);
            var allPairsOfCells = GetIndicesForDistinctPairs(0, row);

            RemoveCandidatesBasedOnNakedPairs(allPairsOfCells, allCellsOfInterest, ref numberOfRemovedCandidates);
        }
        
        foreach (var column in AllColumns)
        {
            var allCellsOfInterest = GetIndicesForColumn(column);
            var allPairsOfCells = GetIndicesForDistinctPairs(1, column);
            
            RemoveCandidatesBasedOnNakedPairs(allPairsOfCells, allCellsOfInterest, ref numberOfRemovedCandidates);
        }
        
        foreach (var box in AllBoxes)
        {
            var allCellsOfInterest = GetIndicesForBox(box);
            var allPairsOfCells = GetIndicesForDistinctPairs(2, box);
            
            RemoveCandidatesBasedOnNakedPairs(allPairsOfCells, allCellsOfInterest, ref numberOfRemovedCandidates);
        }
        
        return numberOfRemovedCandidates>0;
    }

    private void RemoveCandidatesBasedOnNakedPairs(List<(Position, Position)> allPairsOfCells,
        List<Position> allCellsOfInterest,
        ref int numberOfRemovedCandidates)
    {
        foreach (var pairOfCells in allPairsOfCells)
        {
            if (ArePotentialTwins(pairOfCells) && HaveEqualCandidates(pairOfCells))
            {
                EliminateCandidatesFromOtherCells(allCellsOfInterest, pairOfCells, ref numberOfRemovedCandidates);
                //break;
            }
        }
    }

    private int EliminateCandidatesFromOtherCells(List<Position> allCellsOfInterest, (Position, Position) pairOfCells, ref int numberOfRemovedCandidates)
    {
        foreach (var cell in allCellsOfInterest)
        {
            if (IsUndefined(cell) && IsDisjointFrom(cell,pairOfCells))
            {
                EliminateCandidatesFromCell(cell, pairOfCells.Item1, ref numberOfRemovedCandidates);
            }
        }

        return numberOfRemovedCandidates;
    }

    private void EliminateCandidatesFromCell(Position cell, Position reference, ref int numberOfRemovedCandidates)
    {
        foreach (var digit in AllDigits)
        {
            if (IsUndefined(cell) && GetCandidate(cell, digit) && GetCandidate(reference, digit))
            {
                SetCandidate(cell, digit, false);
                _removedCandidates.Add("("+cell.Row+","+cell.Column+") "+digit);
                numberOfRemovedCandidates++;
            }
        }
    }
    
    public static bool IsDisjointFrom(Position cell, (Position, Position) pairOfCells)
    {
        return cell != pairOfCells.Item1 && cell != pairOfCells.Item2;
    }

    private bool ArePotentialTwins((Position, Position) pair)
    {
        return _potentialTwins[pair.Item1.Row, pair.Item1.Column] && _potentialTwins[pair.Item2.Row, pair.Item2.Column];
    }

    private bool HaveEqualCandidates((Position, Position) pair)
    {
        var cellsAreEqual = true;
            
        foreach (var digit in AllDigits)
        {
            if (GetCandidate(pair.Item1, digit) == GetCandidate(pair.Item2, digit)) 
                continue;
                
            cellsAreEqual = false;
            break;
        }
        
        return cellsAreEqual;
    }
    
    private void MarkPotentialTwins()
    {
        _potentialTwins = new bool[GridSize, GridSize];
        
        ForEachCell(position =>
        {
            if (!_undefinedCells[position.Row, position.Column]) 
                return;
            
            _potentialTwins[position.Row, position.Column] = CountCandidates(position) == 2;
        });
    }

    private void MarkUndefinedCells()
    {
        _undefinedCells = new bool[GridSize, GridSize];
        
        ForEachCell(position =>
        {
            _undefinedCells[position.Row, position.Column] = IsUndefined(position);
        });
    }
}
