using System.Collections.Generic;
using UnityEngine;

public class UIStyleImageMaster : UIStyleImage
{
	public override List<Object> GetControlledObjects()
	{
		List<Object> list = new List<Object>();
		UIStyleImage[] componentsInChildren = GetComponentsInChildren<UIStyleImage>(includeInactive: true);
		foreach (UIStyleImage uIStyleImage in componentsInChildren)
		{
			if (uIStyleImage == this)
			{
				list.Add(this);
			}
			else
			{
				if (!(uIStyleImage.styleName == styleName) || !(uIStyleImage.gameObject != base.gameObject))
				{
					continue;
				}
				List<Object> controlledObjects = uIStyleImage.GetControlledObjects();
				foreach (Object item in controlledObjects)
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	public override List<Object> UpdateStyle()
	{
		List<Object> list = new List<Object>();
		UIStyleImage[] componentsInChildren = GetComponentsInChildren<UIStyleImage>(includeInactive: true);
		foreach (UIStyleImage uIStyleImage in componentsInChildren)
		{
			if (uIStyleImage == this)
			{
				list.Add(this);
			}
			else
			{
				if (!(uIStyleImage.styleName == styleName) || !(uIStyleImage.gameObject != base.gameObject))
				{
					continue;
				}
				uIStyleImage.color = color;
				List<Object> list2 = uIStyleImage.UpdateStyle();
				foreach (Object item in list2)
				{
					list.Add(item);
				}
			}
		}
		return list;
	}
}
