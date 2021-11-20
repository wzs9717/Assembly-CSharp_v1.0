using System;
using System.Collections;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;

[ExecuteInEditMode]
public class MirrorReflection : JSONStorable
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

	public Slider surfaceTexturePowerSlider;

	[SerializeField]
	protected float _surfaceTexturePower = 1f;

	public Slider specularIntensitySlider;

	[SerializeField]
	protected float _specularIntensity = 1f;

	public HSVColorPicker reflectionColorPicker;

	public static bool globalEnabled = true;

	public Transform altObjectWhenMirrorDisabled;

	public bool useSameMaterialWhenMirrorDisabled;

	public float m_ClipPlaneOffset;

	public LayerMask m_ReflectLayers = -1;

	public bool m_UseObliqueClip = true;

	private Hashtable m_ReflectionCameras = new Hashtable();

	private RenderTexture m_ReflectionTextureLeft;

	private RenderTexture m_ReflectionTextureRight;

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
			if (_textureSize != value && (value == 512 || value == 1024 || value == 2048 || value == 4096))
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

	public float surfaceTexturePower
	{
		get
		{
			return _surfaceTexturePower;
		}
		set
		{
			if (_surfaceTexturePower == value)
			{
				return;
			}
			_surfaceTexturePower = value;
			Renderer component = GetComponent<Renderer>();
			if (!(component != null))
			{
				return;
			}
			Material[] array = ((!Application.isPlaying) ? component.sharedMaterials : component.materials);
			Material[] array2 = array;
			foreach (Material material in array2)
			{
				if (material.HasProperty("_MainTexPower"))
				{
					material.SetFloat("_MainTexPower", _surfaceTexturePower);
				}
			}
			if (surfaceTexturePowerSlider != null)
			{
				surfaceTexturePowerSlider.value = value;
			}
		}
	}

	public float specularIntensity
	{
		get
		{
			return _specularIntensity;
		}
		set
		{
			if (_specularIntensity == value)
			{
				return;
			}
			_specularIntensity = value;
			Renderer component = GetComponent<Renderer>();
			if (!(component != null))
			{
				return;
			}
			Material[] array = ((!Application.isPlaying) ? component.sharedMaterials : component.materials);
			Material[] array2 = array;
			foreach (Material material in array2)
			{
				if (material.HasProperty("_SpecularIntensity"))
				{
					material.SetFloat("_SpecularIntensity", _specularIntensity);
				}
			}
			if (specularIntensitySlider != null)
			{
				specularIntensitySlider.value = value;
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
			if (surfaceTexturePowerSlider != null)
			{
				SliderControl component3 = surfaceTexturePowerSlider.GetComponent<SliderControl>();
				if (component3 == null || component3.defaultValue != surfaceTexturePower)
				{
					needsStore = true;
					jSON["surfaceTexturePower"].AsFloat = surfaceTexturePower;
				}
			}
			if (specularIntensitySlider != null)
			{
				SliderControl component4 = specularIntensitySlider.GetComponent<SliderControl>();
				if (component4 == null || component4.defaultValue != specularIntensity)
				{
					needsStore = true;
					jSON["specularIntensity"].AsFloat = specularIntensity;
				}
			}
			if (reflectionColorPicker != null && (reflectionColorPicker.hue != reflectionColorPicker.defaultHue || reflectionColorPicker.saturation != reflectionColorPicker.defaultSaturation || reflectionColorPicker.cvalue != reflectionColorPicker.defaultCvalue))
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
		if (jc["surfaceTexturePower"] != null)
		{
			surfaceTexturePower = jc["surfaceTexturePower"].AsFloat;
		}
		else if ((bool)surfaceTexturePowerSlider)
		{
			SliderControl component3 = surfaceTexturePowerSlider.GetComponent<SliderControl>();
			if (component3 != null)
			{
				surfaceTexturePower = component3.defaultValue;
			}
		}
		if (jc["specularIntensity"] != null)
		{
			specularIntensity = jc["specularIntensity"].AsFloat;
		}
		else if ((bool)specularIntensitySlider)
		{
			SliderControl component4 = specularIntensitySlider.GetComponent<SliderControl>();
			if (component4 != null)
			{
				specularIntensity = component4.defaultValue;
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

	private void RenderMirror(Camera reflectionCamera)
	{
		Vector3 position = base.transform.position;
		Vector3 up = base.transform.up;
		float w = 0f - Vector3.Dot(up, position) - m_ClipPlaneOffset;
		Vector4 plane = new Vector4(up.x, up.y, up.z, w);
		Matrix4x4 reflectionMat = Matrix4x4.zero;
		CalculateReflectionMatrix(ref reflectionMat, plane);
		reflectionCamera.worldToCameraMatrix *= reflectionMat;
		if (m_UseObliqueClip)
		{
			Vector4 clipPlane = CameraSpacePlane(reflectionCamera, position, up, 1f);
			reflectionCamera.projectionMatrix = reflectionCamera.CalculateObliqueMatrix(clipPlane);
		}
		reflectionCamera.cullingMask = -17 & m_ReflectLayers.value;
		GL.invertCulling = true;
		reflectionCamera.transform.position = reflectionCamera.cameraToWorldMatrix.GetPosition();
		reflectionCamera.transform.rotation = reflectionCamera.cameraToWorldMatrix.GetRotation();
		reflectionCamera.Render();
		GL.invertCulling = false;
	}

	public void OnWillRenderObject()
	{
		Renderer component = GetComponent<Renderer>();
		if (!base.enabled || !component || !component.sharedMaterial || !component.enabled || !globalEnabled)
		{
			return;
		}
		Camera current = Camera.current;
		if (!current)
		{
			return;
		}
		Vector3 rhs = current.transform.position - base.transform.position;
		float num = Vector3.Dot(base.transform.up, rhs);
		if (num <= 0.001f || s_InsideRendering)
		{
			return;
		}
		s_InsideRendering = true;
		CreateMirrorObjects(current, out var reflectionCamera);
		int pixelLightCount = QualitySettings.pixelLightCount;
		if (_disablePixelLights)
		{
			QualitySettings.pixelLightCount = 0;
		}
		UpdateCameraModes(current, reflectionCamera);
		Vector3 position = current.transform.position;
		if (current.stereoEnabled)
		{
			if (current.stereoTargetEye == StereoTargetEyeMask.Both)
			{
				reflectionCamera.ResetWorldToCameraMatrix();
				if (CameraTarget.rightTarget != null && CameraTarget.rightTarget.targetCamera != null && current.transform.parent != null)
				{
					reflectionCamera.transform.position = current.transform.parent.TransformPoint(InputTracking.GetLocalPosition((VRNode)1));
					reflectionCamera.transform.rotation = current.transform.parent.rotation * InputTracking.GetLocalRotation((VRNode)1);
					reflectionCamera.worldToCameraMatrix = CameraTarget.rightTarget.worldToCameraMatrix;
					reflectionCamera.projectionMatrix = CameraTarget.rightTarget.projectionMatrix;
				}
				else
				{
					reflectionCamera.transform.position = current.transform.parent.TransformPoint(InputTracking.GetLocalPosition((VRNode)1));
					reflectionCamera.transform.rotation = current.transform.parent.rotation * InputTracking.GetLocalRotation((VRNode)1);
					reflectionCamera.worldToCameraMatrix = current.GetStereoViewMatrix(Camera.StereoscopicEye.Right);
					reflectionCamera.projectionMatrix = current.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
				}
				reflectionCamera.targetTexture = m_ReflectionTextureRight;
				RenderMirror(reflectionCamera);
				reflectionCamera.ResetWorldToCameraMatrix();
				if (CameraTarget.leftTarget != null && CameraTarget.leftTarget.targetCamera != null && current.transform.parent != null)
				{
					reflectionCamera.transform.position = current.transform.parent.TransformPoint(InputTracking.GetLocalPosition((VRNode)0));
					reflectionCamera.transform.rotation = current.transform.parent.rotation * InputTracking.GetLocalRotation((VRNode)0);
					reflectionCamera.worldToCameraMatrix = CameraTarget.leftTarget.worldToCameraMatrix;
					reflectionCamera.projectionMatrix = CameraTarget.leftTarget.projectionMatrix;
				}
				else
				{
					reflectionCamera.transform.position = current.transform.parent.TransformPoint(InputTracking.GetLocalPosition((VRNode)0));
					reflectionCamera.transform.rotation = current.transform.parent.rotation * InputTracking.GetLocalRotation((VRNode)0);
					reflectionCamera.worldToCameraMatrix = current.GetStereoViewMatrix(Camera.StereoscopicEye.Left);
					reflectionCamera.projectionMatrix = current.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
				}
				position = reflectionCamera.transform.position;
				reflectionCamera.targetTexture = m_ReflectionTextureLeft;
				RenderMirror(reflectionCamera);
			}
			else if (current.stereoTargetEye == StereoTargetEyeMask.Left)
			{
				reflectionCamera.ResetWorldToCameraMatrix();
				reflectionCamera.transform.position = current.transform.position;
				reflectionCamera.transform.rotation = current.transform.rotation;
				reflectionCamera.worldToCameraMatrix = current.worldToCameraMatrix;
				reflectionCamera.projectionMatrix = current.projectionMatrix;
				reflectionCamera.targetTexture = m_ReflectionTextureLeft;
				RenderMirror(reflectionCamera);
			}
			else if (current.stereoTargetEye == StereoTargetEyeMask.Right)
			{
				reflectionCamera.ResetWorldToCameraMatrix();
				reflectionCamera.transform.position = current.transform.position;
				reflectionCamera.transform.rotation = current.transform.rotation;
				reflectionCamera.worldToCameraMatrix = current.worldToCameraMatrix;
				reflectionCamera.projectionMatrix = current.projectionMatrix;
				reflectionCamera.targetTexture = m_ReflectionTextureLeft;
				RenderMirror(reflectionCamera);
			}
		}
		else
		{
			reflectionCamera.ResetWorldToCameraMatrix();
			reflectionCamera.transform.position = current.transform.position;
			reflectionCamera.transform.rotation = current.transform.rotation;
			reflectionCamera.worldToCameraMatrix = current.worldToCameraMatrix;
			reflectionCamera.projectionMatrix = current.projectionMatrix;
			reflectionCamera.targetTexture = m_ReflectionTextureLeft;
			RenderMirror(reflectionCamera);
		}
		Material[] array = ((!Application.isPlaying) ? component.sharedMaterials : component.materials);
		Vector4 value = default(Vector4);
		value.x = position.x;
		value.y = position.y;
		value.z = position.z;
		value.w = 0f;
		Material[] array2 = array;
		foreach (Material material in array2)
		{
			if (material.HasProperty("_ReflectionTex"))
			{
				material.SetTexture("_ReflectionTex", m_ReflectionTextureLeft);
			}
			if (material.HasProperty("_LeftReflectionTex"))
			{
				material.SetTexture("_LeftReflectionTex", m_ReflectionTextureLeft);
			}
			if (material.HasProperty("_RightReflectionTex"))
			{
				material.SetTexture("_RightReflectionTex", m_ReflectionTextureRight);
			}
			if (material.HasProperty("_LeftCameraPosition"))
			{
				material.SetVector("_LeftCameraPosition", value);
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
		if (!m_ReflectionTextureRight || !m_ReflectionTextureLeft || _oldReflectionTextureSize != _textureSize || _oldAntiAliasing != _antiAliasing)
		{
			if ((bool)m_ReflectionTextureLeft)
			{
				UnityEngine.Object.DestroyImmediate(m_ReflectionTextureLeft);
			}
			m_ReflectionTextureLeft = new RenderTexture(_textureSize, _textureSize, 24);
			m_ReflectionTextureLeft.name = "__MirrorReflectionLeft" + GetInstanceID();
			m_ReflectionTextureLeft.antiAliasing = _antiAliasing;
			m_ReflectionTextureLeft.isPowerOfTwo = true;
			m_ReflectionTextureLeft.hideFlags = HideFlags.DontSave;
			if ((bool)m_ReflectionTextureRight)
			{
				UnityEngine.Object.DestroyImmediate(m_ReflectionTextureRight);
			}
			m_ReflectionTextureRight = new RenderTexture(_textureSize, _textureSize, 24);
			m_ReflectionTextureRight.name = "__MirrorReflectionRight" + GetInstanceID();
			m_ReflectionTextureRight.antiAliasing = _antiAliasing;
			m_ReflectionTextureRight.isPowerOfTwo = true;
			m_ReflectionTextureRight.hideFlags = HideFlags.DontSave;
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
			if (num == 512 || num == 1024 || num == 2048 || num == 4096)
			{
				textureSize = num;
			}
			else
			{
				Debug.LogError("Attempted to set texture size to " + size + " which is not a valid value of 512, 1024, 2048, 4096");
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
		Material material = null;
		Renderer component = GetComponent<Renderer>();
		if (component != null)
		{
			Material[] materials = component.materials;
			if (materials != null)
			{
				material = materials[0];
			}
		}
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
			if (material != null && material.HasProperty("_ReflectionColor"))
			{
				Color color = material.GetColor("_ReflectionColor");
				HSVColor hSVColor = HSVColorPicker.RGBToHSV(color.r, color.g, color.b);
				reflectionColorPicker.defaultHue = hSVColor.H;
				reflectionColorPicker.defaultSaturation = hSVColor.S;
				reflectionColorPicker.defaultCvalue = hSVColor.V;
				reflectionColorPicker.hue = hSVColor.H;
				reflectionColorPicker.saturation = hSVColor.S;
				reflectionColorPicker.cvalue = hSVColor.V;
			}
		}
		if (reflectionOpacitySlider != null)
		{
			if (material != null && material.HasProperty("_ReflectionOpacity"))
			{
				float num = (_reflectionOpacity = material.GetFloat("_ReflectionOpacity"));
				reflectionOpacitySlider.value = num;
				SliderControl component2 = reflectionOpacitySlider.GetComponent<SliderControl>();
				if (component2 != null)
				{
					component2.defaultValue = num;
				}
			}
			reflectionOpacitySlider.onValueChanged.AddListener(delegate
			{
				reflectionOpacity = reflectionOpacitySlider.value;
			});
		}
		if (reflectionBlendSlider != null)
		{
			if (material != null && material.HasProperty("_ReflectionBlendTexPower"))
			{
				float num2 = (_reflectionBlend = material.GetFloat("_ReflectionBlendTexPower"));
				reflectionBlendSlider.value = num2;
				SliderControl component3 = reflectionBlendSlider.GetComponent<SliderControl>();
				if (component3 != null)
				{
					component3.defaultValue = num2;
				}
			}
			reflectionBlendSlider.onValueChanged.AddListener(delegate
			{
				reflectionBlend = reflectionBlendSlider.value;
			});
		}
		if (surfaceTexturePowerSlider != null)
		{
			if (material != null && material.HasProperty("_MainTexPower"))
			{
				float num3 = (_surfaceTexturePower = material.GetFloat("_MainTexPower"));
				surfaceTexturePowerSlider.value = num3;
				SliderControl component4 = surfaceTexturePowerSlider.GetComponent<SliderControl>();
				if (component4 != null)
				{
					component4.defaultValue = num3;
				}
			}
			surfaceTexturePowerSlider.onValueChanged.AddListener(delegate
			{
				surfaceTexturePower = surfaceTexturePowerSlider.value;
			});
		}
		if (specularIntensitySlider != null)
		{
			if (material != null && material.HasProperty("_SpecularIntensity"))
			{
				float num4 = (_specularIntensity = material.GetFloat("_SpecularIntensity"));
				specularIntensitySlider.value = num4;
				SliderControl component5 = specularIntensitySlider.GetComponent<SliderControl>();
				if (component5 != null)
				{
					component5.defaultValue = num4;
				}
			}
			specularIntensitySlider.onValueChanged.AddListener(delegate
			{
				specularIntensity = specularIntensitySlider.value;
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
		if ((bool)m_ReflectionTextureRight)
		{
			UnityEngine.Object.DestroyImmediate(m_ReflectionTextureRight);
			m_ReflectionTextureRight = null;
		}
		if ((bool)m_ReflectionTextureLeft)
		{
			UnityEngine.Object.DestroyImmediate(m_ReflectionTextureLeft);
			m_ReflectionTextureLeft = null;
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

	private void Update()
	{
		Renderer component = GetComponent<Renderer>();
		if (component != null)
		{
			component.enabled = globalEnabled || useSameMaterialWhenMirrorDisabled;
		}
		if (useSameMaterialWhenMirrorDisabled)
		{
			if (globalEnabled)
			{
				return;
			}
			Material[] array = ((!Application.isPlaying) ? component.sharedMaterials : component.materials);
			Material[] array2 = array;
			foreach (Material material in array2)
			{
				if (material.HasProperty("_ReflectionTex"))
				{
					material.SetTexture("_ReflectionTex", null);
				}
				if (material.HasProperty("_LeftReflectionTex"))
				{
					material.SetTexture("_LeftReflectionTex", null);
				}
				if (material.HasProperty("_RightReflectionTex"))
				{
					material.SetTexture("_RightReflectionTex", null);
				}
			}
		}
		else if (altObjectWhenMirrorDisabled != null)
		{
			altObjectWhenMirrorDisabled.gameObject.SetActive(!globalEnabled);
		}
	}
}
