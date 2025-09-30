# Notes

## To Do
- finish implementation of basic strategies
  - extend pointing pairs to triplets
    - PointingPairs strategy already covers triplets -> rename it to PointingTuples
    - requires further refactoring
  - check if all elements of the basic strategies from https://www.sudokuwiki.org/Sudoku.htm are covered
  - identify relevant puzzles for each strategy and improve tests
  - clean up/refactor 
    - naked tuples
    - hidden tuples
    - BoxLineReduction
    - PointingPairs
- improve statistics, error checking and debug support
  - should be helpful -> locate the problematic code as precisely as possible
  - detect when puzzle can not be solved -> none of the strategies helps
  - detect errors -> puzzle got into an inconsistent state
  - statistics can be activated on demand (impact on solver performance)
  - detailed diagnostics can be activated on demand (impact on solver performance)
  - clean up code for printing solutions in DebugOutput -> currently not used
- make project ready to publish
    - find a name for the project
    - decide on a license before publishing on GitHub
    - polish the code to make the GitHub-repo public
    - remove data from git retrospectively, e.g. puzzle files, this notes, ...

## Done
- removed deprecated code for naked and hidden pairs
- extend BoxLineReduction to triplets -> code not based on pairs or triplets
- implement remaining basic strategies (identify relevant puzzles for each)
  - implemented generic solver strategy for naked tuple (used for pairs, triplets and quadruplets)
  - implemented generic solver strategy for hidden tuples (used for pairs, triplets and quadruplets)
  - replaced NakedPairs by generic NakedTuples version
  - replaced previous GenerateNTuple by generic Combinations implementations
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
- which strategies are needed might also depend on the order in which the strategies are executed!
- ideas for code improvement
  - consider using ImmutableList where it is feasible
  - use Enumerable more instead of expanded lists
  - use typealiases to make code more readable
  - in InputSolver.RunSolver all puzzles are run through the solver before any output is written
    - consider calling solver and writing output puzzle after puzzle
  - parallel execution when many puzzles are processed
  - perform pruning etc. only based on actually changed cells

## Limitations
- Grid.Init assumes that the input puzzles always follows the expected syntax -> no error handling otherwise