# Notes

## Bugs
- precalculating indices does not work in GetIndicesForDistinctPairs, see GetIndicesForDistinctPairsTest

## To Do
- clean up/refactor 
  - BoxLineReduction
  - PointingPairs
  - HiddenPairs
- repair
  - HiddenPairs
- which basic strategies from  https://www.sudokuwiki.org/Sudoku.htm have not been implemented yet? 
- implement more basic strategies
  - naked triples
  - hidden triples
  - naked quads
  - hidden quads
- make solver usable for end users - command line tool
- improve statistics, error checking and debug support
  - should be helpful
  - can be activated on demand (impact on solver performance)
  - tradeoff with different diagnostic levels
- clean up code to read in sets of puzzles
- extract duplicate code into common helper functions
- reduce number of method parameters by using more instance variables
- check if existing strategies work as intended/identify test puzzles where the strategy is needed
- perform pruning etc. only based on actually changed cells
- make a nicer equality check for BitArrays than AreEqual
- try to benefit more from the C# Enumerable class

## Done
- change strategies such that they count the number of newly fixed values/removed candidates
- implemented the box-line-reduction strategy
- calculate indices only once -> as expected this has a big impact on performance as proven with profiling
- improved check for modification in NakedSingles by counting how many values are set
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