using UnityEngine;

public class FactionCardDetails : CardDetails
{
    [Header("Faction Details")]
    public FactionsEnum faction;
    public FactionAbilities factionAbility;

    void Awake()
    {
        base.Initialize(CardClass.Faction, new Resources(0, 0, 0, 0, 0, 0, 0, 0));
    }

    public void Initialize()
    {
        Awake();
    }

    public FactionAbilities GetFactionAbility()
    {
        return factionAbility;
    }
}
