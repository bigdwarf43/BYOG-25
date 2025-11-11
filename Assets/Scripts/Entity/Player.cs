using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class Player: Entity
{
    [Header("Player Settings")]
    [SerializeField] private int attackDamage = 1;
    [SerializeField] public List<ScriptableObject> availableAbilities = new List<ScriptableObject>();


    [SerializeField] Queue<ScriptableObject> abilityQueue = new Queue<ScriptableObject>();
    public GameObject AbilitiesUi;
    public GameObject PlayerHealthUi;
    public GameObject HeartPrefab;

    public static event Action OnPlayerDead;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Die()
    {
        base.Die();
        // Invoke the event
        OnPlayerDead?.Invoke();
    }

    public void AttackEntity(Tile target_tile)
    {
        AudioManager.PlaySfx("attack");
        base.AttackEntity(target_tile);
    }

    public override void TakeDamage(int damage)
    {
        int hearts_to_remove = Mathf.Min(damage, PlayerHealthUi.transform.childCount);
        Debug.Log("hearts to remove " + hearts_to_remove.ToString());

        int i = 0;
        foreach (Transform heart in PlayerHealthUi.transform)
        {
            if (i == hearts_to_remove) break;
            if(heart.gameObject.active == true)
            {
                heart.gameObject.SetActive(false);
                i += 1;
            }
            
        }

        base.TakeDamage(damage);
    }


    public void Initialize(Tile initTile, GameObject abilitiesUiObj, GameObject healthUi, GameObject heartPrefab)
    {
        base.Initialize(initTile);
        AbilitiesUi = abilitiesUiObj;
        PlayerHealthUi = healthUi;
        HeartPrefab = heartPrefab;

        InitiateHealthUi();
        InitiateAbilities();

        
    }

    public void InitiateHealthUi()
    {
        foreach(Transform heart in PlayerHealthUi.transform)
        {
            Destroy(heart.gameObject);
        }

        for (int i = 0; i < currentHealth; i++)
        {
            Instantiate(HeartPrefab, PlayerHealthUi.transform);
        }
    }

    private void InitiateAbilities()
    {
        // Ensure at least 2 abilities in queue
        if (availableAbilities != null && availableAbilities.Count > 0)
        {
            AddRandomAbilityToQueue();
            AddRandomAbilityToQueue();
        }

        UpdateAllAbilityUI();
        currentAbility = abilityQueue.Peek() as IAbility;
        currentAbility.Activate(this, null );
    }



    public void MoveEntity(Tile dest)
    {
        base.MoveEntity(dest);
       
    }

    public void SwapAbilities(string move_direction)
    {
    
        currentAbility?.Deactivate(this, move_direction);

        ScriptableObject poppped_ability =  abilityQueue.Dequeue();
        AddRandomAbilityToQueue();

        currentAbility =  abilityQueue.Peek() as IAbility;
        currentAbility.Activate(this, move_direction);

        UpdateAllAbilityUI();
    }

    private ScriptableObject AddRandomAbilityToQueue()
    {

        ScriptableObject abilityToAdd = availableAbilities.Count == 1
            ? availableAbilities[0]
            : availableAbilities[UnityEngine.Random.Range(0, availableAbilities.Count)];

        abilityQueue.Enqueue(abilityToAdd);
        return abilityToAdd;
    }

    private void UpdateAllAbilityUI()
    {
        if (AbilitiesUi == null) return;

        if (availableAbilities.Count == 1)
        {
            AbilitiesUi.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            AbilitiesUi.transform.GetChild(1).gameObject.SetActive(true);

        }

        int i = 0;
        foreach (ScriptableObject ability in abilityQueue)
        {
            UpdateAbilityTokenUI(i, ability);
            i += 1;

        }


        UpdatePlayerSprite();


    }

    private void UpdateAbilityTokenUI(int childIndex, ScriptableObject abilityAsset)
    {

        Image tokenHolder = AbilitiesUi.transform.GetChild(childIndex).GetComponent<Image>();
        if (tokenHolder == null || abilityAsset == null) return;

        // Cast to IAbility so we can access AbilityToken
        IAbility ability = abilityAsset as IAbility;
        if (ability != null)
        {
            tokenHolder.sprite = ability.AbilityToken;
        }
        else
        {
            Debug.LogWarning($"Ability asset at index {childIndex} does not implement IAbility.");
        }


    }

    private void UpdatePlayerSprite()
    {
        if (abilityQueue == null || abilityQueue.Count == 0)
            return;

        // Get the next ability
        ScriptableObject abilityAsset = abilityQueue.Peek();

        // Cast to IAbility to access the AbilityToken property
        IAbility ability = abilityAsset as IAbility;
        if (ability == null)
        {
            Debug.LogWarning("Ability asset does not implement IAbility!");
            return;
        }

        // Assign the ability’s token sprite to the player sprite
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = ability.AbilityToken;
        }
    }
}
