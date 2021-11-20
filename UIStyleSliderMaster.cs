using System.Collections.Generic;
using UnityEngine;

public class UIStyleSliderMaster : UIStyleSlider
{
	public override List<Object> GetControlledObjects()
	{
		List<Object> list = new List<Object>();
		UIStyleSlider[] componentsInChildren = GetComponentsInChildren<UIStyleSlider>(includeInactive: true);
		foreach (UIStyleSlider uIStyleSlider in componentsInChildren)
		{
			if (uIStyleSlider == this)
			{
				list.Add(this);
			}
			else
			{
				if (!(uIStyleSlider.styleName == styleName) || !(uIStyleSlider.gameObject != base.gameObject))
				{
					continue;
				}
				List<Object> controlledObjects = uIStyleSlider.GetControlledObjects();
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
		UIStyleSlider[] componentsInChildren = GetComponentsInChildren<UIStyleSlider>(includeInactive: true);
		foreach (UIStyleSlider uIStyleSlider in componentsInChildren)
		{
			if (uIStyleSlider == this)
			{
				list.Add(this);
			}
			else
			{
				if (!(uIStyleSlider.styleName == styleName) || !(uIStyleSlider.gameObject != base.gameObject))
				{
					continue;
				}
				uIStyleSlider.normalColor = normalColor;
				uIStyleSlider.highlightedColor = highlightedColor;
				uIStyleSlider.pressedColor = pressedColor;
				uIStyleSlider.disabledColor = disabledColor;
				uIStyleSlider.colorMultiplier = colorMultiplier;
				List<Object> list2 = uIStyleSlider.UpdateStyle();
				foreach (Object item in list2)
				{
					list.Add(item);
				}
			}
		}
		return list;
	}
}
