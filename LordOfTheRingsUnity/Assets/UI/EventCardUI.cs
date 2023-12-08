using UnityEngine;
using UnityEngine.UI;

public class EventCardUI : CardUI
{
    [Header("Event Card UI")]
    [SerializeField]
    protected Image eventTypeIcon;
    public override bool Initialize(string cardId, NationsEnum owner, bool refresh = false)
    {
        if (!base.Initialize(cardId, owner, refresh))
            return false;

        initialized = false;

        EventCardDetails eventDetails = details as EventCardDetails;
        if (eventDetails != null)
            eventTypeIcon.enabled = eventDetails.eventType == EventType.Immediate;
        else
            return false;

        eventTypeIcon.enabled = (details as EventCardDetails).eventType == EventType.Immediate;

        initialized = true;
        return initialized;
    }
}
