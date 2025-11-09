using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

// Example ScriptableObject implementation
[CreateAssetMenu(fileName = "LaserAbility", menuName = "Ability/LaserAbility")]
public class LaserAbility : ScriptableObject, IAbility
{
    [SerializeField] private Sprite abilityToken;
    public Sprite AbilityToken => abilityToken;

    [SerializeField] private bool oneTimeEffect;
    public bool OneTimeEffect => oneTimeEffect;

    public Sprite EnemySprite;
    public void Activate(Entity entity, string move_direction)
    {
  
    }

    public void Deactivate(Entity entity, string move_direction)
    {
        List<Tile> tile_list = entity.currentTile.world_map.current_map.FindCrossTiles(entity.currentTile);

        foreach (Tile tile in tile_list)
        {
            if (tile == null) continue;

            tile.ChangeColor(Color.red);

            if (tile.occupant is Enemy enemy)
                enemy.TakeDamage(1);

            // Start coroutine to reset color
            entity.StartCoroutine(RevertTileColor(tile, 0.3f));
        }
    }

    /*    public void Deactivate(Entity entity, string move_direction)
        {
            if (move_direction!=null)
            {
                Tile moveTile = entity.currentTile.world_map.current_map.FindTileInDirection(entity.currentTile, move_direction);

                if (moveTile)
                {
                    List<Tile> tile_list = entity.currentTile.world_map.current_map.FindCrossTiles(moveTile);

                    foreach (Tile tile in tile_list)
                    {
                        if (tile == null) continue;

                        tile.ChangeColor(Color.red);

                        if (tile.occupant is Enemy enemy)
                            enemy.TakeDamage(1);

                        // Start coroutine to reset color
                        entity.StartCoroutine(RevertTileColor(tile, 0.3f));
                    }
                }
            }


        }*/
    private IEnumerator RevertTileColor(Tile tile, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (tile != null)
            tile.ResetColor();  // assuming your Tile class has a ResetColor() method
    }
}

