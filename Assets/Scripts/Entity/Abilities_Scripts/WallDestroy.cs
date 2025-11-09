using UnityEngine;

[CreateAssetMenu(fileName = "WallDestroy", menuName = "Ability/WallDestroy")]
public class WallDestroy : ScriptableObject, IAbility
{
    [SerializeField] private Sprite abilityToken;
    public Sprite AbilityToken => abilityToken;

    [SerializeField] private bool oneTimeEffect;
    public bool OneTimeEffect => oneTimeEffect;

    public Sprite EnemySprite;

    public void Activate(Entity entity, string move_direction)
    {
        entity.can_destroy_walls = true;
    }

    public void Deactivate(Entity entity, string move_direction)
    {
        entity.can_destroy_walls = false;

    }
}