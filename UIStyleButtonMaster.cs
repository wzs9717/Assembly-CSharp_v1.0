using System.Collections.Generic;
using UnityEngine;

public class UIStyleButtonMaster : UIStyleButton
{
	public override List<Object> GetControlledObjects()
	{
		List<Object> list = new List<Object>();
		UIStyleButton[] componentsInChildren = GetComponentsInChildren<UIStyleButton>(includeInactive: true);
		foreach (UIStyleButton uIStyleButton in componentsInChildren)
		{
			if (uIStyleButton == this)
			{
				list.Add(this);
			}
			else
			{
				if (!(uIStyleButton.styleName == styleName) || !(uIStyleButton.gameObject != base.gameObject))
				{
					continue;
				}
				List<Object> controlledObjects = uIStyleButton.GetControlledObjects();
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
		UIStyleButton[] componentsInChildren = GetComponentsInChildren<UIStyleButton>(includeInactive: true);
		foreach (UIStyleButton uIStyleButton in componentsInChildren)
		{
			if (uIStyleButton == this)
			{
				list.Add(this);
			}
			else
			{
				if (!(uIStyleButton.styleName == styleName) || !(uIStyleButton.gameObject != base.gameObject))
				{
					continue;
				}
				uIStyleButton.normalColor = normalColor;
				uIStyleButton.highlightedColor = highlightedColor;
				uIStyleButton.pressedColor = pressedColor;
				uIStyleButton.disabledColor = disabledColor;
				uIStyleButton.colorMultiplier = colorMultiplier;
				List<Object> list2 = uIStyleButton.UpdateStyle();
				foreach (Object item in list2)
				{
					list.Add(item);
				}
			}
		}
		return list;
	}
}
