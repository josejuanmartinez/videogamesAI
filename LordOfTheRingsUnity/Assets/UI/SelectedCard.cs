using System.Collections.Generic;
using UnityEngine;

public class SelectedCard
{
    private CityDetails cityDetails;
    private CardDetails movableCardSelected;
    private CardDetails secondaryCardSelected;
    private CardUI movableCardUI;
    private NationsEnum owner;

    private Board board;

    public SelectedCard()
    {
        cityDetails = null;
        movableCardSelected = null;
        secondaryCardSelected = null;
        movableCardUI = null;
        owner = NationsEnum.ABANDONED;
        board = GameObject.Find("Board").GetComponent<Board>();
    }
    public void Select(CardDetails details, NationsEnum owner)
    {
        if (details == null)
            return;
        this.owner = owner;
        cityDetails = null;
        if (details.IsMovableClass())
        {
            movableCardUI = GameObject.Find("Board").GetComponent<Board>().GetCardManager().GetCardUI(details);
            if(movableCardUI != null)
            {
                movableCardSelected = details;
                secondaryCardSelected = null;
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

    public void Select(CityDetails details, NationsEnum owner)
    {
        if (details == null)
            return;
        this.owner = owner;
        cityDetails = details;
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
        return cityDetails != null;
    }

    public CardDetails GetMovableCardSelected()
    {
        return movableCardSelected;
    }
    public CardDetails GetCardSelected()
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

    public string GetMovableCardSelectedId()
    {
        return movableCardSelected != null ? movableCardSelected.cardId : null;
    }

    public CardUI GetMovableCardSelectedUI()
    {
        return movableCardUI;
    }

    public bool IsHazardCreatureSelected()
    {
        if (GetCardSelected() == null)
            return false;
        return GetCardSelected().cardClass == CardClass.HazardCreature;
    }

    public HazardCreatureCardDetails GetHazardCreatureCardDetails()
    {
        if (GetCardSelected() == null)
            return null;
        if (!IsHazardCreatureSelected())
            return null;
        return GetCardSelected() as HazardCreatureCardDetails;
    }

    public CityDetails GetCitySelected()
    {
        return cityDetails;
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