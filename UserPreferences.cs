using System;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;

[ExecuteInEditMode]
public class UserPreferences : MonoBehaviour
{
	private class QualityLevel
	{
		public float renderScale;

		public int msaaLevel;

		public ShadowResolution shadowResolution;

		public int pixelLightCount;

		public ShaderLOD shaderLOD;

		public int smoothPasses;

		public bool mirrorReflections;

		public bool closeObjectBlur;

		public bool softPhysics;

		public PhysicsRate physicsRate;
	}

	private class QualityLevels
	{
		public static Dictionary<string, QualityLevel> levels = new Dictionary<string, QualityLevel>
		{
			{
				"UltraLow",
				new QualityLevel
				{
					renderScale = 1f,
					msaaLevel = 0,
					shadowResolution = ShadowResolution.Low,
					pixelLightCount = 0,
					shaderLOD = ShaderLOD.Low,
					smoothPasses = 1,
					mirrorReflections = false,
					closeObjectBlur = false,
					softPhysics = false,
					physicsRate = PhysicsRate.High
				}
			},
			{
				"Low",
				new QualityLevel
				{
					renderScale = 1f,
					msaaLevel = 2,
					shadowResolution = ShadowResolution.Low,
					pixelLightCount = 1,
					shaderLOD = ShaderLOD.Low,
					smoothPasses = 1,
					mirrorReflections = false,
					closeObjectBlur = false,
					softPhysics = false,
					physicsRate = PhysicsRate.High
				}
			},
			{
				"Mid",
				new QualityLevel
				{
					renderScale = 1f,
					msaaLevel = 8,
					shadowResolution = ShadowResolution.Medium,
					pixelLightCount = 2,
					shaderLOD = ShaderLOD.High,
					smoothPasses = 1,
					mirrorReflections = false,
					closeObjectBlur = false,
					softPhysics = true,
					physicsRate = PhysicsRate.High
				}
			},
			{
				"High",
				new QualityLevel
				{
					renderScale = 1.25f,
					msaaLevel = 4,
					shadowResolution = ShadowResolution.High,
					pixelLightCount = 3,
					shaderLOD = ShaderLOD.High,
					smoothPasses = 2,
					mirrorReflections = true,
					closeObjectBlur = false,
					softPhysics = true,
					physicsRate = PhysicsRate.High
				}
			},
			{
				"Ultra",
				new QualityLevel
				{
					renderScale = 1.5f,
					msaaLevel = 2,
					shadowResolution = ShadowResolution.VeryHigh,
					pixelLightCount = 4,
					shaderLOD = ShaderLOD.High,
					smoothPasses = 3,
					mirrorReflections = true,
					closeObjectBlur = true,
					softPhysics = true,
					physicsRate = PhysicsRate.High
				}
			},
			{
				"Max",
				new QualityLevel
				{
					renderScale = 2f,
					msaaLevel = 2,
					shadowResolution = ShadowResolution.VeryHigh,
					pixelLightCount = 4,
					shaderLOD = ShaderLOD.High,
					smoothPasses = 4,
					mirrorReflections = true,
					closeObjectBlur = true,
					softPhysics = true,
					physicsRate = PhysicsRate.High
				}
			}
		};
	}

	public enum ShaderLOD
	{
		Low = 250,
		Medium = 400,
		High = 600
	}

	public enum PhysicsRate
	{
		Low,
		Mid,
		High
	}

	public static UserPreferences singleton;

	private bool _disableSave;

	private bool _disableToggles;

	public Toggle ultraLowQualityToggle;

	public Toggle lowQualityToggle;

	public Toggle midQualityToggle;

	public Toggle highQualityToggle;

	public Toggle ultraQualityToggle;

	public Toggle maxQualityToggle;

	public Toggle customQualityToggle;

	public Slider renderScaleSlider;

	[SerializeField]
	private float _renderScale = 1f;

	public UIPopup msaaPopup;

	[SerializeField]
	private int _msaaLevel = 8;

	public UIPopup shadowResolutionPopup;

	[SerializeField]
	private ShadowResolution _shadowResolution;

	public UIPopup smoothPassesPopup;

	[SerializeField]
	private int _smoothPasses = 3;

	public UIPopup pixelLightCountPopup;

	[SerializeField]
	private int _pixelLightCount = 4;

	public UIPopup shaderLODPopup;

	[SerializeField]
	private ShaderLOD _shaderLOD = ShaderLOD.High;

	public Camera normalCamera;

	public Camera mirrorReflectionCamera1;

