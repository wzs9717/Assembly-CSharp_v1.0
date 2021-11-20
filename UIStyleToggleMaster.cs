using System.Collections.Generic;
using UnityEngine;

public class UIStyleToggleMaster : UIStyleToggle
{
	public override List<Object> GetControlledObjects()
	{
		List<Object> list = new List<Object>();
		UIStyleToggle[] componentsInChildren = GetComponentsInChildren<UIStyleToggle>(includeInactive: true);
		foreach (UIStyleToggle uIStyleToggle in componentsInChildren)
		{
			if (uIStyleToggle == this)
			{
				list.Add(this);
			}
			else
			{
				if (!(uIStyleToggle.styleName == styleName) || !(uIStyleToggle.gameObject != base.gameObject))
				{
					continue;
				}
				List<Object> controlledObjects = uIStyleToggle.GetControlledObjects();
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
		UIStyleToggle[] componentsInChildren = GetComponentsInChildren<UIStyleToggle>(includeInactive: true);
		foreach (UIStyleToggle uIStyleToggle in componentsInChildren)
		{
			if (uIStyleToggle == this)
			{
				list.Add(this);
			}
			else
			{
				if (!(uIStyleToggle.styleName == styleName) || !(uIStyleToggle.gameObject != base.gameObject))
				{
					continue;
				}
				uIStyleToggle.normalColor = normalColor;
				uIStyleToggle.highlightedColor = highlightedColor;
				uIStyleToggle.pressedColor = pressedColor;
				uIStyleToggle.disabledColor = disabledColor;
				uIStyleToggle.colorMultiplier = colorMultiplier;
				List<Object> list2 = uIStyleToggle.UpdateStyle();
				foreach (Object item in list2)
				{
					list.Add(item);
				}
			}
		}
		return list;
	}
}
