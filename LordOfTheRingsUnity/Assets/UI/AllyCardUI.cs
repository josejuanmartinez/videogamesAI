using System;
using UnityEngine;
using UnityEngine.UI;

public class AllyCardUI : CardUI
{
    [Header("Ally Card UI")]
    [Header("References")]
    [SerializeField]
    protected Image hurtIcon;
    [SerializeField]
    protected Image exhaustedIcon;
    [SerializeField]
    protected Image bleedingIcon;
    [SerializeField]
    protected Image poisonedIcon;
    [SerializeField]
    protected Image morgulIcon;
    [Header("Stats")]
    [SerializeField]
    protected bool hurt;
    [SerializeField]
    protected bool exhausted;
    [SerializeField]
    protected bool bleeding;
    [SerializeField]
    protected bool poisoned;
    [SerializeField]
    protected bool morgul;
    [Header("Company")]
    [SerializeField]
    protected string inCompanyOf;

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

        CheckStatusEffects();

        initialized = true;

        return initialized;
    }
    public void CheckStatusEffects()
    {
        if (board.GetCardManager().GetCardUI(details) != null)
        {
            CardUI existingCardUI = board.GetCardManager().GetCardUI(details);
            if (existingCardUI != null)
            {
                if (existingCardUI as AllyCardUI != null)
                {
                    AllyCardUI existingCharacterCardUI = existingCardUI as AllyCardUI;
                    bool value = existingCharacterCardUI.GetHurtIcon().enabled;
                    if (hurtIcon.enabled != value)
                    {
                        hurtIcon.enabled = value;
                        AddMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value ? "hurt" : "healed"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
                    }

                    value = existingCharacterCardUI.GetExhaustedIcon().enabled;
                    if (exhaustedIcon.enabled != value)
                    {
                        exhaustedIcon.enabled = value;
                        AddMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value ? "exhausted" : "rested"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
                    }

                    value = existingCharacterCardUI.GetPoisonedIcon().enabled;
                    if (poisonedIcon.enabled != value)
                    {
                        poisonedIcon.enabled = value;
                        AddMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value ? "poisoned" : "Healed"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
                    }

                    value = existingCharacterCardUI.GetMorgulIcon().enabled;
                    if (morgulIcon.enabled != value)
                    {
                        morgulIcon.enabled = value;
                        AddMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value ? "morgul" : "healed"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
                    }


                    value = existingCharacterCardUI.GetBleedingIcon().enabled;
                    if (bleedingIcon.enabled != value)
                    {
                        bleedingIcon.enabled = value;
                        AddMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value ? "bleeding" : "healed"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
                    }

                }
            }
        }
    }

    public Image GetHurtIcon()
    {
        return hurtIcon;
    }

    public Image GetExhaustedIcon()
    {
        return exhaustedIcon;
    }
    public Image GetPoisonedIcon()
    {
        return poisonedIcon;
    }
    public Image GetBleedingIcon()
    {
        return bleedingIcon;
    }
    public Image GetImmovableIcon()
    {
        return bleedingIcon;
    }
    public Image GetMorgulIcon()
    {
        return morgulIcon;
    }

    public void SetCompanyLeader(string companyLeader)
    {
        inCompanyOf = companyLeader;
    }

    public void RemoveCompanyLeader()
    {
        inCompanyOf = null;
    }

    public bool IsInCompany()
    {
        return inCompanyOf != null;
    }

    public bool IsBleeding()
    {
        return bleeding;
    }

    public bool IsPoisoned()
    {
        return poisoned;
    }

    public bool IsMorgul()
    {
        return morgul;
    }

    public Color GetTotalProwessColor()
    {
        return Color.white;
    }

    public Color GetTotalDefenceColor()
    {
        return Color.white;
    }

    public int GetTotalProwess()
    {
        AllyCardDetails charDetails = GetAllyCardDetails();
        int prowess = charDetails.GetProwess();

        if (IsHurt() || IsPoisoned())
            prowess--;

        return Math.Max(1, prowess);
    }

    public int GetTotalDefence()
    {
        AllyCardDetails charDetails = GetAllyCardDetails();
        int defence = charDetails.GetDefence();

        if (IsHurt())
            defence--;
        if (IsPoisoned())
            defence--;

        return Math.Max(1, defence);
    }

    public int GetTotalMind()
    {
        AllyCardDetails charDetails = GetAllyCardDetails();
        int mind = charDetails.GetMind();

        if (IsMorgul())
            mind += 3;

        return Math.Min(1, mind);
    }
    public int GetTotalInfluence()
    {
        return 0;
    }

    public int GetTotalMovement()
    {
        int movement = MovementConstants.unitsMovement;

        if (morgul)
            movement -= 4;
        if (bleeding)
            movement -= 3;
        if (poisoned)
            movement -= 2;
        if (hurt)
            movement -= 1;

        return Math.Max(MaxStats.minMovement, movement);
    }

    public void Hurt()
    {
        hurt = true;
        hurtIcon.enabled = true;
        AddMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("hurt"), 1, "hurt");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            AllyCardUI originalCharacter = originalCard as AllyCardUI;
            if (originalCharacter != null)
                originalCharacter.Hurt();
        }
    }
    public void Exhausted()
    {
        exhausted = true;
        exhaustedIcon.enabled = true;
        AddMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("exhausted"), 1, "exhausted");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            AllyCardUI originalCharacter = originalCard as AllyCardUI;
            if (originalCharacter != null)
            {
                exhausted = true;
                exhaustedIcon.enabled = true;
            }
        }
    }
    public void Poisons()
    {
        poisoned = true;
        poisonedIcon.enabled = true;
        AddMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("poisoned"), 1, "poisons");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            AllyCardUI originalCharacter = originalCard as AllyCardUI;
            if (originalCharacter != null)
            {
                poisoned = true;
                poisonedIcon.enabled = true;
            }
        }
    }
    public void Bleeding()
    {
        bleeding = true;
        bleedingIcon.enabled = true;
        AddMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("bleeding"), 1, "bleeding");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            AllyCardUI originalCharacter = originalCard as AllyCardUI;
            if (originalCharacter != null)
            {
                bleeding = true;
                bleedingIcon.enabled = true;
            }
        }
    }
    public void Curses()
    {
        morgul = true;
        morgulIcon.enabled = true;
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        AddMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("cursed"), 1, "curses");
        if (originalCard != null)
        {
            AllyCardUI originalCharacter = originalCard as AllyCardUI;
            if (originalCharacter != null)
            {
                morgul = true;
                morgulIcon.enabled = true;
            }
        }
    }
    public void Heals()
    {
        hurt = false;
        exhausted = false;
        hurtIcon.enabled = false;
        exhaustedIcon.enabled = false;

        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            AllyCardUI originalCharacter = originalCard as AllyCardUI;
            if (originalCharacter != null)
            {
                hurt = false;
                exhausted = false;
                hurtIcon.enabled = false;
                exhaustedIcon.enabled = false;
                AddMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("healed"), 1, "success");
            }
        }
    }

    public void Won(HazardCreatureCardDetails details)
    {
        AddMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("won"), 1, "success");
        deckManager.AddToWonPile(owner, details);
    }
    public void Lose(HazardCreatureCardDetails details)
    {
        deckManager.AddToDiscardPile(owner, details);
    }
    public void Resisted(HazardCreatureCardDetails details)
    {
        AddMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("resisted"), 1, "resisted");
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
    
    public string GetInCompanyOf()
    {
        return inCompanyOf;
    }

    public void Dies()
    {
        inCompanyOf = "";
        CardUI card = board.GetCardManager().GetCardUI(details);
        if (card != null)
        {
            AllyCardUI original = card as AllyCardUI;
            if (original != null)
            {
                string leader = original.GetInCompanyOf();
                if (leader != null)
                {
                    CardUI leaderUI = board.GetCharacterManager().GetCharacterOfPlayer(card.GetOwner(), leader);
                    if(leaderUI != null)
                    {
                        CharacterCardUI charLeaderUI = leaderUI as CharacterCardUI;
                        if(charLeaderUI != null)
                        {
                            charLeaderUI.RemoveAlly(card.GetDetails());
                        }
                    }                        
                    AddMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("died"), 1, "failure");
                    DestroyImmediate(gameObject);
                }                
            }
        }
    }
}
