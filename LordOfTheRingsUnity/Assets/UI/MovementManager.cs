using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Aoiti.Pathfinding;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
using System.Dynamic;

public class MovementManager : MonoBehaviour
{
    [Header("Initialization")]
    public LineRenderer lineRenderer;
    public Tilemap cardTilemap;
    public Tilemap movementTilemap;
    public Image[] rightMovementSprites;
    public TextMeshProUGUI[] rightMovementCosts;
    public CompanyManager companyManagerLayout;
    public float speed = 1f;


    public static Vector3Int[] directionsEvenY = new Vector3Int[6] {Vector3Int.left, Vector3Int.left + Vector3Int.up, Vector3Int.up,
                                                 Vector3Int.right,Vector3Int.down, Vector3Int.left + Vector3Int.down};

    public static Vector3Int[] directionsUnevenY = new Vector3Int[6] {Vector3Int.left, Vector3Int.up, Vector3Int.right + Vector3Int.up,
                                                 Vector3Int.right,Vector3Int.right + Vector3Int.down, Vector3Int.down};

    public static short LEFT = 0;
    public static short UP_LEFT = 1;
    public static short UP_RIGHT = 2;
    public static short RIGHT = 3;
    public static short DOWN_RIGHT = 4;
    public static short DOWN_LEFT = 5;

    // **** REFERENCES TO STATIC OBJECTS *****
    private CellHover cellHover;
    private ManaManager manaManager;
    private TilemapSelector tilemapSelector;
    private SelectedItems selectedItems;
    private FOWManager fow;
    private TerrainManager terrainManager;
    private CameraController cameraController;
    private EventsManager eventsManager;
    private Turn turn;
    private Mouse mouse;
    private Board board;
    private DeckManager deckManager;
    private CombatPopupManager combatPopupManager;
    private Game game;
    // ****************************************

    private List<Vector3> positions = new ();
    private short moved = 0;
    private int movement= 0;

    private Pathfinder<Vector3Int> pathfinder;

    public static Vector3Int NULL = Vector3Int.one * int.MinValue;
    public static Vector2Int NULL2 = Vector2Int.one * int.MinValue;

    private Vector3Int showingPathDestination = NULL;

    
    private List<Vector3Int> path;

    private CardUI lastSelected = null;
    private Vector2Int lastHex = NULL2;
    
    [Range(0.001f,1f)]
    public float stepTime;


