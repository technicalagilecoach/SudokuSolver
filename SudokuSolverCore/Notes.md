# Notes
- run propagation between different solver strategies? -> already done, if valueModified==true the 
 remainder of the Solve() loop is skipped and the next iteration starts with propagation 
- Grid.Init assumes that the input puzzles always follows the expected syntax -> no error handling otherwise
- handling connected twins for regions is missing
- brute force search is not implemented but could help if all else fails
- make a nicer equality check for BitArrays than AreEqual