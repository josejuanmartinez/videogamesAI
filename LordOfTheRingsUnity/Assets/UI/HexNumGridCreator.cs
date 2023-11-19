using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexNumGridCreator : MonoBehaviour
{
    public GameObject hexNum;
    public Tilemap baseTilemap;
    public Transform worldSpaceCanvas;

    public bool recreate;

    void Awake()
    {
        recreate = false;
        int children = worldSpaceCanvas.childCount;
        for (int i = 0; i < children; i++)
            DestroyImmediate(worldSpaceCanvas.GetChild(0).gameObject);
        BoundsInt bounds = baseTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                GameObject go = Instantiate(hexNum, worldSpaceCanvas, true);
                Vector3 center = baseTilemap.GetCellCenterWorld(tilePosition);
                Vector3 cellSize = baseTilemap.cellSize;
                go.transform.position = new Vector3(center.x, center.y + (cellSize.y / 2), 0);
                go.GetComponent<TextMeshProUGUI>().text = x + "," + y;
            }
        }
    }

    void Update()
    {
        if(recreate)
            Awake();
    }
}
