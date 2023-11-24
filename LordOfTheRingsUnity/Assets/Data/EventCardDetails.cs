using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventCardDetails : CardDetails
{
    [Header("Event Card Details")]
    public List<EventAbilities> abilities;
    public EventType eventType;
    
    [SerializeField]
    private string playableAtCity;

    private bool isLoaded;

    void Awake()
    {
        isLoaded = false;    
    }

    public bool Initialize()
    {
        isLoaded = Initialize(CardClass.Event, new Resources(0,0,0,0,0,0,0,0));
        return isLoaded;
    }

    public bool IsLoaded()
    {
        return isLoaded;
    }

    void Update()
    {
        if (!isLoaded)
            Initialize();
    }

    public List<string> GetEffectsStrings()
    {
        return abilities.Select(x => x.ToString()).ToList();
    }
}
