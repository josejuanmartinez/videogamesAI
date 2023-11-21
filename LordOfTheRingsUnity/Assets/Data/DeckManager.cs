using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;


public class DeckManager : MonoBehaviour
{
    public GameObject handPrefab;
    public Transform deckTransform;
    public CanvasGroup deckCanvasGroup;
    public GameObject deckCardUIPrefab;
    public short handSize = 5;

    public List<string> startWithId;
        
    private Dictionary<NationsEnum, GameObject> hands;
    private Dictionary<NationsEnum, List<GameObject>> cards;
    private Dictionary<NationsEnum, List<CardDetails>> drawnCards;
    public  Dictionary<NationsEnum, List<CardDetails>> discardPile;
    public Dictionary<NationsEnum, List<CardDetails>> wonPile;
    public Dictionary<NationsEnum, int> lastCardDrawn;
    public Dictionary<NationsEnum, bool> hasCards;
    
    private Board board;
    private PlaceDeck placeDeckManager;
    private Turn turn;
    private CardDetailsRepo cardRepo;  
    private SelectedItems selectedItems;
    private CameraController cameraController;

    private List<DirtyReasonEnum> isDirty;
    private string loadingPlayer;
    private bool isInitialized;
    void Awake()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
        placeDeckManager = GameObject.Find("PlaceDeckManager").GetComponent<PlaceDeck>();
        turn = GameObject.Find("Turn").GetComponent<Turn>();
        cardRepo = GameObject.Find("CardDetailsRepo").GetComponent<CardDetailsRepo>();
        selectedItems = GameObject.Find("SelectedItems").GetComponent<SelectedItems>();
        cameraController = GameObject.Find("CameraController").GetComponent<CameraController>();

        lastCardDrawn = new();
        hasCards = new();

        drawnCards = new();        
        cards = new();
        discardPile = new();
        wonPile = new();

        hands = new();

