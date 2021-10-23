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

# Moves

Based on the states above we probably only need two methods for moves on any game class:

* Reveal - Uncover a Covered or Mined cell which then transitions into the Uncovered or Exploded state respectively
* ToggleFlag
  * If a cell is Covered or Mined transition to the Flagged True or Flagged False state respectively
  * If a cell is Flagged True or Flagged False transition to the Covered or Mined state respectively

You _could_ have separate methods for `Flag` and `Unflag` but that would require any client to track the state of a cell. I don't think should be its responsibility. That would also mean similar code if you had multiple clients e.g. Console, Windows App, Blazor App etc

When revealing a cell one of three things should happen according to "standard" minesweeper rules:

* The cell is mined and you lose the game
* The cell has a hint that is non-zero and you are shown the hint
* The cell has a hint of zero and all adjacent cells that are not revealed, or flagged, or mined are revealed automatically (i.e. the reveal method should operate recursively, but without losing you the game)

# Other Ideas

* Give the player a number of "lives" i.e. they can detonate x mines before losing
* Give the player a number of "assists" or "aids"
  * This might be to randomly uncover a cell that has no mine
  * This might be to randomly flag a cell correctly
  * Show if any flags have been put on cells that don't contain a mine