	public Camera mirrorReflectionCamera2;

	public Toggle mirrorReflectionsToggle;

	[SerializeField]
	private bool _mirrorReflections = true;

	public Toggle mirrorToggle;

	[SerializeField]
	private bool _mirrorToDisplay;

	public Slider targetAlphaSlider;

	[SerializeField]
	private float _targetAlpha = 1f;

	public Slider pointLightShadowBlurSlider;

	[SerializeField]
	private float _pointLightShadowBlur = 0.5f;

	public Slider pointLightShadowBiasBaseSlider;

	[SerializeField]
	private float _pointLightShadowBiasBase = 0.01f;

	public Slider shadowFilterLevelSlider;

	[SerializeField]
	private float _shadowFilterLevel = 3f;

	public Toggle closeObjectBlurToggle;

	[SerializeField]
	private bool _closeObjectBlur = true;

	public Toggle softPhysicsToggle;

	[SerializeField]
	private bool _softPhysics = true;

	public UIPopup physicsRatePopup;

	[SerializeField]
	private PhysicsRate _physicsRate = PhysicsRate.High;

	public SteamVR_PlayArea steamPlayArea;

	public HSVColorPicker boundaryColorPicker;

	public Toggle showBoundaryToggle;

	[SerializeField]
	private bool _showBoundary = true;

	public float renderScale
	{
		get
		{
			return _renderScale;
		}
		set
		{
			if (_renderScale != value)
			{
				_renderScale = value;
				if (renderScaleSlider != null)
				{
					renderScaleSlider.value = value;
				}
				SyncRenderScale();
				SavePreferences();
			}
		}
	}

	public int msaaLevel
	{
		get
		{
			return _msaaLevel;
		}
		set
		{
			if (_msaaLevel != value && (value == 0 || value == 2 || value == 4 || value == 8))
			{
				_msaaLevel = value;
				SyncMsaa();
				SyncMsaaPopup();
				SavePreferences();
			}
		}
	}

	public ShadowResolution shadowResolution
	{
		get
		{
			return _shadowResolution;
		}
		set
		{
			if (_shadowResolution != value)
			{
				_shadowResolution = value;
				SyncShadowResolution();
				SyncShadowResolutionPopup();
				SavePreferences();
			}
		}
	}

	public int smoothPasses
	{
		get
		{
			return _smoothPasses;
		}
		set
		{
			if (_smoothPasses != value && value >= 0 && value <= 4)
			{
				_smoothPasses = value;
				SyncSmoothPassesPopup();
				SavePreferences();
			}
		}
	}

	public int pixelLightCount
	{
		get
		{
			return _pixelLightCount;
		}
		set
		{
			if (_pixelLightCount != value)
			{
				_pixelLightCount = value;
				SyncPixelLightCount();
				if (pixelLightCountPopup != null)
				{
					pixelLightCountPopup.currentValue = pixelLightCount.ToString();
				}
				SavePreferences();
			}
		}
	}

	public ShaderLOD shaderLOD
	{
		get
		{
			return _shaderLOD;
		}
		set
		{
			if (_shaderLOD != value)
			{
				_shaderLOD = value;
				SetInternalShaderLOD();
				if (shaderLODPopup != null)
				{
					shaderLODPopup.currentValue = _shaderLOD.ToString();
				}
			}
		}
	}

	public bool mirrorReflections
	{
		get
		{
			return _mirrorReflections;
		}
		set
		{
			if (_mirrorReflections != value)
			{
				_mirrorReflections = value;
				if (mirrorReflectionsToggle != null)
				{
					mirrorReflectionsToggle.isOn = _mirrorReflections;
				}
				SyncMirrorReflections();
				SavePreferences();
			}
		}
	}

	public bool mirrorToDisplay
	{
		get
		{
			return _mirrorToDisplay;
		}
		set
		{
			if (_mirrorToDisplay != value)
			{
				_mirrorToDisplay = value;
				if (mirrorToggle != null)
				{
					mirrorToggle.isOn = _mirrorToDisplay;
				}
				SyncMirrorToDisplay();
				SavePreferences();
			}
		}
	}

	public float targetAlpha
	{
		get
		{
			return _targetAlpha;
		}
		set
		{
			if (_targetAlpha != value)
			{
				_targetAlpha = value;
				FreeControllerV3.targetAlpha = _targetAlpha;
				SelectTarget.useGlobalAlpha = true;
				SelectTarget.globalAlpha = _targetAlpha;
				if (targetAlphaSlider != null)
				{
					targetAlphaSlider.value = _targetAlpha;
				}
				SavePreferences();
			}
		}
	}

