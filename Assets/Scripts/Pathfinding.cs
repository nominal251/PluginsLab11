using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    [Header("Random Grid Settings")]
    [SerializeField] private int gridWidth = 5;
    [SerializeField] private int gridHeight = 5;
    [SerializeField] private float obstacleProbability = 0.25f;

    private List<Vector2Int> path = new List<Vector2Int>();
    private Vector2Int start = new Vector2Int(0, 1);
    private Vector2Int goal = new Vector2Int(4, 4);
    private Vector2Int next;
    private Vector2Int current;

    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
    };

    private int[,] grid = new int[,]
    {
        { 0, 1, 0, 0, 0 },
        { 0, 1, 0, 1, 0 },
        { 0, 0, 0, 1, 0 },
        { 0, 1, 1, 1, 0 },
        { 0, 0, 0, 0, 0 }
    };

    private void Start()
    {
        FindPath(start, goal);
    }

    [ContextMenu("Add Random Obstacle")]
    public void AddRandomObstacle()
    {
        // Collect every open cell except the start and goal
        List<Vector2Int> openCells = new List<Vector2Int>();
        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                Vector2Int cell = new Vector2Int(x, y);
                if (grid[y, x] == 0 && cell != start && cell != goal)
                {
                    openCells.Add(cell);
                }
            }
        }

        if (openCells.Count == 0)
        {
            Debug.Log("No open cells left.");
            return;
        }

        AddObstacle(openCells[Random.Range(0, openCells.Count)]);
    }

    [ContextMenu("Generate Random Grid")]
    private void GenerateRandomGridFromMenu()
    {
        GenerateRandomGrid(gridWidth, gridHeight, obstacleProbability);
    }

    public void GenerateRandomGrid(int width, int height, float obstacleProbability)
    {
        if (width <= 0 || height <= 0)
        {
            Debug.LogWarning("GenerateRandomGrid: width and height must be greater than 0.");
            return;
        }

        grid = new int[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                grid[y, x] = Random.value < obstacleProbability ? 1 : 0;
            }
        }

        if (IsInBounds(start)) grid[start.y, start.x] = 0;
        if (IsInBounds(goal)) grid[goal.y, goal.x] = 0;

        FindPath(start, goal); // recalculate so the path reflects the change
    }

    public void AddObstacle(Vector2Int position)
    {
        if (!IsInBounds(position))
        {
            Debug.LogWarning($"AddObstacle: {position} is outside the grid.");
            return;
        }

        grid[position.y, position.x] = 1;
        FindPath(start, goal); // recalculate so the path reflects the change
    }

    private void OnDrawGizmos()
    {
        float cellSize = 1f;

        // Draw grid cells
        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                Vector3 cellPosition = new Vector3(x * cellSize, 0, y * cellSize);
                Gizmos.color = grid[y, x] == 1 ? Color.black : Color.white;
                Gizmos.DrawCube(cellPosition, new Vector3(cellSize, 0.1f, cellSize));
            }
        }

        // Draw path
        foreach (var step in path)
        {
            Vector3 cellPosition = new Vector3(step.x * cellSize, 0, step.y * cellSize);
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(cellPosition, new Vector3(cellSize, 0.1f, cellSize));
        }

        // Draw start and goal
        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector3(start.x * cellSize, 0, start.y * cellSize), new Vector3(cellSize, 0.1f, cellSize));

        Gizmos.color = Color.red;
        Gizmos.DrawCube(new Vector3(goal.x * cellSize, 0, goal.y * cellSize), new Vector3(cellSize, 0.1f, cellSize));
    }

    private bool IsInBounds(Vector2Int point)
    {
        return point.x >= 0 && point.x < grid.GetLength(1) && point.y >= 0 && point.y < grid.GetLength(0);
    }

    private void FindPath(Vector2Int start, Vector2Int goal)
    {
        path.Clear();

        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(start);

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        cameFrom[start] = start;

        while (frontier.Count > 0)
        {
            current = frontier.Dequeue();

            if (current == goal)
            {
                break;
            }

            foreach (Vector2Int direction in directions)
            {
                next = current + direction;

                if (IsInBounds(next) && grid[next.y, next.x] == 0 && !cameFrom.ContainsKey(next))
                {
                    frontier.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        if (!cameFrom.ContainsKey(goal))
        {
            Debug.Log("Path not found.");
            return;
        }

        // Trace path from goal to start
        Vector2Int step = goal;
        while (step != start)
        {
            path.Add(step);
            step = cameFrom[step];
        }
        path.Add(start);
        path.Reverse();
    }
}