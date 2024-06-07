using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Brainyism;
using HoneyFramework;
using System.IO;

namespace Brainyism
{
  public enum MapPane { Main, ImageMap }

  [CustomEditor(typeof(MapEditor))]
  public class MapEditorInspector : Editor
  {
    public MapEditor MapEditor;
    public SerializedProperty TerrainPaletteEnabled;
    public SerializedProperty GeneratePanelEnabled;
    public SerializedProperty RiverPanelEnabled;
    public SerializedProperty FileManagementPanelEnabled;
    public SerializedProperty TerrainScrollBox;
    public SerializedProperty RiverScrollBox;
    public SerializedProperty River;
    public SerializedProperty Rivers;
    public SerializedProperty HeightOffset;
    public SerializedProperty DefaultTerrainIndex;
    public SerializedProperty OffsetX;
    public SerializedProperty OffsetY;

    public List<Vector3> VisibleRiver = new List<Vector3>();
    public float RiverPanelMinSize = 80;
    public float RiverPanelMore = 300;
    public float RiverPanelLess = 80;
    public MapPane SelectedMapPane;
    private string[] MapPaneToolbar = { "Main", "Image Map" };

    private Color Current;

    /// <summary>
    /// Get the properties in a way that allows serialization 
    /// </summary>
    public void OnEnable()
    {
      this.serializedObject.Update();

      this.MapEditor = (MapEditor)target;
      this.TerrainPaletteEnabled = this.serializedObject.FindProperty("TerrainPaletteEnabled");
      this.GeneratePanelEnabled = this.serializedObject.FindProperty("GeneratePanelEnabled");
      this.RiverPanelEnabled = this.serializedObject.FindProperty("RiverPanelEnabled");
      this.FileManagementPanelEnabled = this.serializedObject.FindProperty("FileManagementPanelEnabled");
      this.TerrainScrollBox = this.serializedObject.FindProperty("TerrainScrollBox");
      this.RiverScrollBox = this.serializedObject.FindProperty("RiverScrollBox");
      this.River = this.serializedObject.FindProperty("River");
      this.Rivers = this.serializedObject.FindProperty("Rivers");
      this.HeightOffset = this.serializedObject.FindProperty("HeightOffset");
      this.DefaultTerrainIndex = this.serializedObject.FindProperty("DefaultTerrainIndex");
      this.OffsetX = this.serializedObject.FindProperty("OffsetX");
      this.OffsetY = this.serializedObject.FindProperty("OffsetY");

      this.SelectedMapPane = MapPane.Main;
    }

    /// <summary>
    /// Initialize the map editor 
    /// </summary>
    public void Init()
    {
      if (this.MapEditor.Data == null || this.MapEditor.Data.Count == 0 || this.MapEditor.Data[0].Terrain == null)
      {
        this.MapEditor.Data = new List<PaletteEntryData>();

        if (MHTerrain.list.Count == 0)
          TerrainDefinition.ReloadDefinitions();

        foreach (MHTerrain t in MHTerrain.list.OrderBy(t => t.name))
        {
          this.MapEditor.Data.Add(
            new PaletteEntryData()
            {
              Terrain = t,
              Image = TerrainDefinition.LoadTexture(t.diffusePath),
              Blend = 0f,
              Rotation = 0f,
              RandomPosition = true,
              RandomRotation = true
            });
        }
      }
    }

    /// <summary>
    /// Display custom inspector for map manipulation
    /// </summary>
    public override void OnInspectorGUI()
    {
      this.serializedObject.Update();

      Init();

      // Misc Feedback
      EditorGUILayout.BeginVertical("Box");
      if (EditorApplication.isPlaying)
      {
        //GUILayout.Label("Ray Distance: " + this.MapEditor.Distance);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Mouse " + this.MapEditor.MousePos);
        GUILayout.Label(string.Format("Hex ({0}, {1}, {2})", this.MapEditor.HexPos.x, this.MapEditor.HexPos.y, this.MapEditor.HexPos.z));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(string.Format("World ({0:0.00}, {1:0.00}, {2:0.00})", this.MapEditor.WorldPos.x, this.MapEditor.WorldPos.y, this.MapEditor.WorldPos.z));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField(string.Format("World Generated in {0}", this.MapEditor.Stopwatch.Elapsed));
      }
      else
      {
        GUILayout.Label("No information available when not in play mode");
      }
      EditorGUILayout.EndVertical();

      this.Current = GUI.color;

      DisplayFileManagerPanel();

      DisplayMapPanel();

      DisplayTerrainPalette();

      DisplayRiverPanel();

      GUI.color = this.Current;

      this.serializedObject.ApplyModifiedProperties();

      Repaint();
    }

