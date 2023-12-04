using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class CityUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("City  UI")]
    [Header("References")]
    public GameObject displacement;
    public Image detailsGold;
    public Image detailsClothes;
    public Image detailsFood;
    public Image detailsWood;
    public Image detailsMetal;
    public Image detailsHorses;
    public Image detailsGems;
    public Image detailsLeather;
    public TextMeshProUGUI cityName;
    public Button button;
    public GameObject goHealth;
    public Image imgHealth;
    public Image alignment;
    public Image haven;
    public GameObject nextGameObject;
    public CanvasGroup tapped;

    [Header("Initialization")]
    [SerializeField]
    private Vector2Int hex;
    [SerializeField]
    private NationsEnum owner;

    [Header("Generated based on owner")]
    [SerializeField]
    private List<string> automaticAttacks;

    private string cityId;
    private bool isClicked = false;
    
    private Board board;
    private CityDetails details;
    private SelectedItems selectedItems;
    private Tilemap t;
    private Game game;
    private Turn turn;
    private SpritesRepo spritesRepo;
    private CameraController cameraManager;
    private TerrainManager terrainManager;
    private CardDetailsRepo cardDetailsRepo;
    private ResourcesManager resourcesManager;
    private Mouse mouse;
    private PlaceDeck placeDeckManager;
    private FOWManager fowManager;

    private bool initialized = false;
    private int health;

    private void Awake()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
        selectedItems = GameObject.Find("SelectedItems").GetComponent<SelectedItems>();
        t = GameObject.Find("CardTypeTilemap").GetComponent<Tilemap>();
        game = GameObject.Find("Game").GetComponent<Game>();
        turn = GameObject.Find("Turn").GetComponent<Turn>();
        spritesRepo = GameObject.Find("SpritesRepo").GetComponent<SpritesRepo>();
        cameraManager = GameObject.Find("CameraController").GetComponent<CameraController>();
        mouse = GameObject.Find("Mouse").GetComponent<Mouse>();

        cardDetailsRepo = GameObject.Find("CardDetailsRepo").GetComponent<CardDetailsRepo>();
        board = GameObject.Find("Board").GetComponent<Board>();
        resourcesManager = GameObject.Find("ResourcesManager").GetComponent<ResourcesManager>();
        terrainManager = GameObject.Find("TerrainManager").GetComponent<TerrainManager>();
        placeDeckManager = GameObject.Find("PlaceDeckManager").GetComponent<PlaceDeck>();
        fowManager = GameObject.Find("FOWManager").GetComponent<FOWManager>();
        initialized = false;
        health = 100;
    }
    public bool Initialize()
    {
        if (!board.IsInitialized() ||
            !cardDetailsRepo.IsInitialized() ||
            !game.IsInitialized() ||
            !fowManager.IsInitialized() ||
            string.IsNullOrEmpty(cityId)
            )
            return false;
        GameObject prefab = cardDetailsRepo.GetCityGameObject(cityId);
        if(prefab == null)
        {
            Debug.LogError(string.Format("Unable to find {0} in Initial Decks", cityId));
            return false;
        }
        GameObject cityObject = Instantiate(prefab);
        cityObject.name = cityId;
        cityObject.transform.SetParent(transform);

        details = cityObject.GetComponent<CityDetails>();

        TileAndMovementCost tile = terrainManager.GetTileAndMovementCost(hex);
        details.Initialize(tile.terrain.terrainType, tile.cardInfo.cardType);
        
        GenerateAutomaticAttacks();

        board.AddCity(hex, this);
        // Debug.Log(string.Format("{0} has registered itself at {1}", cityId, hex));
        initialized = true;

        resourcesManager.Add(owner, details.GetCityProduction());

        nextGameObject.SetActive(turn.GetCurrentPlayer() == owner);

        Vector3 cellWorldCenter = t.GetCellCenterWorld(new Vector3Int(hex.x, hex.y, 0));

        gameObject.transform.position = cellWorldCenter;

        detailsGold.enabled = details.GetCityProduction().resources[ResourceType.GOLD] > 0 && IsRevealedOrHiddenVisible(turn.GetCurrentPlayer());
        detailsClothes.enabled = details.GetCityProduction().resources[ResourceType.CLOTHES] > 0 && IsRevealedOrHiddenVisible(turn.GetCurrentPlayer());
        detailsFood.enabled = details.GetCityProduction().resources[ResourceType.FOOD] > 0 && IsRevealedOrHiddenVisible(turn.GetCurrentPlayer());
        detailsWood.enabled = details.GetCityProduction().resources[ResourceType.WOOD] > 0 && IsRevealedOrHiddenVisible(turn.GetCurrentPlayer());
        detailsMetal.enabled = details.GetCityProduction().resources[ResourceType.METAL] > 0 && IsRevealedOrHiddenVisible(turn.GetCurrentPlayer());
        detailsHorses.enabled = details.GetCityProduction().resources[ResourceType.MOUNTS] > 0 && IsRevealedOrHiddenVisible(turn.GetCurrentPlayer());
        detailsGems.enabled = details.GetCityProduction().resources[ResourceType.GEMS] > 0 && IsRevealedOrHiddenVisible(turn.GetCurrentPlayer());
        detailsLeather.enabled = details.GetCityProduction().resources[ResourceType.LEATHER] > 0 && IsRevealedOrHiddenVisible(turn.GetCurrentPlayer());

        alignment.sprite = spritesRepo.GetSprite(Nations.alignments[owner].ToString());
        alignment.enabled = IsRevealedOrHiddenVisible(turn.GetCurrentPlayer());
        haven.enabled = details.isHaven && IsRevealedOrHiddenVisible(turn.GetCurrentPlayer());
        cityName.text = IsRevealedOrHiddenVisible(turn.GetCurrentPlayer()) ? GameObject.Find("Localization").GetComponent<Localization>().Localize(details.GetCityID()) : StringConstants.hidden;

        goHealth.SetActive(IsRevealedOrHiddenVisible(turn.GetCurrentPlayer()));

        //Debug.Log(string.Format("{0} finished loading at {1}", cityObject.name, Time.realtimeSinceStartup));

        initialized = true;
        return true;
    }

    public void GenerateAutomaticAttacks()
    {
        automaticAttacks = new();
        List<HazardCreatureCardDetails> res = cardDetailsRepo.GetHazardCardsOfNation(owner);
        if (res.Count < 1)
            return;
        res.Shuffle();
        res = res.GetRange(0, Math.Min(res.Count, CitySizes.automaticAttacks[details.size]));
        automaticAttacks = res.Select(x => x.cardId).ToList();
    }

    public bool IsRevealedOrHiddenVisible(NationsEnum watcher)
    {
        if (!details.isHidden)
            return true;

        if (Nations.alignments[watcher] == Nations.alignments[owner])
            return true;

        return false;
    }

    void Update()
    {
        if (!initialized)
        {
            cityId ??= gameObject.name;
            Initialize();
            return;
        }

        if (Input.GetKeyUp(KeyCode.Escape))
            isClicked = false;

        if (isClicked && selectedItems.GetSelectedCityDetails() != null && selectedItems.GetSelectedCityDetails().GetCityID() != details.GetCityID())
            isClicked = false;

        displacement.transform.localPosition = new Vector3(0, DisplacementPixels.down, 0);
    }

    public bool IsInitialized()
    {
        return initialized;
    }

    public CityDetails GetDetails()
    {
        return details;
    }

    public string GetCityId()
    {
        return cityId;
    }
    public void Toggle()
    {
        isClicked = !isClicked;
        if (isClicked)
        {
            board.SelectHex(hex);
            selectedItems.SelectCityUI(this);
        }            
        else if (selectedItems.GetSelectedCityDetails()!= null && selectedItems.GetSelectedCityDetails().GetCityID() != details.GetCityID())
        {
            board.SelectHex(Board.NULL);
            selectedItems.UnselectCityDetails();
        }            
    }

    public void Tap()
    {
        tapped.alpha = 0.5f;
    }

    public void Next()
    {
        CityUI next = board.GetNextCityUI(this);
        
        if (next != null)
            cameraManager.LookToCity(next);
    }

    public NationsEnum GetOwner()
    {
        return owner;
    }
    
    public Vector2Int GetHex()
    {
        return hex;
    }

    public void SetHealth(int health)
    {
        imgHealth.fillAmount = health / 100;
    }
    public void SetHoverCity()
    {
        if (button.interactable)
            mouse.ChangeCursor("clickable");
        else
            mouse.ChangeCursor("unclickable");
        placeDeckManager.SetCardToShow(new HoveredCard(owner, details.GetCityID(), CardClass.Place));
    }
    public void RemoveHoverCity()
    {
        mouse.RemoveCursor();
        placeDeckManager.RemoveCardToShow(new HoveredCard(owner, details.GetCityID(), CardClass.Place));
    }
    public float GetDistanceTo(Vector2Int hex)
    {
        return Vector2Int.Distance(hex, this.hex);
    }

    public void Damage(int damage)
    {
        health -= damage;
    }

    public List<string> GetAutomaticAttacks(NationsEnum nation)
    {
        AlignmentsEnum visitor = Nations.alignments[nation];
        AlignmentsEnum city = Nations.alignments[owner];
        if (visitor != city)
            return automaticAttacks;
        return new List<string>();
    }
    public List<ObjectType> GetPlayableObjects(NationsEnum nation)
    {
        List<ObjectType> objectSlots = new();

        AlignmentsEnum visitor = Nations.alignments[nation];
        AlignmentsEnum city = Nations.alignments[owner];
        if (visitor != city)
            objectSlots = details.GetPlayableObjects();

        return objectSlots;
    }
    public List<RingType> GetPlayableRings(NationsEnum nation)
    {
        List<RingType> objectSlots = new();

        AlignmentsEnum visitor = Nations.alignments[nation];
        AlignmentsEnum city = Nations.alignments[owner];
        if (visitor != city)
            objectSlots = details.GetPlayableRings();

        return objectSlots;
    }
    public List<string> GetPlayableRingsString(NationsEnum nation)
    {
        List<RingType> objectSlots = new();

        AlignmentsEnum visitor = Nations.alignments[nation];
        AlignmentsEnum city = Nations.alignments[owner];
        if (visitor != city)
            objectSlots = details.GetPlayableRings();

        return objectSlots.Select(x => x.ToString()).ToList();
    }

    public List<string> GetPlayableObjectsStrings(NationsEnum nation)
    {
        List<ObjectType> objectSlots = new();

        AlignmentsEnum visitor = Nations.alignments[nation];
        AlignmentsEnum city = Nations.alignments[owner];
        if (visitor != city)
            objectSlots = details.GetPlayableObjects();

        return objectSlots.Select(x => x.ToString()).ToList();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouse.ChangeCursor("clickable");
        placeDeckManager.SetCardToShow(new HoveredCard(owner, cityId, CardClass.Place));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouse.RemoveCursor();
        placeDeckManager.RemoveCardToShow(new HoveredCard(owner, cityId, CardClass.Place));
    }
}
