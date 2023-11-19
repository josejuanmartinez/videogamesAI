using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TilemapSelector : MonoBehaviour
{
    public float tooltipBoardSecs = 3f;
    public GameObject tooltip;
    public Tilemap selectionTilemap;

    public Tilemap[] terrainTilemaps;
    public Sprite selectedSprite;

    public static Vector3Int NULL = Vector3Int.one * int.MinValue;
    public Vector3Int hoverPos = NULL;

    private Tile selectedTile, unselectedTile;
    private CameraController cameraController;

    private EventSystem eventSystem;
    private DiceManager diceManager;
    private SelectedItems selectedItems;

    private float tileSelectedAt;

    void Awake()
    {
        selectedTile = ScriptableObject.CreateInstance<Tile>();
        selectedTile.sprite = selectedSprite;
        unselectedTile = ScriptableObject.CreateInstance<Tile>();
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        cameraController = Camera.main.GetComponent<CameraController>();
        diceManager = GameObject.Find("DiceManager").GetComponent<DiceManager>();
        selectedItems = GameObject.Find("SelectedItems").GetComponent<SelectedItems>();
        tileSelectedAt = float.MaxValue;
    }

    void Update()
    {
        if(IsOverUI())
        {
            Reset();
            return;
        }

        if(cameraController.IsDragging())
        {
            Reset();
            return;
        }

        if (diceManager.IsDicing())
        {
            Reset();
            return;
        }         

        if (Input.GetKeyUp(KeyCode.Escape))
            Reset();


        /*if (Input.GetMouseButtonDown(0))
        {
            tooltip.SetActive(false);
            selectedItems.UnselectAll();
            return;
        }*/

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cardTilePos = selectionTilemap.WorldToCell(mouseWorldPos);
        Vector3 cardCellCenter = selectionTilemap.CellToWorld(cardTilePos);
        cardCellCenter = new Vector3(cardCellCenter.x, cardCellCenter.y, 0);
            
        //cardTilePos = new Vector3Int(cardTilePos.x, cardTilePos.y, 0);
        cardTilePos = selectionTilemap.WorldToCell(cardCellCenter);
        
        if (hoverPos == null)
            tooltip.SetActive(false);

        if (cardTilePos != hoverPos)
        {
            tooltip.SetActive(false);
            if (hoverPos  != NULL)
            {
                selectionTilemap.SetTile(hoverPos, unselectedTile);
                tileSelectedAt = Time.realtimeSinceStartup;
            }                
            if (cardTilePos != NULL)
            {
                hoverPos = cardTilePos;
                selectionTilemap.SetTile(cardTilePos, selectedTile);
                tileSelectedAt = Time.realtimeSinceStartup;
            }
        }
        else
        {

            if(!Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2) &&
                cardTilePos != null &&
                hoverPos != null &&
                tileSelectedAt != float.MaxValue
                )
            {
                if (Time.realtimeSinceStartup - tileSelectedAt > tooltipBoardSecs)
                {
                    if(!tooltip.activeSelf)
                    {
                        tooltip.SetActive(true);
                        tooltip.GetComponent<MapTooltip>().Initialize(hoverPos);
                        tooltip.transform.position = selectionTilemap.GetCellCenterWorld(cardTilePos);
                    }                    
                } 
                else
                {
                    tooltip.SetActive(false);
                }
            }
        }
    }

    public void Reset()
    {
        if (hoverPos!= NULL)
            selectionTilemap.SetTile(hoverPos, unselectedTile);
        hoverPos = NULL;
        tileSelectedAt = float.MaxValue;
        tooltip.SetActive(false);
    }

    public bool IsOverUI()
    {
        PointerEventData pointerEventData = new(eventSystem) { position = Input.mousePosition };

        List<RaycastResult> results = new();

        eventSystem.RaycastAll(pointerEventData, results);

        if (results.Count > 0)
            return true;
        
        return false;
    }
}