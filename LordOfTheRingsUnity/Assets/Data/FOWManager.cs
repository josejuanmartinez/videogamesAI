using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FOWManager : MonoBehaviour
{
    public short cityVisionLevel = 5;
    public short cardVisionLevel = 3;
    public Tile fowTile;
    
    Tilemap fow;
    Board board;
    Game game;
    Turn turn;

    bool isInitialized;

    void Awake()
    {
        fow = GameObject.Find("FOWTilemap").GetComponent<Tilemap>();
        board = GameObject.Find("Board").GetComponent<Board>();
        game = GameObject.Find("Game").GetComponent<Game>();
        turn = GameObject.Find("Turn").GetComponent<Turn>();
        isInitialized = false;
    }


    public void Initialize()
    {
        isInitialized = board.IsInitialized() && game.IsInitialized() && turn.IsInitialized();
        //if (isInitialized)
        //    Debug.Log("FOWManager initialized at " + Time.realtimeSinceStartup);
    }
    void Update()
    {
        if (!isInitialized)
            Initialize();
    }

    // Update is called once per frame
    public void UpdateCitiesFOW()
    {
        if(!isInitialized)
        {
            Initialize();
            return;
        }            

        List <CityUI> cities = board.GetCityManager().GetCitiesOfPlayer(turn.GetCurrentPlayer()); 
        foreach (CityUI city in cities)
            UpdateCityFOW(city);
    }

    public void UpdateCityFOW(CityUI city)
    {
        if (!isInitialized)
        {
            Initialize();
            return;
        }

        Vector3Int cityHex = new (city.GetHex().x, city.GetHex().y, 0);
        HashSet<Vector3Int> hexesToClean = new() { cityHex };
        for (int i = 0; i < cityVisionLevel; i++)
        {
            HashSet<Vector3Int> moreHexesToClean = new();
            foreach (Vector3Int hex in hexesToClean)
            {
                List<Vector3Int> v3Surroundings = HexTranslator.GetSurroundings(hex);
                moreHexesToClean.UnionWith(v3Surroundings);
            }
            hexesToClean = moreHexesToClean;
        }
        foreach (Vector3Int surrounding in hexesToClean)
        {
            fow.SetTile(surrounding, null);
            fow.RefreshTile(surrounding);
            game.GetHumanPlayer().SetCitySeesTile(new Vector2Int(surrounding.x, surrounding.y));
        }
    }

    public void UpdateCardsFOW()
    {
        if (!isInitialized)
        {
            Initialize();
            return;
        }

        List<CardUI> cards = board.GetCardManager().GetCardsInPlayOfOwner(turn.GetCurrentPlayer());
        foreach (CardUI card in cards)
        {
            UpdateCardFOW(card);
        }
    }

    public void UpdateCardFOW(CardUI card)
    {
        if (!isInitialized)
        {
            Initialize();
            return;
        }

        if (card.GetCardClass() != CardClass.Character && card.GetCardClass() != CardClass.HazardCreature)
            return;

        Vector2Int hex = card.GetCardClass() ==
            CardClass.Character ? ((CharacterCardUIBoard)card).GetHex() : ((HazardCreatureCardUIBoard)card).GetHex();

        Vector3Int cardHex = new(hex.x, hex.y, 0);
        HashSet<Vector3Int> hexesToClean = new() { cardHex };
        for (int i = 0; i < cardVisionLevel; i++)
        {
            HashSet<Vector3Int> moreHexesToClean = new();
            foreach (Vector3Int anotherHex in hexesToClean)
            {
                List<Vector3Int> v3Surroundings = HexTranslator.GetSurroundings(anotherHex);
                moreHexesToClean.UnionWith(v3Surroundings);
            }
            hexesToClean.UnionWith(moreHexesToClean);
        }
        foreach (Vector3Int surrounding in hexesToClean)
        {
            fow.SetTile(surrounding, null);
            fow.RefreshTile(surrounding);
            game.GetHumanPlayer().SetCardSeesTile(new Vector2Int(surrounding.x, surrounding.y));
        }
    }

    public void UpdateCardFOW(Vector3Int newHex, Vector3Int oldHex)
    {
        if (!isInitialized)
        {
            Initialize();
            return;
        }

        Vector3Int cardHex = new (oldHex.x, oldHex.y, 0);
        HashSet<Vector3Int> hexesToClean = new() { cardHex };
        for (int i = 0; i < cardVisionLevel; i++)
        {
            HashSet<Vector3Int> moreHexesToClean = new();
            foreach (Vector3Int hex in hexesToClean)
            {
                List<Vector3Int> v3Surroundings = HexTranslator.GetSurroundings(hex);
                moreHexesToClean.UnionWith(v3Surroundings);
            }
            hexesToClean.UnionWith(moreHexesToClean);
        }
        foreach (Vector3Int surrounding in hexesToClean)
        {
            if(!game.GetHumanPlayer().CitySeesTile(new Vector2Int(surrounding.x, surrounding.y))) {
                fow.SetTile(surrounding, fowTile);
                fow.RefreshTile(surrounding);
            }
            game.GetHumanPlayer().UnsetCardSeesTile(new Vector2Int(surrounding.x, surrounding.y));
        }

        cardHex = new Vector3Int(newHex.x, newHex.y, 0);
        hexesToClean = new HashSet<Vector3Int>() { cardHex };
        for (int i = 0; i < cardVisionLevel; i++)
        {
            HashSet<Vector3Int> moreHexesToClean = new ();
            foreach (Vector3Int hex in hexesToClean)
            {
                List<Vector3Int> v3Surroundings = HexTranslator.GetSurroundings(hex);
                moreHexesToClean.UnionWith(v3Surroundings);
            }
            hexesToClean.UnionWith(moreHexesToClean);
        }
        foreach (Vector3Int surrounding in hexesToClean)
        {
            fow.SetTile(surrounding, null);
            fow.RefreshTile(surrounding);
            game.GetHumanPlayer().SetCardSeesTile(new Vector2Int(surrounding.x, surrounding.y));
        }
    }

    public bool IsInitialized()
    {
        return isInitialized;
    }
}
