using System.Collections.Generic;
using System.Linq;

public class HazardCreaturesManager
{
    Board board;
    public HazardCreaturesManager(Board board)
    {
        this.board = board;
    }

    public List<CardUI> GetHazardCreaturesOfPlayer(NationsEnum owner)
    {
        return board.GetTiles().Values.SelectMany(x => x.GetCardsUI()).Where(y => y.GetCardClass() == CardClass.HazardCreature && y.GetOwner() == owner).ToList();
    }
    public void RefreshMovement(NationsEnum nation)
    {
        GetHazardCreaturesOfPlayer(nation).FindAll(x => x.IsHazardCreatureCardUIBoard()).Select(x => (HazardCreatureCardUIBoard)x).ToList().ForEach(x => x.SetMoved(0));
    }
}
