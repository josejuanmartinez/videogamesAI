using UnityEngine;
using UnityEngine.UI;

public class EventCardUI : CardUI
{
    [Header("Event Card UI")]
    [SerializeField]
    protected Image eventTypeIcon;
    public override bool Initialize(string cardId, NationsEnum owner)
    {
        if (!base.Initialize(cardId, owner))
            return false;

        initialized = false;

        EventCardDetails eventDetails = (EventCardDetails)details;
        if (eventDetails != null)
            eventTypeIcon.enabled = eventDetails.eventType == EventType.Immediate;
        else
            return false;

        eventTypeIcon.enabled = ((EventCardDetails)details).eventType == EventType.Immediate;

        initialized = true;
        return initialized;
    }
}