    void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        manaManager = GameObject.Find("ManaManager").GetComponent<ManaManager>();
        tilemapSelector = GameObject.Find("TilemapSelector").GetComponent<TilemapSelector>();
        cellHover = tilemapSelector.GetComponent<CellHover>();
        selectedItems = GameObject.Find("SelectedItems").GetComponent<SelectedItems>();
        fow = GameObject.Find("FOWManager").GetComponent<FOWManager>();
        terrainManager = GameObject.Find("TerrainManager").GetComponent<TerrainManager>();
        eventsManager = GameObject.Find("EventsManager").GetComponent<EventsManager>();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        turn = GameObject.Find("Turn").GetComponent<Turn>();
        mouse = GameObject.Find("Mouse").GetComponent<Mouse>();
        board = GameObject.Find("Board").GetComponent<Board>();
        combatPopupManager = GameObject.Find("CombatPopupManager").GetComponent<CombatPopupManager>();
        game = GameObject.Find("Game").GetComponent<Game>();
        companyManagerLayout.Hide();
    }

    public float DistanceFunc(Vector3Int a, Vector3Int b)
    {
        return (a-b).sqrMagnitude;
    }

    public Dictionary<Vector3Int,float> ConnectionsAndCosts(Vector3Int a)
    {
        Dictionary<Vector3Int, float> result= new ();
        bool even = (a.y % 2 == 0);
        Vector3Int[] directions = even ? directionsEvenY : directionsUnevenY;

        foreach (Vector3Int dir in directions)
        {
            Vector3Int destination = dir + a;
            destination = new Vector3Int(destination.x, destination.y, 0);
            int destinationPos = HexTranslator.GetNormalizedCellPosInt(destination);
            TileAndMovementCost tmc = terrainManager.GetTileAndMovementCost(destinationPos);
            if (tmc.movable)
                result.Add(destination, tmc.movementCost);
        }
        return result;
    }

    // Start is called before the first frame update
    void Start()
    {
        pathfinder = new Pathfinder<Vector3Int>(DistanceFunc, ConnectionsAndCosts);
    }

    // Update is called once per frame
    void Update()
    {
        if (!game.FinishedLoading())
            return;
        if (game.IsPopup()   || 
            tilemapSelector.IsOverUI()     || 
            Input.GetKeyUp(KeyCode.Escape) ||
            (Input.GetMouseButton(0) && (Input.GetMouseButton(1) || Input.GetMouseButton(2))))
        {
            Reset();
            return;
        }

        if (Input.mousePosition.x < 0 || Input.mousePosition.y < 0 || Input.mousePosition.x > Screen.width || Input.mousePosition.y > Screen.height)
        {
            StopAllCoroutines();
            Reset();
            cameraController.RemovePreventDrag();
        }

        bool movableCard = selectedItems.IsMovableSelected() || selectedItems.IsHazardCreatureSelected();

        if (movableCard)
        {
            CardUI cardUI = selectedItems.GetSelectedMovableCardUI();
            if (cardUI == null)
            {
                Reset();
                return;
            }

            CardDetails cardDetails = cardUI.GetDetails();

            if (cardDetails == null)
            {
                Reset();
                return;
            }

            bool isImmovable = false;
            Vector2Int hex = new(int.MinValue, int.MinValue);

            if (cardDetails.cardClass == CardClass.Character)
            {
                CharacterCardUIBoard character = cardUI as CharacterCardUIBoard;
                if (character == null)
                {
                    Reset();
                    return;
                }
                if (cardDetails as CharacterCardDetails != null)
                {
                    CharacterCardDetails charDetails = cardDetails as CharacterCardDetails;
                    isImmovable = charDetails.isImmovable;
                }
                hex = character.GetHex();
            }
            else if (cardDetails.cardClass == CardClass.HazardCreature)
            {
                HazardCreatureCardUIBoard creature = cardUI as HazardCreatureCardUIBoard;
                if (creature == null)
                {
                    Reset();
                    return;
                }
                hex = creature.GetHex();
            }
            cameraController.RemovePreventDrag();
            if (isImmovable)
            {
                if (!eventsManager.IsEventInPlay(EventAbilities.CanMove, turn.GetCurrentPlayer()))
                {
                    if (Input.GetMouseButton(1))
                    {
                        mouse.ChangeCursor("immovable");
                        cameraController.PreventDrag();

                        return;
                    }
                    else
                    {
                        cameraController.RemovePreventDrag();
                        mouse.RemoveCursor();
                        Reset();
                        return;
                    }
                }
            }

            if (hex.x == int.MinValue || hex.y == int.MinValue)
            {
                Reset();
                return;
            }

            //mouse.RemoveCursor();

            Vector2Int hexSelected = hex;
            Vector3Int currentCellPos = new(hexSelected.x, hexSelected.y, 0);
            Vector3Int target = cellHover.last;


            if (cardUI != lastSelected)
            {
                lastSelected = cardUI;
                companyManagerLayout.Initialize(cardUI.GetOwner());
            }

            if (Input.GetMouseButton(1))
            {
                if (target != currentCellPos)
                {
                    cameraController.PreventDrag();

                    pathfinder.GenerateAstarPath(currentCellPos, target, out path);
                    path.Insert(0, currentCellPos);
                    if (showingPathDestination != path[^1])
                    {
                        Reset();
                        StopAllCoroutines();
                        StartCoroutine(RenderPath());
                        showingPathDestination = path[^1];
                    }
                }
                else
                {
                    Reset();
                    StopAllCoroutines();
                    StartCoroutine(RenderPath());
                    showingPathDestination = path[^1];
                    return;
                }
                    
            }
            else if (Input.GetMouseButtonUp(1))
            {   
                StopAllCoroutines();
                StartCoroutine(MovePath());
                Reset();
            }
        } 
        else
        {
            companyManagerLayout.Hide();
            lastSelected = null;
        }

    }

    public void Reset()
    {
        positions = new List<Vector3>();
        movementTilemap.ClearAllTiles();
        for(int i=0;i<rightMovementSprites.Length;i++)
        {
            rightMovementSprites[i].enabled = false;
            rightMovementCosts[i].enabled = false;
        }            
        positions.Clear();
        moved = 0;
        movement = MovementConstants.unitsMovement;
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
        mouse.RemoveCursor("immovable");
        mouse.RemoveCursor("movement");
    }

    public void Move(List<Vector3Int> path)
    {
        this.path = path;
        StartCoroutine(MovePath());
    }

    IEnumerator MovePath()
    {
        cameraController.PreventDrag();
        CardUI selectedCardUIForMovement = selectedItems.GetSelectedMovableCardUI();
        if(selectedCardUIForMovement == null)
        {
            Reset();
            yield return null;
        }

        if (selectedCardUIForMovement != null)
        {
            int moved = -1;
            int movement = MovementConstants.unitsMovement;

            if (selectedCardUIForMovement.GetCardClass() == CardClass.Character)
            {
                moved = (selectedCardUIForMovement as CharacterCardUIBoard).GetMoved();
                movement = (selectedCardUIForMovement as CharacterCardUIBoard).GetTotalMovement();
            }   
            else if (selectedCardUIForMovement.GetCardClass() == CardClass.HazardCreature)
            {
                moved = (selectedCardUIForMovement as HazardCreatureCardUIBoard).GetMoved();
                movement = (selectedCardUIForMovement as HazardCreatureCardUIBoard).GetTotalMovement();
            }                
            
            if (moved == -1 || moved >= movement)
            {
                Reset();
                yield return null;
            }

            if (selectedCardUIForMovement.GetCardClass() == CardClass.Character)
                (selectedCardUIForMovement as CharacterCardUIBoard).Moving();
            else if (selectedCardUIForMovement.GetCardClass() == CardClass.HazardCreature)
                (selectedCardUIForMovement as HazardCreatureCardUIBoard).Moving();

            int currentPointIndex = 0;

            //int maxPath = positions.Count;
            int maxPath = path.Count;

            if (maxPath < 2)
                yield return null;

            List<CardTypesEnum> accumulatedMana = new();

            Vector3 initialPosition = cardTilemap.CellToWorld(path[currentPointIndex]);
            initialPosition = new Vector3(initialPosition.x, initialPosition.y, 0);
            Vector3Int initialCell = cardTilemap.WorldToCell(initialPosition);

            if (moved == 0)
            {
                CardInfo initialCardInfo = terrainManager.GetCardInfo(cardTilemap.GetTile(initialCell) as Tile);
                if (initialCardInfo != null)
                    accumulatedMana.Add(initialCardInfo.cardType);
                else
                {
                    Reset();
                    yield return null;
                }                    
            }
            while (currentPointIndex + 1 < maxPath)
            {
                Vector3 startPosition = cardTilemap.CellToWorld(path[currentPointIndex]);
                startPosition = new Vector3(startPosition.x, startPosition.y, 0);
                Vector3 targetPosition = cardTilemap.CellToWorld(path[(currentPointIndex + 1)]);
                targetPosition = new Vector3(targetPosition.x, targetPosition.y, 0);
                
                float currentLerpTime = 0f;

                Vector3Int startCell = cardTilemap.WorldToCell(startPosition);
                Vector3Int targetCell = cardTilemap.WorldToCell(targetPosition);

                if(
                    selectedCardUIForMovement.GetCardClass() == CardClass.HazardCreature &&
                    board.GetTile(new Vector2Int(targetCell.x, targetCell.y)).GetCity() != null &&
                    Nations.alignments[selectedCardUIForMovement.GetOwner()] != Nations.alignments[board.GetTile(new Vector2Int(targetCell.x, targetCell.y)).GetCity().GetOwner()]
                  )
                {
                    CityUI city = board.GetTile(new Vector2Int(targetCell.x, targetCell.y)).GetCity();
                    combatPopupManager.Initialize(selectedCardUIForMovement, city);

                    Reset();
                    break;
                }

                short movementCost = terrainManager.GetMovementCost(targetCell);
                movementCost = CheckMountedOrBoarded(movementCost, startCell, selectedCardUIForMovement);

                if (moved + movementCost > MovementConstants.unitsMovement)
                    break;

                lastHex = new Vector2Int(targetCell.x, targetCell.y);

                if (selectedCardUIForMovement.GetCardClass() == CardClass.Character)
                {
                    if ((selectedCardUIForMovement as CharacterCardUIBoard).GetTotalMovement() < moved + movementCost)
                        break;
                    (selectedCardUIForMovement as CharacterCardUIBoard).AddMovement(movementCost);
                    moved = (selectedCardUIForMovement as CharacterCardUIBoard).GetMoved();
                }
                else if (selectedCardUIForMovement.GetCardClass() == CardClass.HazardCreature)
                {
                    if ((selectedCardUIForMovement as HazardCreatureCardUIBoard).GetTotalMovement() < moved + movementCost)
                        break;
                    (selectedCardUIForMovement as HazardCreatureCardUIBoard).AddMovement(movementCost);
                    moved = (selectedCardUIForMovement as HazardCreatureCardUIBoard).GetMoved();
                }
                

                while (currentLerpTime < 1f)
                {
                    currentLerpTime += Time.deltaTime * speed;
                    Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, currentLerpTime);
                    selectedCardUIForMovement.gameObject.transform.position = currentPosition;
                    yield return null;
                }

                currentPointIndex++;
                if (selectedCardUIForMovement.GetCardClass() == CardClass.Character)
                    (selectedCardUIForMovement as CharacterCardUIBoard).AddToHex(new Vector2Int(targetCell.x, targetCell.y));
                else if (selectedCardUIForMovement.GetCardClass() == CardClass.HazardCreature)
                    (selectedCardUIForMovement as HazardCreatureCardUIBoard).AddToHex(new Vector2Int(targetCell.x, targetCell.y));

                fow.UpdateCardFOW(targetCell, startCell);

                CardInfo ci = terrainManager.GetCardInfo(cardTilemap.GetTile(targetCell) as Tile);
                if (ci != null)
                    accumulatedMana.Add(ci.cardType);
                else
                {
                    Reset();
                    yield return null;
                }
                    
            }
            // UPDATE MANA FOR PLAYER
            if (selectedCardUIForMovement.GetCardClass() == CardClass.Character)
                manaManager.AddMana(turn.GetCurrentPlayer(), accumulatedMana);
            
            // STOP MOVING
            if (selectedCardUIForMovement.GetCardClass() == CardClass.Character)
                (selectedCardUIForMovement as CharacterCardUIBoard).StopMoving();
            else if (selectedCardUIForMovement.GetCardClass() == CardClass.HazardCreature)
                (selectedCardUIForMovement as HazardCreatureCardUIBoard).StopMoving();

            //REFRESH STATUS EFFECTS (BUFF AND DEBUFF)
            foreach(CardUI card in board.GetTile(initialCell).GetCardsUI())
            {
                if ((card as CharacterCardUI) != null)
                    (card as CharacterCardUI).CheckStatusEffects();
            }

            selectedItems.SelectCardDetails(
                selectedCardUIForMovement.GetDetails(),
                selectedCardUIForMovement.GetOwner());

            // IF CHARACTER, ENEMIES
            if (selectedCardUIForMovement.GetCardClass() == CardClass.Character)
                CheckEnemies(accumulatedMana, selectedCardUIForMovement);

            deckManager.Dirty(DirtyReasonEnum.CHAR_SELECTED);

            cameraController.RemovePreventDrag();
            cameraController.LookToCard(selectedCardUIForMovement);

            selectedCardUIForMovement.AddMessage(string.Format("{0}/{1} moved", moved, movement), 1f, Color.white);

            Reset();
        }
        else
            Reset();
    }

    IEnumerator RenderPath()
    {
        if (Input.mousePosition.x < 0 || Input.mousePosition.y < 0 || Input.mousePosition.x > Screen.width || Input.mousePosition.y > Screen.height)
        {
            StopAllCoroutines();
            Reset();
            yield return null;
        }

        if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
        {
            Reset();
            StopAllCoroutines();
            yield return null;
        }

        cameraController.PreventDrag();
        CardUI selectedCardUIForMovement = selectedItems.GetSelectedMovableCardUI();
        if (selectedCardUIForMovement == null)
        {
            Reset();
            StopAllCoroutines();
            yield return null;
        }

        if (selectedCardUIForMovement.GetCardClass() == CardClass.Character)
        {
            moved = (selectedCardUIForMovement as CharacterCardUIBoard).GetMoved();
            movement = (selectedCardUIForMovement as CharacterCardUIBoard).GetTotalMovement();
        }
        else if (selectedCardUIForMovement.GetCardClass() == CardClass.HazardCreature)
        {
            moved = (selectedCardUIForMovement as HazardCreatureCardUIBoard).GetMoved();
            movement = (selectedCardUIForMovement as HazardCreatureCardUIBoard).GetTotalMovement();
        }
        else
        {
            Reset();
            StopAllCoroutines();
            yield return null;
        }            

        for (int p=0;p<path.Count;p++)
        {
            //Movement Tilemap
            Vector3Int cardTilePos = path[p];

            Vector3 cardCellCenter = cardTilemap.CellToWorld(cardTilePos);
            cardCellCenter = new Vector3(cardCellCenter.x, cardCellCenter.y, -1);
            positions.Add(cardCellCenter);
                        
            short movementCost = (p != 0) ? terrainManager.GetMovementCost(cardTilePos) : (short)0;
            movementCost = CheckMountedOrBoarded(movementCost, cardTilePos, selectedCardUIForMovement);

            if (moved + movementCost > movement)
            {
                mouse.ChangeCursor("immovable");
                StopAllCoroutines();
                positions = positions.GetRange(0, p);
                path = path.GetRange(0, p);
                lineRenderer.positionCount = positions.Count;
                lineRenderer.SetPositions(positions.ToArray());
                break;
            }
            else
            {
                mouse.ChangeCursor("movement");
                //mouse.RemoveCursor();
            }
                

            moved += movementCost;
            rightMovementCosts[positions.Count - 1].text = moved.ToString();
            rightMovementCosts[positions.Count - 1].enabled = true;

            ShowPOI(cardTilePos);
            
            Sprite spriteMovement = terrainManager.GetSpriteMovement(cardTilePos);

            if (spriteMovement == null)
            {
                Reset();
                StopAllCoroutines();
                break;
            }
            if (rightMovementSprites.Length < positions.Count)
            {
                Reset();
                StopAllCoroutines();
                break;
            }

            lineRenderer.positionCount = positions.Count;
            lineRenderer.SetPositions(positions.ToArray());

            rightMovementSprites[positions.Count - 1].type = Image.Type.Simple;
            rightMovementSprites[positions.Count - 1].preserveAspect = true;
            rightMovementSprites[positions.Count - 1].sprite = spriteMovement;
            rightMovementSprites[positions.Count - 1].enabled = true;
            
        }
        yield return new WaitForSeconds(stepTime);
    }

    public bool ShowPOI(Vector3Int cardTilePos)
    {
        bool poi = false;
        BoardTile boardTile = board.GetTile(cardTilePos);
        
        // CASE 1: Point of Interest if foreign City
        if (boardTile.HasCity() && Nations.alignments[boardTile.GetCity().GetOwner()] != game.GetHumanPlayer().GetAlignment())
        {
            MapTooltipEnum mapTooltip = selectedItems.IsHazardCreatureSelected() ? MapTooltipEnum.CITY_ATTACK : MapTooltipEnum.CITY_ENTER;
            tilemapSelector.ShowTooltipAt(cardTilePos, mapTooltip);
            poi = true;
        }
        // OTHER CASES SOON

        if (!poi)
            tilemapSelector.HideTooltip();
        return poi;
    }


    public short CheckMountedOrBoarded(short movementCost, Vector3Int cardTilePos, CardUI selectedCardUIForMovement)
    {
        short newCost = movementCost;
        if(selectedCardUIForMovement != null)
        {
            bool isMounted = false;
            bool isBoarded = false;
            if (selectedCardUIForMovement.GetCardClass() == CardClass.Character)
            {
                CharacterCardDetails details = selectedCardUIForMovement.GetCharacterDetails();
                if(details != null)
                {
                    isMounted = details.abilities.Contains(CharacterAbilitiesEnum.Mounted);
                    isBoarded = details.abilities.Contains(CharacterAbilitiesEnum.Boarded);
                }
            }
            else if (selectedCardUIForMovement.GetCardClass() == CardClass.HazardCreature)
            {
                HazardCreatureCardDetails details = selectedCardUIForMovement.GetHazardCreatureDetails();
                if (details != null)
                {
                    isMounted = details.hazardAbilities.Contains(HazardAbilitiesEnum.Mounted);
                    isBoarded = details.hazardAbilities.Contains(HazardAbilitiesEnum.Boarded);
                }
            }

            TileAndMovementCost info = terrainManager.GetTileAndMovementCost(cardTilePos);
            switch (info.terrain.terrainType)
            {
                case TerrainsEnum.SEA:
                case TerrainsEnum.COAST:
                    if (isBoarded)
                        newCost--;
                    break;
                case TerrainsEnum.WASTE:
                case TerrainsEnum.PLAINS:
                case TerrainsEnum.GRASS:
                case TerrainsEnum.ASHES:
                case TerrainsEnum.ICE:
                case TerrainsEnum.DESERT:
                case TerrainsEnum.FOREST:
                    if (isMounted)
                        newCost--;
                    break;
            }
        }

        return Math.Min(MovementConstants.unitsMovement, Math.Max((short)1, newCost));
    }
    public Vector2Int GetLastHex()
    {
        return lastHex;
    }

    public void CheckEnemies(List<CardTypesEnum> accumulatedMana, CardUI selectedCardUIForMovement)
    {
        Dictionary<NationsEnum, float> distances = board.GetCityManager().GetEnemyNeighbourCities(lastHex, turn.GetCurrentPlayer());

        List<Tuple<string, NationsEnum>> toCombat = new();

        List<float> normDistancesToProbas = distances.Values.ToList();
        normDistancesToProbas = normDistancesToProbas.Select(distance => 1 - (distance / normDistancesToProbas.Sum())).ToList();
        normDistancesToProbas.ForEach(x => x *= game.GetMultiplierByDifficulty());
        for (int i = 0; i < normDistancesToProbas.Count; i++)
        {
            List<CardTypesEnum> manaForOpponent = new();
            foreach (CardTypesEnum cardType in accumulatedMana)
            {
                if (Mathf.RoundToInt(normDistancesToProbas[i]) == 1)
                    manaForOpponent.Add(cardType);
            }
            NationsEnum owner = distances.Keys.ToList()[i];
            manaManager.AddMana(owner, manaForOpponent);

            List<CardDetails> creaturesInHand = deckManager.GetCardsInHandOfType(CardClass.HazardCreature, owner);
            creaturesInHand.Shuffle();
            foreach (CardDetails cardDetails in creaturesInHand)
            {
                HazardCreatureCardDetails hazardCard = cardDetails as HazardCreatureCardDetails;
                if (hazardCard == null)
                    continue;
                if (manaManager.HasEnoughMana(owner, hazardCard.GetCardTypes()))
                {
                    toCombat.Add(new Tuple<string, NationsEnum>(cardDetails.cardId, owner));
                    manaManager.RemoveMana(owner, hazardCard.GetCardTypes());
                }
            }
        }
        if (toCombat.Count > 0)
        {
            TileAndMovementCost tileInfo = terrainManager.GetTileAndMovementCost(lastHex);
            bool res = combatPopupManager.Initialize(
                selectedCardUIForMovement,
                toCombat,
                tileInfo.terrain.terrainType.ToString().ToLower()
                );
            if (res)
                combatPopupManager.ShowPopup();
        }
    }
}