    /// <summary>
    /// Custom editor display for the Terrain Palette
    /// </summary>
    public void DisplayTerrainPalette()
    {
      GUI.color = new Color(161f / 255f, 224f / 255f, 255f / 255f);
      this.TerrainPaletteEnabled.boolValue = EditorGUILayout.Foldout(this.TerrainPaletteEnabled.boolValue, " Terrain Palette");
      GUI.color = this.Current;
      this.HeightOffset.floatValue = EditorGUILayout.FloatField("Height Offset", this.HeightOffset.floatValue);

      if (!this.TerrainPaletteEnabled.boolValue)
        return;

      if (this.MapEditor.Data.Count == 0 || this.MapEditor.Data[0].Terrain == null)
      {
        EditorGUILayout.BeginHorizontal("Box");
        EditorGUILayout.LabelField("No terrain definitions have been loaded");
        EditorGUILayout.EndHorizontal();

        return;
      }

      this.TerrainScrollBox.vector2Value = EditorGUILayout.BeginScrollView(this.TerrainScrollBox.vector2Value);
      EditorGUILayout.BeginVertical();

      foreach (PaletteEntryData data in this.MapEditor.Data)
      {
        // Tile Name
        EditorGUILayout.BeginHorizontal("Box", GUILayout.Width(300));
        GUILayout.Label(data.Terrain.name, EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal("Box", GUILayout.Height(80f), GUILayout.Width(300));

        // Image Button
        if (this.MapEditor.Selected == null || this.MapEditor.Selected != data)
        {
          if (GUILayout.Button(data.Image, GUILayout.Width(64), GUILayout.Height(64)))
          {
            this.MapEditor.Selected = data;
            //EditorGUI.DrawPreviewTexture(EditorGUILayout.GetControlRect(GUILayout.Width(64f), GUILayout.Height(64f)), td.diffuse);
            //Texture temp = EditorGUILayout.ObjectField(td.source.name, td.diffuse, typeof(Texture), true) as Texture;
          }
        }
        else
          EditorGUI.DrawPreviewTexture(EditorGUILayout.GetControlRect(GUILayout.Width(64f), GUILayout.Height(64f)), this.MapEditor.Selected.Image);

        DisplayGenerationFields(data);

        EditorGUILayout.EndHorizontal();
      }

      EditorGUILayout.EndVertical();
      EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// Display the terrain tile sub-panel
    /// </summary>
    /// <param name="data"></param>
    private void DisplayGenerationFields(PaletteEntryData data)
    {
      EditorGUILayout.BeginVertical();
      EditorGUILayout.BeginVertical();

      // Rotation, Position, and Random
      EditorGUILayout.BeginVertical("Box", GUILayout.MinHeight(60));

      EditorGUILayout.LabelField("                            Random", GUILayout.MaxWidth(165));
      EditorGUILayout.BeginHorizontal();
      data.Rotation = DisplayFloat(data.Rotation, "Rotation:", 0f, 360f);
      data.RandomRotation = EditorGUILayout.Toggle(data.RandomRotation, GUILayout.MaxWidth(70));
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.BeginHorizontal();
      data.Blend = DisplayFloat(data.Blend, "Blend:", 0f, 1f);
      data.RandomPosition = EditorGUILayout.Toggle(data.RandomPosition, GUILayout.MaxWidth(70));
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.EndVertical();

      EditorGUILayout.EndVertical();

      // Foregrounds
      if (data.Terrain.fgTypes.Count != 0)
      {
        EditorGUILayout.BeginVertical("Box", GUILayout.MinWidth(225));

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        foreach (MHSimpleCounter fgt in data.Terrain.fgTypes)
        {
          EditorGUILayout.LabelField(string.Format("({0}) {1}", fgt.count, fgt.name), GUILayout.MaxWidth(140));
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
      }

      // Tags
      if (data.Terrain.typeList.Count != 0)
      {
        EditorGUILayout.BeginVertical("Box", GUILayout.MinWidth(225));

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        foreach (MHTerrain.Type t in data.Terrain.typeList)
        {
          EditorGUILayout.LabelField(t.ToString(), GUILayout.MaxWidth(140));
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
      }

      EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Display the map panel
    /// </summary>
    public void DisplayMapPanel()
    {
      GUI.color = new Color(135f / 255f, 175f / 255f, 232f / 255f);
      this.GeneratePanelEnabled.boolValue = EditorGUILayout.Foldout(this.GeneratePanelEnabled.boolValue, " Map Generation");
      if (!this.GeneratePanelEnabled.boolValue)
        return;
      GUI.color = this.Current;

      if (World.instance == null || (World.instance.status != World.Status.Ready && World.instance.status != World.Status.NotReady))
      {
        EditorGUILayout.BeginHorizontal("Box");
        EditorGUILayout.LabelField("World engine is busy");
        EditorGUILayout.EndHorizontal();
        return;
      }

      //if (GUILayout.Button("Force Rebuild"))
      //  this.MapEditor.RebuildAll();

      this.SelectedMapPane = (MapPane)GUILayout.Toolbar((int)this.SelectedMapPane, this.MapPaneToolbar);
      if (this.SelectedMapPane == MapPane.Main)
        DisplayMainMapPanel();
      else
        DisplayImageMapPanel();
    }

    /// <summary>
    /// Display the map panel
    /// </summary>
    public void DisplayMainMapPanel()
    {
      EditorGUILayout.BeginVertical("Box");
      this.MapEditor.CustomMapGenerator = EditorGUILayout.ObjectField("Custom:", this.MapEditor.CustomMapGenerator, typeof(CustomGenerator), true) as CustomGenerator;
      if (World.instance == null)
      {
        EditorGUILayout.LabelField("Only available in play mode");
        EditorGUILayout.EndVertical();
        return;
      }

      string title = "* Select terrain from palette to enable *";
      if (this.MapEditor.Selected == null)
        title = string.Format("Generate Map of {0}", this.MapEditor.Data.First().Terrain.name);
      else if (this.MapEditor.Selected.Terrain != null)
        title = string.Format("Generate Map of {0}", this.MapEditor.Selected.Terrain.name);

      if (World.instance.status != World.Status.Ready && World.instance.status != World.Status.NotReady)
      {
        EditorGUILayout.BeginHorizontal("Box");
        EditorGUILayout.LabelField("World engine is busy");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
      }
      else
      {
        // Custom Map Generator
        if (this.MapEditor.CustomMapGenerator == null)
          EditorGUILayout.LabelField("Attach a class that derives from CustomGenerator");
        else
        {
          if (GUILayout.Button("Generate Custom Map"))
            if (World.instance.status == World.Status.Ready || World.instance.status == World.Status.NotReady)
            {
              this.MapEditor.GenerateCustomMap();
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // Generate Random
        if (GUILayout.Button("Generate Random Map"))
          if (World.instance.status == World.Status.Ready || World.instance.status == World.Status.NotReady)
          {
            this.MapEditor.GenerateRandomMap();
          }

        // Generate Selected Tile
        if (this.MapEditor.Selected.Terrain == null)
        {
          EditorGUILayout.BeginHorizontal("Box");
          EditorGUILayout.LabelField(title);
          EditorGUILayout.EndHorizontal();
        }
        else if (GUILayout.Button(title))
          if (World.instance.status == World.Status.Ready || World.instance.status == World.Status.NotReady)
          {
            this.MapEditor.GenerateSelectedMap();
          }
      }
    }

    public string ErrorMessage = "";
    /// <summary>
    /// Display the image map panel
    /// </summary>
    public void DisplayImageMapPanel()
    {
      if (World.instance == null)
      {
        EditorGUILayout.LabelField("Only available in play mode");
        return;
      }

      EditorGUILayout.BeginVertical("Box");

      // Build list of terrain types
      List<string> temp = new List<string>();
      foreach (PaletteEntryData data in this.MapEditor.Data)
        temp.Add(data.Terrain.name);

      GUILayout.BeginHorizontal();
      GUILayout.BeginVertical();
      if (GUILayout.Button("Add Lookup", GUILayout.Width(100)))
        this.MapEditor.AddLookup(Color.black, "");
      GUILayout.Label("Default: ", GUILayout.Width(50));
      this.DefaultTerrainIndex.intValue = EditorGUILayout.Popup(this.DefaultTerrainIndex.intValue, temp.ToArray(), GUILayout.Width(100));
      GUILayout.EndVertical();

      GUILayout.BeginVertical();
      GUILayout.Label("Offset X: ", GUILayout.Width(50));
      this.OffsetX.intValue = EditorGUILayout.IntField(this.OffsetX.intValue, GUILayout.Width(30));
      GUILayout.Label("Offset Y: ", GUILayout.Width(50));
      this.OffsetY.intValue = EditorGUILayout.IntField(this.OffsetY.intValue, GUILayout.Width(30));
      GUILayout.EndVertical();

      this.MapEditor.Image = EditorGUILayout.ObjectField("", this.MapEditor.Image, typeof(Texture2D), true) as Texture2D;
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label(new GUIContent("T", "Toggle between color lookup and numeric input"), GUILayout.Width(20));
      GUILayout.Label("Lookup", GUILayout.Width(50));
      GUILayout.Label("Terrain", GUILayout.Width(100));
      GUILayout.Label(new GUIContent("R", "Use value (0-180) in g for rotation (value*2)"), GUILayout.Width(30));
      GUILayout.Label(new GUIContent("B", "Use value in b for blend (value / 255)"), GUILayout.Width(30));
      GUILayout.EndHorizontal();

      int selected;
      int index = 0;
      LookupEntry entry;
      while (index < this.MapEditor.Lookups.Count)
      {
        GUILayout.BeginHorizontal();

        entry = this.MapEditor.Lookups[index];

        entry.UseColor = GUILayout.Toggle(entry.UseColor, "", GUILayout.Width(20f));
        if (entry.UseColor)
        {
          entry.Color = EditorGUILayout.ColorField(entry.Color, GUILayout.Width(50f));
          entry.Red = entry.Color.r;
        }
        else
        {
          entry.Red = (byte)Mathf.Clamp(EditorGUILayout.IntField(entry.Red, GUILayout.Width(50f)), 0, 255);
          entry.Color = new Color32(entry.Red, entry.Color.g, entry.Color.b, 255);
        }

        selected = temp.IndexOf(entry.Tile);
        if (selected < 0)
          selected = 0;
        selected = EditorGUILayout.Popup(selected, temp.ToArray(), GUILayout.Width(100));
        entry.Tile = temp[selected];

        entry.UseRotation = EditorGUILayout.Toggle(entry.UseRotation, GUILayout.Width(30));
        entry.UseBlend = EditorGUILayout.Toggle(entry.UseBlend, GUILayout.Width(30));

        if (GUILayout.Button("X", GUILayout.Width(30f)))
          this.MapEditor.Lookups.Remove(entry);

        GUILayout.EndHorizontal();

        index++;
      }

      string errors = "";
      if (this.MapEditor.Lookups.Count == 0)
        errors += "You need to add at least one lookup";

      if (this.MapEditor.Lookups.GroupBy(t => t.Tile).Select(t => t.First()).Count() < this.MapEditor.Lookups.Count)
        errors += "\r\nOnly one tile mapping is allowed";

      if (this.MapEditor.Lookups.GroupBy(t => t.Color.r).Select(t => t.First()).Count() < this.MapEditor.Lookups.Count)
        errors += "\r\nAll red values must be unique";

      if (this.MapEditor.Image == null)
        errors += "\r\nYou need to select a texture";
      else
      {
        try
        {
          this.MapEditor.Image.GetPixel(0, 0);
        }
        catch (System.Exception)
        {
          errors += "\r\nThe texture must be readable";
        }
      }

      if (errors.Length > 0)
      {
        //GUILayout.Space(10f);
        GUILayout.BeginHorizontal("Box");
        GUI.color = new Color(0.75f, 0f, 0f);
        GUILayout.Label(errors);
        GUI.color = this.Current;
        GUILayout.EndHorizontal();
      }

      GUILayout.Space(10f);

      if (GUILayout.Button("Generate Map From Texture"))
      {
        if (World.instance.status == World.Status.Ready || World.instance.status == World.Status.NotReady)
        {
          try
          {
            this.ErrorMessage = "";
            this.MapEditor.GenerateMapFromTexture2D();
          }
          catch (System.Exception e)
          {
            this.ErrorMessage = e.Message;
          }
        }
      }

      if (this.ErrorMessage.Length > 0)
        GUILayout.TextArea(this.ErrorMessage);

      EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Display the river panel
    /// </summary>
    public void DisplayRiverPanel()
    {
      EditorGUILayout.BeginHorizontal();

      GUI.color = new Color(135f / 255f, 225f / 255f, 232f / 255f);
      this.RiverPanelEnabled.boolValue = EditorGUILayout.Foldout(this.RiverPanelEnabled.boolValue, " River Management");
      if (!this.RiverPanelEnabled.boolValue)
        return;
      GUI.color = this.Current;

      if (this.RiverPanelMinSize == this.RiverPanelLess)
      {
        if (GUILayout.Button("Show More"))
          this.RiverPanelMinSize = this.RiverPanelMore;
      }
      else
      {
        if (GUILayout.Button("Show Less"))
          this.RiverPanelMinSize = this.RiverPanelLess;
      }
      EditorGUILayout.EndHorizontal();

      this.RiverScrollBox.vector2Value = EditorGUILayout.BeginScrollView(this.RiverScrollBox.vector2Value, GUILayout.MinHeight(this.RiverPanelMinSize));

      EditorGUILayout.BeginVertical("Box");
      if (World.instance == null)
      {
        EditorGUILayout.LabelField("Only available in play mode");
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        return;
      }

      if (World.instance.status != World.Status.Ready && World.instance.status != World.Status.NotReady)
      {
        EditorGUILayout.BeginHorizontal("Box");
        EditorGUILayout.LabelField("World engine is busy");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        return;
      }

      if (this.MapEditor.Rivers.Count == 0)
      {
        EditorGUILayout.LabelField("No rivers found");
      }
      else if (GUILayout.Button("Delete All Rivers"))
      {
        this.MapEditor.DeleteAllRivers();
      }

      foreach (List<Vector3> river in this.MapEditor.Rivers)
      {
        if (river.Count > 0)
        {
          EditorGUILayout.BeginHorizontal("Box");
          EditorGUILayout.LabelField(string.Format(" River {0}, {1}, {2}", river[0].x, river[0].y, river[0].z));

          if (this.VisibleRiver != river)
          {
            if (GUILayout.Button("Show"))
              this.VisibleRiver = river;
          }
          else if (GUILayout.Button("Hide"))
            this.VisibleRiver = null;

          if (GUILayout.Button("Delete"))
            this.MapEditor.DeleteRiver(river);
          EditorGUILayout.EndHorizontal();

          if (this.VisibleRiver == river)
            foreach (Vector3 v in river)
            {
              EditorGUILayout.BeginHorizontal();
              EditorGUILayout.LabelField(string.Format("{0}, {1}, {2}", v.x, v.y, v.z));
              EditorGUILayout.EndHorizontal();
            }
        }
      }

      EditorGUILayout.EndScrollView();
      EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Display the File Management Panel
    /// </summary>
    public void DisplayFileManagerPanel()
    {
      GUI.color = new Color(149f / 255f, 164f / 255f, 255f / 255f);
      this.FileManagementPanelEnabled.boolValue = EditorGUILayout.Foldout(this.FileManagementPanelEnabled.boolValue, " File Management");
      if (!this.FileManagementPanelEnabled.boolValue)
        return;
      GUI.color = this.Current;

      EditorGUILayout.BeginHorizontal();

      if (World.instance == null)
      {
        EditorGUILayout.BeginHorizontal("Box");
        EditorGUILayout.LabelField("Only available in play mode");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndHorizontal();
        return;
      }

      if (World.instance.status != World.Status.Ready && World.instance.status != World.Status.NotReady)
      {
        EditorGUILayout.BeginHorizontal("Box");
        EditorGUILayout.LabelField("World engine is busy");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndHorizontal();
        return;
      }

      EditorGUILayout.BeginVertical();

      EditorGUILayout.BeginHorizontal("Box");
      EditorGUILayout.LabelField(this.MapEditor.FileName);
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.BeginHorizontal();

      if (GUILayout.Button("Open Map"))
      {
        string path = EditorUtility.OpenFilePanel("Select Terrain File to Load", EditorPrefs.GetString("HoneyEditorMapPath", Application.dataPath), "hon");
        string chr = path.IndexOf(@"\") == -1 ? @"/" : @"\";
        int index = path.LastIndexOf(chr) + 1;
        EditorPrefs.SetString("HoneyEditorMapPath", path.Substring(0, index));
        this.MapEditor.FileName = path.Substring(index, path.Length - index);
        this.MapEditor.LoadMap(path);
      }

      if (GUILayout.Button("Save Map"))
      {
        string path = EditorUtility.SaveFilePanel("Save Map", EditorPrefs.GetString("HoneyEditorMapPath", Application.dataPath), "Map", "");
        string chr = path.IndexOf(@"\") == -1 ? @"/" : @"\";
        int index = path.LastIndexOf(chr) + 1;
        EditorPrefs.SetString("HoneyEditorMapPath", path.Substring(0, index));
        this.MapEditor.SaveMap(path);
      }

      EditorGUILayout.EndVertical();

      EditorGUILayout.EndVertical();

      EditorGUILayout.EndHorizontal();
    }

    public float DisplayFloat(float holder, string label, float min, float max)
    {
      EditorGUIUtility.labelWidth = 70f;
      holder = EditorGUILayout.FloatField(label, holder, GUILayout.MaxWidth(140));
      EditorGUIUtility.labelWidth = 0f;

      if (holder < min)
        holder = min;
      else if (holder > max)
        holder = max;

      return holder;
    }
  }
}