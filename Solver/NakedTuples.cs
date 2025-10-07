using static SudokuSolver.IndicesAndIterators;
using static SudokuSolver.Puzzle;

namespace SudokuSolver;

public class NakedTuples(Puzzle puzzle, int tupleSize) : Strategy(puzzle)
{
    private int TupleSize { get; } = tupleSize;
    public bool Handle()
    {
        int numberOfRemovedCandidates = 0;
        
        foreach (var row in AllRows)
        {
            var allCellsOfInterest = GetIndicesForRow(row).Where(IsUndefined).ToList();
            FindNakedTuplesAndRemoveCandidates(allCellsOfInterest, ref numberOfRemovedCandidates);
        }
        
        foreach (var column in AllColumns)
        {
            var allCellsOfInterest = GetIndicesForColumn(column).Where(IsUndefined).ToList();;
            FindNakedTuplesAndRemoveCandidates(allCellsOfInterest, ref numberOfRemovedCandidates);
        }
        
        foreach (var box in AllBoxes)
        {
            var allCellsOfInterest = GetIndicesForBox(box).Where(IsUndefined).ToList();;
            FindNakedTuplesAndRemoveCandidates(allCellsOfInterest, ref numberOfRemovedCandidates);
        }
        
        return numberOfRemovedCandidates > 0;
    }

    private void FindNakedTuplesAndRemoveCandidates(List<Position> allCellsOfInterest, ref int numberOfRemovedCandidates)
    {
        var tuples = GeneratePotentialNakedTuples(allCellsOfInterest);
        var nakedTuples = FindNakedTuples(tuples);

        RemoveCandidatesBasedOnNakedTuples(allCellsOfInterest, nakedTuples, ref numberOfRemovedCandidates);
    }

    private List<List<Position>> GeneratePotentialNakedTuples(List<Position> allCellsOfInterest)
    {
        var tupleCandidates = allCellsOfInterest.Where(x => CountCandidates(x)<=TupleSize).ToList();
        
        if (tupleCandidates.Count < TupleSize)
            return [];

        return Combinations(tupleCandidates, TupleSize);
    }

    private List<Tuple<List<Position>, SortedSet<int>>> FindNakedTuples(List<List<Position>> tuples)
    {
        List<Tuple<List<Position>,SortedSet<int>>> nakedTuples = [];
        foreach (var tuple in tuples)
        {
            SortedSet<int> candidateDigits = [];

            for (int i=0; i<tuple.Count(); i++)
                CollectDigits(candidateDigits, tuple[i]);
                
            if (candidateDigits.Count == TupleSize)
                nakedTuples.Add(new Tuple<List<Position>, SortedSet<int>>(tuple,candidateDigits));
                
            void CollectDigits(SortedSet<int> sortedSet, Position position)
            {
                var candidates = GetCandidates(position);
                foreach (var digit in AllDigits)
                {
                    if (candidates[digit-1])
                        sortedSet.Add(digit); 
                }
            }
        }

        return nakedTuples;
    }

    private void RemoveCandidatesBasedOnNakedTuples(List<Position> allCellsOfInterest,
        List<Tuple<List<Position>, SortedSet<int>>> nakedTuples, ref int numberOfRemovedCandidates)
    {
        foreach (var tuple in nakedTuples)
        {
            var numbers = tuple.Item2;

            foreach (var cell in allCellsOfInterest)
            {
                if (!tuple.Item1.Contains(cell))
                {
                    RemoveCandidates(cell, numbers.ToList(), ref numberOfRemovedCandidates);
                }
            }
        }
    }
}