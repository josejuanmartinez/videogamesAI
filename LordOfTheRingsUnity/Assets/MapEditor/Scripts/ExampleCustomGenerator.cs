using UnityEngine;
using System.Collections;
using HoneyFramework;
using System.Collections.Generic;
using System.Linq;

public class ExampleCustomGenerator : CustomGenerator
{
	public int Radius;
	public int ChunkRadius;

	public override void Generate()
	{
		// Get the world engine and tell it we are preparing to generate
		World world = World.instance;
		world.chunkRadius = this.ChunkRadius;
		world.status = HoneyFramework.World.Status.Preparation;

		// Refresh terrain definitions
		TerrainDefinition.ReloadDefinitions();

		// Clear out the old hex data
		world.hexes = new Dictionary<Vector3i, Hex>();

		// Get the hexes we are going to populate
		List<Vector3i> rangeHexes = HexNeighbors.GetRange(new Vector3i(), this.Radius);
		int terrainCount = TerrainDefinition.definitions.Count;
		if (terrainCount < 1)
		{
			Debug.LogError("No terrain definitions to use!");
			return;
		}

		// Add the hex data
		Hex hex;
		MHTerrain td;
		foreach (Vector3i v in rangeHexes)
		{
			hex = new Hex();
			hex.orderPosition = Random.Range(0f, 1f);
			hex.rotationAngle = Random.Range(0f, 360f);

			// Just grab the first non-sea terrain type
			td = MHTerrain.list.First(t => t.seaType == false);
			hex.terrainType = TerrainDefinition.definitions.First(t => t.source.OID == td.OID);

			hex.position = v;
			World.instance.ReadyToPolishHex(hex);

			world.hexes.Add(v, hex);
		}

		// Force terrain redraw
		for (int x = -world.chunkRadius; x <= world.chunkRadius; x++)
			for (int y = -world.chunkRadius; y <= world.chunkRadius; y++)
			{
				world.PrepareChunkData(new Vector2i(x, y));
			}

		// Update the status to show the engine is generating the terrain
		world.status = HoneyFramework.World.Status.TerrainGeneration;
	}
}
