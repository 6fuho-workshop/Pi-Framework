using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PandaBT
{
	public static partial class Compat
	{
		public static T FindObjectOfType<T>(bool includeInactive = false)
		where T : Object
		{
			return GameObject.FindFirstObjectByType<T>(includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude);
		}

		public static T[] FindObjectsOfType<T>(bool includeInactive = false)
			where T : Object
		{
			return GameObject.FindObjectsByType<T>(
				findObjectsInactive: includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude,
				sortMode: FindObjectsSortMode.None
			);
		}

	}
}

