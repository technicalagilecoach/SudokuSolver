using static SudokuSolver.IndicesAndIterators;
using static SudokuSolver.Puzzle;

namespace SudokuSolver;

public class HiddenTuples(Puzzle puzzle, int tupleSize) : Strategy(puzzle: puzzle)
{
    private int _numberOfRemovedCandidates;
    
    private int TupleSize { get; } = tupleSize;
    public bool Handle()
    {
        _numberOfRemovedCandidates = 0;
        
        foreach (var row in AllRows)
        {
            var allCellsOfInterest = GetIndicesForRow(row).Where(IsUndefined).ToList();
            FindHiddenTuplesAndEliminateCandidates(allCellsOfInterest);
        }
        
        foreach (var column in AllColumns)
        {
            var allCellsOfInterest = GetIndicesForColumn(column).Where(IsUndefined).ToList();;
            FindHiddenTuplesAndEliminateCandidates(allCellsOfInterest);
        }
        
        foreach (var box in AllBoxes)
        {
            var allCellsOfInterest = GetIndicesForBox(box).Where(IsUndefined).ToList();;
            FindHiddenTuplesAndEliminateCandidates(allCellsOfInterest);
        }
        
        return _numberOfRemovedCandidates > 0;
    }

    private void FindHiddenTuplesAndEliminateCandidates(List<Position> allCellsOfInterest)
    {
        var numberTuples = GeneratePotentialNumberTuples(allCellsOfInterest);
        var hiddenTuples = IdentifyHiddenTuples(allCellsOfInterest, numberTuples);
        RemoveCandidatesBasedOnHiddenTuples(hiddenTuples);
    }

    private List<List<int>> GeneratePotentialNumberTuples(List<Position> allCellsOfInterest)
    {
        var numberDistribution = CountDigitDistributionInArea(allCellsOfInterest).ToList();

        SortedSet<int> relevantNumbers = [];
        foreach (var number in AllDigits)
        {
            var x = numberDistribution[number-1];
            if (x>0 && x<=TupleSize)
                relevantNumbers.Add(number);
        }

        var numberTuples = Combinations<int>(relevantNumbers.ToList(),TupleSize);
        return numberTuples;
    }

    private void RemoveCandidatesBasedOnHiddenTuples(List<(List<Position>, List<int>)> hiddenTuples)
    {
        foreach (var tuple in hiddenTuples)
        {
            foreach (var position in tuple.Item1)
            {
                var numbersToRemove = AllDigits.Where(x => !tuple.Item2.Contains(x)).ToList();
                RemoveCandidates(position, numbersToRemove, ref _numberOfRemovedCandidates);
            }
        }
    }

    private List<(List<Position>, List<int>)> IdentifyHiddenTuples(List<Position> allCellsOfInterest, List<List<int>> numberTuples)
    {
        var hiddenTuples = new List<(List<Position>,List<int>)>();

        foreach (var tuple in numberTuples)
        {
            List<Position> potentialCellsOfTuple = [];
            
            foreach (var position in allCellsOfInterest)
            {
                int count = 0;
                var candidates = GetCandidates(position);

                foreach (var number in tuple)
                {
                    if (candidates[number-1])
                        count++;
                }
                
                if (count>=2)
                    potentialCellsOfTuple.Add(position);
            }

            if (potentialCellsOfTuple.Count == TupleSize)
            {
                bool hiddenTuple = true;
                foreach (var cell in allCellsOfInterest)
                {
                    if (!potentialCellsOfTuple.Contains(cell))
                    {
                        var candidates = GetCandidates(cell);
                        foreach (var number in tuple)
                        {
                            if (candidates[number - 1])
                            {
                                hiddenTuple = false;
                                break;
                            }
                        }
                    }
                }

                if (hiddenTuple)
                    hiddenTuples.Add((potentialCellsOfTuple, tuple));
            }
        }

        return hiddenTuples;
    }
}