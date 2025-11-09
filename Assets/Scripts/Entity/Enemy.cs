using UnityEngine;
using System.Collections;
using static UnityEngine.GraphicsBuffer;


public class Enemy : Entity
{
    [Header("Ability setiings")]
    [SerializeField]
    ScriptableObject[] availableAbilities;

    [Header("Movement Settings")]
    [SerializeField]
    private int detectionRadius = 3; // Distance at which enemy starts moving towards player

    protected override void Start()
    {
        base.Start();
        
        if (availableAbilities.Length > 0)
        {
            ScriptableObject scriptableAbility = availableAbilities[Random.Range(0, availableAbilities.Length)];
            currentAbility = scriptableAbility as IAbility;
            currentAbility.Activate(this, null);
            var spriteField = scriptableAbility.GetType().GetField("EnemySprite");
            Sprite abilitySprite = spriteField.GetValue(scriptableAbility) as Sprite;
            transform.GetComponent<SpriteRenderer>().sprite = abilitySprite;
        }
    }


    public int GetDistanceToPlayer(Player player)
    {
        if (player == null || player.currentTile == null || currentTile == null) return int.MaxValue;
        
        int rowDistance = Mathf.Abs(currentTile.row - player.currentTile.row);
        int colDistance = Mathf.Abs(currentTile.col - player.currentTile.col);
        
        return rowDistance + colDistance; // Manhattan distance
    }
    
    // FIX: Add method to check if player is within detection radius
    public bool IsPlayerInRange(Player player)
    {
        return GetDistanceToPlayer(player) <= detectionRadius;
    }
    
    // FIX: Add method to get direction towards player
    public Vector2Int GetDirectionTowardsPlayer(Player player)
    {
        if (player == null || player.currentTile == null || currentTile == null) return Vector2Int.zero;
        
        int rowDiff = player.currentTile.row - currentTile.row;
        int colDiff = player.currentTile.col - currentTile.col;
        
        // Prioritize movement in the direction with larger difference
        if (Mathf.Abs(rowDiff) > Mathf.Abs(colDiff))
        {
            return new Vector2Int(rowDiff > 0 ? 1 : -1, 0); // Move vertically
        }
        else
        {
            return new Vector2Int(0, colDiff > 0 ? 1 : -1); // Move horizontally
        }
    }
    
    // OOP CONCEPT: Polymorphism - Override collision handling for enemy-specific behavior
    protected override void HandleCollision(Entity otherEntity)
    {
        if (otherEntity is Player)
        {
            // Enemy attacks player
            otherEntity.TakeDamage(attack_damage);
            
      
        }
    }
    
    // OOP CONCEPT: Polymorphism - Override death handling for enemy-specific behavior
    protected override void Die()
    {
        
        // Call base Die method to handle cleanup
        base.Die();
    }

    private void Update()
    {
    }
}
