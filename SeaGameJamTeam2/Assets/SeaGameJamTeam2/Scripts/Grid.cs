using System.Collections.Generic;
using System.Globalization;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public GameObject towerPrefab; // Prefab of the tower to be placed
    public LayerMask groundLayer;   // Layer to check for tower placement
    public LayerMask towerLayer;   // Layer to check for tower placement
    public Vector3 gridSize = new Vector3(1f, 1f, 1f); // Size of the grid cell
    
    public int gridWidth = 10;// Number of columns in the grid
    public int gridHeight = 10; // Number of rows in the grid
    
    // Add a reference to the enemy's world tree target
    public Transform worldTree;

    private List<GameObject> placedTowers = new List<GameObject>();

    private AStar pathfind;
    
    // Start is called before the first frame update
    void Start()
    {
        pathfind = new AStar(this);
        // Assuming enemies will start from this GameObject's position
        FindPath(transform.position, worldTree.position);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                Vector3 gridPosition = GetNearestGridPosition(hit.point);

                if (!IsBlocked(gridPosition))
                {
                    GameObject newTower = Instantiate(towerPrefab, gridPosition, Quaternion.identity);
                    placedTowers.Add(newTower);
                }
            }
        }
    }

    Vector3 GetNearestGridPosition(Vector3 position)
    {
        Vector3Int gridPosition = WorldToGrid(position);
        Vector3 snappedPosition = GridToWorld(gridPosition);
        return snappedPosition;
    }

    bool IsBlocked(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, gridSize.x / 2, towerLayer);
        return colliders.Length > 0;
    }
    bool IsBlocked(Vector3Int gridPosition)
    {
        Vector3 position = GridToWorld(gridPosition);
        Collider[] colliders = Physics.OverlapSphere(position, gridSize.x / 2, towerLayer);
        return colliders.Length > 0;
    }
    
    bool IsMovementBlocked(Vector3Int currentCell, Vector3Int neighborCell)
    {
        if (!IsValidCell(neighborCell.x, neighborCell.y))
        {
            return false; // Neighbor is out of bounds
        }

        if (!IsBlocked(neighborCell))
        {
            return false;
        }

        // Calculate the absolute difference between the current and neighbor cell indices
        Vector3Int diffX = new Vector3Int(neighborCell.x - currentCell.x, 0, 0);
        Vector3Int diffY = new Vector3Int(0, neighborCell.y - currentCell.y, 0);

        // Check if at least one adjacent cell is clear
        bool isAdjacentClear = (!IsBlocked(currentCell + diffX))
                               || (!IsBlocked(currentCell + diffY));

        return isAdjacentClear;
    }

    public Vector3Int WorldToGrid(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / gridSize.x);
        int z = Mathf.FloorToInt(0);
        int y = Mathf.FloorToInt(position.z / gridSize.z);
        return new Vector3Int(x, y, z);
    }

    public Vector3 GridToWorld(Vector3Int gridPosition)
    {
        float x = gridPosition.x * gridSize.x + gridSize.x / 2;
        float z = gridPosition.y * gridSize.y + gridSize.y / 2;
        float y = gridPosition.z * gridSize.z + gridSize.z / 2;
        return new Vector3(x, y, z);
    }
    
    bool IsValidCell(int x, int y)
    {
        return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
    }

    public List<Vector3Int> GetOpenNeighbors(Vector3Int pos)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        // Define relative positions for neighboring cells (orthogonal and diagonal)
        int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
        int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

        for (int i = 0; i < dx.Length; i++)
        {
            int newX = pos.x + dx[i];
            int newY = pos.y + dy[i];
            Vector3Int neighbor = new Vector3Int(newX, newY);

            if (IsValidCell(newX, newY) && !IsMovementBlocked(pos, neighbor))
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }
    
    
    void FindPath(Vector3 start, Vector3 target)
    {
        Vector3Int startCell = WorldToGrid(start);
        Vector3Int targetCell = WorldToGrid(target);

        List<Vector3Int> path = pathfind.AStarAlgorithm(startCell, targetCell);

        // Now you can do something with the path, like move your enemy along it.
    }
    
    // IEnumerator MoveAlongPath(List<Vector3Int> path)
    // {
    //     foreach (Vector3Int cell in path)
    //     {
    //         Vector3 worldPosition = GridToWorld(cell);
    //         while (Vector3.Distance(transform.position, worldPosition) > 0.1f)
    //         {
    //             transform.position = Vector3.MoveTowards(transform.position, worldPosition, Time.deltaTime * moveSpeed);
    //             yield return null;
    //         }
    //     }
    // }

}
