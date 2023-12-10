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
            Vector2Int v2hex = new(surrounding.x, surrounding.y);
            fow.SetTile(surrounding, null);
            fow.RefreshTile(surrounding);
            game.GetHumanPlayer().SetCitySeesTile(new Vector2Int(surrounding.x, surrounding.y));
            if (board.GetTile(v2hex).HasCity())
                board.GetTile(v2hex).GetCity().RefreshCityUICanvas();
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
            CardClass.Character ? (card as CharacterCardUIBoard).GetHex() : (card as HazardCreatureCardUIBoard).GetHex();
        
        bool isBlind = card.GetCardClass() ==
            CardClass.Character ? (card as CharacterCardUIBoard).IsBlind() : (card as HazardCreatureCardUIBoard).IsBlind();

        Vector3Int cardHex = new(hex.x, hex.y, 0);
        HashSet<Vector3Int> hexesToClean = new() { cardHex };
        if(!isBlind)
        {
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
        }        
        foreach (Vector3Int surrounding in hexesToClean)
        {
            Vector2Int v2hex = new(surrounding.x, surrounding.y);
            fow.SetTile(surrounding, null);
            fow.RefreshTile(surrounding);
            game.GetHumanPlayer().SetCardSeesTile(v2hex);
            if (board.GetTile(v2hex).HasCity())
                board.GetTile(v2hex).GetCity().RefreshCityUICanvas();
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
            Vector2Int v2hex = new(surrounding.x, surrounding.y);
            fow.SetTile(surrounding, null);
            fow.RefreshTile(surrounding);
            game.GetHumanPlayer().SetCardSeesTile(new Vector2Int(surrounding.x, surrounding.y));
            if (board.GetTile(v2hex).HasCity())
                board.GetTile(v2hex).GetCity().RefreshCityUICanvas();
        }
    }

    public bool IsInitialized()
    {
        return isInitialized;
    }
}