	public float pointLightShadowBlur
	{
		get
		{
			return _pointLightShadowBlur;
		}
		set
		{
			if (_pointLightShadowBlur != value)
			{
				_pointLightShadowBlur = value;
				SyncShadowBlur();
				if (pointLightShadowBlurSlider != null)
				{
					pointLightShadowBlurSlider.value = _pointLightShadowBlur;
				}
				SavePreferences();
			}
		}
	}

	public float pointLightShadowBiasBase
	{
		get
		{
			return _pointLightShadowBiasBase;
		}
		set
		{
			if (_pointLightShadowBiasBase != value)
			{
				_pointLightShadowBiasBase = value;
				SyncShadowBiasBase();
				if (pointLightShadowBiasBaseSlider != null)
				{
					pointLightShadowBiasBaseSlider.value = _pointLightShadowBiasBase;
				}
				SavePreferences();
			}
		}
	}

	public float shadowFilterLevel
	{
		get
		{
			return _shadowFilterLevel;
		}
		set
		{
			if (_shadowFilterLevel != value)
			{
				_shadowFilterLevel = value;
				SyncShadowFilterLevel();
				if (shadowFilterLevelSlider != null)
				{
					shadowFilterLevelSlider.value = _shadowFilterLevel;
				}
			}
		}
	}

	public bool closeObjectBlur
	{
		get
		{
			return _closeObjectBlur;
		}
		set
		{
			if (_closeObjectBlur != value)
			{
				_closeObjectBlur = value;
				if (closeObjectBlurToggle != null)
				{
					closeObjectBlurToggle.isOn = _closeObjectBlur;
				}
				SyncCloseObjectBlur();
				SavePreferences();
			}
		}
	}

	public bool softPhysics
	{
		get
		{
			return _softPhysics;
		}
		set
		{
			if (_softPhysics != value)
			{
				_softPhysics = value;
				if (softPhysicsToggle != null)
				{
					softPhysicsToggle.isOn = _softPhysics;
				}
				SyncSoftPhysics();
				SavePreferences();
			}
		}
	}

	public PhysicsRate physicsRate
	{
		get
		{
			return _physicsRate;
		}
		set
		{
			if (_physicsRate != value)
			{
				_physicsRate = value;
				SyncPhysics();
				if (physicsRatePopup != null)
				{
					physicsRatePopup.currentValue = _physicsRate.ToString();
				}
			}
		}
	}

	public bool showBoundary
	{
		get
		{
			return _showBoundary;
		}
		set
		{
			if (_showBoundary != value)
			{
				_showBoundary = value;
				if (showBoundaryToggle != null)
				{
					showBoundaryToggle.isOn = _showBoundary;
				}
				SyncShowBoundary();
				SavePreferences();
			}
		}
	}

	public void SetQuality(string qualityName)
	{
		if (QualityLevels.levels.TryGetValue(qualityName, out var value))
		{
			_disableSave = true;
			renderScale = value.renderScale;
			msaaLevel = value.msaaLevel;
			shadowResolution = value.shadowResolution;
			pixelLightCount = value.pixelLightCount;
			shaderLOD = value.shaderLOD;
			smoothPasses = value.smoothPasses;
			mirrorReflections = value.mirrorReflections;
			closeObjectBlur = value.closeObjectBlur;
			softPhysics = value.softPhysics;
			physicsRate = value.physicsRate;
			_disableSave = false;
			SavePreferences();
		}
		else
		{
			Debug.LogError("Could not find quality level " + qualityName);
		}
	}

