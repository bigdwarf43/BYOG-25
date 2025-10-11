using UnityEngine;

public abstract class TileData
{
    public bool wall_up;
    public bool wall_down;
    public bool wall_left;
    public bool wall_right;

    public abstract void OnPlayerEnter(Player player);

}


public class FloorTile: TileData
{
    public override void OnPlayerEnter(Player player)
    {
        Debug.Log("stepped on");
    }
}
