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
    [SerializeField]
    protected List<CardDetails> objects;
    [SerializeField]
    protected List<CardDetails> goldRings;
    [SerializeField]
    protected List<CardDetails> allies;

    protected CardUI potentialLeader;

    public override bool Initialize(string cardId, NationsEnum owner)
    {
        if (!base.Initialize(cardId, owner))
            return false;

        hurtIcon.enabled = false;
        exhaustedIcon.enabled = false;
        poisonedIcon.enabled = false;
        morgulIcon.enabled = false;
        bleedingIcon.enabled = false;
        immovableIcon.enabled = false;

        objects = new();
        allies = new();

        CheckStatusEffects();
        GetObjectsAndAllies();

        initialized = true;

        return true;
    }

    public void GetObjectsAndAllies()
    {
        if (board.GetCardManager().GetCardUI(details) != null)
        {
            CardUI existingCardUI = board.GetCardManager().GetCardUI(details);
            if (existingCardUI != null)
            {
                if ((CharacterCardUI)existingCardUI != null)
                {
                    CharacterCardUI existingCharacterCardUI = (CharacterCardUI)existingCardUI;

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
                if ((CharacterCardUI)existingCardUI != null)
                {
                    CharacterCardUI existingCharacterCardUI = (CharacterCardUI)existingCardUI;
                    bool value = existingCharacterCardUI.GetHurtIcon().enabled;
                    if(hurtIcon.enabled != value)
                    {
                        hurtIcon.enabled = value;
                        ShowMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value? "hurt" : "healed"),
                            1f,
                            colorManager.GetColor(value? "failure" : "success"));
                    }

                    value = existingCharacterCardUI.GetExhaustedIcon().enabled;
                    if (exhaustedIcon.enabled != value)
                    {
                        exhaustedIcon.enabled = value;
                        ShowMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value? "exhausted": "rested"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
                    }

                    value = existingCharacterCardUI.GetPoisonedIcon().enabled;
                    if (poisonedIcon.enabled != value)
                    {
                        poisonedIcon.enabled = value;
                        ShowMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value? "poisoned": "Healed"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
                    }

                    value = existingCharacterCardUI.GetMorgulIcon().enabled;
                    if (morgulIcon.enabled != value)
                    {
                        morgulIcon.enabled = value;
                        ShowMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value? "morgul": "healed"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
                    }


                    value = existingCharacterCardUI.GetBleedingIcon().enabled;
                    if (bleedingIcon.enabled != value)
                    {
                        bleedingIcon.enabled = value;
                        ShowMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value? "bleeding" : "healed"),
                            1f,
                            colorManager.GetColor(value ? "failure" : "success"));
                    }

                    value = IsImmovable();
                    if (immovableIcon.enabled != value)
                    {
                        immovableIcon.enabled = value;
                        ShowMessage(
                            GameObject.Find("Localization").GetComponent<Localization>().Localize(value? "rooted" : "unrooted"),
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
        if (((CharacterCardDetails)details).isImmovable)
            return !eventsManager.IsEventInPlay(EventAbilities.CanMove, turn.GetCurrentPlayer());
        return false;
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
                ObjectCardDetails objectDetails = (ObjectCardDetails)cardDetail;
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
                RingCardDetails objectDetails = (RingCardDetails)cardDetail;
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
                ObjectCardDetails objectDetails = (ObjectCardDetails)cardDetail;
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
                RingCardDetails objectDetails = (RingCardDetails)cardDetail;
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
                ObjectCardDetails objectDetails = (ObjectCardDetails)cardDetail;
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
                RingCardDetails objectDetails = (RingCardDetails)cardDetail;
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

        if (morgul)
            influence -= 3;

        //GET OBJECTS OF THE CHAR
        foreach (CardDetails cardDetail in objects)
        {
            if (cardDetail.cardClass == CardClass.Object)
            {
                ObjectCardDetails objectDetails = (ObjectCardDetails)cardDetail;
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
                RingCardDetails objectDetails = (RingCardDetails)cardDetail;
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

        if (morgul)
            movement -= 4;
        if (bleeding)
            movement -= 3;
        if (poisoned)
            movement -= 2;
        if (hurt)
            movement -= 1;

        //GET OBJECTS OF THE CHAR
        foreach (CardDetails cardDetail in objects)
        {
            if (cardDetail.cardClass == CardClass.Object)
            {
                ObjectCardDetails objectDetails = (ObjectCardDetails)cardDetail;
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

    public void Hurt()
    {
        hurt = true;
        hurtIcon.enabled = true;
        ShowMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("hurt"), 1, "hurt");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            CharacterCardUI originalCharacter = (CharacterCardUI)originalCard;
            if(originalCharacter != null)
                originalCharacter.Hurt();
        }
    }
    public void Exhausted()
    {
        exhausted = true;
        exhaustedIcon.enabled = true;
        ShowMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("exhausted"), 1, "exhausted");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            CharacterCardUI originalCharacter = (CharacterCardUI)originalCard;
            if(originalCharacter != null)
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
        ShowMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("poisoned"), 1, "poisons");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            CharacterCardUI originalCharacter = (CharacterCardUI)originalCard;
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
        ShowMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("bleeding"), 1, "bleeding");
        CardUI originalCard = board.GetCardManager().GetCardUI(details);
        if (originalCard != null)
        {
            CharacterCardUI originalCharacter = (CharacterCardUI)originalCard;
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
        ShowMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("cursed"), 1, "curses");
        if (originalCard != null)
        {
            CharacterCardUI originalCharacter = (CharacterCardUI)originalCard;
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
            CharacterCardUI originalCharacter = (CharacterCardUI)originalCard;
            if (originalCharacter != null)
            {
                hurt = false;
                exhausted = false;
                hurtIcon.enabled = false;
                exhaustedIcon.enabled = false;
                ShowMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("healed"), 1, "success");
            }
        }
    }

    public void Won(HazardCreatureCardDetails details)
    {
        ShowMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("won"), 1, "success");
        deckManager.AddToWonPile(owner, details);
    }
    public void Lose(HazardCreatureCardDetails details)
    {
        deckManager.AddToDiscardPile(owner, details);
    }
    public void Resisted(HazardCreatureCardDetails details)
    {
        ShowMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("resisted"), 1, "resisted");
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
            CharacterCardUIBoard original = (CharacterCardUIBoard)card;
            if(original != null)
            {
                board.GetTile(original.GetHex()).RemoveCard(this);
                inCompanyOf = "";
                foreach (CardDetails companion in board.GetCharacterManager().GetCharactersInCompanyOf(details))
                {
                    CardUI originalCard = board.GetCardManager().GetCardUI(companion);
                    if (originalCard != null)
                    {
                        CharacterCardUIBoard originalCharacter = (CharacterCardUIBoard)originalCard;
                        if (originalCharacter != null)
                            originalCharacter.inCompanyOf = "";
                    }
                }
                ShowMessage(GameObject.Find("Localization").GetComponent<Localization>().Localize("died"), 1, "failure");
                resourcesManager.RecalculateInfluence(owner);
                DestroyImmediate(gameObject);
            }
        }
    }

}
