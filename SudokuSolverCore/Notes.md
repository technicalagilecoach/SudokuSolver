# Notes

## To Do
- make a nicer equality check for BitArrays than AreEqual
- try to benefit more from the C# Enumerable class
- move PotentialValues to a separate class -> grid should only hold the actual values
- unify indices (digits from 1 to 9, row/column/box indices 0 to 8)
- calculate indices only once
- implement more strategies
  - hidden pairs
  - (naked) triples
  - hidden triples

## Done
- handling pairs for boxes
- pointing pairs strategy implemented

## Ideas 
- row, columns and boxes can be treated equally when using a container with all cells/indices of that box
- brute force search is not implemented but could help if all other strategies fail
- implement more strategies mentioned at https://www.sudokuwiki.org/Sudoku.htm

## Limitations
- Grid.Init assumes that the input puzzles always follows the expected syntax -> no error handling otherwise