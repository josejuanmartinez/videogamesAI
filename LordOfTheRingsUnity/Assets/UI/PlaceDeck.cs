using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlaceDeck : MonoBehaviour
{
    public CardTemplateUI cardTemplate;
    public short hideTime = 2;    
    
    private short diceResults = 0;

    private SelectedItems selectedItems;
    private DiceManager diceManager;
    private InputPopupManager inputPopupManager;
    private CombatPopupManager combatPopupManager;
    private ResourcesManager resourcesManager;
    private Turn turn;
    private Board board;
    private DeckManager deckManager;
    private HUDMessageManager hudMessageManager;
    private EventsManager eventsManager;
    private CameraController cameraController;
    private Game game;
    private ManaManager manaManager;

    private HoveredCard cardToShow;

    void Awake()
    {
        selectedItems = GameObject.Find("SelectedItems").GetComponent<SelectedItems>();
        diceManager = GameObject.Find("DiceManager").GetComponent<DiceManager>();
        inputPopupManager = GameObject.Find("InputPopupManager").GetComponent<InputPopupManager>();
        combatPopupManager = GameObject.Find("CombatPopupManager").GetComponent<CombatPopupManager>();
        hudMessageManager = GameObject.Find("HUDMessageManager").GetComponent<HUDMessageManager>();
        manaManager = GameObject.Find("ManaManager").GetComponent<ManaManager>();
        resourcesManager = GameObject.Find("ResourcesManager").GetComponent<ResourcesManager>();
        turn = GameObject.Find("Turn").GetComponent<Turn>();
        board = GameObject.Find("Board").GetComponent<Board>();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        eventsManager = GameObject.Find("EventsManager").GetComponent<EventsManager>();
        cameraController = GameObject.Find("CameraController").GetComponent<CameraController>();
        game = GameObject.Find("Game").GetComponent<Game>();    
    }

    void Update()
    {

        if (!game.FinishedLoading())
            return;
        if (cardToShow == null)
        {
            // SHOWING SELECTED
            HoveredCard selectedCard = null;
            if (selectedItems.IsCitySelected())
                selectedCard = selectedItems.GetSelectedCityDetailsAsHover();
            else if (selectedItems.IsCardSelected())
                selectedCard = selectedItems.GetSelectedCardDetailsAsHover();

            if (selectedCard != null)
            {
                cardToShow = selectedCard;
                cardTemplate.Show();
                cardTemplate.Initialize(
                    selectedCard.GetOwner(),
                    selectedCard.GetCardId(),
                    selectedCard.GetCardClass(),
                    false);
            } 
            else
                cardTemplate.Hide();
            
            return;
        }
        else
        {
            // SHOWING HOVERED
            string shownCard = null;
            if (cardTemplate.GetCardDetails() != null)
                shownCard = cardTemplate.GetCardDetails().cardId;
            else if (cardTemplate.GetCity() != null)
                shownCard = cardTemplate.GetCity().GetCityId();

            if (shownCard == null || (shownCard != null && shownCard != cardToShow.GetCardId()))
            {
                cardTemplate.Show();
                cardTemplate.Initialize(
                    cardToShow.GetOwner(),
                    cardToShow.GetCardId(),
                    cardToShow.GetCardClass(),
                    true);
            }
        }
    }

    public void PlayCard()
    {
        if (!selectedItems.IsCardAlreadyInPlay())
        {
            if (selectedItems.IsMovableSelected())
                PlayCharacter();
            if (selectedItems.IsHazardCreatureSelected())
                PlayHazardCreature();
            if (selectedItems.IsObjectSelected())
                PlayObject();                
            if (selectedItems.IsEventSelected() || selectedItems.IsHazardEventSelected())
                PlayEvent();
            if (selectedItems.IsFactionSelected())
                PlayFaction();
            if (selectedItems.IsAllySelected())
                PlayAlly();
            if (selectedItems.IsGoldRingSelected())
                PlayGoldRing();
        }
    }

    public void GatherDiceResultsForPlayingEvents(int diceValue, CardDetails cardDetails)
    {
        if (cardDetails == null)
            return;

        if (diceValue == 0)
            diceValue = DiceManager.D10;

        diceResults = (short)diceValue;

        int diceRequired = game.RequiredDice(cardDetails.cardClass);
        
        CardUI avatar = board.GetCharacterManager().GetAvatar(turn.GetCurrentPlayer());
        cameraController.LookToCard(avatar);
        
        if (diceResults >= diceRequired)
        {
            CardUI cardUI = eventsManager.AddEvent(cardDetails, turn.GetCurrentPlayer());
            if (cardUI != null)
            {
                hudMessageManager.ShowMessage(
                    avatar,
                    GameObject.Find("Localization").GetComponent<Localization>().Localize(cardDetails.cardId),
                    true
                );
            }
            deckManager.DiscardAndDraw(turn.GetCurrentPlayer(), cardDetails, false);
            RefreshStatusEffects();
        }
        else
        {
            hudMessageManager.ShowMessage(
                    avatar,
                    diceValue.ToString() + " < " + diceRequired.ToString(),
                    false
                );
        }        
        StartCoroutine(HideDices());
    }

    public void GatherDiceResultsForInfluencingFactions(int diceValue, CardDetails cardDetails)
    {
        if (cardDetails == null)
            return;

        if (diceValue == 0)
            diceValue = DiceManager.D10;

        diceResults = (short)diceValue;

        int diceRequired = game.RequiredDice(cardDetails.cardClass);

        CardUI charUI = selectedItems.GetSelectedMovableCardUI();
        if (charUI == null)
            return;

        CharacterCardUI characterCardUI = charUI as CharacterCardUI;
        if (characterCardUI == null)
            return;

        cameraController.LookToCard(charUI);

        if (diceResults >= diceRequired)
        {
            CardUI cardUI = eventsManager.AddFaction(cardDetails, turn.GetCurrentPlayer());
            if (cardUI != null)
            {
                hudMessageManager.ShowMessage(
                   charUI,
                   GameObject.Find("Localization").GetComponent<Localization>().Localize(cardDetails.cardId),
                   true
               );
            }
            deckManager.DiscardAndDraw(turn.GetCurrentPlayer(), cardDetails, false);

        }
        else
        {
            hudMessageManager.ShowMessage(
                    charUI,
                    diceValue.ToString() + " < " + diceRequired.ToString(),
                    false
                );
        }

        StartCoroutine(HideDices());
    }


    public void GatherDiceResultsForRecruitingAllies(int diceValue, CardDetails cardDetails)
    {
        if (cardDetails == null)
            return;

        if (diceValue == 0)
            diceValue = DiceManager.D10;

        diceResults = (short)diceValue;

        int diceRequired = game.RequiredDice(cardDetails.cardClass);

        CardUI charUI = selectedItems.GetSelectedMovableCardUI();
        if (charUI == null)
            return;

        CharacterCardUI characterCardUI = charUI as CharacterCardUI;
        if (characterCardUI == null)
            return;
        
        cameraController.LookToCard(charUI);

        if (diceResults >= diceRequired)
        {
            characterCardUI.AddAlly(cardDetails);
            hudMessageManager.ShowMessage(
                charUI,
                GameObject.Find("Localization").GetComponent<Localization>().Localize(cardDetails.cardId),
                true
            );
            deckManager.DiscardAndDraw(turn.GetCurrentPlayer(), cardDetails, false);
        }
        else
        {
            hudMessageManager.ShowMessage(
                charUI,
                diceValue.ToString() + " < " + diceRequired.ToString(),
                false
            );
        }

        StartCoroutine(HideDices());
    }
    
    public void GatherDiceResultsForAnalyzingGoldRing(int diceValue, CardDetails cardDetails)
    {
        if (cardDetails == null)
            return;

        if (diceValue == 0)
            diceValue = DiceManager.D10;

        diceResults = (short)diceValue;

        GoldRingDetails goldRingDetails = cardDetails as GoldRingDetails;
        if (goldRingDetails == null)
            return;

        if (diceResults <= goldRingDetails.theOneRingMax && diceResults >= goldRingDetails.theOneRingMin)
            goldRingDetails.SetRevealedSlot(RingType.TheOneRing);
        else if (diceResults <= goldRingDetails.dwarvenRingMax && diceResults >= goldRingDetails.dwarvenRingMin)
            goldRingDetails.SetRevealedSlot(RingType.DwarvenRing);
        else if (diceResults <= goldRingDetails.magicRingMax && diceResults >= goldRingDetails.magicRingMin)
            goldRingDetails.SetRevealedSlot(RingType.MagicRing);
        else if (diceResults <= goldRingDetails.mindRingMax && diceResults >= goldRingDetails.mindRingMin)
            goldRingDetails.SetRevealedSlot(RingType.MindRing);
        else
            goldRingDetails.SetRevealedSlot(RingType.LesserRing);

        CardUI charUI = selectedItems.GetSelectedMovableCardUI();
        if (charUI == null)
            return;

        CharacterCardUI characterCardUI = charUI as CharacterCardUI;
        if (characterCardUI == null)
            return;

        cameraController.LookToCard(charUI);

        characterCardUI.AddGoldRing(cardDetails);

        hudMessageManager.ShowMessage(
            charUI,
            string.Format("+{0}",GameObject.Find("Localization").GetComponent<Localization>().Localize(goldRingDetails.GetRevealedSlot().ToString())),
            true
        );
        deckManager.DiscardAndDraw(turn.GetCurrentPlayer(), cardDetails, false);
        
        StartCoroutine(HideDices());
    }

    public void GatherDiceResultsForSpawningCards(int diceValue, SpawnCardLocation spawnCardLocation, CardDetails cardDetails)
    {
        if (cardDetails == null)
            return;
        
        if (diceValue == 0)
            diceValue = DiceManager.D10;

        diceResults = (short)diceValue;

        int diceRequired = game.RequiredDice(cardDetails.cardClass);
        
        if (diceResults >= diceRequired)
        {
            CardUI cardUI = board.CreateCardUI(cardDetails, spawnCardLocation);
            if (cardUI != null)
            {
                hudMessageManager.ShowMessage(
                    cardUI,
                    GameObject.Find("Localization").GetComponent<Localization>().Localize(cardDetails.cardId),
                    true);
                // Update resources
                if (cardDetails.IsClassOf(CardClass.Character))
                {
                    resourcesManager.RecalculateInfluence(turn.GetCurrentPlayer());
                    selectedItems.SelectCardDetails(cardDetails, cardUI.GetOwner());
                }
                // Update mana
                if (cardDetails.IsClassOf(CardClass.HazardCreature))
                {
                    HazardCreatureCardDetails hazard = (HazardCreatureCardDetails) cardDetails;
                    foreach (CardTypesEnum cardType in hazard.GetCardTypes())
                    {
                        int newRes = manaManager.mana[turn.GetCurrentPlayer()][cardType] > 0 ? manaManager.mana[turn.GetCurrentPlayer()][cardType] - 1 : 0;
                        manaManager.mana[turn.GetCurrentPlayer()][cardType] = (short)newRes;
                    }
                    manaManager.Dirty();
                }
            }
            deckManager.DiscardAndDraw(turn.GetCurrentPlayer(), cardDetails, false);
        }
        else
        {
            CardUI cardUI = board.GetCharacterManager().GetAvatar(turn.GetCurrentPlayer());
            cameraController.LookToCard(cardUI);

            hudMessageManager.ShowMessage(
                    cardUI,
                    diceValue.ToString() + " < " + diceRequired.ToString(),
                    false
                );
        }            

        StartCoroutine(HideDices());
    }

    IEnumerator HideDices()
    {
        cardToShow = null;
        selectedItems.UnselectAll();

        yield return new WaitForSeconds(hideTime);
        cardTemplate.button.enabled = true;
    }

    public void RefreshStatusEffects()
    {
        List<CardUI> cards = board.GetCardManager().GetCardsInPlayOfOwner(turn.GetCurrentPlayer());
        foreach (CardUI card in cards)
        {
            if (card as CharacterCardUIBoard != null)
                (card as CharacterCardUIBoard).CheckStatusEffects();
        }
    }
    public void CharacterAtHaven()
    {
        inputPopupManager.HidePopup();
        StartCoroutine(diceManager.RollToSpawnCard(SpawnCardLocation.AtHaven, selectedItems.GetSelectedCardDetails()));
    }

    public void CharacterAtHomeTown()
    {
        inputPopupManager.HidePopup();
        StartCoroutine(diceManager.RollToSpawnCard(SpawnCardLocation.AtHomeTown, selectedItems.GetSelectedCardDetails()));
    }
    public void CastEvent()
    {
        inputPopupManager.HidePopup();
        StartCoroutine(diceManager.RollForEvents(selectedItems.GetSelectedCardDetails()));
    }
    public void InfluenceFaction()
    {
        inputPopupManager.HidePopup();
        StartCoroutine(diceManager.RollForFactions(selectedItems.GetSelectedCardDetails()));
    }

    public void RecruitAlly()
    {
        inputPopupManager.HidePopup();
        StartCoroutine(diceManager.RollForAllies(selectedItems.GetSelectedCardDetails()));
    }

    public void AnalyzeGoldRing()
    {
        inputPopupManager.HidePopup();
        StartCoroutine(diceManager.RollForGoldRing(selectedItems.GetSelectedCardDetails()));
    }
    public void Cancel()
    {
        inputPopupManager.HidePopup();
        cardTemplate.button.enabled = true;
    }

    public void HazardCreatureAtLastCell()
    {
        inputPopupManager.HidePopup();
        StartCoroutine(diceManager.RollToSpawnCard(SpawnCardLocation.AtLastCell, selectedItems.GetSelectedCardDetails()));
    }

    public void HazardCreatureAtHaven()
    {
        inputPopupManager.HidePopup();
        StartCoroutine(diceManager.RollToSpawnCard(SpawnCardLocation.AtHaven, selectedItems.GetSelectedCardDetails()));
    }
    public void TapToGetObject()
    {
        inputPopupManager.HidePopup();

        CardDetails cardDetails = selectedItems.GetSelectedCardDetails();
        if(cardDetails != null)
        {
            CardDetails characterDetails = selectedItems.GetSelectedMovableCard();
            if(characterDetails != null)
            {
                CardUI character = board.GetCardManager().GetCardUI(characterDetails);
                if(character != null)
                {
                    CharacterCardUIBoard original = (CharacterCardUIBoard)character;
                    if(original != null)
                    {
                        CityUI city = board.GetCityManager().GetCityAtHex(original.GetHex());
                        if (city != null && !combatPopupManager.Initialize(character, city))
                                combatPopupManager.HidePopup();
                    }                    
                }                
            }            
        }
    }

    public void PlayCharacter()
    {
        if (selectedItems.GetSelectedCardDetails() == null)
            return;

        OkOption option = new()
        {
            text = GameObject.Find("Localization").GetComponent<Localization>().Localize("home_town"),
            cardBoolFunc = CharacterAtHomeTown
        };
        List<OkOption> options = new() { };

        CardDetails cardDetails = selectedItems.GetSelectedCardDetails();
        if(cardDetails != null)
        {
            string homeTown = deckManager.CanSpawnCharacterAtHome(cardDetails, turn.GetCurrentPlayer());
            if (homeTown != null)
            {
                option.text = GameObject.Find("Localization").GetComponent<Localization>().Localize(homeTown);
                options.Add(option);
            }                
        }

        inputPopupManager.Initialize(GameObject.Find("Localization").GetComponent<Localization>().Localize("recruit_character"), GameObject.Find("Localization").GetComponent<Localization>().Localize("where_to_recruit") + "\n <b>" + GameObject.Find("Localization").GetComponent<Localization>().Localize(cardDetails.cardId) + "?", cardDetails.cardSprite, options, Cancel);
        cardTemplate.button.enabled = false;
    }
    public void PlayHazardCreature()
    {
        if (selectedItems.GetSelectedCardDetails() == null)
            return;

        OkOption option1 = new()
        {
            text = GameObject.Find("Localization").GetComponent<Localization>().Localize("last_hex"),
            cardBoolFunc = HazardCreatureAtLastCell
        };
        OkOption option2 = new()
        {
            text = GameObject.Find("Localization").GetComponent<Localization>().Localize("at_haven"),
            cardBoolFunc = HazardCreatureAtHaven
        };
        List<OkOption> options = new() {  };

        CardDetails cardDetails = selectedItems.GetSelectedCardDetails();
        if (cardDetails != null)
        {
            Vector2 hex = deckManager.CanSpawnHazardCreatureAtLastHex();
            if (hex.x != float.MinValue && hex.y != float.MinValue)
            {
                option1.text = hex.ToString();
                options.Add(option1);
            }
        }

        options.Add(option2);

        string message = GameObject.Find("Localization").GetComponent<Localization>().Localize("where_to_recruit") + "\n <b>" + GameObject.Find("Localization").GetComponent<Localization>().Localize(cardDetails.cardId) + "</b>?\n\n";

        inputPopupManager.Initialize(GameObject.Find("Localization").GetComponent<Localization>().Localize("spawn_a_creature"), message, cardDetails.cardSprite, options, Cancel);
        cardTemplate.button.enabled = false;
    }

    public void PlayObject()
    {
        if (selectedItems.GetSelectedMovableCard() == null)
            return;

        CardDetails cardDetails = selectedItems.GetSelectedCardDetails();
        if (cardDetails == null)
            return;

        CardDetails characterDetails = selectedItems.GetSelectedMovableCard();
        if (characterDetails == null)
            return;

        if (characterDetails.cardClass != CardClass.Character)
            return;

        CardUI character = board.GetCardManager().GetCardUI(characterDetails);
        if (character == null) 
            return;

        CharacterCardUIBoard original = character as CharacterCardUIBoard;
        if (original == null)
            return;

        CityUI city = board.GetCityManager().GetCityAtHex(original.GetHex());
        if (city == null)
            return;
        
        OkOption option1 = new()
        {
            text = GameObject.Find("Localization").GetComponent<Localization>().Localize("Enter"),
            cardBoolFunc = TapToGetObject
        };
        List<OkOption> options = new() { option1 };

        inputPopupManager.Initialize(GameObject.Find("Localization").GetComponent<Localization>().Localize(city.GetCityId()), GameObject.Find("Localization").GetComponent<Localization>().Localize("do_you_want_to_enter_this_place"), city.GetSprite(), options, Cancel);
        cardTemplate.button.enabled = false;
    }

    public void PlayFaction()
    {
        if (selectedItems.GetSelectedCardDetails() == null)
            return;

        CardDetails cardDetails = selectedItems.GetSelectedCardDetails();
        if (cardDetails == null)
            return;

        OkOption option = new()
        {
            text = GameObject.Find("Localization").GetComponent<Localization>().Localize("Play"),
            cardBoolFunc = InfluenceFaction
        };
        List<OkOption> options = new() { option };


        inputPopupManager.Initialize(GameObject.Find("Localization").GetComponent<Localization>().Localize("faction"), GameObject.Find("Localization").GetComponent<Localization>().Localize("do_you_want_to_influence_faction") + "\n <b>" + GameObject.Find("Localization").GetComponent<Localization>().Localize(cardDetails.cardId) + "?", cardDetails.cardSprite, options, Cancel);
        cardTemplate.button.enabled = false;
    }

    public void PlayAlly()
    {
        if (selectedItems.GetSelectedCardDetails() == null)
            return;

        CardDetails cardDetails = selectedItems.GetSelectedCardDetails();
        if (cardDetails == null)
            return;

        OkOption option = new()
        {
            text = GameObject.Find("Localization").GetComponent<Localization>().Localize("Recruit"),
            cardBoolFunc = RecruitAlly
        };
        List<OkOption> options = new() { option };


        inputPopupManager.Initialize(GameObject.Find("Localization").GetComponent<Localization>().Localize("ally"), GameObject.Find("Localization").GetComponent<Localization>().Localize("do_you_want_to_recruite_ally") + "\n <b>" + GameObject.Find("Localization").GetComponent<Localization>().Localize(cardDetails.cardId) + "?", cardDetails.cardSprite, options, Cancel);
        cardTemplate.button.enabled = false;
    }

    public void PlayGoldRing()
    {
        if (selectedItems.GetSelectedCardDetails() == null)
            return;

        CardDetails cardDetails = selectedItems.GetSelectedCardDetails();
        if (cardDetails == null)
            return;

        OkOption option = new()
        {
            text = GameObject.Find("Localization").GetComponent<Localization>().Localize("analyze"),
            cardBoolFunc = AnalyzeGoldRing
        };
        List<OkOption> options = new() { option };


        inputPopupManager.Initialize(GameObject.Find("Localization").GetComponent<Localization>().Localize("gold_ring"), GameObject.Find("Localization").GetComponent<Localization>().Localize("do_you_want_to_analyze_the_gold_ring") + "\n <b>" + GameObject.Find("Localization").GetComponent<Localization>().Localize(cardDetails.cardId) + "?", cardDetails.cardSprite, options, Cancel);
        cardTemplate.button.enabled = false;
    }

    public void PlayEvent()
    {
        if (selectedItems.GetSelectedCardDetails() == null)
            return;

        CardDetails cardDetails = selectedItems.GetSelectedCardDetails();
        if (cardDetails == null)
            return;

        OkOption option = new()
        {
            text = GameObject.Find("Localization").GetComponent<Localization>().Localize("Play"),
            cardBoolFunc = CastEvent
        };
        List<OkOption> options = new() { option };


        inputPopupManager.Initialize(GameObject.Find("Localization").GetComponent<Localization>().Localize("event"), GameObject.Find("Localization").GetComponent<Localization>().Localize("do_you_want_to_cast_event") + "\n <b>" + GameObject.Find("Localization").GetComponent<Localization>().Localize(cardDetails.cardId) + "?", cardDetails.cardSprite, options, Cancel);
        cardTemplate.button.enabled = false;
    }

    public short GetDiceResults()
    {
        return diceResults;
    }

    public void SetCardToShow(HoveredCard newCardToShow)
    {
        cardToShow = newCardToShow;
    }

    public HoveredCard GetCardToShow()
    {
        return cardToShow;
    }

    public void RemoveCardToShow(HoveredCard cardToHide)
    {
        if (cardToShow == null)
            return;
        if (cardToHide == null)
            return;

        if (cardToShow.GetCardId() == cardToHide.GetCardId() && cardToShow.GetOwner() == cardToHide.GetOwner())
            cardToShow = null;
    }
}
