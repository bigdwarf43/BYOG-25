using UnityEngine;

public class RoomData
{
    public int row;
    public int col;
    public TileData[,] tiles;

    public RoomData(int rows, int cols)
    {
        tiles = new TileData[rows, cols];
    }
}
