using System;
using System.Collections.Generic;
using UnityEngine;
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
    [Header("Company")]
    [SerializeField]
    protected string inCompanyOf;
    [SerializeField]
    protected List<CardDetails> objects;
    [SerializeField]
    protected List<CardDetails> goldRings;
    [SerializeField]
    protected List<CardDetails> allies;

    protected CardUI potentialLeader;

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

        objects = new();
        allies = new();

        CheckStatusEffects();
        GetObjectsAndAllies();

        initialized = true;

        return initialized;
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

    public bool CanMove()
    {
        return IsImmovable();
    }
    public bool IsImmovable()
    {
        if ((details as CharacterCardDetails).isImmovable)
            return !eventsManager.IsEventInPlay(EventAbilities.CanMove, turn.GetCurrentPlayer());
        else
            return immovableIcon.enabled;
    }

    public bool IsBleeding()
    {
        return bleedingIcon.enabled;
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
    public bool IsExhausted()
    {
        return exhaustedIcon.enabled;
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

        if (IsHurt() || IsPoisoned())
            prowess--;

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
        return Math.Min(1, mind);
    }
    public int GetTotalInfluence()
    {
        int influence;
        CharacterCardDetails charDetails = GetCharacterDetails();
        influence = charDetails.GetInfluence();

        if (IsMorgul())
            influence -= 3;

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
        return Math.Max(1, influence);
    }

    public int GetTotalMovement()
    {
        int movement;
        movement = MovementConstants.characterMovement;

        if (IsMorgul())
            movement -= 4;
        if (IsIce())
            movement -= 4;

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
            CharacterCardUI originalCharacter = originalCard as CharacterCardUI;
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Hurt();
        }
        if (cardDetails != null)
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
            CharacterCardUI originalCharacter = originalCard as CharacterCardUI;
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Exhausted();
        }
        if (cardDetails != null)
            Lose(cardDetails);
    }

    public void Immovable(HazardCreatureCardDetails cardDetails = null)
    {
        immovableIcon.enabled = true;
        AddMessage(
            GameObject.Find("Localization").GetComponent<Localization>().Localize("exhausted"),
            0.5f,
            "immovable");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            CharacterCardUI originalCharacter = originalCard as CharacterCardUI;
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Immovable();
        }
        if (cardDetails != null)
            Lose(cardDetails);
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
            CharacterCardUI originalCharacter = originalCard as CharacterCardUI;
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Fire();
        }
        if (cardDetails != null)
            Lose(cardDetails);
    }
    public void Ice(HazardCreatureCardDetails cardDetails = null)
    {
        fireIcon.enabled = true;
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        AddMessage(
            GameObject.Find("Localization").GetComponent<Localization>().Localize("ice"),
            0.5f,
            "ice");
        if (originalCard != null)
        {
            CharacterCardUI originalCharacter = originalCard as CharacterCardUI;
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Ice();
        }
        if (cardDetails != null)
            Lose(cardDetails);
    }
    public void Morgul(HazardCreatureCardDetails cardDetails = null)
    {
        morgulIcon.enabled = true;
        AddMessage(
            GameObject.Find("Localization").GetComponent<Localization>().Localize("morgul"),
            0.5f,
            "morgul");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            CharacterCardUI originalCharacter = originalCard as CharacterCardUI;
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Morgul();
        }
        if (cardDetails != null)
            Lose(cardDetails);
    }
    public void Poisoned(HazardCreatureCardDetails cardDetails = null)
    {
        poisonedIcon.enabled = true;
        AddMessage(
            GameObject.Find("Localization").GetComponent<Localization>().Localize("poisoned"),
            0.5f,
            "poisoned");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            CharacterCardUI originalCharacter = originalCard as CharacterCardUI;
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Poisoned();
        }
        if (cardDetails != null)
            Lose(cardDetails);
    }
    public void Heals()
    {
        hurtIcon.enabled = false;
        exhaustedIcon.enabled = false;
        AddMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("healed"), 0.5f, "healed");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            CharacterCardUI originalCharacter = originalCard as CharacterCardUI;
            if (originalCharacter != null && originalCharacter != this)
                originalCharacter.Heals();
        }
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
