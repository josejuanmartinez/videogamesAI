using System.Collections.Generic;
using System.Linq;

public class CharacterManager
{
    readonly Board board;
    public CharacterManager(Board board)
    {
        this.board = board;
    }

    public List<CardUI> GetCharactersOfPlayer(NationsEnum owner)
    {
        if (!board.IsAllLoaded())
            return new List<CardUI>();
        return board.GetTiles().Values.SelectMany(x => x.GetCardsUI()).Where(y => y.GetCardClass() == CardClass.Character && y.GetOwner() == owner).ToList();
    }

    public List<CardUI> GetCharactersOfPlayerNonAvatar(NationsEnum owner)
    {
        if (!board.IsAllLoaded())
            return new List<CardUI>();
        return board.GetTiles().Values.SelectMany(x => x.GetCardsUI()).Where(y => y.GetCardClass() == CardClass.Character && y.GetOwner() == owner && !y.IsAvatar()).ToList();
    }

    public List<CityUI> GetCitiesWithCharactersOfPlayer(NationsEnum owner)
    {
        if (!board.IsAllLoaded())
            return new List<CityUI>();
        List<CardUI> character = GetCharactersOfPlayer(owner);
        List<CharacterCardUIBoard> characterUiBoard = character.FindAll(x => (x as CharacterCardUIBoard) != null).Select(x => (x as CharacterCardUIBoard)).ToList();
        return characterUiBoard.Select(x => x.GetHex()).Where(x => board.GetTile(x).HasCity()).Select(x => board.GetTile(x).GetCity()).ToList();
    }

    public List<string> GetCityStringsWithCharactersOfPlayer(NationsEnum owner)
    {
        if (!board.IsAllLoaded())
            return new List<string>();
        List<CardUI> character = GetCharactersOfPlayer(owner);
        List<CharacterCardUIBoard> characterUiBoard = character.FindAll(x => (x as CharacterCardUIBoard) != null).Select(x => (x as CharacterCardUIBoard)).ToList();
        return characterUiBoard.Select(x => x.GetHex()).Where(x => board.GetTile(x).HasCity()).Select(x => board.GetTile(x).GetCity().GetDetails().GetCityID()).ToList();
    }
    public CardUI GetAvatar(NationsEnum owner)
    {
        if (!board.IsAllLoaded())
            return null;
        List<CardUI> character = GetCharactersOfPlayer(owner);
        return character.FirstOrDefault(x => x.IsAvatar());
    }

    public List<CardDetails> GetCharactersInCompanyOf(CardDetails leader)
    {
        if (!board.IsAllLoaded())
            return new List<CardDetails>();
        if (leader != null)
            return board.GetTiles().Values.SelectMany(x => x.GetCardsUI()).ToList().
                FindAll(x => x.IsCharacterUI()).Select(x => (x as CharacterCardUI)).ToList().FindAll(y => y.GetInCompanyOf() == leader.cardId).Select(x => x.GetDetails()).ToList();
        else
            return new List<CardDetails>();
    }
    public List<CardDetails> GetCharactersInCompanyOf(CardUI leader)
    {
        if (!board.IsAllLoaded())
            return new List<CardDetails>();
        if (leader != null)
            return GetCharactersInCompanyOf(leader.GetDetails());
        else
            return null;
    }
    public List<CardDetails> GetMovingWithMe(CardDetails leader)
    {
        if (!board.IsAllLoaded())
            return new List<CardDetails>();
        if (leader == null)
            return new List<CardDetails>();

        List<CardDetails> res = new() { leader };
        if(leader.cardClass == CardClass.Character)
            res.AddRange(GetCharactersInCompanyOf(leader));
        return res;
    }

    public void RefreshMovement(NationsEnum nation)
    {
        if (nation == NationsEnum.ABANDONED)
            return;
        GetCharactersOfPlayer(nation).FindAll(x => (x as CharacterCardUIBoard) != null).Select(x => (x as CharacterCardUIBoard)).ToList().ForEach(x => x.SetMoved(0));
    }

    public CardUI GetCharacterOfPlayer(NationsEnum nation, string cardId)
    {
        return GetCharactersOfPlayer(nation).Find(x => x.GetCardID() == cardId);
    }
}
