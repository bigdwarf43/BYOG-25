using UnityEngine;

public class RoomRenderer : MonoBehaviour
{
    public GameObject tilePrefab;
    public int gridRows = 9, gridCols = 9;
    public float tileSize = 5f;
    private TileController[,] tileObjects;

    public void RenderRoom(RoomData room)
    {
        if (tileObjects == null)
        {
            tileObjects = new TileController[gridRows, gridCols];
            for (int room_row = 0; room_row < gridRows; room_row++)
            {
                for (int room_col = 0; room_col < gridCols; room_col++)
                {
                    Vector3 pos = new Vector3((room_col * tileSize) + transform.position.x, room_row * tileSize * -1 + transform.position.y, 0);
                    var obj = Instantiate(tilePrefab,
                        pos,
                        Quaternion.identity, transform);

                    int index = room_row * gridCols + room_col;
                    if (index % 2 == 0)
                    {
                        obj.GetComponent<SpriteRenderer>().color = Color.white;
                    }
                    else
                    {
                        obj.GetComponent<SpriteRenderer>().color = Color.green;
                    }

                    tileObjects[room_row, room_col] = obj.GetComponent<TileController>();

                }
            }
        }



        // Update tile data
        for (int room_row = 0; room_row < gridRows; room_row++)
        {
            for (int room_col = 0; room_col < gridCols; room_col++)
            {
                tileObjects[room_row, room_col].Init(room.tiles[room_row, room_col]);
            }
        }
    }

    public void ChangeRoom(RoomData room_data)
    {
        // Update tile data
        for (int room_row = 0; room_row < gridRows; room_row++)
        {
            for (int room_col = 0; room_col < gridCols; room_col++)
            {
                tileObjects[room_row, room_col].Init(room_data.tiles[room_row, room_col]);
            }
        }
    }
}
