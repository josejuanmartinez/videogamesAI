using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialDeck : MonoBehaviour
{
    public NationsEnum owner;
    public List<GameObject> cards;

    public CardDetails GetCardDetails(string cardId)
    {
        return cards.Find(x => x.GetComponent<CardDetails>() != null && x.GetComponent<CardDetails>().cardId == cardId).GetComponent<CardDetails>();
    }
    public GameObject GetCardPrefab(string cardId)
    {
        return cards.Find(x => x.GetComponent<CardDetails>() != null && x.GetComponent<CardDetails>().cardId == cardId);
    }
}
