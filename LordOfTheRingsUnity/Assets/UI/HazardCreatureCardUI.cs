using UnityEngine;
using UnityEngine.UI;

public class HazardCreatureCardUI : CardUI
{
    [Header("Hazard Creature Card UI")]
    [SerializeField]
    protected bool hurt;
    [SerializeField]
    protected bool exhausted; 
    [SerializeField]
    protected Image hurtIcon;
    [SerializeField]
    protected Image exhaustedIcon;

    public override bool Initialize(string cardId, NationsEnum owner)
    {
        if (!base.Initialize(cardId, owner))
            return false;

        hurtIcon.enabled = false;
        exhaustedIcon.enabled = false;

        if (board.GetCardManager().GetCardUI(details) != null)
        {
            CardUI existingCardUI = board.GetCardManager().GetCardUI(details);
            if (existingCardUI != null)
            {
                if ((HazardCreatureCardUI)existingCardUI != null)
                {   
                    HazardCreatureCardUI existingCreatureCardUI = (HazardCreatureCardUI)existingCardUI;
                    hurtIcon.enabled = existingCreatureCardUI.GetHurtIcon().enabled;
                    exhaustedIcon.enabled = existingCreatureCardUI.GetExhaustedIcon().enabled;
                }
            }
        }

        initialized = true;

        return true;
    }


    public Color GetTotalProwessColor()
    {
        if (GetHazardCreatureDetails() == null)
            return Color.white;
        if (GetTotalProwess() > GetHazardCreatureDetails().GetProwess())
            return colorManager.GetColor("success");
        else if (GetTotalProwess() < GetHazardCreatureDetails().GetProwess())
            return colorManager.GetColor("failure");
        else
            return Color.white;
    }

    public Color GetTotalDefenceColor()
    {
        if (GetTotalDefence() > GetHazardCreatureDetails().GetDefence())
            return colorManager.GetColor("success");
        else if (GetTotalDefence() < GetHazardCreatureDetails().GetDefence())
            return colorManager.GetColor("failure");
        else
            return Color.white;
    }

    public int GetTotalProwess()
    {
        HazardCreatureCardDetails hazardDetails = GetHazardCreatureDetails();
        int prowess = hazardDetails.prowess;

        //GET SOMETHING?

        // SUM PROWESS?
        return prowess;
    }

    public int GetTotalDefence()
    {
        HazardCreatureCardDetails hazardDetails = GetHazardCreatureDetails();
        int defence = hazardDetails.defence;

        //GET SOMETHING?

        // SUM PROWESS?
        return defence;
    }

    public void Hurt(HazardCreatureCardDetails cardDetails)
    {
        bool die = hurt;
        hurt = true;
        hurtIcon.enabled = true;
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            HazardCreatureCardUI originalCharacter = ((HazardCreatureCardUI)originalCard);
            originalCharacter.Hurt(cardDetails);
        }
        Lose(cardDetails);
        if (die)
            Dies();
    }
    public void Exhausted(HazardCreatureCardDetails cardDetails)
    {
        exhausted = true;
        exhaustedIcon.enabled = true;
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            HazardCreatureCardUI originalCharacter = ((HazardCreatureCardUI)originalCard);
            if (originalCharacter != null)
                originalCharacter.Exhausted(cardDetails);
        }
        Lose(cardDetails);
    }
    public void Heal()
    {
        hurt = false;
        exhausted = false;
        hurtIcon.enabled = false;
        exhaustedIcon.enabled = false;

        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            HazardCreatureCardUI originalCharacter = ((HazardCreatureCardUI)originalCard);
            if (originalCharacter != null)
                originalCharacter.Heal();
        }
    }

    public void Won(HazardCreatureCardDetails details)
    {
        AddMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("won"), 1, "success");
        AddMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("VP+")+ details.GetVictoryPoints(), 1, "success");
        deckManager.AddToWonPile(owner, details);
    }
    public void Lose(HazardCreatureCardDetails details)
    {
        deckManager.AddToDiscardPile(owner, details);
    }
    public bool IsHurt()
    {
        return hurt;
    }
    public bool IsExhausted()
    {
        return exhausted;
    }
    public Image GetHurtIcon()
    {
        return hurtIcon;
    }

    public Image GetExhaustedIcon()
    {
        return exhaustedIcon;
    }

    public void Dies()
    {
        CardUI card = board.GetCardManager().GetCardUI(details);
        if (card != null)
        {
            HazardCreatureCardUIBoard original = card as HazardCreatureCardUIBoard;
            if (original != null)
            {
                board.GetTile(original.GetHex()).RemoveCard(this);
                AddMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("died"), 1, "failure");
                DestroyImmediate(gameObject);
            }
        }
    }
}
