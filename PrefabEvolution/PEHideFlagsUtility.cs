using UnityEngine;

namespace PrefabEvolution
{
	internal static class PEHideFlagsUtility
	{
		internal static void HideFlagsSet(this Object obj, HideFlags flags, bool value)
		{
			if (value)
			{
				obj.AddHideFlags(flags);
			}
			else
			{
				obj.RemoveHideFlags(flags);
			}
		}

		internal static void AddHideFlags(this Object obj, HideFlags flags)
		{
			obj.hideFlags |= flags;
		}

		internal static void RemoveHideFlags(this Object obj, HideFlags flags)
		{
			obj.hideFlags &= ~flags;
		}
	}
}
