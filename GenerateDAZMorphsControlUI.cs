using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateDAZMorphsControlUI : GenerateTabbedUI
{
	protected List<string> enabledMorphNames;

	public string[] morphNames;

	public bool[] morphEnabled;

	[SerializeField]
	protected DAZMorphBank _morphBank1;

	[SerializeField]
	protected DAZMorphBank _morphBank2;

	protected Dictionary<string, string> morphDisplayNameToRealName;

	protected List<string> morphDisplayNames;

	protected Dictionary<string, Slider> morphDisplayNameToSlider;

	public DAZMorphBank morphBank1
	{
		get
		{
			return _morphBank1;
		}
		set
		{
			if (_morphBank1 != value)
			{
				_morphBank1 = value;
				TabChange(currentTab.ToString(), on: true);
			}
		}
	}

	public DAZMorphBank morphBank2
	{
		get
		{
			return _morphBank2;
		}
		set
		{
			if (_morphBank2 != value)
			{
				_morphBank2 = value;
				TabChange(currentTab.ToString(), on: true);
			}
		}
	}

	public void SortMorphNames()
	{
		Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
		if (morphEnabled != null && morphEnabled.Length == morphNames.Length)
		{
			for (int i = 0; i < morphNames.Length; i++)
			{
				dictionary.Add(morphNames[i], morphEnabled[i]);
			}
		}
		Array.Sort(morphNames);
		morphEnabled = new bool[morphNames.Length];
		for (int j = 0; j < morphNames.Length; j++)
		{
			if (dictionary.TryGetValue(morphNames[j], out var value))
			{
				morphEnabled[j] = value;
			}
			else
			{
				morphEnabled[j] = true;
			}
		}
	}

	protected void InitMorphDisplayNames()
	{
		morphDisplayNames = new List<string>();
		morphDisplayNameToRealName = new Dictionary<string, string>();
		for (int i = 0; i < morphNames.Length; i++)
		{
			string text = morphNames[i];
			string[] array = text.Split(':');
			if (array.Length == 3)
			{
				string value = array[1];
				string text2 = array[2];
				morphDisplayNames.Add(text2);
				morphDisplayNameToRealName.Add(text2, value);
			}
		}
	}

	public List<string> GetMorphDisplayNames()
	{
		if (morphDisplayNames == null)
		{
			InitMorphDisplayNames();
		}
		return morphDisplayNames;
	}

	public bool IsMorphPoseControl(string morphDisplayName)
	{
		if (morphDisplayNameToRealName == null)
		{
			InitMorphDisplayNames();
		}
		if (morphDisplayNameToRealName.TryGetValue(morphDisplayName, out var value))
		{
			DAZMorph dAZMorph = null;
			if (_morphBank1 != null)
			{
				dAZMorph = _morphBank1.GetMorph(value);
			}
			if (dAZMorph == null && _morphBank2 != null)
			{
				dAZMorph = _morphBank2.GetMorph(value);
			}
			if (dAZMorph != null)
			{
				return dAZMorph.isPoseControl;
			}
		}
		return false;
	}

	public DAZMorph GetMorph(string morphRealName)
	{
		DAZMorph dAZMorph = null;
		if (_morphBank1 != null)
		{
			dAZMorph = _morphBank1.GetMorph(morphRealName);
		}
		if (dAZMorph == null && _morphBank2 != null)
		{
			dAZMorph = _morphBank2.GetMorph(morphRealName);
		}
		return dAZMorph;
	}

	public float GetMorphValue(string morphDisplayName)
	{
		if (morphDisplayNameToRealName == null)
		{
			InitMorphDisplayNames();
		}
		if (morphDisplayNameToRealName.TryGetValue(morphDisplayName, out var value))
		{
			DAZMorph dAZMorph = null;
			if (_morphBank1 != null)
			{
				dAZMorph = _morphBank1.GetMorph(value);
			}
			if (dAZMorph == null && _morphBank2 != null)
			{
				dAZMorph = _morphBank2.GetMorph(value);
			}
			if (dAZMorph != null)
			{
				return dAZMorph.morphValue;
			}
		}
		return 0f;
	}

	public void SetMorphValue(string morphDisplayName, float val)
	{
		if (morphDisplayNameToRealName == null)
		{
			InitMorphDisplayNames();
		}
		if (morphDisplayNameToRealName.TryGetValue(morphDisplayName, out var value))
		{
			DAZMorph dAZMorph = null;
			if (_morphBank1 != null)
			{
				dAZMorph = _morphBank1.GetMorph(value);
			}
			if (dAZMorph == null && _morphBank2 != null)
			{
				dAZMorph = _morphBank2.GetMorph(value);
			}
			if (dAZMorph != null)
			{
				dAZMorph.morphValue = val;
				SetMorphSliderValue(morphDisplayName, val);
			}
		}
	}

	public float GetMorphDefaultValue(string morphDisplayName)
	{
		if (morphDisplayNameToRealName == null)
		{
			InitMorphDisplayNames();
		}
		if (morphDisplayNameToRealName.TryGetValue(morphDisplayName, out var value))
		{
			DAZMorph dAZMorph = null;
			if (_morphBank1 != null)
			{
				dAZMorph = _morphBank1.GetMorph(value);
			}
			if (dAZMorph == null && _morphBank2 != null)
			{
				dAZMorph = _morphBank2.GetMorph(value);
			}
			if (dAZMorph != null)
			{
				return dAZMorph.startValue;
			}
		}
		return 0f;
	}

	public void ResetMorphs()
	{
		if (_morphBank1 != null)
		{
			_morphBank1.ResetMorphs();
		}
		if (_morphBank2 != null)
		{
			_morphBank2.ResetMorphs();
		}
	}

	public bool IsMorphBank1Morph(string morphRealName)
	{
		if (_morphBank1 != null)
		{
			DAZMorph morph = _morphBank1.GetMorph(morphRealName);
			if (morph != null)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsMorphBank2Morph(string morphRealName)
	{
		if (_morphBank2 != null)
		{
			DAZMorph morph = _morphBank2.GetMorph(morphRealName);
			if (morph != null)
			{
				return true;
			}
		}
		return false;
	}

	protected void SetMorphSliderValue(string morphDisplayName, float val)
	{
		if (morphDisplayNameToSlider != null && morphDisplayNameToSlider.TryGetValue(morphDisplayName, out var value))
		{
			value.value = val;
		}
	}

	public override void TabChange(string name, bool on)
	{
		morphDisplayNameToSlider = new Dictionary<string, Slider>();
		base.TabChange(name, on);
	}

	public void AutoGenerateMorphNames()
	{
		Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
		if (morphEnabled != null && morphEnabled.Length == morphNames.Length)
		{
			for (int i = 0; i < morphNames.Length; i++)
			{
				dictionary.Add(morphNames[i], morphEnabled[i]);
			}
		}
		List<string> list = new List<string>();
		if (_morphBank1 != null)
		{
			foreach (DAZMorph morph in _morphBank1.morphs)
			{
				if (morph.visible)
				{
					list.Add(morph.region + ":" + morph.morphName + ":" + morph.displayName);
				}
			}
		}
		if (_morphBank2 != null)
		{
			foreach (DAZMorph morph2 in _morphBank2.morphs)
			{
				if (morph2.visible)
				{
					list.Add(morph2.region + ":" + morph2.morphName + ":" + morph2.displayName);
				}
			}
		}
		morphNames = list.ToArray();
		morphEnabled = new bool[morphNames.Length];
		for (int j = 0; j < morphNames.Length; j++)
		{
			if (dictionary.TryGetValue(morphNames[j], out var value))
			{
				morphEnabled[j] = value;
			}
			else
			{
				morphEnabled[j] = true;
			}
		}
		SortMorphNames();
	}

	private void SetMorphUIControls(Transform morphUI, string morphName, string morphDisplayName)
	{
		SetDAZMorphFromUISlider component = morphUI.GetComponent<SetDAZMorphFromUISlider>();
		if (component != null)
		{
			component.morph1Name = morphName;
			if (IsMorphBank1Morph(morphName))
			{
				component.morphBank = _morphBank1;
			}
			else if (IsMorphBank2Morph(morphName))
			{
				component.morphBank = _morphBank2;
			}
		}
		foreach (Transform item in morphUI)
		{
			if (item.name == "Text")
			{
				Text component2 = item.GetComponent<Text>();
				if (component2 != null)
				{
					component2.text = morphDisplayName;
				}
			}
		}
		Slider[] componentsInChildren = morphUI.GetComponentsInChildren<Slider>(includeInactive: true);
		SliderControl[] componentsInChildren2 = morphUI.GetComponentsInChildren<SliderControl>(includeInactive: true);
		if (componentsInChildren != null && componentsInChildren2 != null)
		{
			Slider slider = componentsInChildren[0];
			SliderControl sliderControl = componentsInChildren2[0];
			morphDisplayNameToSlider.Add(morphDisplayName, slider);
			DAZMorph morph = GetMorph(morphName);
			if (morph != null)
			{
				slider.minValue = morph.min;
				slider.maxValue = morph.max;
				slider.value = morph.morphValue;
				sliderControl.defaultValue = morph.startValue;
			}
		}
	}

	protected override Transform InstantiateControl(Transform parent, int index)
	{
		Transform transform = base.InstantiateControl(parent, index);
		string text = enabledMorphNames[index];
		string[] array = text.Split(':');
		if (array.Length == 3)
		{
			string morphName = array[1];
			string morphDisplayName = array[2];
			SetMorphUIControls(transform, morphName, morphDisplayName);
		}
		else
		{
			Debug.LogError("morphName format should be morphRealName:morphDisplayName");
		}
		return transform;
	}

	public override void Generate()
	{
		base.Generate();
		InitMorphDisplayNames();
		enabledMorphNames = new List<string>();
		for (int i = 0; i < morphNames.Length; i++)
		{
			if (morphEnabled[i])
			{
				enabledMorphNames.Add(morphNames[i]);
			}
		}
		if (controlUIPrefab != null && tabUIPrefab != null && tabButtonUIPrefab != null)
		{
			for (int j = 0; j < enabledMorphNames.Count; j++)
			{
				AllocateControl();
			}
		}
	}
}
