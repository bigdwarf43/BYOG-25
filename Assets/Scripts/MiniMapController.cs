using UnityEngine;
using UnityEngine.UI;

public class MiniMapController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] GameObject roomIconPrefab;

    int world_rows;
    int world_cols;

    public MapNode[,] nodes;

    private Image[,] roomIcons;

    public void Init(MapNode[,] worldNodes)
    {
        world_rows = worldNodes.GetLength(0);
        world_cols = worldNodes.GetLength(1);   


        nodes = worldNodes;
        roomIcons = new Image[world_rows, world_cols];

        // Clear existing icons
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        // Ensure GridLayoutGroup is configured (optional)
        GridLayoutGroup grid = GetComponent<GridLayoutGroup>();
        if (grid != null)
        {
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = world_cols;
        }

        // Generate icons
        for (int r = 0; r < world_rows; r++)
        {
            for (int c = 0; c < world_cols; c++)
            {
                GameObject iconObj = Instantiate(roomIconPrefab, transform);
                Image img = iconObj.GetComponent<Image>();
                img.enabled = false;
                roomIcons[r, c] = img;
            }
        }

        Debug.Log(roomIcons);
    }

    public void UpdateMinimap(MapNode[,] worldNodes, MapNode current_map)
    {
        for (int r = 0; r < world_rows; r++)
        {
            for (int c = 0; c < world_cols; c++)
            {
                MapNode node = worldNodes[r, c];
                if (node == null) continue;

                // Current room
                if (node == current_map)
                {
                    roomIcons[r, c].enabled = true;
                    roomIcons[r, c].color = Color.green;
                }
                // Explored rooms
                else if (node.explored) // assumes MapNode has a bool "explored" property
                {
                    roomIcons[r, c].enabled = true;
                    roomIcons[r, c].color = Color.white;
                }
                // Unexplored
                else
                {
                    continue;
                }
            }
        }
    }
}
