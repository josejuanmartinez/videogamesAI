public class HoveredCard
{
    readonly NationsEnum nation;
    readonly string cardId;
    readonly CardClass cardClass;

    public HoveredCard() { }

    public HoveredCard(NationsEnum nation, string cardId, CardClass cardClass)
    {
        this.nation = nation;
        this.cardId = cardId;
        this.cardClass = cardClass;
    }

    public NationsEnum GetOwner()
    {
        return nation;
    }
    public string GetCardId()
    {
        return cardId;
    }
    public CardClass GetCardClass()
    {
        return cardClass;
    }
}