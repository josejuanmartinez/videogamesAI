using System.Collections.Generic;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
    public GameObject eventPrefab;
    public GameObject hazardEventPrefab;
    public GameObject factionPrefab;
    public GameObject eventsLayout;

    public Dictionary<NationsEnum, List<EventAbilities>> eventsInPlay;
    public Dictionary<NationsEnum, List<HazardEventAbilities>> hazardEventsInPLay;
    public Dictionary<NationsEnum, List<FactionAbilities>> factionsInPlay;

    void Awake()
    {
        eventsInPlay = new Dictionary<NationsEnum, List<EventAbilities>>();
        hazardEventsInPLay = new Dictionary<NationsEnum, List<HazardEventAbilities>>();
        factionsInPlay = new Dictionary<NationsEnum, List<FactionAbilities>>();
    }

    public CardUI AddEvent(CardDetails cardDetails, NationsEnum owner)
    {
        if (cardDetails == null)
            return null;
        if (cardDetails.cardClass == CardClass.Event)
        {
            GameObject go = Instantiate(eventPrefab, eventsLayout.transform);
            go.name = cardDetails.cardId;
            EventCardUI eventUI = go.GetComponent<EventCardUI>();
            eventUI.Initialize(cardDetails.cardId, owner);
            EventCardDetails eventDetails= (EventCardDetails)cardDetails;
            if (eventDetails != null)
            {
                if (!eventsInPlay.ContainsKey(owner))
                    eventsInPlay.Add(owner, new List<EventAbilities>());
                eventsInPlay[owner].AddRange(eventDetails.abilities);
            }
            return eventUI;
        } 
        else if (cardDetails.cardClass == CardClass.HazardEvent)
        {
            GameObject go = Instantiate(hazardEventPrefab, eventsLayout.transform);
            go.name = cardDetails.cardId;
            HazardEventCardUI eventUI = go.GetComponent<HazardEventCardUI>();
            eventUI.Initialize(cardDetails.cardId, owner);
            HazardEventCardDetails eventDetails = (HazardEventCardDetails)cardDetails;
            if (eventDetails != null)
            {
                if (!hazardEventsInPLay.ContainsKey(owner))
                    hazardEventsInPLay.Add(owner, new List<HazardEventAbilities>());
                hazardEventsInPLay[owner].AddRange(eventDetails.abilities);
            }
            else
                return null;
            return eventUI;
        }
        return null;
    }
    public CardUI AddFaction(CardDetails cardDetails, NationsEnum owner)
    {
        if (cardDetails == null)
            return null;
        if (cardDetails.cardClass == CardClass.Faction)
        {
            GameObject go = Instantiate(factionPrefab, eventsLayout.transform);
            go.name = cardDetails.cardId;
            FactionCardUI eventUI = go.GetComponent<FactionCardUI>();
            eventUI.Initialize(cardDetails.cardId, owner);
            FactionCardDetails factionDetails = (FactionCardDetails)cardDetails;
            if (factionDetails != null)
            {
                if (!factionsInPlay.ContainsKey(owner))
                    factionsInPlay.Add(owner, new List<FactionAbilities>());
                factionsInPlay[owner].Add(factionDetails.factionAbility);
            }
            else 
                return null;
            return eventUI;
        }
        return null;
    }
    public void RemoveEvent(string cardId)
    {
        int iChildren = eventsLayout.transform.childCount;
        for (int i=0; i<iChildren; i++)
        {
            if(eventsLayout.transform.GetChild(i).gameObject.name == cardId)
            {
                DestroyImmediate(eventsLayout.transform.GetChild(i).gameObject);
                break;
            }                
        }
    }
    
    public bool IsEventInPlay(EventAbilities ability, NationsEnum owner) {
        if (!eventsInPlay.ContainsKey(owner))
            return false;
        return eventsInPlay[owner].Contains(ability);
    }
    public bool IsHazardEventInPlay(HazardEventAbilities ability, NationsEnum owner)
    {
        if (!hazardEventsInPLay.ContainsKey(owner))
            return false;
        return hazardEventsInPLay[owner].Contains(ability);
    }
}
