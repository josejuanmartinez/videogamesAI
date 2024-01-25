using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public Transform deckTransform;
    public CanvasGroup deckCanvasGroup;
    public GameObject deckCardUIPrefab;

    public List<string> startWithId;

    private Game game;
    private Board board;
    private PlaceDeck placeDeckManager;
    private Turn turn;
    private CardDetailsRepo cardRepo;  
    private SelectedItems selectedItems;
    private CameraController cameraController;

    private List<CardsOfPlayer> cardsOfPlayer;

    private List<DirtyReasonEnum> isDirty;
    private string loadingPlayer;
    private bool isInitialized;

    
    void Awake()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
        board = GameObject.Find("Board").GetComponent<Board>();
        placeDeckManager = GameObject.Find("PlaceDeckManager").GetComponent<PlaceDeck>();
        turn = GameObject.Find("Turn").GetComponent<Turn>();
        cardRepo = GameObject.Find("CardDetailsRepo").GetComponent<CardDetailsRepo>();
        selectedItems = GameObject.Find("SelectedItems").GetComponent<SelectedItems>();
        cameraController = GameObject.Find("CameraController").GetComponent<CameraController>();

        cardsOfPlayer = new();

        isDirty = new();

        isInitialized = false;
    }

    void Initialize()
    {
        if (!cardRepo.IsInitialized())
            return;
        if (!turn.IsInitialized())
            return;
        if (!board.IsAllLoaded())
            return;
        if (isInitialized)
            return;
        foreach (NationsEnum nation in Enum.GetValues(typeof(NationsEnum)))
        {
            if (nation == NationsEnum.ABANDONED)
                continue;
            loadingPlayer = nation.ToString();
            bool human = game.GetHumanNation() == nation;

            GameObject go = new("deck_" + nation.ToString());
            go.transform.parent = transform;
            CardsOfPlayer cardsOfThisPlayer = go.AddComponent<CardsOfPlayer>();
            cardsOfThisPlayer.Initialize(nation,
                    game.GetHumanNation() == nation ? deckTransform : null,
                    deckCardUIPrefab,
                    5,
                    human? startWithId : new List<string>());
            cardsOfPlayer.Add(cardsOfThisPlayer);
        }
        //Debug.Log(string.Format("DeckManager loaded at {0}", Time.realtimeSinceStartup));
        isInitialized = true;
    }

    public bool IsInitialized()
    {
        return isInitialized;
    }

    public string GetLoadingPlayer()
    {
        return loadingPlayer ?? "";
    }

    public void AddToWonPile(NationsEnum owner, CardDetails details)
    {
        cardsOfPlayer.Find(x => x.GetNation() == owner).AddToWonPile(details);
    }
    public void AddToDiscardPile(NationsEnum owner, CardDetails details)
    {
        cardsOfPlayer.Find(x => x.GetNation() == owner).AddToDiscardPile(details);
    }    

    void Update()
    {
        bool visible = !game.IsPopup() && !cameraController.IsPreventedDrag();

        deckCanvasGroup.alpha = visible? 1 : 0;
        deckCanvasGroup.interactable = visible;

        if (!isInitialized)
        {
            Initialize();
            return;
        }
        if(isDirty.Count() > 0)
        {
            short handSize = cardsOfPlayer.Find(x => x.GetNation() == turn.GetCurrentPlayer()).GetHandSize();
            for (int i = 0; i< handSize; i++)
                if (GetHandCardGameObject(turn.GetCurrentPlayer(), i) != null)
                    GetHandCardGameObject(turn.GetCurrentPlayer(), i).GetComponent<CardTemplateUI>().Dirty(isDirty);
            isDirty = new();
        }
    }

    private GameObject GetHandCardGameObject(NationsEnum nation, int cardShown)
    {
        if(game.GetHumanNation() != nation)
        {
            Debug.LogError(string.Format("Trying to access to the instantiated deck of {0} but it is not human!", nation));
            return null;
        }
        CardsOfPlayer humanCards = cardsOfPlayer.Find(x => x.GetNation() == nation);
        if(!humanCards.HasCards())
            return null;
        return humanCards.GetHandCardGameObject(cardShown);
    }

    public void DiscardAndDraw(NationsEnum nation, CardDetails card, bool discarded)
    {
        cardsOfPlayer.Find(x => x.GetNation() == nation).DiscardAndDraw(card, discarded);
        placeDeckManager.RemoveCardToShow(new HoveredCard(nation, card.cardId, card.cardClass));
    }

    public string CanSpawnCharacterAtHome(CardDetails cardDetails, NationsEnum owner)
    {
        if (owner == NationsEnum.ABANDONED)
            return null;

        if (!cardDetails.IsClassOf(CardClass.Character))
            return null;
        
        CharacterCardDetails character = cardDetails as CharacterCardDetails;

        if (character  == null) 
            return null;

        if (board.GetCityManager().GetCityOfPlayer(owner, character.GetHomeTown()) != null)
            return character.GetHomeTown();

        return null;
    }

    public Vector2 CanSpawnHazardCreatureAtLastHex()
    {
        Vector2 negativeInfinity = new (float.MinValue, float.MinValue);
        CardUI character = selectedItems.GetSelectedMovableCardUI();
        if (character == null)
            return negativeInfinity;

        HazardCreatureCardDetails hazardDetails = selectedItems.GetHazardCreatureCardDetails();
        if (hazardDetails == null)
            return negativeInfinity;

        if (character == null)
            return negativeInfinity;

        CharacterCardUIBoard characterUI = character as CharacterCardUIBoard;
        if(characterUI == null) 
            return negativeInfinity;

        Vector2Int hex = characterUI.GetHex();
        if(hex == null)
            return negativeInfinity;

        return hex;
    }

    public void Dirty(DirtyReasonEnum DirtyReasonEnum)
    {
        isDirty.Add(DirtyReasonEnum);
    }

    public bool HasCardInDeck(NationsEnum nation, CardClass cardClass)
    {
        CardsOfPlayer cards = cardsOfPlayer.Find(x => x.GetNation() == nation);
        if (!cards.HasCards()) return false;

        for (int i = 0; i < cards.GetHandSize(); i++)
        {
            if (GetHandCardGameObject(nation, i) == null)
                continue;
            
            if (GetHandCardGameObject(nation, i).GetComponentInChildren<DeckCardUI>() == null)
                continue;
            DeckCardUI deckCard = GetHandCardGameObject(nation, i).GetComponentInChildren<DeckCardUI>();
            if (deckCard.GetCardDetails() == null)
                continue;
            if (deckCard.GetCardDetails().cardClass != cardClass)
                continue;
            return true;
        }
        return false;
    }

    public bool HasObjectSlotInDeck(NationsEnum nation, ObjectType objSlot)
    {
        CardsOfPlayer cards = cardsOfPlayer.Find(x => x.GetNation() == nation);
        if (!cards.HasCards()) return false;

        if (!HasCardInDeck(nation, CardClass.Object)) return false; 

        for (int i = 0; i < cards.GetHandSize(); i++)
        {
            if (GetHandCardGameObject(nation, i) == null)
                continue;

            if(GetHandCardGameObject(nation, i).TryGetComponent<ObjectCardDetails>(out var details))
                if (details.objectSlot == objSlot) return true;
        }
        return false;
    }
    public bool HasRingSlotInDeck(NationsEnum nation, RingType objSlot)
    {
        CardsOfPlayer cards = cardsOfPlayer.Find(x => x.GetNation() == nation);
        if (!cards.HasCards()) return false;

        if (!HasCardInDeck(nation, CardClass.Ring)) return false;

        for (int i = 0; i < cards.GetHandSize(); i++)
        {
            if (GetHandCardGameObject(nation,i) == null)
                continue;
            if (GetHandCardGameObject(nation,i).TryGetComponent<RingCardDetails>(out var details))
                if (details.objectSlot == objSlot) return true;
        }
        return false;
    }

    public List<CardDetails> GetCardsInHandOfType(CardClass cardClass, NationsEnum owner)
    {
        CardsOfPlayer cards = cardsOfPlayer.Find(x => x.GetNation() == owner);
        return cards.GetCardsInHandOfType(cardClass);
    }
}