	private void CheckQualityLevels()
	{
		bool flag = CheckQualityLevel("UltraLow");
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		bool flag6 = false;
		bool isOn = false;
		if (!flag)
		{
			flag2 = CheckQualityLevel("Low");
			if (!flag2)
			{
				flag3 = CheckQualityLevel("Mid");
				if (!flag3)
				{
					flag4 = CheckQualityLevel("High");
					if (!flag4)
					{
						flag5 = CheckQualityLevel("Ultra");
						if (!flag5)
						{
							flag6 = CheckQualityLevel("Max");
							if (!flag6)
							{
								isOn = true;
							}
						}
					}
				}
			}
		}
		_disableToggles = true;
		if (ultraLowQualityToggle != null)
		{
			ultraLowQualityToggle.isOn = flag;
		}
		else if (flag)
		{
			isOn = true;
		}
		if (lowQualityToggle != null)
		{
			lowQualityToggle.isOn = flag2;
		}
		else if (flag2)
		{
			isOn = true;
		}
		if (midQualityToggle != null)
		{
			midQualityToggle.isOn = flag3;
		}
		else if (flag3)
		{
			isOn = true;
		}
		if (highQualityToggle != null)
		{
			highQualityToggle.isOn = flag4;
		}
		else if (flag4)
		{
			isOn = true;
		}
		if (ultraQualityToggle != null)
		{
			ultraQualityToggle.isOn = flag5;
		}
		else if (flag5)
		{
			isOn = true;
		}
		if (maxQualityToggle != null)
		{
			maxQualityToggle.isOn = flag6;
		}
		else if (flag6)
		{
			isOn = true;
		}
		if (customQualityToggle != null)
		{
			customQualityToggle.isOn = isOn;
		}
		_disableToggles = false;
	}

	private bool CheckQualityLevel(string qualityName)
	{
		bool result = false;
		if (QualityLevels.levels.TryGetValue(qualityName, out var value))
		{
			result = true;
			if (_renderScale != value.renderScale || _msaaLevel != value.msaaLevel || _shadowResolution != value.shadowResolution || _pixelLightCount != value.pixelLightCount || _shaderLOD != value.shaderLOD || _smoothPasses != value.smoothPasses || _mirrorReflections != value.mirrorReflections || _closeObjectBlur != value.closeObjectBlur || _physicsRate != value.physicsRate || _softPhysics != value.softPhysics)
			{
				result = false;
			}
		}
		else
		{
			Debug.LogError("Could not find quality level " + qualityName);
		}
		return result;
	}

	public void SavePreferences()
	{
		if (!_disableSave)
		{
			JSONClass jSONClass = new JSONClass();
			jSONClass["renderScale"].AsFloat = _renderScale;
			jSONClass["msaaLevel"].AsInt = _msaaLevel;
			jSONClass["shadowResolution"] = _shadowResolution.ToString();
			jSONClass["pixelLightCount"].AsInt = _pixelLightCount;
			jSONClass["shaderLOD"] = _shaderLOD.ToString();
			jSONClass["smoothPasses"].AsInt = _smoothPasses;
			jSONClass["mirrorReflections"].AsBool = _mirrorReflections;
			jSONClass["mirrorToDisplay"].AsBool = _mirrorToDisplay;
			jSONClass["targetAlpha"].AsFloat = _targetAlpha;
			jSONClass["showBoundary"].AsBool = _showBoundary;
			jSONClass["softBodyPhysics"].AsBool = _softPhysics;
			if (boundaryColorPicker != null && (boundaryColorPicker.hue != boundaryColorPicker.defaultHue || boundaryColorPicker.saturation != boundaryColorPicker.defaultSaturation || boundaryColorPicker.cvalue != boundaryColorPicker.defaultCvalue))
			{
				jSONClass["boundaryColor"]["h"].AsFloat = boundaryColorPicker.hue;
				jSONClass["boundaryColor"]["s"].AsFloat = boundaryColorPicker.saturation;
				jSONClass["boundaryColor"]["v"].AsFloat = boundaryColorPicker.cvalue;
			}
			string value = jSONClass.ToString(string.Empty);
			StreamWriter streamWriter = new StreamWriter("prefs.json");
			streamWriter.Write(value);
			streamWriter.Close();
			CheckQualityLevels();
		}
	}

