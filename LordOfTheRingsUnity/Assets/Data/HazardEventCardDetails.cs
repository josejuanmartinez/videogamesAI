using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HazardEventCardDetails : CardDetails
{
    [Header("Hazard Event Card Details")]
    public List<HazardEventAbilities> abilities;
    public EventType eventType;

    void Awake()
    {
        base.Initialize(CardClass.HazardEvent, new Resources(0, 0, 0, 0, 0, 0, 0, 0));
    }
    public void Initialize()
    {
        Awake();
    }
    public List<string> GetEffectsStrings()
    {
        return abilities.Select(x => x.ToString()).ToList();
    }
}
