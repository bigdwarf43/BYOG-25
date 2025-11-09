using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MapNode
{
    public GameObject game_map;
    public int row;
    public int col;

    public MapNode left_room;
    public MapNode right_room;
    public MapNode up_room;
    public MapNode down_room;

    public bool explored = false;

}
public class WorldMap : MonoBehaviour
{
    [Header("World config")]
    [SerializeField]
    int world_rows = 5;
    [SerializeField]
    int world_cols = 5;

    public MapNode current_map_node;
    public GameMap current_map;

    MapNode[,] nodes;
    Player player;

    [SerializeField]
    GameObject GameMap;

    [SerializeField]
    List<ScriptableObject> Abilities = new List<ScriptableObject>();

    [SerializeField]
    ScriptableObject healthTopUp;
    // Keep track of which abilities have already been spawned
    private HashSet<ScriptableObject> usedAbilities = new HashSet<ScriptableObject>();

    [SerializeField]
    TextMeshProUGUI room_ref_text;

    [Header("Minimap")]
    [SerializeField]
    MiniMapController mini_map;

    public void Start()
    {

    }

    public void Initialize(int grid_rows, int grid_cols, Player player)
    {
        this.nodes = new MapNode[world_rows, world_cols];
        mini_map.Init(this.nodes);
        this.player = player;
        usedAbilities.Clear();

        // initialize the current map
        int rows = nodes.GetLength(0);
        int cols = nodes.GetLength(1);

        int centerRow, centerCol;

        // For odd: exact middle, for even: pick the top-left of the central 2x2 block
        centerRow = (rows - 1) / 2;
        centerCol = (cols - 1) / 2;

        // Create all nodes
        for (int row = 0; row < world_rows; row++)
        {
            for (int col = 0; col < world_cols; col++)
            {

                GameObject game_map = Instantiate(GameMap, transform);
                GameMap map_obj = game_map.GetComponent<GameMap>();
                // Start with all directions
                List<string> exit_dirs = new List<string> { "up", "down", "left", "right" };

                bool add_walls = true;
                bool add_enemies = true;
                ScriptableObject spawn_ability = null;



                // --- Corner cases ---
                if (row == 0 && col == 0)                // Top-left corner
                {
                    exit_dirs.Remove("up");
                    exit_dirs.Remove("left");
                    add_walls = false;
                    add_enemies = false;
                    spawn_ability = GetNextUnusedAbility();
                }
                else if (row == 0 && col == world_cols - 1) // Top-right corner
                {
                    exit_dirs.Remove("up");
                    exit_dirs.Remove("right");
                    add_walls = false;
                    add_enemies = false;
                    spawn_ability = GetNextUnusedAbility();
                }
                else if (row == world_rows - 1 && col == 0) // Bottom-left corner
                {
                    add_walls = false;
                    add_enemies = false;
                    spawn_ability = GetNextUnusedAbility();
                    exit_dirs.Remove("left");
                    exit_dirs.Remove("down");
                }
                else if (row == world_rows - 1 && col == world_cols - 1) // Bottom-right corner
                {
                    exit_dirs.Remove("down");
                    exit_dirs.Remove("right");
                    add_walls = false;
                    spawn_ability = GetNextUnusedAbility();

                    add_enemies = false;
                }
                else if(row==centerRow && col == centerCol)
                {
                    add_enemies = false;
                    add_walls = false;
                }
                else if (row == centerRow && col == 0)
                {
                    add_enemies = false;
                    add_walls = false;
                    spawn_ability = healthTopUp;
                }
                else if (row == centerRow && col == world_cols - 1)
                {
                    add_enemies = false;
                    add_walls = false;
                    spawn_ability = healthTopUp;

                }
                else if (row == 0 && col == centerCol)
                {
                    add_enemies = false;
                    add_walls = false;
                    spawn_ability = healthTopUp;
                }
                else if (row == world_rows-1 && col == centerCol)
                {
                    add_enemies = false;
                    add_walls = false;
                    spawn_ability = healthTopUp;

                }


                if (row == 0)
                {
                    exit_dirs.Remove("up");
                }
                else if(row == world_rows - 1)
                {
                    exit_dirs.Remove("down");
                }

                if (col == 0)
                {
                    exit_dirs.Remove("left");
                }
                else if(col == world_cols - 1)
                {
                    exit_dirs.Remove("right");
                }

                map_obj.Init(grid_rows, grid_cols, this, exit_dirs.ToArray(), add_walls, add_enemies, spawn_ability);
                game_map.SetActive(false);

                this.nodes[row, col] = new MapNode
                {
                    row = row,
                    col = col,
                    game_map = game_map

                };
            }
        }

        // Connect neighbors (graph links)
        for (int row = 0; row < world_rows; row++)
        {
            for (int col = 0; col < world_cols; col++)
            {
                MapNode node = this.nodes[row, col];

                if (col > 0) node.left_room = this.nodes[row, col - 1];
                if (col < world_cols - 1) node.right_room = this.nodes[row, col + 1];
                if (row > 0) node.up_room = this.nodes[row - 1, col];
                if (row < world_rows - 1) node.down_room = this.nodes[row + 1, col];
            }
        }


        

        this.current_map_node = nodes[centerRow, centerCol];
        this.current_map_node.game_map.SetActive(true);
        this.current_map_node.explored = true;
        this.current_map = this.current_map_node.game_map.GetComponent<GameMap>();

        mini_map.UpdateMinimap(this.nodes, this.current_map_node);

    }

    private ScriptableObject GetNextUnusedAbility()
    {
        foreach (var ability in Abilities)
        {
            if (!usedAbilities.Contains(ability))
            {
                usedAbilities.Add(ability);
                return ability;
            }
        }
        

        return null;
    }

    public void ChangeRoom(int exit_tile_row, int exit_tile_col)
    {


        int randRow = this.current_map_node.row;
        int randCol = this.current_map_node.col;


        string opposite_exit_dir = null;
        if (exit_tile_row == 0)
        {
            randRow -= 1;
            opposite_exit_dir = "down";
        }
        else if (exit_tile_row == current_map.grid_rows - 1)
        {
            randRow += 1;

            opposite_exit_dir = "up";
        }
        else if (exit_tile_col == 0)
        {
            randCol -= 1;
            opposite_exit_dir = "right";
        }
        else if (exit_tile_col == current_map.grid_cols - 1)
        {
            randCol += 1;
            opposite_exit_dir = "left";
        }

        room_ref_text.text = "ROOM: " + randRow.ToString() + ", " + randCol.ToString();

        MapNode random_game_map_node = this.nodes[randRow, randCol];

        // set the old map as inactive
        this.current_map_node.game_map.SetActive(false);

        // set the random game map as the new map
        this.current_map_node = random_game_map_node;

        // set it active
        this.current_map_node.game_map.SetActive(true);
        this.current_map_node.explored = true;

        this.current_map = random_game_map_node.game_map.GetComponent<GameMap>();

        Tile tile = this.current_map.FindTileNearExit(opposite_exit_dir);
        this.player.MoveToWithoutLerp(tile);

        AudioManager.PlaySfx("room_change");

        mini_map.UpdateMinimap(this.nodes, this.current_map_node);


    }

}
