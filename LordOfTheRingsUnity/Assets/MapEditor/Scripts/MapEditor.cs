using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HoneyFramework;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Brainyism
{
  public enum RiverPlacement { NotStarted, Left, Right }

  [System.Serializable]
  public class MapEditor : MonoBehaviour
  {
    public GameObject TerrainMarkerTemplate;

    public bool TerrainPaletteEnabled;
    public bool GeneratePanelEnabled;
    public bool RiverPanelEnabled;
    public bool FileManagementPanelEnabled;
    public Vector2 TerrainScrollBox;
    public Vector2 RiverScrollBox;
    public string FileName;
    public float HeightOffset;

    public LineRenderer RiverHighlight;
    public List<Chunk> MapUpdates;

    public Text HandleInputLabel;
    public bool HandleInput;

    public List<TerrainChange> TerrainChanges;

    public CustomGenerator CustomMapGenerator;
    public Texture2D Image;
    public List<LookupEntry> Lookups = new List<LookupEntry>();
    public int DefaultTerrainIndex;
    public int OffsetX;
    public int OffsetY;

    public PaletteEntryData Selected;
    public List<PaletteEntryData> Data;

    public Button CommitButton;
    public Button DropButton;

    public List<List<Vector3>> Rivers = new List<List<Vector3>>();
    public List<Vector3> River;
    public RiverData CurrentRiverStart;
    public Vector3 RiverPos = Vector3.zero;
    public RiverPlacement RiverStarted = RiverPlacement.NotStarted;
    public GameObject MarkerContainer;
    public NextRiverMarker Marker;
    public List<NextRiverMarker> Markers;

    public Vector3 WorldPos = Vector3i.zero;
    public string MousePos = "";
    public Vector3i HexPos = Vector3i.zero;
    public float Distance = 0f;

    public System.Diagnostics.Stopwatch Stopwatch = new System.Diagnostics.Stopwatch();

    public Plane plane = new Plane(Vector3.up, Vector3.zero);
    public Camera Camera;

    /// <summary>
    /// Inititalization
    /// </summary>
    public void Start()
    {
      this.HandleInput = true;
      this.MapUpdates = new List<Chunk>();
      this.TerrainChanges = new List<TerrainChange>();
      this.Camera = GameObject.Find("WorldCamera").GetComponent<Camera>();
    }

    public void StartTimer()
    {
      this.Stopwatch.Start();
    }

    public void StopTimer()
    {
      this.Stopwatch.Stop();
    }

    public void ResetTimer()
    {
      this.Stopwatch.Reset();
    }

    /// <summary>
    /// Capture user input
    /// </summary>
    public void Update()
    {
      this.CommitButton.enabled = this.MapUpdates.Count > 0 && this.HandleInput;
      this.DropButton.enabled = this.HandleInput;

      if (World.instance != null && this.Stopwatch.IsRunning && World.instance.status == World.Status.Ready)
        StopTimer();

      if (!this.HandleInput)
        return;

      bool hit = false;

      this.MousePos = string.Format("({0}, {1}, {2})", Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
      var ray = this.Camera.ScreenPointToRay(Input.mousePosition);
      this.Distance = 100f;
      if (this.plane.Raycast(ray, out this.Distance))
      {
        this.WorldPos = ray.GetPoint(this.Distance);
        this.HexPos = HexPosition(ray, this.Distance);

        //Debug.DrawLine(ray.GetPoint(0f), this.WorldPos, Color.red);

        // Bail if this didn't click on the map
        if (!IsMapHex(this.HexPos))
          return;
        hit = true;
      }
      else
        this.WorldPos = Vector3i.zero;

      if (World.instance != null && World.instance.status == World.Status.Ready && !EventSystem.current.IsPointerOverGameObject())
      {
        if (Input.GetMouseButton(0))
        {
          if (hit)
          {
            if (IsMapHex(this.HexPos) && this.Selected != null && this.Selected.Terrain != null)
            {
              // See if a marker is already on this hex
              TerrainChange change = this.TerrainChanges.FirstOrDefault(t => HexCoordinates.WorldToHex(t.Marker.transform.position) == this.HexPos);
              if (change == null)
              {
                change = new TerrainChange();
                change.HexPos = this.HexPos;
                change.Selected = this.Selected;

                // Display a terrain marker until they commit changes
                change.Marker = Instantiate(this.TerrainMarkerTemplate) as GameObject;
                Vector3 pos = HexCoordinates.HexToWorld3D(this.HexPos);
                change.Marker.transform.position = new Vector3(pos.x, Mathf.Max(0f, World.GetWorldHeightAt(pos) + this.HeightOffset), pos.z);
                change.Marker.transform.parent = this.transform;
                Material material = new Material(Shader.Find("Diffuse"));
                material.SetTexture("_MainTex", TerrainDefinition.LoadTexture(this.Selected.Terrain.diffusePath));
                material.SetTextureScale("_MainTex", new Vector2(2f, 2f));
                MeshRenderer mr = change.Marker.GetComponent<MeshRenderer>();
                mr.sharedMaterial = material;
                this.TerrainChanges.Add(change);
              }
              else
                change.Marker.GetComponent<MeshRenderer>().sharedMaterial.SetTexture(0, TerrainDefinition.LoadTexture(this.Selected.Terrain.diffusePath));

              // Mark this chunk for re-drawing
              MarkChunkForRebuild();
            }
          }
          else
          {
            Debug.LogWarning(string.Format("Hex position not valid: {0}, {1}, {2}", this.HexPos.x, this.HexPos.y, this.HexPos.z));
          }
        }
        else if (Input.GetMouseButton(1))
        {
          Vector3 riverPosition;

          // Add a river node
          if (this.RiverStarted == RiverPlacement.NotStarted)
          {
            // Start a new river
            this.River = new List<Vector3>();
            this.Rivers.Add(this.River);

            // Calculate the river node (we always start with left aligned)
            riverPosition = new Vector3(this.HexPos.x - 0.5f, this.HexPos.y + 0.5f, this.HexPos.z + 0.5f);
            AddRiverNode(riverPosition);

            // Initialize the river markers
            this.Marker.transform.position = HexCoordinates.HexToWorld3D(riverPosition);
            this.Marker.HexCoordinate = riverPosition;
            SetNextRiverMarker(this.Markers[0], new Vector3(-1f, 0f, 0f));
            SetNextRiverMarker(this.Markers[1], new Vector3(0f, 0f, -1f));
            SetNextRiverMarker(this.Markers[2], new Vector3(0f, -1f, 0f));

            // Next river is right aligned
            this.RiverStarted = RiverPlacement.Right;

            // Show river handles
            this.MarkerContainer.SetActive(true);

            // Mark the chunks that need to be rebuilt
            MarkChunkForRebuild();
          }
          else
          {
            // Add a node to the river we are working on
            RaycastHit marker;
            if (Physics.Raycast(ray, out marker))
            {
              if (marker.collider.name.Contains("RiverMarker"))
              {
                OnRiverNodeClick(marker.collider.GetComponent<NextRiverMarker>());

                // Mark the chunks that need to be rebuilt
                MarkChunkForRebuild();
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// See if this is a valid map hex
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    public bool IsMapHex(Vector3i hexPosition)
    {
      if (World.instance == null)
        return false;

      if (World.GetInstance().hexes.Any(t => t.Key == hexPosition))
      {
        Hex hex = World.GetInstance().hexes[hexPosition];
        return (hex.terrainType.source.mode != MHTerrain.Mode.IsBorderType ||
                hex.terrainType.source.seaType);
      }


      return false;
    }

    /// <summary>
    /// Commits and applies any river changes
    /// </summary>
    private void CommitRiverChanges()
    {
      this.RiverStarted = RiverPlacement.NotStarted;

      // Hide the river markers
      this.MarkerContainer.SetActive(false);

      RebuildRiversFromEditorList();
    }

    /// <summary>
    /// Rebuilds any chunks with changes
    /// </summary>
    public void RebuildDirtyChunks()
    {
      // Commit any river changes so they show up in the redraw
      CommitRiverChanges();

      // Prepare terrain for updating
      foreach (TerrainChange change in this.TerrainChanges)
      {
        // Create the hex we have selected
        Hex h = SetHexDefinition(change.HexPos, change.Selected);
        // Overwrite the old hex
        World.GetInstance().hexes[change.HexPos] = h;
        // Clear the marker
        DestroyObject(change.Marker);
      }
      this.TerrainChanges.Clear();

      // Rebuild the affected chunks
      Hex hex;

      List<Chunk> chunks = new List<Chunk>();
      foreach (KeyValuePair<Vector2i, Chunk> chunk in World.instance.chunks)
        if (this.MapUpdates.Contains(chunk.Value))
          chunks.Add(chunk.Value);

      foreach (Chunk chunk in chunks)
      {
        if (chunk == null || chunk.hexesCovered == null || chunk.hexesCovered.Count == 0)
          continue;

        hex = chunk.hexesCovered.First().Value;
        World.instance.ReadyToPolishHex(hex);
        hex.RebuildChunksOwningThisHex();
      }

      // Clear the river highlighter
      this.RiverHighlight.SetVertexCount(0);

      // Clear the dirty chunks list
      this.MapUpdates.Clear();
    }

    //public void RebuildAll()
    //{
    //  this.MapUpdates.Clear();
    //  foreach (KeyValuePair<Vector2i, Chunk> chunk in World.instance.chunks)
    //    this.MapUpdates.Add(chunk.Value);
    //}

    /// <summary>
    /// Mark any possible chunks that need to be updated due to terrain or river changes
    /// </summary>
    public void MarkChunkForRebuild()
    {
      Vector2 position = HexCoordinates.HexToWorld(this.HexPos);
      float xMin = position.x - Hex.hexTexturePotentialReach;
      float xMax = position.x + Hex.hexTexturePotentialReach;
      float yMin = position.y - Hex.hexTexturePotentialReach;
      float yMax = position.y + Hex.hexTexturePotentialReach;
      Chunk chunk;

      // Add this chunk and any potention overlaps
      chunk = Chunk.WorldToChunk(new Vector3(xMin, 0f, yMin));
      if (!this.MapUpdates.Contains(chunk))
        this.MapUpdates.Add(chunk);
      chunk = Chunk.WorldToChunk(new Vector3(xMax, 0f, yMin));
      if (!this.MapUpdates.Contains(chunk))
        this.MapUpdates.Add(chunk);
      chunk = Chunk.WorldToChunk(new Vector3(xMin, 0f, yMax));
      if (!this.MapUpdates.Contains(chunk))
        this.MapUpdates.Add(chunk);
      chunk = Chunk.WorldToChunk(new Vector3(xMax, 0f, yMax));
      if (!this.MapUpdates.Contains(chunk))
        this.MapUpdates.Add(chunk);
    }

    /// <summary>
    /// Set the river marker hex and transform coordinates
    /// </summary>
    /// <param name="marker">The marker to modify</param>
    /// <param name="offset">The offset to move the marker to</param>
    public void SetNextRiverMarker(NextRiverMarker marker, Vector3 offset)
    {
      marker.HexCoordinate = this.Marker.HexCoordinate + offset;
      marker.transform.position = HexCoordinates.HexToWorld3D(marker.HexCoordinate);
    }

    /// <summary>
    /// Add a new river node at the selected marker location
    /// </summary>
    /// <param name="marker">The marker that was selected</param>
    public void OnRiverNodeClick(NextRiverMarker marker)
    {
      this.Marker.transform.position = marker.transform.position;
      this.Marker.HexCoordinate = marker.HexCoordinate;
      AddRiverNode(marker.HexCoordinate);

      switch (this.RiverStarted)
      {
        case RiverPlacement.Left:
          SetNextRiverMarker(this.Markers[0], new Vector3(-1f, 0f, 0f));
          SetNextRiverMarker(this.Markers[1], new Vector3(0f, 0f, -1f));
          SetNextRiverMarker(this.Markers[2], new Vector3(0f, -1f, 0f));

          this.RiverStarted = RiverPlacement.Right;
          break;
        case RiverPlacement.Right:
          SetNextRiverMarker(this.Markers[0], new Vector3(1f, 0f, 0f));
          SetNextRiverMarker(this.Markers[1], new Vector3(0f, 0f, 1f));
          SetNextRiverMarker(this.Markers[2], new Vector3(0f, 1f, 0f));

          this.RiverStarted = RiverPlacement.Left;
          break;
      }

      // Display the river path
      Vector3 position;
      this.RiverHighlight.SetVertexCount(this.River.Count);
      for (int i = 0; i < this.River.Count; i++)
        if (i != 0)
        {
          position = HexCoordinates.HexToWorld3D(this.River[i - 1]);
          this.RiverHighlight.SetPosition(i - 1, new Vector3(position.x, World.GetWorldHeightAt(position) + 0.1f, position.z));

          position = HexCoordinates.HexToWorld3D(this.River[i]);
          this.RiverHighlight.SetPosition(i, new Vector3(position.x, World.GetWorldHeightAt(position) + 0.1f, position.z));
        }
    }

    /// <summary>
    /// Determine Hex Coordinates from raycast from a plane
    /// </summary>
    /// <param name="ray">The plane object's raycast</param>
    /// <param name="distance">The distance to hit the plane from the screen</param>
    /// <returns>Hex Coordinate</returns>
    public Vector3i HexPosition(Ray ray, float distance)
    {
      if (World.instance == null)
        return Vector3i.zero;

      Vector3 hitPoint = ray.GetPoint(distance);
      hitPoint -= World.instance.transform.position;

      Vector2 hexWorldPosition = VectorUtils.Vector3To2D(hitPoint);
      return HexCoordinates.GetHexCoordAt(hexWorldPosition);
    }

    /// <summary>
    /// Set the terrain at the specified position
    /// </summary>
    /// <param name="position">The position of the hex to modify</param>
    /// <param name="data">Contains the terrain type, image, along with the rotation and blend settings</param>
    /// <returns>The modified hex</returns>
    public Hex SetHexDefinition(Vector3i position, PaletteEntryData data)
    {
      Hex hex = new Hex();

      if (data.RandomPosition)
        hex.orderPosition = Random.Range(0f, 1f);
      else
        hex.orderPosition = data.Blend;

      if (data.RandomRotation)
        hex.rotationAngle = Random.Range(0f, 360f);
      else
        hex.rotationAngle = data.Rotation;

      hex.terrainType = TerrainDefinition.definitions.First(t => t.source.OID == data.Terrain.OID);
      hex.position = position;

      World.GetInstance().ReadyToPolishHex(hex);

      return hex;
    }

    public Hex SetHexDefinition(Vector3i position, PaletteEntryData data, bool useRotation, int rotation, bool useBlend, float blend)
    {
      Hex hex = new Hex();

      if (useBlend)
        hex.orderPosition = blend;
      else
        hex.orderPosition = Random.Range(0f, 1f);

      if (useRotation)
        hex.rotationAngle = rotation;
      else
        hex.rotationAngle = Random.Range(0f, 360f);

      hex.terrainType = TerrainDefinition.definitions.First(t => t.source.OID == data.Terrain.OID);
      hex.position = position;

      World.GetInstance().ReadyToPolishHex(hex);

      return hex;
    }

    /// <summary>
    /// Creates a blank map based on the hex and chunk radius settings in the Hex Framework's World object 
    /// and the selected terrain type
    /// </summary>
    public void InitBlankMap()
    {
      World world = World.GetInstance();
      world.status = HoneyFramework.World.Status.Preparation;

      TerrainDefinition.ReloadDefinitions();

      GenerateOceanWorld(world);

      ForceTerrainRedraw();

      world.status = HoneyFramework.World.Status.TerrainGeneration;
    }

    /// <summary>
    /// Populates the specified world with the selected terrain type
    /// </summary>
    /// <param name="world">The world to modify</param>
    protected void GenerateOceanWorld(World world)
    {
      world.hexes = new Dictionary<Vector3i, Hex>();

      List<Vector3i> rangeHexes = HexNeighbors.GetRange(new Vector3i(), world.hexRadius);
      int terrainCount = TerrainDefinition.definitions.Count;
      if (terrainCount < 1)
      {
        Debug.LogError("No terrain definitions to use!");
        return;
      }

      PaletteEntryData tile = this.Selected;
      if (this.Selected == null || this.Selected.Terrain.seaType)
        tile = this.Data.First();

      foreach (Vector3i v in rangeHexes)
      {
        world.hexes.Add(v, SetHexDefinition(v, tile));
      }
    }

    /// <summary>
    /// Populates the specified world with the specified Texture2D and lookup table
    /// </summary>
    /// <param name="world">The world to modify</param>
    protected void GenerateWorldFromTexture2D(World world)
    {
      try
      {
        world.hexes = new Dictionary<Vector3i, Hex>();

        List<Vector3i> rangeHexes = HexNeighbors.GetRange(new Vector3i(), world.hexRadius);
        int terrainCount = TerrainDefinition.definitions.Count;
        if (terrainCount < 1)
        {
          Debug.LogError("No terrain definitions to use!");
          return;
        }

        bool useRotation, useBlend;
        float g;
        float b;
        int rotation;
        float blend;
        PaletteEntryData tile;
        LookupEntry lookup = null;
        Color32[] colors = this.Image.GetPixels32();
        Color32 color = Color.black;
        int x, y;
        foreach (Vector3i v in rangeHexes)
        {
          lookup = null;
          x = v.x + this.OffsetX;
          y = v.y + this.OffsetY;
          if (x >= 0 && x < this.Image.width && y >= 0 && y < this.Image.height)
          {
            color = colors[x + (y * this.Image.width)];
            lookup = this.Lookups.FirstOrDefault(t => t.Color.r == color.r);
          }

          if (lookup == null)
          {
            tile = this.Data[this.DefaultTerrainIndex];
            g = 0;
            b = 0f;
            useRotation = false;
            useBlend = false;
          }
          else
          {
            g = color.g;
            b = color.b;
            useRotation = lookup.UseRotation;
            useBlend = lookup.UseBlend;
            tile = this.Data.FirstOrDefault(t => t.Terrain.name == lookup.Tile);
            if (tile == null)
              tile = this.Data[this.DefaultTerrainIndex];
          }

          rotation = Mathf.Clamp((int)(g * 2), 0, 360);
          blend = Mathf.Clamp01(b / 255);
          world.hexes.Add(v, SetHexDefinition(v, tile, useRotation, rotation, useBlend, blend));
        }
      }
      catch (System.Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// Adds a river node at the specified location
    /// </summary>
    /// <param name="position">The hex coordinates to place the river node</param>
    public void AddRiverNode(Vector3 position)
    {
      // Add this node to the river we are working on
      this.River.Add(position);
    }

    /// <summary>
    /// Rebuilds the rivers
    /// </summary>
    public void RebuildRiversFromEditorList()
    {
      // Clear the world engine's river list
      World.instance.riversStart.Clear();

      // Build the river linked list
      foreach (List<Vector3> list in this.Rivers)
        World.instance.riversStart.Add(BuildRiverDataList(list));

      // Make adjacent hexes aware of the river nodes
      foreach (RiverData r in World.GetInstance().riversStart)
      {
        // Waiting on MUHA to make this public
        //RiverFactory.InformHexesNextToRiver(r);
        InformHexesNextToRiver(r);
      }

      // Smooth the rivers out
      foreach (RiverData r in World.GetInstance().riversStart)
      {
        // Waiting on MUHA to make this public
        //RiverFactory.PostProcesRiver(r);
        PostProcesRiver(r);
      }
    }

    /// <summary>
    /// Add a river node to the river we are currently working on
    /// </summary>
    /// <param name="river">Position of the new node</param>
    /// <returns>World engine river data</returns>
    private RiverData BuildRiverDataList(List<Vector3> river)
    {
      RiverData start = new RiverData(river[0]);
      RiverData current = start;
      RiverData temp;
      for (int i = 1; i < river.Count; i++)
      {
        temp = new RiverData(river[i]);
        current.next = temp;
        current = temp;
      }

      return start;
    }

    /// <summary>
    /// Rebuild river list
    /// </summary>
    /// <param name="river">River data from an existing river</param>
    /// <returns>World engine river data</returns>
    public void RebuildEditorRiverLists()
    {
      this.Rivers.Clear();

      foreach (RiverData river in World.instance.riversStart)
      {
        List<Vector3> nodes = new List<Vector3>();
        RiverData current = river;
        do
        {
          nodes.Add(current.position);
          current = current.next;
        } while (current != null);

        this.Rivers.Add(nodes);
      }
    }

    /// <summary>
    /// Delete all the rivers and refresh the map
    /// </summary>
    public void DeleteAllRivers()
    {
      if (World.instance == null)
        return;

      World.instance.status = HoneyFramework.World.Status.Preparation;

      this.River.Clear();
      this.Rivers.Clear();

      RebuildRiversFromEditorList();

      ForceTerrainRedraw();

      World.instance.status = HoneyFramework.World.Status.TerrainGeneration;
    }

    /// <summary>
    /// Force the terrain to redraw.  Usually needed after clearing all rivers, etc.
    /// </summary>
    public void ForceTerrainRedraw()
    {
        for (int x = -World.instance.chunkRadius; x <= World.instance.chunkRadius; x++)
            for (int y = -World.instance.chunkRadius; y <= World.instance.chunkRadius; y++)
                World.instance.PrepareChunkData(new Vector2i(x, y));
    }

    /// <summary>
    /// Generate a custom map
    /// </summary>
    public void GenerateCustomMap()
    {
      ResetTimer();
      StartTimer();

      DataManager.Reload();
      this.CustomMapGenerator.Generate();

      RebuildEditorRiverLists();
    }

    /// <summary>
    /// Generate a map using the selected tile
    /// </summary>
    public void GenerateSelectedMap()
    {
      ResetTimer();
      StartTimer();

      DataManager.Reload();
      this.InitBlankMap();

      RebuildEditorRiverLists();
    }

    /// <summary>
    /// Generate a map using the built in random map generator
    /// </summary>
    public void GenerateRandomMap()
    {
      ResetTimer();
      StartTimer();

      DataManager.Reload();
      World.instance.Initialize();

      RebuildEditorRiverLists();
    }

    /// <summary>
    /// Generate a map from specified Texture2D and lookup table
    /// </summary>
    public void GenerateMapFromTexture2D()
    {
      ResetTimer();
      StartTimer();

      World world = World.GetInstance();
      world.status = HoneyFramework.World.Status.Preparation;

      TerrainDefinition.ReloadDefinitions();

      DataManager.Reload();
      this.GenerateWorldFromTexture2D(world);

      RebuildEditorRiverLists();

      ForceTerrainRedraw();

      World.instance.status = HoneyFramework.World.Status.TerrainGeneration;
    }

    /// <summary>
    /// Delete the specified river and refresh the affected chunks
    /// </summary>
    /// <param name="river"></param>
    public void DeleteRiver(List<Vector3> river)
    {
      if (World.instance == null)
        return;

      World.instance.status = HoneyFramework.World.Status.Preparation;

      if (this.River.Count > 0 && this.River[0] == river[0])
        this.River.Clear();

      this.Rivers.Remove(river);

      RebuildRiversFromEditorList();

      ForceTerrainRedraw();

      World.instance.status = HoneyFramework.World.Status.TerrainGeneration;
    }

    /// <summary>
    /// Load a Map
    /// </summary>
    /// <param name="file">Path and filename to the map file to load</param>
    public void LoadMap(string file)
    {
      if (file.Length == 0)
        return;

      ResetTimer();
      StartTimer();

      World.instance.status = HoneyFramework.World.Status.Preparation;

      TerrainDefinition.ReloadDefinitions();

      // Load
      SaveStructure data;

      using (Stream stream = File.Open(file, FileMode.Open))
      {
        BinaryFormatter bFormatter = new BinaryFormatter();
        data = (SaveStructure)bFormatter.Deserialize(stream);
      }

      if (data == null)
        return;

      World.instance.hexRadius = data.worldHexRadius;
      World.instance.riversStart = data.riverData;

      RebuildEditorRiverLists();

      foreach (Hex hex in data.hexes)
      {
        World.instance.hexes[hex.position] = hex;
      }

      ForceTerrainRedraw();

      World.instance.status = HoneyFramework.World.Status.TerrainGeneration;
    }

    /// <summary>
    /// Save a map file
    /// </summary>
    /// <param name="file">The path and filename to save the map to</param>
    public void SaveMap(string file)
    {
      if (file.Length == 0)
        return;

      SaveStructure save = new SaveStructure();
      save.worldHexRadius = World.instance.hexRadius;
      save.riverData = World.instance.riversStart;

      save.hexes = new List<Hex>();
      foreach (KeyValuePair<Vector3i, Hex> pair in World.instance.hexes)
      {
        save.hexes.Add(pair.Value);
      }

      using (Stream stream = File.Open(file + ".hon", FileMode.Create))
      {
        BinaryFormatter bFormatter = new BinaryFormatter();
        bFormatter.Serialize(stream, save);
      }
    }

    /// <summary>
    /// Drop any pending changes
    /// </summary>
    public void DropChanges()
    {
      foreach (TerrainChange change in this.TerrainChanges)
        DestroyObject(change.Marker);
      this.TerrainChanges.Clear();
      this.MapUpdates.Clear();
      this.River.Clear();
      this.Rivers.Clear();
      this.RiverStarted = RiverPlacement.NotStarted;
      this.RiverHighlight.SetVertexCount(0);
      this.MarkerContainer.SetActive(false);
    }

    /// <summary>
    /// Toggle the flag that determines if we check for input
    /// </summary>
    public void OnActiveClick()
    {
      this.HandleInput = !this.HandleInput;
      if (this.HandleInput)
      {
        this.HandleInputLabel.text = "Map Editor Is Active";
      }
      else
      {
        this.HandleInputLabel.text = "Map Editor Is Inactive";
      }
    }

    private void InformHexesNextToRiver(RiverData startPoint)
    {
      if (startPoint == null || startPoint.next == null || startPoint.next.next == null)
        return;

      RiverData walker = startPoint;
      walker.neighbours = walker.GetNeighbours();

      while (walker.next != null)
      {
        walker.next.neighbours = walker.next.GetNeighbours();

        List<Vector3i> common = walker.neighbours.FindAll(o => walker.next.neighbours.IndexOf(o) > -1);

        if (common.Count == 2)
        {
          Hex h1 = World.GetInstance().hexes[common[0]];
          Hex h2 = World.GetInstance().hexes[common[1]];

          if (h1 != null && h2 != null)
          {
            if (h1.directionsPassingRiver.IndexOf(h2) == -1)
              h1.directionsPassingRiver.Add(h2);

            if (h2.directionsPassingRiver.IndexOf(h1) == -1)
              h2.directionsPassingRiver.Add(h1);
          }
        }

        walker = walker.next;
      }
    }

    private void PostProcesRiver(RiverData startPoint)
    {
      if (startPoint == null || startPoint.next == null || startPoint.next.next == null)
        return;

      RiverData walker = startPoint;
      RiverData stopAt = null;
      int blendNodeCount = 5;

      List<RiverData> riverSequence = new List<RiverData>();
      while (walker != null)
      {
        if (stopAt == null && walker.usedByPostProcess)
        {
          blendNodeCount--;
          if (blendNodeCount <= 0)
            stopAt = walker;
        }

        walker.neighbours = walker.GetNeighbours();
        riverSequence.Add(walker);
        walker.usedByPostProcess = true;
        walker = walker.next;
      }

      RiverData p1;
      RiverData p2;
      RiverData p3;

      RiverData pN0;
      RiverData pN1;
      RiverData pNi;

      List<RiverData> newRiverSequence = new List<RiverData>();
      newRiverSequence.Add(riverSequence[0]);

      int finalIndex = -1;

      for (int i = 0; i < riverSequence.Count - 2; i++)
      {
        p1 = riverSequence[i];
        p2 = riverSequence[i + 1];
        p3 = riverSequence[i + 2];

        pN0 = newRiverSequence[newRiverSequence.Count - 1];

        Vector3 v1 = (p1.position.GetVector() + p3.position.GetVector()) * 0.5f;
        Vector3 v2 = (v1 + p2.position) * 0.5f;

        pN1 = new RiverData(v2, true);
        pN1.neighbours = p2.neighbours;

        Vector3 interPoint = (pN0.position.GetVector() + pN1.position.GetVector() + p1.position.GetVector() + p2.position.GetVector()) * 0.25f;
        pNi = new RiverData(interPoint, true);

        newRiverSequence.Add(pNi);
        newRiverSequence.Add(pN1);

        pN0 = pN1;

        if (p1 == stopAt)
          finalIndex = newRiverSequence.Count;
      }

      Vector3i last = Vector3i.down;
      for (int i = 0; i < riverSequence.Count - 1; i++)
      {
        int common = 0;
        List<Vector3i> commonArea = new List<Vector3i>();

        for (int k = i; k < riverSequence.Count; k++)
        {
          List<Vector3i> newCommonArea;
          if (k == i)
            newCommonArea = riverSequence[k].neighbours;
          else
            newCommonArea = commonArea.Intersect(riverSequence[k].neighbours).ToList();

          if (newCommonArea == null)
            Debug.LogWarning("new common area is null at " + k + "; " + riverSequence[k].position);

          if (newCommonArea.Count == 0)
            break;

          commonArea = newCommonArea;
          common++;
        }

        if (common >= 4 && last != commonArea[0])
        {
          if (commonArea.Count != 1)
            Debug.LogError("Common area count is " + commonArea.Count + " which is wrong. It should at this stage always reach 1");

          int newListIndex = i * 2;
          int newListSecondIndex = (i + common) * 2;
          int smoothingMargin = 2;
          Vector3 center = new Vector3(commonArea[0].x, commonArea[0].y, commonArea[0].z);

          for (int j = newListIndex - smoothingMargin; j < newListSecondIndex + 1 + smoothingMargin; j++)
          {
            if (j > 0 && j < newRiverSequence.Count)
            {
              Vector3 dir = newRiverSequence[j].position - center;

              if (j >= newListSecondIndex)
              {
                int offset = j - newListSecondIndex;
                newRiverSequence[j].position += dir.normalized * (1 + smoothingMargin - offset) * 0.06f; //smoothing offset
              }

              if (j < newListIndex)
              {
                int offset = newListIndex - j;
                newRiverSequence[j].position += dir.normalized * (1 + smoothingMargin - offset) * 0.06f; //smoothing offset
              }
            }
          }

          last = commonArea[0];
        }
      }

      for (int i = 0; i < newRiverSequence.Count - 1; i++)
      {
        newRiverSequence[i].SetNext(newRiverSequence[i + 1]);
        if (finalIndex > -1 && finalIndex == i)
          break;
      }

      if (finalIndex == -1)
        newRiverSequence[newRiverSequence.Count - 1].SetNext(riverSequence[riverSequence.Count - 1]);
    }

    /// <summary>
    /// Add a lookup entry
    /// </summary>
    /// <param name="color">The color to map to a tile</param>
    /// <param name="tilename">The name of the tile to map to</param>
    public void AddLookup(Color32 color, string tilename)
    {
      this.Lookups.Add(new LookupEntry() { Color = color, Red = color.r, Tile = tilename, UseColor = true });
    }
  }

  public class TerrainChange
  {
    public GameObject Marker;
    public Vector3i HexPos;
    public PaletteEntryData Selected;
  }

  public class LookupEntry
  {
    public Color32 Color;
    public byte Red;
    public string Tile;
    public bool UseColor;
    public bool UseRotation;
    public bool UseBlend;
  }
}