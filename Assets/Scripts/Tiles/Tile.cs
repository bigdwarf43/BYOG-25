using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{

    public int row, col;
    
    public Entity occupant = null;
    private HashSet<string> tile_walls = new HashSet<string>();

    [SerializeField]
    public GameObject upper_wall, lower_wall, left_wall, right_wall;

    // What world this tile belongs to
    protected WorldMap world_map;

    public void Init(int _row, int _col, WorldMap world_map)
    {
        this.world_map = world_map;
        this.row  = _row;
        this.col  = _col;
        this.name = $"Tile ({this.row},{this.col})";

        if ((this.row + this.col) % 2 == 0)
        {
            transform.GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            transform.GetComponent<SpriteRenderer>().color = Color.green;
        }

    }

    public void AddWalls(string wall)
    {
            if (wall == "up")
            {
                upper_wall.SetActive(true);
            }
            else if (wall == "down")
            {
                lower_wall.SetActive(true);
            }
            else if (wall == "left")
            {
                left_wall.SetActive(true);
            }
            else if (wall == "right")
            {
                right_wall.SetActive(true);
            }

            tile_walls.Add(wall);


    }

    public bool IsDirectionValid(string direction)
    {
        if (tile_walls.Contains(direction))
        {
            return false;
        }
        return true;
    }

    public virtual void SteppedOn(Entity entity)
    {
        return;
    }
}
