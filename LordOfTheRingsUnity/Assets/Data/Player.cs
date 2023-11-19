using Newtonsoft.Json.Bson;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player
{
    private readonly NationsEnum nation;
    private readonly AlignmentsEnum aligment;
    private readonly HashSet<Vector2Int> citySeesTiles = new ();
    private readonly HashSet<Vector2Int> cardSeesTiles = new ();
    public bool isHuman = false;
    public Player(NationsEnum nation, bool isHuman)
    {
        this.nation = nation;
        aligment = Nations.alignments[nation];
        this.isHuman = isHuman;
    }

    public NationsEnum GetNation()
    {
        return nation;
    }

    public AlignmentsEnum GetAlignment()
    {
        return aligment;
    }

    public void SetCitySeesTile(Vector2Int tile)
    {
        // Always visible
        citySeesTiles.Add(tile);
    }
    public void UnsetCitySeesTile(Vector2Int tile)
    {
        if(citySeesTiles.Contains(tile))
            citySeesTiles.Remove(tile);
    }
    public void SetCardSeesTile(Vector2Int tile)
    {
        //Visible as long as a card is in it
        cardSeesTiles.Add(tile);
    }

    public void UnsetCardSeesTile(Vector2Int tile)
    {
        if(cardSeesTiles.Contains(tile))
            cardSeesTiles.Remove(tile);
    }
    public bool CitySeesTile(Vector2Int tile)
    {
        return citySeesTiles.Contains(tile);
    }

    public bool SeesTile(Vector2Int tile)
    {
        return citySeesTiles.Contains(tile) || cardSeesTiles.Contains(tile);
    }
}