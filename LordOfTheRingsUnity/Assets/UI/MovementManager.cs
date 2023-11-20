using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Aoiti.Pathfinding;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

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
    private short totalMovement = 0;

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
        if (cameraController.IsPopupOpen()   || 
            tilemapSelector.IsOverUI()     || 
            Input.GetKeyUp(KeyCode.Escape) ||
            (Input.GetMouseButton(0) && (Input.GetMouseButton(1) || Input.GetMouseButton(2))))
        {
            Reset();
            return;
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
                CharacterCardUIBoard character = (CharacterCardUIBoard)cardUI;
                if (character == null)
                {
                    Reset();
                    return;
                }
                if ((CharacterCardDetails)cardDetails != null)
                {
                    CharacterCardDetails charDetails = (CharacterCardDetails)cardDetails;
                    isImmovable = charDetails.isImmovable;
                }
                hex = character.GetHex();
            }
            else if (cardDetails.cardClass == CardClass.HazardCreature)
            {
                HazardCreatureCardUIBoard creature = (HazardCreatureCardUIBoard)cardUI;
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

            mouse.RemoveCursor();

            Vector2Int hexSelected = hex;
            Vector3Int currentCellPos = new(hexSelected.x, hexSelected.y, 0);
            Vector3Int target = cellHover.last;

            if (cardUI != lastSelected)
            {
                lastSelected = cardUI;
                companyManagerLayout.Initialize(cardUI.GetOwner());
            }

            if (currentCellPos == target)
            {
                //Debug.Log("Same hex!");
                Reset();
                return;
            }

            if (Input.GetMouseButton(1))
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
        totalMovement = 0;
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
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
            if (selectedCardUIForMovement.GetCardClass() == CardClass.Character)
                moved = ((CharacterCardUIBoard)selectedCardUIForMovement).GetMoved();
            else if (selectedCardUIForMovement.GetCardClass() == CardClass.HazardCreature)
                moved = ((HazardCreatureCardUIBoard)selectedCardUIForMovement).GetMoved();
            
            if (moved == -1 || moved >= MovementConstants.characterMovement)
            {
                Reset();
                yield return null;
            }

            if (selectedCardUIForMovement.GetCardClass() == CardClass.Character)
                ((CharacterCardUIBoard)selectedCardUIForMovement).Moving();
            else if (selectedCardUIForMovement.GetCardClass() == CardClass.HazardCreature)
                ((HazardCreatureCardUIBoard)selectedCardUIForMovement).Moving();

            int currentPointIndex = 0;

            int maxPath = positions.Count;

            if (maxPath < 2)
                yield return null;

            List<CardTypesEnum> accumulatedMana = new();

            if(moved == 0)
            {
                Vector3 initialPosition = cardTilemap.CellToWorld(path[currentPointIndex]);
                initialPosition = new Vector3(initialPosition.x, initialPosition.y, 0);
                Vector3Int initialCell = cardTilemap.WorldToCell(initialPosition);
                CardInfo initialCardInfo = terrainManager.GetCardInfo(initialCell);
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
                    combatPopupManager.Initialize(selectedCardUIForMovement, city.GetDetails());

                    Reset();
                    break;
                }

                short movement = terrainManager.GetMovementCost(targetCell);
                if (moved + movement > MovementConstants.characterMovement)
                    break;

                if (selectedCardUIForMovement.GetCardClass() == CardClass.Character)
                    (selectedCardUIForMovement as CharacterCardUIBoard).AddMovement(movement);
                else if (selectedCardUIForMovement.GetCardClass() == CardClass.HazardCreature)
                    (selectedCardUIForMovement as HazardCreatureCardUIBoard).AddMovement(movement);

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

                CardInfo ci = terrainManager.GetCardInfo(targetCell);
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

            selectedItems.SelectCardDetails(
                selectedCardUIForMovement.GetDetails(),
                selectedCardUIForMovement.GetOwner());

            // IF CHARACTER, ENEMIES
            if (selectedCardUIForMovement.GetCardClass() == CardClass.Character)
                CheckEnemies(accumulatedMana, selectedCardUIForMovement);

            // DIRTY IF CITIES
            Vector2Int finalHex = NULL2;
            if (selectedCardUIForMovement.GetCardClass() == CardClass.Character)
                finalHex = (selectedCardUIForMovement as CharacterCardUIBoard).GetHex();
            else if (selectedCardUIForMovement.GetCardClass() == CardClass.HazardCreature)
                finalHex = (selectedCardUIForMovement as HazardCreatureCardUIBoard).GetHex();

            if (finalHex != NULL2 && board.GetTile(finalHex).HasCity())
                    deckManager.Dirty(DirtyReasonEnum.CHAR_SELECTED);

            cameraController.RemovePreventDrag();
        }
        else
            Reset();
    }

    IEnumerator RenderPath()
    {
        if(Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
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
            totalMovement = ((CharacterCardUIBoard)selectedCardUIForMovement).GetMoved();
        else if (selectedCardUIForMovement.GetCardClass() == CardClass.HazardCreature)
            totalMovement = ((HazardCreatureCardUIBoard)selectedCardUIForMovement).GetMoved();
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
                        
            short movement = (p != 0) ? terrainManager.GetMovementCost(cardTilePos) : (short)0;
            
            if (totalMovement + movement > MovementConstants.characterMovement)
            {
                StopAllCoroutines();
                positions = positions.GetRange(0, p);
                path = path.GetRange(0, p);
                lineRenderer.positionCount = positions.Count;
                lineRenderer.SetPositions(positions.ToArray());
                break;
            }

            totalMovement += movement;
            rightMovementCosts[positions.Count - 1].text = totalMovement.ToString();
            rightMovementCosts[positions.Count - 1].enabled = true;


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
                if (manaManager.HasEnoughMana(owner, hazardCard.cardTypes))
                {
                    toCombat.Add(new Tuple<string, NationsEnum>(cardDetails.cardId, owner));
                    manaManager.RemoveMana(owner, hazardCard.cardTypes);
                }
            }
        }
        if (toCombat.Count > 0)
        {
            TileAndMovementCost tileInfo = terrainManager.GetTileAndMovementCost(lastHex);
            bool res = combatPopupManager.Initialize(
                selectedCardUIForMovement,
                toCombat,
                tileInfo.terrain.terrainType.ToString()
                );
            if (res)
                combatPopupManager.ShowPopup();
        }
    }
}

