using System;
using System.Collections.Generic;
using UnityEngine;

public class CardsOfPlayer: MonoBehaviour
{
    [SerializeField]
    private short handSize = 5;
    [SerializeField]
    private NationsEnum nation;
    [SerializeField]
    private InitialDeck initialDeck;
    [SerializeField]
    private List<CardDetails> drawnCards;

    private Transform handTransform;

    [SerializeField]
    private List<CardDetails> discardPile;
    [SerializeField]
    private List<CardDetails> wonPile;
    [SerializeField]
    private int lastCardDrawn;
    [SerializeField]
    private bool hasCards;

    private GameObject deckCardUIPrefab;
    private List<string> startsWithId;

    private Board board;
    void Awake()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
    }

    public void Initialize(
        NationsEnum nation,
        Transform handTransform,
        GameObject deckCardUIPrefab,
        short handSize = 5,
        List<string> startsWithId = null)
    {
        initialDeck = GameObject.Find("cards_" + nation.ToString().ToLower()).GetComponent<InitialDeck>();            
        drawnCards = new();
        discardPile = new();
        wonPile = new();
        lastCardDrawn = -1;
        hasCards = initialDeck.cards.Count > 0;
        this.nation = nation;
        this.handSize = handSize;
        this.handTransform = handTransform;
        this.deckCardUIPrefab = deckCardUIPrefab;
        this.startsWithId = startsWithId;
        Shuffle();
        DrawHand();
    }

    private void Shuffle()
    {
        if (!hasCards)
            return;

        for (int i = 0; i < initialDeck.cards.Count; i++)
        {
            int rnd = UnityEngine.Random.Range(0, initialDeck.cards.Count);
            ListExtensions.Swap(initialDeck.cards, i, rnd);
        }

        int startsWithIter = 0;
        if(startsWithId != null)
        {
            foreach (string cardId in startsWithId)
            {
                try
                {
                    GameObject go = initialDeck.cards.Find(x => x.GetComponent<CardDetails>() != null && x.GetComponent<CardDetails>().cardId == cardId);
                    if (go != null)
                    {
                        int pos = initialDeck.cards.IndexOf(go);
                        if (pos != -1)
                            ListExtensions.Swap(initialDeck.cards, pos, startsWithIter);
                    }
                }
                catch (Exception)
                {
                    Debug.LogWarning(string.Format("Unable to Initialize the Deck with {0}", cardId));
                }                    
                startsWithIter++;
            }
        }        
    }

    public void DrawHand()
    {
        if (!hasCards)
            return;

        for (int i = 0; i < handSize; i++)
            Draw();
    }


    public GameObject GetHandCardGameObject(int cardShown)
    {
        if (handTransform == null)
            return null;

        if (handTransform.childCount <= cardShown || cardShown < 0)
            return null;

        return handTransform.GetChild(handTransform.childCount - 1 - cardShown).gameObject;
    }

    public void Draw()
    {
        if (!hasCards)
            return;

        CardDetails nextCard;
        while (true)
        {
            // This is the counter of cards drawn from the Deck (all cards)
            lastCardDrawn = (lastCardDrawn + 1) % initialDeck.cards.Count;

            nextCard = initialDeck.cards[lastCardDrawn].GetComponent<CardDetails>();
            if (!nextCard.isUnique || board.GetCardManager().GetCardUI(nextCard.cardId) == null)
                break;
        }        

        drawnCards.Add(nextCard);

        if(handTransform != null)
            CreateCard(initialDeck.cards[lastCardDrawn]);

        for (int i = handSize - 1; i > 0; i--)
        {
            if (GetHandCardGameObject(i - 1) != null)
            {
                if (GetHandCardGameObject(i - 1).TryGetComponent<DeckCardUI>(out var deckCard))
                    deckCard.IncreaseHandPosition();
            }
        }
    }
    public void CreateCard(GameObject definitionCard)
    {
        CardDetails cardDetails = definitionCard.GetComponent<CardDetails>();

        // I instantiate the Frame
        GameObject instantiatedCardTemplate = Instantiate(deckCardUIPrefab, handTransform);
        instantiatedCardTemplate.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        instantiatedCardTemplate.GetComponent<RectTransform>().localScale = Vector3.one;
        instantiatedCardTemplate.name = cardDetails.name;

        DeckCardUI deckCard = instantiatedCardTemplate.GetComponent<DeckCardUI>();
        deckCard.Initialize(nation, cardDetails.cardId, cardDetails.cardClass, false);
    }
    public NationsEnum GetNation()
    {
        return nation;
    }
    public void AddToWonPile(CardDetails details)
    {
        wonPile.Add(details);
    }
    public void AddToDiscardPile(CardDetails details)
    {
        discardPile.Add(details);
    }

    public short GetHandSize()
    {
        return handSize;
    }

    public bool HasCards()
    {
        return hasCards;
    }

    public void DiscardAndDraw(CardDetails card, bool discarded)
    {
        if (!hasCards)
            return;

        int index = -1;
        short handPos = -1;
        for (int i = 0; i < handSize; i++)
        {
            if (GetHandCardGameObject(i) == null)
                continue;
            string cardId = GetHandCardGameObject(i).GetComponent<CardTemplateUI>().GetCardDetails().cardId;
            if (cardId == card.cardId)
            {
                index = i;
                handPos = GetHandCardGameObject(i).GetComponent<DeckCardUI>().GetHandPos();
                break;
            }
        }
        if (index == -1 || handPos == -1)
        {
            Debug.LogError("Unable to discard card " + card.cardId);
            return;
        }

        // Destroy the card from hand
        Destroy(GetHandCardGameObject(index));

        if (discarded)
            AddToDiscardPile(card);

        //For all the cards before, I increase the counter
        for (int i = handPos - 1; i >= 0; i--)
            GetHandCardGameObject(i).GetComponent<DeckCardUI>().IncreaseHandPosition();

        // This is the counter of cards drawn from the Deck (all cards)
        lastCardDrawn = (lastCardDrawn + 1) % initialDeck.cards.Count;

        CreateCard(initialDeck.cards[lastCardDrawn]);
    }

    public List<CardDetails> GetCardsInHandOfType(CardClass cardClass)
    {
        return drawnCards.FindAll(x => x.cardClass == cardClass);
    }
}
