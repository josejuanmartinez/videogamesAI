using UnityEngine;
using UnityEngine.UI;

public class HazardEventCardUI : CardUI
{
    [Header("Hazard Event Card UI")]
    [SerializeField]
    protected Image eventTypeIcon;
    [SerializeField]
    protected Image hazardIcon;
    public override bool Initialize(string cardId, NationsEnum owner)
    {
        if (!base.Initialize(cardId, owner))
            return false;

        HazardEventCardDetails hazardEventDetails = (HazardEventCardDetails)details;
        if (hazardEventDetails != null)
            eventTypeIcon.enabled = hazardEventDetails.eventType == EventType.Immediate;
        else
            return false;

        eventTypeIcon.enabled = ((EventCardDetails) details).eventType == EventType.Immediate;

        return true;
    }
}
