using System;
using System.Collections.Generic;

namespace PrefabEvolution
{
	[Serializable]
	public class ExposedPropertyGroup : BaseExposedData
	{
		public static Dictionary<int, bool> expandedDict = new Dictionary<int, bool>();

		private bool expandedLoaded;

		private bool expanded = true;

		public bool Expanded
		{
			get
			{
				if (!expandedLoaded)
				{
					expandedDict.TryGetValue(base.Id, out expanded);
					expandedLoaded = true;
				}
				return expanded;
			}
			set
			{
				if (value != expanded)
				{
					if (!expandedDict.ContainsKey(base.Id))
					{
						expandedDict.Add(base.Id, value: true);
					}
					expandedDict[base.Id] = value;
					expanded = value;
				}
			}
		}
	}
}
