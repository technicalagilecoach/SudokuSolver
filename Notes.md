# Notes

## Bugs

## To Do
- make project ready to publish
  - find a name for the project
  - decide on a license before publishing on GitHub
  - polish the code to make the GitHub-repo public
  - remove data from git retrospectively, e.g. puzzle files, this notes, ...
- use typealiases to make code mroe readable
- HiddenPairs
  - test/repair and refactor
  - exclude naked pairs!
  - It needs to be extended such that more than one hidden pair may occur in one particular area.
  - more example puzzles needed
- clean up/refactor 
  - BoxLineReduction
  - PointingPairs
- replace GenerateNTuple by generic implementations
- replace NakedPairs by generic NakedTuples version
- extend BoxLineReduction to triplets
- extend PointingPairs to triplets
- implement remaining basic strategies (identify relevant puzzles for each)
  - can the code for naked pairs/hidden pairs be generalized?
  - can the code for triples/quads be specialized that it also works for pairs?
  - naked triples
  - hidden triples
  - naked quads
  - hidden quads
- improve statistics, error checking and debug support
  - which strategies are needed might also depend on the order in which the strategies are executed!
  - clean up code for printing solutions in DebugOutput -> currently not used
  - should be helpful
  - can be activated on demand (impact on solver performance)
  - tradeoff with different diagnostic levels
- in InputSolver.RunSolver all puzzles are run through the solver before any output is written 
  - consider calling solver and writing output puzzle after puzzle
- parallel execution when many puzzles are processed
- perform pruning etc. only based on actually changed cells

## Done
- implemented messy but generic code for handling naked tuples 
  - works for pairs, triplets and most likely also for quadruplets (no test case for quadruplets yet) 
- improved Hidden Pairs strategy such that it performs as expected with on example puzzle.
- implement statistics to find out which strategies are actually needed for solving a specific puzzle
- clean up code for calling the solver in SolverWrapper.cs
- clean up code in Writer.cs
- clean up code for reading in sets of puzzles in Input.cs
- move Writer.SolvePuzzles somewhere else -> SolverWrapper
- clean up code in InputCommands
- tidy up PuzzlesFromFilesTests? -> remove (file base tests are a mess, could be tested with CLI instead)
- command line switch to save only the unsolved puzzles
- preserve the puzzle names from input files of type MultiplePuzzlesWithName when storing the results
- use the same symbol for unsolved cells as in the input files when writing the results
  - currently done by replacing the characters afterward -> inefficient
  - Puzzle.Init -> no need to change, internally 0 is used to denote undefined cells
  - Puzzle.PrintCells -> additional parameter undefinedSymbol
- highlight the original fixed cells in red for SinglePuzzle mode with console output
- store output files in the same format as the input files
- distinguish different input file types
- make solver usable for end users - command line tool
- move code to read puzzles/write solutions away from Tests project
- move puzzles, notes, ... to toplevel folder outside the projects
- error handling for command line version (output file already exists, directory not writeable, ...)
- BUG precalculating indices does not work in GetIndicesForDistinctPairs, see GetIndicesForDistinctPairsTest
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