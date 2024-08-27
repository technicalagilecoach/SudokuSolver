# A simple Sudoku solver
The initial commit presents a quick and dirty implementation of a solver for Sudoku puzzles.

The solver is capable of solving simple Sudoku puzzles, where during the solution process there is always at least one unsolved field which has a unique solution. It cannot solve hard puzzles, for which one has to choose between two potential values during the solution process.

The code was written without TDD and without having clean code principles in mind. Only some minor refactorings (extract method and rename symbol) where used to simplify debugging. The goal was to solve the first puzzle as soon as possible. No attention was given to readability or maintainability (or even performance) of the code. Version control was not used.

At the moment, changing the solver in order to handle hard puzzles as well, seems like a daunting task. I've reached the point where the code got hard to change quite quickly. And I've already spent almost half of the time to debug stupid mistakes (mostly caused by confusing index variables when using copy and paste).

- Will I be able to make the code easy to change by rigorously refactoring the code?
- Could this situation have been avoided by using strict TDD from the start?
