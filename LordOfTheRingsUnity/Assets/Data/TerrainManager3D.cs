using HoneyFramework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainManager3D : MonoBehaviour
{
    public TerrainCamera3D terrainCamera3D;
    public Tilemap[] terrainTilemaps;
    public bool generate = false;
    public bool save = false;
    public bool load = false;
    public bool drawChunk = false;

    public Vector3i hexToTest;

    private TerrainManager terrainManager;
    private List<Vector2i> chunksDrawn;
    private Dictionary<Vector3i, List<Vector2i>> hexesChunks;

    private bool hexesLoaded = false;
    
    private string chunkLookAt = string.Empty;

    private void Awake()
    {
        terrainManager = GameObject.Find("TerrainManager").GetComponent<TerrainManager>();
        chunksDrawn = new List<Vector2i>();
    }

    private void Update()
    {
        if(terrainManager.IsAwaken() && !hexesLoaded)
        {
            PreloadTerrain();
        }
        
        if(chunkLookAt != string.Empty)
        {
            GameObject go = GameObject.Find(chunkLookAt);
            Debug.Log("Trying to find " + chunkLookAt);
            if(go)
            {
                terrainCamera3D.LookAt(go.transform);
                chunkLookAt = string.Empty;
            }
        }

        if(drawChunk)
        {
            drawChunk = false;
            DrawChunk(hexToTest.x, hexToTest.y);
        }

        if (save)
        {
            save = false;
            SaveTerrain();
        }
        
        if (load)
        {
            load = false;
            LoadTerrain();
        }

        if (generate)
        {

            generate = false;

            World.GetInstance().status = HoneyFramework.World.Status.Preparation;
            

            TerrainDefinition.ReloadDefinitions();
            DataManager.Reload();

            BoundsInt bounds = terrainTilemaps[0].cellBounds;

            World.GetInstance().hexes = new Dictionary<Vector3i, Hex>();
            int hexRadius = Mathf.Max(bounds.size.x, bounds.size.y);
            World.GetInstance().SetHexRadius(hexRadius);

            List<Vector3i> rangeHexes = HexNeighbors.GetRange(new Vector3i(), hexRadius);

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

                    hex.orderPosition = UnityEngine.Random.Range(0f, 1f);
                    hex.rotationAngle = UnityEngine.Random.Range(0f, 360f);

                    hex.terrainType = TerrainDefinition.definitions.First(t => t.source.name == terrainType.ToString());
                    hex.position = vi;

                    World.GetInstance().ReadyToPolishHex(hex);
                    World.GetInstance().hexes.Add(vi, hex);
                }
            }

            DrawTerrain();
        }
    }

    private void SaveTerrain()
    {
        SaveManager.Save(World.GetInstance());
    }
    
    private void PreloadTerrain()
    {
        hexesLoaded = false;
        hexesChunks = SaveManager.LoadTerrain(World.GetInstance());
        hexesLoaded = true;
        Debug.Log("3D Terrain manager finished PRELOAD at " + Time.realtimeSinceStartup);
    }
    private void LoadTerrain()
    {
        hexesLoaded = false;
        hexesChunks = SaveManager.LoadTerrain(World.GetInstance());
        hexesLoaded = true;
        Debug.Log("3D Terrain manager finished FULL LOAD at " + Time.realtimeSinceStartup);
        Debug.Log("Drawing chunks...");
        DrawTerrain();
        Debug.Log("Finished drawing chunks at" + Time.realtimeSinceStartup);
    }

    public void DrawTerrain()
    {
        for (int x = -World.GetInstance().chunkRadius; x <= World.GetInstance().chunkRadius; x++)
            for (int y = -World.GetInstance().chunkRadius; y <= World.GetInstance().chunkRadius; y++)
                World.instance.PrepareChunkData(new Vector2i(x, y));
    }
    public void DrawChunk(int x, int y)
    {
        Vector3i vihex = new Vector3i(x, y, 0);
        List<Vector2i> chunks = hexesChunks[vihex];
        foreach (Vector2i chunk in chunks)
        {
            if(!chunksDrawn.Contains(chunk))
            {
                SaveManager.LoadChunkFromChunkPosition(World.GetInstance(), chunk);
                World.instance.PrepareChunkData(chunk);
                chunksDrawn.Add(chunk);
                string chunkGameObjectName = "Chunk" + chunk.ToString();
                Debug.Log(chunkGameObjectName + " loaded.");
                chunkLookAt = chunkGameObjectName;
            }
            else
            {
                Debug.Log("Chunk " + chunk.ToString() + " already drawn.");
            }
        }            
    }
}
