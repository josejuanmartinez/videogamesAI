using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardDetailsRepo : MonoBehaviour
{
    private List<InitialDeck> nationsInitialDecks;
    private List<CardDetails> allCardDetails;
    private Dictionary<NationsEnum, List<string>> cardNationDictionary;
    private Dictionary<NationsEnum, List<CardDetails>> cardNationDetailsDictionary;

    private List<CardDetails> cardsPendingInitialization;

    private bool isInitialized;

    void Awake()
    {
        allCardDetails = new();
        cardNationDictionary = new();
        cardNationDetailsDictionary = new();
        isInitialized = false;
        cardsPendingInitialization = new();
        nationsInitialDecks = new();
    }

    public bool Initialize()
    {
        bool ready = true;
#if UNITY_EDITOR
        ready = GetComponent<PrefabLoader>().IsInitialized();
#endif
        if (!ready)
            return ready;
        for(int i=0; i<transform.childCount; i++)
            nationsInitialDecks.Add(transform.GetChild(i).GetComponent<InitialDeck>());
        
        foreach (NationsEnum nation in Enum.GetValues(typeof(NationsEnum)))
        {
            InitialDeck initial = nationsInitialDecks.Find(x => x.owner == nation);
            if (initial == null)
            {
                Debug.LogWarning(string.Format("Deck for nation {0} not configured", nation.ToString()));
                continue;
            }
            foreach (GameObject go in initial.cards)
            {
                if (go.GetComponent<CardDetails>() == null)
                {
                    Debug.LogWarning(string.Format("{0} does not have a CardDetails component!", go.name));
                    continue;
                }
                CardDetails cardDetails = go.GetComponent<CardDetails>();
                cardsPendingInitialization.Add(cardDetails);
                NationsEnum owner = nation;

                allCardDetails ??= new();
                allCardDetails.Add(cardDetails);

                if (!cardNationDictionary.ContainsKey(owner))
                    cardNationDictionary[owner] = new();
                cardNationDictionary[owner].Add(cardDetails.cardId);

                if (!cardNationDetailsDictionary.ContainsKey(owner))
                    cardNationDetailsDictionary[owner] = new();
                cardNationDetailsDictionary[owner].Add(cardDetails);
            }
        }

        /*foreach (GameObject go in cityDetailsPrefabs)
        {
            CityDetails cityDetails = go.GetComponent<CityDetails>();
            NationsEnum owner = cityDetails.initialOwner;
            if (!cityNationDetailsDictionary.ContainsKey(owner))
                cityNationDetailsDictionary[owner] = new ();
            cityNationDetailsDictionary[owner].Add(cityDetails);
        }*/

        isInitialized = true;
        //Debug.Log("CardDetailsRepo initialized at " + Time.realtimeSinceStartup);
        return isInitialized;
    }

    void Update()
    {
        if (!isInitialized)
        {
            Initialize();
            return;
        }

        List<CardDetails> pending = new();
        foreach(CardDetails cardDetails in cardsPendingInitialization)
        {
            if (!InitializeCardDetails(cardDetails))
                pending.Add(cardDetails);
            //else
            //    Debug.Log(string.Format("{0} initialized itself!", cardDetails.cardId));
        }
            
        cardsPendingInitialization = pending;
    }

    public bool InitializeCardDetails(CardDetails cardDetails)
    {
        bool res = false;
        if (cardDetails as CharacterCardDetails != null)
            res = (cardDetails as CharacterCardDetails).Initialize();
        if (cardDetails as HazardCreatureCardDetails != null)
            res = (cardDetails as HazardCreatureCardDetails).Initialize();
        if (cardDetails as ObjectCardDetails != null)
            res = (cardDetails as ObjectCardDetails).Initialize();
        if (cardDetails as AllyCardDetails != null)
            res = (cardDetails as AllyCardDetails).Initialize();
        if (cardDetails as EventCardDetails != null)
            res = (cardDetails as EventCardDetails).Initialize();
        if (cardDetails as HazardEventCardDetails != null)
            res = (cardDetails as HazardEventCardDetails).Initialize();
        if (cardDetails as RingCardDetails != null)
            res = (cardDetails as RingCardDetails).Initialize();
        if (cardDetails as GoldRingDetails != null)
            res = (cardDetails as GoldRingDetails).Initialize();
        if (cardDetails as FactionCardDetails != null)
            res = (cardDetails as FactionCardDetails).Initialize();
        return res;
    }

    public GameObject GetCardGameObject(string cardId, NationsEnum owner)
    {
        InitialDeck initial = nationsInitialDecks.Find(x => x.owner == owner);
        if (initial == null)
        {
            Debug.LogWarning(string.Format("Deck for nation {0} not configured", owner.ToString()));
            return null;
        }
        return initial.cards.Find(x => x.GetComponent<CardDetails>() != null && x.GetComponent<CardDetails>().cardId == cardId);
    }

    public CardDetails GetCardDetails(string cardId, NationsEnum owner)
    {
        if (GetCardGameObject(cardId, owner) == null)
            return null;
        return GetCardGameObject(cardId, owner).GetComponent<CardDetails>();
    }

    public CardDetails GetCardDetails(Tuple<string, NationsEnum> cardTuple)
    {
        return GetCardDetails(cardTuple.Item1, cardTuple.Item2);
    }
    public bool IsInitialized() 
    { 
        return isInitialized; 
    }

    public List<string> GetCardsOfNation(NationsEnum nation)
    {
        if (cardNationDictionary.ContainsKey(nation))
            return cardNationDictionary[nation];
        else
            return new List<string>();
    }


    public List<HazardCreatureCardDetails> GetHazardCardsOfNation(NationsEnum nation)
    {
        if (cardNationDictionary.ContainsKey(nation))
            return cardNationDetailsDictionary[nation].FindAll(x => (x as HazardCreatureCardDetails != null)).Select(x => x as HazardCreatureCardDetails).ToList();
        else
            return new List<HazardCreatureCardDetails>();
    }
    
}
