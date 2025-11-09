using UnityEngine;

public class AbilityPickup : Entity
{

    public ScriptableObject ability_to_grant;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
    }

    public void HandleCollision(Player player)
    {
        Debug.Log("Ability granted");
        IAbility ability_interface = ability_to_grant as IAbility;

        if (ability_to_grant != null && !ability_interface.OneTimeEffect)
            player.availableAbilities.Add(ability_to_grant);
        else
            ability_interface.Activate(player, null);

        this.currentTile.occupant = null;
        Destroy(gameObject);
    }
}
