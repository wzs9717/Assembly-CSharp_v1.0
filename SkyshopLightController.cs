using mset;
using SimpleJSON;
using UnityEngine.UI;

public class SkyshopLightController : JSONStorable
{
	public float minMasterIntensity;

	public float maxMasterIntensity = 10f;

	public float minDiffuseIntensity;

	public float maxDiffuseIntensity = 10f;

	public float minSpecularIntensity;

	public float maxSpecularIntensity = 10f;

	public float minCamExposure;

	public float maxCamExposure = 10f;

	public Slider masterIntensitySlider;

	private float _masterIntensity;

	public Slider diffuseIntensitySlider;

	private float _diffuseIntensity;

	public Slider specularIntensitySlider;

	private float _specularIntensity;

	public Slider camExposureSlider;

	private float _camExposure;

	public float masterIntensity
	{
		get
		{
			return _masterIntensity;
		}
		set
		{
			if (_masterIntensity != value)
			{
				_masterIntensity = value;
				if (_masterIntensity > maxMasterIntensity)
				{
					_masterIntensity = maxMasterIntensity;
				}
				if (_masterIntensity < minMasterIntensity)
				{
					_masterIntensity = minMasterIntensity;
				}
				if (masterIntensitySlider != null)
				{
					masterIntensitySlider.value = _masterIntensity;
				}
				if ((bool)SkyManager.Get().GlobalSky)
				{
					SkyManager.Get().GlobalSky.MasterIntensity = _masterIntensity;
					SkyManager.Get().GlobalSky.Apply();
				}
			}
		}
	}

