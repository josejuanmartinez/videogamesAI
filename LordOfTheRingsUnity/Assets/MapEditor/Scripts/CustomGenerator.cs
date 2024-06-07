using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Derive from this class and override this method to create a custom map generator.
/// </summary>
public abstract class CustomGenerator : MonoBehaviour
{
	/// <summary>
	/// Derive from this class and override this method to create a custom map generator.
	/// </summary>
	public abstract void Generate();
}
