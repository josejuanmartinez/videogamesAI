using System;
using System.Collections.Generic;
using System.Linq;
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

    List <StatusEffectsApplied> effectsApplied;

    public override bool Initialize(string cardId, NationsEnum owner, bool refresh = false)
    {
        if (!base.Initialize(cardId, owner, refresh))
            return false;

        initialized = false;
        effectsApplied = new();

        hurtIcon.enabled = false;
        exhaustedIcon.enabled = false;
        immovableIcon.enabled = false;
        bleedingIcon.enabled = false;
        poisonedIcon.enabled = false;
        morgulIcon.enabled = false;
        fireIcon.enabled = false;
        iceIcon.enabled = false;
        blindIcon.enabled = false;
        buffedIcon.enabled = false;
        debuffedIcon.enabled = false;

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
        return effectsApplied.FindAll(x => x.effect == StatusEffect.IMMOVIBILITY).Count() > 0;
    }

    public bool IsBleeding()
    {
        return effectsApplied.FindAll(x => x.effect == StatusEffect.BLOOD).Count() > 0;
    }
    public bool IsBlind()
    {
        return effectsApplied.FindAll(x => x.effect == StatusEffect.BLIND).Count() > 0;
    }
    public bool IsTrapped()
    {
        return effectsApplied.FindAll(x => x.effect == StatusEffect.TRAP).Count() > 0;
    }

    public bool IsPoisoned()
    {
        return effectsApplied.FindAll(x => x.effect == StatusEffect.POISON).Count() > 0;
    }

    public bool IsMorgul()
    {
        return effectsApplied.FindAll(x => x.effect == StatusEffect.MORGUL).Count() > 0;
    }

    public bool IsIce()
    {
        return effectsApplied.FindAll(x => x.effect == StatusEffect.ICE).Count() > 0;
    }

    public bool IsFire()
    {
        return effectsApplied.FindAll(x => x.effect == StatusEffect.FIRE).Count() > 0;
    }
    public bool IsHurt()
    {
        return effectsApplied.FindAll(x => x.effect == StatusEffect.WOUND).Count() > 0;
    }
    public bool IsBuffed()
    {
        return effectsApplied.FindAll(x => x.effect == StatusEffect.BUFF).Count() > 0;
    }
    public bool IsDebuffed()
    {
        return effectsApplied.FindAll(x => x.effect == StatusEffect.DEBUFF).Count() > 0;
    }
    public bool IsExhausted()
    {
        return effectsApplied.FindAll(x => x.effect == StatusEffect.EXHAUSTATION).Count() > 0;
    }

    public void CheckIfBuffed(HazardCreatureCardUI creature)
    {
        HazardCreatureCardUIBoard creatureBoard = creature as HazardCreatureCardUIBoard;
        if (creatureBoard == null)
            return;
        
        effectsApplied = effectsApplied.FindAll(x => x.effect != StatusEffect.BUFF).ToList();
        if(board.IsHexBuffed(creatureBoard.GetHex(), creatureBoard.GetOwner()))
            effectsApplied.Add(new StatusEffectsApplied(StatusEffect.BUFF, turn.GetTurnNumber(), false));

        effectsApplied = effectsApplied.FindAll(x => x.effect != StatusEffect.DEBUFF).ToList();
        if (board.IsHexDebuffed(creatureBoard.GetHex(), creatureBoard.GetOwner()))
            effectsApplied.Add(new StatusEffectsApplied(StatusEffect.DEBUFF, turn.GetTurnNumber(), false));
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
                    CheckIfBuffed(existingHazardCreatureCardUI);
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
                    value = existingHazardCreatureCardUI.IsBuffed();
                    if (buffedIcon.enabled != value)
                    {
                        buffedIcon.enabled = value;
                        AddMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value ? "buffed" : "unbuffed"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
                    }
                    value = existingHazardCreatureCardUI.IsDebuffed();
                    if (debuffedIcon.enabled != value)
                    {
                        debuffedIcon.enabled = value;
                        AddMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value ? "debuffed" : "undebuffed"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
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
                    value = existingHazardCreatureCardUI.IsTrapped();
                    if (blindIcon.enabled != value)
                    {
                        blindIcon.enabled = value;
                        AddMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value ? "trapped" : "freed"),
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
        HazardCreatureCardDetails charDetails = GetHazardCreatureDetails();
        prowess = charDetails.GetProwess();

        if (IsHurt())
            prowess--;
        if (IsDebuffed())
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
        HazardCreatureCardDetails charDetails = GetHazardCreatureDetails();
        defence = charDetails.GetDefence();

        if (IsHurt())
            defence--;
        if (IsDebuffed())
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
        effectsApplied.Add(new StatusEffectsApplied(StatusEffect.WOUND, turn.GetTurnNumber(), true));
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
        CheckStatusEffects();
        if (cardDetails != null)
            Lose(cardDetails);
        if (die)
            Dies();
    }
    public void Exhausted(HazardCreatureCardDetails cardDetails = null)
    {
        effectsApplied.Add(new StatusEffectsApplied(StatusEffect.EXHAUSTATION, turn.GetTurnNumber(), true));
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
        CheckStatusEffects();
        if (cardDetails != null)
            Lose(cardDetails);
    }

    public void Trapped()
    {
        effectsApplied.Add(new StatusEffectsApplied(StatusEffect.TRAP, turn.GetTurnNumber(), true));
        AddMessage(
            GameObject.Find("Localization").GetComponent<Localization>().Localize("trapped"),
            0.5f,
            "trapped");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            HazardCreatureCardUI originalCharacter = originalCard as HazardCreatureCardUI;
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Trapped();
        }
        CheckStatusEffects();
    }
    public void Fire()
    {
        effectsApplied.Add(new StatusEffectsApplied(StatusEffect.FIRE, turn.GetTurnNumber(), true));
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
        CheckStatusEffects();
    }
    public void Blind()
    {
        effectsApplied.Add(new StatusEffectsApplied(StatusEffect.BLIND, turn.GetTurnNumber(), true));
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
        CheckStatusEffects();
    }
    public void Ice()
    {
        effectsApplied.Add(new StatusEffectsApplied(StatusEffect.ICE, turn.GetTurnNumber(), true));
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
        CheckStatusEffects();
    }
    public void Morgul()
    {
        effectsApplied.Add(new StatusEffectsApplied(StatusEffect.MORGUL, turn.GetTurnNumber(), false));
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
        CheckStatusEffects();
    }
    public void Poisoned()
    {
        effectsApplied.Add(new StatusEffectsApplied(StatusEffect.POISON, turn.GetTurnNumber(), true));
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
        CheckStatusEffects();
    }
    public void Bleeding()
    {
        effectsApplied.Add(new StatusEffectsApplied(StatusEffect.BLOOD, turn.GetTurnNumber(), false));
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
        CheckStatusEffects();
    }

    public void Heal()
    {
        effectsApplied = effectsApplied.FindAll(x => !x.healable).ToList();

        AddMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("healed"), 0.5f, "healed");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            HazardCreatureCardUI originalCharacter = ((HazardCreatureCardUI)originalCard);
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Heal();
        }
        CheckStatusEffects();
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
    public List<StatusEffectsApplied> GetEffects()
    {
        return effectsApplied;
    }
    public void SetEffects(List<StatusEffectsApplied> newEffects)
    {
        effectsApplied = newEffects;
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
