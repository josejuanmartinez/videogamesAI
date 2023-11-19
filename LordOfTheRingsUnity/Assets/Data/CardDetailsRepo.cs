using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardDetailsRepo : MonoBehaviour
{
    [Header("Deck Cards by Player")]
    public List<InitialDeck> nationsInitialDecks;
    
    [Header("Initial Cities")]
    public List<GameObject> cityDetailsPrefabs;

    private List<CardDetails> allCardDetails;
    private Dictionary<NationsEnum, List<string>> cardNationDictionary;
    private Dictionary<NationsEnum, List<CardDetails>> cardNationDetailsDictionary;

    private bool isInitialized = false;

    void Awake()
    {
        cardNationDictionary = new();
        cardNationDetailsDictionary = new();

        foreach(NationsEnum nation in Enum.GetValues(typeof(NationsEnum)))
        {
            InitialDeck initial = nationsInitialDecks.Find(x => x.owner == nation);
            if(initial == null)
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
                InitializeCardDetails(cardDetails);
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
        Debug.Log("CardDetailsRepo initialized");
    }

    public void InitializeCardDetails(CardDetails cardDetails)
    {
        if (cardDetails as CharacterCardDetails != null)
            (cardDetails as CharacterCardDetails).Initialize();
        if (cardDetails as HazardCreatureCardDetails != null)
            (cardDetails as HazardCreatureCardDetails).Initialize();
        if (cardDetails as ObjectCardDetails != null)
            (cardDetails as ObjectCardDetails).Initialize();
        if (cardDetails as AllyCardDetails != null)
            (cardDetails as AllyCardDetails).Initialize();
        if (cardDetails as EventCardDetails != null)
            (cardDetails as EventCardDetails).Initialize();
        if (cardDetails as HazardEventCardDetails != null)
            (cardDetails as HazardEventCardDetails).Initialize();
        if (cardDetails as RingCardDetails != null)
            (cardDetails as RingCardDetails).Initialize();
        if (cardDetails as GoldRingDetails != null)
            (cardDetails as GoldRingDetails).Initialize();
        if (cardDetails as FactionCardDetails != null)
            (cardDetails as FactionCardDetails).Initialize();
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

    public GameObject GetCityGameObject(string cardId)
    {
        return cityDetailsPrefabs.Find(x => x.name == cardId);
    }

    public CardDetails GetCardDetails(string cardId, NationsEnum owner)
    {
        if (GetCardGameObject(cardId, owner) == null)
            return null;
        return GetCardGameObject(cardId, owner).GetComponent<CardDetails>();
    }

    public CityDetails GetCityDetails(string cardId)
    {
        if (GetCityGameObject(cardId) == null)
            return null;
        return GetCityGameObject(cardId).GetComponent<CityDetails>();
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
