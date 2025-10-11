using UnityEngine;

public class TileController : MonoBehaviour
{
    public TileData data;

    public GameObject wall_up;
    public GameObject wall_down;
    public GameObject wall_right;
    public GameObject wall_left;


    public void Init(TileData tileData)
    {
        data = tileData;

        if (tileData.wall_up)
        {
            wall_up.SetActive(true);
        }
        else
        {
            wall_up.SetActive(false);

        }


        if (tileData.wall_right)
        {
            wall_right.SetActive(true);
        }
        else
        {
            wall_right.SetActive(false);

        }

        if (tileData.wall_left)
        {
            wall_left.SetActive(true);  
        }

        else
        {
            wall_left.SetActive(false);
        }


        if (tileData.wall_down)
        {
            wall_down.SetActive(true);
        }
        else
        {
            wall_down.SetActive(false);
        }
    }

    public void OnPlayerEnter(Player player)
    {
        data.OnPlayerEnter(player);
    }
}