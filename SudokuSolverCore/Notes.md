# Notes

## To Do
- extract duplicate code into common helper functions
- check if existing strategies work as intended/identify test puzzles where the strategy is needed
- implement more strategies
  - hidden pairs
  - (naked) triples
  - hidden triples
- calculate indices only once
- perform pruning etc. only based on actually changed cells
- make a nicer equality check for BitArrays than AreEqual
- try to benefit more from the C# Enumerable class

## Done
- extract common base class for different strategies
- unify indices (digits from 1 to 9, row/column/box indices 0 to 8)
- move PotentialValues to a separate class
  - PotentialValues have been renamed to candidates
  - grid should only hold the actual values
    - Cell class was removed
    - cells and candidates are now different members of the Puzzle class 
- handling pairs for boxes
- pointing pairs strategy implemented

## Ideas 
- row, columns and boxes can be treated equally when using a container with all cells/indices of that box
- brute force search is not implemented but could help if all other strategies fail
- implement more strategies mentioned at https://www.sudokuwiki.org/Sudoku.htm

## Limitations
- Grid.Init assumes that the input puzzles always follows the expected syntax -> no error handling otherwise