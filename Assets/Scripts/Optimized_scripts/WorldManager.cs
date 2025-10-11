using UnityEngine;

public class WorldManager : MonoBehaviour
{
    RoomRenderer room_renderer;
    WorldData world_map;

    [SerializeField]
    GameObject Player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.world_map = new WorldData();
        this.world_map.Initialize();

        this.room_renderer  = transform.GetComponent<RoomRenderer>();
        this.room_renderer.RenderRoom(this.world_map.rooms[0, 0]);


    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
            int randRow = Random.Range(0, this.world_map.max_world_rows); // max exclusive
            int randCol = Random.Range(0, this.world_map.max_world_cols);

            RoomData randomRoom = this.world_map.rooms[randRow, randCol];
            this.room_renderer.ChangeRoom(randomRoom);

        }
        
    }
}
