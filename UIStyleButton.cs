using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStyleButton : UIStyle
{
	public Color normalColor = Color.white;

	public Color highlightedColor = new Color(0.7f, 0.7f, 0.7f, 1f);

	public Color pressedColor = new Color(0.5f, 0.5f, 0.5f, 1f);

	public Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

	public float colorMultiplier = 1f;

	public override List<Object> GetControlledObjects()
	{
		List<Object> list = new List<Object>();
		list.Add(this);
		Button component = GetComponent<Button>();
		if (component != null)
		{
			list.Add(component);
		}
		return list;
	}

	public override List<Object> UpdateStyle()
	{
		Button component = GetComponent<Button>();
		if (component != null)
		{
			ColorBlock colors = component.colors;
			colors.normalColor = normalColor;
			colors.highlightedColor = highlightedColor;
			colors.pressedColor = pressedColor;
			colors.disabledColor = disabledColor;
			colors.colorMultiplier = colorMultiplier;
			component.colors = colors;
		}
		return GetControlledObjects();
	}
}
