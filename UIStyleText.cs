using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStyleText : UIStyle
{
	public Color color = new Color(1f, 1f, 1f, 1f);

	public FontStyle fontStyle;

	public int fontSize = 28;

	public override List<Object> GetControlledObjects()
	{
		List<Object> list = new List<Object>();
		list.Add(this);
		Text component = GetComponent<Text>();
		if (component != null)
		{
			list.Add(component);
		}
		return list;
	}

	public override List<Object> UpdateStyle()
	{
		Text component = GetComponent<Text>();
		if (component != null)
		{
			component.color = color;
			component.fontSize = fontSize;
			component.fontStyle = fontStyle;
		}
		return GetControlledObjects();
	}
}
