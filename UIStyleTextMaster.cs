using System.Collections.Generic;
using UnityEngine;

public class UIStyleTextMaster : UIStyleText
{
	public override List<Object> GetControlledObjects()
	{
		List<Object> list = new List<Object>();
		UIStyleText[] componentsInChildren = GetComponentsInChildren<UIStyleText>(includeInactive: true);
		foreach (UIStyleText uIStyleText in componentsInChildren)
		{
			if (uIStyleText == this)
			{
				list.Add(this);
			}
			else
			{
				if (!(uIStyleText.styleName == styleName) || !(uIStyleText.gameObject != base.gameObject))
				{
					continue;
				}
				List<Object> controlledObjects = uIStyleText.GetControlledObjects();
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
		UIStyleText[] componentsInChildren = GetComponentsInChildren<UIStyleText>(includeInactive: true);
		foreach (UIStyleText uIStyleText in componentsInChildren)
		{
			if (uIStyleText == this || !(uIStyleText.styleName == styleName) || !(uIStyleText.gameObject != base.gameObject))
			{
				continue;
			}
			uIStyleText.color = color;
			uIStyleText.fontSize = fontSize;
			uIStyleText.fontStyle = fontStyle;
			List<Object> list2 = uIStyleText.UpdateStyle();
			foreach (Object item in list2)
			{
				list.Add(item);
			}
		}
		list.Add(this);
		return list;
	}
}
