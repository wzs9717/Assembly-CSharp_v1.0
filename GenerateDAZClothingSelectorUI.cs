using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateDAZClothingSelectorUI : GenerateTabbedUI
{
	public DAZCharacterSelector characterSelector;

	protected List<string> enabledClothingItemNames;

	[HideInInspector]
	public string[] clothingItemNames;

	[HideInInspector]
	public bool[] clothingItemEnabled;

	protected Dictionary<string, Toggle> clothingItemNameToToggle;

	public void SetClothingItemToggle(string clothingItemName, bool on)
	{
		if (clothingItemNameToToggle != null && clothingItemNameToToggle.TryGetValue(clothingItemName, out var value))
		{
			value.isOn = on;
		}
	}

	public override void TabChange(string name, bool on)
	{
		clothingItemNameToToggle = new Dictionary<string, Toggle>();
		base.TabChange(name, on);
	}

	protected override Transform InstantiateControl(Transform parent, int index)
	{
		Transform transform = base.InstantiateControl(parent, index);
		string text = enabledClothingItemNames[index];
		Toggle component = transform.GetComponent<Toggle>();
		if (component != null)
		{
			string cname = text;
			clothingItemNameToToggle.Add(cname, component);
			component.onValueChanged.AddListener(delegate(bool arg0)
			{
				if (characterSelector != null)
				{
					characterSelector.SetActiveClothingItem(cname, arg0);
				}
			});
			component.isOn = characterSelector.IsClothingItemActive(text);
		}
		foreach (Transform item in transform)
		{
			if (item.name == "Text")
			{
				Text component2 = item.GetComponent<Text>();
				if (component2 != null)
				{
					component2.text = text;
				}
			}
		}
		return transform;
	}

	public void AutoGenerateItems()
	{
		Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
		if (clothingItemEnabled != null && clothingItemEnabled.Length == clothingItemNames.Length)
		{
			for (int i = 0; i < clothingItemNames.Length; i++)
			{
				dictionary.Add(clothingItemNames[i], clothingItemEnabled[i]);
			}
		}
		List<string> list = new List<string>();
		if (characterSelector != null)
		{
			DAZClothingItem[] clothingItems = characterSelector.clothingItems;
			foreach (DAZClothingItem dAZClothingItem in clothingItems)
			{
				list.Add(dAZClothingItem.displayName);
			}
		}
		clothingItemNames = list.ToArray();
		clothingItemEnabled = new bool[clothingItemNames.Length];
		for (int k = 0; k < clothingItemNames.Length; k++)
		{
			if (dictionary.TryGetValue(clothingItemNames[k], out var value))
			{
				clothingItemEnabled[k] = value;
			}
			else
			{
				clothingItemEnabled[k] = true;
			}
		}
	}

	public override void Generate()
	{
		enabledClothingItemNames = new List<string>();
		for (int i = 0; i < clothingItemNames.Length; i++)
		{
			if (clothingItemEnabled[i])
			{
				enabledClothingItemNames.Add(clothingItemNames[i]);
			}
		}
		base.Generate();
		clothingItemNameToToggle = new Dictionary<string, Toggle>();
		if (controlUIPrefab != null && tabUIPrefab != null && tabButtonUIPrefab != null && clothingItemNames != null)
		{
			for (int j = 0; j < enabledClothingItemNames.Count; j++)
			{
				AllocateControl();
			}
		}
	}
}
