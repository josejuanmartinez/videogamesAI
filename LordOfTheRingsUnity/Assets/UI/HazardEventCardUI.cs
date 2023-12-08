using UnityEngine;
using UnityEngine.UI;

public class HazardEventCardUI : CardUI
{
    [Header("Hazard Event Card UI")]
    [SerializeField]
    protected Image eventTypeIcon;
    [SerializeField]
    protected Image hazardIcon;
    public override bool Initialize(string cardId, NationsEnum owner, bool refresh = false)
    {
        if (!base.Initialize(cardId, owner, refresh))
            return false;

        initialized = false;

        HazardEventCardDetails hazardEventDetails = details as HazardEventCardDetails;
        if (hazardEventDetails != null)
            eventTypeIcon.enabled = hazardEventDetails.eventType == EventType.Immediate;
        else
            return false;

        eventTypeIcon.enabled = (details as EventCardDetails).eventType == EventType.Immediate;

        initialized = true;
        return initialized;
    }
}