	public float diffuseIntensity
	{
		get
		{
			return _diffuseIntensity;
		}
		set
		{
			if (_diffuseIntensity != value)
			{
				_diffuseIntensity = value;
				if (_diffuseIntensity > maxDiffuseIntensity)
				{
					_diffuseIntensity = maxDiffuseIntensity;
				}
				if (_diffuseIntensity < minDiffuseIntensity)
				{
					_diffuseIntensity = minDiffuseIntensity;
				}
				if (diffuseIntensitySlider != null)
				{
					diffuseIntensitySlider.value = _diffuseIntensity;
				}
				if ((bool)SkyManager.Get().GlobalSky)
				{
					SkyManager.Get().GlobalSky.DiffIntensity = _diffuseIntensity;
					SkyManager.Get().GlobalSky.Apply();
				}
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
			if (_specularIntensity != value)
			{
				_specularIntensity = value;
				if (_specularIntensity > maxSpecularIntensity)
				{
					_specularIntensity = maxSpecularIntensity;
				}
				if (_specularIntensity < minSpecularIntensity)
				{
					_specularIntensity = minSpecularIntensity;
				}
				if (specularIntensitySlider != null)
				{
					specularIntensitySlider.value = _specularIntensity;
				}
				if ((bool)SkyManager.Get().GlobalSky)
				{
					SkyManager.Get().GlobalSky.SpecIntensity = _specularIntensity;
					SkyManager.Get().GlobalSky.Apply();
				}
			}
		}
	}

	public float camExposure
	{
		get
		{
			return _camExposure;
		}
		set
		{
			if (_camExposure != value)
			{
				_camExposure = value;
				if (_camExposure > maxCamExposure)
				{
					_camExposure = maxCamExposure;
				}
				if (_camExposure < minCamExposure)
				{
					_camExposure = minCamExposure;
				}
				if (camExposureSlider != null)
				{
					camExposureSlider.value = _camExposure;
				}
				if ((bool)SkyManager.Get().GlobalSky)
				{
					SkyManager.Get().GlobalSky.CamExposure = _camExposure;
					SkyManager.Get().GlobalSky.Apply();
				}
			}
		}
	}

	public override JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true)
	{
		JSONClass jSON = base.GetJSON(includePhysical, includeAppearance);
		if (includeAppearance)
		{
			if (masterIntensitySlider != null)
			{
				SliderControl component = masterIntensitySlider.GetComponent<SliderControl>();
				if (component == null || component.defaultValue != masterIntensity)
				{
					needsStore = true;
					jSON["masterIntensity"].AsFloat = masterIntensity;
				}
			}
			if (diffuseIntensitySlider != null)
			{
				SliderControl component2 = diffuseIntensitySlider.GetComponent<SliderControl>();
				if (component2 == null || component2.defaultValue != diffuseIntensity)
				{
					needsStore = true;
					jSON["diffuseIntensity"].AsFloat = diffuseIntensity;
				}
			}
			if (specularIntensitySlider != null)
			{
				SliderControl component3 = specularIntensitySlider.GetComponent<SliderControl>();
				if (component3 == null || component3.defaultValue != specularIntensity)
				{
					needsStore = true;
					jSON["specularIntensity"].AsFloat = specularIntensity;
				}
			}
			if (camExposureSlider != null)
			{
				SliderControl component4 = camExposureSlider.GetComponent<SliderControl>();
				if (component4 == null || component4.defaultValue != camExposure)
				{
					needsStore = true;
					jSON["camExposure"].AsFloat = camExposure;
				}
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
		if (jc["masterIntensity"] != null)
		{
			masterIntensity = jc["masterIntensity"].AsFloat;
		}
		else if (masterIntensitySlider != null)
		{
			SliderControl component = masterIntensitySlider.GetComponent<SliderControl>();
			if (component != null)
			{
				masterIntensity = component.defaultValue;
			}
		}
		if (jc["diffuseIntensity"] != null)
		{
			diffuseIntensity = jc["diffuseIntensity"].AsFloat;
		}
		else if (diffuseIntensitySlider != null)
		{
			SliderControl component2 = diffuseIntensitySlider.GetComponent<SliderControl>();
			if (component2 != null)
			{
				diffuseIntensity = component2.defaultValue;
			}
		}
		if (jc["specularIntensity"] != null)
		{
			specularIntensity = jc["specularIntensity"].AsFloat;
		}
		else if (specularIntensitySlider != null)
		{
			SliderControl component3 = specularIntensitySlider.GetComponent<SliderControl>();
			if (component3 != null)
			{
				specularIntensity = component3.defaultValue;
			}
		}
		if (jc["camExposure"] != null)
		{
			camExposure = jc["camExposure"].AsFloat;
		}
		else if (camExposureSlider != null)
		{
			SliderControl component4 = camExposureSlider.GetComponent<SliderControl>();
			if (component4 != null)
			{
				camExposure = component4.defaultValue;
			}
		}
	}

	private void SetInitialValues()
	{
		if ((bool)SkyManager.Get().GlobalSky)
		{
			_masterIntensity = SkyManager.Get().GlobalSky.MasterIntensity;
		}
		if ((bool)SkyManager.Get().GlobalSky)
		{
			_diffuseIntensity = SkyManager.Get().GlobalSky.DiffIntensity;
		}
		if ((bool)SkyManager.Get().GlobalSky)
		{
			_specularIntensity = SkyManager.Get().GlobalSky.SpecIntensity;
		}
		if ((bool)SkyManager.Get().GlobalSky)
		{
			_camExposure = SkyManager.Get().GlobalSky.CamExposure;
		}
	}

	private void InitUI()
	{
		if (masterIntensitySlider != null)
		{
			masterIntensitySlider.value = _masterIntensity;
			masterIntensitySlider.onValueChanged.AddListener(delegate
			{
				masterIntensity = masterIntensitySlider.value;
			});
			SliderControl component = masterIntensitySlider.GetComponent<SliderControl>();
			if (component != null)
			{
				component.defaultValue = _masterIntensity;
			}
		}
		if (diffuseIntensitySlider != null)
		{
			diffuseIntensitySlider.value = _diffuseIntensity;
			diffuseIntensitySlider.onValueChanged.AddListener(delegate
			{
				diffuseIntensity = diffuseIntensitySlider.value;
			});
			SliderControl component2 = diffuseIntensitySlider.GetComponent<SliderControl>();
			if (component2 != null)
			{
				component2.defaultValue = _diffuseIntensity;
			}
		}
		if (specularIntensitySlider != null)
		{
			specularIntensitySlider.value = _specularIntensity;
			specularIntensitySlider.onValueChanged.AddListener(delegate
			{
				specularIntensity = specularIntensitySlider.value;
			});
			SliderControl component3 = specularIntensitySlider.GetComponent<SliderControl>();
			if (component3 != null)
			{
				component3.defaultValue = _specularIntensity;
			}
		}
		if (camExposureSlider != null)
		{
			camExposureSlider.value = _camExposure;
			camExposureSlider.onValueChanged.AddListener(delegate
			{
				camExposure = camExposureSlider.value;
			});
			SliderControl component4 = camExposureSlider.GetComponent<SliderControl>();
			if (component4 != null)
			{
				component4.defaultValue = _camExposure;
			}
		}
	}

	private void Start()
	{
		SetInitialValues();
		InitUI();
	}
}
