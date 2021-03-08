# GameDev
Project created for the game development course taken in SUPSI.

## Running the game
### From release, without GizMos
Simply go to the releases page, download for your platform and run.

### From within unity
Clone the repository and open it in Unity. 
The project was created using Unity 2020.1f

## Cool facts

### Custom A* path-finding on sphere
This game contains a custom multi threaded implementation with path smoothing of the A* algorithm. 
A* algorithm is used to achieve a compromise between speed and precision. It usually find a good solution that is indeed not always the optimal solution. 

The reason why I devoleped this implementation was to achieve pathfinding on a sphere, which is the terrain of the game.
Unity built-in pathfinding was of no help because it only can handle flat or partially flat terrains.

The process basically consists of creating a virtual copy of the sphere mesh, with higher level of detail (usign an Icosphere), and to use each vertex in that mesh as a node in the graph to be feeded to A*. 
After the terrain is initialized, if an entity capable of pathfinding (i.e. an AIAgent) enters the surface of the sphere, it starts calculating the path to the targe on a separate thread using C# Tasks.
When a valid path to the target node is available, the entity starts moving towards his path and in the same time it starts simplifying the path with a naif algorithm.

Further features can be added to the algorithm to improve performace, for example, I planned to add shader computation for the graph to significantly increase the performance of the algorithm.
