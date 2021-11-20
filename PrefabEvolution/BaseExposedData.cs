using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace PrefabEvolution
{
	public class BaseExposedData : ISerializationCallbackReceiver
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct Comparer : IComparer<BaseExposedData>
		{
			public int Compare(BaseExposedData x, BaseExposedData y)
			{
				return (int)Mathf.Sign(x.Order - y.Order);
			}
		}

		[NonSerialized]
		public PEExposedProperties Container;

		[SerializeField]
		private int guid = Guid.NewGuid().GetHashCode();

		public string Label;

		public int ParentId;

		public float Order;

		public int SiblingIndex => Brothers.ToList().IndexOf(this);

		public int Id => guid;

		public BaseExposedData Parent
		{
			get
			{
				return Container[ParentId];
			}
			set
			{
				ParentId = value.Id;
			}
		}

		public IEnumerable<BaseExposedData> Children => Container.OrderedItems.Where((BaseExposedData item) => item.ParentId == Id);

		public IEnumerable<BaseExposedData> Brothers
		{
			get
			{
				BaseExposedData parent = Parent;
				return Container.OrderedItems.Where((BaseExposedData i) => i.Parent == parent);
			}
		}

		public bool Inherited => Container.GetInherited(Id);

		public bool Hidden
		{
			get
			{
				return Container.GetHidden(Id);
			}
			set
			{
				Container.SetHide(this, value);
			}
		}

		public virtual void OnBeforeSerialize()
		{
		}

		public virtual void OnAfterDeserialize()
		{
		}

		public float GetOrder(bool next)
		{
			int index = ((!next) ? (SiblingIndex - 1) : (SiblingIndex + 1));
			BaseExposedData baseExposedData = Brothers.ElementAtOrDefault(index);
			return (baseExposedData != null) ? ((Order + baseExposedData.Order) * 0.5f) : (Order + (float)(next ? 1 : (-1)));
		}

		public virtual BaseExposedData Clone()
		{
			BaseExposedData baseExposedData = Activator.CreateInstance(GetType()) as BaseExposedData;
			baseExposedData.ParentId = ParentId;
			baseExposedData.Label = Label;
			baseExposedData.guid = guid;
			baseExposedData.Order = Order;
			return baseExposedData;
		}
	}
}
