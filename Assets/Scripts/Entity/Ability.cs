using UnityEngine;

// Interface for abilities
public interface IAbility
{
    Sprite AbilityToken { get; }
    bool OneTimeEffect { get; }

    void Activate(Entity entity, string move_direction);
    void Deactivate(Entity entity, string move_direction);
}