using UnityEngine;

// Example ScriptableObject implementation
[CreateAssetMenu(fileName = "WallPhase", menuName = "Ability/WallPhase")]
public class WallPhase : ScriptableObject, IAbility
{
    [SerializeField] private Sprite abilityToken;
    public Sprite AbilityToken => abilityToken;

    [SerializeField] private bool oneTimeEffect;
    public bool OneTimeEffect => oneTimeEffect;

    public Sprite EnemySprite;
    public void Activate(Entity entity, string move_direction)
    {
        entity.ignore_walls = true;
        // You can start a coroutine or timer in the entity if needed.
    }

    public void Deactivate(Entity entity, string move_direction)
    {
        entity.ignore_walls = false;
    }
}

