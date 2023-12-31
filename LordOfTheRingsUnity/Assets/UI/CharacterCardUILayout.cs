using UnityEngine;

public class CharacterCardUILayout : CharacterCardUI
{
    [Header("Character Card UI Layout")]
    [SerializeField]
    private CanvasGroup nextCanvasGroup;

    public override bool Initialize(string cardId, NationsEnum owner, bool refresh = false)
    {
        return base.Initialize(cardId, owner, refresh);
    }

    void Update()
    {
        if (!initialized) return;

        ShowNext();
    }

    public void Next()
    {
        CardUI next = board.GetNextCardUI(this);
        if (next != null)
        {
            CardUI nextOriginal = board.GetCardManager().GetCardUI(next.GetDetails());
            if(nextOriginal != null)
            {
                selectedItems.SelectCardDetails(nextOriginal.GetDetails(), nextOriginal.GetOwner());
                BoardTile boardTile = null;
                if (nextOriginal.GetCardClass() == CardClass.Character)
                    boardTile = ((CharacterCardUIBoard)nextOriginal).GetBoardTile();
                else if (nextOriginal.GetCardClass() == CardClass.HazardCreature)
                    boardTile = ((HazardCreatureCardUIBoard)nextOriginal).GetBoardTile();
                if(boardTile != null)
                    boardTile?.SetFirstAtHex(nextOriginal);
            }            
        }
    }

    public void HideNext()
    {
        nextCanvasGroup.alpha = 0;
        nextCanvasGroup.interactable = false;
        nextCanvasGroup.blocksRaycasts = false;
    }

    public void ShowNext()
    {
        nextCanvasGroup.alpha = 1;
        nextCanvasGroup.interactable = true;
        nextCanvasGroup.blocksRaycasts = true;
    }
}
