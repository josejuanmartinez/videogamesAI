using UnityEngine;
using UnityEngine.Tilemaps;

public class FOWCreator : MonoBehaviour
{
    public Tile fowTile;
    
    void Awake()
    {
        Tilemap tilemap = GetComponent<Tilemap>();
        BoundsInt bounds = tilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                tilemap.SetTile(tilePosition, fowTile);


            }
        }
    }
}
