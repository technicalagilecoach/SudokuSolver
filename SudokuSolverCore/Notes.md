# Notes

## To Do
- handling hidden pairs is not implemented
- handling (naked) triples is not implemented
- handling hidden triples is not implemented
- make a nicer equality check for BitArrays than AreEqual
- try to benefit more from the C# Enumerable class
- move PotentialValues to a separate class -> grid should only hold the actual values

## Done
- handling pairs for regions

## Ideas 
- row, columns and regions can be treated equally when using a container with all cells/indices of that region
- brute force search is not implemented but could help if all other strategies fail
- implement more strategies mentioned at https://www.sudokuwiki.org/Sudoku.htm

## Limitations
- Grid.Init assumes that the input puzzles always follows the expected syntax -> no error handling otherwise