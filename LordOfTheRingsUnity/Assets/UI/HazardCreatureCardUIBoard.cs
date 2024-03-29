using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HazardCreatureCardUIBoard : HazardCreatureCardUI, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hazard Creature Card UI Board")]
    [SerializeField]
    private Button button;
    
    //private ParticlesActivationCondition activationCondition;
    private AnimationActivationCondition activationCondition;
    private Image animationImage;

    [SerializeField]
    private CanvasGroup nextCanvasGroup;

    [Header("Initialization")]
    [SerializeField]
    protected Vector2Int hex;

    private BoardTile boardTile;
    private MovementManager movementManager;
    private bool isSelected;
    private short moved;
    private bool isMoving;
    private bool isVisible;
    private Vector3 currentPosition;
    void Awake()
    {
        activationCondition = GetComponentInChildren<AnimationActivationCondition>();
        animationImage = activationCondition.gameObject.GetComponent<Image>();
        colorManager = GameObject.Find("ColorManager").GetComponent<ColorManager>();
        movementManager = GameObject.Find("MovementManager").GetComponent<MovementManager>();
        //activationCondition = GetComponentInChildren<ParticlesActivationCondition>();
    }


    public bool Initialize(Vector2Int hex, string cardId, NationsEnum owner, short moved = 0)
    {
        animationImage.color = colorManager.GetNationColor(owner);
        this.hex = hex;

        if (!base.Initialize(cardId, owner))
            return false;

        initialized = false;
        isMoving = false;
        isVisible = false;
        isSelected = false;

        hurtIcon.enabled = false;
        exhaustedIcon.enabled = false;

        this.moved = moved;

        boardTile = board.AddCard(hex, this);

        Debug.Log(GameObject.Find("Localization").GetComponent<Localization>().Localize(details.cardId) + " registered itself in Board at " + HexTranslator.GetDebugTileInfo(hex) + " " + HexTranslator.GetNormalizedCellPosString(hex));

        activationCondition.Initialize(() => selectedItems != null && selectedItems.GetSelectedMovableCard() == details);

        button.interactable = owner == turn.GetCurrentPlayer();

        PlaceOnBoard();
        CheckStatusEffects();

        initialized = true;
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

            if (hex.x != int.MinValue && hex.y != int.MinValue)
                Initialize(hex, cardId, owner, moved);
            else
                Debug.LogError("Trying to initialize a board card with unset hex");

            return;
        }

        boardTile = board.GetTile(hex);

        if (boardTile == null)
            return;

        if (selectedItems != null && selectedItems.GetSelectedMovableCard() == details)
            button.Select();

        if (Input.GetKeyUp(KeyCode.Escape))
            isSelected = false;

        if (isSelected && selectedItems.GetSelectedCardDetails()!=null && selectedItems.GetSelectedCardDetails().cardId != details.cardId)
            isSelected = false;

        if (isMoving)
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
        if (!IsVisibleToHumanPlayer())
        {
            Hide();
            return;
        }
            

        bool visibleBefore = isVisible;
        BoardTile t = board.GetTile(hex);
        if (t == null)
        {
            Hide();
            return;
        }
            

        isVisible = game.GetHumanPlayer().SeesTile(hex);
        isVisible &= !t.IsHiddenByOtherCharacter(this);
        isVisible &= (!movementManager.renderingPath || selectedItems.GetSelectedMovableCardUI() == this);

        bool visibleAfter = isVisible;

        if (visibleBefore != visibleAfter)
        {
            if (visibleAfter)
                Show();
            else
                Hide();
        }
    }

    public void Show()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public bool IsVisibleToHumanPlayer()
    {
        if (!game.GetHumanPlayer().SeesTile(hex))
            return false;

        if (Nations.alignments[game.GetHumanNation()] == Nations.alignments[owner])
            return true;
        else
            return false;
    }

    public bool CanJoin()
    {
        if (boardTile == null)
            return false;
        return
            selectedItems.IsMovableSelected() &&
            selectedItems.GetSelectedCardDetails() == this &&
            IsAvatar() &&
            boardTile.GetTotalUnitsAtHex() > 0 &&
            !inputPopupManager.IsShown() &&
            isSelected &&
            isVisible;
    }
    public CardUI GetMergeCandidate()
    {
        return boardTile.GetCardsUI().FindAll(x => x != null).
            DefaultIfEmpty(null).
            FirstOrDefault(x => x.GetCharacterDetails().GetInfluence() >= GetCharacterDetails().GetMind() &&
            x != this);
    }

    public void Toggle()
    {
        if (turn.GetCurrentPlayer() != owner)
        {
            isSelected = false;
            return;
        }

        board.SelectHex(hex);
        selectedItems.SelectCardDetails(details, owner);

        /*isSelected = !isSelected;
        if (isSelected)
        {
            board.SelectHex(hex);
            selectedItems.SelectCardDetails(details, owner);
        }
        else if (selectedItems.GetSelectedCardDetails() != null && selectedItems.GetSelectedCardDetails() == details)
        {
            selectedItems.UnselectCardDetails();
            board.SelectHex(Board.NULL);
        }*/
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
        base.OnPointerEnter(eventData);

        mouse.RemoveCursor();
    }
    public BoardTile GetBoardTile()
    {
        return boardTile;
    }

}
