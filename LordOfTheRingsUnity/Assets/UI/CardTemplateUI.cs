using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class CardTemplateUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Card Template UI")]
    [Header("References")]
    public Image image;
    public GameObject descriptionLayout;
    public TextMeshProUGUI quote;
    public TextMeshProUGUI hometown;
    public TextMeshProUGUI title;
    public TextMeshProUGUI influence;
    public SimpleTooltip influenceTooltip;
    public TextMeshProUGUI mind;
    public SimpleTooltip mindTooltip;
    public TextMeshProUGUI prowess;
    public SimpleTooltip prowessTooltip;
    public TextMeshProUGUI defence;
    public SimpleTooltip defenceTooltip;
    public TextMeshProUGUI probability;
    public SimpleTooltip probabilityTooltip;
    public CanvasGroup probabilityCanvasGroup;
    public GameObject mindGroup;
    public GameObject influenceGroup;
    public GameObject prowessGroup;
    public GameObject defenceGroup;
    public Button button;
    public Image frame;
    public Image frametop;
    public Image framebottom;
    [SerializeField]
    private SimpleTooltip tooltip;

    [Header("Resources")]
    public GameObject resources;
    public GameObject resourcePrefab;

    [Header("Name Ellipsis")]
    public int ellipsisSize = 10;
    public float animationSpeed = 1f;

    protected CardDetails cardDetails;
    protected CityDetails cityDetails;

    protected List<CardCondition> conditions;
    protected Resources missingResources;
    protected Dictionary<CardTypesEnum, int> missingMana;
    protected HashSet<PlayableConditionResultEnum> conditionsFailed;

    protected bool animate;
    protected string cardName;
    protected float accDelta;
    protected int startIndex;

    protected Turn turn;
    protected Mouse mouse;
    protected SelectedItems selectedItems;
    protected PlaceDeck placeDeckManager;
    protected ResourcesManager resourcesManager;
    protected Board board;
    protected CardDetailsRepo cardDetailsRepo;
    protected SpritesRepo spritesRepo;
    protected ColorManager colorManager;
    protected DeckManager deck;
    protected Game game;
    protected ManaManager manaManager;
    protected ContentGenerator contentGenerator;

    protected bool initialized;
    protected bool isAwaken = false;
    protected NationsEnum owner;
    protected List<DirtyReasonEnum> dirtyMessages;

    private bool isInPlay;

    void Awake()
    {
        turn = GameObject.Find("Turn").GetComponent<Turn>();
        mouse = GameObject.Find("Mouse").GetComponent<Mouse>();
        selectedItems = GameObject.Find("SelectedItems").GetComponent<SelectedItems>();
        placeDeckManager = GameObject.Find("PlaceDeckManager").GetComponent<PlaceDeck>();
        resourcesManager = GameObject.Find("ResourcesManager").GetComponent<ResourcesManager>();
        board = GameObject.Find("Board").GetComponent<Board>();
        cardDetailsRepo = GameObject.Find("CardDetailsRepo").GetComponent<CardDetailsRepo>();
        spritesRepo = GameObject.Find("SpritesRepo").GetComponent<SpritesRepo>();
        colorManager = GameObject.Find("ColorManager").GetComponent<ColorManager>();
        deck = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        game = GameObject.Find("Game").GetComponent<Game>();
        manaManager = GameObject.Find("ManaManager").GetComponent<ManaManager>();
        if (GameObject.Find("ContentGenerator")!= null)
            contentGenerator = GameObject.Find("ContentGenerator").GetComponent<ContentGenerator>();

        initialized = false;
        animate = false;
        cardName = "";
        accDelta = 0f;
        startIndex = 0;
        isAwaken = true;
        owner = NationsEnum.ABANDONED;
        dirtyMessages = new List<DirtyReasonEnum>();
        isInPlay = false;
    }

    public virtual bool Initialize(NationsEnum owner, string cardId, CardClass cardClass)
    {
        if (!isAwaken)
            Awake();

        if (!game.IsInitialized() || !board.IsAllLoaded() || !turn.IsNewTurnLoaded())
            return false;

        if (cardId == null)
            return false;

        if (cardClass == CardClass.Place)
        {
            if (board.GetCityManager().GetCityUI(cardId) != null)
                return InitializeCity(owner, board.GetCityManager().GetCityUI(cardId).GetDetails());
        }
        else
        {
            if (board.GetCardManager().GetCardUI(cardId) != null)
                return InitializeCard(owner, board.GetCardManager().GetCardUI(cardId).GetDetails(), true);
            else if (cardDetailsRepo.GetCardDetails(cardId, owner) != null)
                return InitializeCard(owner, cardDetailsRepo.GetCardDetails(cardId, owner), false);
        }

        return false;
    }

    protected bool InitializeCard(NationsEnum owner, CardDetails cardDetails, bool isInPlay)
    {
        this.cardDetails = cardDetails;
        this.owner = owner;
        this.isInPlay = isInPlay;

        if (!isAwaken)
            Awake();

        if (!game.IsInitialized() || !board.IsAllLoaded() || !turn.IsNewTurnLoaded())
            return false;

        conditions = new();
        missingMana = new();
        missingResources = new Resources(0, 0, 0, 0, 0, 0, 0, 0);
        conditionsFailed = new();

        cityDetails = null;

        if (cardDetails == null)
            return false;

        this.owner = owner;

        //COMMON
        frame.color = colorManager.GetColor(cardDetails.cardClass.ToString());
        frametop.color = colorManager.GetColor(cardDetails.cardClass.ToString());
        framebottom.color = colorManager.GetColor(cardDetails.cardClass.ToString());
        hometown.enabled = false;
        cardName = GameObject.Find("Localization").GetComponent<Localization>().Localize(cardDetails.cardId);
        image.sprite = cardDetails.cardSprite;
        animate = cardName.Length > ellipsisSize;
        if (!animate)
            title.text = cardName;
        int children = descriptionLayout.transform.childCount;
        for (int i = 0; i < children; i++)
            DestroyImmediate(descriptionLayout.transform.GetChild(0).gameObject);

        bool hasStats = (cardDetails.cardClass == CardClass.Ally || cardDetails.cardClass == CardClass.Character || cardDetails.cardClass == CardClass.Object || cardDetails.cardClass == CardClass.HazardCreature);

        //STATS
        mindGroup.SetActive(hasStats);
        influenceGroup.SetActive(hasStats);
        prowessGroup.SetActive(hasStats);
        defenceGroup.SetActive(hasStats);

        ShowProbability();
        button.enabled = true;

        if (cardDetails.IsClassOf(CardClass.Character))
        {
            CharacterCardDetails charDetails = (CharacterCardDetails)cardDetails;
            if (charDetails == null)
                return false;

            if (IsDeckCard())
            {
                SetMind(charDetails.GetMind());
                SetInfluence(charDetails.GetInfluence());
                SetProwess(charDetails.GetProwess());
                SetDefence(charDetails.GetDefence());
            }
            else
                SetInPlayCharacterStats(charDetails);

            SetAdditionalCharacterInformation(charDetails);
        }
        else if (cardDetails.IsClassOf(CardClass.HazardCreature))
        {
            mindGroup.SetActive(false);
            influenceGroup.SetActive(false);
            
            HazardCreatureCardDetails hazardCreatureDetails = (HazardCreatureCardDetails)cardDetails;
            SetProwess(hazardCreatureDetails.prowess);
            SetDefence(hazardCreatureDetails.defence);
            AddDescriptionSlot("races", hazardCreatureDetails.GetRaceStrings());
            AddDescriptionSlot("abilities", hazardCreatureDetails.GetAbilitiesStrings(), spritesRepo.GetSprite("unknown_ability"));
        }
        else if (cardDetails.IsClassOf(CardClass.Event))
        {
            EventCardDetails eventDetails = (EventCardDetails)cardDetails;
            AddDescriptionSlot("duration", eventDetails.eventType.ToString(), spritesRepo.GetSprite("duration"));
            AddDescriptionSlot("corruption", eventDetails.CalculateCorruption().ToString(), spritesRepo.GetSprite("corruption"));
            AddDescriptionSlot("effects", eventDetails.GetEffectsStrings(), spritesRepo.GetSprite("unknown_ability"));
        }
        else if (cardDetails.IsClassOf(CardClass.HazardEvent))
        {
            HazardEventCardDetails eventDetails = (HazardEventCardDetails)cardDetails;
            AddDescriptionSlot("duration", eventDetails.eventType.ToString(), spritesRepo.GetSprite("duration"));
            AddDescriptionSlot("corruption", eventDetails.CalculateCorruption().ToString(), spritesRepo.GetSprite("corruption"));
            AddDescriptionSlot("effects", eventDetails.GetEffectsStrings(), spritesRepo.GetSprite("unknown_ability"));

        }
        else if (cardDetails.IsClassOf(CardClass.Object))
        {
            ObjectCardDetails objectDetails = (ObjectCardDetails) cardDetails;
            SetProwess(objectDetails.prowess);
            SetDefence(objectDetails.defence);
            SetMind(objectDetails.mind);
            SetInfluence(objectDetails.influence);
            AddDescriptionSlot("object_slot", objectDetails.objectSlot.ToString(), spritesRepo.GetSprite("object_slot"));
            AddDescriptionSlot("allowed_classes", objectDetails.requiredClass.Count > 0 ? objectDetails.GetClassesStrings() : new List<string>() { "all_classes" }, spritesRepo.GetSprite("unknown_class"));
            AddDescriptionSlot("abilities", objectDetails.GetAbilitiesString(), spritesRepo.GetSprite("unknown_ability"));

        }
        else if (cardDetails.IsClassOf(CardClass.Ally))
        {
            AllyCardDetails allyDetails = (AllyCardDetails)cardDetails;
            hometown.enabled = true;
            hometown.text = GameObject.Find("Localization").GetComponent<Localization>().Localize(allyDetails.GetHomeTown());

            SetProwess(allyDetails.GetProwess());
            SetDefence(allyDetails.GetDefence());
            SetMind(allyDetails.GetMind());
            influenceGroup.SetActive(false);
            AddDescriptionSlot("abilities", allyDetails.GetAbilitiesStrings(), spritesRepo.GetSprite("unknown_ability"));

        }
        else if (cardDetails.IsClassOf(CardClass.Faction))
        {
            FactionCardDetails factionDetails = (FactionCardDetails)cardDetails;
            hometown.enabled = true;
            hometown.text = GameObject.Find("Localization").GetComponent<Localization>().Localize(factionDetails.GetHomeTown());
            AddDescriptionSlot("abilities", factionDetails.GetFactionAbility().ToString(), spritesRepo.GetSprite("unknown_ability"));

        }
        else if (cardDetails.IsClassOf(CardClass.Ring))
        {
            RingCardDetails objectDetails = (RingCardDetails) cardDetails;
            SetProwess(objectDetails.prowess);
            SetDefence(objectDetails.defence);
            SetMind(objectDetails.mind);
            SetInfluence(objectDetails.influence);
            AddDescriptionSlot("object_slot", objectDetails.objectSlot.ToString(), spritesRepo.GetSprite("object_slot"));
            AddDescriptionSlot("allowed_classes", objectDetails.requiredClass.Count > 0 ? objectDetails.GetClassesStrings() : new List<string>() { "all_classes" }, spritesRepo.GetSprite("unknown_class"));
            AddDescriptionSlot("abilities", objectDetails.GetAbilitiesStrings(), spritesRepo.GetSprite("unknown_ability"));
        }
        else if (cardDetails.IsClassOf(CardClass.GoldRing))
        {
            GoldRingDetails objectDetails = (GoldRingDetails) cardDetails;
            AddDescriptionSlot("the_one_ring", objectDetails.OneRing(), spritesRepo.GetSprite("the_one_ring"));
            AddDescriptionSlot("dwarvenring", objectDetails.DwarvenRing(), spritesRepo.GetSprite("dwarvenring"));
            AddDescriptionSlot("magicring", objectDetails.MagicRing(), spritesRepo.GetSprite("magicring"));
            AddDescriptionSlot("mindring", objectDetails.MindRing(), spritesRepo.GetSprite("mindring"));
        }

        quote.text = GameObject.Find("Localization").GetComponent<Localization>().LocalizeQuote(cardDetails.cardId);

        #if UNITY_EDITOR
        if (contentGenerator != null &&
            string.IsNullOrEmpty(quote.text) &&
            !contentGenerator.IsSleeping() &&
            !contentGenerator.Generating() &&
            contentGenerator.StillNotGenerated(cardDetails.cardId) && 
            !contentGenerator.Ignored(cardDetails.cardId))
        {
            if (UnityEditor.EditorUtility.DisplayDialog("Generate with AI?", cardDetails.cardId, "OK", "Ignore"))
                contentGenerator.Generate(cardDetails.cardId, GameObject.Find("Localization").GetComponent<Localization>().CreateDescriptionForGeneration(cardDetails));
            else
            {
                if (UnityEditor.EditorUtility.DisplayDialog("Sleep?", cardDetails.cardId, "OK", "No"))
                    contentGenerator.AddIgnore(cardDetails.cardId, true);
                else
                    contentGenerator.AddIgnore(cardDetails.cardId, false);
            }
                
        }
        #endif

        ClearAllRequirements();
        CreateConditions();
        dirtyMessages.Add(DirtyReasonEnum.INITIALIZATION);
        initialized = true;

        return true;
    }

    private bool InitializeCity(NationsEnum owner, CityDetails cityDetails)
    {
        cardDetails = null;
        isInPlay = true;
        this.cityDetails = cityDetails;

        if (!isAwaken)
            Awake();

        if (cityDetails == null)
            return false;

        if (!isAwaken)
            Awake();

        if (!game.IsInitialized() || !board.IsAllLoaded() || !turn.IsNewTurnLoaded())
            return false;

        conditions = new();
        missingMana = new();
        missingResources = new Resources(0, 0, 0, 0, 0, 0, 0, 0);
        conditionsFailed = new();

        CityUI cityUI = board.GetCityManager().GetCityUI(cityDetails.cityId);
        if (cityUI == null)
            return false;

        this.owner = owner;

        Destroy(GetComponent<AutoZoom>());

        int children = descriptionLayout.transform.childCount;
        for (int i = 0; i < children; i++)
            DestroyImmediate(descriptionLayout.transform.GetChild(0).gameObject);

        quote.text = GameObject.Find("Localization").GetComponent<Localization>().LocalizeQuote(cityDetails.cityId);

        HideProbability();
        button.enabled = false;

        mindGroup.SetActive(false);
        prowessGroup.SetActive(false);
        defenceGroup.SetActive(false);
        influenceGroup.SetActive(false);

        cardName = GameObject.Find("Localization").GetComponent<Localization>().Localize(cityDetails.cityId);
        image.sprite = cityDetails.GetSprite();
        animate = cardName.Length > ellipsisSize;
        if (!animate)
            title.text = cardName;

        hometown.enabled = true;
        hometown.text = GameObject.Find("Localization").GetComponent<Localization>().Localize(cityDetails.regionId);


        if (!cityUI.IsRevealedOrHiddenVisible(turn.GetCurrentPlayer()))
            return true;

        if (cityUI.GetOwner() == NationsEnum.ABANDONED)
            AddDescriptionSlot("owner", "abandoned", spritesRepo.GetSprite("abandoned"));
        else
            AddDescriptionSlot("owner", cityUI.GetOwner().ToString());

        AddDescriptionSlot("objects", cityUI.GetPlayableObjectsStrings(turn.GetCurrentPlayer()), spritesRepo.GetSprite("object"));
        AddDescriptionSlot("rings", cityUI.GetPlayableRingsString(turn.GetCurrentPlayer()), spritesRepo.GetSprite("ring"));
        AddDescriptionSlot("automatic_attacks", cityUI.GetAutomaticAttacks(turn.GetCurrentPlayer()), spritesRepo.GetSprite("default"));
        if(cityDetails.isHidden)
            AddDescriptionSlot("city_visibility", "hidden");
        if(cityDetails.hasPort)
            AddDescriptionSlot("port", "has_port");
        if(cityDetails.hasHoard)
            AddDescriptionSlot("hoard", "has_hoard");

        ClearAllRequirements();
        ShowProduction();

        #if UNITY_EDITOR
        if (contentGenerator != null &&
            string.IsNullOrEmpty(quote.text) &&
            !contentGenerator.IsSleeping() &&
            !contentGenerator.Generating() &&
            contentGenerator.StillNotGenerated(cityDetails.cityId) &&
            !contentGenerator.Ignored(cityDetails.cityId))
        {
            if (UnityEditor.EditorUtility.DisplayDialog("Generate with AI?", cityDetails.cityId, "OK", "Ignore"))
                contentGenerator.Generate(cityDetails.cityId, GameObject.Find("Localization").GetComponent<Localization>().CreateDescriptionForGeneration(cityDetails));
            else
            {
                if (UnityEditor.EditorUtility.DisplayDialog("Sleep?", cityDetails.cityId, "OK", "No"))
                    contentGenerator.AddIgnore(cityDetails.cityId, true);
                else
                    contentGenerator.AddIgnore(cityDetails.cityId, false);
            }

        }
        #endif


        initialized = true;
        return true;
    }

    public void ShowProduction()
    {
        if (cityDetails == null)
            return;
        Resources production = cityDetails.GetCityProduction();
        foreach(ResourceType resource in Enum.GetValues(typeof(ResourceType))) 
        {
            int prod = production.resources[resource];
            if(prod>0)
                InstantiateResource(resource.ToString(), prod);
        }
    }

    public virtual DeckCardUIRequirement InstantiateResource(string spriteId, int value)
    {
        if (value < 1)
            return null;
        GameObject go = Instantiate(resourcePrefab, resources.transform);
        go.name = spriteId + "_resource";
        go.transform.SetParent(resources.transform);

        DeckCardUIRequirement deckCardUIrequirement = go.GetComponent<DeckCardUIRequirement>();
        deckCardUIrequirement.Initialize(spriteId, value);

        return deckCardUIrequirement;
    }

    public void ClearAllRequirements()
    {
        int productionChildren = resources.transform.childCount;
        for (int i = 0; i < productionChildren; i++)
            DestroyImmediate(resources.transform.GetChild(0).gameObject);
    }

    public CardDescriptionSlot AddDescriptionSlot(string title, string stringId, Sprite fallbackIcon = null)
    {
        CardDescriptionSlot res = null;
        GameObject goImage = new();
        goImage.transform.SetParent(descriptionLayout.transform, false);
        CardDescriptionSlot desc = goImage.AddComponent<CardDescriptionSlot>();
        bool result = desc.Initialize(title, stringId, fallbackIcon);
        if (!result)
            DestroyImmediate(desc);
        else
            res = desc;
        
        return res;
    }
    public List<CardDescriptionSlot> AddDescriptionSlot(string title, List<string> stringIds, Sprite fallbackIcon = null)
    {
        List<CardDescriptionSlot> res = new();
        foreach(string stringId in  stringIds)
        {
            GameObject goImage = new();
            goImage.transform.SetParent(descriptionLayout.transform, false);
            CardDescriptionSlot desc = goImage.AddComponent<CardDescriptionSlot>();
         
            bool result = desc.Initialize(title, stringId, fallbackIcon);
            if (!result)
                DestroyImmediate(desc);
            else
                res.Add(desc);
        }
        return res;
    }    

    public void SetInPlayCharacterStats(CharacterCardDetails charDetails)
    {
        CardUI cardUI = board.GetCardManager().GetCardUI(cardDetails);
        CharacterCardUI characterUI = (CharacterCardUI)cardUI;
        if (characterUI == null)
        {
            SetMind(charDetails.GetMind());
            SetInfluence(charDetails.GetInfluence());
            SetProwess(charDetails.GetProwess());
            SetDefence(charDetails.GetDefence());
            Debug.LogError("It seems " + charDetails.cardId + " is not a deck card but could not retrieve playing version!");
            return;
        }            

        int totalMind = characterUI.GetTotalMind();
        SetMind(totalMind,
            totalMind < charDetails.GetMind() ? "success" : (totalMind > charDetails.GetMind() ? "failure" : null)
            );

        int totalInfluence = characterUI.GetTotalInfluence();
        SetInfluence(totalInfluence,
            totalInfluence > charDetails.influence ? "success" : (totalInfluence < charDetails.GetInfluence() ? "failure" : null)
            );

        int totalProwess = characterUI.GetTotalProwess();
        SetProwess(totalProwess,
            totalProwess > charDetails.prowess ? "success" : (totalProwess < charDetails.GetProwess() ? "failure" : null)
            );

        int totalDefence = characterUI.GetTotalDefence();
        SetDefence(totalDefence,
            totalDefence > charDetails.defence ? "success" : (totalDefence < charDetails.GetDefence() ? "failure" : null)
            );

    }

    void SetAdditionalCharacterInformation(CharacterCardDetails charDetails)
    {
        hometown.enabled = true;
        hometown.text = GameObject.Find("Localization").GetComponent<Localization>().Localize(charDetails.homeTown);

        AddDescriptionSlot("race", charDetails.GetRaceSubRaceStrings());
        List<string> classes = charDetails.GetClassesStrings();
        AddDescriptionSlot("classes", classes, spritesRepo.GetSprite("unknown_class"));
        AddDescriptionSlot("abilities", charDetails.GetAbilitiesStrings(), spritesRepo.GetSprite("unknown_ability"));
    }
    
    protected virtual void Update()
    {
        if (!initialized)
        {
            if(cardDetails != null)
            {
                Debug.Log("Initializing " + cardDetails.name);
                Initialize(owner, cardDetails.cardId, cardDetails.cardClass);
                cityDetails = null;
            }                
            else if(cityDetails != null)
            {
                Debug.Log("Initializing " + cityDetails.name);
                Initialize(owner, cityDetails.cityId, CardClass.Place);
                cardDetails = null;
            }
            return;
        }
        if (game.GetHumanPlayer().GetNation() == owner)
        {
            if (dirtyMessages.Count() > 0 && this.cardDetails != null)
                RefreshRequirements();
            AnimateName();
        }

        if (button != null)
            button.interactable = button.enabled ? conditionsFailed.Count() < 1 : true;
    }

    public void AnimateName()
    {
        if (animate && accDelta >= animationSpeed)
        {
            accDelta = 0;
            if (startIndex + ellipsisSize >= cardName.Length)
                startIndex = 0;
            else
                startIndex++;
            title.text = cardName.Substring(startIndex, ellipsisSize);
        }
        accDelta += Time.deltaTime;
    }

    public void Dirty(DirtyReasonEnum dirty)
    {
        dirtyMessages.Add(dirty);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (!isAwaken)
            Awake();

        if (button.interactable && conditionsFailed.Count() < 1)
        {
            mouse.ChangeCursor("clickable");
            tooltip.HideTooltip();
        }            
        else
        {
            string missingCondition = conditionsFailed.First().ToString();
            mouse.ChangeCursor("unclickable");
            tooltip.ShowTooltip();
            tooltip.infoLeft = GameObject.Find("Localization").GetComponent<Localization>().Localize("missing_conditions");
            tooltip.infoRight = GameObject.Find("Localization").GetComponent<Localization>().Localize(missingCondition);
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (!isAwaken)
            Awake();

        mouse.RemoveCursor();
        tooltip.HideTooltip();
    }

    public virtual void OnClick()
    {
        if (selectedItems.GetSelectedCardDetails() != null && selectedItems.GetSelectedCardDetails() == cardDetails)
            placeDeckManager.PlayCard();
    }

    private void ShowProbability()
    {
        if (cardDetails.isUnique)
        {
            CardUI card = board.GetCardManager().GetCardUI(cardDetails);
            if (card != null)
            {
                HideProbability();
                return;
            }            
        }
        int requiredDice = game.RequiredDice(cardDetails.cardClass);
        if (requiredDice < 2)
        {
            HideProbability();
            return;
        }

        probabilityCanvasGroup.alpha = 1;
        probability.text = requiredDice.ToString();
        probabilityTooltip.enabled = true;
        probabilityTooltip.infoLeft = GameObject.Find("Localization").GetComponent<Localization>().Localize("probability");
        probabilityTooltip.infoRight = probability.text;
        
    }

    private void HideProbability()
    {
        probabilityCanvasGroup.alpha = 0;
        probability.text = "";
        probabilityTooltip.enabled = false;
    }

    public bool IsDeckCard()
    {
        if (cardDetails.cardClass == CardClass.Place)
            return false;
        return GetComponent<DeckCardUI>() != null || board.GetCardManager().GetCardUI(cardDetails) == null;
    }

    public CardDetails GetCardDetails()
    {
        return cardDetails;
    }

    public CityDetails GetCityDetails() 
    { 
        return cityDetails; 
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
        cardDetails = null;
        cityDetails = null;
    }


    public virtual void RefreshRequirements()
    {
        if (owner == turn.GetCurrentPlayer())
        {
            foreach(DirtyReasonEnum isDirty in dirtyMessages)
            {
                conditionsFailed = new();
                foreach (CardCondition cc in conditions)
                {
                    if (cc.GetDirtyCheck() == isDirty|| isDirty == DirtyReasonEnum.INITIALIZATION)
                    {
                        if (cc.RunCondition().Count() > 0)
                        {
                            conditionsFailed.AddRange(cc.GetLastResult());
                        }
                            
                    }
                }
            }            
        }
        dirtyMessages = new List<DirtyReasonEnum>();
    }

    public HashSet<PlayableConditionResultEnum> IsObjectCardPlayable()
    {
        ObjectCardDetails details = cardDetails as ObjectCardDetails;
        if (details == null)
            return new HashSet<PlayableConditionResultEnum>() { PlayableConditionResultEnum.SLOT_NOT_FOUND };

        CharacterCardUIBoard characterUI = selectedItems.GetSelectedMovableCardUI() as CharacterCardUIBoard;
        if (characterUI == null)
            return new HashSet<PlayableConditionResultEnum>() { PlayableConditionResultEnum.SLOT_NOT_FOUND };

        CityUI city = board.GetCityManager().GetCityAtHex(characterUI.GetHex());
        if (city == null)
            return new HashSet<PlayableConditionResultEnum>() { PlayableConditionResultEnum.SLOT_NOT_FOUND };

        bool slotFound = false;
        foreach (ObjectType obj in city.GetPlayableObjects(owner))
        {
            if (obj == details.objectSlot)
            {
                slotFound = true;
                break;
            }
        }

        HashSet<PlayableConditionResultEnum> result = new();
        if (!slotFound)
            result.Add(PlayableConditionResultEnum.SLOT_NOT_FOUND);

        return result;
    }
    public HashSet<PlayableConditionResultEnum> IsGoldRingCardPlayable()
    {
        CharacterCardUIBoard characterUI = selectedItems.GetSelectedMovableCardUI() as CharacterCardUIBoard;
        if (characterUI == null)
            return new HashSet<PlayableConditionResultEnum>() { PlayableConditionResultEnum.RING_TYPE_NOT_FOUND };
        CityUI city = board.GetCityManager().GetCityAtHex(characterUI.GetHex());
        if (city == null)
            return new HashSet<PlayableConditionResultEnum>() { PlayableConditionResultEnum.RING_TYPE_NOT_FOUND };

        HashSet<PlayableConditionResultEnum> result = new();
        if (city.GetPlayableRings(characterUI.GetOwner()).Count() < 1)
            result.Add(PlayableConditionResultEnum.RING_TYPE_NOT_FOUND);

        return result;
    }

    public HashSet<PlayableConditionResultEnum> IsSelectedCharacter()
    {
        HashSet<PlayableConditionResultEnum> result = new();

        CharacterCardUIBoard characterUI = selectedItems.GetSelectedMovableCardUI() as CharacterCardUIBoard;
        if (characterUI == null)
            result.Add(PlayableConditionResultEnum.SELECT_CHAR);
        else if (characterUI.IsMoving())
            result.Add(PlayableConditionResultEnum.SELECT_CHAR);

        return result;
    }

    public HashSet<PlayableConditionResultEnum> IsSelectedCharacterAtCity()
    {
        HashSet<PlayableConditionResultEnum> result = new();

        CharAtCityRequiredEnum cityReq = cardDetails.IsCharAtCityRequired();
        if (cityReq == CharAtCityRequiredEnum.NONE)
            return result;

        CharacterCardUIBoard characterUI = selectedItems.GetSelectedMovableCardUI() as CharacterCardUIBoard;
        if (characterUI == null)
        {
            result.Add(PlayableConditionResultEnum.SELECT_CHAR);
            result.Add(PlayableConditionResultEnum.NOT_AT_CITY);
        }
        else if (characterUI.IsMoving())
        {
            result.Add(PlayableConditionResultEnum.NOT_AT_CITY);
            result.Add(PlayableConditionResultEnum.SELECT_CHAR);
        }
        else
        {
            CityUI city = board.GetCityManager().GetCityAtHex(characterUI.GetHex());
            if (city == null)
                result.Add(PlayableConditionResultEnum.NOT_AT_CITY);
            else
            {
                switch (cityReq)
                {
                    case CharAtCityRequiredEnum.FOREIGNCITY:
                        if (Nations.alignments[city.GetOwner()] == Nations.alignments[turn.GetCurrentPlayer()])
                            result.Add(PlayableConditionResultEnum.NOT_AT_CITY);
                        break;
                    case CharAtCityRequiredEnum.OWNCITY:
                        if (Nations.alignments[city.GetOwner()] != Nations.alignments[turn.GetCurrentPlayer()])
                            result.Add(PlayableConditionResultEnum.NOT_AT_CITY);
                        break;
                    case CharAtCityRequiredEnum.SPECIFICCITY:
                        string hometown = cardDetails.GetHomeTown();
                        if (!string.IsNullOrEmpty(hometown) && hometown != city.GetCityId())
                            result.Add(PlayableConditionResultEnum.NOT_AT_CITY);
                        break;
                }
            }

            //if (cityDetails.IsTapped(owner))
            //    result.Add(PlayableConditionResultEnum.CITY_TAPPED);
        }

        return result;
    }

    public HashSet<PlayableConditionResultEnum> IsEnoughMana()
    {
        HashSet<PlayableConditionResultEnum> result = new();
        HazardCreatureCardDetails details = cardDetails as HazardCreatureCardDetails;
        if (details == null)
            return new HashSet<PlayableConditionResultEnum>() { PlayableConditionResultEnum.NO_MANA };

        missingMana = manaManager.GetMissingMana(owner, details.cardTypes);

        if(missingMana.Count() > 0)
            result.Add(PlayableConditionResultEnum.NO_MANA);

        return result;
    }

    public HashSet<PlayableConditionResultEnum> HasPlayerInfluence()
    {
        CharacterCardDetails character = cardDetails as CharacterCardDetails;
        if (character == null)
            return new HashSet<PlayableConditionResultEnum>() { PlayableConditionResultEnum.NO_INFLUENCE };

        HashSet<PlayableConditionResultEnum> result = new();

        if (resourcesManager.GetFreeInfluence(turn.GetCurrentPlayer(), false) < character.GetMind())
            result.Add(PlayableConditionResultEnum.NO_INFLUENCE);

        return result;
    }

    public HashSet<PlayableConditionResultEnum> IsRingCardPlayable()
    {
        RingCardDetails details = cardDetails as RingCardDetails;
        if (details == null)
            return new HashSet<PlayableConditionResultEnum>() { PlayableConditionResultEnum.RING_TYPE_NOT_FOUND };

        HashSet<PlayableConditionResultEnum> result = new();
        bool foundSlot = false;

        CharacterCardUIBoard characterUI = selectedItems.GetSelectedMovableCardUI() as CharacterCardUIBoard;
        if (characterUI == null)
            return new HashSet<PlayableConditionResultEnum>() { PlayableConditionResultEnum.RING_TYPE_NOT_FOUND };

        if (characterUI.GetGoldRings().Count < 1)
            result.Add(PlayableConditionResultEnum.RING_TYPE_NOT_FOUND);
        else
        {
            foreach (CardDetails ringDetails in characterUI.GetGoldRings())
            {
                GoldRingDetails goldRingDetails = ringDetails as GoldRingDetails;
                if (goldRingDetails == null)
                    continue;
                if (goldRingDetails.GetRevealedSlot() == details.objectSlot)
                {
                    foundSlot = true;
                    break;
                }
            }
        }

        if (!foundSlot)
            result.Add(PlayableConditionResultEnum.RING_TYPE_NOT_FOUND);

        return result;
    }

    public virtual void CreateConditions()
    {
        if (isInPlay)
            return;
        conditions.Add(new CardCondition(DirtyReasonEnum.NEW_RESOURCES, () => CalculateMissingResources()));
        switch (cardDetails.cardClass)
        {
            case CardClass.HazardCreature:
                conditions.Add(new CardCondition(DirtyReasonEnum.NEW_MANA,
                    () => IsEnoughMana()));
                break;
            case CardClass.Character:
                conditions.Add(new CardCondition(DirtyReasonEnum.NEW_INFLUENCE,
                    () => HasPlayerInfluence()));
                break;
            case CardClass.Object:
                conditions.Add(new CardCondition(DirtyReasonEnum.CHAR_SELECTED,
                    () => IsSelectedCharacter()));
                conditions.Add(new CardCondition(DirtyReasonEnum.CHAR_SELECTED,
                    () => IsObjectCardPlayable()));
                break;
            case CardClass.Ring:
                conditions.Add(new CardCondition(DirtyReasonEnum.CHAR_SELECTED,
                    () => IsSelectedCharacterAtCity()));
                conditions.Add(new CardCondition(DirtyReasonEnum.CHAR_SELECTED,
                    () => IsRingCardPlayable()));
                break;
            case CardClass.GoldRing:
                conditions.Add(new CardCondition(DirtyReasonEnum.CHAR_SELECTED,
                    () => IsSelectedCharacterAtCity()));
                conditions.Add(new CardCondition(DirtyReasonEnum.CHAR_SELECTED,
                    () => IsGoldRingCardPlayable()));
                break;
            case CardClass.Faction:
                conditions.Add(new CardCondition(DirtyReasonEnum.CHAR_SELECTED,
                    () => IsSelectedCharacterAtCity()));
                break;
            case CardClass.Event:
                conditions.Add(new CardCondition(DirtyReasonEnum.CHAR_SELECTED,
                    () => IsSelectedCharacterAtCity()));
                break;
            case CardClass.HazardEvent:
                conditions.Add(new CardCondition(DirtyReasonEnum.CHAR_SELECTED,
                    () => IsSelectedCharacterAtCity()));
                break;
            case CardClass.Ally:
                conditions.Add(new CardCondition(DirtyReasonEnum.CHAR_SELECTED,
                    () => IsSelectedCharacterAtCity()));
                break;
        }
        //conditionResults ??= new HashSet<PlayableConditionResultEnum>() { PlayableConditionResultEnum.NULL };
        //return conditionResults;
    }

    public virtual HashSet<PlayableConditionResultEnum> CalculateMissingResources()
    {   
        missingResources = new(0, 0, 0, 0, 0, 0, 0, 0);

        //FOOD REQUIRED
        int requiredFood = cardDetails.GetResourcesRequired().resources[ResourceType.FOOD];
        int availableFood = resourcesManager.GetFoodStores();
        missingResources.resources[ResourceType.FOOD] = requiredFood > availableFood ? requiredFood - availableFood : 0;

        //GOLD REQUIRED
        int requiredClothes = cardDetails.GetResourcesRequired().resources[ResourceType.CLOTHES];
        int availableClothes = resourcesManager.GetClothesStores();
        missingResources.resources[ResourceType.CLOTHES] = requiredClothes > availableClothes ? requiredClothes - availableClothes : 0;

        //GOLD REQUIRED
        int requiredGold = cardDetails.GetResourcesRequired().resources[ResourceType.GOLD];
        int availableGold = resourcesManager.GetGoldStores();
        missingResources.resources[ResourceType.GOLD] = requiredGold > availableGold ? requiredGold - availableGold : 0;

        //HORSES REQUIRED
        int requiredHorses = cardDetails.GetResourcesRequired().resources[ResourceType.MOUNTS];
        int availableHorses = resourcesManager.GetHorsesStores();
        missingResources.resources[ResourceType.MOUNTS] = requiredHorses > availableHorses ? requiredHorses - availableHorses : 0;

        //WOOD REQUIRED
        int requiredWood = cardDetails.GetResourcesRequired().resources[ResourceType.WOOD];
        int availableWood = resourcesManager.GetWoodStores();
        missingResources.resources[ResourceType.WOOD] = requiredWood > availableWood ? requiredWood - availableWood : 0;

        //METAL REQUIRED
        int requiredMetal = cardDetails.GetResourcesRequired().resources[ResourceType.METAL];
        int availableMetal = resourcesManager.GetMetalStores();
        missingResources.resources[ResourceType.METAL] = requiredMetal > availableMetal ? requiredMetal - availableMetal : 0;

        //GEMS REQUIRED
        int requiredGems = cardDetails.GetResourcesRequired().resources[ResourceType.GEMS];
        int availableGems = resourcesManager.GetGemsStores();
        missingResources.resources[ResourceType.GEMS] = requiredGems > availableGems ? requiredGems - availableGems : 0;

        //LEATHER REQUIRED
        int requiredLeather = cardDetails.GetResourcesRequired().resources[ResourceType.LEATHER];
        int availableLeather = resourcesManager.GetLeatherStores();
        missingResources.resources[ResourceType.LEATHER] = requiredLeather > availableLeather ? requiredLeather - availableLeather : 0;

        HashSet<PlayableConditionResultEnum> result = new();
        if (missingResources.Sum() > 0)
            result.Add(PlayableConditionResultEnum.NO_RESOURCES);
        return result;
    }

    public void SetProwess(int prowessValue, string color = null)
    {
        if(prowessValue < 1)
        {
            prowessGroup.SetActive(false);
            return;
        }
        prowess.text = prowessValue.ToString();
        prowessTooltip.infoLeft = GameObject.Find("Localization").GetComponent<Localization>().Localize("prowess");
        prowessTooltip.infoRight = prowessValue.ToString();
        prowess.color = color == null ? Color.white : colorManager.GetColor(color);
    }

    public void SetDefence(int defenceValue, string color = null)
    {
        if (defenceValue < 1)
        {
            defenceGroup.SetActive(false);
            return;
        }
        defence.text = defenceValue.ToString();
        defenceTooltip.infoLeft = GameObject.Find("Localization").GetComponent<Localization>().Localize("defence");
        defenceTooltip.infoRight = defenceValue.ToString();
        defence.color = color == null ? Color.white : colorManager.GetColor(color);
    }
    public void SetMind(int mindValue, string color = null)
    {
        if (mindValue < 1)
        {
            mindGroup.SetActive(false);
            return;
        }
        mind.text = mindValue.ToString();
        mindTooltip.infoLeft = GameObject.Find("Localization").GetComponent<Localization>().Localize("mind");
        mindTooltip.infoRight = mindValue.ToString();
        mind.color = color == null ? Color.white : colorManager.GetColor(color);
    }
    public void SetInfluence(int influenceValue, string color = null)
    {
        if (influenceValue < 1)
        {
            influenceGroup.SetActive(false);
            return;
        }
        influence.text = influenceValue.ToString();
        influenceTooltip.infoLeft = GameObject.Find("Localization").GetComponent<Localization>().Localize("influence");
        influenceTooltip.infoRight = influenceValue.ToString();
        influence.color = color == null ? Color.white : colorManager.GetColor(color);
    }
}
