# Minesweeper

This is a programming exercise based on the popular, and well-known game "Minesweeper".

# Code-Writing Exercise

A field of N x M squares is represented by N lines of exactly M characters each. The character '*' represents 
a position that contains a mine. The character '.' represents a position that does not contain a mine. 

Consider the example input of a 3 x 4 minefield of 12 squares, and 2 of which are mines:

```
3 4
*...
..*.
....
```

Your task is to write a program that accepts this input and produces as output a hint-field of identical dimensions. Each square must show either a '*' for a mine _or_ the number of adjacent mines if the square does not contain a mine. In this exercise "adjacent" is used to include those squares touching diagonally, as well as those sharing a border.

The expected output for the example input would be:

```
*211
12*1
0111
```




