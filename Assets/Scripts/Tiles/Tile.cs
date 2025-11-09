using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{

    public int row, col;
    
    public Entity occupant = null;
    private HashSet<string> tile_walls = new HashSet<string>();
    private Color originalColor;

    [SerializeField]
    public GameObject upper_wall, lower_wall, left_wall, right_wall;

    // What world this tile belongs to
    public WorldMap world_map;

    public void Init(int _row, int _col, WorldMap world_map)
    {
        this.world_map = world_map;
        this.row  = _row;
        this.col  = _col;
        this.name = $"Tile ({this.row},{this.col})";

        if ((this.row + this.col) % 2 == 0)
        {
            originalColor = new Color(228f / 255f, 115f / 255f, 135f / 255f);
            transform.GetComponent<SpriteRenderer>().color = originalColor;
        }
        else
        {
            originalColor = new Color(178f / 255f, 82f / 255f, 102f / 255f);
            transform.GetComponent<SpriteRenderer>().color = originalColor;
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

    public void ChangeColor(Color color)
    {
        Debug.Log("Changing color");
        transform.GetComponent<SpriteRenderer>().color = color;

    }

    public void ResetColor()
    {
        transform.GetComponent<SpriteRenderer>().color = originalColor;
    }
}
