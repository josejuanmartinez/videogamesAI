using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeckCardUI : CardTemplateUI, IPointerEnterHandler, IPointerExitHandler
{
    private short handPos;
    private string cardId;
    private CardClass cardClass;
    private NationsEnum nation;
    private List<DeckCardUIRequirement> requirements;
    private bool awaken = false;

    void Awake()
    {
        placeDeckManager = GameObject.Find("PlaceDeckManager").GetComponent<PlaceDeck>();
        cardDetailsRepo = GameObject.Find("CardDetailsRepo").GetComponent<CardDetailsRepo>();
        if(turn == null)
            turn = GameObject.Find("Turn").GetComponent<Turn>();
        handPos = 0;
        initialized = false;
        canBePlayed = false;
        isDirty = DirtyReason.INITIALIZATION;
        requirements = new List<DeckCardUIRequirement>();
        awaken = true;
    }

    public override bool Initialize(NationsEnum owner, string cardId, CardClass cardClass)
    {
        if (!awaken)
            Awake();
        this.cardId = cardId;
        this.cardClass = cardClass;
        this.nation = owner;

        if (!turn.IsNewTurnLoaded())
            return false;

        if (initialized)
            return false;

        if (cardId == null)
            return false;

        if (owner == NationsEnum.ABANDONED)
            return false;

        if (!InitializeCard(owner, cardDetailsRepo.GetCardDetails(cardId, owner)))
            return false;

        if (!resourcesManager.isInitialized)
            return false;

        if (cardDetails == null)
            return false;

        if (game.GetHumanPlayer().GetNation() != owner)
            return true;

        CalculateAllRequirements();
        
        initialized = true;
        
        return true;
    }

    protected override void Update()
    {
        if (!initialized)
        {
            Initialize(nation, cardId, cardClass);
            return;
        }
        if(game.GetHumanPlayer().GetNation() == owner)
        {
            if (isDirty != DirtyReason.NONE)
                RefreshRequirements();
            AnimateName();
        }
    }

    public override void RefreshRequirements()
    {
        if (owner == turn.GetCurrentPlayer())
        {
            switch (isDirty)
            {
                case DirtyReason.INITIALIZATION:
                    CalculateAllRequirements();
                    break;
                case DirtyReason.CHAR_SELECTED:
                    CalculateIsCharacterAtCity();
                    break;
                case DirtyReason.NEW_RESOURCES:
                    CalculateMissingResources();
                    break;
                case DirtyReason.NONE:
                    break;
            }
        }
        
        isDirty = DirtyReason.NONE;
    }

    public override void CalculateAllRequirements()
    {
        canBePlayed = CalculateMissingResources().Sum() < 1;
        canBePlayed &= CalculateConditions().Contains(PlayableConditionResult.SUCCESS);
    }

    public override void CalculateIsCharacterAtCity()
    {
        HashSet<PlayableConditionResult> conditionResults = deck.IsSelectedCharacterAtCity(cardDetails);

        isSelectedCharAtForeignCity = conditionResults.Count < 1;

        if (cardDetails.cardClass == CardClass.Object)
        {
            HashSet<PlayableConditionResult> objectConditionResults = deck.IsObjectCardPlayable(cardDetails, owner);
            isSelectedCharAtForeignCity &= objectConditionResults.Contains(PlayableConditionResult.SUCCESS);
            if (objectConditionResults.Count > 0)
                conditionResults.AddRange(objectConditionResults);            
        }            
        else if (cardDetails.cardClass == CardClass.GoldRing)
        {
            HashSet<PlayableConditionResult> objectConditionResults = deck.IsGoldRingCardPlayable(cardDetails, owner);
            isSelectedCharAtForeignCity &= objectConditionResults.Contains(PlayableConditionResult.SUCCESS);
            if (objectConditionResults.Count > 0)
                conditionResults.AddRange(objectConditionResults);
        }
        
        canBePlayed = missing.Sum() < 1 && isSelectedCharAtForeignCity;

        requirements = requirements.FindAll(x => x != null && x.gameObject != null);
        requirements.FindAll(x => 
                                    x.requirementName == "character" || 
                                    x.requirementName == "city" ||
                                    x.requirementName == "slot" ||
                                    x.requirementName == "ring"
                                    ).ForEach(x => DestroyImmediate(x.gameObject));

        if (isSelectedCharAtForeignCity)
            return;
        
        if (conditionResults.Contains(PlayableConditionResult.SELECT_CHAR))
            InstantiateResource("character", 1);
        if (conditionResults.Contains(PlayableConditionResult.NOT_AT_CITY))
            InstantiateResource("city", 1);
        if (conditionResults.Contains(PlayableConditionResult.RING_TYPE_NOT_FOUND))
            InstantiateResource("ring", 1);
        if (conditionResults.Contains(PlayableConditionResult.SLOT_NOT_FOUND))
            InstantiateResource("slot", 1);
    }


    public override HashSet<PlayableConditionResult> CalculateConditions()
    {
        HashSet<PlayableConditionResult> conditionResults = base.CalculateConditions();
        if (conditionResults.Contains(PlayableConditionResult.SUCCESS))
            return conditionResults;

        switch (cardDetails.cardClass)
        {
            case CardClass.HazardCreature:
                HazardCreatureCardDetails hazardDetails = (HazardCreatureCardDetails)cardDetails;
                if (conditionResults.Contains(PlayableConditionResult.NO_MANA))
                {
                    foreach (CardTypesEnum c in Enum.GetValues(typeof(CardTypesEnum)))
                    {
                        requirements = requirements.FindAll(x => x != null && x.gameObject != null);
                        requirements.FindAll(x => x.requirementName == c.ToString()).ForEach(x => DestroyImmediate(x.gameObject));
                    }

                    Dictionary<CardTypesEnum, int> requiredCardTypes = new();
                    foreach (CardTypesEnum c in hazardDetails.cardTypes)
                    {
                        if (!requiredCardTypes.ContainsKey(c))
                            requiredCardTypes.Add(c, 1);
                        else
                            requiredCardTypes[c]++;
                    }

                    foreach (CardTypesEnum c in requiredCardTypes.Keys)
                    {
                        int required = requiredCardTypes[c];
                        int available = manaManager.mana[nation][c];
                        int missing = required - available;
                        if (missing > 0)
                            InstantiateResource(c.ToString(), missing);
                    }
                }
                break;
            default:
                requirements = requirements.FindAll(x => x != null && x.gameObject != null);
                requirements.FindAll(x => 
                    x.requirementName == "hometown"  || 
                    x.requirementName == "influence" ||
                    x.requirementName == "inplay" ||
                    x.requirementName == "city" ||
                    x.requirementName == "city_tapped" ||
                    x.requirementName == "slot" ||
                    x.requirementName == "ring").ForEach(x => DestroyImmediate(x.gameObject));

                if (conditionResults.Contains(PlayableConditionResult.NO_HOMETOWN))
                    InstantiateResource("hometown", 1);
                if (conditionResults.Contains(PlayableConditionResult.NO_INFLUENCE))
                    InstantiateResource("influence", 1);
                if (conditionResults.Contains(PlayableConditionResult.ALREADY_IN_PLAY))
                    InstantiateResource("inplay", 1);
                if (conditionResults.Contains(PlayableConditionResult.NOT_AT_CITY))
                    InstantiateResource("city", 1);
                if (conditionResults.Contains(PlayableConditionResult.CITY_TAPPED))
                    InstantiateResource("city_tapped", 1);
                if (conditionResults.Contains(PlayableConditionResult.SLOT_NOT_FOUND))
                    InstantiateResource("slot", 1);
                if (conditionResults.Contains(PlayableConditionResult.IS_RING) || 
                    conditionResults.Contains(PlayableConditionResult.RING_TYPE_NOT_FOUND))
                    InstantiateResource("ring", 1);
                if (conditionResults.Contains(PlayableConditionResult.SELECT_CHAR))
                    InstantiateResource("character", 1);
                break;
        }
        return conditionResults;
    }

    public override DeckCardUIRequirement InstantiateResource(string spriteId, int value)
    {
        if (value < 1)
            return null;
        GameObject go = Instantiate(resourcePrefab, resources.transform);
        go.name = spriteId + "_resource";
        go.transform.SetParent(resources.transform);

        DeckCardUIRequirement deckCardUIrequirement = go.GetComponent<DeckCardUIRequirement>();
        deckCardUIrequirement.Initialize(spriteId, value);
        
        requirements.Add(deckCardUIrequirement);
        
        return deckCardUIrequirement;
    }

    public override Resources CalculateMissingResources()
    {
        base.CalculateMissingResources();
        foreach(ResourceType res in Enum.GetValues(typeof(ResourceType)))
        {
            requirements = requirements.FindAll(x => x != null && x.gameObject != null);
            requirements.FindAll(x => x.requirementName == res.ToString()).ForEach(x => DestroyImmediate(x.gameObject));
            InstantiateResource(res.ToString(), missing.resources[res]);
        }

        canBePlayed = missing.Sum() < 1 && isSelectedCharAtForeignCity;

        return missing;
    }

    public void IncreaseHandPosition()
    {
        handPos++;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        mouse.ChangeCursor("clickable");
        placeDeckManager.SetCardToShow(new HoveredCard(nation, cardDetails.cardId, cardDetails.cardClass));
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        mouse.RemoveCursor();
        placeDeckManager.RemoveCardToShow(new HoveredCard(nation, cardDetails.cardId, cardDetails.cardClass));
    }
    public short GetHandPos()
    {
        return handPos;
    }

    public override void OnClick()
    {
        selectedItems.SelectCardDetails(cardDetails, nation);
    }
}
