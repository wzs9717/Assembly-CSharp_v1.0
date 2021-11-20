using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateEmbeddedSceneSelectorUI : GenerateTabbedUI
{
	public JSONEmbed[] embeddedScenes;

	protected Dictionary<string, JSONEmbed> sceneNameToJSONEmbed;

	private void OnClick(string sceneName)
	{
		if (sceneNameToJSONEmbed != null && sceneNameToJSONEmbed.TryGetValue(sceneName, out var value))
		{
			SuperController.singleton.LoadFromJSONEmbed(value);
		}
	}

	protected override Transform InstantiateControl(Transform parent, int index)
	{
		Transform transform = base.InstantiateControl(parent, index);
		JSONEmbed jSONEmbed = embeddedScenes[index];
		string text = jSONEmbed.name;
		Button component = transform.GetComponent<Button>();
		if (component != null)
		{
			string cname = text;
			component.onClick.AddListener(delegate
			{
				OnClick(cname);
			});
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
			if (item.name == "Image")
			{
				Image component3 = item.GetComponent<Image>();
				if (component3 != null)
				{
					component3.sprite = jSONEmbed.sprite;
				}
			}
		}
		return transform;
	}

	public override void Generate()
	{
		base.Generate();
		sceneNameToJSONEmbed = new Dictionary<string, JSONEmbed>();
		if (controlUIPrefab != null && tabUIPrefab != null && tabButtonUIPrefab != null && embeddedScenes != null)
		{
			for (int i = 0; i < embeddedScenes.Length; i++)
			{
				sceneNameToJSONEmbed.Add(embeddedScenes[i].name, embeddedScenes[i]);
				AllocateControl();
			}
		}
	}
}
