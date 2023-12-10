using System;
using UnityEngine;
using UnityEngine.UI;

public class HazardCreatureCardUI : CardUI
{
    [Header("Hazard Creature Card UI")]
    [Header("References")]
    [SerializeField]
    protected Image hurtIcon;
    [SerializeField]
    protected Image exhaustedIcon;
    [SerializeField]
    protected Image immovableIcon;
    [SerializeField]
    protected Image bleedingIcon;
    [SerializeField]
    protected Image poisonedIcon;
    [SerializeField]
    protected Image morgulIcon;
    [SerializeField]
    protected Image fireIcon;
    [SerializeField]
    protected Image iceIcon;
    [SerializeField]
    protected Image buffedIcon;
    [SerializeField]
    protected Image debuffedIcon;
    [SerializeField]
    protected Image blindIcon;


    public override bool Initialize(string cardId, NationsEnum owner, bool refresh = false)
    {
        if (!base.Initialize(cardId, owner, refresh))
            return false;

        initialized = false;

        hurtIcon.enabled = false;
        exhaustedIcon.enabled = false;
        poisonedIcon.enabled = false;
        morgulIcon.enabled = false;
        bleedingIcon.enabled = false;
        immovableIcon.enabled = false;
        fireIcon.enabled = false;
        iceIcon.enabled = false;
        buffedIcon.enabled = false;
        debuffedIcon.enabled = false;
        blindIcon.enabled = false;

        CheckStatusEffects();

        if (board.GetCardManager().GetCardUI(details) != null)
        {
            CardUI existingCardUI = board.GetCardManager().GetCardUI(details);
            if (existingCardUI != null)
            {
                if ((existingCardUI as HazardCreatureCardUI ) != null)
                {
                    HazardCreatureCardUI existingCreatureCardUI = existingCardUI as HazardCreatureCardUI;
                    hurtIcon.enabled = existingCreatureCardUI.GetHurtIcon().enabled;
                    exhaustedIcon.enabled = existingCreatureCardUI.GetExhaustedIcon().enabled;
                }
            }
        }

        initialized = true;

        return initialized;
    }

    public bool IsImmovable()
    {
        return immovableIcon.enabled;
    }

    public bool IsBleeding()
    {
        return bleedingIcon.enabled;
    }
    public bool IsBlind()
    {
        return blindIcon.enabled;
    }

    public bool IsPoisoned()
    {
        return poisonedIcon.enabled;
    }

    public bool IsMorgul()
    {
        return morgulIcon.enabled;
    }

    public bool IsIce()
    {
        return fireIcon.enabled;
    }

    public bool IsFire()
    {
        return iceIcon.enabled;
    }
    public bool IsHurt()
    {
        return hurtIcon.enabled;
    }
    public bool IsBuffed()
    {
        return buffedIcon.enabled;
    }
    public bool IsUnbuffed()
    {
        return debuffedIcon.enabled;
    }
    public bool IsExhausted()
    {
        return exhaustedIcon.enabled;
    }

    public void CheckStatusEffects()
    {
        if (board.GetCardManager().GetCardUI(details) != null)
        {
            CardUI existingCardUI = board.GetCardManager().GetCardUI(details);
            if (existingCardUI != null)
            {
                if (existingCardUI as HazardCreatureCardUI!= null)
                {
                    HazardCreatureCardUI existingHazardCreatureCardUI = existingCardUI as HazardCreatureCardUI;
                    bool value = existingHazardCreatureCardUI.IsHurt();
                    if (hurtIcon.enabled != value)
                    {
                        hurtIcon.enabled = value;
                        AddMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value ? "hurt" : "healed"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
                    }

                    value = existingHazardCreatureCardUI.IsExhausted();
                    if (exhaustedIcon.enabled != value)
                    {
                        exhaustedIcon.enabled = value;
                        AddMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value ? "exhausted" : "rested"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
                    }

                    value = existingHazardCreatureCardUI.IsPoisoned();
                    if (poisonedIcon.enabled != value)
                    {
                        poisonedIcon.enabled = value;
                        AddMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value ? "poisoned" : "Healed"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
                    }

                    value = existingHazardCreatureCardUI.IsMorgul();
                    if (morgulIcon.enabled != value)
                    {
                        morgulIcon.enabled = value;
                        AddMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value ? "morgul" : "healed"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
                    }


                    value = existingHazardCreatureCardUI.IsBleeding();
                    if (bleedingIcon.enabled != value)
                    {
                        bleedingIcon.enabled = value;
                        AddMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value ? "bleeding" : "healed"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
                    }

                    value = existingHazardCreatureCardUI.IsImmovable();
                    if (immovableIcon.enabled != value)
                    {
                        immovableIcon.enabled = value;
                        AddMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value ? "rooted" : "unrooted"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
                    }
                    value = existingHazardCreatureCardUI.IsFire();
                    if (fireIcon.enabled != value)
                    {
                        fireIcon.enabled = value;
                        AddMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value ? "burning" : "fire_put_away"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
                    }
                    value = existingHazardCreatureCardUI.IsIce();
                    if (iceIcon.enabled != value)
                    {
                        iceIcon.enabled = value;
                        AddMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value ? "frozen" : "heated"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
                    }
                    if (existingCardUI as CharacterCardUIBoard != null)
                    {
                        CharacterCardUIBoard existingCharacterCardUIBoard = existingCardUI as CharacterCardUIBoard;
                        value = board.IsHexBuffed(existingCharacterCardUIBoard.GetHex(), existingCharacterCardUIBoard.GetOwner());
                        if (buffedIcon.enabled != value)
                        {
                            buffedIcon.enabled = value;
                            AddMessage(
                                GameObject.Find("Localization").GetComponent<Localization>().Localize(value ? "buffed" : "unbuffed"),
                                1f,
                                colorManager.GetColor(value ? "failure" : "success"));
                        }
                        value = board.IsHexDebuffed(existingCharacterCardUIBoard.GetHex(), existingCharacterCardUIBoard.GetOwner());
                        if (debuffedIcon.enabled != value)
                        {
                            debuffedIcon.enabled = value;
                            AddMessage(
                                GameObject.Find("Localization").GetComponent<Localization>().Localize(value ? "debuffed" : "undebuffed"),
                                1f,
                                colorManager.GetColor(value ? "failure" : "success"));
                        }
                    }
                    value = existingHazardCreatureCardUI.IsBlind();
                    if (blindIcon.enabled != value)
                    {
                        blindIcon.enabled = value;
                        AddMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value ? "blinded" : "sees"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
                    }
                }
            }
        }
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
        int prowess;
        CharacterCardDetails charDetails = GetCharacterDetails();
        prowess = charDetails.GetProwess();

        if (IsHurt())
            prowess--;
        if (IsUnbuffed())
            prowess++;
        if (IsPoisoned())
            prowess--;
        if (IsFire())
            prowess -= 3;
        if (IsBleeding())
            prowess -= 2;

        return Math.Max(1, prowess);
    }

