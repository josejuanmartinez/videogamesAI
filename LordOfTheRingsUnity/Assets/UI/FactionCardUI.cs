using UnityEngine;
using UnityEngine.UI;

public class FactionCardUI : CardUI
{
    [Header("Faction Card UI")]
    [SerializeField]
    protected Image factionTypeIcon;
    public override bool Initialize(string cardId, NationsEnum owner)
    {
        if (!base.Initialize(cardId, owner))
            return false;
        
        initialized = false;

        FactionCardDetails factionDetails = (FactionCardDetails)details;
        if (factionDetails != null)
            factionTypeIcon.sprite = spritesRepo.GetSprite(factionDetails.factionAbility.ToString());
        else
            return false;

        initialized = true;

        return initialized;
    }
}
