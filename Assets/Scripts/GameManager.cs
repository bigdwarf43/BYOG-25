using UnityEngine;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{

    // Prefabs
    [SerializeField]
    GameObject Player;
    [SerializeField]
    GameObject GameMap;

    [SerializeField]
    GameObject WorldMap;

    // Map conf
    [Header("Map config")]
    [SerializeField]
    int grid_rows;
    [SerializeField]
    int grid_cols;
    [SerializeField]
    float cellSize;

    Player Player_obj;
    WorldMap world_map;

    // Touch parameters
    private Vector2 startTouchPos;
    private Vector2 endTouchPos;
    private bool swipeDetected;

    [SerializeField] private float minSwipeDistance = 50f; // in pixels


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.Player_obj = Instantiate(Player, transform).GetComponent<Player>();

        this.world_map = WorldMap.GetComponent<WorldMap>();
        this.world_map.Initialize(grid_rows, grid_cols, this.Player_obj);

        Tile random_tile = this.world_map.current_map.FindEmptyTile();

        this.Player_obj.Initialize(random_tile);
    }
    
    public void ManageTouch()
    {
        if (Input.touchCount > 0)
        {

            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPos = touch.position;
                    break;

                case TouchPhase.Ended:
                    endTouchPos = touch.position;
                    SwipeDetected(startTouchPos, endTouchPos);
                    break;
            }
        }

        // For testing in editor (mouse drag)
        if (Application.isEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                startTouchPos = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                endTouchPos = Input.mousePosition;
                SwipeDetected(startTouchPos, endTouchPos);
            }
        }
    }


    private void SwipeDetected(Vector2 start_touch, Vector2 end_touch)
    {
        Vector2 swipe = end_touch - start_touch;
        if (swipe.magnitude < minSwipeDistance) return;

        if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
        {
            // Horizontal swipe
            if (swipe.x > 0)
                ManageInput("right");
            else
                ManageInput("left");

        }
        else
        {
            // Vertical swipe
            if (swipe.y > 0)
                ManageInput("up");
            else
                ManageInput("down");
        }
    }

/*    public void ManageInput(string key)
    {

        if (this.Player_obj.CanMove)
        {
            int current_row = this.Player_obj.currentTile.row;
            int current_col = this.Player_obj.currentTile.col;

            if (key=="right")
            {

                if (this.world_map.current_map.IsMoveValid(current_row, current_col, "right")){
                    current_col += 1;
                    Tile targetTile = this.world_map.current_map.GetTileAt(current_row, current_col);
                    if (targetTile != null)
                    {
                        if (targetTile.occupant == null)
                        {
                            StartCoroutine(this.Player_obj.MoveTo(targetTile));
                        }
                        else if (targetTile.occupant is Enemy)
                        {
                            Player_obj.AttackEntity(targetTile);
                        }
                        if (!(targetTile is ExitTile))
                            this.world_map.current_map.Tick();

                    }
                }

            }
            else if (key == "left")
            {
                if (this.world_map.current_map.IsMoveValid(current_row, current_col, "left"))
                {
                    current_col -= 1;
                    Tile targetTile = this.world_map.current_map.GetTileAt(current_row, current_col);
                    if (targetTile != null)
                    {
                        if (targetTile.occupant == null)
                        {
                            StartCoroutine(this.Player_obj.MoveTo(targetTile));

                        }
                        else if (targetTile.occupant is Enemy)
                        {
                            Player_obj.AttackEntity(targetTile);
                        }
                        if (!(targetTile is ExitTile))
                            this.world_map.current_map.Tick();

                    }
                }
            }
            else if (key == "up")
            {

                if (this.world_map.current_map.IsMoveValid(current_row, current_col, "up"))
                {
                    current_row -= 1;
                    Tile targetTile = this.world_map.current_map.GetTileAt(current_row, current_col);
                    if (targetTile != null)
                    {
                        if (targetTile.occupant == null)
                        {
                            StartCoroutine(this.Player_obj.MoveTo(targetTile));

                        }
                        else if (targetTile.occupant is Enemy)
                        {
                            Player_obj.AttackEntity(targetTile);
                        }
                        if (!(targetTile is ExitTile))
                            this.world_map.current_map.Tick();

                    }
                }
            }
            else if (key == "down")
            {
                if (this.world_map.current_map.IsMoveValid(current_row, current_col, "down"))
                {
                    current_row += 1;
                    Tile targetTile = this.world_map.current_map.GetTileAt(current_row, current_col);
                    if (targetTile != null)
                    {
                        if (targetTile.occupant == null)
                        {
                            StartCoroutine(this.Player_obj.MoveTo(targetTile));

                        }
                        else if (targetTile.occupant is Enemy)
                        {
                            Player_obj.AttackEntity(targetTile);
                        }
                        if (!(targetTile is ExitTile))
                            this.world_map.current_map.Tick();

                    }
                }
            }
        }
    }*/

    public void ManageInput(string key)
    {
        if (!Player_obj.CanMove) return;

        int current_row = Player_obj.currentTile.row;
        int current_col = Player_obj.currentTile.col;

        // Define movement offsets for each direction
        var directionOffsets = new Dictionary<string, Vector2Int>
    {
        { "up", new Vector2Int(-1, 0) },
        { "down", new Vector2Int(1, 0) },
        { "left", new Vector2Int(0, -1) },
        { "right", new Vector2Int(0, 1) }
    };

        // If invalid input, ignore
        if (!directionOffsets.ContainsKey(key)) return;

        Vector2Int offset = directionOffsets[key];
        int newRow = current_row + offset.x;
        int newCol = current_col + offset.y;

        var map = world_map.current_map;

        if (!map.IsMoveValid(current_row, current_col, key))
            return;

        Tile targetTile = map.GetTileAt(newRow, newCol);
        if (targetTile == null) return;

        if (targetTile.occupant == null)
        {
            StartCoroutine(Player_obj.MoveTo(targetTile));
        }
        else if (targetTile.occupant is Enemy)
        {
            Player_obj.AttackEntity(targetTile);
        }

        if (!(targetTile is ExitTile))
        {
            map.Tick();
        }
    }


    // Update is called once per frame
    void Update()
    {

        ManageTouch();
    }
}
