using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PrefabEvolution
{
	[Serializable]
	public class PEExposedProperties : ISerializationCallbackReceiver
	{
		[NonSerialized]
		internal List<BaseExposedData> InheritedProperties;

		[NonSerialized]
		public PEPrefabScript PrefabScript;

		public List<ExposedProperty> Properties = Utils.Create<List<ExposedProperty>>();

		public List<ExposedPropertyGroup> Groups = Utils.Create<List<ExposedPropertyGroup>>();

		[SerializeField]
		private List<int> Hidden = Utils.Create<List<int>>();

		public BaseExposedData this[int id] => Items.FirstOrDefault((BaseExposedData p) => p.Id == id);

		public BaseExposedData this[string label] => OrderedItems.FirstOrDefault((BaseExposedData p) => p.Label == label);

		public IEnumerable<BaseExposedData> Items => GetInheritedProperties().Concat(Properties.OfType<BaseExposedData>().Concat(Groups.OfType<BaseExposedData>()));

		public IEnumerable<BaseExposedData> OrderedItems
		{
			get
			{
				BaseExposedData.Comparer comparer = default(BaseExposedData.Comparer);
				List<BaseExposedData> list = Items.ToList();
				list.Sort(comparer);
				return list;
			}
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
		}

		public IEnumerable<BaseExposedData> GetInheritedProperties()
		{
			if (InheritedProperties == null)
			{
				InheritedProperties = new List<BaseExposedData>();
				if (PrefabScript == null)
				{
					return InheritedProperties;
				}
				if (PrefabScript.ParentPrefab != null)
				{
					PEPrefabScript component = PrefabScript.ParentPrefab.GetComponent<PEPrefabScript>();
					if (component == null)
					{
						Debug.Log("Inherited property Error: Prefab script not found on", PrefabScript);
						return InheritedProperties;
					}
					InheritedProperties.AddRange(component.Properties.Items.Where((BaseExposedData i) => !i.Hidden).Select(delegate(BaseExposedData p)
					{
						BaseExposedData baseExposedData = p.Clone();
						baseExposedData.Container = this;
						return baseExposedData;
					}));
					Properties.RemoveAll((ExposedProperty p) => p.Inherited);
					Groups.RemoveAll((ExposedPropertyGroup p) => p.Inherited);
					Hidden.RemoveAll((int p) => Items.All((BaseExposedData item) => item.Id != p));
					foreach (ExposedProperty item in InheritedProperties.OfType<ExposedProperty>())
					{
						item.Target = PrefabScript.Links[component.Links[item.Target]]?.InstanceTarget;
						if (item.Target == null)
						{
							Debug.Log("Inherited property Error: Local target is not found Path:" + item.PropertyPath, PrefabScript);
						}
					}
				}
			}
			return InheritedProperties;
		}

		public void Add(BaseExposedData exposed)
		{
			exposed.Container = this;
			if (exposed is ExposedProperty exposed2)
			{
				Add(exposed2);
			}
			else
			{
				Add(exposed as ExposedPropertyGroup);
			}
		}

		public void Add(ExposedProperty exposed)
		{
			exposed.Container = this;
			if (!Properties.Contains(exposed))
			{
				Properties.Add(exposed);
			}
		}

		public void Add(ExposedPropertyGroup exposed)
		{
			exposed.Container = this;
			if (!Groups.Contains(exposed))
			{
				Groups.Add(exposed);
			}
		}

		public void Remove(int id)
		{
			Properties.RemoveAll((ExposedProperty p) => p.Id == id);
			Groups.RemoveAll((ExposedPropertyGroup p) => p.Id == id);
		}

		public ExposedProperty FindProperty(string label)
		{
			return Items.OfType<ExposedProperty>().FirstOrDefault((ExposedProperty p) => p.Label == label);
		}

		public ExposedProperty FindProperty(int id)
		{
			return Items.OfType<ExposedProperty>().FirstOrDefault((ExposedProperty p) => p.Id == id);
		}

		public ExposedProperty FindProperty(uint id)
		{
			return Items.OfType<ExposedProperty>().FirstOrDefault((ExposedProperty p) => p.Id == (int)id);
		}

		public bool GetInherited(int id)
		{
			return GetInheritedProperties().Any((BaseExposedData i) => i.Id == id);
		}

		public bool GetHidden(int id)
		{
			return Hidden.Any((int i) => i == id);
		}

		public void SetHide(BaseExposedData property, bool state)
		{
			if (state != Hidden.Contains(property.Id))
			{
				if (state)
				{
					Hidden.Add(property.Id);
				}
				else
				{
					Hidden.Remove(property.Id);
				}
				Hidden.Sort();
			}
		}
	}
}
