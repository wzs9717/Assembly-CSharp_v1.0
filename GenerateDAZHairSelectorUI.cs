using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateDAZHairSelectorUI : GenerateTabbedUI
{
	public DAZCharacterSelector characterSelector;

	protected List<string> enabledHairNames;

	[HideInInspector]
	public string[] hairNames;

	[HideInInspector]
	public bool[] hairEnabled;

	protected Dictionary<string, Toggle> hairNameToToggle;

	public void SetActiveHairToggle(string hairName)
	{
		if (enabledHairNames == null || hairNameToToggle == null)
		{
			return;
		}
		for (int i = 0; i < enabledHairNames.Count; i++)
		{
			string text = enabledHairNames[i];
			if (hairNameToToggle.TryGetValue(text, out var value))
			{
				if (text == hairName)
				{
					value.isOn = true;
				}
				else
				{
					value.isOn = false;
				}
			}
		}
	}

	public override void TabChange(string name, bool on)
	{
		hairNameToToggle = new Dictionary<string, Toggle>();
		base.TabChange(name, on);
	}

	protected override Transform InstantiateControl(Transform parent, int index)
	{
		Transform transform = base.InstantiateControl(parent, index);
		string text = enabledHairNames[index];
		Toggle component = transform.GetComponent<Toggle>();
		if (component != null)
		{
			string cname = text;
			hairNameToToggle.Add(cname, component);
			component.onValueChanged.AddListener(delegate(bool arg0)
			{
				if (characterSelector != null && arg0)
				{
					characterSelector.SetSelectedHairGroup(cname);
				}
			});
			component.isOn = characterSelector.selectedHairGroup.displayName == text;
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
		if (hairEnabled != null && hairEnabled.Length == hairNames.Length)
		{
			for (int i = 0; i < hairNames.Length; i++)
			{
				dictionary.Add(hairNames[i], hairEnabled[i]);
			}
		}
		List<string> list = new List<string>();
		if (characterSelector != null)
		{
			DAZHairGroup[] hairGroups = characterSelector.hairGroups;
			foreach (DAZHairGroup dAZHairGroup in hairGroups)
			{
				list.Add(dAZHairGroup.displayName);
			}
		}
		hairNames = list.ToArray();
		hairEnabled = new bool[hairNames.Length];
		for (int k = 0; k < hairNames.Length; k++)
		{
			if (dictionary.TryGetValue(hairNames[k], out var value))
			{
				hairEnabled[k] = value;
			}
			else
			{
				hairEnabled[k] = true;
			}
		}
	}

	public override void Generate()
	{
		enabledHairNames = new List<string>();
		for (int i = 0; i < hairNames.Length; i++)
		{
			if (hairEnabled[i])
			{
				enabledHairNames.Add(hairNames[i]);
			}
		}
		base.Generate();
		hairNameToToggle = new Dictionary<string, Toggle>();
		if (controlUIPrefab != null && tabUIPrefab != null && tabButtonUIPrefab != null)
		{
			for (int j = 0; j < enabledHairNames.Count; j++)
			{
				AllocateControl();
			}
		}
	}
}
