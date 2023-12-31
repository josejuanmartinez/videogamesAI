using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public struct HUDMessage {
    public string text;
    public Color color;
    public float delay;

    public HUDMessage(string text, float delay, Color color)
    {
        this.text = text;
        this.delay = delay;
        this.color = color;
    }
}

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
        
    protected bool isAwaken = false;
    protected bool initialized;
    protected bool messageBeingShowing;
    protected List<HUDMessage> hudMessages;

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
        hudMessages = new();
        initialized = false;
        messageBeingShowing = false;
    }

    public virtual bool Initialize(string cardId, NationsEnum owner, bool refresh = false)
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

        details ??= GetComponentInChildren<CharacterCardDetails>();
        if (details == null || refresh)
        {
            GameObject prefab = cardDetailsRepo.GetCardGameObject(cardId, owner);
            if (prefab == null)
            {
                Debug.LogError(string.Format("Unable to find in initial decks prefab {0}", cardId));
                return false;
            }

            GameObject cardObject = Instantiate(prefab);
            cardObject.name = cardId + "_details";
            cardObject.transform.SetParent(transform);
            details = cardObject.GetComponent<CardDetails>();
        }        

        cardName.text = GameObject.Find("Localization").GetComponent<Localization>().Localize(details.cardId);
        image.sprite = details.cardSprite;
        alignmentIcon.sprite = spritesRepo.GetSprite(Nations.alignments[owner].ToString());

        initialized = true;

        //Debug.Log(string.Format("{0} finished loading at {1}", cardObject.name, Time.realtimeSinceStartup));
        return initialized;
    }

    void Update()
    {
        if (hudMessages.Count > 0 && !messageBeingShowing)
        {
            messageBeingShowing = true;
            ShowMessage(hudMessages[0]);
            hudMessages.RemoveAt(0);
        }
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
        if (GetComponentInChildren<CharacterCardDetails>() != null)
            return GetComponentInChildren<CharacterCardDetails>();
        else
            return null;
    }
    public AllyCardDetails GetAllyCardDetails()
    {
        if (GetComponentInChildren<AllyCardDetails>() != null)
            return GetComponentInChildren<AllyCardDetails>();
        else
            return null;
    }
    

    public HazardCreatureCardDetails GetHazardCreatureDetails()
    {
        if (GetComponentInChildren<HazardCreatureCardDetails>() != null)
            return GetComponentInChildren<HazardCreatureCardDetails>();
        else
            return null;
    }
    public GoldRingDetails GetGoldRingDetails()
    {
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
        if (details == null)
            return;
        placeDeckManager.RemoveCardToShow(new HoveredCard(owner, details.cardId, details.cardClass));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (details == null)
            return;
        placeDeckManager.SetCardToShow(new HoveredCard(owner, details.cardId, details.cardClass));
    }

    public NationsEnum GetOwner()
    {
        return owner;
    }

    public string GetCardId()
    {
        return cardId;
    }

    public CardDetails GetDetails()
    {
        return details;
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
        messageBeingShowing = false;
        yield return null;
    }
}
