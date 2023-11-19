using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Card UI")]
    [Header("References")]
    public TextMeshProUGUI cardName;
    public Image image;
    public Image alignmentIcon;
    public Image frame;
    public CanvasGroup canvasGroup;
    public GameObject message;
    
    [Header("Initialization")]
    [SerializeField]
    protected NationsEnum owner;

    protected string cardId;
    protected Board board;
    protected InputPopupManager inputPopupManager;
    protected Tilemap t;
    protected SelectedItems selectedItems;
    protected Game game;
    protected Turn turn;
    protected CardDetailsRepo cardDetailsRepo;
    protected ResourcesManager resourcesManager;
    protected Mouse mouse;
    protected CardDetails details;
    protected SpritesRepo spritesRepo;
    protected PlaceDeck placeDeckManager;
    protected EventsManager eventsManager;
    protected ColorManager colorManager;
    protected DeckManager deckManager;
    protected FOWManager fowManager;

    protected bool initialized = false;
    protected bool isAwaken = false;

    void Awake()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
        game = GameObject.Find("Game").GetComponent<Game>();
        selectedItems = GameObject.Find("SelectedItems").GetComponent<SelectedItems>();
        t = GameObject.Find("CardTypeTilemap").GetComponent<Tilemap>();
        inputPopupManager = GameObject.Find("InputPopupManager").GetComponent<InputPopupManager>();
        turn = GameObject.Find("Turn").GetComponent<Turn>();
        spritesRepo = GameObject.Find("SpritesRepo").GetComponent<SpritesRepo>();
        cardDetailsRepo = GameObject.Find("CardDetailsRepo").GetComponent<CardDetailsRepo>();
        resourcesManager = GameObject.Find("ResourcesManager").GetComponent<ResourcesManager>();
        mouse = GameObject.Find("Mouse").GetComponent<Mouse>();
        placeDeckManager = GameObject.Find("PlaceDeckManager").GetComponent<PlaceDeck>();
        eventsManager = GameObject.Find("EventsManager").GetComponent<EventsManager>();
        colorManager = GameObject.Find("ColorManager").GetComponent<ColorManager>();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        fowManager = GameObject.Find("FOWManager").GetComponent<FOWManager>();
        isAwaken = true;
    }

    public virtual bool Initialize(string cardId, NationsEnum owner)
    {
        if(!isAwaken)
            Awake();

        this.cardId = cardId;
        this.owner = owner;

        if (!board.IsInitialized())
            return false;

        if (!cardDetailsRepo.IsInitialized())
            return false;

        if (!game.IsInitialized())
            return false;

        if (!fowManager.IsInitialized())
            return false;

        GameObject prefab = cardDetailsRepo.GetCardGameObject(cardId, owner);
        if(prefab == null)
        {
            Debug.LogError(string.Format("Unable to find in initial decks prefab {0}", cardId));
            return false;
        }

        GameObject cardObject = Instantiate(prefab);
        cardObject.name = cardId + "_details";
        cardObject.transform.SetParent(transform);
        details = cardObject.GetComponent<CardDetails>();

        cardName.text = GameObject.Find("Localization").GetComponent<Localization>().Localize(details.cardId);
        image.sprite = details.cardSprite;
        alignmentIcon.sprite = spritesRepo.GetSprite(Nations.alignments[owner].ToString());

        return true;
    }

    public bool IsCharacterCardUIBoard()
    {
        return this is CharacterCardUIBoard;
    }

    public bool IsHazardCreatureCardUIBoard()
    {
        return this is HazardCreatureCardUIBoard;
    }

    public bool IsCharacterUI()
    {
        return this is CharacterCardUI;
    }

    public bool IsHazardCreatureUI()
    {
        return this is HazardCreatureCardUI;
    }

    public CharacterCardDetails GetCharacterDetails()
    {
        if (!initialized)
            Debug.LogError("Calling to `GetCharacterDetails` but CardUI still not initialized!");
        if (GetComponentInChildren<CharacterCardDetails>() != null)
            return GetComponentInChildren<CharacterCardDetails>();
        else
            return null;
    }
    public AllyCardDetails GetAllyCardDetails()
    {
        if (!initialized)
            Debug.LogError("Calling to `GetAllyCardDetails` but CardUI still not initialized!");
        if (GetComponentInChildren<AllyCardDetails>() != null)
            return GetComponentInChildren<AllyCardDetails>();
        else
            return null;
    }
    

    public HazardCreatureCardDetails GetHazardCreatureDetails()
    {
        if (!initialized)
            Debug.LogError("Calling to `GetHazardCharacterDetails` but CardUI still not initialized!");
        if (GetComponentInChildren<HazardCreatureCardDetails>() != null)
            return GetComponentInChildren<HazardCreatureCardDetails>();
        else
            return null;
    }
    public GoldRingDetails GetGoldRingDetails()
    {
        if (!initialized)
            Debug.LogError("Calling to `GetGoldRingDetails` but CardUI still not initialized!");
        if (GetComponentInChildren<GoldRingDetails>() != null)
            return GetComponentInChildren<GoldRingDetails>();
        else
            return null;
    }
    public CardClass GetCardClass()
    {
        if (!initialized)
            return CardClass.NONE;
        if (!details)
            return CardClass.NONE;
        return details.cardClass;
    }

    public bool IsAvatar()
    {
        if (!initialized)
            Debug.LogError("Calling to `IsAvatar` but CardUI still not initialized!");
        if (GetCardClass() == CardClass.Character && GetComponentInChildren<CharacterCardDetails>() != null)
            return GetComponentInChildren<CharacterCardDetails>().isAvatar;
        else
            return false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        placeDeckManager.RemoveCardToShow(new HoveredCard(owner, details.cardId, details.cardClass));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        placeDeckManager.SetCardToShow(new HoveredCard(owner, details.cardId, details.cardClass));
    }

    public NationsEnum GetOwner()
    {
        return owner;
    }

    public string GetCardID()
    {
        return cardId;
    }

    public CardDetails GetDetails()
    {
        return details;
    }
    public void ShowMessage(string text, float delay, string color)
    {
        message.SetActive(true);
        message.GetComponentInChildren<TextMeshProUGUI>().text = text;
        message.GetComponentInChildren<TextMeshProUGUI>().color = colorManager.GetColor(color);
        message.GetComponent<Animation>().Play();
        StartCoroutine(HideMessage(delay));
    }
    public void ShowMessage(string text, float delay, Color color)
    {
        message.SetActive(true);
        message.GetComponentInChildren<TextMeshProUGUI>().text = text;
        message.GetComponentInChildren<TextMeshProUGUI>().color = color;
        message.GetComponent<Animation>().Play();
        StartCoroutine(HideMessage(delay));
    }
    IEnumerator HideMessage(float hideMessageSeconds)
    {
        yield return new WaitForSeconds(hideMessageSeconds);
        message.GetComponent<Animation>().Rewind();
        message.SetActive(false);
        yield return null;
    }
}
