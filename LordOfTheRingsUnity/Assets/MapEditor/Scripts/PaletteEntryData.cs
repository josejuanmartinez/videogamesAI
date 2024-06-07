using HoneyFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Brainyism
{
	[Serializable]
	public class PaletteEntryData
	{
		public MHTerrain Terrain;
		public Texture Image;
		public float Rotation;
		public float Blend;
		public bool RandomRotation;
		public bool RandomPosition;
	}
}
