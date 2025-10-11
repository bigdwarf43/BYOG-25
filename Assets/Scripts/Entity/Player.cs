using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Player: Entity
{
    // FIX: Player-specific health settings
    [Header("Player Settings")]
    [SerializeField]
    private int attackDamage = 1;

    protected override void Start()
    {
        base.Start();
    }
    
    // OOP CONCEPT: Polymorphism - Override death handling for player-specific behavior
    protected override void Die()
    {
        Debug.Log("Game Over! Player has died!");
        // Could trigger game over screen or restart here
        
        // Call base Die method to handle cleanup
        base.Die();
    }

}
