using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardTile
{
    Vector2Int hex;
    CityUI city;
    readonly List<CardUI> cards = new();
    
    public BoardTile(Vector2Int hex, CityUI city, List<CardUI> cards)
    {
        this.hex = hex;
        this.city = city;
        this.cards = cards;
    }

    public BoardTile(Vector2Int hex, CityUI city)
    {
        this.hex = hex;
        this.city = city;
        this.cards = new List<CardUI>();
    }

    public BoardTile(Vector2Int hex, CardUI card)
    {
        this.hex = hex;
        this.city = null;
        this.cards = new List<CardUI>() { card };
    }

    public BoardTile(Vector2Int hex)
    {
        this.hex = hex;
        this.city = null;
        this.cards = new List<CardUI>();
    }

    public BoardTile(Vector2Int hex, List<CardUI> cards)
    {
        this.hex = hex;
        this.city = null;
        this.cards = cards;
    }

    public void RemoveCard(CardUI card)
    {
        cards.Remove(card);
    }

    public void AddCard(CardUI card)
    {
        cards.Add(card);
        SetFirstAtHex(card);
    }

    public void RemoveCity()
    {
        city = null;
    }

    public void AddCity(CityUI city)
    {
        this.city = city; 
    }

    public bool HasCity()
    {
        return city != null;
    }

    public bool HasCards()
    {
        return cards.Count > 0;
    }

    public CityUI GetCity()
    { 
        return city; 
    }

    public List<CardUI> GetCardsUI()
    { 
        return cards; 
    }
    public bool IsHiddenByOtherCharacter(CardUI card)
    {
        List<CardUI> units = GetVisibleUnitsAtHex();
        // Just one, then show it!
        if (units.Count <= 1)
            return false;
        // More than one but I'm the first!
        if (units.Count > 1 && units[0] == card)
            return false;
        return true;
    }

    public List<CardUI> GetVisibleUnitsAtHex()
    {
        return cards.FindAll(x => (x.IsHazardCreatureUI()) || (x.IsCharacterUI() && string.IsNullOrEmpty(((CharacterCardUI)x).GetInCompanyOf()))).ToList();
    }

    public int GetTotalUnitsAtHex()
    {
        return GetVisibleUnitsAtHex().Count;
    }

    public CardUI GetNextAtHex(CardUI card)
    {
        List<CardUI> allAtHex = GetVisibleUnitsAtHex();
        if (allAtHex.Count < 1)
            return null;
        if(allAtHex.Count == 1)
            return allAtHex[0];

        int pos = allAtHex.FindIndex(x => x.GetCardId() == card.GetCardId());
        if (pos == -1)
            return null;

        return allAtHex[(pos + 1) % allAtHex.Count];
    }

    public void SwapPositions(CardUI oldCard, CardUI newCard)
    {
        int newPosition = cards.IndexOf(newCard);
        int oldPosition = cards.IndexOf(oldCard);
        if(newPosition != -1 && oldPosition != -1 && newPosition != oldPosition)
            ListExtensions.Swap(cards, oldPosition, newPosition);
    }
    public void SetFirstAtHex(CardUI card)
    {
        List<CardUI> totalUnits = GetVisibleUnitsAtHex();
        if (totalUnits.Count < 1)
            return;
        if (totalUnits.Count == 1)
            return;

        int pos = totalUnits.IndexOf(card);
        if (pos < 1)
            return;

        int totalCards = cards.Count;
        CardUI oldFirst = totalUnits[0];

        cards.Remove(card);
        cards.Remove(oldFirst);
        cards.Insert(0, card);
        cards.Insert(totalCards - 1, oldFirst);
    }

    public bool IsBuffedFor(NationsEnum nation)
    {
        bool found = false;
        foreach(CardUI card in cards)
        {
            if (card.GetOwner() != nation)
                continue;
            if (card as HazardCreatureCardUI != null)
            {
                HazardCreatureCardUI hazardUIcard = card as HazardCreatureCardUI;
                if(hazardUIcard.GetHazardCreatureDetails() != null)
                {
                    HazardCreatureCardDetails details = hazardUIcard.GetHazardCreatureDetails();
                    if (details.GetAbilities().Contains(HazardAbilitiesEnum.Buffs))
                    {
                        found = true;
                        break;
                    }
                }
            }
        }
        return found;
    }

    public bool IsDeBuffedFor(NationsEnum nation)
    {
        bool found = false;
        foreach (CardUI card in cards)
        {
            if (card.GetOwner() != nation)
                continue;
            if (card as HazardCreatureCardUI != null)
            {
                HazardCreatureCardUI hazardUIcard = card as HazardCreatureCardUI;
                if (hazardUIcard.GetHazardCreatureDetails() != null)
                {
                    HazardCreatureCardDetails details = hazardUIcard.GetHazardCreatureDetails();
                    if (details.GetAbilities().Contains(HazardAbilitiesEnum.Debuffs))
                    {
                        found = true;
                        break;
                    }
                }
            }
        }
        return found;
    }
}
