using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class SetTransformScale : JSONStorable
{
	public Slider scaleSlider;

	public Slider scaleSliderAlt;

	[SerializeField]
	protected float _scale = 1f;

	public float scale
	{
		get
		{
			return _scale;
		}
		set
		{
			if (_scale != value)
			{
				_scale = value;
				Vector3 localScale = default(Vector3);
				localScale.x = value;
				localScale.y = value;
				localScale.z = value;
				base.transform.localScale = localScale;
				if (scaleSlider != null)
				{
					scaleSlider.value = _scale;
				}
				if (scaleSliderAlt != null)
				{
					scaleSliderAlt.value = _scale;
				}
			}
		}
	}

	public override JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true)
	{
		JSONClass jSON = base.GetJSON(includePhysical, includeAppearance);
		if (includePhysical && scaleSlider != null)
		{
			SliderControl component = scaleSlider.GetComponent<SliderControl>();
			if (component == null || component.defaultValue != _scale)
			{
				needsStore = true;
				jSON["scale"].AsFloat = _scale;
			}
		}
		return jSON;
	}

	public override void RestoreFromJSON(JSONClass jc, bool restorePhysical = true, bool restoreAppearance = true)
	{
		base.RestoreFromJSON(jc, restorePhysical, restoreAppearance);
		if (jc["scale"] != null)
		{
			scale = jc["scale"].AsFloat;
		}
		else if (scaleSlider != null)
		{
			SliderControl component = scaleSlider.GetComponent<SliderControl>();
			if (component != null)
			{
				scale = component.defaultValue;
			}
		}
	}

	public void SetScale(float s)
	{
		scale = s;
	}

	protected virtual void InitUI()
	{
		if (scaleSlider != null)
		{
			scaleSlider.value = _scale;
			SliderControl component = scaleSlider.GetComponent<SliderControl>();
			if (component != null)
			{
				component.defaultValue = _scale;
			}
			scaleSlider.onValueChanged.AddListener(SetScale);
		}
		if (scaleSliderAlt != null)
		{
			scaleSliderAlt.value = _scale;
			SliderControl component2 = scaleSliderAlt.GetComponent<SliderControl>();
			if (component2 != null)
			{
				component2.defaultValue = _scale;
			}
			scaleSliderAlt.onValueChanged.AddListener(SetScale);
		}
	}

	private void Awake()
	{
		InitUI();
	}
}
