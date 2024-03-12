using System.Collections.Generic;
using UnityEngine;

public class SelectedCard
{
    private CityUI city;
    private CardDetails movableCardSelected;
    private CardDetails secondaryCardSelected;
    private CardUI movableCardUI;
    private NationsEnum owner;

    private Board board;

    public SelectedCard()
    {
        city = null;
        movableCardSelected = null;
        secondaryCardSelected = null;
        movableCardUI = null;
        owner = NationsEnum.ABANDONED;
        board = GameObject.Find("Board").GetComponent<Board>();
    }

    public void Select(CardUI cardUI)
    {
        Select(cardUI.GetDetails(), cardUI.GetOwner());
    }

    public void Select(CardDetails details, NationsEnum owner)
    {
        if (details == null)
            return;
        this.owner = owner;
        city = null;
        if (details.IsMovableClass())
        {
            movableCardUI = GameObject.Find("Board").GetComponent<Board>().GetCardManager().GetCardUI(details);
            if(movableCardUI != null)
            {
                movableCardSelected = details;
                secondaryCardSelected = null;
                movableCardUI.MoveToTopBoard();
            }
            else
            {
                movableCardSelected = null;
                secondaryCardSelected = details;
            }
        }
        else
            secondaryCardSelected = details;
    }

    public void Select(CityUI city)
    {
        if (city == null)
            return;
        owner = city.GetOwner();
        this.city = city;
        movableCardSelected = null;
        movableCardUI = null;
        secondaryCardSelected = null;
    }

    public bool IsMovableSelected()
    {
        return movableCardSelected != null;
    }

    public bool IsCharacterSelected()
    {
        return movableCardSelected != null && movableCardSelected as CharacterCardDetails != null;
    }

    public bool IfPrimaryAndSecondarySelected()
    {
        return movableCardSelected != null && secondaryCardSelected != null;
    }
    public bool IsSecondaryCardSelected(CardClass cardClass)
    {
        return secondaryCardSelected != null && secondaryCardSelected.cardClass == cardClass;
    }

    public bool IsCitySelected()
    {
        return city != null;
    }

    public CardDetails GetSelectedMovableCard()
    {
        return movableCardSelected;
    }
    public CardDetails GetSelectedCard()
    {
        if (secondaryCardSelected != null)
            return secondaryCardSelected;
        else if (movableCardSelected != null)
            return movableCardSelected;
        else
            return null;
    }
    public bool IsCardSelected()
    {
        return movableCardSelected != null || secondaryCardSelected != null;
    }

    public string GetSelectedMovableCardId()
    {
        return movableCardSelected != null ? movableCardSelected.cardId : null;
    }

    public CardUI GetSelectedMovableCardUI()
    {
        return movableCardUI;
    }

    public bool IsHazardCreatureSelected()
    {
        if (GetSelectedCard() == null)
            return false;
        return GetSelectedCard().cardClass == CardClass.HazardCreature;
    }

    public HazardCreatureCardDetails GetHazardCreatureCardDetails()
    {
        if (GetSelectedCard() == null)
            return null;
        if (!IsHazardCreatureSelected())
            return null;
        return GetSelectedCard() as HazardCreatureCardDetails;
    }

    public CityUI GetSelectedCity()
    {
        return city;
    }
    
    public bool IsCardAlreadyInPlay()
    {
        CardDetails card = IfPrimaryAndSecondarySelected()? secondaryCardSelected : movableCardSelected;
        if (card != null)
        {
            CardUI cardUI = board.GetCardManager().GetCardUI(card);
            return cardUI != null && card.isUnique;
        }
        else
            return false;
    }

    public List<CardDetails> GetCompany()
    { 
        if (!IsMovableSelected())
            return new List<CardDetails>();
        else
            return board.GetCharacterManager().GetMovingWithMe(movableCardSelected);
    }

    public NationsEnum GetOwner()
    {
        return owner;
    }
}