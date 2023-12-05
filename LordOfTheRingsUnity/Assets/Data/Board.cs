using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
    private Game game;

    private Transform cardsCanvasTransform;
    private bool initialized;
    private bool allLoaded;

    private bool loadingCitiesOnDemand;
    private bool loadingCharactersOnDemand;
    private List<CityUI> ondemandLoadCityUI;
    private List<CharacterCardUIBoard> ondemandLoadCharacterUI;
    private List<HazardCreatureCardUIBoard> ondemandLoadHazardCreatureUI;

    private CharacterCardUIBoard humanAvatar;

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
        game = GameObject.Find("Game").GetComponent<Game>();
        
        ondemandLoadCityUI = new();
        ondemandLoadCharacterUI = new();
        ondemandLoadHazardCreatureUI = new();

        loadingCitiesOnDemand = false;
        loadingCharactersOnDemand = false;

        initialized = false;
        allLoaded = false;
    }
    void Initialize()
    {
        initialized = GameObject.Find("Localization").GetComponent<Localization>().IsInitialized();
        //if (initialized)
        //    Debug.Log("Board Initialized at " + Time.realtimeSinceStartup);
    }

    double EuclideanDistance(Vector2Int p1, Vector2Int p2)
    {
        return Math.Sqrt(Math.Pow(p1.x - p2.x, 2) + Math.Pow(p1.y - p2.y, 2));
    }

    public void AddOnDemandLoadCity(CityUI cityUI)
    {
        ondemandLoadCityUI.Add(cityUI);
    }

    public void AddOnDemandLoadCharacter(CharacterCardUIBoard characterUI)
    {
        ondemandLoadCharacterUI.Add(characterUI);
    }
    public void AddOnDemandLoadCreature(HazardCreatureCardUIBoard hazardUI)
    {
        //if (humanAvatar == null)
        //    humanAvatar = characterManager.GetAvatar(game.GetHumanNation()) as CharacterCardUIBoard;

        ondemandLoadHazardCreatureUI.Add(hazardUI);
        
        //if (humanAvatar != null)
        //    ondemandLoadHazardCreatureUI = ondemandLoadHazardCreatureUI.OrderBy(p => EuclideanDistance(humanAvatar.GetHex(), p.GetHex())).ToList();


    }

    public bool CalculateAllLoaded()
    {
        if (initialized == false)
            return false;
        
        foreach (Transform t in cardsCanvas.transform)
        {
            CharacterCardUIBoard character = t.gameObject.GetComponent<CharacterCardUIBoard>();
            if (character.GetOwner() == game.GetHumanNation() && !character.IsInitialized())
                return false;
        }
        //float charTime = Time.realtimeSinceStartup;
        
        foreach (Transform t in citiesCanvas.transform)
        {
            CityUI city = t.gameObject.GetComponent<CityUI>();
            if(city.GetOwner() == game.GetHumanNation() && !city.IsInitialized())
                return false;
        }
        //Debug.Log("Board finishes loading char UI cards at " + charTime);
        //Debug.Log("Board finishes loading cities UI cards at " + Time.realtimeSinceStartup);
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
        if(allLoaded)
        {
            if (ondemandLoadCityUI.Count > 0 && !loadingCitiesOnDemand)
                StartCoroutine(LoadOnDemandCityUI());
            if (ondemandLoadCharacterUI.Count > 0 && !loadingCharactersOnDemand)
                StartCoroutine(LoadOnDemandCharacterUI());
        }
        
    }

    IEnumerator LoadOnDemandCityUI() 
    {
        //Debug.Log("Loading cities UI on demand...");
        loadingCitiesOnDemand = true;
        if(humanAvatar == null)
            humanAvatar = characterManager.GetAvatar(game.GetHumanNation()) as CharacterCardUIBoard;
        if (humanAvatar != null)
            ondemandLoadCityUI = ondemandLoadCityUI.OrderBy(p => EuclideanDistance(humanAvatar.GetHex(), p.GetHex())).ToList();

        while (ondemandLoadCityUI.Count > 0)
        {
            CityUI city = ondemandLoadCityUI[0];
            if (!city.IsInitialized())
            {
                city.Initialize();
                yield return new WaitUntil(city.IsInitialized);
                //Debug.Log(string.Format("{0} is initialized", city.GetCityId()));
                yield return new WaitForSecondsRealtime(3f);
            }
            ondemandLoadCityUI.Remove(city);
        }
        loadingCitiesOnDemand = false;
    }

    IEnumerator LoadOnDemandCharacterUI()
    {
        //Debug.Log("Loading characters UI on demand...");
        loadingCharactersOnDemand = true;
        if (humanAvatar == null)
            humanAvatar = characterManager.GetAvatar(game.GetHumanNation()) as CharacterCardUIBoard;
        if (humanAvatar != null)
            ondemandLoadCharacterUI = ondemandLoadCharacterUI.OrderBy(p => EuclideanDistance(humanAvatar.GetHex(), p.GetHex())).ToList();

        while (ondemandLoadCharacterUI.Count > 0)
        {
            CharacterCardUIBoard character = ondemandLoadCharacterUI[0];
            if (!character.IsInitialized())
            {
                character.Initialize();
                yield return new WaitUntil(character.IsInitialized);
                //Debug.Log(string.Format("{0} is initialized", character.GetCardId()));
                yield return new WaitForSecondsRealtime(3f);
            }
            ondemandLoadCharacterUI.Remove(character);
        }
        loadingCharactersOnDemand = false;
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
                city = cityManager.GetCityOfPlayer(turn.GetCurrentPlayer(), character.GetHomeTown());
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
            int pos = cards.FindIndex(x => x.GetCardId() == card.GetCardId());
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
