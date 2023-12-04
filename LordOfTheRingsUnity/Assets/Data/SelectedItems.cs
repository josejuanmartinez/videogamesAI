using System;
using System.Collections.Generic;
using UnityEngine;

public class SelectedItems : MonoBehaviour
{
    [Header("Initialization")]
    public CanvasGroup lastSelectedMovableCanvasGroup;
    public GameObject lastSelectedMovableLayout;
    public GameObject CharacterCardUIPopupPrefab;
    public CompanyManager companyManagerLayout;

    private SelectedCard selection;

    private Board board;
    private DeckManager deckManager;
        
    private CameraController cameraController;
    private Turn turn;
    private PlaceDeck placeDeckManager;
    private Game game;

    private void Awake()
    {
        selection = new SelectedCard();
        board = GameObject.Find("Board").GetComponent<Board>();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        cameraController = Camera.main.GetComponent<CameraController>();
        turn = GameObject.Find("Turn").GetComponent<Turn>();
        placeDeckManager = GameObject.Find("PlaceDeckManager").GetComponent<PlaceDeck>();
        game = GameObject.Find("Game").GetComponent<Game>();
    }

    private void Update()
    {
        if (!game.FinishedLoading())
            return;
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            UnselectAll();
            return;
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            CardUI next = board.GetNextCardUI(selection.GetMovableCardSelectedUI());
            if (next != null)
                SelectCardDetails(next.GetDetails(), next.GetOwner());
        }
        CheckIfShowLastChar();
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

                CardUI cardUI = selection.GetMovableCardSelectedUI();
                if (cardUI != null)
                {
                    if(cardUI.GetCardClass() == CardClass.Character)
                    {
                        GameObject go = Instantiate(CharacterCardUIPopupPrefab, lastSelectedMovableLayout.transform);
                        go.name = cardUI.GetCardID();
                        CharacterCardUIPopup character = go.GetComponent<CharacterCardUIPopup>();
                        character.Initialize(cardUI.GetCardID(), turn.GetCurrentPlayer());
                    }
                    else
                        lastSelectedMovableCanvasGroup.alpha = 0;
                }                
            }            
        }
    }

    public void SelectCityDetails(CityDetails city, NationsEnum owner)
    {
        selection.Select(city, owner);
        deckManager.Dirty(DirtyReasonEnum.CHAR_SELECTED);
        cameraController.LookToCity(city);
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

        if (selection.GetCardSelected() == cardDetails)
            return;

        if(cardDetails.IsMovableClass())
            deckManager.Dirty(DirtyReasonEnum.CHAR_SELECTED);

        selection.Select(cardDetails, owner);

        if (board.GetCardManager().GetCardUI(cardDetails) == null)
            return;

        companyManagerLayout.Initialize(turn.GetCurrentPlayer());

        cameraController.LookToCard(cardDetails);
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
        SelectCityDetails(cityUI.GetDetails(), cityUI.GetOwner());
    }

    public CardDetails GetSelectedCardDetails()
    {
        if (selection == null)
            return null;
        return selection.GetCardSelected();
    }
    public CardUI GetSelectedMovableCardUI()
    {
        if (selection == null)
            return null;
        return selection.GetMovableCardSelectedUI();
    }
    public HazardCreatureCardDetails GetHazardCreatureCardDetails()
    {
        if (selection == null)
            return null;
        return selection.GetHazardCreatureCardDetails();
    }
    public CityDetails GetSelectedCityDetails()
    {
        if (selection == null)
            return null;
        return selection.GetCitySelected();
    }
    public CardDetails GetSelectedMovableCard()
    {
        if (selection == null)
            return null;
        return selection.GetMovableCardSelected();
    }

    public HoveredCard GetSelectedCityDetailsAsHover()
    {
        if (selection == null)
            return new HoveredCard();
        CityDetails cityDetails = GetSelectedCityDetails();
        if (cityDetails == null)
            return new HoveredCard();
        return new HoveredCard(selection.GetOwner(), cityDetails.GetCityID(), CardClass.Place);
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
}
