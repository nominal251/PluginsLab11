1. BFS works by searching all nodes at the present depth before moving to the next depth, ensuring the shortest path is found. 
Djikstra explores the cheapest current cell, finding the lowest-cost path, but still searching in all directions like BFS
A* explores cells heading towards the goal, so it searches fewer cells than the other paths

2. When updating obstacles in real-time, the path has to constantly be recalculated if a new obstacle intersects with the current path

3. For a much larger grid or open world, I would pick A* as it searching towards the goal, avoiding less direct routes, means it works more efficiently. In a large grid, BFS is simpler but would waste lots of searches on fruitless routes

4. It would depend on the size and complexity of the grid. With a simple grid, like the one for this lab, BFS would still be fine, but with a more complex grid A* would be much more efficient.