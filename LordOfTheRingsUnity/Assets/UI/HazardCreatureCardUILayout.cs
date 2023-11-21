using UnityEngine;

public class HazardCreatureCardUILayout : HazardCreatureCardUI
{
    [Header("Hazard Character Card UI Layout")]
    [SerializeField]
    private CanvasGroup nextCanvasGroup;

    public override bool Initialize(string cardId, NationsEnum owner)
    {
        return base.Initialize(cardId, owner);
    }

    void Update()
    {
        if (!initialized) 
            return;

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
                selectedItems.SelectCardDetails(nextOriginal.GetDetails(), owner);
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
