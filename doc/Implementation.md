# Overview

The following describes how I might go about turning this into a full, playable implementation which initially would probably be done as a console application while I build the engine up.

# Idea In Brief

Treat each cell as one of two types of finite state machine. Which of these two it is will depend on whether the cell starts as mined or not. The game maintains a link between each cell, and its state. If, at the end of a turn any cell is in a state that says it has a detonated mine the game is lost. If all cells are in the state of having been correctly flagged or uncovered the game is won.

# Possible States

* Covered / Hidden - The player has not uncovered this cell, and implies it contains no mine
* Mined - The player has not uncovered this cell, and implies it contains a mine
* Flagged False - The player has flagged this cell, but it does not contain a mine
* Flagged True - The player has flagged this cell, and it does contain a mine
* Uncovered - The player has uncovered this cell, and implies it contains no mine
* Exploded - The player has uncovered this cell, and implies it contains a mine

# Other Implementation Ideas

## Useful Design Patterns

You _might_ want to consider combining the [`State`](https://en.wikipedia.org/wiki/State_pattern) pattern with the [`Flyweight`](https://en.wikipedia.org/wiki/Flyweight_pattern) pattern, particularly as it seems unlikely the state will need to know anything about the cell it is the state for. We might have a _lot_ of cells. The default expert-level game on [Minesweeper Online](https://minesweeperonline.com/) is 16x30 (480 cells) with 10 mines. It can go up to 99x99 (9,801 cells). It seems like it would be _much_ more efficient to only store one instance of each state, rather than one instance per cell in that state.

## Potential Problems

### Invalid Transitions

Initial versions represented the different states for a cell as a record rather than an abstract base class with different implementations. We were storing the required information in `Dictionary<Cell, CellState>`. As the transitions between state were defined in `Game` rather than a hypothetical `FlaggedTrueCellState` class there would be the question of how to get good exception messages if there is no valid transition e.g.:

> You cannot flag an exploded cell.

You would _probably_ also want a custom exception type e.g. `InvalidCellStateTransitionException` with the intended start and end states as properties of the exception type.

It might be reasonable to store the invalid transitions in a transition map along with the message you would want for the exception.

# Moves

Based on the states above we probably only need two methods for moves on any game class:

* Reveal / Clear
  * Uncover a Covered or Mined cell which then transitions into the Uncovered or Exploded state respectively
  * [Minesweeper Online](https://minesweeperonline.com/) does not allow flagged squares to be cleared
* ToggleFlag
  * If a cell is Covered or Mined transition to the Flagged True or Flagged False state respectively
  * If a cell is Flagged True or Flagged False transition to the Covered or Mined state respectively

You _could_ have separate methods for `Flag` and `Unflag` but that would require any client to track the state of a cell. I don't think should be its responsibility. That would also mean similar code if you had multiple clients e.g. Console, Windows App, Blazor App etc

When revealing a cell one of three things should happen according to "standard" minesweeper rules:

* The cell is mined and you lose the game
* The cell has a hint that is non-zero and you are shown the hint
* The cell has a hint of zero and all adjacent cells that are not revealed, or flagged, or mined are revealed automatically (i.e. the reveal method should operate recursively, but without losing you the game)

# Other Game Ideas

* Give the player a number of "lives" i.e. they can detonate x mines before losing
* Give the player a number of "assists" or "aids"
  * This might be to randomly uncover a cell that has no mine
  * This might be to randomly flag a cell correctly
  * Show if any flags have been put on cells that don't contain a mine
* Can I make games out of other polygons the tessellate well such as hexagons or octagons?
  * _How would I specify coordinates, determine adjacency etc in this case?_