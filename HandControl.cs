using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class HandControl : JSONStorable
{
	public AdjustJointSprings handJointSprings;

	public Slider handGraspStrengthSlider;

	[SerializeField]
	public float _handGraspStrength;

	public AdjustJointTargets fingerJointTargets1;

	public AdjustJointTargets fingerJointTargets2;

	public Slider fingerGraspSlider;

	public float fingerGraspMin;

	public float fingerGraspMax = 1f;

	[SerializeField]
	public float _fingerGrasp;

	public AdjustJointTargets thumbJointTargets;

	public Slider thumbGraspSlider;

	public float thumbGraspMin;

	public float thumbGraspMax = 1f;

	[SerializeField]
	public float _thumbGrasp;

	public ConfigurableJoint thumbRootJoint;

	public Slider thumbRootRotationDriveXTargetSlider;

	private float _thumbRootRotationDriveXTargetMin;

	private float _thumbRootRotationDriveXTargetMax;

	[SerializeField]
	private float _thumbRootRotationDriveXTarget;

	public Slider thumbRootRotationDriveYTargetSlider;

	private float _thumbRootRotationDriveYTargetMin;

	private float _thumbRootRotationDriveYTargetMax;

	[SerializeField]
	private float _thumbRootRotationDriveYTarget;

	public Slider thumbRootRotationDriveZTargetSlider;

	private float _thumbRootRotationDriveZTargetMin;

	private float _thumbRootRotationDriveZTargetMax;

	[SerializeField]
	private float _thumbRootRotationDriveZTarget;

	public float handGraspStrength
	{
		get
		{
			return _handGraspStrength;
		}
		set
		{
			if (_handGraspStrength != value)
			{
				_handGraspStrength = value;
				if (handGraspStrengthSlider != null)
				{
					handGraspStrengthSlider.value = value;
				}
				if (handJointSprings != null)
				{
					handJointSprings.percent = _handGraspStrength;
				}
			}
		}
	}

	public float fingerGrasp
	{
		get
		{
			return _fingerGrasp;
		}
		set
		{
			if (_fingerGrasp != value)
			{
				_fingerGrasp = Mathf.Clamp(value, fingerGraspMin, fingerGraspMax);
				if (fingerGraspSlider != null)
				{
					fingerGraspSlider.value = _fingerGrasp;
				}
				if (fingerJointTargets1 != null)
				{
					fingerJointTargets1.xPercent = _fingerGrasp;
				}
				if (fingerJointTargets2 != null)
				{
					fingerJointTargets2.xPercent = _fingerGrasp;
				}
			}
		}
	}

	public float thumbGrasp
	{
		get
		{
			return _thumbGrasp;
		}
		set
		{
			if (_thumbGrasp != value)
			{
				_thumbGrasp = Mathf.Clamp(value, thumbGraspMin, thumbGraspMax);
				if (thumbGraspSlider != null)
				{
					thumbGraspSlider.value = _thumbGrasp;
				}
				if (thumbJointTargets != null)
				{
					thumbJointTargets.xPercent = _thumbGrasp;
				}
			}
		}
	}

	public float thumbRootRotationDriveXTarget
	{
		get
		{
			return _thumbRootRotationDriveXTarget;
		}
		set
		{
			if (_thumbRootRotationDriveXTarget != value)
			{
				_thumbRootRotationDriveXTarget = value;
				if (thumbRootRotationDriveXTargetSlider != null)
				{
					thumbRootRotationDriveXTargetSlider.value = value;
				}
				SetThumbRootDrive();
			}
		}
	}

	public float thumbRootRotationDriveYTarget
	{
		get
		{
			return _thumbRootRotationDriveYTarget;
		}
		set
		{
			if (_thumbRootRotationDriveYTarget != value)
			{
				_thumbRootRotationDriveYTarget = value;
				if (thumbRootRotationDriveYTargetSlider != null)
				{
					thumbRootRotationDriveYTargetSlider.value = value;
				}
				SetThumbRootDrive();
			}
		}
	}

	public float thumbRootRotationDriveZTarget
	{
		get
		{
			return _thumbRootRotationDriveZTarget;
		}
		set
		{
			if (_thumbRootRotationDriveZTarget != value)
			{
				_thumbRootRotationDriveZTarget = value;
				if (thumbRootRotationDriveZTargetSlider != null)
				{
					thumbRootRotationDriveZTargetSlider.value = value;
				}
				SetThumbRootDrive();
			}
		}
	}

	public override JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true)
	{
		JSONClass jSON = base.GetJSON(includePhysical, includeAppearance);
		if (includePhysical)
		{
			if (handGraspStrengthSlider != null)
			{
				SliderControl component = handGraspStrengthSlider.GetComponent<SliderControl>();
				if (component == null || component.defaultValue != handGraspStrength)
				{
					needsStore = true;
					jSON["handGraspStrength"].AsFloat = handGraspStrength;
				}
			}
			if (fingerGraspSlider != null)
			{
				SliderControl component2 = fingerGraspSlider.GetComponent<SliderControl>();
				if (component2 == null || component2.defaultValue != fingerGrasp)
				{
					needsStore = true;
					jSON["fingerGrasp"].AsFloat = fingerGrasp;
				}
			}
			if (thumbGraspSlider != null)
			{
				SliderControl component3 = thumbGraspSlider.GetComponent<SliderControl>();
				if (component3 == null || component3.defaultValue != thumbGrasp)
				{
					needsStore = true;
					jSON["thumbGrasp"].AsFloat = thumbGrasp;
				}
			}
			if (thumbRootRotationDriveXTargetSlider != null)
			{
				SliderControl component4 = thumbRootRotationDriveXTargetSlider.GetComponent<SliderControl>();
				if (component4 == null || component4.defaultValue != thumbRootRotationDriveXTarget)
				{
					needsStore = true;
					jSON["thumbDriveXTarget"].AsFloat = thumbRootRotationDriveXTarget;
				}
			}
			if (thumbRootRotationDriveYTargetSlider != null)
			{
				SliderControl component5 = thumbRootRotationDriveYTargetSlider.GetComponent<SliderControl>();
				if (component5 == null || component5.defaultValue != thumbRootRotationDriveYTarget)
				{
					needsStore = true;
					jSON["thumbDriveYTarget"].AsFloat = thumbRootRotationDriveYTarget;
				}
			}
			if (thumbRootRotationDriveZTargetSlider != null)
			{
				SliderControl component6 = thumbRootRotationDriveZTargetSlider.GetComponent<SliderControl>();
				if (component6 == null || component6.defaultValue != thumbRootRotationDriveZTarget)
				{
					needsStore = true;
					jSON["thumbDriveZTarget"].AsFloat = thumbRootRotationDriveZTarget;
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
		if (jc["handGraspStrength"] != null)
		{
			handGraspStrength = jc["handGraspStrength"].AsFloat;
		}
		else if (handGraspStrengthSlider != null)
		{
			SliderControl component = handGraspStrengthSlider.GetComponent<SliderControl>();
			if (component != null)
			{
				handGraspStrength = component.defaultValue;
			}
		}
		if (jc["fingerGrasp"] != null)
		{
			fingerGrasp = jc["fingerGrasp"].AsFloat;
		}
		else if (fingerGraspSlider != null)
		{
			SliderControl component2 = fingerGraspSlider.GetComponent<SliderControl>();
			if (component2 != null)
			{
				fingerGrasp = component2.defaultValue;
			}
		}
		if (jc["thumbGrasp"] != null)
		{
			thumbGrasp = jc["thumbGrasp"].AsFloat;
		}
		else if (thumbGraspSlider != null)
		{
			SliderControl component3 = thumbGraspSlider.GetComponent<SliderControl>();
			if (component3 != null)
			{
				thumbGrasp = component3.defaultValue;
			}
		}
		if (jc["thumbDriveXTarget"] != null)
		{
			thumbRootRotationDriveXTarget = jc["thumbDriveXTarget"].AsFloat;
		}
		else if (thumbRootRotationDriveXTargetSlider != null)
		{
			SliderControl component4 = thumbRootRotationDriveXTargetSlider.GetComponent<SliderControl>();
			if (component4 != null)
			{
				thumbRootRotationDriveXTarget = component4.defaultValue;
			}
		}
		if (jc["thumbDriveYTarget"] != null)
		{
			thumbRootRotationDriveYTarget = jc["thumbDriveYTarget"].AsFloat;
		}
		else if (thumbRootRotationDriveYTargetSlider != null)
		{
			SliderControl component5 = thumbRootRotationDriveYTargetSlider.GetComponent<SliderControl>();
			if (component5 != null)
			{
				thumbRootRotationDriveYTarget = component5.defaultValue;
			}
		}
		if (jc["thumbDriveZTarget"] != null)
		{
			thumbRootRotationDriveZTarget = jc["thumbDriveZTarget"].AsFloat;
		}
		else if (thumbRootRotationDriveZTargetSlider != null)
		{
			SliderControl component6 = thumbRootRotationDriveZTargetSlider.GetComponent<SliderControl>();
			if (component6 != null)
			{
				thumbRootRotationDriveZTarget = component6.defaultValue;
			}
		}
	}

	private void SetThumbRootDrive()
	{
		if (thumbRootJoint != null)
		{
			Quaternion targetRotation = Quaternion.Euler(_thumbRootRotationDriveXTarget, _thumbRootRotationDriveYTarget, _thumbRootRotationDriveZTarget);
			thumbRootJoint.targetRotation = targetRotation;
		}
	}

	private void InitUI()
	{
		if (handGraspStrengthSlider != null)
		{
			handGraspStrengthSlider.value = _handGraspStrength;
			handGraspStrengthSlider.onValueChanged.AddListener(delegate
			{
				handGraspStrength = handGraspStrengthSlider.value;
			});
			SliderControl component = handGraspStrengthSlider.GetComponent<SliderControl>();
			if (component != null)
			{
				component.defaultValue = _handGraspStrength;
			}
		}
		if (thumbGraspSlider != null)
		{
			thumbGraspSlider.value = _thumbGrasp;
			thumbGraspSlider.onValueChanged.AddListener(delegate
			{
				thumbGrasp = thumbGraspSlider.value;
			});
			SliderControl component2 = thumbGraspSlider.GetComponent<SliderControl>();
			if (component2 != null)
			{
				component2.defaultValue = _thumbGrasp;
			}
		}
		if (fingerGraspSlider != null)
		{
			fingerGraspSlider.value = _thumbGrasp;
			fingerGraspSlider.onValueChanged.AddListener(delegate
			{
				fingerGrasp = fingerGraspSlider.value;
			});
			SliderControl component3 = fingerGraspSlider.GetComponent<SliderControl>();
			if (component3 != null)
			{
				component3.defaultValue = _fingerGrasp;
			}
		}
		if (thumbRootRotationDriveXTargetSlider != null)
		{
			thumbRootRotationDriveXTargetSlider.minValue = _thumbRootRotationDriveXTargetMin;
			thumbRootRotationDriveXTargetSlider.maxValue = _thumbRootRotationDriveXTargetMax;
			thumbRootRotationDriveXTargetSlider.value = _thumbRootRotationDriveXTarget;
			thumbRootRotationDriveXTargetSlider.onValueChanged.AddListener(delegate
			{
				thumbRootRotationDriveXTarget = thumbRootRotationDriveXTargetSlider.value;
			});
			SliderControl component4 = thumbRootRotationDriveXTargetSlider.GetComponent<SliderControl>();
			if (component4 != null)
			{
				component4.defaultValue = _thumbRootRotationDriveXTarget;
			}
		}
		if (thumbRootRotationDriveYTargetSlider != null)
		{
			thumbRootRotationDriveYTargetSlider.minValue = _thumbRootRotationDriveYTargetMin;
			thumbRootRotationDriveYTargetSlider.maxValue = _thumbRootRotationDriveYTargetMax;
			thumbRootRotationDriveYTargetSlider.value = _thumbRootRotationDriveYTarget;
			thumbRootRotationDriveYTargetSlider.onValueChanged.AddListener(delegate
			{
				thumbRootRotationDriveYTarget = thumbRootRotationDriveYTargetSlider.value;
			});
			SliderControl component5 = thumbRootRotationDriveYTargetSlider.GetComponent<SliderControl>();
			if (component5 != null)
			{
				component5.defaultValue = _thumbRootRotationDriveYTarget;
			}
		}
		if (thumbRootRotationDriveZTargetSlider != null)
		{
			thumbRootRotationDriveZTargetSlider.minValue = _thumbRootRotationDriveZTargetMin;
			thumbRootRotationDriveZTargetSlider.maxValue = _thumbRootRotationDriveZTargetMax;
			thumbRootRotationDriveZTargetSlider.value = _thumbRootRotationDriveZTarget;
			thumbRootRotationDriveZTargetSlider.onValueChanged.AddListener(delegate
			{
				thumbRootRotationDriveZTarget = thumbRootRotationDriveZTargetSlider.value;
			});
			SliderControl component6 = thumbRootRotationDriveZTargetSlider.GetComponent<SliderControl>();
			if (component6 != null)
			{
				component6.defaultValue = _thumbRootRotationDriveZTarget;
			}
		}
	}

	private void Awake()
	{
		Vector3 eulerAngles = thumbRootJoint.targetRotation.eulerAngles;
		if (eulerAngles.x > 180f)
		{
			eulerAngles.x -= 360f;
		}
		else if (eulerAngles.x < -180f)
		{
			eulerAngles.x += 360f;
		}
		if (eulerAngles.y > 180f)
		{
			eulerAngles.y -= 360f;
		}
		else if (eulerAngles.y < -180f)
		{
			eulerAngles.y += 360f;
		}
		if (eulerAngles.z > 180f)
		{
			eulerAngles.z -= 360f;
		}
		else if (eulerAngles.z < -180f)
		{
			eulerAngles.z += 360f;
		}
		_thumbRootRotationDriveXTarget = eulerAngles.x;
		_thumbRootRotationDriveYTarget = eulerAngles.y;
		_thumbRootRotationDriveZTarget = eulerAngles.z;
		if (thumbRootJoint.lowAngularXLimit.limit < thumbRootJoint.highAngularXLimit.limit)
		{
			_thumbRootRotationDriveXTargetMin = thumbRootJoint.lowAngularXLimit.limit;
			_thumbRootRotationDriveXTargetMax = thumbRootJoint.highAngularXLimit.limit;
		}
		else
		{
			_thumbRootRotationDriveXTargetMin = thumbRootJoint.highAngularXLimit.limit;
			_thumbRootRotationDriveXTargetMax = thumbRootJoint.lowAngularXLimit.limit;
		}
		_thumbRootRotationDriveYTargetMin = 0f - thumbRootJoint.angularYLimit.limit;
		_thumbRootRotationDriveYTargetMax = thumbRootJoint.angularYLimit.limit;
		_thumbRootRotationDriveZTargetMin = 0f - thumbRootJoint.angularZLimit.limit;
		_thumbRootRotationDriveZTargetMax = thumbRootJoint.angularZLimit.limit;
	}

	private void Start()
	{
		InitUI();
	}
}
