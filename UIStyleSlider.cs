using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStyleSlider : UIStyle
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
		Slider component = GetComponent<Slider>();
		if (component != null)
		{
			list.Add(component);
		}
		return list;
	}

	public override List<Object> UpdateStyle()
	{
		Slider component = GetComponent<Slider>();
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
