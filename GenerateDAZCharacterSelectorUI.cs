using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateDAZCharacterSelectorUI : GenerateTabbedUI
{
	public DAZCharacterSelector characterSelector;

	protected List<string> enabledCharacterNames;

	[HideInInspector]
	public string[] characterNames;

	[HideInInspector]
	public bool[] characterEnabled;

	protected Dictionary<string, Toggle> characterNameToToggle;

	private void OnClick(string characterName)
	{
		if (characterSelector != null)
		{
			characterSelector.selectCharacterByName(characterName);
		}
	}

	public void SetActiveCharacterToggle(string characterName)
	{
		if (enabledCharacterNames == null || characterNameToToggle == null)
		{
			return;
		}
		for (int i = 0; i < enabledCharacterNames.Count; i++)
		{
			string text = enabledCharacterNames[i];
			if (characterNameToToggle.TryGetValue(text, out var value))
			{
				if (text == characterName)
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
		characterNameToToggle = new Dictionary<string, Toggle>();
		base.TabChange(name, on);
	}

	protected override Transform InstantiateControl(Transform parent, int index)
	{
		Transform transform = base.InstantiateControl(parent, index);
		string text = enabledCharacterNames[index];
		Toggle component = transform.GetComponent<Toggle>();
		if (component != null)
		{
			string cname = text;
			characterNameToToggle.Add(cname, component);
			component.onValueChanged.AddListener(delegate(bool arg0)
			{
				if (arg0)
				{
					OnClick(cname);
				}
			});
			if (characterSelector.selectedCharacter != null && characterSelector.selectedCharacter.displayName == text)
			{
				component.isOn = true;
			}
			else
			{
				component.isOn = false;
			}
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
		if (characterEnabled != null && characterEnabled.Length == characterNames.Length)
		{
			for (int i = 0; i < characterNames.Length; i++)
			{
				dictionary.Add(characterNames[i], characterEnabled[i]);
			}
		}
		List<string> list = new List<string>();
		if (characterSelector != null)
		{
			DAZCharacter[] characters = characterSelector.characters;
			foreach (DAZCharacter dAZCharacter in characters)
			{
				list.Add(dAZCharacter.displayName);
			}
		}
		characterNames = list.ToArray();
		characterEnabled = new bool[characterNames.Length];
		for (int k = 0; k < characterNames.Length; k++)
		{
			if (dictionary.TryGetValue(characterNames[k], out var value))
			{
				characterEnabled[k] = value;
			}
			else
			{
				characterEnabled[k] = true;
			}
		}
	}

	public override void Generate()
	{
		enabledCharacterNames = new List<string>();
		for (int i = 0; i < characterNames.Length; i++)
		{
			if (characterEnabled[i])
			{
				enabledCharacterNames.Add(characterNames[i]);
			}
		}
		base.Generate();
		characterNameToToggle = new Dictionary<string, Toggle>();
		if (controlUIPrefab != null && tabUIPrefab != null && tabButtonUIPrefab != null && characterNames != null)
		{
			for (int j = 0; j < enabledCharacterNames.Count; j++)
			{
				AllocateControl();
			}
		}
	}
}
