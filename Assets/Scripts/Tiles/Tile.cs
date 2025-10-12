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
            transform.GetComponent<SpriteRenderer>().color = new Color(228f / 255f, 115f / 255f, 135f / 255f);
        }
        else
        {
            transform.GetComponent<SpriteRenderer>().color = new Color(178f / 255f, 82f / 255f, 102f / 255f);
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

    public bool RemoveWall(string wall)
    {
        if (wall == "up")
        {
            upper_wall.SetActive(false);
        }
        else if (wall == "down")
        {
            lower_wall.SetActive(false);
        }
        else if (wall == "left")
        {
            left_wall.SetActive(false);
        }
        else if (wall == "right")
        {
            right_wall.SetActive(false);
        }

        // Remove from the wall list if it exists
        if (tile_walls.Contains(wall))
        {
            tile_walls.Remove(wall);
            return true;
        }

        return false;
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
