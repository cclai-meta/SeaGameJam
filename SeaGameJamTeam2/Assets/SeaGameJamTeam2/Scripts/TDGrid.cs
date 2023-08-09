using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using UnityEditor.SceneManagement;
using UnityEngine;

public class TDGrid : MonoBehaviour
{
    public GameObject towerPrefab; // Prefab of the tower to be placed
    public GameObject enemyPrefab;
    public float spawnInterval = 2.0f; 

    public List<Transform> enemySpawnLocations;
    
    public LayerMask groundLayer;   // Layer to check for tower placement
    public LayerMask towerLayer;   // Layer to check for tower placement
    public LayerMask navigationLayer;   // Layer to check for tower placement
    
    public Vector3 gridSize = new Vector3(1f, 1f, 1f); // Size of the grid cell
    
    public int gridWidth = 10;// Number of columns in the grid
    public int gridHeight = 10; // Number of rows in the grid

    public GameObject StartButtom;
    
    // Add a reference to the enemy's world tree target
    public Transform worldTree;
    public Vector3Int goalCell;

    private List<GameObject> placedTowers = new List<GameObject>();

    private AStar pathfinder;
    
    private float spawnTimer = 0.5f;
    private int enemySpawnIndex = 0;
    public int maxEnemiesInFirstWave = 20;
    private int maxEnemiesInWave = 20;
    private int roundNumber = 1;
    private float currentSpawnInterval;


    private bool canPlace = true;

    public int money = 6;
    
    private bool go = false;

    private List<EnemyMovement> enemies = new();

    public int Lives = 20;

    private static TDGrid theGrid;
    
    
    public event Action OnRoundComplete; // Event to signal enemy's death
    public event Action OnDeath; // Event to signal enemy's death

    public static TDGrid Get()
    {
        return theGrid;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        theGrid = this;
        pathfinder = new AStar(this);
        goalCell = WorldToGrid(worldTree.position);
    }

    public void StartRound()
    {
        go = true;
        maxEnemiesInWave = maxEnemiesInFirstWave * roundNumber;
    }

    public void SetCanPlace(bool b)
    {
        canPlace = b;
    }
    
    void Update()
    {
        if (canPlace && Input.GetMouseButtonDown(0) && money > 0) // Left mouse button
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                Vector3 gridPosition = GetNearestGridPosition(hit.point);

                if (!IsBlocked(gridPosition, towerLayer))
                {
                    GameObject newTower = Instantiate(towerPrefab, gridPosition, Quaternion.identity);
                    placedTowers.Add(newTower);
                    money--;
                    foreach (EnemyMovement enemy in enemies)
                    {
                        updateEnemyPath(enemy);
                    }
                }
            }
        }
        
        currentSpawnInterval += Time.deltaTime;

        if (go && spawnTimer < (currentSpawnInterval * roundNumber) && enemySpawnIndex < maxEnemiesInWave)
        {
            Transform startLocation = enemySpawnLocations[enemySpawnIndex % enemySpawnLocations.Count];
            SpawnEnemy(startLocation);
            enemySpawnIndex++;
            currentSpawnInterval = 0.0f; // Reset the spawn timer
        }
    }

    Vector3 GetNearestGridPosition(Vector3 position)
    {
        Vector3Int gridPosition = WorldToGrid(position);
        Vector3 snappedPosition = GridToWorld(gridPosition);
        return snappedPosition;
    }

    public bool IsBlocked(Vector3 position, LayerMask layer)
    {
        Collider[] colliders = Physics.OverlapSphere(position, gridSize.x / 2, layer);
        return colliders.Length > 0;
    }
    public bool IsBlocked(Vector3Int gridPosition, LayerMask layer)
    {
        Vector3 position = GridToWorld(gridPosition);
        Collider[] colliders = Physics.OverlapSphere(position, gridSize.x / 2, layer);
        return colliders.Length > 0;
    }
    
    public bool IsMovementBlocked(Vector3Int currentCell, Vector3Int neighborCell)
    {
        if (!IsValidCell(neighborCell.x, neighborCell.y))
        {
            return true; // Neighbor is out of bounds
        }

        if (IsBlocked(neighborCell, navigationLayer))
        {
            return true;
        }

        // Calculate the absolute difference between the current and neighbor cell indices
        Vector3Int diffX = new Vector3Int(neighborCell.x - currentCell.x, 0, 0);
        Vector3Int diffY = new Vector3Int(0, neighborCell.y - currentCell.y, 0);

        // Check if at least one adjacent cell is clear
        bool isAdjacentClear = (IsBlocked(currentCell + diffX, navigationLayer))
                               || (IsBlocked(currentCell + diffY, navigationLayer));

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
        return x >= -gridWidth && x < gridWidth && y >= -gridHeight && y < gridHeight;
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

    void SpawnEnemy(Transform startLocation)
    {
        Vector3Int spawnGridLocation = WorldToGrid(startLocation.position);
        GameObject newEnemy = Instantiate(enemyPrefab, startLocation.position, Quaternion.identity);

        Hittable enemyHit = newEnemy.GetComponent<Hittable>();
        enemyHit.HP = enemyHit.HP * roundNumber;
        
        // Assign the path to the enemy
        EnemyMovement enemyMovement = newEnemy.GetComponent<EnemyMovement>();
        updateEnemyPath(enemyMovement);
        enemyMovement.OnDeath += HandleEntityDeath;
        
        enemies.Add(enemyMovement);
    }
    
    public void HandleEntityDeath(bool wasKilledByPlayer, GameObject enemy)
    {
        EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
        if (enemyMovement)
        {
            enemies.Remove(enemyMovement);
            if (!wasKilledByPlayer)
            {
                Lives--;
                if (Lives == 0)
                {
                    OnDeath?.Invoke();
                }
            }

            if (enemies.Count == 0)
            {
                money += 2 * roundNumber;
                OnRoundComplete?.Invoke();
            }
        }
    }

    void updateEnemyPath(EnemyMovement enemy)
    {
        Vector3Int start = WorldToGrid(enemy.gameObject.transform.position);
        // Get the enemy's path using A* pathfinding
        List<Vector3Int> path = pathfinder.AStarAlgorithm(start, goalCell);

        List<Vector3> worldPath = new List<Vector3>();
        foreach (Vector3Int point in path)
        {
            worldPath.Add(GridToWorld(point));
        }
        enemy.SetPath(worldPath);
    }
}
