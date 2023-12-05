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

    public List<GameObject> toActivate;
    public List<TilemapRenderer> gridsToEnable;

    public List<CardClass> difficultyClasses;
    public List<int> baseDifficulty;

    private List<Player> players;

    private DeckManager deckManager;
    private Settings settings;

    private bool isInitialized;
    private bool finishedLoading;
    private bool shownLoadingMessage;
    void Awake()
    {
        players = new();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        settings = GameObject.Find("Settings").GetComponent<Settings>();
        isInitialized = false;
        finishedLoading = false;
        shownLoadingMessage = false;
    }

    public void Initialize()
    {
        foreach (GameObject go in toActivate)
            if (!go.activeSelf)
                go.SetActive(true);

        foreach (TilemapRenderer tilemap in gridsToEnable)
            tilemap.enabled = true;

        for (int i = 0; i < Enum.GetValues(typeof(NationsEnum)).Length; i++)
        {
            if ((NationsEnum)i == NationsEnum.ABANDONED)
                continue;
            players.Add(new Player((NationsEnum)i, ((NationsEnum)i == settings.GetHumanPlayer())));
        }
        isInitialized = true;
        //Debug.Log("Game initialized at " + Time.realtimeSinceStartup);
    }
    public Player GetHumanPlayer()
    {
        return players.First(x => x.isHuman);
    }

    public NationsEnum GetHumanNation()
    {
        return settings.GetHumanPlayer();
    }

    public bool IsInitialized() {
        return isInitialized;
    }

    public short RequiredDice(CardClass cardClass)
    {
        short res = (short)baseDifficulty[(int)cardClass];
        switch (settings.GetDifficulty())
        {
            case DifficultiesEnum.Medium:
                res += 1;
                break;
            case DifficultiesEnum.Hard:
                res += 2;
                break;
        }
        return res;
    }

    public float GetMultiplierByDifficulty()
    {
        return settings.GetDifficulty() switch
        {
            DifficultiesEnum.Easy => 1f,
            DifficultiesEnum.Medium => 1.5f,
            DifficultiesEnum.Hard => 2f,
            _ => 1f,
        };
    }
    public float GetCriticalByDifficulty()
    {
        return settings.GetDifficulty() switch
        {
            DifficultiesEnum.Easy => 0.9f,
            DifficultiesEnum.Medium => 0.85f,
            DifficultiesEnum.Hard => 0.8f,
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

        switch (settings.GetDifficulty())
        {
            case DifficultiesEnum.Medium:
                goldCost += 10;
                break;
            case DifficultiesEnum.Hard:
                goldCost += 25;
                break;
        }

        return goldCost;
    }

    public int GetCorruptionBaseByDifficulty()
    {
        if (!IsInitialized())
            throw new Exception("Trying to get Corruption of card but it's not initialized!");

        return settings.GetDifficulty() switch
        {
            DifficultiesEnum.Medium => 1,
            DifficultiesEnum.Hard => 2,
            _ => 0,
        };
    }

    private void Update()
    {
        if (!isInitialized)
        {
            Initialize();
            return;
        }
            

        if (finishedLoading)
            return;

        if(deckManager.IsInitialized())
        {
            Debug.Log(string.Format("Loading finished at {0}", Time.realtimeSinceStartup));
            finishedLoading = true;
            loadManager.SetActive(false);
        }            
        else if (!shownLoadingMessage)
        {
            loadingText.text = string.Format(
                "{0}...",
                GameObject.Find("Localization").GetComponent<Localization>().Localize("loading")
                );
            shownLoadingMessage = true;
        }
    }

    public bool FinishedLoading()
    {
        return finishedLoading;
    }
}
