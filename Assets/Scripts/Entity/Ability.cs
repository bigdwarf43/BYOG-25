using UnityEngine;

// Interface for abilities
public interface IAbility
{
    void Activate(Entity entity);

    void Deactivate(Entity entity);
}

// Example ScriptableObject implementation
[CreateAssetMenu(menuName = "Ability/WallPhase")]
public class WallPhase : ScriptableObject, IAbility
{
    public Sprite AbilityToken;
    public Sprite EnemySprite;
    public void Activate(Entity entity)
    {
        entity.ignore_walls = true;
        // You can start a coroutine or timer in the entity if needed.
    }

    public void Deactivate(Entity entity)
    {
        entity.ignore_walls = false;
    }
}


// Example ScriptableObject implementation
[CreateAssetMenu(menuName = "Ability/WallDestroy")]
public class WallDestroy : ScriptableObject, IAbility
{
    public Sprite AbilityToken;
    public Sprite EnemySprite;

    public void Activate(Entity entity)
    {
        entity.can_destroy_walls = true;
    }

    public void Deactivate(Entity entity)
    {
        entity.can_destroy_walls = false;

    }
}

[CreateAssetMenu(menuName = "Ability/BasicNoPower")]
public class BasicNoPower : ScriptableObject, IAbility
{
    public Sprite AbilityToken;
    public Sprite EnemySprite;

    public void Activate(Entity entity)
    {
    }

    public void Deactivate(Entity entity)
    {

    }
}