using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player: Entity
{
    [Header("Player Settings")]
    [SerializeField] private int attackDamage = 1;
    [SerializeField] ScriptableObject[] availableAbilities;


    [SerializeField] Queue<ScriptableObject> abilityQueue = new Queue<ScriptableObject>();

    public GameObject AbilitiesUi;


    protected override void Start()
    {
        base.Start();
    }

    protected override void Die()
    {
        Debug.Log("Game Over! Player has died!");
        base.Die();
    }

    public void Initialize(Tile initTile, GameObject abilitiesUiObj)
    {
        base.Initialize(initTile);
        AbilitiesUi = abilitiesUiObj;

        InitiateAbilities();

        
    }

    private void InitiateAbilities()
    {
        // Ensure at least 2 abilities in queue
        if (availableAbilities != null && availableAbilities.Length > 0)
        {
            AddRandomAbilityToQueue();
            AddRandomAbilityToQueue();
        }

        UpdateAllAbilityUI();
        currentAbility = abilityQueue.Peek() as IAbility;
        currentAbility.Activate(this);
    }



    public void MoveEntity(Tile dest)
    {
        base.MoveEntity(dest);
       
    }

    public void SwapAbilities()
    {
    
        Debug.Log("SWAP ABILITY");
        currentAbility?.Deactivate(this);

        ScriptableObject poppped_ability =  abilityQueue.Dequeue();
        AddRandomAbilityToQueue();

        currentAbility =  abilityQueue.Peek() as IAbility;
        currentAbility.Activate(this);

        UpdateAllAbilityUI();
    }

    private ScriptableObject AddRandomAbilityToQueue()
    {

        ScriptableObject abilityToAdd = availableAbilities.Length == 1
            ? availableAbilities[0]
            : availableAbilities[Random.Range(0, availableAbilities.Length)];

        abilityQueue.Enqueue(abilityToAdd);
        return abilityToAdd;
    }

    private void UpdateAllAbilityUI()
    {
        if (AbilitiesUi == null) return;

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

        var spriteField = abilityAsset.GetType().GetField("AbilityToken");
        Sprite tokenSprite = spriteField.GetValue(abilityAsset) as Sprite;
        tokenHolder.sprite = tokenSprite;
        
    }

    private void UpdatePlayerSprite()
    {
        ScriptableObject abilityAsset = abilityQueue.Peek();
        var spriteField = abilityAsset.GetType().GetField("AbilityToken");

        Sprite abilitySprite = spriteField.GetValue(abilityAsset) as Sprite;

        SpriteRenderer sr = transform.GetComponent<SpriteRenderer>();
        sr.sprite = abilitySprite;
        
    }
}
