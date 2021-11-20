using System;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class MaterialOptions : JSONStorable
{
	[HideInInspector]
	public Transform copyFromTransform;

	[HideInInspector]
	public string copyFromId;

	[HideInInspector]
	public int copyFromTextureGroup = 1;

	[HideInInspector]
	public int copyToTextureGroup = 1;

	public MaterialOptions copyUIFrom;

	public Transform materialContainer;

	public Material materialForDefaults;

	public int[] paramMaterialSlots;

	public string color1Name;

	public string color1DisplayName;

	public Text color1DisplayNameText;

	public Color color1CurrentColor;

	public HSVColor color1CurrentHSVColor;

	public HSVColor color1DefaultHSVColor;

	public HSVColorPicker color1Picker;

	public RectTransform color1Container;

	public string color2Name;

	public string color2DisplayName;

	public Text color2DisplayNameText;

	public Color color2CurrentColor;

	public HSVColor color2CurrentHSVColor;

	public HSVColor color2DefaultHSVColor;

	public HSVColorPicker color2Picker;

	public RectTransform color2Container;

	public string color3Name;

	public string color3DisplayName;

	public Text color3DisplayNameText;

	public Color color3CurrentColor;

	public HSVColor color3CurrentHSVColor;

	public HSVColor color3DefaultHSVColor;

	public HSVColorPicker color3Picker;

	public RectTransform color3Container;

	public string param1Name;

	public string param1DisplayName;

	public Text param1DisplayNameText;

	public Text param1DisplayNameTextAlt;

	public float param1CurrentValue;

	public float param1DefaultValue;

	public float param1MinValue;

	public float param1MaxValue = 1f;

	public Slider param1Slider;

	public Slider param1SliderAlt;

	public string param2Name;

	public string param2DisplayName;

	public Text param2DisplayNameText;

	public Text param2DisplayNameTextAlt;

	public float param2CurrentValue;

	public float param2DefaultValue;

	public float param2MinValue;

	public float param2MaxValue = 1f;

	public Slider param2Slider;

	public Slider param2SliderAlt;

	public string param3Name;

	public string param3DisplayName;

	public Text param3DisplayNameText;

	public Text param3DisplayNameTextAlt;

	public float param3CurrentValue;

	public float param3DefaultValue;

	public float param3MinValue;

	public float param3MaxValue = 1f;

	public Slider param3Slider;

	public Slider param3SliderAlt;

	public string param4Name;

	public string param4DisplayName;

	public Text param4DisplayNameText;

	public Text param4DisplayNameTextAlt;

	public float param4CurrentValue;

	public float param4DefaultValue;

	public float param4MinValue;

	public float param4MaxValue = 1f;

	public Slider param4Slider;

	public Slider param4SliderAlt;

	public string param5Name;

	public string param5DisplayName;

	public Text param5DisplayNameText;

	public Text param5DisplayNameTextAlt;

	public float param5CurrentValue;

	public float param5DefaultValue;

	public float param5MinValue;

	public float param5MaxValue = 1f;

	public Slider param5Slider;

	public Slider param5SliderAlt;

	public string param6Name;

	public string param6DisplayName;

	public Text param6DisplayNameText;

	public Text param6DisplayNameTextAlt;

	public float param6CurrentValue;

	public float param6DefaultValue;

	public float param6MinValue;

	public float param6MaxValue = 1f;

	public Slider param6Slider;

	public Slider param6SliderAlt;

	public string param7Name;

	public string param7DisplayName;

	public Text param7DisplayNameText;

	public Text param7DisplayNameTextAlt;

	public float param7CurrentValue;

	public float param7DefaultValue;

	public float param7MinValue;

	public float param7MaxValue = 1f;

	public Slider param7Slider;

	public Slider param7SliderAlt;

	public string param8Name;

	public string param8DisplayName;

	public Text param8DisplayNameText;

	public Text param8DisplayNameTextAlt;

	public float param8CurrentValue;

	public float param8DefaultValue;

	public float param8MinValue;

	public float param8MaxValue = 1f;

	public Slider param8Slider;

	public Slider param8SliderAlt;

	public MaterialOptionTextureGroup textureGroup1;

	public UIPopup textureGroup1Popup;

	public UIPopup textureGroup1PopupAlt;

	public Text textureGroup1Text;

	public Text textureGroup1TextAlt;

	public string startingTextureGroup1Set;

	public string currentTextureGroup1Set;

	public MaterialOptionTextureGroup textureGroup2;

	public UIPopup textureGroup2Popup;

	public UIPopup textureGroup2PopupAlt;

	public Text textureGroup2Text;

	public Text textureGroup2TextAlt;

	public string startingTextureGroup2Set;

	public string currentTextureGroup2Set;

	public MaterialOptionTextureGroup textureGroup3;

	public UIPopup textureGroup3Popup;

	public UIPopup textureGroup3PopupAlt;

	public Text textureGroup3Text;

	public Text textureGroup3TextAlt;

	public string startingTextureGroup3Set;

	public string currentTextureGroup3Set;

	public MaterialOptionTextureGroup textureGroup4;

	public UIPopup textureGroup4Popup;

	public UIPopup textureGroup4PopupAlt;

	public Text textureGroup4Text;

	public Text textureGroup4TextAlt;

	public string startingTextureGroup4Set;

	public string currentTextureGroup4Set;

	public MaterialOptionTextureGroup textureGroup5;

	public UIPopup textureGroup5Popup;

	public UIPopup textureGroup5PopupAlt;

	public Text textureGroup5Text;

	public Text textureGroup5TextAlt;

	public string startingTextureGroup5Set;

	public string currentTextureGroup5Set;

	protected MeshRenderer[] meshRenderers;

	protected bool wasInit;

	public override JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true)
	{
		JSONClass jSON = base.GetJSON(includePhysical, includeAppearance);
		if (includeAppearance)
		{
			if (color1Picker != null && color1DisplayName != null && color1DisplayName != string.Empty && (color1Picker.hue != color1Picker.defaultHue || color1Picker.saturation != color1Picker.defaultSaturation || color1Picker.cvalue != color1Picker.defaultCvalue))
			{
				needsStore = true;
				jSON[color1DisplayName]["h"].AsFloat = color1Picker.hue;
				jSON[color1DisplayName]["s"].AsFloat = color1Picker.saturation;
				jSON[color1DisplayName]["v"].AsFloat = color1Picker.cvalue;
			}
			if (color2Picker != null && color2DisplayName != null && color2DisplayName != string.Empty && (color2Picker.hue != color2Picker.defaultHue || color2Picker.saturation != color2Picker.defaultSaturation || color2Picker.cvalue != color2Picker.defaultCvalue))
			{
				needsStore = true;
				jSON[color2DisplayName]["h"].AsFloat = color2Picker.hue;
				jSON[color2DisplayName]["s"].AsFloat = color2Picker.saturation;
				jSON[color2DisplayName]["v"].AsFloat = color2Picker.cvalue;
			}
			if (color3Picker != null && color3DisplayName != null && color3DisplayName != string.Empty && (color3Picker.hue != color3Picker.defaultHue || color3Picker.saturation != color3Picker.defaultSaturation || color3Picker.cvalue != color3Picker.defaultCvalue))
			{
				needsStore = true;
				jSON[color3DisplayName]["h"].AsFloat = color3Picker.hue;
				jSON[color3DisplayName]["s"].AsFloat = color3Picker.saturation;
				jSON[color3DisplayName]["v"].AsFloat = color3Picker.cvalue;
			}
			if (param1Slider != null && param1DisplayName != null && param1DisplayName != string.Empty)
			{
				SliderControl component = param1Slider.GetComponent<SliderControl>();
				if (component == null || component.defaultValue != param1CurrentValue)
				{
					needsStore = true;
					jSON[param1DisplayName].AsFloat = param1CurrentValue;
				}
			}
			if (param2Slider != null && param2DisplayName != null && param2DisplayName != string.Empty)
			{
				SliderControl component2 = param2Slider.GetComponent<SliderControl>();
				if (component2 == null || component2.defaultValue != param2CurrentValue)
				{
					needsStore = true;
					jSON[param2DisplayName].AsFloat = param2CurrentValue;
				}
			}
			if (param3Slider != null && param3DisplayName != null && param3DisplayName != string.Empty)
			{
				SliderControl component3 = param3Slider.GetComponent<SliderControl>();
				if (component3 == null || component3.defaultValue != param3CurrentValue)
				{
					needsStore = true;
					jSON[param3DisplayName].AsFloat = param3CurrentValue;
				}
			}
			if (param4Slider != null && param4DisplayName != null && param4DisplayName != string.Empty)
			{
				SliderControl component4 = param4Slider.GetComponent<SliderControl>();
				if (component4 == null || component4.defaultValue != param4CurrentValue)
				{
					needsStore = true;
					jSON[param4DisplayName].AsFloat = param4CurrentValue;
				}
			}
			if (param5Slider != null && param5DisplayName != null && param5DisplayName != string.Empty)
			{
				SliderControl component5 = param5Slider.GetComponent<SliderControl>();
				if (component5 == null || component5.defaultValue != param5CurrentValue)
				{
					needsStore = true;
					jSON[param5DisplayName].AsFloat = param5CurrentValue;
				}
			}
			if (param6Slider != null && param6DisplayName != null && param6DisplayName != string.Empty)
			{
				SliderControl component6 = param6Slider.GetComponent<SliderControl>();
				if (component6 == null || component6.defaultValue != param6CurrentValue)
				{
					needsStore = true;
					jSON[param6DisplayName].AsFloat = param6CurrentValue;
				}
			}
			if (param7Slider != null && param7DisplayName != null && param7DisplayName != string.Empty)
			{
				SliderControl component7 = param7Slider.GetComponent<SliderControl>();
				if (component7 == null || component7.defaultValue != param7CurrentValue)
				{
					needsStore = true;
					jSON[param7DisplayName].AsFloat = param7CurrentValue;
				}
			}
			if (param8Slider != null && param8DisplayName != null && param8DisplayName != string.Empty)
			{
				SliderControl component8 = param8Slider.GetComponent<SliderControl>();
				if (component8 == null || component8.defaultValue != param8CurrentValue)
				{
					needsStore = true;
					jSON[param8DisplayName].AsFloat = param8CurrentValue;
				}
			}
			if (textureGroup1Popup != null && textureGroup1 != null && textureGroup1.name != null && textureGroup1.name != string.Empty)
			{
				needsStore = true;
				jSON[textureGroup1.name] = textureGroup1Popup.currentValue;
			}
			if (textureGroup2Popup != null && textureGroup2 != null && textureGroup2.name != null && textureGroup2.name != string.Empty)
			{
				needsStore = true;
				jSON[textureGroup2.name] = textureGroup2Popup.currentValue;
			}
			if (textureGroup3Popup != null && textureGroup3 != null && textureGroup3.name != null && textureGroup3.name != string.Empty)
			{
				needsStore = true;
				jSON[textureGroup3.name] = textureGroup3Popup.currentValue;
			}
			if (textureGroup4Popup != null && textureGroup4 != null && textureGroup4.name != null && textureGroup4.name != string.Empty)
			{
				needsStore = true;
				jSON[textureGroup4.name] = textureGroup4Popup.currentValue;
			}
			if (textureGroup5Popup != null && textureGroup5 != null && textureGroup5.name != null && textureGroup5.name != string.Empty)
			{
				needsStore = true;
				jSON[textureGroup5.name] = textureGroup5Popup.currentValue;
			}
		}
		return jSON;
	}

	public override void RestoreFromJSON(JSONClass jc, bool restorePhysical = true, bool restoreAppearance = true)
	{
		base.RestoreFromJSON(jc, restorePhysical, restoreAppearance);
		if (!restoreAppearance)
		{
			return;
		}
		bool activeInHierarchy = base.gameObject.activeInHierarchy;
		if (color1Picker != null && color1DisplayName != null && color1DisplayName != string.Empty)
		{
			if (activeInHierarchy)
			{
				if (jc[color1DisplayName] != null)
				{
					if (jc[color1DisplayName]["h"] != null)
					{
						color1Picker.hue = jc[color1DisplayName]["h"].AsFloat;
					}
					if (jc[color1DisplayName]["s"] != null)
					{
						color1Picker.saturation = jc[color1DisplayName]["s"].AsFloat;
					}
					if (jc[color1DisplayName]["v"] != null)
					{
						color1Picker.cvalue = jc[color1DisplayName]["v"].AsFloat;
					}
				}
				else
				{
					color1Picker.Reset();
				}
			}
			else if (jc[color1DisplayName] != null)
			{
				if (jc[color1DisplayName]["h"] != null)
				{
					color1CurrentHSVColor.H = jc[color1DisplayName]["h"].AsFloat;
				}
				if (jc[color1DisplayName]["s"] != null)
				{
					color1CurrentHSVColor.S = jc[color1DisplayName]["s"].AsFloat;
				}
				if (jc[color1DisplayName]["v"] != null)
				{
					color1CurrentHSVColor.V = jc[color1DisplayName]["v"].AsFloat;
				}
			}
			else if (wasInit)
			{
				color1CurrentHSVColor = color1DefaultHSVColor;
			}
		}
		if (color2Picker != null && color2DisplayName != null && color2DisplayName != string.Empty)
		{
			if (activeInHierarchy)
			{
				if (jc[color2DisplayName] != null)
				{
					if (jc[color2DisplayName]["h"] != null)
					{
						color2Picker.hue = jc[color2DisplayName]["h"].AsFloat;
					}
					if (jc[color2DisplayName]["s"] != null)
					{
						color2Picker.saturation = jc[color2DisplayName]["s"].AsFloat;
					}
					if (jc[color2DisplayName]["v"] != null)
					{
						color2Picker.cvalue = jc[color2DisplayName]["v"].AsFloat;
					}
				}
				else
				{
					color2Picker.Reset();
				}
			}
			else if (jc[color2DisplayName] != null)
			{
				if (jc[color2DisplayName]["h"] != null)
				{
					color2CurrentHSVColor.H = jc[color2DisplayName]["h"].AsFloat;
				}
				if (jc[color2DisplayName]["s"] != null)
				{
					color2CurrentHSVColor.S = jc[color2DisplayName]["s"].AsFloat;
				}
				if (jc[color2DisplayName]["v"] != null)
				{
					color2CurrentHSVColor.V = jc[color2DisplayName]["v"].AsFloat;
				}
			}
			else if (wasInit)
			{
				color2CurrentHSVColor = color2DefaultHSVColor;
			}
		}
		if (color3Picker != null && color3DisplayName != null && color3DisplayName != string.Empty)
		{
			if (activeInHierarchy)
			{
				if (jc[color3DisplayName] != null)
				{
					if (jc[color3DisplayName]["h"] != null)
					{
						color3Picker.hue = jc[color3DisplayName]["h"].AsFloat;
					}
					if (jc[color3DisplayName]["s"] != null)
					{
						color3Picker.saturation = jc[color3DisplayName]["s"].AsFloat;
					}
					if (jc[color3DisplayName]["v"] != null)
					{
						color3Picker.cvalue = jc[color3DisplayName]["v"].AsFloat;
					}
				}
				else
				{
					color3Picker.Reset();
				}
			}
			else if (jc[color3DisplayName] != null)
			{
				if (jc[color3DisplayName]["h"] != null)
				{
					color3CurrentHSVColor.H = jc[color3DisplayName]["h"].AsFloat;
				}
				if (jc[color3DisplayName]["s"] != null)
				{
					color3CurrentHSVColor.S = jc[color3DisplayName]["s"].AsFloat;
				}
				if (jc[color3DisplayName]["v"] != null)
				{
					color3CurrentHSVColor.V = jc[color3DisplayName]["v"].AsFloat;
				}
			}
			else if (wasInit)
			{
				color3CurrentHSVColor = color3DefaultHSVColor;
			}
		}
		if (param1Slider != null && param1DisplayName != null && param1DisplayName != string.Empty)
		{
			if (activeInHierarchy)
			{
				if (jc[param1DisplayName] != null)
				{
					SetParam1CurrentValueUpdateUI(jc[param1DisplayName].AsFloat);
				}
				else
				{
					SliderControl component = param1Slider.GetComponent<SliderControl>();
					if (component != null)
					{
						SetParam1CurrentValueUpdateUI(component.defaultValue);
					}
				}
			}
			else if (jc[param1DisplayName] != null)
			{
				SetParam1CurrentValue(jc[param1DisplayName].AsFloat);
			}
			else if (wasInit)
			{
				SetParam1CurrentValue(param1DefaultValue);
			}
		}
		if (param2Slider != null && param2DisplayName != null && param2DisplayName != string.Empty)
		{
			if (activeInHierarchy)
			{
				if (jc[param2DisplayName] != null)
				{
					SetParam2CurrentValueUpdateUI(jc[param2DisplayName].AsFloat);
				}
				else
				{
					SliderControl component2 = param2Slider.GetComponent<SliderControl>();
					if (component2 != null)
					{
						SetParam2CurrentValueUpdateUI(component2.defaultValue);
					}
				}
			}
			else if (jc[param2DisplayName] != null)
			{
				SetParam2CurrentValue(jc[param2DisplayName].AsFloat);
			}
			else if (wasInit)
			{
				SetParam2CurrentValue(param2DefaultValue);
			}
		}
		if (param3Slider != null && param3DisplayName != null && param3DisplayName != string.Empty)
		{
			if (activeInHierarchy)
			{
				if (jc[param3DisplayName] != null)
				{
					SetParam3CurrentValueUpdateUI(jc[param3DisplayName].AsFloat);
				}
				else
				{
					SliderControl component3 = param3Slider.GetComponent<SliderControl>();
					if (component3 != null)
					{
						SetParam3CurrentValueUpdateUI(component3.defaultValue);
					}
				}
			}
			else if (jc[param3DisplayName] != null)
			{
				SetParam3CurrentValue(jc[param3DisplayName].AsFloat);
			}
			else if (wasInit)
			{
				SetParam3CurrentValue(param3DefaultValue);
			}
		}
		if (param4Slider != null && param4DisplayName != null && param4DisplayName != string.Empty)
		{
			if (activeInHierarchy)
			{
				if (jc[param4DisplayName] != null)
				{
					SetParam4CurrentValueUpdateUI(jc[param4DisplayName].AsFloat);
				}
				else
				{
					SliderControl component4 = param4Slider.GetComponent<SliderControl>();
					if (component4 != null)
					{
						SetParam4CurrentValueUpdateUI(component4.defaultValue);
					}
				}
			}
			else if (jc[param4DisplayName] != null)
			{
				SetParam4CurrentValue(jc[param4DisplayName].AsFloat);
			}
			else if (wasInit)
			{
				SetParam4CurrentValue(param4DefaultValue);
			}
		}
		if (param5Slider != null && param5DisplayName != null && param5DisplayName != string.Empty)
		{
			if (activeInHierarchy)
			{
				if (jc[param5DisplayName] != null)
				{
					SetParam5CurrentValueUpdateUI(jc[param5DisplayName].AsFloat);
				}
				else
				{
					SliderControl component5 = param5Slider.GetComponent<SliderControl>();
					if (component5 != null)
					{
						SetParam5CurrentValueUpdateUI(component5.defaultValue);
					}
				}
			}
			else if (jc[param5DisplayName] != null)
			{
				SetParam5CurrentValue(jc[param5DisplayName].AsFloat);
			}
			else if (wasInit)
			{
				SetParam5CurrentValue(param5DefaultValue);
			}
		}
		if (param6Slider != null && param6DisplayName != null && param6DisplayName != string.Empty)
		{
			if (activeInHierarchy)
			{
				if (jc[param6DisplayName] != null)
				{
					SetParam6CurrentValueUpdateUI(jc[param6DisplayName].AsFloat);
				}
				else
				{
					SliderControl component6 = param6Slider.GetComponent<SliderControl>();
					if (component6 != null)
					{
						SetParam6CurrentValueUpdateUI(component6.defaultValue);
					}
				}
			}
			else if (jc[param6DisplayName] != null)
			{
				SetParam6CurrentValue(jc[param6DisplayName].AsFloat);
			}
			else if (wasInit)
			{
				SetParam6CurrentValue(param6DefaultValue);
			}
		}
		if (param7Slider != null && param7DisplayName != null && param7DisplayName != string.Empty)
		{
			if (activeInHierarchy)
			{
				if (jc[param7DisplayName] != null)
				{
					SetParam7CurrentValueUpdateUI(jc[param7DisplayName].AsFloat);
				}
				else
				{
					SliderControl component7 = param7Slider.GetComponent<SliderControl>();
					if (component7 != null)
					{
						SetParam7CurrentValueUpdateUI(component7.defaultValue);
					}
				}
			}
			else if (jc[param7DisplayName] != null)
			{
				SetParam7CurrentValue(jc[param7DisplayName].AsFloat);
			}
			else if (wasInit)
			{
				SetParam7CurrentValue(param7DefaultValue);
			}
		}
		if (param8Slider != null && param8DisplayName != null && param8DisplayName != string.Empty)
		{
			if (activeInHierarchy)
			{
				if (jc[param8DisplayName] != null)
				{
					SetParam8CurrentValueUpdateUI(jc[param8DisplayName].AsFloat);
				}
				else
				{
					SliderControl component8 = param8Slider.GetComponent<SliderControl>();
					if (component8 != null)
					{
						SetParam8CurrentValueUpdateUI(component8.defaultValue);
					}
				}
			}
			else if (jc[param8DisplayName] != null)
			{
				SetParam8CurrentValue(jc[param8DisplayName].AsFloat);
			}
			else if (wasInit)
			{
				SetParam8CurrentValue(param8DefaultValue);
			}
		}
		if (textureGroup1Popup != null && textureGroup1 != null && textureGroup1.name != null && textureGroup1.name != string.Empty)
		{
			if (activeInHierarchy)
			{
				if (jc[textureGroup1.name] != null)
				{
					SetTextureGroup1SetUpdateUI(jc[textureGroup1.name]);
				}
				else if (textureGroup1.sets != null && textureGroup1.sets.Length > 0)
				{
					if (startingTextureGroup1Set != null && startingTextureGroup1Set != string.Empty)
					{
						SetTextureGroup1SetUpdateUI(startingTextureGroup1Set);
					}
					else
					{
						SetTextureGroup1SetUpdateUI(textureGroup1.sets[0].name);
					}
				}
			}
			else if (jc[textureGroup1.name] != null)
			{
				SetTextureGroup1Set(jc[textureGroup1.name]);
			}
			else if (wasInit)
			{
				SetTextureGroup1Set(startingTextureGroup1Set);
			}
		}
		if (textureGroup2Popup != null && textureGroup2 != null && textureGroup2.name != null && textureGroup2.name != string.Empty)
		{
			if (activeInHierarchy)
			{
				if (jc[textureGroup2.name] != null)
				{
					SetTextureGroup2SetUpdateUI(jc[textureGroup2.name]);
				}
				else if (textureGroup2.sets != null && textureGroup2.sets.Length > 0)
				{
					if (startingTextureGroup2Set != null && startingTextureGroup2Set != string.Empty)
					{
						SetTextureGroup2SetUpdateUI(startingTextureGroup2Set);
					}
					else
					{
						SetTextureGroup2SetUpdateUI(textureGroup2.sets[0].name);
					}
				}
			}
			else if (jc[textureGroup2.name] != null)
			{
				SetTextureGroup2Set(jc[textureGroup2.name]);
			}
			else if (wasInit)
			{
				SetTextureGroup2Set(startingTextureGroup2Set);
			}
		}
		if (textureGroup3Popup != null && textureGroup3 != null && textureGroup3.name != null && textureGroup3.name != string.Empty)
		{
			if (activeInHierarchy)
			{
				if (jc[textureGroup3.name] != null)
				{
					SetTextureGroup3SetUpdateUI(jc[textureGroup3.name]);
				}
				else if (textureGroup3.sets != null && textureGroup3.sets.Length > 0)
				{
					if (startingTextureGroup3Set != null && startingTextureGroup3Set != string.Empty)
					{
						SetTextureGroup3SetUpdateUI(startingTextureGroup3Set);
					}
					else
					{
						SetTextureGroup3SetUpdateUI(textureGroup3.sets[0].name);
					}
				}
			}
			else if (jc[textureGroup3.name] != null)
			{
				SetTextureGroup3Set(jc[textureGroup3.name]);
			}
			else if (wasInit)
			{
				SetTextureGroup3Set(startingTextureGroup3Set);
			}
		}
		if (textureGroup4Popup != null && textureGroup4 != null && textureGroup4.name != null && textureGroup4.name != string.Empty)
		{
			if (activeInHierarchy)
			{
				if (jc[textureGroup4.name] != null)
				{
					SetTextureGroup4SetUpdateUI(jc[textureGroup4.name]);
				}
				else if (textureGroup4.sets != null && textureGroup4.sets.Length > 0)
				{
					if (startingTextureGroup4Set != null && startingTextureGroup4Set != string.Empty)
					{
						SetTextureGroup4SetUpdateUI(startingTextureGroup4Set);
					}
					else
					{
						SetTextureGroup4SetUpdateUI(textureGroup4.sets[0].name);
					}
				}
			}
			else if (jc[textureGroup4.name] != null)
			{
				SetTextureGroup4Set(jc[textureGroup4.name]);
			}
			else if (wasInit)
			{
				SetTextureGroup4Set(startingTextureGroup4Set);
			}
		}
		if (!(textureGroup5Popup != null) || textureGroup5 == null || textureGroup5.name == null || !(textureGroup5.name != string.Empty))
		{
			return;
		}
		if (activeInHierarchy)
		{
			if (jc[textureGroup5.name] != null)
			{
				SetTextureGroup5SetUpdateUI(jc[textureGroup5.name]);
			}
			else if (textureGroup5.sets != null && textureGroup5.sets.Length > 0)
			{
				if (startingTextureGroup5Set != null && startingTextureGroup5Set != string.Empty)
				{
					SetTextureGroup5SetUpdateUI(startingTextureGroup5Set);
				}
				else
				{
					SetTextureGroup5SetUpdateUI(textureGroup5.sets[0].name);
				}
			}
		}
		else if (jc[textureGroup5.name] != null)
		{
			SetTextureGroup5Set(jc[textureGroup5.name]);
		}
		else if (wasInit)
		{
			SetTextureGroup5Set(startingTextureGroup5Set);
		}
	}

	public void CopyUI()
	{
		if (copyUIFrom != null)
		{
			color1DisplayNameText = copyUIFrom.color1DisplayNameText;
			color1Picker = copyUIFrom.color1Picker;
			color1Container = copyUIFrom.color1Container;
			color2DisplayNameText = copyUIFrom.color2DisplayNameText;
			color2Picker = copyUIFrom.color2Picker;
			color2Container = copyUIFrom.color2Container;
			color3DisplayNameText = copyUIFrom.color3DisplayNameText;
			color3Picker = copyUIFrom.color3Picker;
			color3Container = copyUIFrom.color3Container;
			param1DisplayNameText = copyUIFrom.param1DisplayNameText;
			param1DisplayNameTextAlt = copyUIFrom.param1DisplayNameTextAlt;
			param1Slider = copyUIFrom.param1Slider;
			param1SliderAlt = copyUIFrom.param1SliderAlt;
			param2DisplayNameText = copyUIFrom.param2DisplayNameText;
			param2DisplayNameTextAlt = copyUIFrom.param2DisplayNameTextAlt;
			param2Slider = copyUIFrom.param2Slider;
			param2SliderAlt = copyUIFrom.param2SliderAlt;
			param3DisplayNameText = copyUIFrom.param3DisplayNameText;
			param3DisplayNameTextAlt = copyUIFrom.param3DisplayNameTextAlt;
			param3Slider = copyUIFrom.param3Slider;
			param3SliderAlt = copyUIFrom.param3SliderAlt;
			param4DisplayNameText = copyUIFrom.param4DisplayNameText;
			param4DisplayNameTextAlt = copyUIFrom.param4DisplayNameTextAlt;
			param4Slider = copyUIFrom.param4Slider;
			param4SliderAlt = copyUIFrom.param4SliderAlt;
			param5DisplayNameText = copyUIFrom.param5DisplayNameText;
			param5DisplayNameTextAlt = copyUIFrom.param5DisplayNameTextAlt;
			param5Slider = copyUIFrom.param5Slider;
			param5SliderAlt = copyUIFrom.param5SliderAlt;
			param6DisplayNameText = copyUIFrom.param6DisplayNameText;
			param6DisplayNameTextAlt = copyUIFrom.param6DisplayNameTextAlt;
			param6Slider = copyUIFrom.param6Slider;
			param6SliderAlt = copyUIFrom.param6SliderAlt;
			param7DisplayNameText = copyUIFrom.param7DisplayNameText;
			param7DisplayNameTextAlt = copyUIFrom.param7DisplayNameTextAlt;
			param7Slider = copyUIFrom.param7Slider;
			param7SliderAlt = copyUIFrom.param7SliderAlt;
			param8DisplayNameText = copyUIFrom.param8DisplayNameText;
			param8DisplayNameTextAlt = copyUIFrom.param8DisplayNameTextAlt;
			param8Slider = copyUIFrom.param8Slider;
			param8SliderAlt = copyUIFrom.param8SliderAlt;
			textureGroup1Popup = copyUIFrom.textureGroup1Popup;
			textureGroup1PopupAlt = copyUIFrom.textureGroup1PopupAlt;
			textureGroup2Popup = copyUIFrom.textureGroup2Popup;
			textureGroup2PopupAlt = copyUIFrom.textureGroup2PopupAlt;
			textureGroup3Popup = copyUIFrom.textureGroup3Popup;
			textureGroup3PopupAlt = copyUIFrom.textureGroup3PopupAlt;
			textureGroup4Popup = copyUIFrom.textureGroup4Popup;
			textureGroup4PopupAlt = copyUIFrom.textureGroup4PopupAlt;
			textureGroup5Popup = copyUIFrom.textureGroup5Popup;
			textureGroup5PopupAlt = copyUIFrom.textureGroup5PopupAlt;
			textureGroup1Text = copyUIFrom.textureGroup1Text;
			textureGroup1TextAlt = copyUIFrom.textureGroup1TextAlt;
			textureGroup2Text = copyUIFrom.textureGroup2Text;
			textureGroup2TextAlt = copyUIFrom.textureGroup2TextAlt;
			textureGroup3Text = copyUIFrom.textureGroup3Text;
			textureGroup3TextAlt = copyUIFrom.textureGroup3TextAlt;
			textureGroup4Text = copyUIFrom.textureGroup4Text;
			textureGroup4TextAlt = copyUIFrom.textureGroup4TextAlt;
			textureGroup5Text = copyUIFrom.textureGroup5Text;
			textureGroup5TextAlt = copyUIFrom.textureGroup5TextAlt;
		}
	}

	public void CopyParams()
	{
		if (copyUIFrom != null)
		{
			paramMaterialSlots = (int[])copyUIFrom.paramMaterialSlots.Clone();
			color1Name = copyUIFrom.color1Name;
			color1DisplayName = copyUIFrom.color1DisplayName;
			color2Name = copyUIFrom.color2Name;
			color2DisplayName = copyUIFrom.color2DisplayName;
			color3Name = copyUIFrom.color3Name;
			color3DisplayName = copyUIFrom.color3DisplayName;
			param1Name = copyUIFrom.param1Name;
			param1DisplayName = copyUIFrom.param1DisplayName;
			param1MaxValue = copyUIFrom.param1MaxValue;
			param1MinValue = copyUIFrom.param1MinValue;
			param2Name = copyUIFrom.param2Name;
			param2DisplayName = copyUIFrom.param2DisplayName;
			param2MaxValue = copyUIFrom.param2MaxValue;
			param2MinValue = copyUIFrom.param2MinValue;
			param3Name = copyUIFrom.param3Name;
			param3DisplayName = copyUIFrom.param3DisplayName;
			param3MaxValue = copyUIFrom.param3MaxValue;
			param3MinValue = copyUIFrom.param3MinValue;
			param4Name = copyUIFrom.param4Name;
			param4DisplayName = copyUIFrom.param4DisplayName;
			param4MaxValue = copyUIFrom.param4MaxValue;
			param4MinValue = copyUIFrom.param4MinValue;
			param5Name = copyUIFrom.param5Name;
			param5DisplayName = copyUIFrom.param5DisplayName;
			param5MaxValue = copyUIFrom.param5MaxValue;
			param5MinValue = copyUIFrom.param5MinValue;
			param6Name = copyUIFrom.param6Name;
			param6DisplayName = copyUIFrom.param6DisplayName;
			param6MaxValue = copyUIFrom.param6MaxValue;
			param6MinValue = copyUIFrom.param6MinValue;
			param7Name = copyUIFrom.param7Name;
			param7DisplayName = copyUIFrom.param7DisplayName;
			param7MaxValue = copyUIFrom.param7MaxValue;
			param7MinValue = copyUIFrom.param7MinValue;
			param8Name = copyUIFrom.param8Name;
			param8DisplayName = copyUIFrom.param8DisplayName;
			param8MaxValue = copyUIFrom.param8MaxValue;
			param8MinValue = copyUIFrom.param8MinValue;
		}
	}

	public void CopyTextureGroup()
	{
		if (!(copyUIFrom != null))
		{
			return;
		}
		MaterialOptionTextureGroup materialOptionTextureGroup;
		switch (copyFromTextureGroup)
		{
		default:
			return;
		case 1:
			materialOptionTextureGroup = copyUIFrom.textureGroup1;
			break;
		case 2:
			materialOptionTextureGroup = copyUIFrom.textureGroup2;
			break;
		case 3:
			materialOptionTextureGroup = copyUIFrom.textureGroup3;
			break;
		case 4:
			materialOptionTextureGroup = copyUIFrom.textureGroup4;
			break;
		case 5:
			materialOptionTextureGroup = copyUIFrom.textureGroup5;
			break;
		}
		MaterialOptionTextureGroup materialOptionTextureGroup2;
		switch (copyToTextureGroup)
		{
		default:
			return;
		case 1:
			materialOptionTextureGroup2 = textureGroup1;
			break;
		case 2:
			materialOptionTextureGroup2 = textureGroup2;
			break;
		case 3:
			materialOptionTextureGroup2 = textureGroup3;
			break;
		case 4:
			materialOptionTextureGroup2 = textureGroup4;
			break;
		case 5:
			materialOptionTextureGroup2 = textureGroup5;
			break;
		}
		if (materialOptionTextureGroup != null && materialOptionTextureGroup2 != null)
		{
			materialOptionTextureGroup2.name = materialOptionTextureGroup.name;
			materialOptionTextureGroup2.textureName = materialOptionTextureGroup.textureName;
			materialOptionTextureGroup2.secondaryTextureName = materialOptionTextureGroup.secondaryTextureName;
			materialOptionTextureGroup2.autoCreateDefaultTexture = materialOptionTextureGroup.autoCreateDefaultTexture;
			materialOptionTextureGroup2.autoCreateDefaultSetName = materialOptionTextureGroup.autoCreateDefaultSetName;
			materialOptionTextureGroup2.autoCreateTextureFilePrefix = materialOptionTextureGroup.autoCreateTextureFilePrefix;
			materialOptionTextureGroup2.autoCreateSetNamePrefix = materialOptionTextureGroup.autoCreateSetNamePrefix;
			materialOptionTextureGroup2.materialSlots = (int[])materialOptionTextureGroup.materialSlots.Clone();
			materialOptionTextureGroup2.sets = new MaterialOptionTextureSet[materialOptionTextureGroup.sets.Length];
			for (int i = 0; i < materialOptionTextureGroup.sets.Length; i++)
			{
				materialOptionTextureGroup2.sets[i] = new MaterialOptionTextureSet();
				materialOptionTextureGroup2.sets[i].name = materialOptionTextureGroup.sets[i].name;
				materialOptionTextureGroup2.sets[i].textures = (Texture[])materialOptionTextureGroup.sets[i].textures.Clone();
			}
		}
	}

	public void MergeTextureGroup()
	{
		if (!(copyUIFrom != null))
		{
			return;
		}
		MaterialOptionTextureGroup materialOptionTextureGroup;
		switch (copyFromTextureGroup)
		{
		default:
			return;
		case 1:
			materialOptionTextureGroup = copyUIFrom.textureGroup1;
			break;
		case 2:
			materialOptionTextureGroup = copyUIFrom.textureGroup2;
			break;
		case 3:
			materialOptionTextureGroup = copyUIFrom.textureGroup3;
			break;
		case 4:
			materialOptionTextureGroup = copyUIFrom.textureGroup4;
			break;
		case 5:
			materialOptionTextureGroup = copyUIFrom.textureGroup5;
			break;
		}
		MaterialOptionTextureGroup materialOptionTextureGroup2;
		switch (copyToTextureGroup)
		{
		default:
			return;
		case 1:
			materialOptionTextureGroup2 = textureGroup1;
			break;
		case 2:
			materialOptionTextureGroup2 = textureGroup2;
			break;
		case 3:
			materialOptionTextureGroup2 = textureGroup3;
			break;
		case 4:
			materialOptionTextureGroup2 = textureGroup4;
			break;
		case 5:
			materialOptionTextureGroup2 = textureGroup5;
			break;
		}
		if (materialOptionTextureGroup == null || materialOptionTextureGroup2 == null)
		{
			return;
		}
		int num = materialOptionTextureGroup.sets.Length;
		if (num > 0)
		{
			Debug.Log("Merging in " + num + " sets from " + copyFromTransform.name + " " + materialOptionTextureGroup.name);
			MaterialOptionTextureSet[] array = new MaterialOptionTextureSet[materialOptionTextureGroup2.sets.Length + num];
			int num2 = 0;
			for (int i = 0; i < materialOptionTextureGroup2.sets.Length; i++)
			{
				array[i] = materialOptionTextureGroup2.sets[i];
				num2++;
			}
			for (int j = 0; j < materialOptionTextureGroup.sets.Length; j++)
			{
				array[num2] = new MaterialOptionTextureSet();
				array[num2].name = materialOptionTextureGroup2.autoCreateSetNamePrefix + " " + (num2 + 1);
				array[num2].textures = (Texture[])materialOptionTextureGroup.sets[j].textures.Clone();
				num2++;
			}
			materialOptionTextureGroup2.sets = array;
		}
	}

	protected virtual void SetMaterialParam(string name, float value)
	{
		if (paramMaterialSlots == null)
		{
			return;
		}
		MeshRenderer[] array = meshRenderers;
		foreach (MeshRenderer meshRenderer in array)
		{
			for (int j = 0; j < paramMaterialSlots.Length; j++)
			{
				int num = paramMaterialSlots[j];
				Material[] array2 = ((!Application.isPlaying) ? meshRenderer.sharedMaterials : meshRenderer.materials);
				if (num < array2.Length)
				{
					Material material = array2[num];
					if (material.HasProperty(name))
					{
						material.SetFloat(name, value);
					}
				}
			}
		}
	}

	protected virtual void SetMaterialColor(string name, Color c)
	{
		if (paramMaterialSlots == null)
		{
			return;
		}
		MeshRenderer[] array = meshRenderers;
		foreach (MeshRenderer meshRenderer in array)
		{
			for (int j = 0; j < paramMaterialSlots.Length; j++)
			{
				int num = paramMaterialSlots[j];
				Material[] array2 = ((!Application.isPlaying) ? meshRenderer.sharedMaterials : meshRenderer.materials);
				if (num < array2.Length)
				{
					Material material = array2[num];
					if (material.HasProperty(name))
					{
						material.SetColor(name, c);
					}
				}
			}
		}
	}

	protected virtual void SetMaterialTexture(int slot, string propName, Texture texture)
	{
		if (paramMaterialSlots == null || !(texture != null))
		{
			return;
		}
		MeshRenderer[] array = meshRenderers;
		foreach (MeshRenderer meshRenderer in array)
		{
			Material[] array2 = ((!Application.isPlaying) ? meshRenderer.sharedMaterials : meshRenderer.materials);
			if (slot < array2.Length)
			{
				Material material = array2[slot];
				if (material.HasProperty(propName))
				{
					material.SetTexture(propName, texture);
				}
			}
		}
	}

	protected virtual void SetParam1CurrentValue(float val)
	{
		param1CurrentValue = val;
		SetMaterialParam(param1Name, param1CurrentValue);
	}

	protected virtual void SetParam1CurrentValueUpdateUI(float val)
	{
		SetParam1CurrentValue(val);
		if (param1Slider != null)
		{
			param1Slider.value = val;
		}
		if (param1SliderAlt != null)
		{
			param1SliderAlt.value = val;
		}
	}

	protected virtual void SetParam2CurrentValue(float val)
	{
		param2CurrentValue = val;
		SetMaterialParam(param2Name, param2CurrentValue);
	}

	protected virtual void SetParam2CurrentValueUpdateUI(float val)
	{
		SetParam2CurrentValue(val);
		if (param2Slider != null)
		{
			param2Slider.value = val;
		}
		if (param2SliderAlt != null)
		{
			param2SliderAlt.value = val;
		}
	}

	protected virtual void SetParam3CurrentValue(float val)
	{
		param3CurrentValue = val;
		SetMaterialParam(param3Name, param3CurrentValue);
	}

	protected virtual void SetParam3CurrentValueUpdateUI(float val)
	{
		SetParam3CurrentValue(val);
		if (param3Slider != null)
		{
			param3Slider.value = val;
		}
		if (param3SliderAlt != null)
		{
			param3SliderAlt.value = val;
		}
	}

	protected virtual void SetParam4CurrentValue(float val)
	{
		param4CurrentValue = val;
		SetMaterialParam(param4Name, param4CurrentValue);
	}

	protected virtual void SetParam4CurrentValueUpdateUI(float val)
	{
		SetParam4CurrentValue(val);
		if (param4Slider != null)
		{
			param4Slider.value = val;
		}
		if (param4SliderAlt != null)
		{
			param4SliderAlt.value = val;
		}
	}

	protected virtual void SetParam5CurrentValue(float val)
	{
		param5CurrentValue = val;
		SetMaterialParam(param5Name, param5CurrentValue);
	}

	protected virtual void SetParam5CurrentValueUpdateUI(float val)
	{
		SetParam5CurrentValue(val);
		if (param5Slider != null)
		{
			param5Slider.value = val;
		}
		if (param5SliderAlt != null)
		{
			param5SliderAlt.value = val;
		}
	}

	protected virtual void SetParam6CurrentValue(float val)
	{
		param6CurrentValue = val;
		SetMaterialParam(param6Name, param6CurrentValue);
	}

	protected virtual void SetParam6CurrentValueUpdateUI(float val)
	{
		SetParam6CurrentValue(val);
		if (param6Slider != null)
		{
			param6Slider.value = val;
		}
		if (param6SliderAlt != null)
		{
			param6SliderAlt.value = val;
		}
	}

	protected virtual void SetParam7CurrentValue(float val)
	{
		param7CurrentValue = val;
		SetMaterialParam(param7Name, param7CurrentValue);
	}

	protected virtual void SetParam7CurrentValueUpdateUI(float val)
	{
		SetParam7CurrentValue(val);
		if (param7Slider != null)
		{
			param7Slider.value = val;
		}
		if (param7SliderAlt != null)
		{
			param7SliderAlt.value = val;
		}
	}

	protected virtual void SetParam8CurrentValue(float val)
	{
		param8CurrentValue = val;
		SetMaterialParam(param8Name, param8CurrentValue);
	}

	protected virtual void SetParam8CurrentValueUpdateUI(float val)
	{
		SetParam8CurrentValue(val);
		if (param8Slider != null)
		{
			param8Slider.value = val;
		}
		if (param8SliderAlt != null)
		{
			param8SliderAlt.value = val;
		}
	}

	protected virtual void SetColor1(Color c)
	{
		color1CurrentColor = c;
		color1CurrentHSVColor = HSVColorPicker.RGBToHSV(color1CurrentColor.r, color1CurrentColor.g, color1CurrentColor.b);
		SetMaterialColor(color1Name, c);
	}

	protected virtual void SetColor2(Color c)
	{
		color2CurrentColor = c;
		color2CurrentHSVColor = HSVColorPicker.RGBToHSV(color2CurrentColor.r, color2CurrentColor.g, color2CurrentColor.b);
		SetMaterialColor(color2Name, c);
	}

	protected virtual void SetColor3(Color c)
	{
		color3CurrentColor = c;
		color3CurrentHSVColor = HSVColorPicker.RGBToHSV(color3CurrentColor.r, color3CurrentColor.g, color3CurrentColor.b);
		SetMaterialColor(color3Name, c);
	}

	protected virtual void SetTextureGroup1SetUpdateUI(string setName)
	{
		SetTextureGroup1Set(setName);
		if (textureGroup1Popup != null)
		{
			textureGroup1Popup.currentValueNoCallback = setName;
		}
		if (textureGroup1PopupAlt != null)
		{
			textureGroup1PopupAlt.currentValueNoCallback = setName;
		}
	}

	protected virtual void SetTextureGroup1Set(string setName)
	{
		if (textureGroup1 == null || textureGroup1.sets == null)
		{
			return;
		}
		for (int i = 0; i < textureGroup1.sets.Length; i++)
		{
			if (!(textureGroup1.sets[i].name == setName))
			{
				continue;
			}
			int[] materialSlots = textureGroup1.materialSlots;
			Texture[] textures = textureGroup1.sets[i].textures;
			if (materialSlots.Length != textures.Length)
			{
				continue;
			}
			for (int j = 0; j < materialSlots.Length; j++)
			{
				SetMaterialTexture(materialSlots[j], textureGroup1.textureName, textures[j]);
				if (textureGroup1.secondaryTextureName != null && textureGroup1.secondaryTextureName != string.Empty)
				{
					SetMaterialTexture(materialSlots[j], textureGroup1.secondaryTextureName, textures[j]);
				}
			}
		}
		currentTextureGroup1Set = setName;
	}

	protected virtual void SetTextureGroup2SetUpdateUI(string setName)
	{
		SetTextureGroup2Set(setName);
		if (textureGroup2Popup != null)
		{
			textureGroup2Popup.currentValueNoCallback = setName;
		}
		if (textureGroup2PopupAlt != null)
		{
			textureGroup2PopupAlt.currentValueNoCallback = setName;
		}
	}

	protected virtual void SetTextureGroup2Set(string setName)
	{
		if (textureGroup2 == null || textureGroup2.sets == null)
		{
			return;
		}
		for (int i = 0; i < textureGroup2.sets.Length; i++)
		{
			if (!(textureGroup2.sets[i].name == setName))
			{
				continue;
			}
			int[] materialSlots = textureGroup2.materialSlots;
			Texture[] textures = textureGroup2.sets[i].textures;
			if (materialSlots.Length != textures.Length)
			{
				continue;
			}
			for (int j = 0; j < materialSlots.Length; j++)
			{
				SetMaterialTexture(materialSlots[j], textureGroup2.textureName, textures[j]);
				if (textureGroup2.secondaryTextureName != null && textureGroup2.secondaryTextureName != string.Empty)
				{
					SetMaterialTexture(materialSlots[j], textureGroup2.secondaryTextureName, textures[j]);
				}
			}
		}
		currentTextureGroup2Set = setName;
	}

	protected virtual void SetTextureGroup3SetUpdateUI(string setName)
	{
		SetTextureGroup3Set(setName);
		if (textureGroup3Popup != null)
		{
			textureGroup3Popup.currentValueNoCallback = setName;
		}
		if (textureGroup3PopupAlt != null)
		{
			textureGroup3PopupAlt.currentValueNoCallback = setName;
		}
	}

	protected virtual void SetTextureGroup3Set(string setName)
	{
		if (textureGroup3 == null || textureGroup3.sets == null)
		{
			return;
		}
		for (int i = 0; i < textureGroup3.sets.Length; i++)
		{
			if (!(textureGroup3.sets[i].name == setName))
			{
				continue;
			}
			int[] materialSlots = textureGroup3.materialSlots;
			Texture[] textures = textureGroup3.sets[i].textures;
			if (materialSlots.Length != textures.Length)
			{
				continue;
			}
			for (int j = 0; j < materialSlots.Length; j++)
			{
				SetMaterialTexture(materialSlots[j], textureGroup3.textureName, textures[j]);
				if (textureGroup3.secondaryTextureName != null && textureGroup3.secondaryTextureName != string.Empty)
				{
					SetMaterialTexture(materialSlots[j], textureGroup3.secondaryTextureName, textures[j]);
				}
			}
		}
		currentTextureGroup3Set = setName;
	}

	protected virtual void SetTextureGroup4SetUpdateUI(string setName)
	{
		SetTextureGroup4Set(setName);
		if (textureGroup4Popup != null)
		{
			textureGroup4Popup.currentValueNoCallback = setName;
		}
		if (textureGroup4PopupAlt != null)
		{
			textureGroup4PopupAlt.currentValueNoCallback = setName;
		}
	}

	protected virtual void SetTextureGroup4Set(string setName)
	{
		if (textureGroup4 == null || textureGroup4.sets == null)
		{
			return;
		}
		for (int i = 0; i < textureGroup4.sets.Length; i++)
		{
			if (!(textureGroup4.sets[i].name == setName))
			{
				continue;
			}
			int[] materialSlots = textureGroup4.materialSlots;
			Texture[] textures = textureGroup4.sets[i].textures;
			if (materialSlots.Length != textures.Length)
			{
				continue;
			}
			for (int j = 0; j < materialSlots.Length; j++)
			{
				SetMaterialTexture(materialSlots[j], textureGroup4.textureName, textures[j]);
				if (textureGroup4.secondaryTextureName != null && textureGroup4.secondaryTextureName != string.Empty)
				{
					SetMaterialTexture(materialSlots[j], textureGroup4.secondaryTextureName, textures[j]);
				}
			}
		}
		currentTextureGroup4Set = setName;
	}

	protected virtual void SetTextureGroup5SetUpdateUI(string setName)
	{
		SetTextureGroup5Set(setName);
		if (textureGroup5Popup != null)
		{
			textureGroup5Popup.currentValueNoCallback = setName;
		}
		if (textureGroup5PopupAlt != null)
		{
			textureGroup5PopupAlt.currentValueNoCallback = setName;
		}
	}

	protected virtual void SetTextureGroup5Set(string setName)
	{
		if (textureGroup5 == null || textureGroup5.sets == null)
		{
			return;
		}
		for (int i = 0; i < textureGroup5.sets.Length; i++)
		{
			if (!(textureGroup5.sets[i].name == setName))
			{
				continue;
			}
			int[] materialSlots = textureGroup5.materialSlots;
			Texture[] textures = textureGroup5.sets[i].textures;
			if (materialSlots.Length != textures.Length)
			{
				continue;
			}
			for (int j = 0; j < materialSlots.Length; j++)
			{
				SetMaterialTexture(materialSlots[j], textureGroup5.textureName, textures[j]);
				if (textureGroup5.secondaryTextureName != null && textureGroup5.secondaryTextureName != string.Empty)
				{
					SetMaterialTexture(materialSlots[j], textureGroup5.secondaryTextureName, textures[j]);
				}
			}
		}
		currentTextureGroup5Set = setName;
	}

	public virtual void InitUI()
	{
		if (color1Picker != null)
		{
			if (color1Name != null && color1Name != string.Empty)
			{
				if (color1Container != null)
				{
					color1Container.gameObject.SetActive(value: true);
				}
				if (color1DisplayNameText != null)
				{
					color1DisplayNameText.text = color1DisplayName;
				}
				if (color1CurrentHSVColor == null)
				{
					color1CurrentHSVColor = HSVColorPicker.RGBToHSV(color1CurrentColor.r, color1CurrentColor.g, color1CurrentColor.b);
				}
				color1Picker.hue = color1CurrentHSVColor.H;
				color1Picker.saturation = color1CurrentHSVColor.S;
				color1Picker.cvalue = color1CurrentHSVColor.V;
				if (materialForDefaults != null && materialForDefaults.HasProperty(color1Name))
				{
					Color color = materialForDefaults.GetColor(color1Name);
					HSVColor hSVColor = HSVColorPicker.RGBToHSV(color.r, color.g, color.b);
					color1Picker.defaultHue = hSVColor.H;
					color1Picker.defaultSaturation = hSVColor.S;
					color1Picker.defaultCvalue = hSVColor.V;
				}
				HSVColorPicker hSVColorPicker = color1Picker;
				hSVColorPicker.onColorChangedHandlers = (HSVColorPicker.OnColorChanged)Delegate.Combine(hSVColorPicker.onColorChangedHandlers, new HSVColorPicker.OnColorChanged(SetColor1));
			}
			else
			{
				if (color1Container != null)
				{
					color1Container.gameObject.SetActive(value: false);
				}
				color1Picker.transform.parent.gameObject.SetActive(value: false);
			}
		}
		if (color2Picker != null)
		{
			if (color2Name != null && color2Name != string.Empty)
			{
				if (color2Container != null)
				{
					color2Container.gameObject.SetActive(value: true);
				}
				if (color2DisplayNameText != null)
				{
					color2DisplayNameText.text = color2DisplayName;
				}
				if (color2CurrentHSVColor == null)
				{
					color2CurrentHSVColor = HSVColorPicker.RGBToHSV(color2CurrentColor.r, color2CurrentColor.g, color2CurrentColor.b);
				}
				color2Picker.hue = color2CurrentHSVColor.H;
				color2Picker.saturation = color2CurrentHSVColor.S;
				color2Picker.cvalue = color2CurrentHSVColor.V;
				if (materialForDefaults != null && materialForDefaults.HasProperty(color2Name))
				{
					Color color2 = materialForDefaults.GetColor(color2Name);
					HSVColor hSVColor2 = HSVColorPicker.RGBToHSV(color2.r, color2.g, color2.b);
					color2Picker.defaultHue = hSVColor2.H;
					color2Picker.defaultSaturation = hSVColor2.S;
					color2Picker.defaultCvalue = hSVColor2.V;
				}
				HSVColorPicker hSVColorPicker2 = color2Picker;
				hSVColorPicker2.onColorChangedHandlers = (HSVColorPicker.OnColorChanged)Delegate.Combine(hSVColorPicker2.onColorChangedHandlers, new HSVColorPicker.OnColorChanged(SetColor2));
			}
			else if (color2Container != null)
			{
				color2Container.gameObject.SetActive(value: false);
			}
		}
		if (color3Picker != null)
		{
			if (color3Name != null && color3Name != string.Empty)
			{
				if (color3Container != null)
				{
					color3Container.gameObject.SetActive(value: true);
				}
				if (color3DisplayNameText != null)
				{
					color3DisplayNameText.text = color3DisplayName;
				}
				if (color3CurrentHSVColor == null)
				{
					color3CurrentHSVColor = HSVColorPicker.RGBToHSV(color3CurrentColor.r, color3CurrentColor.g, color3CurrentColor.b);
				}
				color3Picker.hue = color3CurrentHSVColor.H;
				color3Picker.saturation = color3CurrentHSVColor.S;
				color3Picker.cvalue = color3CurrentHSVColor.V;
				if (materialForDefaults != null && materialForDefaults.HasProperty(color3Name))
				{
					Color color3 = materialForDefaults.GetColor(color3Name);
					HSVColor hSVColor3 = HSVColorPicker.RGBToHSV(color3.r, color3.g, color3.b);
					color3Picker.defaultHue = hSVColor3.H;
					color3Picker.defaultSaturation = hSVColor3.S;
					color3Picker.defaultCvalue = hSVColor3.V;
				}
				HSVColorPicker hSVColorPicker3 = color3Picker;
				hSVColorPicker3.onColorChangedHandlers = (HSVColorPicker.OnColorChanged)Delegate.Combine(hSVColorPicker3.onColorChangedHandlers, new HSVColorPicker.OnColorChanged(SetColor3));
			}
			else if (color3Container != null)
			{
				color3Container.gameObject.SetActive(value: false);
			}
		}
		if (param1Slider != null)
		{
			if (param1Name != null && param1Name != string.Empty)
			{
				param1Slider.transform.parent.gameObject.SetActive(value: true);
				if (param1DisplayNameText != null)
				{
					param1DisplayNameText.text = param1DisplayName;
				}
				param1Slider.minValue = param1MinValue;
				param1Slider.maxValue = param1MaxValue;
				param1Slider.value = param1CurrentValue;
				SliderControl component = param1Slider.GetComponent<SliderControl>();
				if (component != null)
				{
					component.defaultValue = param1DefaultValue;
				}
				param1Slider.onValueChanged.AddListener(SetParam1CurrentValueUpdateUI);
			}
			else
			{
				param1Slider.transform.parent.gameObject.SetActive(value: false);
			}
		}
		if (param1SliderAlt != null)
		{
			if (param1Name != null && param1Name != string.Empty)
			{
				param1SliderAlt.transform.parent.gameObject.SetActive(value: true);
				if (param1DisplayNameTextAlt != null)
				{
					param1DisplayNameTextAlt.text = param1DisplayName;
				}
				param1SliderAlt.minValue = param1MinValue;
				param1SliderAlt.maxValue = param1MaxValue;
				param1SliderAlt.value = param1CurrentValue;
				SliderControl component2 = param1SliderAlt.GetComponent<SliderControl>();
				if (component2 != null)
				{
					component2.defaultValue = param1DefaultValue;
				}
				param1SliderAlt.onValueChanged.AddListener(SetParam1CurrentValueUpdateUI);
			}
			else
			{
				param1SliderAlt.transform.parent.gameObject.SetActive(value: false);
			}
		}
		if (param2Slider != null)
		{
			if (param2Name != null && param2Name != string.Empty)
			{
				param2Slider.transform.parent.gameObject.SetActive(value: true);
				if (param2DisplayNameText != null)
				{
					param2DisplayNameText.text = param2DisplayName;
				}
				param2Slider.minValue = param2MinValue;
				param2Slider.maxValue = param2MaxValue;
				param2Slider.value = param2CurrentValue;
				SliderControl component3 = param2Slider.GetComponent<SliderControl>();
				if (component3 != null)
				{
					component3.defaultValue = param2DefaultValue;
				}
				param2Slider.onValueChanged.AddListener(SetParam2CurrentValueUpdateUI);
			}
			else
			{
				param2Slider.transform.parent.gameObject.SetActive(value: false);
			}
		}
		if (param2SliderAlt != null)
		{
			if (param2Name != null && param2Name != string.Empty)
			{
				param2SliderAlt.transform.parent.gameObject.SetActive(value: true);
				if (param2DisplayNameTextAlt != null)
				{
					param2DisplayNameTextAlt.text = param2DisplayName;
				}
				param2SliderAlt.minValue = param2MinValue;
				param2SliderAlt.maxValue = param2MaxValue;
				param2SliderAlt.value = param2CurrentValue;
				SliderControl component4 = param2SliderAlt.GetComponent<SliderControl>();
				if (component4 != null)
				{
					component4.defaultValue = param2DefaultValue;
				}
				param2SliderAlt.onValueChanged.AddListener(SetParam2CurrentValueUpdateUI);
			}
			else
			{
				param2SliderAlt.transform.parent.gameObject.SetActive(value: false);
			}
		}
		if (param3Slider != null)
		{
			if (param3Name != null && param3Name != string.Empty)
			{
				param3Slider.transform.parent.gameObject.SetActive(value: true);
				if (param3DisplayNameText != null)
				{
					param3DisplayNameText.text = param3DisplayName;
				}
				param3Slider.minValue = param3MinValue;
				param3Slider.maxValue = param3MaxValue;
				param3Slider.value = param3CurrentValue;
				SliderControl component5 = param3Slider.GetComponent<SliderControl>();
				if (component5 != null)
				{
					component5.defaultValue = param3DefaultValue;
				}
				param3Slider.onValueChanged.AddListener(SetParam3CurrentValueUpdateUI);
			}
			else
			{
				param3Slider.transform.parent.gameObject.SetActive(value: false);
			}
		}
		if (param3SliderAlt != null)
		{
			if (param3Name != null && param3Name != string.Empty)
			{
				param3SliderAlt.transform.parent.gameObject.SetActive(value: true);
				if (param3DisplayNameTextAlt != null)
				{
					param3DisplayNameTextAlt.text = param3DisplayName;
				}
				param3SliderAlt.minValue = param3MinValue;
				param3SliderAlt.maxValue = param3MaxValue;
				param3SliderAlt.value = param3CurrentValue;
				SliderControl component6 = param3SliderAlt.GetComponent<SliderControl>();
				if (component6 != null)
				{
					component6.defaultValue = param3DefaultValue;
				}
				param3SliderAlt.onValueChanged.AddListener(SetParam3CurrentValueUpdateUI);
			}
			else
			{
				param3SliderAlt.transform.parent.gameObject.SetActive(value: false);
			}
		}
		if (param4Slider != null)
		{
			if (param4Name != null && param4Name != string.Empty)
			{
				param4Slider.transform.parent.gameObject.SetActive(value: true);
				if (param4DisplayNameText != null)
				{
					param4DisplayNameText.text = param4DisplayName;
				}
				param4Slider.minValue = param4MinValue;
				param4Slider.maxValue = param4MaxValue;
				param4Slider.value = param4CurrentValue;
				SliderControl component7 = param4Slider.GetComponent<SliderControl>();
				if (component7 != null)
				{
					component7.defaultValue = param4DefaultValue;
				}
				param4Slider.onValueChanged.AddListener(SetParam4CurrentValueUpdateUI);
			}
			else
			{
				param4Slider.transform.parent.gameObject.SetActive(value: false);
			}
		}
		if (param4SliderAlt != null)
		{
			if (param4Name != null && param4Name != string.Empty)
			{
				param4SliderAlt.transform.parent.gameObject.SetActive(value: true);
				if (param4DisplayNameTextAlt != null)
				{
					param4DisplayNameTextAlt.text = param4DisplayName;
				}
				param4SliderAlt.minValue = param4MinValue;
				param4SliderAlt.maxValue = param4MaxValue;
				param4SliderAlt.value = param4CurrentValue;
				SliderControl component8 = param4SliderAlt.GetComponent<SliderControl>();
				if (component8 != null)
				{
					component8.defaultValue = param4DefaultValue;
				}
				param4SliderAlt.onValueChanged.AddListener(SetParam4CurrentValueUpdateUI);
			}
			else
			{
				param4SliderAlt.transform.parent.gameObject.SetActive(value: false);
			}
		}
		if (param5Slider != null)
		{
			if (param5Name != null && param5Name != string.Empty)
			{
				param5Slider.transform.parent.gameObject.SetActive(value: true);
				if (param5DisplayNameText != null)
				{
					param5DisplayNameText.text = param5DisplayName;
				}
				param5Slider.minValue = param5MinValue;
				param5Slider.maxValue = param5MaxValue;
				param5Slider.value = param5CurrentValue;
				SliderControl component9 = param5Slider.GetComponent<SliderControl>();
				if (component9 != null)
				{
					component9.defaultValue = param5DefaultValue;
				}
				param5Slider.onValueChanged.AddListener(SetParam5CurrentValueUpdateUI);
			}
			else
			{
				param5Slider.transform.parent.gameObject.SetActive(value: false);
			}
		}
		if (param5SliderAlt != null)
		{
			if (param5Name != null && param5Name != string.Empty)
			{
				param5SliderAlt.transform.parent.gameObject.SetActive(value: true);
				if (param5DisplayNameTextAlt != null)
				{
					param5DisplayNameTextAlt.text = param5DisplayName;
				}
				param5SliderAlt.minValue = param5MinValue;
				param5SliderAlt.maxValue = param5MaxValue;
				param5SliderAlt.value = param5CurrentValue;
				SliderControl component10 = param5SliderAlt.GetComponent<SliderControl>();
				if (component10 != null)
				{
					component10.defaultValue = param5DefaultValue;
				}
				param5SliderAlt.onValueChanged.AddListener(SetParam5CurrentValueUpdateUI);
			}
			else
			{
				param5SliderAlt.transform.parent.gameObject.SetActive(value: false);
			}
		}
		if (param6Slider != null)
		{
			if (param6Name != null && param6Name != string.Empty)
			{
				param6Slider.transform.parent.gameObject.SetActive(value: true);
				if (param6DisplayNameText != null)
				{
					param6DisplayNameText.text = param6DisplayName;
				}
				param6Slider.minValue = param6MinValue;
				param6Slider.maxValue = param6MaxValue;
				param6Slider.value = param6CurrentValue;
				SliderControl component11 = param6Slider.GetComponent<SliderControl>();
				if (component11 != null)
				{
					component11.defaultValue = param6DefaultValue;
				}
				param6Slider.onValueChanged.AddListener(SetParam6CurrentValueUpdateUI);
			}
			else
			{
				param6Slider.transform.parent.gameObject.SetActive(value: false);
			}
		}
		if (param6SliderAlt != null)
		{
			if (param6Name != null && param6Name != string.Empty)
			{
				param6SliderAlt.transform.parent.gameObject.SetActive(value: true);
				if (param6DisplayNameTextAlt != null)
				{
					param6DisplayNameTextAlt.text = param6DisplayName;
				}
				param6SliderAlt.minValue = param6MinValue;
				param6SliderAlt.maxValue = param6MaxValue;
				param6SliderAlt.value = param6CurrentValue;
				SliderControl component12 = param6SliderAlt.GetComponent<SliderControl>();
				if (component12 != null)
				{
					component12.defaultValue = param6DefaultValue;
				}
				param6SliderAlt.onValueChanged.AddListener(SetParam6CurrentValueUpdateUI);
			}
			else
			{
				param6SliderAlt.transform.parent.gameObject.SetActive(value: false);
			}
		}
		if (param7Slider != null)
		{
			if (param7Name != null && param7Name != string.Empty)
			{
				param7Slider.transform.parent.gameObject.SetActive(value: true);
				if (param7DisplayNameText != null)
				{
					param7DisplayNameText.text = param7DisplayName;
				}
				param7Slider.minValue = param7MinValue;
				param7Slider.maxValue = param7MaxValue;
				param7Slider.value = param7CurrentValue;
				SliderControl component13 = param7Slider.GetComponent<SliderControl>();
				if (component13 != null)
				{
					component13.defaultValue = param7DefaultValue;
				}
				param7Slider.onValueChanged.AddListener(SetParam7CurrentValueUpdateUI);
			}
			else
			{
				param7Slider.transform.parent.gameObject.SetActive(value: false);
			}
		}
		if (param7SliderAlt != null)
		{
			if (param7Name != null && param7Name != string.Empty)
			{
				param7SliderAlt.transform.parent.gameObject.SetActive(value: true);
				if (param7DisplayNameTextAlt != null)
				{
					param7DisplayNameTextAlt.text = param7DisplayName;
				}
				param7SliderAlt.minValue = param7MinValue;
				param7SliderAlt.maxValue = param7MaxValue;
				param7SliderAlt.value = param7CurrentValue;
				SliderControl component14 = param7SliderAlt.GetComponent<SliderControl>();
				if (component14 != null)
				{
					component14.defaultValue = param7DefaultValue;
				}
				param7SliderAlt.onValueChanged.AddListener(SetParam7CurrentValueUpdateUI);
			}
			else
			{
				param7SliderAlt.transform.parent.gameObject.SetActive(value: false);
			}
		}
		if (param8Slider != null)
		{
			if (param8Name != null && param8Name != string.Empty)
			{
				param8Slider.transform.parent.gameObject.SetActive(value: true);
				if (param8DisplayNameText != null)
				{
					param8DisplayNameText.text = param8DisplayName;
				}
				param8Slider.minValue = param8MinValue;
				param8Slider.maxValue = param8MaxValue;
				param8Slider.value = param8CurrentValue;
				SliderControl component15 = param8Slider.GetComponent<SliderControl>();
				if (component15 != null)
				{
					component15.defaultValue = param8DefaultValue;
				}
				param8Slider.onValueChanged.AddListener(SetParam8CurrentValueUpdateUI);
			}
			else
			{
				param8Slider.transform.parent.gameObject.SetActive(value: false);
			}
		}
		if (param8SliderAlt != null)
		{
			if (param8Name != null && param8Name != string.Empty)
			{
				param8SliderAlt.transform.parent.gameObject.SetActive(value: true);
				if (param8DisplayNameTextAlt != null)
				{
					param8DisplayNameTextAlt.text = param8DisplayName;
				}
				param8SliderAlt.minValue = param8MinValue;
				param8SliderAlt.maxValue = param8MaxValue;
				param8SliderAlt.value = param8CurrentValue;
				SliderControl component16 = param8SliderAlt.GetComponent<SliderControl>();
				if (component16 != null)
				{
					component16.defaultValue = param8DefaultValue;
				}
				param8SliderAlt.onValueChanged.AddListener(SetParam8CurrentValueUpdateUI);
			}
			else
			{
				param8SliderAlt.transform.parent.gameObject.SetActive(value: false);
			}
		}
		if (textureGroup1Popup != null)
		{
			if (textureGroup1 != null && textureGroup1.sets != null && textureGroup1.sets.Length > 0)
			{
				textureGroup1Popup.gameObject.SetActive(value: true);
				textureGroup1Popup.numPopupValues = textureGroup1.sets.Length;
				for (int i = 0; i < textureGroup1.sets.Length; i++)
				{
					textureGroup1Popup.setPopupValue(i, textureGroup1.sets[i].name);
				}
				UIPopup uIPopup = textureGroup1Popup;
				uIPopup.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup.onValueChangeHandlers, new UIPopup.OnValueChange(SetTextureGroup1SetUpdateUI));
			}
			else
			{
				textureGroup1Popup.gameObject.SetActive(value: false);
			}
			SetTextureGroup1Set(currentTextureGroup1Set);
			textureGroup1Popup.currentValue = currentTextureGroup1Set;
		}
		if (textureGroup1PopupAlt != null)
		{
			if (textureGroup1 != null && textureGroup1.sets != null && textureGroup1.sets.Length > 0)
			{
				textureGroup1PopupAlt.gameObject.SetActive(value: true);
				textureGroup1PopupAlt.numPopupValues = textureGroup1.sets.Length;
				for (int j = 0; j < textureGroup1.sets.Length; j++)
				{
					textureGroup1PopupAlt.setPopupValue(j, textureGroup1.sets[j].name);
				}
				UIPopup uIPopup2 = textureGroup1PopupAlt;
				uIPopup2.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup2.onValueChangeHandlers, new UIPopup.OnValueChange(SetTextureGroup1SetUpdateUI));
			}
			else
			{
				textureGroup1PopupAlt.gameObject.SetActive(value: false);
			}
			textureGroup1PopupAlt.currentValue = currentTextureGroup1Set;
		}
		if (textureGroup1Text != null)
		{
			if (textureGroup1 != null)
			{
				textureGroup1Text.text = textureGroup1.name;
			}
			else
			{
				textureGroup1Text.text = string.Empty;
			}
		}
		if (textureGroup1TextAlt != null)
		{
			if (textureGroup1 != null)
			{
				textureGroup1TextAlt.text = textureGroup1.name;
			}
			else
			{
				textureGroup1TextAlt.text = string.Empty;
			}
		}
		if (textureGroup2Popup != null)
		{
			if (textureGroup2 != null && textureGroup2.sets != null && textureGroup2.sets.Length > 0)
			{
				textureGroup2Popup.gameObject.SetActive(value: true);
				textureGroup2Popup.numPopupValues = textureGroup2.sets.Length;
				for (int k = 0; k < textureGroup2.sets.Length; k++)
				{
					textureGroup2Popup.setPopupValue(k, textureGroup2.sets[k].name);
				}
				UIPopup uIPopup3 = textureGroup2Popup;
				uIPopup3.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup3.onValueChangeHandlers, new UIPopup.OnValueChange(SetTextureGroup2SetUpdateUI));
			}
			else
			{
				textureGroup2Popup.gameObject.SetActive(value: false);
			}
			SetTextureGroup2Set(currentTextureGroup2Set);
			textureGroup2Popup.currentValue = currentTextureGroup2Set;
		}
		if (textureGroup2PopupAlt != null)
		{
			if (textureGroup2 != null && textureGroup2.sets != null && textureGroup2.sets.Length > 0)
			{
				textureGroup2PopupAlt.gameObject.SetActive(value: true);
				textureGroup2PopupAlt.numPopupValues = textureGroup2.sets.Length;
				for (int l = 0; l < textureGroup2.sets.Length; l++)
				{
					textureGroup2PopupAlt.setPopupValue(l, textureGroup2.sets[l].name);
				}
				UIPopup uIPopup4 = textureGroup2PopupAlt;
				uIPopup4.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup4.onValueChangeHandlers, new UIPopup.OnValueChange(SetTextureGroup2SetUpdateUI));
			}
			else
			{
				textureGroup2PopupAlt.gameObject.SetActive(value: false);
			}
			textureGroup2PopupAlt.currentValue = currentTextureGroup2Set;
		}
		if (textureGroup2Text != null)
		{
			if (textureGroup2 != null)
			{
				textureGroup2Text.text = textureGroup2.name;
			}
			else
			{
				textureGroup2Text.text = string.Empty;
			}
		}
		if (textureGroup2TextAlt != null)
		{
			if (textureGroup2 != null)
			{
				textureGroup2TextAlt.text = textureGroup2.name;
			}
			else
			{
				textureGroup2TextAlt.text = string.Empty;
			}
		}
		if (textureGroup3Popup != null)
		{
			if (textureGroup3 != null && textureGroup3.sets != null && textureGroup3.sets.Length > 0)
			{
				textureGroup3Popup.gameObject.SetActive(value: true);
				textureGroup3Popup.numPopupValues = textureGroup3.sets.Length;
				for (int m = 0; m < textureGroup3.sets.Length; m++)
				{
					textureGroup3Popup.setPopupValue(m, textureGroup3.sets[m].name);
				}
				UIPopup uIPopup5 = textureGroup3Popup;
				uIPopup5.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup5.onValueChangeHandlers, new UIPopup.OnValueChange(SetTextureGroup3SetUpdateUI));
			}
			else
			{
				textureGroup3Popup.gameObject.SetActive(value: false);
			}
			SetTextureGroup3Set(currentTextureGroup3Set);
			textureGroup3Popup.currentValue = currentTextureGroup3Set;
		}
		if (textureGroup3PopupAlt != null)
		{
			if (textureGroup3 != null && textureGroup3.sets != null && textureGroup3.sets.Length > 0)
			{
				textureGroup3PopupAlt.gameObject.SetActive(value: true);
				textureGroup3PopupAlt.numPopupValues = textureGroup3.sets.Length;
				for (int n = 0; n < textureGroup3.sets.Length; n++)
				{
					textureGroup3PopupAlt.setPopupValue(n, textureGroup3.sets[n].name);
				}
				UIPopup uIPopup6 = textureGroup3PopupAlt;
				uIPopup6.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup6.onValueChangeHandlers, new UIPopup.OnValueChange(SetTextureGroup3SetUpdateUI));
			}
			else
			{
				textureGroup3PopupAlt.gameObject.SetActive(value: false);
			}
			textureGroup3PopupAlt.currentValue = currentTextureGroup3Set;
		}
		if (textureGroup3Text != null)
		{
			if (textureGroup3 != null)
			{
				textureGroup3Text.text = textureGroup3.name;
			}
			else
			{
				textureGroup3Text.text = string.Empty;
			}
		}
		if (textureGroup3TextAlt != null)
		{
			if (textureGroup3 != null)
			{
				textureGroup3TextAlt.text = textureGroup3.name;
			}
			else
			{
				textureGroup3TextAlt.text = string.Empty;
			}
		}
		if (textureGroup4Popup != null)
		{
			if (textureGroup4 != null && textureGroup4.sets != null && textureGroup4.sets.Length > 0)
			{
				textureGroup4Popup.gameObject.SetActive(value: true);
				textureGroup4Popup.numPopupValues = textureGroup4.sets.Length;
				for (int num = 0; num < textureGroup4.sets.Length; num++)
				{
					textureGroup4Popup.setPopupValue(num, textureGroup4.sets[num].name);
				}
				UIPopup uIPopup7 = textureGroup4Popup;
				uIPopup7.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup7.onValueChangeHandlers, new UIPopup.OnValueChange(SetTextureGroup4SetUpdateUI));
			}
			else
			{
				textureGroup4Popup.gameObject.SetActive(value: false);
			}
			SetTextureGroup4Set(currentTextureGroup4Set);
			textureGroup4Popup.currentValue = currentTextureGroup4Set;
		}
		if (textureGroup4PopupAlt != null)
		{
			if (textureGroup4 != null && textureGroup4.sets != null && textureGroup4.sets.Length > 0)
			{
				textureGroup4PopupAlt.gameObject.SetActive(value: true);
				textureGroup4PopupAlt.numPopupValues = textureGroup4.sets.Length;
				for (int num2 = 0; num2 < textureGroup4.sets.Length; num2++)
				{
					textureGroup4PopupAlt.setPopupValue(num2, textureGroup4.sets[num2].name);
				}
				UIPopup uIPopup8 = textureGroup4PopupAlt;
				uIPopup8.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup8.onValueChangeHandlers, new UIPopup.OnValueChange(SetTextureGroup4SetUpdateUI));
			}
			else
			{
				textureGroup4PopupAlt.gameObject.SetActive(value: false);
			}
			textureGroup4PopupAlt.currentValue = currentTextureGroup4Set;
		}
		if (textureGroup4Text != null)
		{
			if (textureGroup4 != null)
			{
				textureGroup4Text.text = textureGroup4.name;
			}
			else
			{
				textureGroup4Text.text = string.Empty;
			}
		}
		if (textureGroup4TextAlt != null)
		{
			if (textureGroup4 != null)
			{
				textureGroup4TextAlt.text = textureGroup4.name;
			}
			else
			{
				textureGroup4TextAlt.text = string.Empty;
			}
		}
		if (textureGroup5Popup != null)
		{
			if (textureGroup5 != null && textureGroup5.sets != null && textureGroup5.sets.Length > 0)
			{
				textureGroup5Popup.gameObject.SetActive(value: true);
				textureGroup5Popup.numPopupValues = textureGroup5.sets.Length;
				for (int num3 = 0; num3 < textureGroup5.sets.Length; num3++)
				{
					textureGroup5Popup.setPopupValue(num3, textureGroup5.sets[num3].name);
				}
				UIPopup uIPopup9 = textureGroup5Popup;
				uIPopup9.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup9.onValueChangeHandlers, new UIPopup.OnValueChange(SetTextureGroup5SetUpdateUI));
			}
			else
			{
				textureGroup5Popup.gameObject.SetActive(value: false);
			}
			SetTextureGroup5Set(currentTextureGroup5Set);
			textureGroup5Popup.currentValue = currentTextureGroup5Set;
		}
		if (textureGroup5PopupAlt != null)
		{
			if (textureGroup5 != null && textureGroup5.sets != null && textureGroup5.sets.Length > 0)
			{
				textureGroup5PopupAlt.gameObject.SetActive(value: true);
				textureGroup5PopupAlt.numPopupValues = textureGroup5.sets.Length;
				for (int num4 = 0; num4 < textureGroup5.sets.Length; num4++)
				{
					textureGroup5PopupAlt.setPopupValue(num4, textureGroup5.sets[num4].name);
				}
				UIPopup uIPopup10 = textureGroup5PopupAlt;
				uIPopup10.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup10.onValueChangeHandlers, new UIPopup.OnValueChange(SetTextureGroup5SetUpdateUI));
			}
			else
			{
				textureGroup5PopupAlt.gameObject.SetActive(value: false);
			}
			textureGroup5PopupAlt.currentValue = currentTextureGroup5Set;
		}
		if (textureGroup5Text != null)
		{
			if (textureGroup5 != null)
			{
				textureGroup5Text.text = textureGroup5.name;
			}
			else
			{
				textureGroup5Text.text = string.Empty;
			}
		}
		if (textureGroup5TextAlt != null)
		{
			if (textureGroup5 != null)
			{
				textureGroup5TextAlt.text = textureGroup5.name;
			}
			else
			{
				textureGroup5TextAlt.text = string.Empty;
			}
		}
	}

	protected virtual void DeregisterUI()
	{
		if (color1Picker != null && color1Name != null && color1Name != string.Empty)
		{
			HSVColorPicker hSVColorPicker = color1Picker;
			hSVColorPicker.onColorChangedHandlers = (HSVColorPicker.OnColorChanged)Delegate.Remove(hSVColorPicker.onColorChangedHandlers, new HSVColorPicker.OnColorChanged(SetColor1));
		}
		if (color2Picker != null && color2Name != null && color2Name != string.Empty)
		{
			HSVColorPicker hSVColorPicker2 = color2Picker;
			hSVColorPicker2.onColorChangedHandlers = (HSVColorPicker.OnColorChanged)Delegate.Remove(hSVColorPicker2.onColorChangedHandlers, new HSVColorPicker.OnColorChanged(SetColor2));
		}
		if (color3Picker != null && color3Name != null && color3Name != string.Empty)
		{
			HSVColorPicker hSVColorPicker3 = color3Picker;
			hSVColorPicker3.onColorChangedHandlers = (HSVColorPicker.OnColorChanged)Delegate.Remove(hSVColorPicker3.onColorChangedHandlers, new HSVColorPicker.OnColorChanged(SetColor3));
		}
		if (param1Name != null && param1Name != string.Empty && param1Slider != null)
		{
			param1Slider.onValueChanged.RemoveListener(SetParam1CurrentValueUpdateUI);
		}
		if (param1Name != null && param1Name != string.Empty && param1SliderAlt != null)
		{
			param1SliderAlt.onValueChanged.RemoveListener(SetParam1CurrentValueUpdateUI);
		}
		if (param2Name != null && param2Name != string.Empty && param2Slider != null)
		{
			param2Slider.onValueChanged.RemoveListener(SetParam2CurrentValueUpdateUI);
		}
		if (param2Name != null && param2Name != string.Empty && param2SliderAlt != null)
		{
			param2SliderAlt.onValueChanged.RemoveListener(SetParam2CurrentValueUpdateUI);
		}
		if (param3Name != null && param3Name != string.Empty && param3Slider != null)
		{
			param3Slider.onValueChanged.RemoveListener(SetParam3CurrentValueUpdateUI);
		}
		if (param3Name != null && param3Name != string.Empty && param3SliderAlt != null)
		{
			param3SliderAlt.onValueChanged.RemoveListener(SetParam3CurrentValueUpdateUI);
		}
		if (param4Name != null && param4Name != string.Empty && param4Slider != null)
		{
			param4Slider.onValueChanged.RemoveListener(SetParam4CurrentValueUpdateUI);
		}
		if (param4Name != null && param4Name != string.Empty && param4SliderAlt != null)
		{
			param4SliderAlt.onValueChanged.RemoveListener(SetParam4CurrentValueUpdateUI);
		}
		if (param5Name != null && param5Name != string.Empty && param5Slider != null)
		{
			param5Slider.onValueChanged.RemoveListener(SetParam5CurrentValueUpdateUI);
		}
		if (param5Name != null && param5Name != string.Empty && param5SliderAlt != null)
		{
			param5SliderAlt.onValueChanged.RemoveListener(SetParam5CurrentValueUpdateUI);
		}
		if (param6Name != null && param6Name != string.Empty && param6Slider != null)
		{
			param6Slider.onValueChanged.RemoveListener(SetParam6CurrentValueUpdateUI);
		}
		if (param6Name != null && param6Name != string.Empty && param6SliderAlt != null)
		{
			param6SliderAlt.onValueChanged.RemoveListener(SetParam6CurrentValueUpdateUI);
		}
		if (param7Name != null && param7Name != string.Empty && param7Slider != null)
		{
			param7Slider.onValueChanged.RemoveListener(SetParam7CurrentValueUpdateUI);
		}
		if (param7Name != null && param7Name != string.Empty && param7SliderAlt != null)
		{
			param7SliderAlt.onValueChanged.RemoveListener(SetParam7CurrentValueUpdateUI);
		}
		if (param8Name != null && param8Name != string.Empty && param8Slider != null)
		{
			param8Slider.onValueChanged.RemoveListener(SetParam8CurrentValueUpdateUI);
		}
		if (param8Name != null && param8Name != string.Empty && param8SliderAlt != null)
		{
			param8SliderAlt.onValueChanged.RemoveListener(SetParam8CurrentValueUpdateUI);
		}
		if (textureGroup1Popup != null && textureGroup1 != null && textureGroup1.sets != null)
		{
			UIPopup uIPopup = textureGroup1Popup;
			uIPopup.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Remove(uIPopup.onValueChangeHandlers, new UIPopup.OnValueChange(SetTextureGroup1SetUpdateUI));
		}
		if (textureGroup1PopupAlt != null && textureGroup1 != null && textureGroup1.sets != null)
		{
			UIPopup uIPopup2 = textureGroup1PopupAlt;
			uIPopup2.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Remove(uIPopup2.onValueChangeHandlers, new UIPopup.OnValueChange(SetTextureGroup1SetUpdateUI));
		}
		if (textureGroup2Popup != null && textureGroup2 != null && textureGroup2.sets != null)
		{
			UIPopup uIPopup3 = textureGroup2Popup;
			uIPopup3.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Remove(uIPopup3.onValueChangeHandlers, new UIPopup.OnValueChange(SetTextureGroup2SetUpdateUI));
		}
		if (textureGroup2PopupAlt != null && textureGroup2 != null && textureGroup2.sets != null)
		{
			UIPopup uIPopup4 = textureGroup2PopupAlt;
			uIPopup4.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Remove(uIPopup4.onValueChangeHandlers, new UIPopup.OnValueChange(SetTextureGroup2SetUpdateUI));
		}
		if (textureGroup3Popup != null && textureGroup3 != null && textureGroup3.sets != null)
		{
			UIPopup uIPopup5 = textureGroup3Popup;
			uIPopup5.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Remove(uIPopup5.onValueChangeHandlers, new UIPopup.OnValueChange(SetTextureGroup3SetUpdateUI));
		}
		if (textureGroup3PopupAlt != null && textureGroup3 != null && textureGroup3.sets != null)
		{
			UIPopup uIPopup6 = textureGroup3PopupAlt;
			uIPopup6.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Remove(uIPopup6.onValueChangeHandlers, new UIPopup.OnValueChange(SetTextureGroup3SetUpdateUI));
		}
		if (textureGroup4Popup != null && textureGroup4 != null && textureGroup4.sets != null)
		{
			UIPopup uIPopup7 = textureGroup4Popup;
			uIPopup7.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Remove(uIPopup7.onValueChangeHandlers, new UIPopup.OnValueChange(SetTextureGroup4SetUpdateUI));
		}
		if (textureGroup4PopupAlt != null && textureGroup4 != null && textureGroup4.sets != null)
		{
			UIPopup uIPopup8 = textureGroup4PopupAlt;
			uIPopup8.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Remove(uIPopup8.onValueChangeHandlers, new UIPopup.OnValueChange(SetTextureGroup4SetUpdateUI));
		}
		if (textureGroup5Popup != null && textureGroup5 != null && textureGroup5.sets != null)
		{
			UIPopup uIPopup9 = textureGroup5Popup;
			uIPopup9.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Remove(uIPopup9.onValueChangeHandlers, new UIPopup.OnValueChange(SetTextureGroup5SetUpdateUI));
		}
		if (textureGroup5PopupAlt != null && textureGroup5 != null && textureGroup5.sets != null)
		{
			UIPopup uIPopup10 = textureGroup5PopupAlt;
			uIPopup10.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Remove(uIPopup10.onValueChangeHandlers, new UIPopup.OnValueChange(SetTextureGroup5SetUpdateUI));
		}
	}

	protected virtual void SetStartingValues()
	{
		if (wasInit)
		{
			return;
		}
		wasInit = true;
		if (materialContainer != null)
		{
			meshRenderers = materialContainer.GetComponentsInChildren<MeshRenderer>();
		}
		else
		{
			meshRenderers = GetComponentsInChildren<MeshRenderer>();
		}
		if (materialForDefaults == null && meshRenderers.Length > 0 && paramMaterialSlots != null && paramMaterialSlots.Length > 0)
		{
			MeshRenderer meshRenderer = meshRenderers[0];
			int num = paramMaterialSlots[0];
			if (num < meshRenderer.materials.Length)
			{
				materialForDefaults = meshRenderer.materials[num];
			}
		}
		if (materialForDefaults != null)
		{
			if (color1Name != null && color1Name != string.Empty && materialForDefaults.HasProperty(color1Name))
			{
				color1CurrentColor = materialForDefaults.GetColor(color1Name);
				color1CurrentHSVColor = HSVColorPicker.RGBToHSV(color1CurrentColor.r, color1CurrentColor.g, color1CurrentColor.b);
				color1DefaultHSVColor = color1CurrentHSVColor;
			}
			else
			{
				color1CurrentColor = Color.black;
				color1CurrentHSVColor = HSVColorPicker.RGBToHSV(0f, 0f, 0f);
				color1DefaultHSVColor = color1CurrentHSVColor;
			}
			if (color2Name != null && color2Name != string.Empty && materialForDefaults.HasProperty(color2Name))
			{
				color2CurrentColor = materialForDefaults.GetColor(color2Name);
				color2CurrentHSVColor = HSVColorPicker.RGBToHSV(color2CurrentColor.r, color2CurrentColor.g, color2CurrentColor.b);
				color2DefaultHSVColor = color2CurrentHSVColor;
			}
			else
			{
				color2CurrentColor = Color.black;
				color2CurrentHSVColor = HSVColorPicker.RGBToHSV(0f, 0f, 0f);
				color2DefaultHSVColor = color2CurrentHSVColor;
			}
			if (color3Name != null && color3Name != string.Empty && materialForDefaults.HasProperty(color3Name))
			{
				color3CurrentColor = materialForDefaults.GetColor(color3Name);
				color3CurrentHSVColor = HSVColorPicker.RGBToHSV(color3CurrentColor.r, color3CurrentColor.g, color3CurrentColor.b);
				color3DefaultHSVColor = color3CurrentHSVColor;
			}
			else
			{
				color3CurrentColor = Color.black;
				color3CurrentHSVColor = HSVColorPicker.RGBToHSV(0f, 0f, 0f);
				color3DefaultHSVColor = color1CurrentHSVColor;
			}
			if (param1Name != null && param1Name != string.Empty && materialForDefaults.HasProperty(param1Name))
			{
				param1CurrentValue = materialForDefaults.GetFloat(param1Name);
				param1DefaultValue = param1CurrentValue;
			}
			if (param2Name != null && param2Name != string.Empty && materialForDefaults.HasProperty(param2Name))
			{
				param2CurrentValue = materialForDefaults.GetFloat(param2Name);
				param2DefaultValue = param2CurrentValue;
			}
			if (param3Name != null && param3Name != string.Empty && materialForDefaults.HasProperty(param3Name))
			{
				param3CurrentValue = materialForDefaults.GetFloat(param3Name);
				param3DefaultValue = param3CurrentValue;
			}
			if (param4Name != null && param4Name != string.Empty && materialForDefaults.HasProperty(param4Name))
			{
				param4CurrentValue = materialForDefaults.GetFloat(param4Name);
				param4DefaultValue = param4CurrentValue;
			}
			if (param5Name != null && param5Name != string.Empty && materialForDefaults.HasProperty(param5Name))
			{
				param5CurrentValue = materialForDefaults.GetFloat(param5Name);
				param5DefaultValue = param5CurrentValue;
			}
			if (param6Name != null && param6Name != string.Empty && materialForDefaults.HasProperty(param6Name))
			{
				param6CurrentValue = materialForDefaults.GetFloat(param6Name);
				param6DefaultValue = param6CurrentValue;
			}
			if (param7Name != null && param7Name != string.Empty && materialForDefaults.HasProperty(param7Name))
			{
				param7CurrentValue = materialForDefaults.GetFloat(param7Name);
				param7DefaultValue = param7CurrentValue;
			}
			if (param8Name != null && param8Name != string.Empty && materialForDefaults.HasProperty(param8Name))
			{
				param8CurrentValue = materialForDefaults.GetFloat(param8Name);
				param8DefaultValue = param8CurrentValue;
			}
		}
		currentTextureGroup1Set = startingTextureGroup1Set;
		currentTextureGroup2Set = startingTextureGroup2Set;
		currentTextureGroup3Set = startingTextureGroup3Set;
		currentTextureGroup4Set = startingTextureGroup4Set;
		currentTextureGroup5Set = startingTextureGroup5Set;
	}

	protected virtual void SetAllParameters()
	{
		if (materialForDefaults != null)
		{
			if (color1Name != null && color1Name != string.Empty && materialForDefaults.HasProperty(color1Name))
			{
				SetMaterialColor(color1Name, color1CurrentColor);
			}
			if (color2Name != null && color2Name != string.Empty && materialForDefaults.HasProperty(color2Name))
			{
				SetMaterialColor(color2Name, color2CurrentColor);
			}
			if (color3Name != null && color3Name != string.Empty && materialForDefaults.HasProperty(color3Name))
			{
				SetMaterialColor(color3Name, color3CurrentColor);
			}
			if (param1Name != null && param1Name != string.Empty && materialForDefaults.HasProperty(param1Name))
			{
				SetMaterialParam(param1Name, param1CurrentValue);
			}
			if (param2Name != null && param2Name != string.Empty && materialForDefaults.HasProperty(param2Name))
			{
				SetMaterialParam(param2Name, param2CurrentValue);
			}
			if (param3Name != null && param3Name != string.Empty && materialForDefaults.HasProperty(param3Name))
			{
				SetMaterialParam(param3Name, param3CurrentValue);
			}
			if (param4Name != null && param4Name != string.Empty && materialForDefaults.HasProperty(param4Name))
			{
				SetMaterialParam(param4Name, param4CurrentValue);
			}
			if (param5Name != null && param5Name != string.Empty && materialForDefaults.HasProperty(param5Name))
			{
				SetMaterialParam(param5Name, param5CurrentValue);
			}
			if (param6Name != null && param6Name != string.Empty && materialForDefaults.HasProperty(param6Name))
			{
				SetMaterialParam(param6Name, param6CurrentValue);
			}
			if (param7Name != null && param7Name != string.Empty && materialForDefaults.HasProperty(param7Name))
			{
				SetMaterialParam(param7Name, param7CurrentValue);
			}
			if (param8Name != null && param8Name != string.Empty && materialForDefaults.HasProperty(param8Name))
			{
				SetMaterialParam(param8Name, param8CurrentValue);
			}
		}
		if (textureGroup1Popup != null && textureGroup1 != null && textureGroup1.sets != null && textureGroup1.sets.Length > 0)
		{
			SetTextureGroup1Set(currentTextureGroup1Set);
		}
		if (textureGroup2Popup != null && textureGroup2 != null && textureGroup2.sets != null && textureGroup2.sets.Length > 0)
		{
			SetTextureGroup2Set(currentTextureGroup2Set);
		}
		if (textureGroup3Popup != null && textureGroup3 != null && textureGroup3.sets != null && textureGroup3.sets.Length > 0)
		{
			SetTextureGroup3Set(currentTextureGroup3Set);
		}
		if (textureGroup4Popup != null && textureGroup4 != null && textureGroup4.sets != null && textureGroup4.sets.Length > 0)
		{
			SetTextureGroup4Set(currentTextureGroup4Set);
		}
		if (textureGroup5Popup != null && textureGroup5 != null && textureGroup5.sets != null && textureGroup5.sets.Length > 0)
		{
			SetTextureGroup5Set(currentTextureGroup5Set);
		}
	}

	public virtual void SyncAllParameters()
	{
		wasInit = false;
		SetStartingValues();
		SetAllParameters();
	}

	protected virtual void Awake()
	{
		SetStartingValues();
	}

	protected virtual void OnEnable()
	{
		InitUI();
	}

	protected virtual void OnDisable()
	{
		DeregisterUI();
	}
}