	public void RestorePreferences()
	{
		string path = "prefs.json";
		_disableSave = true;
		if (File.Exists(path))
		{
			StreamReader streamReader = new StreamReader(path);
			string aJSON = streamReader.ReadToEnd();
			streamReader.Close();
			JSONNode jSONNode = JSON.Parse(aJSON);
			if (jSONNode["renderScale"] != null)
			{
				renderScale = jSONNode["renderScale"].AsFloat;
			}
			if (jSONNode["msaaLevel"] != null)
			{
				msaaLevel = jSONNode["msaaLevel"].AsInt;
			}
			if (jSONNode["shadowResolution"] != null)
			{
				SetShadowResolutionFromString(jSONNode["shadowResolution"]);
			}
			if (jSONNode["pixelLightCount"] != null)
			{
				pixelLightCount = jSONNode["pixelLightCount"].AsInt;
			}
			if (jSONNode["shaderLOD"] != null)
			{
				SetShaderLODFromString(jSONNode["shaderLOD"]);
			}
			if (jSONNode["smoothPasses"] != null)
			{
				smoothPasses = jSONNode["smoothPasses"].AsInt;
			}
			if (jSONNode["mirrorToDisplay"] != null)
			{
				mirrorToDisplay = jSONNode["mirrorToDisplay"].AsBool;
			}
			if (jSONNode["mirrorReflections"] != null)
			{
				mirrorReflections = jSONNode["mirrorReflections"].AsBool;
			}
			if (jSONNode["targetAlpha"] != null)
			{
				targetAlpha = jSONNode["targetAlpha"].AsFloat;
			}
			if (jSONNode["shadowFilterLevel"] != null)
			{
				shadowFilterLevel = jSONNode["shadowFilterLevel"].AsFloat;
			}
			if (jSONNode["pointLightShadowBlur"] != null)
			{
				pointLightShadowBlur = jSONNode["pointLightShadowBlur"].AsFloat;
			}
			if (jSONNode["pointLightShadowBiasBase"] != null)
			{
				pointLightShadowBiasBase = jSONNode["pointLightShadowBiasBase"].AsFloat;
			}
			if (jSONNode["showBoundary"] != null)
			{
				showBoundary = jSONNode["showBoundary"].AsBool;
			}
			if (jSONNode["boundaryColor"] != null)
			{
				if (jSONNode["boundaryColor"]["h"] != null && boundaryColorPicker != null)
				{
					boundaryColorPicker.hue = jSONNode["boundaryColor"]["h"].AsFloat;
				}
				if (jSONNode["boundaryColor"]["s"] != null && boundaryColorPicker != null)
				{
					boundaryColorPicker.saturation = jSONNode["boundaryColor"]["s"].AsFloat;
				}
				if (jSONNode["boundaryColor"]["v"] != null && boundaryColorPicker != null)
				{
					boundaryColorPicker.cvalue = jSONNode["boundaryColor"]["v"].AsFloat;
				}
			}
			if (jSONNode["softBodyPhysics"] != null)
			{
				softPhysics = jSONNode["softBodyPhysics"].AsBool;
			}
		}
		_disableSave = false;
		CheckQualityLevels();
	}

	public void ResetPreferences()
	{
		SetQuality("High");
		_disableSave = true;
		mirrorToDisplay = false;
		targetAlpha = 1f;
		shadowFilterLevel = 3f;
		pointLightShadowBlur = 0.5f;
		pointLightShadowBiasBase = 0.015f;
		showBoundary = true;
		_disableSave = false;
		SavePreferences();
	}

	private void SyncRenderScale()
	{
		if (VRSettings.get_renderScale() != _renderScale)
		{
			VRSettings.set_renderScale(_renderScale);
		}
		if (SteamVR_Camera.sceneResolutionScale != _renderScale)
		{
			SteamVR_Camera.sceneResolutionScale = _renderScale;
		}
	}

	private void SyncMsaa()
	{
		if (QualitySettings.antiAliasing != _msaaLevel)
		{
			QualitySettings.antiAliasing = _msaaLevel;
		}
	}

	private void SyncMsaaPopup()
	{
		if (msaaPopup != null)
		{
			switch (_msaaLevel)
			{
			case 0:
				msaaPopup.currentValue = "Off";
				break;
			case 2:
				msaaPopup.currentValue = "2X";
				break;
			case 4:
				msaaPopup.currentValue = "4X";
				break;
			case 8:
				msaaPopup.currentValue = "8X";
				break;
			}
		}
	}

	public void SetMsaaFromString(string levelString)
	{
		switch (levelString)
		{
		case "Off":
			msaaLevel = 0;
			break;
		case "2X":
			msaaLevel = 2;
			break;
		case "4X":
			msaaLevel = 4;
			break;
		case "8X":
			msaaLevel = 8;
			break;
		}
	}

	private void SyncShadowResolution()
	{
		if (QualitySettings.shadowResolution != _shadowResolution)
		{
			QualitySettings.shadowResolution = _shadowResolution;
		}
	}

	private void SyncShadowResolutionPopup()
	{
		if (shadowResolutionPopup != null)
		{
			switch (_shadowResolution)
			{
			case ShadowResolution.Low:
				shadowResolutionPopup.currentValue = "Low";
				break;
			case ShadowResolution.Medium:
				shadowResolutionPopup.currentValue = "Medium";
				break;
			case ShadowResolution.High:
				shadowResolutionPopup.currentValue = "High";
				break;
			case ShadowResolution.VeryHigh:
				shadowResolutionPopup.currentValue = "VeryHigh";
				break;
			}
		}
	}