        isDirty = new();
        hasCards.Add(NationsEnum.ABANDONED, false);

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
        foreach(NationsEnum nation in Enum.GetValues(typeof(NationsEnum))) 
        {
            loadingPlayer = nation.ToString();
            if (nation == NationsEnum.ABANDONED)
                continue;
            if (hands.ContainsKey(nation))
                continue;
            hands[nation] = Instantiate(handPrefab, deckTransform);
            hands[nation].name = nation.ToString();
            
            lastCardDrawn[nation] = -1;

            drawnCards[nation] = new();
            cards[nation] = new();
            discardPile[nation] = new();
            wonPile[nation] = new();

            List<string> cardNames = cardRepo.GetCardsOfNation(nation);
            if (cardNames.Count < handSize)
            {
                hasCards.Add(nation, false);
                HideHandOf(nation);
                continue;
            } 
            else
                hasCards.Add(nation, true);
                
            cards[nation] = cardNames.Select(x => cardRepo.GetCardGameObject(x, nation)).ToList();
            for(int i=0; i < cards[nation].Count; i++)
            {
                if (cards[nation][i] == null)
                {
                    Debug.LogWarning(string.Format("Something is wrong with {0}'s card {1}",
                        nation.ToString(),
                        cardNames[i]));
                    cards[nation].RemoveAt(i);
                }
            }
            Shuffle(nation);
            DrawHand(nation);
            //Debug.Log("Loaded " + cards[nation].Count + " cards for " + nation.ToString());

            if (turn.GetCurrentPlayer() != nation)
                HideHandOf(nation);
        }
        
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
        wonPile[owner].Add(details);
    }
    public void AddToDiscardPile(NationsEnum owner, CardDetails details)
    {
        discardPile[owner].Add(details);
    }    

    void Update()
    {
        deckCanvasGroup.alpha = cameraController.IsPreventedDrag()? 0 : 1;
        deckCanvasGroup.interactable = !cameraController.IsPreventedDrag();

        if (!isInitialized)
        {
            Initialize();
            return;
        }
        if(isDirty.Count() > 0)
        {
            for (int i = 0; i< handSize; i++)
                if (GetHandCard(turn.GetCurrentPlayer(), i) != null)
                    GetHandCard(turn.GetCurrentPlayer(), i).GetComponent<CardTemplateUI>().Dirty(isDirty);
            isDirty = new();
        }
    }

    private void HideHandOf(NationsEnum owner)
    {
        hands[owner].SetActive(false);
    }

    public void ShowHandOf(NationsEnum owner)
    {
        foreach (NationsEnum nation in Enum.GetValues(typeof(NationsEnum)))
        {
            hands[owner].SetActive(nation == owner);
        }   
    }
    private GameObject GetHandCard(NationsEnum nation, int cardShown)
    {
        if (!hasCards[nation])
            return null;
        if (hands[nation].transform.childCount <= cardShown || cardShown < 0)
            return null;

        return hands[nation].transform.GetChild(hands[nation].transform.childCount - 1 - cardShown).gameObject;
    }

    private void Shuffle(NationsEnum nation)
    {
        if (!hasCards[nation])
            return;

        for (int i = 0; i < cards[nation].Count; i++)
        {
            int rnd = UnityEngine.Random.Range(0, cards[nation].Count);
            ListExtensions.Swap(cards[nation], i, rnd);
        }

        int startsWithIter = 0;
        foreach (string cardId in startWithId)
        {
            try
            {
                GameObject go = cards[nation].Find(x => x.GetComponent<CardDetails>() != null && x.GetComponent<CardDetails>().cardId == cardId);
                if (go != null)
                {
                    int pos = cards[nation].IndexOf(go);
                    if (pos != -1)
                        ListExtensions.Swap(cards[nation], pos, startsWithIter);
                }
            }
            catch (Exception)
            {
                Debug.LogWarning(string.Format("Unable to Initialize the Deck with {0}", cardId));
            }
            startsWithIter++;
        }
    }

    public void DrawHand(NationsEnum nation)
    {
        if (!hasCards[nation])
            return;

        for (int i= 0; i < handSize; i++)
            Draw(nation);
    }

    public void Draw(NationsEnum nation)
    {
        if (!hasCards[nation])
            return;

        // This is the counter of cards drawn from the Deck (all cards)
        lastCardDrawn[nation] = (lastCardDrawn[nation] + 1) % cards[nation].Count;

        drawnCards[nation].Add(cards[nation][lastCardDrawn[nation]].GetComponent<CardDetails>());

        CreateCard(nation, cards[nation][lastCardDrawn[nation]], hands[nation].transform);

        for (int i = handSize - 1; i > 0; i--)
        {
            if (GetHandCard(nation, i - 1) != null)
            {
                if (GetHandCard(nation, i - 1).TryGetComponent<DeckCardUI>(out var deckCard))
                    deckCard.IncreaseHandPosition();
            }
        }
    }

    public void CreateCard(NationsEnum nation, GameObject definitionCard, Transform parentTransform)
    {
        Assert.IsTrue(definitionCard.GetComponent<CardDetails>() != null);
        CardDetails cardDetails = definitionCard.GetComponent<CardDetails>();

        // I instantiate the Frame
        GameObject instantiatedCardTemplate = Instantiate(deckCardUIPrefab, parentTransform);
        instantiatedCardTemplate.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        instantiatedCardTemplate.GetComponent<RectTransform>().localScale = Vector3.one;
        instantiatedCardTemplate.name = cardDetails.name;

        DeckCardUI deckCard = instantiatedCardTemplate.GetComponent<DeckCardUI>();
        deckCard.Initialize(nation, cardDetails.cardId, cardDetails.cardClass, false);
    }

    public void DiscardAndDraw(NationsEnum nation, CardDetails card, bool discarded)
    {
        if (!hasCards[nation])
            return;

        int index = -1;
        short handPos = -1;
        for(int i = 0; i < handSize; i++)
        {
            if (GetHandCard(nation,i) == null)
                continue;
            string cardId = GetHandCard(nation, i).GetComponent<CardTemplateUI>().GetCardDetails().cardId;
            if (cardId == card.cardId)
            {
                index = i;
                handPos = GetHandCard(nation, i).GetComponent<DeckCardUI>().GetHandPos();
                break;
            }                    
        }
        if(index == -1 || handPos == -1)
        {
            Debug.LogError("Unable to discard card " + card.cardId);
            placeDeckManager.RemoveCardToShow(new HoveredCard(nation, card.cardId, card.cardClass));
            return;
        }
        
        // Destroy the card from hand
        Destroy(GetHandCard(nation, index));

        if (discarded)
            AddToDiscardPile(nation, card);

        //For all the cards before, I increase the counter
        for (int i = handPos - 1; i >= 0; i--)
            GetHandCard(nation, i).GetComponent<DeckCardUI>().IncreaseHandPosition();

        // This is the counter of cards drawn from the Deck (all cards)
        lastCardDrawn[nation] = (lastCardDrawn[nation] + 1) % cards[nation].Count;

        CreateCard(nation,cards[nation][lastCardDrawn[nation]], hands[nation].transform);
        placeDeckManager.RemoveCardToShow(new HoveredCard(nation, card.cardId, card.cardClass));
    }


    public string CanSpawnCharacterAtHome(CardDetails cardDetails, NationsEnum owner)
    {
        if (owner == NationsEnum.ABANDONED)
            return null;

        if (!cardDetails.IsClassOf(CardClass.Character))
            return null;
        
        if (!hasCards.ContainsKey(owner)) return null;

        CharacterCardDetails character = cardDetails as CharacterCardDetails;

        if (character  == null) 
            return null;

        if (board.GetCityManager().GetCityOfPlayer(owner, character.homeTown) != null)
            return character.homeTown;

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
        if (!hasCards[nation]) return false;

        for (int i = 0; i < handSize; i++)
        {
            if (GetHandCard(nation, i) == null)
                continue;
            
            if (GetHandCard(nation, i).GetComponentInChildren<DeckCardUI>() == null)
                continue;
            DeckCardUI deckCard = GetHandCard(nation, i).GetComponentInChildren<DeckCardUI>();
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
        if (!hasCards[nation]) return false;

        if (!HasCardInDeck(nation, CardClass.Object)) return false; 

        for (int i = 0; i < handSize; i++)
        {
            if (GetHandCard(nation, i) == null)
                continue;

            if(GetHandCard(nation, i).TryGetComponent<ObjectCardDetails>(out var details))
                if (details.objectSlot == objSlot) return true;
        }
        return false;
    }
    public bool HasRingSlotInDeck(NationsEnum nation, RingType objSlot)
    {
        if (!hasCards[nation]) return false;

        if (!HasCardInDeck(nation, CardClass.Ring)) return false;

        for (int i = 0; i < handSize; i++)
        {
            if (GetHandCard(nation,i) == null)
                continue;
            if (GetHandCard(nation,i).TryGetComponent<RingCardDetails>(out var details))
                if (details.objectSlot == objSlot) return true;
        }
        return false;
    }

    public List<CardDetails> GetCardsInHandOfType(CardClass cardClass, NationsEnum owner)
    {
        return drawnCards[owner].FindAll(x => x.cardClass == cardClass);
    }
}
