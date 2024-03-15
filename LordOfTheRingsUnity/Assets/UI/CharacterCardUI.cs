using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class CharacterCardUI : CardUI
{
    [Header("Character Card UI")]
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
    protected Image blindIcon;
    [Header("Company")]
    [SerializeField]
    protected string inCompanyOf;
    [SerializeField]
    protected List<CardDetails> objects;
    [SerializeField]
    protected List<CardDetails> goldRings;
    [SerializeField]
    protected List<CardDetails> allies;

    private AudioResource voice;

    protected CardUI potentialLeader;

    protected short turnBleeding;
    protected short turnPoisoned;
    protected short turnFire;
    protected short turnIce;
    protected short turnBlind;
    protected short turnImmovable;

    List<StatusEffectsApplied> effectsApplied;

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
        morgulIcon.enabled = false  ;
        fireIcon.enabled = false;
        iceIcon.enabled = false;
        blindIcon.enabled = false;

        objects = new();
        allies = new();

        CheckStatusEffects();
        GetObjectsAndAllies();

        voice = audioRepo.GetVoice(GetCharacterDetails().race, Nations.alignments[owner], GetCharacterDetails().isFemale);

        initialized = true;

        return initialized;
    }

    public AudioResource GetVoice()
    {
        return voice;
    }

    public void GetObjectsAndAllies()
    {
        if (board.GetCardManager().GetCardUI(details) != null)
        {
            CardUI existingCardUI = board.GetCardManager().GetCardUI(details);
            if (existingCardUI != null)
            {
                if (existingCardUI as CharacterCardUI != null)
                {
                    CharacterCardUI existingCharacterCardUI = existingCardUI as CharacterCardUI;

                    objects = existingCharacterCardUI.objects;
                    allies = existingCharacterCardUI.allies;
                }
            }
        }
    }
    public void CheckIfBuffed(CharacterCardUI character)
    {
        CharacterCardUIBoard characterBoard = character as CharacterCardUIBoard;
        if (characterBoard == null)
            return;

        effectsApplied = effectsApplied.FindAll(x => x.effect != StatusEffect.BUFF).ToList();
        if (board.IsHexBuffed(characterBoard.GetHex(), characterBoard.GetOwner()))
            effectsApplied.Add(new StatusEffectsApplied(StatusEffect.BUFF, turn.GetTurnNumber(), false));

        effectsApplied = effectsApplied.FindAll(x => x.effect != StatusEffect.DEBUFF).ToList();
        if (board.IsHexDebuffed(characterBoard.GetHex(), characterBoard.GetOwner()))
            effectsApplied.Add(new StatusEffectsApplied(StatusEffect.DEBUFF, turn.GetTurnNumber(), false));
    }

    public void CheckStatusEffects()
    {
        if (board.GetCardManager().GetCardUI(details) != null)
        {
            CardUI existingCardUI = board.GetCardManager().GetCardUI(details);
            if (existingCardUI != null)
            {
                if (existingCardUI as CharacterCardUI != null)
                {
                    CharacterCardUI existingCharacterCardUI = existingCardUI as CharacterCardUI;
                    bool value = existingCharacterCardUI.IsHurt();
                    if (hurtIcon.enabled != value)
                    {
                        hurtIcon.enabled = value;
                        AddMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value ? "hurt" : "healed"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
                    }

                    value = existingCharacterCardUI.IsExhausted();
                    if (exhaustedIcon.enabled != value)
                    {
                        exhaustedIcon.enabled = value;
                        AddMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value ? "exhausted" : "rested"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
                    }

                    value = existingCharacterCardUI.IsPoisoned();
                    if (poisonedIcon.enabled != value)
                    {
                        poisonedIcon.enabled = value;
                        AddMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value ? "poisoned" : "Healed"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
                    }

                    value = existingCharacterCardUI.IsMorgul();
                    if (morgulIcon.enabled != value)
                    {
                        morgulIcon.enabled = value;
                        AddMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value ? "morgul" : "healed"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
                    }


                    value = existingCharacterCardUI.IsBleeding();
                    if (bleedingIcon.enabled != value)
                    {
                        bleedingIcon.enabled = value;
                        AddMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value ? "bleeding" : "healed"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
                    }

                    value = existingCharacterCardUI.IsImmovable();
                    if (immovableIcon.enabled != value)
                    {
                        immovableIcon.enabled = value;
                        AddMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value ? "rooted" : "unrooted"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
                    }
                    value = existingCharacterCardUI.IsFire();
                    if (fireIcon.enabled != value)
                    {
                        fireIcon.enabled = value;
                        AddMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value ? "burning" : "fire_put_away"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
                    }
                    value = existingCharacterCardUI.IsIce();
                    if (iceIcon.enabled != value)
                    {
                        iceIcon.enabled = value;
                        AddMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value ? "frozen" : "heated"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
                    }
                    value = existingCharacterCardUI.IsBlind();
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

    public void RemoveAlly(CardDetails ally)
    {
        if (allies.Contains(ally))
            allies.Remove(ally);
    }

    public bool IsInCompany()
    {
        return inCompanyOf != null;
    }

    public bool IsImmovable()
    {
        if ((details as CharacterCardDetails).isImmovable)
            return !eventsManager.IsEventInPlay(EventAbilities.CanMove, turn.GetCurrentPlayer());
        else
            return effectsApplied.Select(x => x.effect == StatusEffect.IMMOVIBILITY).Count() > 0;
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
        return effectsApplied.FindAll(x => x.effect == StatusEffect.POISON).Count() > 0; ;
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
    public bool IsExhausted()
    {
        return effectsApplied.FindAll(x => x.effect == StatusEffect.EXHAUSTATION).Count() > 0;
    }
    public Color GetTotalProwessColor()
    {
        if (GetTotalProwess() > GetCharacterDetails().GetProwess())
            return colorManager.GetColor("success");
        else if (GetTotalProwess() < GetCharacterDetails().GetProwess())
            return colorManager.GetColor("failure");
        else
            return Color.white;                
    }

    public Color GetTotalDefenceColor()
    {
        if (GetTotalDefence() > GetCharacterDetails().GetDefence())
            return colorManager.GetColor("success");
        else if (GetTotalDefence() < GetCharacterDetails().GetDefence())
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
        if (IsPoisoned())
            prowess--;
        if (IsFire())
            prowess -= 3;
        if (IsBleeding())
            prowess -= 2;

        //GET OBJECTS OF THE CHAR
        foreach (CardDetails cardDetail in objects)
        {
            if(cardDetail.cardClass == CardClass.Object)
            {
                ObjectCardDetails objectDetails = cardDetail as ObjectCardDetails;
                if (objectDetails != null)
                {
                    prowess = Math.Min(
                        prowess + objectDetails.prowess,
                        MaxStats.maxProwess
                    );
                }
            } 
            else if (cardDetail.cardClass == CardClass.Ring)
            {
                RingCardDetails objectDetails = cardDetail as RingCardDetails;
                if (objectDetails != null)
                {
                    prowess = Math.Min(
                        prowess + objectDetails.prowess,
                        MaxStats.maxProwess
                    );
                }
            }
        }
        return Math.Max(1, prowess);
    }

    public int GetTotalDefence()
    {
        int defence;
        CharacterCardDetails charDetails = GetCharacterDetails();
        defence = charDetails.GetDefence();

        if (IsHurt())
            defence--;
        if (IsPoisoned())
            defence--;
        if (IsFire())
            defence -= 3;
        if (IsBleeding())
            defence -= 2;

        //GET OBJECTS OF THE CHAR
        foreach (CardDetails cardDetail in objects)
        {
            if (cardDetail.cardClass == CardClass.Object)
            {
                ObjectCardDetails objectDetails = cardDetail as ObjectCardDetails;
                if (objectDetails != null)
                {
                    defence = Math.Min(
                        defence + objectDetails.defence,
                        MaxStats.maxDefence
                    );
                }
            }
            else if (cardDetail.cardClass == CardClass.Ring)
            {
                RingCardDetails objectDetails = cardDetail as RingCardDetails;
                if (objectDetails != null)
                {
                    defence = Math.Min(
                        defence + objectDetails.defence,
                        MaxStats.maxDefence
                    );
                }
            }
        }
        return Math.Max(1, defence);
    }


    public int GetTotalMind()
    {
        int mind;
        CharacterCardDetails charDetails = GetCharacterDetails();
        mind = charDetails.GetMind();

        if (IsMorgul())
            mind += 3;

        //GET OBJECTS OF THE CHAR
        foreach (CardDetails cardDetail in objects)
        {
            if (cardDetail.cardClass == CardClass.Object)
            {
                ObjectCardDetails objectDetails = cardDetail as ObjectCardDetails;
                if (objectDetails != null)
                {
                    mind = Math.Max(
                        mind - objectDetails.mind,
                        MaxStats.mindMind
                    );
                }
            }
            else if (cardDetail.cardClass == CardClass.Ring)
            {
                RingCardDetails objectDetails = cardDetail as RingCardDetails;
                if (objectDetails != null)
                {
                    mind = Math.Max(
                        mind - objectDetails.mind,
                        MaxStats.mindMind
                    );
                }
            }
        }
        return Math.Max(1, mind);
    }
    public int GetTotalInfluence()
    {
        int influence;
        CharacterCardDetails charDetails = GetCharacterDetails();
        influence = charDetails.GetInfluence();

        if (IsMorgul())
            influence = 0;

        //GET OBJECTS OF THE CHAR
        foreach (CardDetails cardDetail in objects)
        {
            if (cardDetail.cardClass == CardClass.Object)
            {
                ObjectCardDetails objectDetails = cardDetail as ObjectCardDetails;
                if (objectDetails != null)
                {
                    influence = Math.Min(
                        influence + objectDetails.influence,
                        MaxStats.maxInfluence
                    );
                }
            }
            else if (cardDetail.cardClass == CardClass.Ring)
            {
                RingCardDetails objectDetails = cardDetail as RingCardDetails;
                if (objectDetails != null)
                {
                    influence = Math.Min(
                        influence + objectDetails.influence,
                        MaxStats.maxInfluence
                    );
                }
            }
        }
        return Math.Max(0, influence);
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

        //GET OBJECTS OF THE CHAR
        foreach (CardDetails cardDetail in objects)
        {
            if (cardDetail.cardClass == CardClass.Object)
            {
                ObjectCardDetails objectDetails = cardDetail as ObjectCardDetails;
                if (objectDetails != null)
                {
                    movement = Math.Min(
                        movement + objectDetails.movement,
                        MaxStats.maxMovement
                    );
                }
            }
        }
        return Math.Max(MaxStats.minMovement, movement);
    }

    public void Hurt(HazardCreatureCardDetails attackerDetails, bool cascaded = false)
    {
        bool die = IsHurt();
        effectsApplied.Add(new StatusEffectsApplied(StatusEffect.WOUND, turn.GetTurnNumber(), true));
        if(!cascaded)
            AddMessage(
                GameObject.Find("Localization").GetComponent<Localization>().Localize("hurt"),
                0.5f,
                "hurt");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            CharacterCardUI originalCharacter = originalCard as CharacterCardUI;
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Hurt(attackerDetails, true);
        }
        CheckStatusEffects();
        if (!cascaded)
            Lose(attackerDetails);
        if (die)
            Dies();
    }
    public void Exhausted(HazardCreatureCardDetails attackerDetails, bool cascaded = false)
    {
        effectsApplied.Add(new StatusEffectsApplied(StatusEffect.EXHAUSTATION, turn.GetTurnNumber(), true));
        if(!cascaded)
            AddMessage(
                GameObject.Find("Localization").GetComponent<Localization>().Localize("exhausted"),
                0.5f,
                "exhausted");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            CharacterCardUI originalCharacter = originalCard as CharacterCardUI;
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Exhausted(attackerDetails, true);
        }
        CheckStatusEffects();
        if (!cascaded)
            Lose(attackerDetails);
    }

    public void Trapped(bool cascaded = false)
    {
        effectsApplied.Add(new StatusEffectsApplied(StatusEffect.TRAP, turn.GetTurnNumber(), true));
        if (!cascaded)
            AddMessage(
                GameObject.Find("Localization").GetComponent<Localization>().Localize("trapped"),
                0.5f,
                "trapped");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            CharacterCardUI originalCharacter = originalCard as CharacterCardUI;
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Trapped(true);
        }
        CheckStatusEffects();
    }
    public void Fire(bool cascaded = false)
    {
        effectsApplied.Add(new StatusEffectsApplied(StatusEffect.FIRE, turn.GetTurnNumber(), true));
        if (!cascaded)
            AddMessage(
                GameObject.Find("Localization").GetComponent<Localization>().Localize("fire"),
                0.5f,
                "fire");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            CharacterCardUI originalCharacter = originalCard as CharacterCardUI;
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Fire(true);
        }
        CheckStatusEffects();
    }
    public void Blind(bool cascaded = false)
    {
        effectsApplied.Add(new StatusEffectsApplied(StatusEffect.BLIND, turn.GetTurnNumber(), true));
        if (!cascaded)
            AddMessage(
                GameObject.Find("Localization").GetComponent<Localization>().Localize("blind"),
                0.5f,
                "blind");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            CharacterCardUI originalCharacter = originalCard as CharacterCardUI;
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Blind(true);
        }
        CheckStatusEffects();
    }
    public void Ice(bool cascaded = false)
    {
        effectsApplied.Add(new StatusEffectsApplied(StatusEffect.ICE, turn.GetTurnNumber(), true));
        if (!cascaded)
            AddMessage(
                GameObject.Find("Localization").GetComponent<Localization>().Localize("ice"),
                0.5f,
                "ice");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            CharacterCardUI originalCharacter = originalCard as CharacterCardUI;
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Ice(true);
        }
        CheckStatusEffects();
    }
    public void Morgul(bool cascaded = false)
    {
        effectsApplied.Add(new StatusEffectsApplied(StatusEffect.MORGUL, turn.GetTurnNumber(), false));
        if (!cascaded)
            AddMessage(
                GameObject.Find("Localization").GetComponent<Localization>().Localize("morgul"),
                0.5f,
                "morgul");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            CharacterCardUI originalCharacter = originalCard as CharacterCardUI;
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Morgul(true);
        }
        CheckStatusEffects();
    }
    public void Poisoned(bool cascaded = false)
    {
        effectsApplied.Add(new StatusEffectsApplied(StatusEffect.POISON, turn.GetTurnNumber(), true));
        if (!cascaded)
            AddMessage(
                GameObject.Find("Localization").GetComponent<Localization>().Localize("poisoned"),
                0.5f,
                "poisoned");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            CharacterCardUI originalCharacter = originalCard as CharacterCardUI;
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Poisoned(true);
        }
        CheckStatusEffects();
    }
    public void Bleeding(bool cascaded = false)
    {
        effectsApplied.Add(new StatusEffectsApplied(StatusEffect.BLOOD, turn.GetTurnNumber(), false));
        if (!cascaded)
            AddMessage(
                GameObject.Find("Localization").GetComponent<Localization>().Localize("bleeding"),
                0.5f,
                "bleeding");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            CharacterCardUI originalCharacter = originalCard as CharacterCardUI;
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Bleeding(true);
        }
        CheckStatusEffects();
    }

    public void Heal(bool cascaded = false)
    {
        effectsApplied = effectsApplied.FindAll(x => !x.healable).ToList();
        if (!cascaded)
            AddMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("healed"), 0.5f, "healed");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            CharacterCardUI originalCharacter = ((CharacterCardUI)originalCard);
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Heal(true);
        }
        CheckStatusEffects();
    }

    public void Won(HazardCreatureCardDetails details)
    {
        AddMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("won"), 1, "success");
        AddMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("VP+") + details.GetVictoryPoints(), 1, "success");
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

    public void AddObject(CardDetails cardDetails)
    {
        if (!cardDetails.IsClassOf(CardClass.Object) &&
           !cardDetails.IsClassOf(CardClass.Ring))
        {
            Debug.LogWarning("Trying to add " + cardDetails.cardId + " as an object to a char!");
            return;
        }

        objects.Add(cardDetails);
    }
    public void AddAlly(CardDetails cardDetails)
    {
        if (!cardDetails.IsClassOf(CardClass.Ally))
        {
            Debug.LogWarning("Trying to add " + cardDetails.cardId + " as an ally to a char!");
            return;
        }

        allies.Add(cardDetails);
    }
    public void AddGoldRing(CardDetails cardDetails)
    {
        if (cardDetails.IsClassOf(CardClass.GoldRing))
        {
            Debug.LogWarning("Trying to add " + cardDetails.cardId + " as a gold ring to a char!");
            return;
        }

        goldRings.Add(cardDetails);
    }

    public List<CardDetails> GetObjects()
    {
         return objects;
    }
    public List<CardDetails> GetAllies()
    {
        return allies;
    }
    public List<CardDetails> GetGoldRings()
    {
        return goldRings;
    }

    public string GetInCompanyOf()
    {
        return inCompanyOf;
    }

    public void Dies()
    {
        CardUI card = board.GetCardManager().GetCardUI(details);
        if(card != null)
        {
            CharacterCardUIBoard original = card as CharacterCardUIBoard;
            if (original != null)
            {
                board.GetTile(original.GetHex()).RemoveCard(this);
                inCompanyOf = "";
                foreach (CardDetails companion in board.GetCharacterManager().GetCharactersInCompanyOf(details))
                {
                    CardUI originalCard = board.GetCardManager().GetCardUI(companion);
                    if (originalCard != null)
                    {
                        CharacterCardUIBoard originalCharacter = originalCard as CharacterCardUIBoard;
                        if (originalCharacter != null)
                            originalCharacter.inCompanyOf = "";
                    }
                }
                AddMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("died"), 1, "failure");
                resourcesManager.RecalculateInfluence(owner);
                DestroyImmediate(gameObject);
            }
        }
    }

}
