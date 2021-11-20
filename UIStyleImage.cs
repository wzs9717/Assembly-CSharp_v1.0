using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStyleImage : UIStyle
{
	public Color color = new Color(1f, 1f, 1f, 1f);

	public override List<Object> GetControlledObjects()
	{
		List<Object> list = new List<Object>();
		list.Add(this);
		Image component = GetComponent<Image>();
		if (component != null)
		{
			list.Add(component);
		}
		return list;
	}

	public override List<Object> UpdateStyle()
	{
		Image component = GetComponent<Image>();
		if (component != null)
		{
			component.color = color;
		}
		return GetControlledObjects();
	}
}
