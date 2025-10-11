using UnityEngine;

public class WorldData
{

    public int max_world_rows = 5;
    public int max_world_cols = 5;
    public RoomData[,] rooms;
    public int max_room_rows = 9;
    public int max_room_cols = 9;
    float wallProbability = 0.5f;


    public void Initialize()
    {
        rooms = new RoomData[max_world_rows, max_world_cols];
        for (int world_row = 0; world_row < max_world_rows; world_row++)
        {
            for (int world_col = 0; world_col < max_world_cols; world_col++)
            {
                var room = new RoomData(max_room_rows, max_room_cols);
                for (int room_row = 0; room_row < max_room_rows; room_row++)
                {
                    for (int room_col = 0; room_col < max_room_cols; room_col++)
                    {
                        var tile = new FloorTile();

                        if (room_row != 0 && room_col != 0 && room_col != max_room_cols - 1 && room_row != max_room_rows - 1)
                        {
                            if (Random.Range(0f, 1.0f) < wallProbability)
                            {
                                // Pick one random wall direction
                                int randomDir = Random.Range(0, 4); // 0 = up, 1 = down, 2 = left, 3 = right

                                switch (randomDir)
                                {
                                    case 0: tile.wall_up = true; break;
                                    case 1: tile.wall_down = true; break;
                                    case 2: tile.wall_left = true; break;
                                    case 3: tile.wall_right = true; break;
                                }
                            }
                            
                        }
                        

                        room.tiles[room_row, room_col] = tile;

                    }
                }



                rooms[world_row, world_col] = room;
            }
        }
    }
}