	public void SetShadowResolutionFromString(string levelString)
	{
		switch (levelString)
		{
		case "Low":
			shadowResolution = ShadowResolution.Low;
			break;
		case "Medium":
			shadowResolution = ShadowResolution.Medium;
			break;
		case "High":
			shadowResolution = ShadowResolution.High;
			break;
		case "VeryHigh":
			shadowResolution = ShadowResolution.VeryHigh;
			break;
		}
	}

	private void SyncSmoothPassesPopup()
	{
		if (smoothPassesPopup != null)
		{
			smoothPassesPopup.currentValue = _smoothPasses.ToString();
		}
	}

	public void SetSmoothPassesFromString(string levelString)
	{
		try
		{
			smoothPasses = int.Parse(levelString);
		}
		catch (FormatException)
		{
			Debug.LogError("Attempted to set smooth passes to " + levelString + " which is not an int");
		}
	}

	private void SyncPixelLightCount()
	{
		if (QualitySettings.pixelLightCount != _pixelLightCount)
		{
			QualitySettings.pixelLightCount = _pixelLightCount;
		}
	}

	public void SetPixelLightCountFromString(string countString)
	{
		try
		{
			pixelLightCount = int.Parse(countString);
		}
		catch (FormatException)
		{
			Debug.LogError("Attempted to set pixel light count to " + countString + " which is not an int");
		}
	}

	private void SetInternalShaderLOD()
	{
		Shader.globalMaximumLOD = (int)_shaderLOD;
	}

	public void SetShaderLODFromString(string lod)
	{
		try
		{
			shaderLOD = (ShaderLOD)Enum.Parse(typeof(ShaderLOD), lod);
		}
		catch (ArgumentException)
		{
			Debug.LogError("Attempted to set shader lod " + lod + " which is not a valid lod string");
		}
	}

	private void SyncMirrorReflections()
	{
		MirrorReflection.globalEnabled = _mirrorReflections;
		if (normalCamera != null)
		{
			normalCamera.enabled = !_mirrorReflections;
		}
		if (mirrorReflectionCamera1 != null)
		{
			mirrorReflectionCamera1.enabled = _mirrorReflections;
		}
		if (mirrorReflectionCamera2 != null)
		{
			mirrorReflectionCamera2.enabled = _mirrorReflections;
		}
	}

	private void SyncMirrorToDisplay()
	{
		if (VRSettings.get_showDeviceView() != _mirrorToDisplay)
		{
			VRSettings.set_showDeviceView(_mirrorToDisplay);
		}
	}

	private void SyncShadowBlur()
	{
		Shader.SetGlobalFloat("_ShadowPointKernel", _pointLightShadowBlur);
	}

	private void SyncShadowBiasBase()
	{
		Shader.SetGlobalFloat("_ShadowPointBiasBase", _pointLightShadowBiasBase);
	}

	private void SyncShadowFilterLevel()
	{
		int value = (int)_shadowFilterLevel;
		Shader.SetGlobalInt("_ShadowFilterLevel", value);
	}

	private void SyncCloseObjectBlur()
	{
	}

	private void SyncSoftPhysics()
	{
		DAZPhysicsMesh.globalEnable = _softPhysics;
	}

	private void SyncPhysics()
	{
		switch (_physicsRate)
		{
		case PhysicsRate.High:
			Time.fixedDeltaTime = 0.01111111f;
			break;
		case PhysicsRate.Mid:
			Time.fixedDeltaTime = 0.0125f;
			break;
		case PhysicsRate.Low:
			Time.fixedDeltaTime = 0.02222222f;
			break;
		}
		Time.maximumDeltaTime = Time.fixedDeltaTime;
	}

	public void SetPhysicsRateFromString(string pr)
	{
		try
		{
			physicsRate = (PhysicsRate)Enum.Parse(typeof(PhysicsRate), pr);
		}
		catch (ArgumentException)
		{
			Debug.LogError("Attempted to set physics rate " + pr + " which is not a valid physics rate string");
		}
	}

	private void SyncBoundaryColor(Color c)
	{
		if (!(steamPlayArea != null))
		{
			return;
		}
		Renderer component = steamPlayArea.gameObject.GetComponent<Renderer>();
		if (!(component != null))
		{
			return;
		}
		Material[] array = ((!Application.isPlaying) ? component.sharedMaterials : component.materials);
		Material[] array2 = array;
		foreach (Material material in array2)
		{
			if (material.HasProperty("_Color"))
			{
				material.SetColor("_Color", c);
			}
		}
	}

