using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CellHover : MonoBehaviour
{
    public TextMeshProUGUI hex;
    private bool debug = true;

    Tilemap tilemap;
    public Vector3Int last = Vector3Int.zero;

    private void Awake()
    {
        tilemap = GameObject.Find("MovementTilemap").GetComponent<Tilemap>();

        // Get the position of the first tile in the Tilemap's bounds
        Vector3Int minTilePosition = new Vector3Int(tilemap.cellBounds.min.x, tilemap.cellBounds.min.y, 0);
        Vector3Int maxTilePosition = new Vector3Int(tilemap.cellBounds.max.x, tilemap.cellBounds.max.y, 0);
        

        // Get the tile at the first position
        TileBase firstTile = tilemap.GetTile(minTilePosition);
        TileBase lastTile = tilemap.GetTile(maxTilePosition);

    }
    // Update is called once per frame
    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        try
        {
            Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);
            cellPosition = new Vector3Int(cellPosition.x, cellPosition.y, 0);
            //Vector3 tileCenter = tilemap.CellToWorld(cellPosition) + new Vector3(0.5f, 0.5f, 0f);
            if (cellPosition != last)
            {
                hex.text = HexTranslator.GetNormalizedCellPosString(cellPosition, debug);
                last = cellPosition;
            }
        }
        catch { }
    }

    public void Toggle()
    {
        debug = !debug;
    }
}
