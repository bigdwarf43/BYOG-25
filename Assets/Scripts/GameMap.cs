using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class GameMap : MonoBehaviour
{

    public int grid_rows, grid_cols;
    float cell_size = 0.5f;

    // direction maps in the order {row, col}
    static Dictionary<string, Vector2> directions_map = new Dictionary<string, Vector2>
    {
        {"up", new Vector2(1, 0)},
        {"down", new Vector2(-1, 0)},
        {"left", new Vector2(0, -1)},
        {"right", new Vector2(0, 1)},

    };
    List<string> direction_keys = GameMap.directions_map.Keys.ToList();


    // direction maps in the order {row, col}
    static Dictionary<string, ExitTile> exit_map = new Dictionary<string, ExitTile>
    {
        {"up", null},
        {"down", null},
        {"left", null},
        {"right", null},

    };

    // Prefabs
    [Header("Prefabs")]
    [SerializeField]
    GameObject tile_prefab;
    [SerializeField]
    GameObject enemy_prefab;

    List<Enemy> enemy_list = new List<Enemy>();
    private Tile[,] tiles; // 2D array to store tile references

    // What world this map belongs to
    WorldMap world_map;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init(int grid_rows, int grid_cols, WorldMap world_map, string[] exits = null)
    {

        this.world_map = world_map;
        this.grid_rows = grid_rows + 2;
        this.grid_cols = grid_cols + 2;

        // Initialize exits if not provided
        if (exits == null)
        {
            exits = new string[] { "up", "down", "left", "right" };
        }

        tiles = new Tile[this.grid_rows, this.grid_cols];
        GenerateGrid(exits);
        PopulateEnemies();

    }

    void PopulateEnemies()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject enemy = Instantiate(enemy_prefab, transform);
            Enemy enemy_obj = enemy.transform.GetComponent<Enemy>();

            Tile emptyTile = FindEmptyTile();
            if (emptyTile != null)
            {
                enemy_obj.Initialize(emptyTile);
                this.enemy_list.Add(enemy_obj);

            }
        }

    }

    public Tile FindEmptyTile()
    {
        List<Tile> emptyTiles = new List<Tile>();

        for (int row = 1; row < grid_rows-1; row++)
        {
            for (int col = 1; col < grid_cols-1; col++)
            {
                Tile tile = GetTileAt(row, col);
                if (tile != null && tile.occupant == null)
                {
                    emptyTiles.Add(tile);
                }
            }
        }


        if (emptyTiles.Count > 0)
        {
            return emptyTiles[UnityEngine.Random.Range(0, emptyTiles.Count)];
        }

        return null;
    }

    void GenerateGrid(string[] exit_directions)
    {
        for (int row = 1; row < grid_rows-1; row++)
        {
            for (int col = 1; col < grid_cols-1; col++)
            {
                Vector3 pos = new Vector3((col * cell_size) + transform.position.x, row * cell_size * -1 + transform.position.y, 0);
                GameObject obj = Instantiate(tile_prefab, pos, Quaternion.identity, this.transform);
                Tile tile = obj.GetComponent<Tile>();

                tile.Init(row, col, this.world_map);
                tiles[row, col] = tile;
            }
        }

        // Populate walls
        for (int row = 1; row < grid_rows - 1; row++)
        {
            for (int col = 1; col < grid_cols - 1; col++)
            {

                if (row != 1 && col != 1 && col != grid_cols - 2 && row != grid_rows - 2)
                {
                    Tile tile = tiles[row, col];
                    float wallProbability = 0.5f;

                    if (Random.Range(0f, 1.0f) < wallProbability)
                    {
                        string random_wall_key = direction_keys[Random.Range(0, direction_keys.Count)];

                        switch (random_wall_key)
                        {
                            case "up":
                                tiles[row - 1, col].AddWalls("down");
                                break;
                            case "down":
                                tiles[row + 1, col].AddWalls("up");
                                break;
                            case "left":
                                tiles[row, col - 1].AddWalls("right");
                                break;
                            case "right":
                                tiles[row, col + 1].AddWalls("left");
                                break;

                        }

                        tile.AddWalls(random_wall_key);
                    }

                }
            }
        }

        for (int i = 0; i < exit_directions.Length; i++)
        {
            string dir = exit_directions[i];

            int midRow = grid_rows / 2;
            int midCol = grid_cols / 2;

            // Determine row and col in one line using pattern matching
            (int row, int col) = dir switch
            {
                "up" => (0, midCol),
                "down" => (grid_rows - 1, midCol),
                "left" => (midRow, 0),
                "right" => (midRow, grid_cols - 1),
                _ => (0, 0)
            };

            // Compute world position
            Vector3 pos = new Vector3(
                (col * cell_size) + transform.position.x,
                (row * -cell_size) + transform.position.y,
                0
            );

            // Create and initialize exit tile
            GameObject obj = Instantiate(tile_prefab, pos, Quaternion.identity, transform);
            ExitTile exit_tile = obj.AddComponent<ExitTile>();

            exit_map[dir] = exit_tile; // add to the exit map

            exit_tile.Init(row, col, world_map);
            tiles[row, col] = exit_tile;
        }

    }
    Tile GetRandomValidNeighbour(Tile tile)
    {
        Tile[] neighbours = GetValidTileNeighbours(tile);
        var candidates = neighbours.Where(t => t != null && t.occupant == null).ToList();
        return candidates.Count > 0 ? candidates[Random.Range(0, candidates.Count)] : null;
    }
    public void Tick()
    {
        CleanupDeadEnemies();
        Player player = FindPlayer();

        foreach (Enemy enemy in this.enemy_list)
        {

            if (enemy == null || enemy.CurrentHealth <= 0)
                continue;

            Tile currentTile = enemy.currentTile;
            Tile targetTile = currentTile; // default to current tile

            bool hasPlayer = player != null && enemy.IsPlayerInRange(player);

            if (hasPlayer)
            {
                Vector2Int dirToPlayer = enemy.GetDirectionTowardsPlayer(player);

                // Direction mapping
                string direction = dirToPlayer switch
                {
                    { x: -1, y: 0 } => "up",
                    { x: 1, y: 0 } => "down",
                    { x: 0, y: 1 } => "right",
                    { x: 0, y: -1 } => "left",
                    _ => null
                };

                Tile candidateTile = GetTileAt(currentTile.row + dirToPlayer.x, currentTile.col + dirToPlayer.y);

                if (candidateTile != null)
                {
                    if (enemy.currentTile.IsDirectionValid(direction))
                    {
                        targetTile = candidateTile;
                    }
                }

                // Path blocked ? fallback random move
                if (targetTile == currentTile)
                    targetTile = GetRandomValidNeighbour(currentTile);
            }
            else
            {
                // No player in range ? random move
                targetTile = GetRandomValidNeighbour(currentTile);
            }


            // Act
            if (targetTile == null)
                continue;

            if (targetTile.occupant is Player)
            {
                Debug.Log("Attack player");
                enemy.AttackEntity(targetTile);
            }
            else if (IsTileEmpty(targetTile))
            {
                StartCoroutine(enemy.MoveTo(targetTile));
                if (player != null)
                    Debug.Log($"Enemy moving towards player! Distance: {enemy.GetDistanceToPlayer(player)}");
            }
        }
    }

    public bool IsMoveValid(int current_row, int current_col, string direction)
    {
        Debug.Log(current_row.ToString() + current_col.ToString()+ direction);
        print(this.tiles[current_row, current_col]);
        return this.tiles[current_row, current_col].IsDirectionValid(direction);
    }


    private void CleanupDeadEnemies()
    {
        // Remove null references and dead enemies from the list
        enemy_list.RemoveAll(enemy => enemy == null || enemy.CurrentHealth <= 0);
    }

    // OOP CONCEPT: Encapsulation - Method that encapsulates tile access logic
    public Tile GetTileAt(int row, int col)
    {
        if (row < 0 || col < 0 || row >= this.grid_rows || col >= this.grid_cols) return null;
        return tiles[row, col];
    }

    // OOP CONCEPT: Encapsulation - Method that encapsulates tile state checking logic
    public bool IsTileEmpty(Tile tile)
    {
        if (tile == null) return false;
        return tile.occupant == null;
    }

    public Tile[] GetValidTileNeighbours(Tile tile)
    {
        List<Tile> neighbours = new List<Tile>();

        // Up
        if (tile.row + 1 < tiles.GetLength(0) && tile.IsDirectionValid("down"))
            neighbours.Add(tiles[tile.row + 1, tile.col]);

        // Down
        if (tile.row - 1 >= 0 && tile.IsDirectionValid("up"))
            neighbours.Add(tiles[tile.row - 1, tile.col]);

        // Right
        if (tile.col + 1 < tiles.GetLength(1) && tile.IsDirectionValid("right"))
            neighbours.Add(tiles[tile.row, tile.col + 1]);

        // Left
        if (tile.col - 1 >= 0 && tile.IsDirectionValid("left"))
            neighbours.Add(tiles[tile.row, tile.col - 1]);

        return neighbours.ToArray();



    }

    private Player FindPlayer()
    {
        // Find player by searching through all GameObjects with Player component
        Player[] players = FindObjectsOfType<Player>();
        if (players.Length > 0)
        {
            return players[0]; // Return the first player found
        }
        return null;
    }

    // FIX: Add method to update player reference (useful if player respawns)
    public void UpdatePlayerReference()
    {
        Player newPlayer = FindPlayer();
        if (newPlayer != null)
        {
            Debug.Log("Updated player reference in GameMap");
        }
    }

    public Tile FindTileNearExit(string exit_dir)
    {
        if (!exit_map.ContainsKey(exit_dir))
            return null;

        ExitTile exit_tile = exit_map[exit_dir];
        if (exit_tile == null)
            return null;

        int row = exit_tile.row;
        int col = exit_tile.col;

        // Return the tile adjacent to the exit based on its direction
        return exit_dir switch
        {
            "up" => tiles[row + 1, col],
            "down" => tiles[row - 1, col],
            "left" => tiles[row, col + 1],
            "right" => tiles[row, col - 1],
            _ => null
        };
    }


}
