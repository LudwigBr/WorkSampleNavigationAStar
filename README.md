## Work Sample Navigation A*
This project is taken as part of a bigger project and is meant to show an example of my work.

The solution file (.sln) can be opened with Visual Studio. While there is no graphical interface included, there are tests which show the basic functionality.

This progam takes a map (as byte array and rowLength as int) as input. Each byte describes the walkable state of 2 cells. With a given starting position the map will be reduced to all the reachable cells. 
A navigation network can be aligned with the map (and reduced in accuracy in favor of speed if necessary). The A* algorithm is used to find the shortest walkable route between 2 nodes.