    public int GetTotalDefence()
    {
        int defence;
        CharacterCardDetails charDetails = GetCharacterDetails();
        defence = charDetails.GetDefence();

        if (IsHurt())
            defence--;
        if (IsUnbuffed())
            defence++;
        if (IsPoisoned())
            defence--;
        if (IsFire())
            defence -= 3;
        if (IsBleeding())
            defence -= 2;

        return Math.Max(1, defence);
    }

    public int GetTotalMovement()
    {
        if (IsImmovable())
            return 0;

        int movement;
        movement = MovementConstants.unitsMovement;

        if (IsIce())
            movement -= 4;
        if (IsBlind())
            movement -= 8;

        return Math.Max(MaxStats.minMovement, movement);
    }

    public void Hurt(HazardCreatureCardDetails cardDetails = null)
    {
        bool die = IsHurt();
        hurtIcon.enabled = true;
        AddMessage(
            GameObject.Find("Localization").GetComponent<Localization>().Localize("hurt"),
            0.5f,
            "hurt");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            HazardCreatureCardUI originalCharacter = originalCard as HazardCreatureCardUI;
            if(originalCharacter != null && originalCharacter != this)
                originalCharacter.Hurt();
        }
        if(cardDetails != null)
            Lose(cardDetails);
        if (die)
            Dies();
    }
    public void Exhausted(HazardCreatureCardDetails cardDetails = null)
    {
        exhaustedIcon.enabled = true;
        AddMessage(
            GameObject.Find("Localization").GetComponent<Localization>().Localize("exhausted"),
            0.5f,
            "exhausted");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            HazardCreatureCardUI originalCharacter = originalCard as HazardCreatureCardUI;
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Exhausted();
        }
        if (cardDetails != null)
            Lose(cardDetails);
    }

    public void Immovable()
    {
        immovableIcon.enabled = true;
        AddMessage(
            GameObject.Find("Localization").GetComponent<Localization>().Localize("exhausted"),
            0.5f,
            "immovable");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            HazardCreatureCardUI originalCharacter = originalCard as HazardCreatureCardUI;
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Immovable();
        }
    }
    public void Fire(HazardCreatureCardDetails cardDetails = null)
    {
        fireIcon.enabled = true;
        AddMessage(
            GameObject.Find("Localization").GetComponent<Localization>().Localize("fire"),
            0.5f,
            "fire");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            HazardCreatureCardUI originalCharacter = originalCard as HazardCreatureCardUI;
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Fire();
        }
    }
    public void Blind()
    {
        blindIcon.enabled = true;
        AddMessage(
            GameObject.Find("Localization").GetComponent<Localization>().Localize("blind"),
            0.5f,
            "blind");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            HazardCreatureCardUI originalCharacter = originalCard as HazardCreatureCardUI;
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Blind();
        }
    }
    public void Ice()
    {
        fireIcon.enabled = true;
        AddMessage(
            GameObject.Find("Localization").GetComponent<Localization>().Localize("ice"),
            0.5f,
            "ice");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            HazardCreatureCardUI originalCharacter = originalCard as HazardCreatureCardUI;
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Ice();
        }
    }
    public void Morgul()
    {
        morgulIcon.enabled = true;
        AddMessage(
            GameObject.Find("Localization").GetComponent<Localization>().Localize("morgul"),
            0.5f,
            "morgul");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            HazardCreatureCardUI originalCharacter = originalCard as HazardCreatureCardUI;
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Morgul();
        }
    }
    public void Poisoned()
    {
        poisonedIcon.enabled = true;
        AddMessage(
            GameObject.Find("Localization").GetComponent<Localization>().Localize("poisoned"),
            0.5f,
            "poisoned");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            HazardCreatureCardUI originalCharacter = originalCard as HazardCreatureCardUI;
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Poisoned();
        }
    }
    public void Bleeding()
    {
        bleedingIcon.enabled = true;
        AddMessage(
            GameObject.Find("Localization").GetComponent<Localization>().Localize("bleeding"),
            0.5f,
            "bleeding");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            HazardCreatureCardUI originalCharacter = originalCard as HazardCreatureCardUI;
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Bleeding();
        }
    }

    public void Heal()
    {
        hurtIcon.enabled = false;
        exhaustedIcon.enabled = false;
        AddMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("healed"), 0.5f, "healed");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            HazardCreatureCardUI originalCharacter = ((HazardCreatureCardUI)originalCard);
            if (originalCharacter != null && originalCharacter != this)
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
