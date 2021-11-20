using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class DAZMaleAnatomyControl : JSONStorable
{
	public AdjustJointSprings stiffnessSprings;

	public Slider stiffnessSlider;

	[SerializeField]
	private float _stiffness;

	public AdjustJointTarget upDownAngleTarget;

	public Slider upDownAngleSlider;

	[SerializeField]
	private float _upDownAngle;

	public float stiffness
	{
		get
		{
			return _stiffness;
		}
		set
		{
			if (_stiffness != value)
			{
				_stiffness = value;
				if (stiffnessSlider != null)
				{
					stiffnessSlider.value = value;
				}
				if (stiffnessSprings != null)
				{
					stiffnessSprings.percent = value;
				}
			}
		}
	}

	public float upDownAngle
	{
		get
		{
			return _upDownAngle;
		}
		set
		{
			if (_upDownAngle != value)
			{
				_upDownAngle = value;
				if (upDownAngleSlider != null)
				{
					upDownAngleSlider.value = value;
				}
				if (upDownAngleTarget != null)
				{
					upDownAngleTarget.currentTargetRotationX = value;
				}
			}
		}
	}

	public override JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true)
	{
		JSONClass jSON = base.GetJSON(includePhysical, includeAppearance);
		if (includePhysical)
		{
			if (stiffnessSlider != null)
			{
				SliderControl component = stiffnessSlider.GetComponent<SliderControl>();
				if (component == null || component.defaultValue != stiffness)
				{
					needsStore = true;
					jSON["stiffness"].AsFloat = stiffness;
				}
			}
			if (upDownAngleSlider != null)
			{
				SliderControl component2 = upDownAngleSlider.GetComponent<SliderControl>();
				if (component2 == null || component2.defaultValue != upDownAngle)
				{
					needsStore = true;
					jSON["upDownAngle"].AsFloat = upDownAngle;
				}
			}
		}
		return jSON;
	}

	public override void RestoreFromJSON(JSONClass jc, bool restorePhysical = true, bool restoreAppearance = true)
	{
		base.RestoreFromJSON(jc, restorePhysical, restoreAppearance);
		if (!restorePhysical)
		{
			return;
		}
		if (jc["stiffness"] != null)
		{
			stiffness = jc["stiffness"].AsFloat;
		}
		else if (stiffnessSlider != null)
		{
			SliderControl component = stiffnessSlider.GetComponent<SliderControl>();
			if (component != null)
			{
				stiffness = component.defaultValue;
			}
		}
		if (jc["upDownAngle"] != null)
		{
			upDownAngle = jc["upDownAngle"].AsFloat;
		}
		else if (upDownAngleSlider != null)
		{
			SliderControl component2 = upDownAngleSlider.GetComponent<SliderControl>();
			if (component2 != null)
			{
				upDownAngle = component2.defaultValue;
			}
		}
	}

	private void InitUI()
	{
		if (stiffnessSlider != null)
		{
			stiffnessSlider.value = _stiffness;
			stiffnessSlider.onValueChanged.AddListener(delegate
			{
				stiffness = stiffnessSlider.value;
			});
			SliderControl component = stiffnessSlider.GetComponent<SliderControl>();
			if (component != null)
			{
				component.defaultValue = _stiffness;
			}
		}
		if (upDownAngleSlider != null)
		{
			upDownAngleSlider.value = _upDownAngle;
			upDownAngleSlider.onValueChanged.AddListener(delegate
			{
				upDownAngle = upDownAngleSlider.value;
			});
			SliderControl component2 = upDownAngleSlider.GetComponent<SliderControl>();
			if (component2 != null)
			{
				component2.defaultValue = _upDownAngle;
			}
		}
	}

	private void Start()
	{
		InitUI();
	}
}
