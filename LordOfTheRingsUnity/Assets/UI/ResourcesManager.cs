using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using TMPro;
using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
    public TextMeshProUGUI food;
    public TextMeshProUGUI clothes;
    public TextMeshProUGUI gold;
    public TextMeshProUGUI wood;
    public TextMeshProUGUI metal;
    public TextMeshProUGUI gems;
    public TextMeshProUGUI horses;
    public TextMeshProUGUI leather;
    public TextMeshProUGUI influence;

    private Board board;
    private Turn turn;
    private Game game;
    private DeckManager deckManager;
    private SelectedItems selectedItems;

    private readonly Dictionary<NationsEnum, Resources> stores = new ();
    private readonly Dictionary<NationsEnum, Resources> productions = new ();
    private readonly Dictionary<NationsEnum, int> influences = new ();

    private bool initialized;

    void Awake()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
        turn = GameObject.Find("Turn").GetComponent<Turn>();
        game = GameObject.Find("Game").GetComponent<Game>();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        selectedItems = GameObject.Find("SelectedItems").GetComponent<SelectedItems>();
        initialized = false;
    }

    IEnumerator Initialize()
    {
        foreach(NationsEnum nation in Enum.GetValues (typeof (NationsEnum)))
        {
            RecalculatePlayerCitiesProduction(nation);
            RecalculateInfluence(nation);
        }
        initialized = true;
        yield return null;
    }

    public void Add(CityUI city, bool recalculate = true)
    {
        NationsEnum nation = city.GetOwner();
        if (nation == NationsEnum.ABANDONED)
            return;

        AddToStores(nation, city.GetCityProduction(), recalculate);

        // Prod is not stored
        Resources cityProd = stores[nation];
        stores[nation] = cityProd;

        if(recalculate)
        {
            RecalculatePlayerCitiesProduction(nation);
            RecalculateInfluence(nation);
        }
        
    }

    public void AddToStores(NationsEnum nation, Resources cityProduction, bool recalculate = true)
    {
        if (nation == NationsEnum.ABANDONED)
            return;

        if (!stores.ContainsKey(nation))
            stores.Add(nation, cityProduction);
        else
            stores[nation] += cityProduction;       

        if(recalculate)
        {
            RefreshCityProductionStats(nation);
            deckManager.Dirty(DirtyReasonEnum.NEW_RESOURCES);
        }        
    }

    public void RecalculatePlayerCitiesProduction(NationsEnum nation)
    {
        if (nation != game.GetHumanNation())
            return;

        List<CityUI> cities = board.GetCityManager().GetCitiesOfPlayer(nation);
        if (cities == null)
            return;
        int foodProd = 0;
        int clothesProd = 0;
        int goldProd = 0;
        int woodProd = 0;
        int metalProd = 0;
        int horsesProd = 0;
        int gemsProd = 0;
        int leatherProd = 0;

        foreach (CityUI city in cities)
        {
            Resources cityProduction = city.GetCityProduction();
            foodProd += cityProduction.resources[ResourceType.FOOD];
            clothesProd += cityProduction.resources[ResourceType.CLOTHES];
            goldProd += cityProduction.resources[ResourceType.GOLD];
            metalProd += cityProduction.resources[ResourceType.METAL];
            woodProd += cityProduction.resources[ResourceType.WOOD];
            horsesProd += cityProduction.resources[ResourceType.MOUNTS];
            gemsProd += cityProduction.resources[ResourceType.GEMS];
            leatherProd += cityProduction.resources[ResourceType.LEATHER];
        }
        productions[nation] = new Resources(foodProd, goldProd, clothesProd, woodProd, metalProd, horsesProd, gemsProd, leatherProd);

        RefreshCityProductionStats(nation);
    }
    public void RefreshCityProductionStats(NationsEnum nation)
    {
        if (nation != game.GetHumanNation())
            return;

        if (!productions.ContainsKey(nation))
            RecalculatePlayerCitiesProduction(nation);

        int foodBonus = productions[nation].resources[ResourceType.FOOD];
        int clothesBonus = productions[nation].resources[ResourceType.CLOTHES];
        int goldBonus = productions[nation].resources[ResourceType.GOLD];
        int woodBonus = productions[nation].resources[ResourceType.WOOD];
        int metalBonus = productions[nation].resources[ResourceType.METAL];
        int gemsBonus = productions[nation].resources[ResourceType.GEMS];
        int horsesBonus = productions[nation].resources[ResourceType.MOUNTS];
        int leatherBonus = productions[nation].resources[ResourceType.LEATHER];
        food.text = (foodBonus > 0 ? "+" : "<color=\"red\">") + foodBonus.ToString() + (foodBonus > 0 ? "" : "</color=\"red\">") + "/" + stores[nation].resources[ResourceType.FOOD].ToString();
        clothes.text = (clothesBonus > 0 ? "+" : "<color=\"red\">") + clothesBonus.ToString() + (clothesBonus > 0 ? "" : "</color=\"red\">") + "/" + stores[nation].resources[ResourceType.CLOTHES].ToString();
        gold.text = (goldBonus > 0 ? "+" : "<color=\"red\">") + goldBonus.ToString() + (goldBonus > 0 ? "" : "</color=\"red\">") + "/" + stores[nation].resources[ResourceType.GOLD].ToString();
        wood.text = (woodBonus > 0 ? "+" : "<color=\"red\">") + woodBonus.ToString() + (woodBonus > 0 ? "" : "</color=\"red\">") + "/" + stores[nation].resources[ResourceType.WOOD].ToString();
        metal.text = (metalBonus > 0 ? "+" : "<color=\"red\">") + metalBonus.ToString() + (metalBonus > 0 ? "" : "</color=\"red\">") + "/" + stores[nation].resources[ResourceType.METAL].ToString();
        gems.text = (gemsBonus > 0 ? "+" : "<color=\"red\">") +  gemsBonus.ToString() + (gemsBonus > 0 ? "" : "</color=\"red\">") + "/" + stores[nation].resources[ResourceType.GEMS].ToString();
        horses.text = (horsesBonus > 0 ? "+" : "<color=\"red\">") + horsesBonus.ToString() + (horsesBonus > 0 ? "" : "</color=\"red\">") + "/" + stores[nation].resources[ResourceType.MOUNTS].ToString();
        leather.text = (leatherBonus > 0 ? "+" : "<color=\"red\">") + leatherBonus.ToString() + (leatherBonus > 0 ? "" : "</color=\"red\">") + "/" + stores[nation].resources[ResourceType.LEATHER].ToString();
    }

    private void Update()
    {
        if (!initialized && board.IsAllLoaded()) 
            StartCoroutine(Initialize());
    }

    public void RecalculateInfluence(NationsEnum nation)
    {
        if (nation == NationsEnum.ABANDONED)
            return;

        int freeInfluence = Nations.INFLUENCE;
        List<CardUI> characters = board.GetCharacterManager().GetCharactersOfPlayer(nation);
        foreach (CardUI character in characters)
        {
            CharacterCardUIBoard boardUI = character as CharacterCardUIBoard;
            if (boardUI == null)
                continue;
            freeInfluence -= boardUI.GetTotalMind();
        }
        influences[nation] = freeInfluence;
        
        if (nation == game.GetHumanNation())
            RefreshInfluence(turn.GetCurrentPlayer());
    }
    public void SubtractInfluence(NationsEnum nation, int influence)
    {
        if (nation == NationsEnum.ABANDONED)
            return;

        if(!influences.ContainsKey(nation))
            influences[nation] = Nations.INFLUENCE;
        influences[nation] -= influence;
    }

    public void RefreshInfluence(NationsEnum nation)
    {
        if (nation != game.GetHumanNation())
            return;
        if (!influences.ContainsKey(nation))
            influences[nation] = Nations.INFLUENCE;
        influence.text = influences[nation].ToString();
    }

    public int GetFreeInfluence(NationsEnum nation, bool dirty)
    {
        if (!influences.ContainsKey(nation) || dirty)
            RecalculateInfluence(nation);
        return influences[nation];
    }
    
    public int GetFoodStores()
    {
        if (!stores.ContainsKey(turn.GetCurrentPlayer()))
            return 0;
        return stores[turn.GetCurrentPlayer()].resources[ResourceType.FOOD];
    }
    public int GetGoldStores()
    {
        if (!stores.ContainsKey(turn.GetCurrentPlayer()))
            return 0;
        return stores[turn.GetCurrentPlayer()].resources[ResourceType.GOLD];
    }
    public int GetClothesStores()
    {
        if (!stores.ContainsKey(turn.GetCurrentPlayer()))
            return 0;
        return stores[turn.GetCurrentPlayer()].resources[ResourceType.CLOTHES];
    }

    public int GetHorsesStores()
    {
        if (!stores.ContainsKey(turn.GetCurrentPlayer()))
            return 0;
        return stores[turn.GetCurrentPlayer()].resources[ResourceType.MOUNTS];
    }
    public int GetWoodStores()
    {
        if (!stores.ContainsKey(turn.GetCurrentPlayer()))
            return 0;
        return stores[turn.GetCurrentPlayer()].resources[ResourceType.WOOD];
    }
    public int GetMetalStores()
    {
        if (!stores.ContainsKey(turn.GetCurrentPlayer()))
            return 0;
        return stores[turn.GetCurrentPlayer()].resources[ResourceType.METAL];
    }
    public int GetGemsStores()
    {
        if (!stores.ContainsKey(turn.GetCurrentPlayer()))
            return 0;
        return stores[turn.GetCurrentPlayer()].resources[ResourceType.GEMS];
    }
    public int GetLeatherStores()
    {
        if (!stores.ContainsKey(turn.GetCurrentPlayer()))
            return 0;
        return stores[turn.GetCurrentPlayer()].resources[ResourceType.LEATHER];
    }

}
