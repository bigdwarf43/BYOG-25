using UnityEngine;
using System.Collections;
using static UnityEngine.GraphicsBuffer;

// OOP CONCEPT: Inheritance - Enemy inherits from Entity class
// OOP CONCEPT: Polymorphism - Enemy can be used wherever Entity is expected
public class Enemy : Entity
{
    
    // FIX: Add detection radius for player tracking
    [Header("Movement Settings")]
    [SerializeField]
    private int detectionRadius = 3; // Distance at which enemy starts moving towards player

    protected override void Start()
    {
        base.Start();
        currentHealth = maxHealth;
        Debug.Log($"Enemy spawned with {currentHealth} health");
    }

/*    public void AttackPlayer(Tile target_tile)
    {

        if (target_tile != null && target_tile.occupant != null)
        {
            Player player = target_tile.occupant as Player;
            if (player != null)
            {
                // Deal damage
                player.TakeDamage(this.attack_damage);

                Debug.Log($"{this.name} attacked {player.name} for {this.attack_damage} damage!");

                int target_x = target_tile.col;
                int target_y = target_tile.row;

                // Calculate direction from enemy to target
                Vector2 direction = new Vector2(target_x - this.currentTile.col, target_y - this.currentTile.row);
                StartCoroutine(JerkTowards(direction));
            }
        }
    }

    private IEnumerator JerkTowards(Vector2 direction)
    {
        Vector3 startPos = transform.position;
        Vector3 jerkPos = startPos + new Vector3(direction.x, -direction.y, 0) * 0.2f; // 0.2 = jerk distance

        float duration = 0.1f; // how fast the jerk happens
        float t = 0f;

        // Move forward
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(startPos, jerkPos, t);
            yield return null;
        }

        // Move back
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(jerkPos, startPos, t);
            yield return null;
        }

        transform.position = startPos; // ensure exact snap back

    }*/

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
            Debug.Log($"Enemy attacked player for {attack_damage} damage! Player health: {otherEntity.CurrentHealth}/{otherEntity.MaxHealth}");
            
      
        }
    }
    
    // OOP CONCEPT: Polymorphism - Override death handling for enemy-specific behavior
    protected override void Die()
    {
        Debug.Log($"Enemy defeated! +{maxHealth} experience points!");
        // Could add experience points, loot drops, etc. here
        
        // Call base Die method to handle cleanup
        base.Die();
    }

    private void Update()
    {
    }
}
