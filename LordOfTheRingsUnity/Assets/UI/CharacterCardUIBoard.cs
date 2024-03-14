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
    private CanvasGroup nextCanvasGroup;

    private AnimationActivationCondition activationCondition;
    private Image animationImage;
    //private ParticlesActivationCondition activationCondition;

    [Header("Initialization")]
    [SerializeField]
    private Vector2Int hex;
    [SerializeField]
    private bool refresh;

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

    public bool Initialize()
    {
        return Initialize(hex, cardId, owner, 0);
    }

    public bool Initialize(Vector2Int hex, string cardId, NationsEnum owner, short moved = 0)
    {
        animationImage.color = colorManager.GetNationColor(owner);
        this.hex = hex;
        if (string.IsNullOrEmpty(cardId))
            cardId = gameObject.name;
        
        if (!base.Initialize(cardId, owner, refresh))
            return false;

        initialized = false;
        isMoving = false;
        isVisible = false;
        isSelected = false;
        
        hurtIcon.enabled = false;
        exhaustedIcon.enabled = false;

        this.moved = moved;

        boardTile = board.AddCard(hex, this);

        CharacterCardDetails charDetails = details as CharacterCardDetails;
        if (charDetails != null)
            resourcesManager.SubtractInfluence(owner, charDetails.GetMind());
        
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

    public bool IsVisibleToHumanPlayer()
    {
        if (!game.GetHumanPlayer().SeesTile(hex))
            return false;

        if (Nations.alignments[game.GetHumanNation()] == Nations.alignments[owner])
            return true;
        else
            return false;
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
            Initialize();
            if (initialized)
                refresh = false;
            
            return;
        }
        if (!board.IsAllLoaded())
            return;

        boardTile = board.GetTile(hex);
        
        if (boardTile == null)
            return;

        if (selectedItems != null && selectedItems.GetSelectedMovableCard() == details)
            button.Select();

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

    public void Show()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        isVisible = true;
    }
    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        isVisible = false;
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
        isVisible &= string.IsNullOrEmpty(inCompanyOf);
        isVisible &= !t.IsHiddenByOtherCharacter(this);
        isVisible &= (!movementManager.renderingPath || selectedItems.GetSelectedMovableCardUI() == this);
            
        bool visibleAfter = isVisible;

        if(visibleBefore != visibleAfter)
        {
            if (visibleAfter)
                Show();
            else 
                Hide();
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
