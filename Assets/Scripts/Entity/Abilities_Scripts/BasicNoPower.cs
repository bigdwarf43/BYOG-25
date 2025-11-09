using UnityEngine;

[CreateAssetMenu(fileName = "BasicNoPower", menuName = "Ability/BasicNoPower")]
public class BasicNoPower : ScriptableObject, IAbility
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

    }
}
