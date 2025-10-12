using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour
{   
    public Tile currentTile;
    public Tile destinationTile; // Change this to move the entity

    [Header("Health System")]
    [SerializeField]
    protected int maxHealth = 5;
    [SerializeField]
    protected int currentHealth; 
    
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    
    [SerializeField]
    protected float moveSpeed = 0.2f;

    [SerializeField]
    protected int attack_damage = 2;

    [HideInInspector]
    public bool CanMove;

    [HideInInspector]
    public bool ignore_walls = false;
    public bool can_destroy_walls = false;


    [SerializeField]
    private ScriptableObject currentAbilityAsset; // assign in Inspector

    public IAbility currentAbility;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        this.destinationTile = this.currentTile;
        this.CanMove = true;

        if (currentAbilityAsset)
        {
            currentAbility = currentAbilityAsset as IAbility;
            currentAbility.Activate(this);
        }
      
    }

    public void Initialize(Tile initTile)
    {

        currentHealth = maxHealth;
        this.MoveToWithoutLerp(initTile);

    }

    public void MoveToWithoutLerp(Tile tile)
    {
        this.currentTile = tile;
        this.destinationTile = tile;
        this.transform.position = new Vector3(this.destinationTile.transform.position.x, this.destinationTile.transform.position.y, 0);
        this.currentTile.occupant = this;
    }

    public void MoveEntity(Tile dest)
    {
        StartCoroutine(MoveTo(dest));
    }
    public IEnumerator MoveTo(Tile dest)
    {
        this.CanMove = false; // prevent the entity from moving 

        if (dest == null) yield break;

        currentTile.occupant = null;
        destinationTile = dest;

        // do not block the exit tile with occupant
        if (!(destinationTile is ExitTile))
        {
            destinationTile.occupant = this;
        }

        Vector3 startPos = transform.position;
        Vector3 endPos = dest.transform.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / this.moveSpeed;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        transform.position = endPos;
        currentTile = destinationTile;

        this.CanMove = true; // allow the entity to move again
        destinationTile.SteppedOn(this);

    }


    protected virtual void HandleCollision(Entity otherEntity)
    {
        // Base collision handling - can be overridden by derived classes
    }
    

   
    // FIX: Add health management methods
    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    // FIX: Add healing method
    public virtual void Heal(int healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
    }
    
    // FIX: Add death handling method
    protected virtual void Die()
    {
        
        // Remove from current tile
        if (currentTile != null)
        {
            currentTile.occupant = null;
        }
        
        // Destroy the game object
        Destroy(gameObject);
    }


    public void AttackEntity(Tile target_tile)
    {

        if (target_tile != null && target_tile.occupant != null)
        {
            Entity entity = target_tile.occupant as Entity;
            if (entity != null)
            {
                // Deal damage
                entity.TakeDamage(this.attack_damage);


                int target_x = target_tile.col;
                int target_y = target_tile.row;

                // Calculate direction from enemy to target
                Vector2 direction = new Vector2(target_x - this.currentTile.col, target_y - this.currentTile.row);
                StartCoroutine(JerkTowards(direction));
            }
        }
    }

    public IEnumerator JerkTowards(Vector2 direction)
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

    }

    public void Update()
    { 
    }
}
