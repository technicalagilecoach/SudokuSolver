using static SudokuSolver.IndicesAndIterators;
using static SudokuSolver.Puzzle;

namespace SudokuSolver;

public class HiddenTuples(Puzzle puzzle, int tupleSize) : Strategy(puzzle)
{
    private int TupleSize { get; } = tupleSize;
    public bool Handle()
    {
        int numberOfRemovedCandidates = 0;
        
        
        var state = puzzle.PrintCells();
        
        foreach (var row in AllRows)
        {
            var allCellsOfInterest = GetIndicesForRow(row).Where(IsUndefined).ToList();
            numberOfRemovedCandidates = FindHiddenTuplesAndEliminateCandidates(allCellsOfInterest, numberOfRemovedCandidates);
        }
        
        foreach (var column in AllColumns)
        {
            var allCellsOfInterest = GetIndicesForColumn(column).Where(IsUndefined).ToList();;
            numberOfRemovedCandidates = FindHiddenTuplesAndEliminateCandidates(allCellsOfInterest, numberOfRemovedCandidates);
        }
        
        foreach (var box in AllBoxes)
        {
            var allCellsOfInterest = GetIndicesForBox(box).Where(IsUndefined).ToList();;
            numberOfRemovedCandidates = FindHiddenTuplesAndEliminateCandidates(allCellsOfInterest, numberOfRemovedCandidates);
        }
        
        return numberOfRemovedCandidates > 0;
    }

    private int FindHiddenTuplesAndEliminateCandidates(List<Position> allCellsOfInterest, int numberOfRemovedCandidates)
    {
        var state = puzzle.PrintCells();
        
        var digitDistribution = CountDigitDistributionInArea(allCellsOfInterest);
        var relevantDigits = digitDistribution.Where(x=> x<=TupleSize).ToList();

        SortedSet<int> relevantNumbers = [];
        for (var digit = 0; digit < relevantDigits.Count; digit++)
        {
            if (relevantDigits[digit]>0)
                relevantNumbers.Add(digit+1);
        }

        var numberTuples = Combinations<int>(relevantNumbers.ToList(),TupleSize);

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
                    if (candidates[number - 1])
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

        foreach (var tuple in hiddenTuples)
        {
            foreach (var position in tuple.Item1)
            {
                var candidates = GetCandidates(position);

                foreach (var number in AllDigits)
                {
                    if (candidates[number] && !tuple.Item2.Contains(number + 1))
                    {
                        candidates[number] = false;
                        numberOfRemovedCandidates++;
                    }
                }
            }
        }

        return numberOfRemovedCandidates;
    }
}