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

    void Awake()
    {
        base.Initialize(CardClass.Event, new Resources(0,0,0,0,0,0,0,0));
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
