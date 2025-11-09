using UnityEngine;

[CreateAssetMenu(fileName = "HealthUpgrade", menuName = "Ability/HealthUpgrade")]
public class HealthUpgrade : ScriptableObject, IAbility
{
    [SerializeField] private Sprite abilityToken;
    public Sprite AbilityToken => abilityToken;


    [SerializeField] private bool oneTimeEffect;
    public bool OneTimeEffect => oneTimeEffect;

    public Sprite EnemySprite;

    [SerializeField]
    public int heal_points;

    public void Activate(Entity entity, string move_direction)
    {
        Player player = entity as Player;
        player.Heal(heal_points);
        player.InitiateHealthUi();
    }

    public void Deactivate(Entity entity, string move_direction)
    {
        return; 

    }
}