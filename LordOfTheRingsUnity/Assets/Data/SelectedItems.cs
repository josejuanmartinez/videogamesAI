using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SelectedItems : MonoBehaviour
{
    [Header("Initialization")]
    public CanvasGroup lastSelectedMovableCanvasGroup;
    public GameObject lastSelectedMovableLayout;
    public GameObject CharacterCardUIPopupPrefab;
    public CompanyManager companyManagerLayout;

    [Header("Audio")]
    private AudioManager audioManager;
    private AudioRepo audioRepo;

    private SelectedCard selection;

    private Board board;
    private DeckManager deckManager;
        
    private CameraController cameraController;
    private Turn turn;
    private PlaceDeck placeDeckManager;

    private void Awake()
    {
        selection = new SelectedCard();
        board = GameObject.Find("Board").GetComponent<Board>();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        cameraController = Camera.main.GetComponent<CameraController>();
        turn = GameObject.Find("Turn").GetComponent<Turn>();
        placeDeckManager = GameObject.Find("PlaceDeckManager").GetComponent<PlaceDeck>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        audioRepo = GameObject.Find("AudioRepo").GetComponent<AudioRepo>();
    }

    public void CheckIfShowLastChar()
    {
        if (selection == null)
            return;

        int show = 0;
        if (selection.IfPrimaryAndSecondarySelected())
            show = 1;

        if(show != lastSelectedMovableCanvasGroup.alpha)
        {
            lastSelectedMovableCanvasGroup.alpha = show;
            if(show == 1)
            {
                int children = lastSelectedMovableLayout.transform.childCount;
                for (int i = 0; i < children; i++)
                    DestroyImmediate(lastSelectedMovableLayout.transform.GetChild(0).gameObject);

                CardUI cardUI = selection.GetSelectedMovableCardUI();
                if (cardUI != null)
                {
                    if(cardUI.GetCardClass() == CardClass.Character)
                    {
                        GameObject go = Instantiate(CharacterCardUIPopupPrefab, lastSelectedMovableLayout.transform);
                        go.name = cardUI.GetCardId();
                        CharacterCardUIPopup character = go.GetComponent<CharacterCardUIPopup>();
                        character.Initialize(cardUI.GetCardId(), turn.GetCurrentPlayer());
                    }
                    else
                        lastSelectedMovableCanvasGroup.alpha = 0;
                }                
            }            
        }
    }

    public void SelectCityDetails(CityUI city)
    {
        selection.Select(city);
        deckManager.Dirty(DirtyReasonEnum.CHAR_SELECTED);
        cameraController.LookToCity(city);
    }
    public void SelectCardDetails(CardUI card)
    {
        SelectCardDetails(card.GetDetails(), card.GetOwner());
    }

    public void UnselectCityDetails()
    {
        selection = new SelectedCard();
        //deckManager.Dirty(DirtyReasonEnum.CHAR_SELECTED);
        placeDeckManager.RemoveCardToShow(placeDeckManager.GetCardToShow());
    }


    public void SelectCardDetails(CardDetails cardDetails, NationsEnum owner)
    {
        if (cardDetails == null)
            return;

        cameraController.LookToCard(cardDetails);

        if (selection.GetSelectedCard() == cardDetails)
            return;

        if (cardDetails.IsMovableClass())
            deckManager.Dirty(DirtyReasonEnum.CHAR_SELECTED);

        selection.Select(cardDetails, owner);

        placeDeckManager.RemoveCardToShow(placeDeckManager.GetCardToShow());

        if (board.GetCardManager().GetCardUI(cardDetails) == null)
            return;
            
        AudioResource audio = null;
        CardUI card = selection.GetSelectedMovableCardUI();
        if (card != null && card.IsCharacterUI())
            audio = (card as CharacterCardUI).GetVoice();
        if (card != null && card.IsHazardCreatureUI())
            audio = (card as HazardCreatureCardUI).GetVoice();
        
        if(audio != null)
            audioManager.PlaySound(audio);

        companyManagerLayout.Initialize(turn.GetCurrentPlayer());
    }

    public void UnselectCardDetails()
    {
        bool refreshCharSelectedConditions = selection.IsMovableSelected();
        selection = null;
        if (refreshCharSelectedConditions)  
            deckManager.Dirty(DirtyReasonEnum.CHAR_SELECTED);

        placeDeckManager.RemoveCardToShow(placeDeckManager.GetCardToShow());
    }
    public bool IsCardAlreadyInPlay()
    {
        return selection != null && selection.IsCardAlreadyInPlay();
    }
    public bool IsMovableSelected()
    {
        return selection != null && selection.IsMovableSelected();
    }

    public bool IsHazardCreatureSelected()
    {
        return selection != null && selection.IsHazardCreatureSelected();
    }

    public bool IsObjectSelected()
    {
        return selection != null && selection.IsSecondaryCardSelected(CardClass.Object);
    }

    public bool IsEventSelected()
    {
        return selection != null && selection.IsSecondaryCardSelected(CardClass.Event);
    }

    public bool IsFactionSelected()
    {
        return selection != null && selection.IsSecondaryCardSelected(CardClass.Faction);
    }
    public bool IsAllySelected()
    {
        return selection != null && selection.IsSecondaryCardSelected(CardClass.Ally);
    }

    public bool IsHazardEventSelected()
    {
        return selection != null && selection.IsSecondaryCardSelected(CardClass.HazardEvent);
    }
    public bool IsGoldRingSelected()
    {
        return selection != null && selection.IsSecondaryCardSelected(CardClass.GoldRing);
    }
    public bool IsCitySelected()
    {
        return selection != null && selection.IsCitySelected();
    }

    public bool IsMovableCardSelected()
    {
        return selection != null && selection.IsMovableSelected();
    }

    public bool IsCharSelected()
    {
        return selection != null && selection.IsCharacterSelected();
    }

    public void SelectCityUI(CityUI cityUI)
    {
        SelectCityDetails(cityUI);
    }
    public void SelectCardUI(CardUI cardUI)
    {
        SelectCardDetails(cardUI);
    }

    public CardDetails GetSelectedCardDetails()
    {
        if (selection == null)
            return null;
        return selection.GetSelectedCard();
    }
    public CardUI GetSelectedMovableCardUI()
    {
        if (selection == null)
            return null;
        return selection.GetSelectedMovableCardUI();
    }
    public HazardCreatureCardDetails GetHazardCreatureCardDetails()
    {
        if (selection == null)
            return null;
        return selection.GetHazardCreatureCardDetails();
    }
    public CharacterCardDetails GetCharacterCardDetails()
    {
        if (selection == null)
            return null;
        return selection.GetCharacterCardDetails();
    }
    public CityUI GetSelectedCity()
    {
        if (selection == null)
            return null;
        return selection.GetSelectedCity();
    }
    public CardDetails GetSelectedMovableCard()
    {
        if (selection == null)
            return null;
        return selection.GetSelectedMovableCard();
    }

    public HoveredCard GetSelectedCityDetailsAsHover()
    {
        if (selection == null)
            return new HoveredCard();
        CityUI cityDetails = GetSelectedCity();
        if (cityDetails == null)
            return new HoveredCard();
        return new HoveredCard(selection.GetOwner(), cityDetails.GetCityId(), CardClass.Place);
    }
    public HoveredCard GetSelectedCardDetailsAsHover()
    {
        if (selection == null)
            return new HoveredCard();
        CardDetails cardDetails = GetSelectedCardDetails();
        if (cardDetails == null)
            return new HoveredCard();
        return new HoveredCard(selection.GetOwner(), cardDetails.cardId, cardDetails.cardClass);
    }

    public bool IsCardSelected()
    {
        
        return selection != null && selection.IsCardSelected();
    }

    public void UnselectAll()
    {
        UnselectCardDetails();
        UnselectCityDetails();
    }

    public List<CardDetails> GetCompany()
    {
        if(selection == null)
            return new List<CardDetails>();
        return selection.GetCompany();
    }

    public SelectedCard GetSelection()
    {
        return selection;
    }
}
