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
            numberOfRemovedCandidates = FindNakedTuplesAndEliminateCandidates(allCellsOfInterest, numberOfRemovedCandidates);
        }
        
        foreach (var column in AllColumns)
        {
            var allCellsOfInterest = GetIndicesForColumn(column).Where(IsUndefined).ToList();;
            numberOfRemovedCandidates = FindNakedTuplesAndEliminateCandidates(allCellsOfInterest, numberOfRemovedCandidates);
        }
        
        foreach (var box in AllBoxes)
        {
            var allCellsOfInterest = GetIndicesForBox(box).Where(IsUndefined).ToList();;
            numberOfRemovedCandidates = FindNakedTuplesAndEliminateCandidates(allCellsOfInterest, numberOfRemovedCandidates);
        }
        
        return numberOfRemovedCandidates > 0;
    }

    private int FindNakedTuplesAndEliminateCandidates(List<Position> allCellsOfInterest, int numberOfRemovedCandidates)
    {
        var tupleCandidates = allCellsOfInterest.Where(x => CountCandidates(x)<=TupleSize).ToList();

        if (tupleCandidates.Count() < TupleSize)
            return numberOfRemovedCandidates;

        List<List<Position>> tuples = [];

        
        GenerateNTuples(tupleCandidates, tuples);

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
                    if (candidates[digit])
                        sortedSet.Add(digit); 
                }
            }
        }

        foreach (var tuple in nakedTuples)
        {
            var numbers = tuple.Item2;

            foreach (var cell in allCellsOfInterest)
            {
                if (!tuple.Item1.Contains(cell))
                {
                    var candidates = Candidates[cell.Row, cell.Column];
                    foreach (var digit in numbers)
                    {
                        if (candidates[digit])
                        {
                            candidates[digit] = false;
                            numberOfRemovedCandidates++;
                        }
                    }
                }
            }
        }

        return numberOfRemovedCandidates;
    }

    private void GenerateNTuples(List<Position> tupleCandidates, List<List<Position>> tuples)
    {
        if (TupleSize == 2)
        {
            int size = tupleCandidates.Count();
            for (int i = 0; i < size - 1; i++)
            {
                for (int j = i + 1; j < size; j++)
                {
                    tuples.Add(new List<Position> { tupleCandidates[i], tupleCandidates[j]});
                }
            }
        }
        else if (TupleSize == 3)
        {
            int size = tupleCandidates.Count();
            for (int i = 0; i < size - 2; i++)
            {
                for (int j = i + 1; j < size - 1; j++)
                {
                    for (int k = j + 1; k < size; k++)
                    {
                        tuples.Add(new List<Position> { tupleCandidates[i], tupleCandidates[j], tupleCandidates[k] });
                    }
                }
            }
        }
        else if (TupleSize == 4)
        {
            int size = tupleCandidates.Count();
            for (int i = 0; i < size - 3; i++)
            {
                for (int j = i + 1; j < size - 2; j++)
                {
                    for (int k = j + 1; k < size -1; k++)
                    {
                        for (int l = k + 1; l < size; l++)
                        {
                            tuples.Add(new List<Position> { tupleCandidates[i], tupleCandidates[j], tupleCandidates[k], tupleCandidates[l] });
                        }
                    }
                }
            }
        }
    }
}