using System;
using System.Collections.Generic;
using System.Linq;
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
    private bool loaded;
    private bool awaken = false;
    

    void Awake()
    {
        placeDeckManager = GameObject.Find("PlaceDeckManager").GetComponent<PlaceDeck>();
        cardDetailsRepo = GameObject.Find("CardDetailsRepo").GetComponent<CardDetailsRepo>();
        if(turn == null)
            turn = GameObject.Find("Turn").GetComponent<Turn>();
        handPos = 0;
        loaded = false;
        requirements = new List<DeckCardUIRequirement>();
        awaken = true;
    }

    public override bool Initialize(NationsEnum owner, string cardId, CardClass cardClass, bool isHover)
    {
        if (!awaken)
            Awake();
        this.cardId = cardId;
        this.cardClass = cardClass;
        this.nation = owner;
        this.isHover = isHover;

        if (!turn.IsNewTurnLoaded())
            return false;

        if (initialized)
            return false;

        if (cardId == null)
            return false;

        if (owner == NationsEnum.ABANDONED)
            return false;

        if (!InitializeCard(owner, cardDetailsRepo.GetCardDetails(cardId, owner), false, isHover))
            return false;

        if (!resourcesManager.isInitialized)
            return false;

        if (cardDetails == null)
            return false;

        if (game.GetHumanPlayer().GetNation() != owner)
            return true;

        button.enabled = true;
        button.interactable = true;

        loaded = true;

        return true;
    }

    protected override void Update()
    {
        if (!loaded)
        {
            Initialize(nation, cardId, cardClass, isHover);
            return;
        }

        if (game.GetHumanPlayer().GetNation() == owner)
        {
            if (dirtyMessages.Count() > 0 && this.cardDetails != null)
                RefreshRequirements();
            AnimateName();
        }
    }

    public override void RefreshRequirements()
    {
        if (dirtyMessages.Count() < 1)
            return;

        if (owner == turn.GetCurrentPlayer())
        {
            foreach (DirtyReasonEnum isDirty in dirtyMessages)
            {
                //Debug.Log(string.Format("{0} received signal: {1}", cardId, dirtyMessages.ToString()));
                foreach (CardCondition cc in conditions)
                {
                    if (cc.GetDirtyCheck() == isDirty || isDirty == DirtyReasonEnum.INITIALIZATION)
                    {
                        RemoveOldSprites(cc.GetInvolvedSprites());
                        //Debug.Log(string.Format("{0} cleansed: {1}", cardId, cc.GetInvolvedSprites().ToLineSeparatedString()));
                    }
                }
            }
            
            foreach (DirtyReasonEnum isDirty in dirtyMessages)
            {
                conditionsFailed = new();
                //Debug.Log(string.Format("{0} processes signal: {1}", cardId, isDirty.ToString()));

                foreach (CardCondition cc in conditions)
                {
                    if (cc.GetDirtyCheck() == isDirty || isDirty == DirtyReasonEnum.INITIALIZATION)
                    {
                        //Debug.Log(string.Format("{0} processes condition from signal: {1}", cardId, isDirty.ToString()));
                        if (cc.RunCondition().Count() > 0)
                            conditionsFailed.AddRange(cc.GetLastResult());
                        RefreshIcons(cc);
                    }
                }
            }
        }
        dirtyMessages = new ();
    }

    public void RefreshIcons(CardCondition cardCondition)
    {
        Dictionary<string, int> sprites = new();
        
        foreach(PlayableConditionResultEnum conditionResult in cardCondition.GetLastResult())
        {
            switch (conditionResult)
            {
                case PlayableConditionResultEnum.NO_INFLUENCE:
                    sprites.Add("influence", 1);
                    break;
                case PlayableConditionResultEnum.NO_HOMETOWN:
                    sprites.Add("hometown", 1);
                    break;
                case PlayableConditionResultEnum.NO_MANA:
                    foreach(CardTypesEnum c in Enum.GetValues(typeof(CardTypesEnum)))
                        sprites.Add(c.ToString(), missingMana.ContainsKey(c) ? missingMana[c] : 0);
                    break;
                case PlayableConditionResultEnum.NO_RESOURCES:
                    foreach (ResourceType c in Enum.GetValues(typeof(ResourceType)))
                        sprites.Add(c.ToString(), missingResources.resources[c]);
                    break;
                case PlayableConditionResultEnum.NOT_AT_CITY:
                    sprites.Add("city", 1);
                    break;
                case PlayableConditionResultEnum.SLOT_NOT_FOUND:
                    sprites.Add("slot", 1);
                    break;
                case PlayableConditionResultEnum.IS_RING:
                case PlayableConditionResultEnum.RING_TYPE_NOT_FOUND:
                    sprites.Add("ring", 1);
                    break;
                case PlayableConditionResultEnum.SELECT_CHAR:
                    sprites.Add("character", 1);
                    break;
            }
        }

        RemoveOldSprites(sprites.Keys.ToList());
        foreach (string sprite in sprites.Keys)
            InstantiateResource(sprite, sprites[sprite]);
    }

    public void RemoveOldSprites(List<string> sprites)
    {
        requirements = requirements.FindAll(x => x != null && x.gameObject != null);
        requirements.FindAll(x => sprites.Contains(x.requirementName)).ForEach(x => DestroyImmediate(x.gameObject));
    }

    public override DeckCardUIRequirement InstantiateResource(string spriteId, int value)
    {
        if (value < 1)
            return null;
        //Debug.Log(string.Format("{0} requires missing {1}", cardId, spriteId));

        GameObject go = Instantiate(resourcePrefab, resources.transform);
        go.name = spriteId + "_resource";
        go.transform.SetParent(resources.transform);

        DeckCardUIRequirement deckCardUIrequirement = go.GetComponent<DeckCardUIRequirement>();
        deckCardUIrequirement.Initialize(spriteId, value);
        
        requirements.Add(deckCardUIrequirement);
        
        return deckCardUIrequirement;
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
