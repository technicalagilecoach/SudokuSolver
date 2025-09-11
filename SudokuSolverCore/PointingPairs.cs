using static SudokuSolverCore.IndicesAndIterators;
using static SudokuSolverCore.Puzzle;

namespace SudokuSolverCore;

public class PointingPairs(Puzzle puzzle) : Strategy(puzzle) {
    public bool Handle()
    {
        var valueModified = false;

        foreach (var region in AllRegions)
        {
            var positions = GetIndicesForRegion(region);
            
            HandlePointingPairsInRows(positions, region, ref valueModified);
            HandlePointingPairsInColumns(positions, region, ref valueModified);
        }

        return valueModified;
    }

    private void HandlePointingPairsInRows(List<Position> positions, int region, ref bool valueModified)
    {
        var valuesRows = new int[RegionSize,GridSize];

        var encounteredColumns = new HashSet<int>();
            
        for (var rIndex = 0; rIndex < GridSize; rIndex++)
        {
            var index = rIndex;
            var rowPositions = positions.Where(position =>  position.Row % 3 == index );
        
            foreach (var position in rowPositions)
            {
                if (Cells[position.Row, position.Column] == Undefined)
                {
                    encounteredColumns.Add(position.Column);
                        
                    var value = Candidates[position.Row, position.Column];
                    foreach (var digit in AllDigits)
                    {
                        if (value[digit])
                            valuesRows[position.Row % 3,digit]++;
                    }
                }
            }
        }

        var pointingPairsInRows = new List<(int, int)>();
        foreach (var digit in AllDigits)
        { 
            var result = OnlyOnePositive(valuesRows[0,digit], valuesRows[1,digit], valuesRows[2,digit]);
            if (result)
            {
                var positiveIndex = valuesRows[0, digit] > 0 ? 0 : (valuesRows[1, digit] > 0 ? 1 : 2);
                var pos = GetRegionCoordinates(region);
                pointingPairsInRows.Add((digit, pos.Row+positiveIndex));                    
            }
        }

        foreach (var (digit, row) in pointingPairsInRows)
        {
            foreach (var column in AllColumns)
            {
                if (Cells[row, column] == Undefined && !encounteredColumns.Contains(column) && Candidates[row,column][digit])
                {
                    Candidates[row,column][digit] = false;
                    valueModified = true;
                }
            }
        }
    }

    private void HandlePointingPairsInColumns(List<Position> positions, int region, ref bool valueModified)
    {
        var valuesColumns = new int[RegionSize,GridSize];
            
        var encounteredRows = new HashSet<int>();
            
        for (var cIndex = 0; cIndex < GridSize; cIndex++)
        {
            var index = cIndex;
            var columnPositions = positions.Where(position =>  position.Column % 3 == index );
        
            foreach (var position in columnPositions)
            {
                if (Cells[position.Row, position.Column] == Undefined)
                {
                    encounteredRows.Add(position.Row);
                    
                    var value = Candidates[position.Row, position.Column];
                    foreach (var digit in AllDigits)
                    {
                        if (value[digit])
                            valuesColumns[position.Column % 3,digit]++;
                    }
                }
            }
        }
            
        var pointingPairsInColumns = new List<(int, int)>();
        foreach (var digit in AllDigits)
        { 
            var result = OnlyOnePositive(valuesColumns[0,digit], valuesColumns[1,digit], valuesColumns[2,digit]);
            if (result)
            {
                var positiveIndex = valuesColumns[0, digit] > 0 ? 0 : (valuesColumns[1, digit] > 0 ? 1 : 2);
                var pos = GetRegionCoordinates(region);
                pointingPairsInColumns.Add((digit, pos.Column+positiveIndex));                    
            }
        }
            
        foreach (var (digit, column) in pointingPairsInColumns)
        {
            foreach (var row in AllRows)
            {
                if (Cells[row, column] == Undefined && !encounteredRows.Contains(row) && Candidates[row,column][digit])
                {
                    Candidates[row,column][digit] = false;
                    valueModified = true;
                }
            }
        }
    }

    private static bool OnlyOnePositive(int a, int b, int c)
    {
        int positive = 0;

        if (a > 0)
            positive++;
        if (b > 0)
            positive++;
        if (c > 0)
            positive++;
        
        return positive==1;
    }
}