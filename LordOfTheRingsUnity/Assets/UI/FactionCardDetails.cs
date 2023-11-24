using UnityEngine;

public class FactionCardDetails : CardDetails
{
    [Header("Faction Details")]
    public FactionsEnum faction;
    public FactionAbilities factionAbility;

    private bool isLoaded;

    void Awake()
    {
        isLoaded = false;
    }

    public bool Initialize()
    {
        isLoaded = Initialize(CardClass.Faction, new Resources(0, 0, 0, 0, 0, 0, 0, 0));
        return isLoaded;
    }
    public bool IsLoaded()
    {
        return isLoaded;
    }


    void Update()
    {
        if(!isLoaded)
            Initialize();
    }

    public FactionAbilities GetFactionAbility()
    {
        return factionAbility;
    }
}
