using UnityEngine;
using UnityEngine.Events;

public class ExitTile : Tile
{
    public Event on_exit_stepped;
    override public void SteppedOn(Entity entity)
    {
        if (entity is Player)
        {
            world_map.ChangeRoom(this.row, this.col);
            Debug.Log("Player stepped on exit");
        }
        return;
    }
}
