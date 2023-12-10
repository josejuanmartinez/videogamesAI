using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;


[System.Serializable]
public struct TileAndMovementCost
{
    public Vector3Int cellPosition;
    public TerrainInfo terrain;
    public CardInfo cardInfo;
    public bool movable;
    public float movementCost;
}

public class TerrainManager : MonoBehaviour
{
    public Tilemap[] terrainTilemaps;
    public Tilemap cardTilemap;
    public Tilemap movementTilemap;

    public Sprite[] tileTypeSprites;
    public GameObject[] tileTypeGameObjects;

    public Sprite[] tileSprites;
    public GameObject[] tileGameObjects;

    private TileAndMovementCost[] tiles;
    public void Awake()
    {
        BoundsInt bounds = terrainTilemaps[0].cellBounds;

        tiles = new TileAndMovementCost[bounds.size.x * bounds.size.y];

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                bool found = false;

                for (int i = 0; i < terrainTilemaps.Length; i++)
                {

                    Vector3Int tilePosition = new(x, y, 0);
                    //Tile tile = allTiles[i][(y - bounds.yMin) * bounds.size.x + (x - bounds.xMin)] as Tile;
                    Tile tile = terrainTilemaps[i].GetTile(tilePosition) as Tile;
                    if (tile != null)
                    {
                        TerrainInfo terrainInfo = GetTerrainInfo(tile);
                        if(terrainInfo == null)
                        {
                            Debug.LogError(string.Format("{0} does not have a TerrainInfo associated to it.", tile.sprite));
                            continue;
                        }
                        Tile cardTile = cardTilemap.GetTile(tilePosition) as Tile;
                        CardInfo cardInfo = GetCardInfo(cardTile);
                        if (cardInfo == null)
                        {
                            Debug.LogError(string.Format("{0} does not have a CardInfo associated to it.", cardTile.sprite));
                            continue;
                        }
                        short movement = Terrains.movementCost[terrainInfo.terrainType];
                        TileAndMovementCost structValue = new() { cellPosition = tilePosition, terrain = terrainInfo, cardInfo = cardInfo, movable = true, movementCost = movement };

                        int pos = HexTranslator.GetNormalizedCellPosInt(new Vector3Int(x, y, 0));
                        tiles[pos] = structValue;
                        found = true;
                        break;
                    }
                }

                if (!found)
                    Debug.Log("Tile " + x.ToString() + "," + y.ToString() + " does not have any sprite in any layer");
            }
        }
        //Debug.Log("Terrain manager finished loading at " + Time.realtimeSinceStartup);
    }
    public TileAndMovementCost GetTileAndMovementCost(int tileNum)
    {
        return tiles[tileNum];
    }

    public TileAndMovementCost GetTileAndMovementCost(Vector2Int tile)
    {
        int pos = HexTranslator.GetNormalizedCellPosInt(new Vector3Int(tile.x, tile.y, 0));
        return GetTileAndMovementCost(pos);
    }
    public TileAndMovementCost GetTileAndMovementCost(Vector3Int tile)
    {
        int pos = HexTranslator.GetNormalizedCellPosInt(tile);
        return GetTileAndMovementCost(pos);
    }

    public short GetMovementCost(Vector3Int targetCell)
    {
        for (int i = 0; i < terrainTilemaps.Length; i++)
        {
            Tile terrainTile = (Tile)terrainTilemaps[i].GetTile(targetCell);
            if (terrainTile != null)
            {
                if (terrainTile.gameObject.TryGetComponent<TerrainInfo>(out var terrainTileInfo))
                {
                    TerrainsEnum terrainEnum = terrainTileInfo.terrainType;
                    short movement = Terrains.movementCost[terrainEnum];
                    return movement;
                }
            }
        }
        return 0;
    }

    public Sprite GetSpriteMovement(Vector3Int cardTilePos)
    {
        Tile cardTile = (Tile)cardTilemap.GetTile(cardTilePos);
        if (cardTile == null)
            return null;

        CardInfo cardTileInfo = GetCardInfo(cardTile);
        if (cardTileInfo == null)
            return null;

        Sprite cardTileSprite = cardTilemap.GetSprite(cardTilePos);
        Tile newTile = ScriptableObject.CreateInstance<Tile>();
        newTile.sprite = cardTileSprite;
        movementTilemap.SetTile(cardTilePos, newTile);

        // Right Canvas Trail
        return cardTileSprite;
    }
    public TerrainInfo GetTerrainInfo(Tile tile)
    {
        if (tile == null)
        {
            Debug.Log("Unable to get cell tile at " + tile);
            return null;
        }
        int index = Array.FindIndex(tileSprites, x => x == tile.sprite);
        if (index == -1)
        {
            Debug.Log("No GameObject defined for tile sprite " + tile.sprite);
            return null;
        }
        return tileGameObjects[index].GetComponent<TerrainInfo>();
    }

    public CardInfo GetCardInfo(Tile tile)
    {
        if (tile == null)
        {
            Debug.Log("Unable to get cell tile at " + tile);
            return null;
        }
        int index = Array.FindIndex(tileTypeSprites, x => x == tile.sprite);
        if (index == -1)
        {
            Debug.Log("No GameObject defined for tile sprite " + tile.sprite);
            return null;
        }
        return tileTypeGameObjects[index].GetComponent<CardInfo>();            
    }
}
