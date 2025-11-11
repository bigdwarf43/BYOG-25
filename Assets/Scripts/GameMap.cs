using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.RuleTile.TilingRuleOutput;


public class GameMap : MonoBehaviour
{

    public int grid_rows, grid_cols;
    [SerializeField]
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
    [SerializeField]
    GameObject ability_pickup_prefab;
    [SerializeField]
    Sprite exit_tile_sprite;

    List<Enemy> enemy_list = new List<Enemy>();
    private Tile[,] tiles; // 2D array to store tile references

    // What world this map belongs to
    WorldMap world_map;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init(int grid_rows, int grid_cols, WorldMap world_map, string[] exits = null, bool add_walls=true, bool add_enemies=true, ScriptableObject spawn_ability=null)
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
        GenerateGrid(exits, add_walls);
        if (add_enemies)
        {
            PopulateEnemies();
        }

        if (spawn_ability)
        {
            SpawnAbilityPickup(spawn_ability);
        }

    }

    void SpawnAbilityPickup(ScriptableObject ability)
    {
        var abilityObj = ability as IAbility;
        if (abilityObj == null) return;

        int midRow = grid_rows / 2;
        int midCol = grid_cols / 2;

        GameObject ability_pickup = Instantiate(ability_pickup_prefab, transform);
        ability_pickup.GetComponent<SpriteRenderer>().sprite = abilityObj.AbilityToken;
        var pickupScript = ability_pickup.GetComponent<AbilityPickup>();
        pickupScript.ability_to_grant = ability;

        Tile emptyTile = tiles[midRow, midCol];
        if (emptyTile != null)
        {
            pickupScript.Initialize(emptyTile);
        }
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

    void GenerateGrid(string[] exit_directions, bool add_walls=true)
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

        if (add_walls)
        {
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

            // Hacking the exit tile sprite in
            obj.GetComponent<SpriteRenderer>().sprite = exit_tile_sprite;
            obj.GetComponent<SpriteRenderer>().sortingOrder = 0;

            obj.transform.rotation = dir switch
            {
                "up" => Quaternion.identity,
                "down" => Quaternion.Euler(1f,1f,180f),
                "left" => Quaternion.Euler(1f, 1f, 90f),
                "right" => Quaternion.Euler(1f, 1f, -90f),
                _ => Quaternion.identity
            };

            // Slight offset to match the border
            obj.transform.position = dir switch
            {
                "up" => obj.transform.position + new Vector3(0f, 0.1f, 0f),
                "down" => obj.transform.position + new Vector3(0f, -0.1f, 0f),
                "left" => obj.transform.position + new Vector3(-0.1f, 0f, 0f),
                "right" => obj.transform.position + new Vector3(0.1f, 0f, 0f),
                _ => obj.transform.position
            };



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
        StartCoroutine(ProcessEnemies());
        
    }

    private IEnumerator ProcessEnemies()
    {
        yield return new WaitForSeconds(0.15f);
        Player player = FindPlayer();

        bool player_attacked = false;
        foreach (Enemy enemy in enemy_list)
        {
            if (enemy == null || enemy.CurrentHealth <= 0)
                continue;

            Tile currentTile = enemy.currentTile;
            Tile targetTile = currentTile; // default

            bool hasPlayer = player != null && enemy.IsPlayerInRange(player);

            if (hasPlayer)
            {
                targetTile = HandleChase(enemy, player, currentTile);
            }
            else
            {
                targetTile = HandleRandomMove(enemy, currentTile);
            }

            if (targetTile == null || targetTile == currentTile)
                continue;

            // Act
            if (targetTile.occupant is Player)
            {
                enemy.AttackEntity(targetTile);
                player_attacked = true;
            }
            else if (IsTileEmpty(targetTile))
            {
                enemy.MoveEntity(targetTile);

            }
        }

        StartCoroutine(ReEnablePlayerMove(player));


        if (player_attacked)
        {
            AudioManager.PlaySfx("attack");
        }
        else
        {
            AudioManager.PlaySfx("step");
        }
    }


    private IEnumerator ReEnablePlayerMove(Player player)
    {
        // wait 1 frame to ensure all Lerp finished
        yield return null;
        player.CanMove = true;
    }

    private Tile HandleRandomMove(Enemy enemy, Tile currentTile)
    {
        if (enemy.can_destroy_walls)
        {
            Tile[] neighbors = GetNeighbors(currentTile);
            if (neighbors.Length == 0) return currentTile;

            Tile targetTile = neighbors[Random.Range(0, neighbors.Length)];
            string direction = GetDirectionBetween(currentTile, targetTile);

            if (IsWallInThisDirection(currentTile.row, currentTile.col, direction))
            {
                HandleWallDestruction(enemy, direction);
                return currentTile;
            }

            return targetTile;
        }
        else
        {
            return GetRandomValidNeighbour(currentTile);
        }
    }

    private Tile HandleChase(Enemy enemy, Player player, Tile currentTile)
    {
        Vector2Int dirToPlayer = enemy.GetDirectionTowardsPlayer(player);
        string direction = GetDirectionString(dirToPlayer);

        Tile candidateTile = GetTileAt(currentTile.row + dirToPlayer.x, currentTile.col + dirToPlayer.y);

        if (candidateTile != null)
        {
            if (IsMoveValid(currentTile.row, currentTile.col, direction, enemy))
            {
                return candidateTile;
            }
            else if (enemy.can_destroy_walls && IsWallInThisDirection(currentTile.row, currentTile.col, direction))
            {
                HandleWallDestruction(enemy, direction);
                return currentTile; // Can't move but destroyed wall
            }
        }

        // fallback random move
        return GetRandomValidNeighbour(currentTile);
    }

    private void HandleWallDestruction(Enemy enemy, string direction)
    {
        Vector2 dir = direction switch
        {
            "up" => new Vector2(0, -1),
            "down" => new Vector2(0, 1),
            "left" => new Vector2(-1, 0),
            "right" => new Vector2(1, 0),
            _ => Vector2.zero
        };

        if (dir != Vector2.zero)
            StartCoroutine(enemy.JerkTowards(dir));

        DeleteWall(enemy.currentTile.row, enemy.currentTile.col, direction);
    }

    private string GetDirectionString(Vector2Int dir)
    {
        return dir switch
        {
            { x: -1, y: 0 } => "up",
            { x: 1, y: 0 } => "down",
            { x: 0, y: 1 } => "right",
            { x: 0, y: -1 } => "left",
            _ => null
        };
    }


    public string GetDirectionBetween(Tile from, Tile to)
    {
        int rowDiff = to.row - from.row;
        int colDiff = to.col - from.col;

        if (rowDiff == -1 && colDiff == 0)
            return "up";
        if (rowDiff == 1 && colDiff == 0)
            return "down";
        if (rowDiff == 0 && colDiff == 1)
            return "right";
        if (rowDiff == 0 && colDiff == -1)
            return "left";

        return string.Empty; // Not directly adjacent
    }

    public bool IsMoveValid(int current_row, int current_col, string direction, Entity entity=null)
    {

        if (entity && entity.ignore_walls)
        {
            return true;
        }
        else
        {
            return this.tiles[current_row, current_col].IsDirectionValid(direction);
        }
    }

    public bool IsWallInThisDirection(int current_row, int current_col, string direction)
    {
        return !this.tiles[current_row, current_col].IsDirectionValid(direction);

    }

    public bool DeleteWall(int current_row, int current_col, string direction)
    {

        Tile tile = this.tiles[current_row, current_col];

        // Remove the wall from the current tile
        bool wall_removed = tile.RemoveWall(direction);

        if (wall_removed)
        {
            // Determine the neighbor tile’s coordinates and opposite direction
            int neighborRow = current_row;
            int neighborCol = current_col;
            string oppositeDir = "";

            switch (direction)
            {
                case "up":
                    neighborRow = current_row - 1;
                    oppositeDir = "down";
                    break;
                case "down":
                    neighborRow = current_row + 1;
                    oppositeDir = "up";
                    break;
                case "left":
                    neighborCol = current_col - 1;
                    oppositeDir = "right";
                    break;
                case "right":
                    neighborCol = current_col + 1;
                    oppositeDir = "left";
                    break;
                default:
                    Debug.LogWarning($"Invalid direction '{direction}' in DeleteWall.");
                    return false;
            }

            // Check if the neighbor tile exists within bounds
            if (neighborRow >= 0 && neighborRow < tiles.GetLength(0) &&
                neighborCol >= 0 && neighborCol < tiles.GetLength(1))
            {
                Tile neighborTile = tiles[neighborRow, neighborCol];
                neighborTile.RemoveWall(oppositeDir);
            }

            return true;
        }
        else
        {
            return false;
        }
        


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

    public Tile[] GetNeighbors(Tile tile)
    {
        List<Tile> neighbours = new List<Tile>();

        int maxRow = tiles.GetLength(0) - 1;
        int maxCol = tiles.GetLength(1) - 1;

        // --- UP NEIGHBOR (row + 1) ---
        if (tile.row + 1 < maxRow)
            neighbours.Add(tiles[tile.row + 1, tile.col]);

        // --- DOWN NEIGHBOR (row - 1) ---
        if (tile.row - 1 > 0)
            neighbours.Add(tiles[tile.row - 1, tile.col]);

        // --- RIGHT NEIGHBOR (col + 1) ---
        if (tile.col + 1 < maxCol)
            neighbours.Add(tiles[tile.row, tile.col + 1]);

        // --- LEFT NEIGHBOR (col - 1) ---
        if (tile.col - 1 > 0)
            neighbours.Add(tiles[tile.row, tile.col - 1]);

        return neighbours.ToArray();
    }

    /// <summary>
    /// Returns all valid neighboring tiles that the given tile can move to,
    /// depending on wall connections and occupant properties.
    /// </summary>
    public Tile[] GetValidTileNeighbours(Tile tile)
    {
        // Store all valid neighboring tiles here
        List<Tile> neighbours = new List<Tile>();

        // --- UP NEIGHBOR (row + 1) ---
        // Check if the tile above exists (within bounds)
        // Then check if movement "down" from that neighbor into this tile is valid,
        // or if the current occupant can ignore walls entirely.
        if ((tile.row + 1 < tiles.GetLength(0)) &&
            (tile.IsDirectionValid("down") || tile.occupant.ignore_walls))
        {
            neighbours.Add(tiles[tile.row + 1, tile.col]);
        }

        // --- DOWN NEIGHBOR (row - 1) ---
        // Similar logic: ensure within bounds, then check wall or ignore flag
        if ((tile.row - 1 >= 0) &&
            (tile.IsDirectionValid("up") || tile.occupant.ignore_walls))
        {
            neighbours.Add(tiles[tile.row - 1, tile.col]);
        }

        // --- RIGHT NEIGHBOR (col + 1) ---
        if ((tile.col + 1 < tiles.GetLength(1)) &&
            (tile.IsDirectionValid("right") || tile.occupant.ignore_walls))
        {
            neighbours.Add(tiles[tile.row, tile.col + 1]);
        }

        // --- LEFT NEIGHBOR (col - 1) ---
        if ((tile.col - 1 >= 0) &&
            (tile.IsDirectionValid("left") || tile.occupant.ignore_walls))
        {
            neighbours.Add(tiles[tile.row, tile.col - 1]);
        }

        // Return all collected neighbors as an array
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

    public List<Tile> FindCrossTiles(Tile current_tile)
    {
        var tiles = new List<Tile>();

        int row = current_tile.row;
        int col = current_tile.col;

        for (int i = 1; i < grid_rows-1; i++)
        {
            if (i != row)
                tiles.Add(GetTileAt(i, col));
        }

        for (int j = 1; j < grid_cols-1; j++)
        {
            if (j != col)
                tiles.Add(GetTileAt(row, j));
        }

        return tiles;
    }

    public List<Tile> FindAllTilesInDirection(Tile current_tile, string move_direction)
    {
        var result = new List<Tile>();

        int r = current_tile.row;
        int c = current_tile.col;

        switch (move_direction)
        {
            case "up":
                for (int i = r - 1; i >= 0; i--)
                    result.Add(GetTileAt(i, c));
                break;

            case "down":
                for (int i = r + 1; i < tiles.GetLength(0); i++)
                    result.Add(GetTileAt(i, c));
                break;

            case "left":
                for (int j = c - 1; j >= 0; j--)
                    result.Add(GetTileAt(r, j));
                break;

            case "right":
                for (int j = c + 1; j < tiles.GetLength(1); j++)
                    result.Add(GetTileAt(r, j));
                break;

            default:
                // invalid direction -> return empty list
                break;
        }

        return result;
    }


    public Tile FindTileInDirection(Tile current_tile, string direction)
    {
        int r = current_tile.row;
        int c = current_tile.col;

        int targetR = r;
        int targetC = c;

        switch (direction)
        {
            case "up":
                targetR = r - 1;
                break;

            case "down":
                targetR = r + 1;
                break;

            case "left":
                targetC = c - 1;
                break;

            case "right":
                targetC = c + 1;
                break;

            default:
                return null;
        }

        // bounds check
        if (targetR < 0 || targetR >= tiles.GetLength(0)) return null;
        if (targetC < 0 || targetC >= tiles.GetLength(1)) return null;

        // wall validity check
        if (!current_tile.IsDirectionValid(direction) && !current_tile.occupant.ignore_walls)
            return null;

        return tiles[targetR, targetC];
    }




}
