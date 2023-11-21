using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterCardUIBoard : CharacterCardUI, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Character Card UI Board")]
    [SerializeField]
    private Button button;
    [SerializeField]
    //private AnimationActivationCondition activationCondition;
    private ParticlesActivationCondition activationCondition;
    [SerializeField]
    private CanvasGroup nextCanvasGroup;

    [Header("Initialization")]
    [SerializeField]
    private Vector2Int hex;

    private BoardTile boardTile;
    private bool isSelected;
    private short moved;
    private bool isMoving;
    private bool isVisible;
    private Vector3 currentPosition;

    public bool Initialize(Vector2Int hex, string cardId, NationsEnum owner, short moved = 0)
    {
        this.hex = hex;

        if (!base.Initialize(cardId, owner))
            return false;

        initialized = false;

        isMoving = false;
        isVisible = true;
        isSelected = false;
        
        hurtIcon.enabled = false;
        exhaustedIcon.enabled = false;

        this.moved = moved;

        boardTile = board.AddCard(hex, this);

        CharacterCardDetails charDetails = (CharacterCardDetails)details;
        if (charDetails != null)
            resourcesManager.SubtractInfluence(owner, charDetails.GetMind());
        
        Debug.Log(GameObject.Find("Localization").GetComponent<Localization>().Localize(details.cardId) + " registered itself in Board at " + HexTranslator.GetDebugTileInfo(hex) + " " + HexTranslator.GetNormalizedCellPosString(hex));

        activationCondition.Initialize(() => selectedItems != null && selectedItems.GetSelectedMovableCard() == details);

        button.interactable = owner == turn.GetCurrentPlayer();

        PlaceOnBoard();
        CheckStatusEffects();

        initialized = true;

        return initialized;
    }
    public bool IsInitialized()
    {
        return initialized;
    } 
        
    public void PlaceOnBoard()
    {
        Vector3 cellWorldCenter = t.GetCellCenterWorld(new Vector3Int(hex.x, hex.y, 0));
        if (currentPosition != cellWorldCenter)
        {
            gameObject.transform.position = cellWorldCenter;
            currentPosition = cellWorldCenter;
        }
    }

    void Update()
    {
        if (!initialized)
        {
            if (string.IsNullOrEmpty(cardId))
                cardId = gameObject.name;

            if (hex.x != int.MinValue && hex.y != int.MinValue && owner != NationsEnum.ABANDONED)
                Initialize(hex, cardId, owner, 0);
            else
                Debug.LogError("Trying to initialize a board card with unset hex or nation");
            
            return;
        }

        boardTile = board.GetTile(hex);

        if (boardTile == null)
            return;

        if (Input.GetKeyUp(KeyCode.Escape))
            isSelected = false;

        if (isMoving)
            isSelected = false;

        if (isSelected && selectedItems.GetSelectedMovableCard() != null && selectedItems.GetSelectedMovableCard().cardId != details.cardId)
            isSelected = false;

        int totalUnitsAtHex = boardTile.GetTotalUnitsAtHex();
        if (totalUnitsAtHex > 1)
            ShowNext();
        else
            HideNext();
        
        RecalculateIsVisible();

        PlaceOnBoard();
    }

    public void RecalculateIsVisible()
    {
        bool visibleBefore = isVisible;
        BoardTile t = board.GetTile(hex);
        if (t == null)
            isVisible = false;

        isVisible = game.GetHumanPlayer().SeesTile(hex);
        isVisible &= string.IsNullOrEmpty(inCompanyOf);
        isVisible &= !t.IsHiddenByOtherCharacter(this);

        bool visibleAfter = isVisible;

        if(visibleBefore != visibleAfter)
        {
            canvasGroup.alpha = isVisible ? 1 : 0;
            canvasGroup.interactable = isVisible;
            canvasGroup.blocksRaycasts = isVisible;
        }
    }
    public bool CanJoin()
    {
        if (boardTile == null)
            return false;

        return
            selectedItems.IsMovableSelected() &&
            selectedItems.GetSelectedMovableCard() == this &&
            IsAvatar() &&
            boardTile.GetTotalUnitsAtHex() > 0 &&
            !inputPopupManager.IsShown() &&
            isSelected &&
            isVisible &&
            !IsInCompany();
    }

    public void Toggle()
    {
        if (turn.GetCurrentPlayer() != owner)
        {
            isSelected = false;
            return;
        }

        isSelected = true;
        board.SelectHex(hex);
        selectedItems.SelectCardDetails(details, owner);
    }

    public void Moving()
    {
        isMoving = true;
    }

    public void StopMoving()
    {
        isMoving = false;
    }
    public bool IsMoving()
    {
        return isMoving;
    }

    public void AddToHex(Vector2Int newHex)
    {
        board.GetTile(hex).RemoveCard(this);
        hex = newHex;
        board.GetTile(newHex).AddCard(this);
    }
    public void AddMovement(short movement)
    {
        moved += movement;
    }
    public void SetHex(Vector2Int hex)
    {
        this.hex = hex;
        List<CardDetails> company = board.GetCharacterManager().GetCharactersInCompanyOf(details);
        foreach (CardDetails cardDetails in company)
        {
            CardUI originalCard = board.GetCardManager().GetCardUI(cardDetails);
            if (originalCard != null)
            {
                CharacterCardUIBoard originalCharacter = (CharacterCardUIBoard)originalCard;
                if(originalCharacter != null)
                    originalCharacter.hex = hex;
            }
        }
    }

    public void Next()
    {
        CardUI next = boardTile.GetNextAtHex(this);
        if (next != null)
        {
            selectedItems.SelectCardDetails(next.GetDetails(), owner);
            boardTile.SetFirstAtHex(next);
        }
    }
    public Vector2Int GetHex()
    {
        return hex;
    }
    public short GetMoved()
    {
        return moved;
    }
    public void SetMoved(short moved)
    {
        this.moved = moved;
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
    public new void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (button.interactable)
            mouse.ChangeCursor("clickable");
        else
            mouse.ChangeCursor("unclickable");
    }

    public new void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        mouse.RemoveCursor();
    }

    public BoardTile GetBoardTile()
    {
        return boardTile;
    }
}
