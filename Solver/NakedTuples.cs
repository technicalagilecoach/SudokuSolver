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

        List<List<Position>> tuples = Combinations(tupleCandidates,TupleSize);
        

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

    public static List<List<T>> Combinations<T>(List<T> list, int n)
    {
        if (n == 0)
            return [[]];
        if (list.Count == 0)
            return [];

        var head = list[0];
        var tail = list.Count>1?list.GetRange(1,list.Count - 1):[];

        var result = new List<List<T>>();
        
        //head is part of the combination
        var res1 = Combinations(tail, n-1);
        foreach (var item in res1)
        {
            item.Insert(0, head);
            result.Add(item);
        }

        //head is not part of the combination
        var res2 = Combinations(tail, n);
        foreach (var item in res2)
        {
            result.Add(item);
        }
        
        return result;
    }
}