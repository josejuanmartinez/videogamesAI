using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Game : MonoBehaviour
{
    public GameObject loadManager;
    public TextMeshProUGUI loadingText;

    public List<Difficulties> difficulties;
    public Difficulties currentDifficulty;

    public List<GameObject> toActivate;
    public List<TilemapRenderer> gridsToEnable;

    public NationsEnum humanPlayer;

    public List<CardClass> difficultyClasses;
    public List<int> baseDifficulty;

    private List<Player> players;

    private DeckManager deckManager;

    private bool isInitialized = false;
    private bool finishedLoading = false;
    void Awake()
    {
        players = new();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        foreach(GameObject go in toActivate)
        {
            if (!go.activeSelf)
                go.SetActive(true);
        }

        foreach(TilemapRenderer tilemap in gridsToEnable)
        {
            tilemap.enabled = true;
        }
        
        for(int i=0; i<Enum.GetValues(typeof(NationsEnum)).Length; i++)
        {
            if ((NationsEnum)i == NationsEnum.ABANDONED)
                continue;
            players.Add(new Player((NationsEnum)i, ((NationsEnum)i==humanPlayer)));
        }
        isInitialized = true;
        Debug.Log("Game initialized");
    }
    public Player GetHumanPlayer()
    {
        return players.First(x => x.isHuman);
    }

    public bool IsInitialized() {
        return isInitialized;
    }

    public short RequiredDice(CardClass cardClass)
    {
        short res = (short)baseDifficulty[(int)cardClass];
        switch (currentDifficulty)
        {
            case Difficulties.Medium:
                res += 1;
                break;
            case Difficulties.Hard:
                res += 2;
                break;
        }
        return res;
    }

    public float GetMultiplierByDifficulty()
    {
        return currentDifficulty switch
        {
            Difficulties.Easy => 1f,
            Difficulties.Medium => 1.5f,
            Difficulties.Hard => 2f,
            _ => 1f,
        };
    }
    public float GetCriticalByDifficulty()
    {
        return currentDifficulty switch
        {
            Difficulties.Easy => 0.9f,
            Difficulties.Medium => 0.85f,
            Difficulties.Hard => 0.8f,
            _ => 0.9f,
        };
    }

    public float GetCostsByDifficulty(CardClass cardClass)
    {
        int goldCost = 0;
        switch (cardClass)
        {
            case CardClass.Place:
                break;
            case CardClass.Character:
                goldCost = 25;
                break;
            case CardClass.Object:
                goldCost = 15;
                break;
            case CardClass.Faction:
                goldCost = 25;
                break;
            case CardClass.Event:
                goldCost = 25;
                break;
            case CardClass.HazardEvent:
                goldCost = 25;
                break;
            case CardClass.HazardCreature:
                goldCost = 25;
                break;
            case CardClass.Ally:
                goldCost = 25;
                break;
            case CardClass.GoldRing:
                goldCost = 50;
                break;
            case CardClass.Ring:
                goldCost = 25;
                break;
            case CardClass.NONE:
                break;
        }

        switch (currentDifficulty)
        {
            case Difficulties.Medium:
                goldCost += 10;
                break;
            case Difficulties.Hard:
                goldCost += 25;
                break;
        }

        return goldCost;
    }

    public int GetCorruptionBaseByDifficulty()
    {
        return currentDifficulty switch
        {
            Difficulties.Medium => 1,
            Difficulties.Hard => 2,
            _ => 0,
        };
    }

    private void Update()
    {
        if (finishedLoading)
            return;

        if(deckManager.IsInitialized())
        {
            finishedLoading = true;
            loadManager.SetActive(false);
        }            
        else
        {
            loadingText.text = string.Format(
                "{0}...",
                GameObject.Find("Localization").GetComponent<Localization>().Localize("loading")
                );
        }
    }
}
