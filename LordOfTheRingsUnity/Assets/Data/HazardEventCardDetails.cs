using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HazardEventCardDetails : CardDetails
{
    [Header("Hazard Event Card Details")]
    public List<HazardEventAbilities> abilities;
    public EventType eventType;

    private bool isLoaded;

    void Awake()
    {
        isLoaded = false;    
    }

    public bool Initialize()
    {
        isLoaded = Initialize(CardClass.HazardEvent, new Resources(0, 0, 0, 0, 0, 0, 0, 0));
        return isLoaded;
    }
    public bool IsLoaded()
    {
        return isLoaded;
    }

    void Update()
    {
        if(!IsLoaded())
            Initialize();
    }
    public List<string> GetEffectsStrings()
    {
        return abilities.Select(x => x.ToString()).ToList();
    }
}