	private void SetBoundaryColor(Color c)
	{
		SyncBoundaryColor(c);
		SavePreferences();
	}

	private void SyncShowBoundary()
	{
		if (steamPlayArea != null)
		{
			MeshRenderer component = steamPlayArea.GetComponent<MeshRenderer>();
			if (component != null)
			{
				component.enabled = _showBoundary;
			}
		}
	}

	private void SyncShadows()
	{
		SyncShadowFilterLevel();
		SyncShadowBiasBase();
		SyncShadowBlur();
	}

	private void InitUI()
	{
		if (renderScaleSlider != null)
		{
			renderScaleSlider.value = renderScale;
			renderScaleSlider.onValueChanged.AddListener(delegate
			{
				renderScale = renderScaleSlider.value;
			});
			SliderControl component = renderScaleSlider.GetComponent<SliderControl>();
			if (component != null)
			{
				component.defaultValue = 1f;
			}
			SyncRenderScale();
		}
		if (msaaPopup != null)
		{
			UIPopup uIPopup = msaaPopup;
			uIPopup.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup.onValueChangeHandlers, new UIPopup.OnValueChange(SetMsaaFromString));
			SyncMsaaPopup();
			SyncMsaa();
		}
		if (shadowResolutionPopup != null)
		{
			UIPopup uIPopup2 = shadowResolutionPopup;
			uIPopup2.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup2.onValueChangeHandlers, new UIPopup.OnValueChange(SetShadowResolutionFromString));
			SyncShadowResolutionPopup();
			SyncShadowResolution();
		}
		if (smoothPassesPopup != null)
		{
			UIPopup uIPopup3 = smoothPassesPopup;
			uIPopup3.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup3.onValueChangeHandlers, new UIPopup.OnValueChange(SetSmoothPassesFromString));
			SyncSmoothPassesPopup();
		}
		if (pixelLightCountPopup != null)
		{
			UIPopup uIPopup4 = pixelLightCountPopup;
			uIPopup4.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup4.onValueChangeHandlers, new UIPopup.OnValueChange(SetPixelLightCountFromString));
			pixelLightCountPopup.currentValue = pixelLightCount.ToString();
			SyncPixelLightCount();
		}
		if (shaderLODPopup != null)
		{
			UIPopup uIPopup5 = shaderLODPopup;
			uIPopup5.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup5.onValueChangeHandlers, new UIPopup.OnValueChange(SetShaderLODFromString));
			shaderLODPopup.currentValue = _shaderLOD.ToString();
			SetInternalShaderLOD();
		}
		if (mirrorReflectionsToggle != null)
		{
			mirrorReflectionsToggle.onValueChanged.AddListener(delegate
			{
				mirrorReflections = mirrorReflectionsToggle.isOn;
			});
			mirrorReflectionsToggle.isOn = mirrorReflections;
			SyncMirrorReflections();
		}
		if (mirrorToggle != null)
		{
			mirrorToggle.onValueChanged.AddListener(delegate
			{
				mirrorToDisplay = mirrorToggle.isOn;
			});
			mirrorToggle.isOn = mirrorToDisplay;
			SyncMirrorToDisplay();
		}
		if (targetAlphaSlider != null)
		{
			targetAlphaSlider.value = targetAlpha;
			FreeControllerV3.targetAlpha = _targetAlpha;
			targetAlphaSlider.onValueChanged.AddListener(delegate
			{
				targetAlpha = targetAlphaSlider.value;
			});
			SliderControl component2 = targetAlphaSlider.GetComponent<SliderControl>();
			if (component2 != null)
			{
				component2.defaultValue = 1f;
			}
		}
		if (pointLightShadowBlurSlider != null)
		{
			pointLightShadowBlurSlider.value = pointLightShadowBlur;
			SyncShadowBlur();
			pointLightShadowBlurSlider.onValueChanged.AddListener(delegate
			{
				pointLightShadowBlur = pointLightShadowBlurSlider.value;
			});
			SliderControl component3 = pointLightShadowBlurSlider.GetComponent<SliderControl>();
			if (component3 != null)
			{
				component3.defaultValue = 0.5f;
			}
		}
		if (pointLightShadowBiasBaseSlider != null)
		{
			pointLightShadowBiasBaseSlider.value = pointLightShadowBiasBase;
			SyncShadowBiasBase();
			pointLightShadowBiasBaseSlider.onValueChanged.AddListener(delegate
			{
				pointLightShadowBiasBase = pointLightShadowBiasBaseSlider.value;
			});
			SliderControl component4 = pointLightShadowBiasBaseSlider.GetComponent<SliderControl>();
			if (component4 != null)
			{
				component4.defaultValue = 0.01f;
			}
		}
		if (shadowFilterLevelSlider != null)
		{
			shadowFilterLevelSlider.value = shadowFilterLevel;
			SyncShadowFilterLevel();
			shadowFilterLevelSlider.onValueChanged.AddListener(delegate
			{
				shadowFilterLevel = shadowFilterLevelSlider.value;
			});
			SliderControl component5 = shadowFilterLevelSlider.GetComponent<SliderControl>();
			if (component5 != null)
			{
				component5.defaultValue = 3f;
			}
		}
		if (closeObjectBlurToggle != null)
		{
			closeObjectBlurToggle.onValueChanged.AddListener(delegate
			{
				closeObjectBlur = closeObjectBlurToggle.isOn;
			});
			closeObjectBlurToggle.isOn = closeObjectBlur;
			SyncCloseObjectBlur();
		}
		if (physicsRatePopup != null)
		{
			UIPopup uIPopup6 = physicsRatePopup;
			uIPopup6.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup6.onValueChangeHandlers, new UIPopup.OnValueChange(SetPhysicsRateFromString));
			physicsRatePopup.currentValue = _physicsRate.ToString();
			SyncPhysics();
		}
		if (softPhysicsToggle != null)
		{
			softPhysicsToggle.onValueChanged.AddListener(delegate
			{
				softPhysics = softPhysicsToggle.isOn;
			});
			softPhysicsToggle.isOn = softPhysics;
			SyncSoftPhysics();
		}
		if (showBoundaryToggle != null)
		{
			showBoundaryToggle.onValueChanged.AddListener(delegate
			{
				showBoundary = showBoundaryToggle.isOn;
			});
			SyncShowBoundary();
		}
		if (boundaryColorPicker != null)
		{
			HSVColorPicker hSVColorPicker = boundaryColorPicker;
			hSVColorPicker.onColorChangedHandlers = (HSVColorPicker.OnColorChanged)Delegate.Combine(hSVColorPicker.onColorChangedHandlers, new HSVColorPicker.OnColorChanged(SetBoundaryColor));
			SyncBoundaryColor(boundaryColorPicker.currentColor);
		}
		if (ultraLowQualityToggle != null)
		{
			ultraLowQualityToggle.onValueChanged.AddListener(delegate
			{
				if (!_disableToggles && ultraLowQualityToggle.isOn)
				{
					SetQuality("UltraLow");
				}
			});
		}
		if (lowQualityToggle != null)
		{
			lowQualityToggle.onValueChanged.AddListener(delegate
			{
				if (!_disableToggles && lowQualityToggle.isOn)
				{
					SetQuality("Low");
				}
			});
		}
		if (midQualityToggle != null)
		{
			midQualityToggle.onValueChanged.AddListener(delegate
			{
				if (!_disableToggles && midQualityToggle.isOn)
				{
					SetQuality("Mid");
				}
			});
		}
		if (highQualityToggle != null)
		{
			highQualityToggle.onValueChanged.AddListener(delegate
			{
				if (!_disableToggles && highQualityToggle.isOn)
				{
					SetQuality("High");
				}
			});
		}
		if (ultraQualityToggle != null)
		{
			ultraQualityToggle.onValueChanged.AddListener(delegate
			{
				if (!_disableToggles && ultraQualityToggle.isOn)
				{
					SetQuality("Ultra");
				}
			});
		}
		if (maxQualityToggle != null)
		{
			maxQualityToggle.onValueChanged.AddListener(delegate
			{
				if (!_disableToggles && maxQualityToggle.isOn)
				{
					SetQuality("Max");
				}
			});
		}
		if (!(customQualityToggle != null))
		{
			return;
		}
		customQualityToggle.onValueChanged.AddListener(delegate
		{
			if (!_disableToggles && customQualityToggle.isOn)
			{
				CheckQualityLevels();
			}
		});
	}

	private void Start()
	{
		singleton = this;
		RestorePreferences();
		if (Application.isPlaying)
		{
			InitUI();
			CheckQualityLevels();
		}
		else
		{
			SyncShadows();
		}
	}

	private void OnEnable()
	{
		if (!Application.isPlaying)
		{
			SyncShadows();
		}
	}
}
