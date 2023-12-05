using System.Collections.Generic;
using System.Linq;

public class CardManager
{    
    readonly Board board;
    public CardManager(Board board)
    {
        this.board = board;
    }

    public List<CardUI> GetCardsInPlayOfOwner(NationsEnum owner)
    {
        List<CardUI> cards = board.GetTiles().Values.SelectMany(x => x.GetCardsUI()).Where(y => y.GetOwner() == owner).ToList();
        cards.Sort((x, y) => x.GetCardId().CompareTo(y.GetCardId()));
        return cards;
    }

    public List<CardUI> GetCardsInPlayOfOwner(NationsEnum owner, CardClass cardClass)
    {
        List<CardUI> cards = board.GetTiles().Values.SelectMany(x => x.GetCardsUI()).Where(y => y.GetOwner() == owner && y.GetCardClass() == cardClass).ToList();
        cards.Sort((x, y) => x.GetCardId().CompareTo(y.GetCardId()));
        return cards;
    }

    public CardUI GetCardUI(CardDetails cardDetails)
    {
        return board.GetTiles().Values.ToList().SelectMany(x => x.GetCardsUI()).Where(y => y.GetCardId() == cardDetails.cardId).FirstOrDefault();
    }
    public CardUI GetCardUI(string cardId)
    {
        return board.GetTiles().Values.ToList().SelectMany(x => x.GetCardsUI()).Where(y => y.GetCardId() == cardId).FirstOrDefault();
    }

}
