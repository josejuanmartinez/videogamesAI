using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class CityUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("City Details")]
    [SerializeField, PreviewSprite]
    private Sprite sprite;
    [SerializeField] 
    private string descForAIGeneration;
    [SerializeField] 
    private CitySizesEnum size;
    [SerializeField] 
    private bool isHidden = false;
    [SerializeField]
    private bool hasPort = false;
    [SerializeField]
    private bool isUnderground = false;
    [SerializeField]
    private NationRegionsEnum regionId;
    [SerializeField]
    private bool isHaven;
    
    public GameObject message;

    [Header("Automatically Generated")]
    [SerializeField] 
    private List<ObjectType> playableObjects;
    [SerializeField]
    private List<RingType> playableRings;
    [SerializeField]
    private TerrainsEnum terrain;
    [SerializeField]
    private CardTypesEnum cardType;
    [SerializeField]
    private string cityId;
    [SerializeField]
    private int food;
    [SerializeField]
    private int gold;
    [SerializeField]
    private int cloth;
    [SerializeField]
    private int wood;
    [SerializeField]
    private int metal;
    [SerializeField]
    private int horses;
    [SerializeField]
    private int gems;
    [SerializeField]
    private int leather;
     
    [Header("UI")]
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
    public CanvasGroup canvasGroup;

    [Header("Initialization")]
    [SerializeField]
    private bool refresh;
    [SerializeField]
    private Vector2Int hex;
    [SerializeField]
    private NationsEnum owner;
    [SerializeField]
    private int health;

    [Header("Generated based on owner")]
    [SerializeField]
    private List<string> automaticAttacks;

    private bool isClicked = false;    
    private Board board;
    private SelectedItems selectedItems;
    private Tilemap t;
    private Game game;
    private SpritesRepo spritesRepo;
    private CameraController cameraManager;
    private TerrainManager terrainManager;
    private CardDetailsRepo cardDetailsRepo;
    private Mouse mouse;
    private PlaceDeck placeDeckManager;
    private FOWManager fowManager;

    protected bool messagesBeingShowed;
    protected bool messageBeingShowed;
    private HUDMessageManager hudMessageManager;
    protected List<HUDMessage> hudMessages;
    protected ColorManager colorManager;

    private bool initialized = false;
    

    private void Awake()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
        selectedItems = GameObject.Find("SelectedItems").GetComponent<SelectedItems>();
        t = GameObject.Find("CardTypeTilemap").GetComponent<Tilemap>();
        game = GameObject.Find("Game").GetComponent<Game>();
        spritesRepo = GameObject.Find("SpritesRepo").GetComponent<SpritesRepo>();
        cameraManager = GameObject.Find("CameraController").GetComponent<CameraController>();
        mouse = GameObject.Find("Mouse").GetComponent<Mouse>();

        cardDetailsRepo = GameObject.Find("CardDetailsRepo").GetComponent<CardDetailsRepo>();
        board = GameObject.Find("Board").GetComponent<Board>();
        terrainManager = GameObject.Find("TerrainManager").GetComponent<TerrainManager>();
        placeDeckManager = GameObject.Find("PlaceDeckManager").GetComponent<PlaceDeck>();
        fowManager = GameObject.Find("FOWManager").GetComponent<FOWManager>();
        hudMessageManager = GameObject.Find("HUDMessageManager").GetComponent<HUDMessageManager>();
        colorManager = GameObject.Find("ColorManager").GetComponent<ColorManager>();

        hudMessages = new List<HUDMessage>();
        initialized = false;
        
        health = 50;

        Hide();
    }
    public bool Initialize(bool forceRefresh = false)
    {
        if (string.IsNullOrEmpty(cityId))
            cityId = gameObject.name;

        if (!board.IsInitialized() ||
            !cardDetailsRepo.IsInitialized() ||
            !game.IsInitialized() ||
            !fowManager.IsInitialized() ||
            string.IsNullOrEmpty(cityId)
            )
            return false;

        if(forceRefresh)
        {
            TileAndMovementCost tile = terrainManager.GetTileAndMovementCost(hex);
            terrain = tile.terrain.terrainType;
            cardType = tile.cardInfo.cardType;
            GenerateProduction();
            GeneratePlayableObjects();
            GeneratePlayableRings();
            GenerateAutomaticAttacks();
        }

        Vector3 cellWorldCenter = t.GetCellCenterWorld(new Vector3Int(hex.x, hex.y, 0));
        gameObject.transform.position = cellWorldCenter;

        board.AddCity(hex, this, false);

        RefreshCityUICanvas();

        initialized = true;
        return true;
    }

    public void RefreshCityUICanvas()
    {
        if (IsVisibleToHumanPlayer())
            Show();
        else
            Hide();
        
        if (canvasGroup.alpha == 0)
            return;
        Resources cityProduction = GetCityProduction();
        detailsGold.enabled = cityProduction.resources[ResourceType.GOLD] > 0;
        detailsClothes.enabled = cityProduction.resources[ResourceType.CLOTHES] > 0;
        detailsFood.enabled = cityProduction.resources[ResourceType.FOOD] > 0;
        detailsWood.enabled = cityProduction.resources[ResourceType.WOOD] > 0;
        detailsMetal.enabled = cityProduction.resources[ResourceType.METAL] > 0;
        detailsHorses.enabled = cityProduction.resources[ResourceType.MOUNTS] > 0;
        detailsGems.enabled = cityProduction.resources[ResourceType.GEMS] > 0;
        detailsLeather.enabled = cityProduction.resources[ResourceType.LEATHER] > 0;

        alignment.sprite = spritesRepo.GetSprite(Nations.alignments[owner].ToString());
        alignment.enabled = true;
        haven.enabled = isHaven;
        cityName.text = GameObject.Find("Localization").GetComponent<Localization>().Localize(cityId);
        goHealth.SetActive(true);
        nextGameObject.SetActive(game.GetHumanNation() == owner);
    }

    public void GenerateAutomaticAttacks()
    {
        automaticAttacks = new();
        List<HazardCreatureCardDetails> res = cardDetailsRepo.GetHazardCardsOfNation(owner);
        if (res.Count < 1)
            return;
        res.Shuffle();
        res = res.GetRange(0, Math.Min(res.Count, CitySizes.automaticAttacks[size]));
        automaticAttacks = res.Select(x => x.cardId).ToList();
    }

    public bool IsVisibleToHumanPlayer()
    {
        if (!game.GetHumanPlayer().SeesTile(hex))
            return false;

        if (Nations.alignments[game.GetHumanNation()] == Nations.alignments[owner])
            return true;
        else
            return !isHidden;
    }

    void Update()
    {
        if (hudMessages.Count > 0 && !messagesBeingShowed)
            StartCoroutine(ShowHUD());

        if (!initialized || refresh)
        {
            Initialize(refresh);
            if (initialized == true)
                refresh = false;
            return;
        }

        if (!board.IsAllLoaded())
            return;

        if (Input.GetKeyUp(KeyCode.Escape))
            isClicked = false;

        if (isClicked && selectedItems.GetSelectedCity() != null && selectedItems.GetSelectedCity().GetCityId() != cityId)
            isClicked = false;

        displacement.transform.localPosition = new Vector3(0, DisplacementPixels.down, 0);
    }

    IEnumerator ShowHUD()
    {
        messagesBeingShowed = true;
        for (int i = 0; i < hudMessages.Count; i++)
        {
            ShowMessage(hudMessages[i]);
            hudMessages.RemoveAt(i);
            yield return new WaitUntil(() => !messageBeingShowed);
        }
        messagesBeingShowed = false;
    }

    public bool IsInitialized()
    {
        return initialized;
    }

    public string GetCityId()
    {
        return cityId;
    }

    public NationRegionsEnum GetRegion()
    {
        return regionId;
    }

    public void Toggle()
    {
        isClicked = !isClicked;
        if (isClicked)
        {
            board.SelectHex(hex);
            selectedItems.SelectCityUI(this);
        }            
        else if (selectedItems.GetSelectedCity()!= null && selectedItems.GetSelectedCity().GetCityId() != cityId)
        {
            board.SelectHex(Board.NULL);
            selectedItems.UnselectCityDetails();
        }            
    }

    public void Show()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    public void Tap()
    {
        canvasGroup.alpha = 0.5f;
    }
    public void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
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
        placeDeckManager.SetCardToShow(new HoveredCard(owner, cityId, CardClass.Place));
    }
    public void RemoveHoverCity()
    {
        mouse.RemoveCursor();
        placeDeckManager.RemoveCardToShow(new HoveredCard(owner, cityId, CardClass.Place));
    }
    public float GetDistanceTo(Vector2Int hex)
    {
        return Vector2Int.Distance(hex, this.hex);
    }

    public void Damage(int damage)
    {
        health -= damage;

        hudMessageManager.ShowMessage(
            this,
            string.Format("-{0}", damage.ToString()),
            false
        );

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
            objectSlots = GetPlayableObjects();

        return objectSlots;
    }
    public List<RingType> GetPlayableRings(NationsEnum nation)
    {
        List<RingType> objectSlots = new();

        AlignmentsEnum visitor = Nations.alignments[nation];
        AlignmentsEnum city = Nations.alignments[owner];
        if (visitor != city)
            objectSlots = GetPlayableRings();

        return objectSlots;
    }
    public List<string> GetPlayableRingsString(NationsEnum nation)
    {
        List<RingType> objectSlots = new();

        AlignmentsEnum visitor = Nations.alignments[nation];
        AlignmentsEnum city = Nations.alignments[owner];
        if (visitor != city)
            objectSlots = GetPlayableRings();

        return objectSlots.Select(x => x.ToString()).ToList();
    }

    public List<string> GetPlayableObjectsStrings(NationsEnum nation)
    {
        List<ObjectType> objectSlots = new();

        AlignmentsEnum visitor = Nations.alignments[nation];
        AlignmentsEnum city = Nations.alignments[owner];
        if (visitor != city)
            objectSlots = GetPlayableObjects();

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

    public Resources GetCityProduction()
    {
        return new Resources(food, gold, cloth, wood, metal, horses, gems, leather);
    }

    public bool IsHaven()
    {
        return isHaven;
    }

    public Sprite GetSprite()
    {
        return sprite;
    }

    public void GenerateProduction()
    {
        System.Random rd = new();
        food = rd.Next(TerrainBonuses.minBonuses[Terrains.foodBonuses[terrain]], TerrainBonuses.maxBonuses[Terrains.foodBonuses[terrain]]);
        gold = rd.Next(TerrainBonuses.minBonuses[Terrains.goldBonuses[terrain]], TerrainBonuses.maxBonuses[Terrains.goldBonuses[terrain]]);
        cloth = rd.Next(TerrainBonuses.minBonuses[Terrains.clothesBonuses[terrain]], TerrainBonuses.maxBonuses[Terrains.clothesBonuses[terrain]]);
        wood = rd.Next(TerrainBonuses.minBonuses[Terrains.woodBonuses[terrain]], TerrainBonuses.maxBonuses[Terrains.woodBonuses[terrain]]);
        metal = rd.Next(TerrainBonuses.minBonuses[Terrains.metalBonuses[terrain]], TerrainBonuses.maxBonuses[Terrains.metalBonuses[terrain]]);
        gems = rd.Next(TerrainBonuses.minBonuses[Terrains.gemsBonuses[terrain]], TerrainBonuses.maxBonuses[Terrains.gemsBonuses[terrain]]);
        horses = rd.Next(TerrainBonuses.minBonuses[Terrains.horsesBonuses[terrain]], TerrainBonuses.maxBonuses[Terrains.horsesBonuses[terrain]]);
        leather = rd.Next(TerrainBonuses.minBonuses[Terrains.leatherBonuses[terrain]], TerrainBonuses.maxBonuses[Terrains.leatherBonuses[terrain]]);

        switch (size)
        {
            case CitySizesEnum.VERY_BIG:
                gold += (food + cloth + wood + metal + gems + horses + leather);
                break;
            case CitySizesEnum.BIG:
                metal += (int)Math.Round((decimal)(food + cloth + wood + gems + horses + leather) / 2);
                gems += (int)Math.Round((decimal)(food + cloth + wood + metal + horses + leather) / 2);
                break;
            case CitySizesEnum.MEDIUM:
                leather += (int)Math.Round((decimal)(food + cloth + wood + gems + horses + metal) / 2);
                cloth += (int)Math.Round((decimal)(food + metal + wood + gems + horses + leather) / 2);
                break;
            case CitySizesEnum.SMALL:
                wood += (int)Math.Round((decimal)(food + metal + cloth + gems + horses + leather) / 2);
                horses += (int)Math.Round((decimal)(food + metal + wood + gems + cloth + leather) / 2);
                break;
            case CitySizesEnum.VERY_SMALL:
                food += (metal + cloth + wood + gems + horses + leather);
                break;
        }
    }

    public void GeneratePlayableRings()
    {
        playableRings = new();
        foreach (RingType ringType in Enum.GetValues(typeof(RingType)))
        {
            switch (ringType)
            {
                case RingType.MindRing:
                    if (cardType == CardTypesEnum.LAIR || cardType == CardTypesEnum.DARK_BASTION)
                        if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                            playableRings.Add(ringType);
                    break;
                case RingType.DwarvenRing:
                    if (terrain == TerrainsEnum.MOUNTAIN || terrain == TerrainsEnum.OTHER_HILLS_MOUNTAIN || terrain == TerrainsEnum.SNOWHILLS)
                        if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                            playableRings.Add(ringType);
                    break;
                case RingType.MagicRing:
                    if (cardType == CardTypesEnum.WILDERNESS || cardType == CardTypesEnum.FREE_BASTION)
                        if (UnityEngine.Random.Range(0f, 1f) > 0.75f)
                            playableRings.Add(ringType);
                    break;
                case RingType.LesserRing:
                    if (UnityEngine.Random.Range(0f, 1f) > 0.75f)
                        playableRings.Add(ringType);
                    break;
                case RingType.TheOneRing:
                    if (cardType == CardTypesEnum.LAIR || terrain == TerrainsEnum.SWAMP)
                    {
                        if (UnityEngine.Random.Range(0f, 1f) > 0.75f)
                            playableRings.Add(ringType);
                    }
                    break;
            }
        }
    }

    public void GeneratePlayableObjects()
    {
        playableObjects = new();

        if (terrain == TerrainsEnum.SWAMP ||
            terrain == TerrainsEnum.MOUNTAIN ||
            terrain == TerrainsEnum.OTHER_HILLS_MOUNTAIN ||
            terrain == TerrainsEnum.SNOWHILLS ||
            terrain == TerrainsEnum.ICE ||
            terrain == TerrainsEnum.COAST ||
            terrain == TerrainsEnum.SEA
           )
        {

            if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                playableObjects.Add(ObjectType.Palantir);

            if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                playableObjects.Add(ObjectType.Jewelry);
        }

        switch (size)
        {
            case CitySizesEnum.VERY_BIG:
                if (UnityEngine.Random.Range(0f, 1f) > 0.75f)
                    playableObjects.Add(ObjectType.Jewelry);
                if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                    playableObjects.Add(ObjectType.OtherHand);
                if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                    playableObjects.Add(ObjectType.MainHand);
                if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                    playableObjects.Add(ObjectType.Armor);
                break;
            case CitySizesEnum.BIG:
                if (UnityEngine.Random.Range(0f, 1f) > 0.75f)
                    playableObjects.Add(ObjectType.Armor);
                if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                    playableObjects.Add(ObjectType.MainHand);
                if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                    playableObjects.Add(ObjectType.OtherHand);
                if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                    playableObjects.Add(ObjectType.Head);
                break;
            case CitySizesEnum.MEDIUM:
                if (UnityEngine.Random.Range(0f, 1f) > 0.75f)
                    playableObjects.Add(ObjectType.Head);
                if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                    playableObjects.Add(ObjectType.Gloves);
                if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                    playableObjects.Add(ObjectType.Boots);
                break;
            case CitySizesEnum.SMALL:
                if (UnityEngine.Random.Range(0f, 1f) > 0.75f)
                    playableObjects.Add(ObjectType.Gloves);
                if (UnityEngine.Random.Range(0f, 1f) > 0.75f)
                    playableObjects.Add(ObjectType.Boots);
                if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                    playableObjects.Add(ObjectType.Cloak);
                if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                    playableObjects.Add(ObjectType.Belt);
                break;
            case CitySizesEnum.VERY_SMALL:
                if (UnityEngine.Random.Range(0f, 1f) > 0.75f)
                    playableObjects.Add(ObjectType.Cloak);
                if (UnityEngine.Random.Range(0f, 1f) > 0.75f)
                    playableObjects.Add(ObjectType.Belt);
                playableObjects.Add(ObjectType.Consumable);
                playableObjects.Add(ObjectType.Mount);
                break;
        }
    }

    public List<ObjectType> GetPlayableObjects()
    {
        return playableObjects;
    }

    public List<RingType> GetPlayableRings()
    {
        return playableRings;
    }

    public string GetDescForAI()
    {
        return descForAIGeneration;
    }


    public bool IsHidden()
    {
        return isHidden;
    }

    public bool HasPort()
    {
        return hasPort;
    }


    public void AddMessage(string text, float delay, string color)
    {
        hudMessages.Add(new HUDMessage(text, delay, colorManager.GetColor(color)));
    }
    public void AddMessage(string text, float delay, Color color)
    {
        hudMessages.Add(new HUDMessage(text, delay, color));
    }

    public void ShowMessage(HUDMessage hudMessage)
    {
        messageBeingShowed = true;
        message.SetActive(true);
        message.GetComponentInChildren<TextMeshProUGUI>().text = hudMessage.text;
        message.GetComponentInChildren<TextMeshProUGUI>().color = hudMessage.color;
        message.GetComponent<Animation>().Play();
        StartCoroutine(HideMessage(hudMessage.delay));
    }

    IEnumerator HideMessage(float hideMessageSeconds)
    {
        yield return new WaitForSeconds(hideMessageSeconds);
        message.GetComponent<Animation>().Rewind();
        message.SetActive(false);
        messageBeingShowed = false;
        yield return null;
    }
}
