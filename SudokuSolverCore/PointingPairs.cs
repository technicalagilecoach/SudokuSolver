using System.Collections;
using static SudokuSolverCore.IndicesAndIterators;
using static SudokuSolverCore.Puzzle;

namespace SudokuSolverCore;

public class PointingPairs(int[,] cells, BitArray[,] candidates)
{
    public bool Handle()
    {
        var valueModified = false;

        foreach (var region in AllRegions)
        {
            var positions = GetIndicesForRegion(region);
            
            //rows
            valueModified = HandlePointingPairsInRows(positions, region, valueModified);

            //columns
            valueModified = HandlePointingPairsInColumns(positions, region, valueModified);
        }

        return valueModified;
    }

    private bool HandlePointingPairsInRows(List<Position> positions, int region, bool valueModified)
    {
        var valuesRows = new int[RegionSize,GridSize];

        var encounteredColumns = new HashSet<int>();
            
        for (int rIndex = 0; rIndex < GridSize; rIndex++)
        {
            var rowPositions = positions.Where(position =>  position.Row % 3 == rIndex );
        
            foreach (var position in rowPositions)
            {
                if (cells[position.Row, position.Column] == Undefined)
                {
                    encounteredColumns.Add(position.Column);
                        
                    var value = candidates[position.Row, position.Column];
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
            var result = TernaryXOR(valuesRows[0,digit], valuesRows[1,digit], valuesRows[2,digit]);
            if (result)
            {
                var positiveIndex = valuesRows[0, digit] > 0 ? 0 : (valuesRows[1, digit] > 0 ? 1 : 2);
                Position pos = GetRegionCoordinates(region);
                pointingPairsInRows.Add((digit, pos.Row+positiveIndex));                    
            }
        }

        foreach (var ele in pointingPairsInRows)
        {
            var row = ele.Item2;
            foreach (var column in AllColumns)
            {
                if (cells[row, column] == Undefined && !encounteredColumns.Contains(column) && candidates[row,column][ele.Item1])
                {
                    candidates[row,column][ele.Item1] = false;
                    valueModified = true;
                }
            }
        }

        return valueModified;
    }

    private bool HandlePointingPairsInColumns(List<Position> positions, int region, bool valueModified)
    {
        var valuesColumns = new int[RegionSize,GridSize];
            
        var encounteredRows = new HashSet<int>();
            
        for (int cIndex = 0; cIndex < GridSize; cIndex++)
        {
            var columnPositions = positions.Where(position =>  position.Column % 3 == cIndex );
        
            foreach (var position in columnPositions)
            {
                if (cells[position.Row, position.Column] == Undefined)
                {
                    encounteredRows.Add(position.Row);
                    
                    var value = candidates[position.Row, position.Column];
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
            var result = TernaryXOR(valuesColumns[0,digit], valuesColumns[1,digit], valuesColumns[2,digit]);
            if (result)
            {
                var positiveIndex = valuesColumns[0, digit] > 0 ? 0 : (valuesColumns[1, digit] > 0 ? 1 : 2);
                Position pos = GetRegionCoordinates(region);
                pointingPairsInColumns.Add((digit, pos.Column+positiveIndex));                    
            }
        }
            
        foreach (var ele in pointingPairsInColumns)
        {
            var column = ele.Item2;
            foreach (var row in AllRows)
            {
                if (cells[row, column] == Undefined && !encounteredRows.Contains(row) && candidates[row,column][ele.Item1])
                {
                    candidates[row,column][ele.Item1] = false;
                    valueModified = true;
                }
            }
        }

        return valueModified;
    }

    private static bool TernaryXOR(int a, int b, int c)
    {
        return TernaryXOR(a>0,b>0,c>0);
    }

    public static bool TernaryXOR(bool a, bool b, bool c)
    { 
        return (!a&&!b&&c)||(!a&&b&&!c)||(a&&!b&&!c);
    }
}