using HoneyFramework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridsToColourMap : MonoBehaviour
{
    public Tilemap[] terrainTilemaps;
    public bool generate = false;
    public bool save = false;
    public bool load = false;

    private TerrainManager terrainManager;

    private void Awake()
    {
        terrainManager = GameObject.Find("TerrainManager").GetComponent<TerrainManager>();
    }

    private void Update()
    {
        if (save)
        {
            save = false;
            SaveManager.Save(World.GetInstance(), true);
        }

        if (load)
        {
            load = false;
            SaveManager.Load(World.GetInstance());
            ForceTerrainRedraw();

            World.instance.status = HoneyFramework.World.Status.TerrainGeneration;
        }

        if (generate)
        {

            generate = false;

            World.GetInstance().status = HoneyFramework.World.Status.Preparation;
            

            TerrainDefinition.ReloadDefinitions();
            DataManager.Reload();

            BoundsInt bounds = terrainTilemaps[0].cellBounds;

            World.GetInstance().hexes = new Dictionary<Vector3i, Hex>();
            World.GetInstance().hexRadius = Mathf.Max(10, 10);
            World.GetInstance().chunkRadius = 5;
            // World.GetInstance().hexRadius = Mathf.Max(bounds.size.x, bounds.size.y);
            // World.GetInstance().chunkRadius = 25;

            List<Vector3i> rangeHexes = HexNeighbors.GetRange(new Vector3i(), World.GetInstance().hexRadius);

            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    int index = rangeHexes.FindIndex(v => v.x == x && v.y == y);
                    if (index == -1)
                        continue;
                    Vector3i vi = rangeHexes[index];

                    TerrainInfo terrainInfo = null;

                    for (int i = 0; i < terrainTilemaps.Length; i++)
                    {

                        Vector3Int tilePosition = new(x, y, 0);
                        //Tile tile = allTiles[i][(y - bounds.yMin) * bounds.size.x + (x - bounds.xMin)] as Tile;
                        UnityEngine.Tilemaps.Tile tile = terrainTilemaps[i].GetTile(tilePosition) as UnityEngine.Tilemaps.Tile;
                        if (tile != null)
                        {
                            terrainInfo = terrainManager.GetTerrainInfo(tile);
                            if (terrainInfo != null)
                            {
                                break;
                            }
                        }
                    }

                    TerrainsEnum terrainType = TerrainsEnum.PLAINS;
                    if (terrainInfo != null)
                        terrainType = terrainInfo.terrainType;

                    Hex hex = new Hex();

                    hex.orderPosition = Random.Range(0f, 1f);
                    hex.rotationAngle = Random.Range(0f, 360f);

                    hex.terrainType = TerrainDefinition.definitions.First(t => t.source.name == terrainType.ToString());
                    hex.position = vi;

                    World.GetInstance().ReadyToPolishHex(hex);
                    World.GetInstance().hexes.Add(vi, hex);
                }
            }

            ForceTerrainRedraw();

            World.instance.status = HoneyFramework.World.Status.TerrainGeneration;
        }
    }

    public void ForceTerrainRedraw()
    {
        for (int x = -World.instance.chunkRadius; x <= World.instance.chunkRadius; x++)
            for (int y = -World.instance.chunkRadius; y <= World.instance.chunkRadius; y++)
                World.instance.PrepareChunkData(new Vector2i(x, y));
    }
}
