using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board: MonoBehaviour
{
    public GameObject characterUICardBoard;
    public GameObject hazardUICardBoard;
    public GameObject cityObject;
    public GameObject cardsCanvas;
    public GameObject citiesCanvas;

    public static Vector2Int NULL = Vector2Int.one * int.MinValue;

    readonly private Dictionary<Vector2Int, BoardTile> tiles = new();
    private Vector2Int selectedHex = NULL;

    private CardManager cardManager;
    private CityManager cityManager;
    private CharacterManager characterManager;
    private HazardCreaturesManager hazardCreaturesManager;
    private ResourcesManager resourcesManager;
    private Turn turn;
    private Tilemap t;
    private FOWManager fowManager;
    private MovementManager movementManager;

    private Transform cardsCanvasTransform;
    bool initialized;
    bool allLoaded;

    void Awake()
    {
        cardManager = new CardManager(this);
        cityManager = new CityManager(this);
        characterManager = new CharacterManager(this);
        hazardCreaturesManager = new HazardCreaturesManager(this);

        resourcesManager = GameObject.Find("ResourcesManager").GetComponent<ResourcesManager>();
        turn = GameObject.Find("Turn").GetComponent<Turn>();
        cardsCanvasTransform = GameObject.Find("CardsCanvas").transform;
        t = GameObject.Find("CardTypeTilemap").GetComponent<Tilemap>();
        fowManager = GameObject.Find("FOWManager").GetComponent<FOWManager>();
        movementManager = GameObject.Find("MovementManager").GetComponent<MovementManager>();
        initialized = false;
        allLoaded = false;
    }
    void Initialize()
    {
        initialized = GameObject.Find("Localization").GetComponent<Localization>().IsInitialized();
        if (initialized)
            Debug.Log("Board Initialized");
    }

    public bool CalculateAllLoaded()
    {
        if (initialized == false)
            return false;

        foreach (Transform t in cardsCanvas.transform)
        {
            CharacterCardUIBoard character = t.gameObject.GetComponent<CharacterCardUIBoard>();
            if (!character.IsInitialized())
                return false;
        }
        foreach (Transform t in citiesCanvas.transform)
        {
            CityUI city = t.gameObject.GetComponent<CityUI>();
            if (!city.IsInitialized())
                return false;
        }
        allLoaded = true;
        return allLoaded;
    }

    void Update()
    {
        if(!initialized)
        {
            Initialize();
            return;
        }
    }

    public bool IsInitialized()
    {
        return initialized;
    }

    public bool IsAllLoaded()
    {
        if (!initialized) 
            return false;
        if (allLoaded)
            return true;
        else
            return CalculateAllLoaded();
    }
    public void AddCity(Vector2Int hex, CityUI city)
    {
        if (!tiles.ContainsKey(hex))
            tiles[hex] = new BoardTile(hex, city);
        else
        {
            BoardTile bt = tiles[hex];
            bt.AddCity(city);
        }
        resourcesManager.Add(city.GetOwner(), city.GetDetails().GetCityProduction());

        if (city.GetOwner() == turn.GetCurrentPlayer())
            fowManager.UpdateCityFOW(city);
    }
    public BoardTile AddCard(Vector2Int hex, CardUI card)
    {
        if (!tiles.ContainsKey(hex))
            tiles[hex] = new BoardTile(hex, card);
        else
        {
            BoardTile bt = tiles[hex];
            bt.AddCard(card);
        }
        
        if (card.GetOwner() == turn.GetCurrentPlayer())
            fowManager.UpdateCardFOW(card);

        return tiles[hex];
    }

    public Dictionary<Vector2Int, BoardTile> GetTiles()
    {
        return tiles;
    }
    
    public BoardTile GetTile(Vector2Int hex)
    {
        if(!tiles.ContainsKey(hex))
            tiles[hex] = new BoardTile(hex);
        return tiles[hex];            
    }

    public void SelectHex(Vector2Int hex)
    {
        selectedHex = hex;
    }

    public Vector2Int GetSelectedHex()
    {
        return selectedHex;
    }

    public bool IsHexSelected()
    {
        return selectedHex != NULL;
    }

    public CityManager GetCityManager()
    {
        return cityManager;
    }

    public CharacterManager GetCharacterManager()
    {
        return characterManager;
    }

    public HazardCreaturesManager GetHazardCreaturesManager()
    {
        return hazardCreaturesManager;
    }
    public CardManager GetCardManager()
    {
        return cardManager;
    }
    public CardUI CreateCardUI(CardDetails cardDetails, SpawnCardLocation spawnCardLocation)
    {
        if (cardDetails.cardClass == CardClass.Character)
            return CreateCardUICharacter(cardDetails, spawnCardLocation);
        else if (cardDetails.cardClass == CardClass.HazardCreature)
            return CreateCardUIHazardCreature(cardDetails, spawnCardLocation);
        else
            return null;
    }

    public CardUI CreateCardUICharacter(CardDetails cardDetails, SpawnCardLocation spawnCardLocation)
    {
        string cardId = cardDetails.cardId;

        GameObject instantiatedObject = Instantiate(characterUICardBoard, cardsCanvasTransform.transform);

        instantiatedObject.name = cardId;
        instantiatedObject.layer = LayerMask.NameToLayer("UI");
        
        CharacterCardUIBoard card = instantiatedObject.GetComponent<CharacterCardUIBoard>();

        bool success = false;

        Vector2Int hex = Vector2Int.one * int.MinValue;
        CityUI city;
        switch (spawnCardLocation)
        {
            case SpawnCardLocation.AtHaven:
                city = cityManager.GetHavenOfPlayer(turn.GetCurrentPlayer());
                if (city != null)
                {
                    hex = city.GetHex();
                    card.Initialize(hex, cardId, turn.GetCurrentPlayer(), 0);
                    success = true;
                }
                else
                {
                    Debug.Log("Trying to instantiate at haven but player does not have a haven");
                    success = false;
                }
                break;
            case SpawnCardLocation.AtHomeTown:
                CharacterCardDetails character = (CharacterCardDetails)cardDetails;
                success = false;
                // THIS WILL ALREADY TAKE INTO ACCOUNT "ANY" hometowns
                city = cityManager.GetCityOfPlayer(turn.GetCurrentPlayer(), character.homeTown);
                if (city != null)
                {
                    hex = city.GetHex();
                    card.Initialize(hex, cardId, turn.GetCurrentPlayer(), 0);
                    success = true;
                }
                break;
            case SpawnCardLocation.AtLastCell:
                if (movementManager.GetLastHex() != MovementManager.NULL2)
                {
                    hex = movementManager.GetLastHex();
                    card.Initialize(hex, cardId, turn.GetCurrentPlayer(), 0);
                    success = true;
                }
                else
                {
                    Debug.LogError("Unable to instantiate at last hex.");
                    if (instantiatedObject)
                        DestroyImmediate(instantiatedObject);
                    success = false;
                }
                break;
        }
        if (!success || hex == Vector2.one * int.MinValue)
        {
            DestroyImmediate(instantiatedObject);
            if (tiles[hex].GetCardsUI().Contains(card))
                tiles[hex].RemoveCard(card);
            return null;
        }

        // HEX
        card.SetHex(hex);

        Vector3 cellWorldCenter = t.GetCellCenterWorld(new Vector3Int(hex.x, hex.y, 0));

        gameObject.transform.position = cellWorldCenter;

        return card;
    }

    public CardUI CreateCardUIHazardCreature(CardDetails cardDetails, SpawnCardLocation spawnCardLocation)
    {
        string cardId = cardDetails.cardId;

        GameObject instantiatedObject = Instantiate(hazardUICardBoard, cardsCanvasTransform.transform);

        instantiatedObject.name = cardId;
        instantiatedObject.layer = LayerMask.NameToLayer("UI");
        //instantiatedObject.transform.localScale = Vector3.one;
        HazardCreatureCardUIBoard card = instantiatedObject.GetComponent<HazardCreatureCardUIBoard>();

        bool success = false;

        Vector2Int hex = Vector2Int.one * int.MinValue;
        CityUI city;
        switch (spawnCardLocation)
        {
            case SpawnCardLocation.AtHaven:
                city = cityManager.GetHavenOfPlayer(turn.GetCurrentPlayer());
                if (city != null)
                {
                    hex = city.GetHex();
                    //card.Initialize(hex, cardId, turn.GetCurrentPlayer(), MovementConstants.characterMovement);
                    card.Initialize(hex, cardId, turn.GetCurrentPlayer(), 0);
                    success = true;
                }
                else
                {
                    Debug.Log("Trying to instantiate at haven but player does not have a haven");
                    success = false;
                }
                break;
            case SpawnCardLocation.AtLastCell:
                if (movementManager.GetLastHex() != MovementManager.NULL2)
                {
                    hex = movementManager.GetLastHex();
                    //card.Initialize(hex, cardId, turn.GetCurrentPlayer(), MovementConstants.characterMovement);
                    card.Initialize(hex, cardId, turn.GetCurrentPlayer(), 0);
                    success = true;
                }
                else
                {
                    Debug.LogError("Unable to instantiate at last hex.");
                    if (instantiatedObject)
                        DestroyImmediate(instantiatedObject);
                    success = false;
                }

                break;
        }
        if (!success || hex == Vector2.one * int.MinValue)
        {
            DestroyImmediate(instantiatedObject);
            if (tiles[hex].GetCardsUI().Contains(card))
                tiles[hex].RemoveCard(card);
            return null;
        }

        // HEX
        card.SetHex(hex);

        Vector3 cellWorldCenter = t.GetCellCenterWorld(new Vector3Int(hex.x, hex.y, 0));

        gameObject.transform.position = cellWorldCenter;

        return card;
    }

    public CardUI GetNextCardUI(CardUI card)
    {
        if(card == null)
        {
            List<CardUI> cards = cardManager.GetCardsInPlayOfOwner(turn.GetCurrentPlayer());
            if (cards.Count > 0)
                return cards[0];
            return null;
        }
        else
        {
            List<CardUI> cards = cardManager.GetCardsInPlayOfOwner(card.GetOwner());
            int pos = cards.FindIndex(x => x.GetCardID() == card.GetCardID());
            if (pos == -1)
                return null;
            if (cards.Count < 2)
                return card;
            return cards[(pos + 1) % cards.Count];
        }        
    }
    public CityUI GetNextCityUI(CityUI city)
    {
        List<CityUI> cities = cityManager.GetCitiesOfPlayer(city.GetOwner());
        int pos = cities.FindIndex(x => x.GetCityId() == city.GetCityId());
        if (pos == -1)
            return null;
        if (cities.Count < 2)
            return city;
        return cities[(pos + 1) % cities.Count];
    }
}
