using System.Collections.Generic;
using UnityEngine;

public class MapNode
{
    public GameObject game_map;
    public int row;
    public int col;

    public MapNode left_room;
    public MapNode right_room;
    public MapNode up_room;
    public MapNode down_room;

}
public class WorldMap: MonoBehaviour
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

    public void Initialize(int grid_rows, int grid_cols, Player player)
    {
        this.player = player;
        this.nodes = new MapNode[world_rows, world_cols];

        // Create all nodes
        for (int row = 0; row < world_rows; row++)
        {
            for (int col = 0; col < world_cols; col++)
            {

                GameObject game_map = Instantiate(GameMap, transform);
                GameMap map_obj = game_map.GetComponent<GameMap>();
                // Start with all directions
                List<string> exit_dirs = new List<string> { "up", "down", "left", "right" };

                // --- Corner cases ---
                if (row == 0 && col == 0)                // Top-left corner
                {
                    exit_dirs.Remove("up");
                    exit_dirs.Remove("left");
                }
                else if (row == 0 && col == world_cols - 1) // Top-right corner
                {
                    exit_dirs.Remove("up");
                    exit_dirs.Remove("right");
                }
                else if (row == world_rows - 1 && col == 0) // Bottom-left corner
                {
                    exit_dirs.Remove("down");
                    exit_dirs.Remove("left");
                }
                else if (row == world_rows - 1 && col == world_cols - 1) // Bottom-right corner
                {
                    exit_dirs.Remove("down");
                    exit_dirs.Remove("right");
                }

                // --- Edge cases (non-corners) ---
                else if (row == 0)                      // Top edge
                {
                    exit_dirs.Remove("up");
                }
                else if (row == world_rows - 1)         // Bottom edge
                {
                    exit_dirs.Remove("down");
                }
                else if (col == 0)                      // Left edge
                {
                    exit_dirs.Remove("left");
                }
                else if (col == world_cols - 1)         // Right edge
                {
                    exit_dirs.Remove("right");
                }


                map_obj.Init(grid_rows, grid_cols, this, exit_dirs.ToArray());
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


        // initialize the current map
        this.current_map_node = this.nodes[0, 0];
        this.current_map_node.game_map.SetActive(true);
        this.current_map = this.current_map_node.game_map.GetComponent<GameMap>();

    }

    public void ChangeRoom()
    {
        int rowCount = this.nodes.GetLength(0); // number of rows
        int colCount = this.nodes.GetLength(1); // number of columns

        int randRow = Random.Range(0, rowCount);
        int randCol = Random.Range(0, colCount);

        MapNode random_game_map_node = this.nodes[randRow, randCol];

        // set the old map as inactive
        this.current_map_node.game_map.SetActive(false);

        // set the random game map as the new map
        this.current_map_node = random_game_map_node;

        // set it active
        this.current_map_node.game_map.SetActive(true);

        this.current_map = random_game_map_node.game_map.GetComponent<GameMap>();


        this.player.Initialize(this.current_map.FindEmptyTile());
        /*this.player.MoveTo(this.current_map.FindEmptyTile());*/
        
        Debug.Log("changing room");
        Debug.Log(this.current_map_node);
        Debug.Log(this.current_map);

    }

}
