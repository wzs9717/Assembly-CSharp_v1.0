using System;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class AdjustLightV2 : JSONStorable
{
	public UIPopup typeSelector;

	public UIPopup shadowTypeSelector;

	public UIPopup renderModeSelector;

	public Toggle onToggle;

	public Toggle shadowsToggle;

	public Toggle showHaloToggle;

	public Toggle showDustToggle;

	public Slider intensitySlider;

	public Slider intensitySliderAlt;

	public Slider rangeSlider;

	public Slider rangeSliderAlt;

	public Slider shadowStrengthSlider;

	public Slider spotAngleSlider;

	public HSVColorPicker colorPicker;

	public HSVColorPicker colorPickerAlt;

	public bool controlBias = true;

	public float pointBias = 0.001f;

	public float spotBias = 0.001f;

	public float directionalBias = 0.02f;

	public bool controlNearPlane;

	public float pointNearPlane = 0.1f;

	public float spotNearPlane = 0.5f;

	public float directionalNearPlane = 0.5f;

	public MeshRenderer[] emissiveRenderers;

	public bool autoShadowType = true;

	private Light _light;

	private LightShadows saveShadowType = LightShadows.Hard;

	[SerializeField]
	protected bool _showHalo;

	[SerializeField]
	protected bool _showDust;

	public bool on
	{
		get
		{
			if (_light != null)
			{
				return _light.enabled;
			}
			return false;
		}
		set
		{
			if (_light != null)
			{
				if (_light.enabled != value)
				{
					_light.enabled = value;
				}
				if (onToggle != null)
				{
					onToggle.isOn = value;
				}
			}
			if (emissiveRenderers != null)
			{
				MeshRenderer[] array = emissiveRenderers;
				foreach (MeshRenderer meshRenderer in array)
				{
					meshRenderer.enabled = value;
				}
			}
		}
	}

	public float intensity
	{
		get
		{
			if (_light != null)
			{
				return _light.intensity;
			}
			return 0f;
		}
		set
		{
			if (_light != null && _light.intensity != value)
			{
				_light.intensity = value;
				if (intensitySlider != null)
				{
					intensitySlider.value = value;
				}
				if (intensitySliderAlt != null)
				{
					intensitySliderAlt.value = value;
				}
				SyncEmissiveRenderers();
			}
		}
	}

	public float range
	{
		get
		{
			if (_light != null)
			{
				return _light.range;
			}
			return 0f;
		}
		set
		{
			if (_light != null && _light.range != value)
			{
				_light.range = value;
				if (rangeSlider != null)
				{
					rangeSlider.value = value;
				}
				if (rangeSliderAlt != null)
				{
					rangeSliderAlt.value = value;
				}
			}
		}
	}

	public bool shadowsOn
	{
		get
		{
			if (_light != null)
			{
				return _light.shadows != LightShadows.None;
			}
			return false;
		}
		set
		{
			if (_light != null)
			{
				if (value)
				{
					_light.shadows = saveShadowType;
				}
				else
				{
					saveShadowType = _light.shadows;
					_light.shadows = LightShadows.None;
				}
				if (shadowsToggle != null && shadowsToggle.isOn != value)
				{
					shadowsToggle.isOn = value;
				}
			}
		}
	}

	public float shadowStrength
	{
		get
		{
			if (_light != null)
			{
				return _light.shadowStrength;
			}
			return 0f;
		}
		set
		{
			if (_light != null && _light.shadowStrength != value)
			{
				_light.shadowStrength = value;
				if (shadowStrengthSlider != null)
				{
					shadowStrengthSlider.value = value;
				}
			}
		}
	}

	public float spotAngle
	{
		get
		{
			if (_light != null)
			{
				return _light.spotAngle;
			}
			return 0f;
		}
		set
		{
			if (_light != null && _light.spotAngle != value)
			{
				_light.spotAngle = value;
				if (spotAngleSlider != null)
				{
					spotAngleSlider.value = value;
				}
			}
		}
	}

	public bool showHalo
	{
		get
		{
			return _showHalo;
		}
		set
		{
			if (_showHalo != value)
			{
				_showHalo = value;
				if (showHaloToggle != null)
				{
					showHaloToggle.isOn = value;
				}
				SyncShowHalo();
			}
		}
	}

	public bool showDust
	{
		get
		{
			return _showDust;
		}
		set
		{
			if (_showDust != value)
			{
				_showDust = value;
				if (showDustToggle != null)
				{
					showDustToggle.isOn = value;
				}
				SyncShowDust();
			}
		}
	}

	public override JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true)
	{
		JSONClass jSON = base.GetJSON(includePhysical, includeAppearance);
		if (includeAppearance && _light != null)
		{
			if (!on)
			{
				needsStore = true;
				jSON["on"].AsBool = on;
			}
			if (intensitySlider != null)
			{
				SliderControl component = intensitySlider.GetComponent<SliderControl>();
				if (component == null || component.defaultValue != intensity)
				{
					needsStore = true;
					jSON["intensity"].AsFloat = intensity;
				}
			}
			if (rangeSlider != null)
			{
				SliderControl component2 = rangeSlider.GetComponent<SliderControl>();
				if (component2 == null || component2.defaultValue != range)
				{
					needsStore = true;
					jSON["range"].AsFloat = range;
				}
			}
			if (spotAngleSlider != null)
			{
				SliderControl component3 = spotAngleSlider.GetComponent<SliderControl>();
				if (component3 == null || component3.defaultValue != spotAngle)
				{
					needsStore = true;
					jSON["spotAngle"].AsFloat = spotAngle;
				}
			}
			if (!shadowsOn)
			{
				needsStore = true;
				jSON["shadowsOn"].AsBool = shadowsOn;
			}
			if (showHalo)
			{
				needsStore = true;
				jSON["showHalo"].AsBool = showHalo;
			}
			if (showDust)
			{
				needsStore = true;
				jSON["showDust"].AsBool = showDust;
			}
			if (shadowStrengthSlider != null)
			{
				SliderControl component4 = shadowStrengthSlider.GetComponent<SliderControl>();
				if (component4 == null || component4.defaultValue != shadowStrength)
				{
					needsStore = true;
					jSON["shadowStrength"].AsFloat = shadowStrength;
				}
			}
			if (_light.type != LightType.Point)
			{
				needsStore = true;
				jSON["type"] = _light.type.ToString();
			}
			if (_light.renderMode != 0)
			{
				needsStore = true;
				jSON["renderType"] = _light.renderMode.ToString();
			}
			if (colorPicker != null && (colorPicker.hue != colorPicker.defaultHue || colorPicker.saturation != colorPicker.defaultSaturation || colorPicker.cvalue != colorPicker.defaultCvalue))
			{
				needsStore = true;
				jSON["color"]["h"].AsFloat = colorPicker.hue;
				jSON["color"]["s"].AsFloat = colorPicker.saturation;
				jSON["color"]["v"].AsFloat = colorPicker.cvalue;
			}
		}
		return jSON;
	}

	public override void RestoreFromJSON(JSONClass jc, bool restorePhysical = true, bool restoreAppearance = true)
	{
		base.RestoreFromJSON(jc, restorePhysical, restoreAppearance);
		if (!restoreAppearance || !(_light != null))
		{
			return;
		}
		if (jc["on"] != null)
		{
			on = jc["on"].AsBool;
		}
		else
		{
			on = true;
		}
		if (jc["intensity"] != null)
		{
			intensity = jc["intensity"].AsFloat;
		}
		else if (intensitySlider != null)
		{
			SliderControl component = intensitySlider.GetComponent<SliderControl>();
			if (component != null)
			{
				intensity = component.defaultValue;
			}
		}
		if (jc["range"] != null)
		{
			range = jc["range"].AsFloat;
		}
		else if (rangeSlider != null)
		{
			SliderControl component2 = rangeSlider.GetComponent<SliderControl>();
			if (component2 != null)
			{
				range = component2.defaultValue;
			}
		}
		if (jc["spotAngle"] != null)
		{
			spotAngle = jc["spotAngle"].AsFloat;
		}
		else if (spotAngleSlider != null)
		{
			SliderControl component3 = spotAngleSlider.GetComponent<SliderControl>();
			if (component3 != null)
			{
				spotAngle = component3.defaultValue;
			}
		}
		if (jc["shadowsOn"] != null)
		{
			shadowsOn = jc["shadowsOn"].AsBool;
		}
		else
		{
			shadowsOn = true;
		}
		if (jc["showHalo"] != null)
		{
			showHalo = jc["showHalo"].AsBool;
		}
		else
		{
			showHalo = true;
		}
		if (jc["showDust"] != null)
		{
			showDust = jc["showDust"].AsBool;
		}
		else
		{
			showDust = true;
		}
		if (jc["shadowStrength"] != null)
		{
			shadowStrength = jc["shadowStrength"].AsFloat;
		}
		else if (shadowStrengthSlider != null)
		{
			SliderControl component4 = shadowStrengthSlider.GetComponent<SliderControl>();
			if (component4 != null)
			{
				shadowStrength = component4.defaultValue;
			}
		}
		if (jc["type"] != null)
		{
			SetLightType(jc["type"]);
		}
		else
		{
			SetLightType("Point");
		}
		if (jc["renderType"] != null)
		{
			SetRenderType(jc["renderType"]);
		}
		else
		{
			SetRenderType("Auto");
		}
		if (jc["color"] != null)
		{
			if (jc["color"]["h"] != null && colorPicker != null)
			{
				colorPicker.hue = jc["color"]["h"].AsFloat;
			}
			if (jc["color"]["s"] != null && colorPicker != null)
			{
				colorPicker.saturation = jc["color"]["s"].AsFloat;
			}
			if (jc["color"]["v"] != null && colorPicker != null)
			{
				colorPicker.cvalue = jc["color"]["v"].AsFloat;
			}
		}
		else
		{
			colorPicker.Reset();
		}
	}

	public void OnColorChange(Color c)
	{
		if (_light != null)
		{
			_light.color = c;
		}
		if (colorPickerAlt != null)
		{
			colorPickerAlt.hue = colorPicker.hue;
			colorPickerAlt.saturation = colorPicker.saturation;
			colorPickerAlt.cvalue = colorPicker.cvalue;
		}
		SyncEmissiveRenderers();
	}

	public void OnColorChangeAlt(Color c)
	{
		if (_light != null)
		{
			_light.color = c;
		}
		if (colorPicker != null)
		{
			colorPicker.hue = colorPickerAlt.hue;
			colorPicker.saturation = colorPickerAlt.saturation;
			colorPicker.cvalue = colorPickerAlt.cvalue;
		}
		SyncEmissiveRenderers();
	}

	protected void SyncEmissiveRenderers()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		MeshRenderer[] array = emissiveRenderers;
		foreach (MeshRenderer meshRenderer in array)
		{
			meshRenderer.enabled = on;
			if (meshRenderer.material != null)
			{
				meshRenderer.material.color = _light.color * _light.intensity + Color.gray;
			}
		}
	}

	protected void SyncShowHalo()
	{
		Behaviour behaviour = (Behaviour)GetComponent("Halo");
		if (behaviour != null)
		{
			behaviour.enabled = _showHalo;
		}
	}

	protected void SyncShowDust()
	{
		ParticleSystem component = GetComponent<ParticleSystem>();
		if (component != null)
		{
			ParticleSystem.EmissionModule emission = component.emission;
			emission.enabled = _showDust;
		}
	}

	protected void SetAutoShadowType()
	{
		if (controlBias)
		{
			switch (_light.type)
			{
			case LightType.Directional:
				_light.shadowBias = directionalBias;
				break;
			case LightType.Point:
				_light.shadowBias = pointBias;
				break;
			case LightType.Spot:
				_light.shadowBias = spotBias;
				break;
			}
		}
		if (controlNearPlane)
		{
			switch (_light.type)
			{
			case LightType.Directional:
				_light.shadowNearPlane = directionalNearPlane;
				break;
			case LightType.Point:
				_light.shadowNearPlane = pointNearPlane;
				break;
			case LightType.Spot:
				_light.shadowNearPlane = spotNearPlane;
				break;
			}
		}
		if (!autoShadowType)
		{
			return;
		}
		if (_light.type == LightType.Directional)
		{
			saveShadowType = LightShadows.Soft;
			if (_light.shadows != 0)
			{
				SetShadowType("Soft");
			}
		}
		else
		{
			saveShadowType = LightShadows.Hard;
			if (_light.shadows != 0)
			{
				SetShadowType("Hard");
			}
		}
	}

	public void SetLightType(string type)
	{
		if (!(_light != null))
		{
			return;
		}
		try
		{
			LightType lightType = (LightType)Enum.Parse(typeof(LightType), type);
			if (_light.type != lightType)
			{
				_light.type = lightType;
				if (typeSelector != null)
				{
					typeSelector.currentValue = type;
				}
				SetAutoShadowType();
			}
		}
		catch (ArgumentException)
		{
			Debug.LogError("Attempted to set light type to " + type + " which is not a valid light type");
		}
	}

	public void SetShadowType(string type)
	{
		if (!(_light != null))
		{
			return;
		}
		try
		{
			LightShadows lightShadows = (LightShadows)Enum.Parse(typeof(LightShadows), type);
			if (_light.shadows != lightShadows)
			{
				_light.shadows = lightShadows;
				if (shadowTypeSelector != null)
				{
					shadowTypeSelector.currentValue = type;
				}
			}
		}
		catch (ArgumentException)
		{
			Debug.LogError("Attempted to set light shadow type to " + type + " which is not a valid light shadow type");
		}
	}

	public void SetRenderType(string type)
	{
		if (!(_light != null))
		{
			return;
		}
		try
		{
			LightRenderMode lightRenderMode = (LightRenderMode)Enum.Parse(typeof(LightRenderMode), type);
			if (_light.renderMode != lightRenderMode)
			{
				_light.renderMode = lightRenderMode;
				if (renderModeSelector != null)
				{
					renderModeSelector.currentValue = type;
				}
			}
		}
		catch (ArgumentException)
		{
			Debug.LogError("Attempted to set light render type to " + type + " which is not a valid light render type");
		}
	}

	public void ResetColor()
	{
		if (colorPicker != null)
		{
			colorPicker.Reset();
		}
	}

	protected void InitUI()
	{
		if (onToggle != null)
		{
			onToggle.isOn = on;
			onToggle.onValueChanged.AddListener(delegate
			{
				on = onToggle.isOn;
			});
		}
		if (shadowsToggle != null)
		{
			shadowsToggle.isOn = shadowsOn;
			shadowsToggle.onValueChanged.AddListener(delegate
			{
				shadowsOn = shadowsToggle.isOn;
			});
		}
		if (showHaloToggle != null)
		{
			showHaloToggle.isOn = _showHalo;
			showHaloToggle.onValueChanged.AddListener(delegate
			{
				showHalo = showHaloToggle.isOn;
			});
			SyncShowHalo();
		}
		if (showDustToggle != null)
		{
			showDustToggle.isOn = _showDust;
			showDustToggle.onValueChanged.AddListener(delegate
			{
				showDust = showDustToggle.isOn;
			});
			SyncShowDust();
		}
		if (typeSelector != null)
		{
			if (_light != null)
			{
				typeSelector.currentValue = _light.type.ToString();
			}
			UIPopup uIPopup = typeSelector;
			uIPopup.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup.onValueChangeHandlers, new UIPopup.OnValueChange(SetLightType));
		}
		if (shadowTypeSelector != null)
		{
			UIPopup uIPopup2 = shadowTypeSelector;
			uIPopup2.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup2.onValueChangeHandlers, new UIPopup.OnValueChange(SetShadowType));
		}
		if (renderModeSelector != null)
		{
			if (_light != null)
			{
				renderModeSelector.currentValue = _light.renderMode.ToString();
			}
			UIPopup uIPopup3 = renderModeSelector;
			uIPopup3.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup3.onValueChangeHandlers, new UIPopup.OnValueChange(SetRenderType));
		}
		if (intensitySlider != null)
		{
			intensitySlider.value = intensity;
			SliderControl component = intensitySlider.GetComponent<SliderControl>();
			if (component != null)
			{
				component.defaultValue = intensity;
			}
			intensitySlider.onValueChanged.AddListener(delegate
			{
				intensity = intensitySlider.value;
			});
		}
		if (intensitySliderAlt != null)
		{
			intensitySliderAlt.value = intensity;
			SliderControl component2 = intensitySliderAlt.GetComponent<SliderControl>();
			if (component2 != null)
			{
				component2.defaultValue = intensity;
			}
			intensitySliderAlt.onValueChanged.AddListener(delegate
			{
				intensity = intensitySliderAlt.value;
			});
		}
		if (rangeSlider != null)
		{
			rangeSlider.value = range;
			SliderControl component3 = rangeSlider.GetComponent<SliderControl>();
			if (component3 != null)
			{
				component3.defaultValue = range;
			}
			rangeSlider.onValueChanged.AddListener(delegate
			{
				range = rangeSlider.value;
			});
		}
		if (rangeSliderAlt != null)
		{
			rangeSliderAlt.value = range;
			SliderControl component4 = rangeSliderAlt.GetComponent<SliderControl>();
			if (component4 != null)
			{
				component4.defaultValue = range;
			}
			rangeSliderAlt.onValueChanged.AddListener(delegate
			{
				range = rangeSliderAlt.value;
			});
		}
		if (shadowStrengthSlider != null)
		{
			shadowStrengthSlider.value = shadowStrength;
			SliderControl component5 = shadowStrengthSlider.GetComponent<SliderControl>();
			if (component5 != null)
			{
				component5.defaultValue = shadowStrength;
			}
			shadowStrengthSlider.onValueChanged.AddListener(delegate
			{
				shadowStrength = shadowStrengthSlider.value;
			});
		}
		if (spotAngleSlider != null)
		{
			spotAngleSlider.value = spotAngle;
			SliderControl component6 = spotAngleSlider.GetComponent<SliderControl>();
			if (component6 != null)
			{
				component6.defaultValue = spotAngle;
			}
			spotAngleSlider.onValueChanged.AddListener(delegate
			{
				spotAngle = spotAngleSlider.value;
			});
		}
		if (colorPicker != null)
		{
			HSVColorPicker hSVColorPicker = colorPicker;
			hSVColorPicker.onColorChangedHandlers = (HSVColorPicker.OnColorChanged)Delegate.Combine(hSVColorPicker.onColorChangedHandlers, new HSVColorPicker.OnColorChanged(OnColorChange));
			OnColorChange(colorPicker.currentColor);
		}
		if (colorPickerAlt != null)
		{
			HSVColorPicker hSVColorPicker2 = colorPickerAlt;
			hSVColorPicker2.onColorChangedHandlers = (HSVColorPicker.OnColorChanged)Delegate.Combine(hSVColorPicker2.onColorChangedHandlers, new HSVColorPicker.OnColorChanged(OnColorChangeAlt));
		}
	}

	private void Awake()
	{
		_light = GetComponent<Light>();
		if (_light.shadows != 0)
		{
			saveShadowType = _light.shadows;
		}
		else
		{
			saveShadowType = LightShadows.Hard;
		}
		if (Application.isPlaying)
		{
			InitUI();
		}
	}
}
