using System;
using System.Collections;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class MirrorReflectionOld : JSONStorable
{
	public Toggle disablePixelLightsToggle;

	[SerializeField]
	protected bool _disablePixelLights;

	public UIPopup textureSizePopup;

	private int _oldReflectionTextureSize;

	[SerializeField]
	protected int _textureSize = 1024;

	public UIPopup antiAliasingPopup;

	private int _oldAntiAliasing;

	[SerializeField]
	protected int _antiAliasing = 8;

	public Slider reflectionOpacitySlider;

	[SerializeField]
	protected float _reflectionOpacity = 0.5f;

	public Slider reflectionBlendSlider;

	[SerializeField]
	protected float _reflectionBlend = 1f;

	public HSVColorPicker reflectionColorPicker;

	public float m_ClipPlaneOffset;

	public LayerMask m_ReflectLayers = -1;

	private Hashtable m_ReflectionCameras = new Hashtable();

	private RenderTexture m_ReflectionTexture;

	private static bool s_InsideRendering;

	public bool disablePixelLights
	{
		get
		{
			return _disablePixelLights;
		}
		set
		{
			if (_disablePixelLights != value)
			{
				_disablePixelLights = value;
				if (disablePixelLightsToggle != null)
				{
					disablePixelLightsToggle.isOn = value;
				}
			}
		}
	}

	public int textureSize
	{
		get
		{
			return _textureSize;
		}
		set
		{
			if (_textureSize != value && (value == 1024 || value == 2048 || value == 4096))
			{
				_textureSize = value;
				if (textureSizePopup != null)
				{
					textureSizePopup.currentValue = _textureSize.ToString();
				}
			}
		}
	}

	public int antiAliasing
	{
		get
		{
			return _antiAliasing;
		}
		set
		{
			if (_antiAliasing != value && (value == 1 || value == 2 || value == 4 || value == 8))
			{
				_antiAliasing = value;
				if (antiAliasingPopup != null)
				{
					antiAliasingPopup.currentValue = _antiAliasing.ToString();
				}
			}
		}
	}

	public float reflectionOpacity
	{
		get
		{
			return _reflectionOpacity;
		}
		set
		{
			if (_reflectionOpacity == value)
			{
				return;
			}
			_reflectionOpacity = value;
			Renderer component = GetComponent<Renderer>();
			if (!(component != null))
			{
				return;
			}
			Material[] array = ((!Application.isPlaying) ? component.sharedMaterials : component.materials);
			Material[] array2 = array;
			foreach (Material material in array2)
			{
				if (material.HasProperty("_ReflectionOpacity"))
				{
					material.SetFloat("_ReflectionOpacity", _reflectionOpacity);
				}
			}
			if (reflectionOpacitySlider != null)
			{
				reflectionOpacitySlider.value = value;
			}
		}
	}

	public float reflectionBlend
	{
		get
		{
			return _reflectionBlend;
		}
		set
		{
			if (_reflectionBlend == value)
			{
				return;
			}
			_reflectionBlend = value;
			Renderer component = GetComponent<Renderer>();
			if (!(component != null))
			{
				return;
			}
			Material[] array = ((!Application.isPlaying) ? component.sharedMaterials : component.materials);
			Material[] array2 = array;
			foreach (Material material in array2)
			{
				if (material.HasProperty("_ReflectionBlendTexPower"))
				{
					material.SetFloat("_ReflectionBlendTexPower", _reflectionBlend);
				}
			}
			if (reflectionBlendSlider != null)
			{
				reflectionBlendSlider.value = value;
			}
		}
	}

	public override JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true)
	{
		JSONClass jSON = base.GetJSON(includePhysical, includeAppearance);
		if (includeAppearance)
		{
			if (_disablePixelLights)
			{
				needsStore = true;
				jSON["disablePixelLights"].AsBool = disablePixelLights;
			}
			if (_textureSize != 1024)
			{
				needsStore = true;
				jSON["textureSize"].AsInt = textureSize;
			}
			if (_antiAliasing != 8)
			{
				needsStore = true;
				jSON["antiAliasing"].AsInt = antiAliasing;
			}
			if (reflectionOpacitySlider != null)
			{
				SliderControl component = reflectionOpacitySlider.GetComponent<SliderControl>();
				if (component == null || component.defaultValue != reflectionOpacity)
				{
					needsStore = true;
					jSON["reflectionOpacity"].AsFloat = reflectionOpacity;
				}
			}
			if (reflectionBlendSlider != null)
			{
				SliderControl component2 = reflectionBlendSlider.GetComponent<SliderControl>();
				if (component2 == null || component2.defaultValue != reflectionBlend)
				{
					needsStore = true;
					jSON["reflectionBlend"].AsFloat = reflectionBlend;
				}
			}
			if (reflectionColorPicker != null)
			{
				needsStore = true;
				jSON["reflectionColor"]["h"].AsFloat = reflectionColorPicker.hue;
				jSON["reflectionColor"]["s"].AsFloat = reflectionColorPicker.saturation;
				jSON["reflectionColor"]["v"].AsFloat = reflectionColorPicker.cvalue;
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
		if (jc["disablePixelLights"] != null)
		{
			disablePixelLights = jc["disablePixelLights"].AsBool;
		}
		else
		{
			disablePixelLights = false;
		}
		if (jc["textureSize"] != null)
		{
			textureSize = jc["textureSize"].AsInt;
		}
		else
		{
			textureSize = 1024;
		}
		if (jc["antiAliasing"] != null)
		{
			antiAliasing = jc["antiAliasing"].AsInt;
		}
		else
		{
			antiAliasing = 8;
		}
		if (jc["reflectionOpacity"] != null)
		{
			reflectionOpacity = jc["reflectionOpacity"].AsFloat;
		}
		else if ((bool)reflectionOpacitySlider)
		{
			SliderControl component = reflectionOpacitySlider.GetComponent<SliderControl>();
			if (component != null)
			{
				reflectionOpacity = component.defaultValue;
			}
		}
		if (jc["reflectionBlend"] != null)
		{
			reflectionBlend = jc["reflectionBlend"].AsFloat;
		}
		else if ((bool)reflectionBlendSlider)
		{
			SliderControl component2 = reflectionBlendSlider.GetComponent<SliderControl>();
			if (component2 != null)
			{
				reflectionBlend = component2.defaultValue;
			}
		}
		if (jc["reflectionColor"] != null)
		{
			if (jc["reflectionColor"]["h"] != null && reflectionColorPicker != null)
			{
				reflectionColorPicker.hue = jc["reflectionColor"]["h"].AsFloat;
			}
			if (jc["reflectionColor"]["s"] != null && reflectionColorPicker != null)
			{
				reflectionColorPicker.saturation = jc["reflectionColor"]["s"].AsFloat;
			}
			if (jc["reflectionColor"]["v"] != null && reflectionColorPicker != null)
			{
				reflectionColorPicker.cvalue = jc["reflectionColor"]["v"].AsFloat;
			}
		}
		else
		{
			reflectionColorPicker.Reset();
		}
	}

	public void OnWillRenderObject()
	{
		Renderer component = GetComponent<Renderer>();
		if (!base.enabled || !component || !component.sharedMaterial || !component.enabled)
		{
			return;
		}
		Camera current = Camera.current;
		if (!current || s_InsideRendering)
		{
			return;
		}
		s_InsideRendering = true;
		CreateMirrorObjects(current, out var reflectionCamera);
		Vector3 position = base.transform.position;
		Vector3 up = base.transform.up;
		int pixelLightCount = QualitySettings.pixelLightCount;
		if (_disablePixelLights)
		{
			QualitySettings.pixelLightCount = 0;
		}
		UpdateCameraModes(current, reflectionCamera);
		float w = 0f - Vector3.Dot(up, position) - m_ClipPlaneOffset;
		Vector4 plane = new Vector4(up.x, up.y, up.z, w);
		Matrix4x4 reflectionMat = Matrix4x4.zero;
		CalculateReflectionMatrix(ref reflectionMat, plane);
		Vector3 position2 = current.transform.position;
		reflectionCamera.worldToCameraMatrix = current.worldToCameraMatrix * reflectionMat;
		Vector3 position3 = reflectionMat.MultiplyPoint(position2);
		Vector4 clipPlane = CameraSpacePlane(reflectionCamera, position, up, 1f);
		Matrix4x4 matrix4x2 = (reflectionCamera.projectionMatrix = current.CalculateObliqueMatrix(clipPlane));
		reflectionCamera.cullingMask = -17 & m_ReflectLayers.value;
		reflectionCamera.targetTexture = m_ReflectionTexture;
		GL.invertCulling = true;
		reflectionCamera.transform.position = position3;
		Vector3 eulerAngles = current.transform.eulerAngles;
		reflectionCamera.transform.eulerAngles = new Vector3(0f, eulerAngles.y, eulerAngles.z);
		reflectionCamera.Render();
		GL.invertCulling = false;
		Material[] array = ((!Application.isPlaying) ? component.sharedMaterials : component.materials);
		Material[] array2 = array;
		foreach (Material material in array2)
		{
			if (material.HasProperty("_ReflectionTex"))
			{
				material.SetTexture("_ReflectionTex", m_ReflectionTexture);
			}
		}
		if (_disablePixelLights)
		{
			QualitySettings.pixelLightCount = pixelLightCount;
		}
		s_InsideRendering = false;
	}

	private void UpdateCameraModes(Camera src, Camera dest)
	{
		if (dest == null)
		{
			return;
		}
		dest.backgroundColor = src.backgroundColor;
		if (src.clearFlags == CameraClearFlags.Skybox)
		{
			Skybox skybox = src.GetComponent(typeof(Skybox)) as Skybox;
			Skybox skybox2 = dest.GetComponent(typeof(Skybox)) as Skybox;
			if (!skybox || !skybox.material)
			{
				skybox2.enabled = false;
			}
			else
			{
				skybox2.enabled = true;
				skybox2.material = skybox.material;
			}
		}
		dest.farClipPlane = src.farClipPlane;
		dest.nearClipPlane = src.nearClipPlane;
		dest.orthographic = src.orthographic;
		dest.fieldOfView = src.fieldOfView;
		dest.aspect = src.aspect;
		dest.orthographicSize = src.orthographicSize;
	}

	private void CreateMirrorObjects(Camera currentCamera, out Camera reflectionCamera)
	{
		reflectionCamera = null;
		if (!m_ReflectionTexture || _oldReflectionTextureSize != _textureSize || _oldAntiAliasing != _antiAliasing)
		{
			if ((bool)m_ReflectionTexture)
			{
				UnityEngine.Object.DestroyImmediate(m_ReflectionTexture);
			}
			m_ReflectionTexture = new RenderTexture(_textureSize, _textureSize, 16);
			m_ReflectionTexture.name = "__MirrorReflection" + GetInstanceID();
			m_ReflectionTexture.antiAliasing = _antiAliasing;
			m_ReflectionTexture.isPowerOfTwo = true;
			m_ReflectionTexture.hideFlags = HideFlags.DontSave;
			_oldReflectionTextureSize = _textureSize;
			_oldAntiAliasing = _antiAliasing;
		}
		reflectionCamera = m_ReflectionCameras[currentCamera] as Camera;
		if (!reflectionCamera)
		{
			GameObject gameObject = new GameObject("Mirror Refl Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(Camera), typeof(Skybox));
			reflectionCamera = gameObject.GetComponent<Camera>();
			reflectionCamera.enabled = false;
			reflectionCamera.transform.position = base.transform.position;
			reflectionCamera.transform.rotation = base.transform.rotation;
			reflectionCamera.gameObject.AddComponent<FlareLayer>();
			gameObject.hideFlags = HideFlags.DontSave;
			m_ReflectionCameras[currentCamera] = reflectionCamera;
		}
	}

	private static float sgn(float a)
	{
		if (a > 0f)
		{
			return 1f;
		}
		if (a < 0f)
		{
			return -1f;
		}
		return 0f;
	}

	private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		Vector3 point = pos + normal * m_ClipPlaneOffset;
		Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
		Vector3 lhs = worldToCameraMatrix.MultiplyPoint(point);
		Vector3 rhs = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(rhs.x, rhs.y, rhs.z, 0f - Vector3.Dot(lhs, rhs));
	}

	private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
	{
		reflectionMat.m00 = 1f - 2f * plane[0] * plane[0];
		reflectionMat.m01 = -2f * plane[0] * plane[1];
		reflectionMat.m02 = -2f * plane[0] * plane[2];
		reflectionMat.m03 = -2f * plane[3] * plane[0];
		reflectionMat.m10 = -2f * plane[1] * plane[0];
		reflectionMat.m11 = 1f - 2f * plane[1] * plane[1];
		reflectionMat.m12 = -2f * plane[1] * plane[2];
		reflectionMat.m13 = -2f * plane[3] * plane[1];
		reflectionMat.m20 = -2f * plane[2] * plane[0];
		reflectionMat.m21 = -2f * plane[2] * plane[1];
		reflectionMat.m22 = 1f - 2f * plane[2] * plane[2];
		reflectionMat.m23 = -2f * plane[3] * plane[2];
		reflectionMat.m30 = 0f;
		reflectionMat.m31 = 0f;
		reflectionMat.m32 = 0f;
		reflectionMat.m33 = 1f;
	}

	public void SetReflectionColor(Color c)
	{
		Renderer component = GetComponent<Renderer>();
		if (!(component != null))
		{
			return;
		}
		Material[] array = ((!Application.isPlaying) ? component.sharedMaterials : component.materials);
		Material[] array2 = array;
		foreach (Material material in array2)
		{
			if (material.HasProperty("_ReflectionColor"))
			{
				material.SetColor("_ReflectionColor", c);
			}
		}
	}

	public void SetTextureSize(string size)
	{
		try
		{
			int num = int.Parse(size);
			if (num == 1024 || num == 2048 || num == 4096)
			{
				textureSize = num;
			}
			else
			{
				Debug.LogError("Attempted to set texture size to " + size + " which is not a valid value of 1024, 2048, 4096");
			}
		}
		catch (FormatException)
		{
			Debug.LogError("Attempted to set texture size to " + size + " which is not a valid integer");
		}
	}

	public void SetAntialiasing(string aa)
	{
		try
		{
			int num = int.Parse(aa);
			if (num == 1 || num == 2 || num == 4 || num == 8)
			{
				_antiAliasing = num;
			}
			else
			{
				Debug.LogError("Attempted to set antialiasing to " + aa + " which is not a valid value of 1, 2, 4, or 8");
			}
		}
		catch (FormatException)
		{
			Debug.LogError("Attempted to set antialiasing to " + aa + " which is not a valid integer");
		}
	}

	protected void InitUI()
	{
		if (disablePixelLightsToggle != null)
		{
			disablePixelLightsToggle.onValueChanged.AddListener(delegate
			{
				disablePixelLights = disablePixelLightsToggle.isOn;
			});
		}
		if (reflectionColorPicker != null)
		{
			HSVColorPicker hSVColorPicker = reflectionColorPicker;
			hSVColorPicker.onColorChangedHandlers = (HSVColorPicker.OnColorChanged)Delegate.Combine(hSVColorPicker.onColorChangedHandlers, new HSVColorPicker.OnColorChanged(SetReflectionColor));
			Renderer component = GetComponent<Renderer>();
			if (component != null)
			{
				Material[] materials = component.materials;
				if (materials != null)
				{
					Material material = materials[0];
					if (material.HasProperty("_ReflectionColor"))
					{
						Color color = material.GetColor("_ReflectionColor");
						HSVColor hSVColor = HSVColorPicker.RGBToHSV(color.r, color.g, color.b);
						reflectionColorPicker.defaultHue = hSVColor.H;
						reflectionColorPicker.defaultSaturation = hSVColor.S;
						reflectionColorPicker.defaultCvalue = hSVColor.V;
					}
				}
			}
		}
		if (reflectionOpacitySlider != null)
		{
			reflectionOpacitySlider.onValueChanged.AddListener(delegate
			{
				reflectionOpacity = reflectionOpacitySlider.value;
			});
		}
		if (reflectionBlendSlider != null)
		{
			reflectionBlendSlider.onValueChanged.AddListener(delegate
			{
				reflectionBlend = reflectionBlendSlider.value;
			});
		}
		if (textureSizePopup != null)
		{
			UIPopup uIPopup = textureSizePopup;
			uIPopup.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup.onValueChangeHandlers, new UIPopup.OnValueChange(SetTextureSize));
		}
		if (antiAliasingPopup != null)
		{
			UIPopup uIPopup2 = antiAliasingPopup;
			uIPopup2.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup2.onValueChangeHandlers, new UIPopup.OnValueChange(SetAntialiasing));
		}
	}

	private void OnDisable()
	{
		if ((bool)m_ReflectionTexture)
		{
			UnityEngine.Object.DestroyImmediate(m_ReflectionTexture);
			m_ReflectionTexture = null;
		}
		foreach (DictionaryEntry reflectionCamera in m_ReflectionCameras)
		{
			UnityEngine.Object.DestroyImmediate(((Camera)reflectionCamera.Value).gameObject);
		}
		m_ReflectionCameras.Clear();
	}

	private void Awake()
	{
		if (Application.isPlaying)
		{
			InitUI();
		}
	}
}
