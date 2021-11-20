using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class AdjustJoints : JSONStorable
{
	public ConfigurableJoint joint1;

	public ConfigurableJoint joint2;

	[SerializeField]
	protected float _mass;

	public Slider massSlider;

	public Slider massSliderAlt;

	public bool useSetCenterOfGravity;

	public Vector3 lowCenterOfGravity;

	public Vector3 highCenterOfGravity;

	public bool useJoint1COGForJoint2 = true;

	public Vector3 lowCenterOfGravityJoint2;

	public Vector3 highCenterOfGravityJoint2;

	protected Vector3 currentCenterOfGravity;

	protected Vector3 currentCenterOfGravityJoint2;

	[SerializeField]
	protected float _centerOfGravityPercent;

	public Slider centerOfGravityPercentSlider;

	public Slider centerOfGravityPercentSliderAlt;

	[SerializeField]
	protected float _spring;

	public Slider springSlider;

	[SerializeField]
	protected float _damper;

	public Slider damperSlider;

	[SerializeField]
	protected float _positionSpringX;

	public Slider positionSpringXSlider;

	[SerializeField]
	protected float _positionDamperX;

	public Slider positionDamperXSlider;

	[SerializeField]
	protected float _positionSpringY;

	public Slider positionSpringYSlider;

	[SerializeField]
	protected float _positionDamperY;

	public Slider positionDamperYSlider;

	[SerializeField]
	protected float _positionSpringZ;

	public Slider positionSpringZSlider;

	[SerializeField]
	protected float _positionDamperZ;

	public Slider positionDamperZSlider;

	public bool useSmoothChanges;

	public Vector3 setJoint1TargetRotation;

	public Vector3 setJoint2TargetRotation;

	public Vector3 smoothedJoint1TargetRotation;

	public Vector3 smoothedJoint1TargetRotationVelocity;

	public Vector3 smoothedJoint2TargetRotation;

	public Vector3 smoothedJoint2TargetRotationVelocity;

	public Slider smoothTargetRotationSpringSlider;

	[SerializeField]
	protected float _smoothTargetRotationSpring = 1f;

	public Slider smoothTargetRotationDamperSlider;

	[SerializeField]
	protected float _smoothTargetRotationDamper = 1f;

	[SerializeField]
	protected float _targetRotationX;

	public Slider targetRotationXSlider;

	public Slider targetRotationXSliderAlt;

	public bool invertJoint2RotationX;

	[SerializeField]
	protected float _targetRotationY;

	public Slider targetRotationYSlider;

	public Slider targetRotationYSliderAlt;

	public bool invertJoint2RotationY;

	[SerializeField]
	protected float _targetRotationZ;

	public Slider targetRotationZSlider;

	public Slider targetRotationZSliderAlt;

	public bool invertJoint2RotationZ;

	public float additionalJoint1RotationX;

	public float additionalJoint1RotationY;

	public float additionalJoint1RotationZ;

	public float additionalJoint2RotationX;

	public float additionalJoint2RotationY;

	public float additionalJoint2RotationZ;

	public float mass
	{
		get
		{
			return _mass;
		}
		set
		{
			if (_mass != value)
			{
				_mass = value;
				if (massSlider != null)
				{
					massSlider.value = value;
				}
				if (massSliderAlt != null)
				{
					massSliderAlt.value = value;
				}
				SyncMass();
			}
		}
	}

	public float centerOfGravityPercent
	{
		get
		{
			return _centerOfGravityPercent;
		}
		set
		{
			if (_centerOfGravityPercent != value)
			{
				_centerOfGravityPercent = value;
				if (centerOfGravityPercentSlider != null)
				{
					centerOfGravityPercentSlider.value = value;
				}
				if (centerOfGravityPercentSliderAlt != null)
				{
					centerOfGravityPercentSliderAlt.value = value;
				}
				SyncCenterOfGravity();
			}
		}
	}

	public float spring
	{
		get
		{
			return _spring;
		}
		set
		{
			if (_spring != value)
			{
				_spring = value;
				if (springSlider != null)
				{
					springSlider.value = value;
				}
				SyncJoint();
			}
		}
	}

	public float damper
	{
		get
		{
			return _damper;
		}
		set
		{
			if (_damper != value)
			{
				_damper = value;
				if (damperSlider != null)
				{
					damperSlider.value = value;
				}
				SyncJoint();
			}
		}
	}

	public float positionSpringX
	{
		get
		{
			return _positionSpringX;
		}
		set
		{
			if (_positionSpringX != value)
			{
				_positionSpringX = value;
				if (positionSpringXSlider != null)
				{
					positionSpringXSlider.value = value;
				}
				SyncJointPositionXDrive();
			}
		}
	}

	public float positionDamperX
	{
		get
		{
			return _positionDamperX;
		}
		set
		{
			if (_positionDamperX != value)
			{
				_positionDamperX = value;
				if (positionDamperXSlider != null)
				{
					positionDamperXSlider.value = value;
				}
				SyncJointPositionXDrive();
			}
		}
	}

	public float positionSpringY
	{
		get
		{
			return _positionSpringY;
		}
		set
		{
			if (_positionSpringY != value)
			{
				_positionSpringY = value;
				if (positionSpringYSlider != null)
				{
					positionSpringYSlider.value = value;
				}
				SyncJointPositionYDrive();
			}
		}
	}

	public float positionDamperY
	{
		get
		{
			return _positionDamperY;
		}
		set
		{
			if (_positionDamperY != value)
			{
				_positionDamperY = value;
				if (positionDamperYSlider != null)
				{
					positionDamperYSlider.value = value;
				}
				SyncJointPositionYDrive();
			}
		}
	}

	public float positionSpringZ
	{
		get
		{
			return _positionSpringZ;
		}
		set
		{
			if (_positionSpringZ != value)
			{
				_positionSpringZ = value;
				if (positionSpringZSlider != null)
				{
					positionSpringZSlider.value = value;
				}
				SyncJointPositionZDrive();
			}
		}
	}

	public float positionDamperZ
	{
		get
		{
			return _positionDamperZ;
		}
		set
		{
			if (_positionDamperZ != value)
			{
				_positionDamperZ = value;
				if (positionDamperZSlider != null)
				{
					positionDamperZSlider.value = value;
				}
				SyncJointPositionZDrive();
			}
		}
	}

	public float smoothTargetRotationSpring
	{
		get
		{
			return _smoothTargetRotationSpring;
		}
		set
		{
			if (_smoothTargetRotationSpring != value)
			{
				_smoothTargetRotationSpring = value;
				if (smoothTargetRotationSpringSlider != null)
				{
					smoothTargetRotationSpringSlider.value = value;
				}
			}
		}
	}

	public float smoothTargetRotationDamper
	{
		get
		{
			return _smoothTargetRotationDamper;
		}
		set
		{
			if (_smoothTargetRotationDamper != value)
			{
				_smoothTargetRotationDamper = value;
				if (smoothTargetRotationDamperSlider != null)
				{
					smoothTargetRotationDamperSlider.value = value;
				}
			}
		}
	}

	public float targetRotationX
	{
		get
		{
			return _targetRotationX;
		}
		set
		{
			if (_targetRotationX != value)
			{
				_targetRotationX = value;
				if (targetRotationXSlider != null)
				{
					targetRotationXSlider.value = value;
				}
				if (targetRotationXSliderAlt != null)
				{
					targetRotationXSliderAlt.value = value;
				}
				SyncTargetRotation();
			}
		}
	}

	public float targetRotationY
	{
		get
		{
			return _targetRotationY;
		}
		set
		{
			if (_targetRotationY != value)
			{
				_targetRotationY = value;
				if (targetRotationYSlider != null)
				{
					targetRotationYSlider.value = value;
				}
				if (targetRotationYSliderAlt != null)
				{
					targetRotationYSliderAlt.value = value;
				}
				SyncTargetRotation();
			}
		}
	}

	public float targetRotationZ
	{
		get
		{
			return _targetRotationZ;
		}
		set
		{
			if (_targetRotationZ != value)
			{
				_targetRotationZ = value;
				if (targetRotationZSlider != null)
				{
					targetRotationZSlider.value = value;
				}
				if (targetRotationZSliderAlt != null)
				{
					targetRotationZSliderAlt.value = value;
				}
				SyncTargetRotation();
			}
		}
	}

	public override JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true)
	{
		JSONClass jSON = base.GetJSON(includePhysical, includeAppearance);
		if (includePhysical)
		{
			if (massSlider != null)
			{
				SliderControl component = massSlider.GetComponent<SliderControl>();
				if (component == null || component.defaultValue != mass)
				{
					needsStore = true;
					jSON["mass"].AsFloat = mass;
				}
			}
			if (centerOfGravityPercentSlider != null)
			{
				SliderControl component2 = centerOfGravityPercentSlider.GetComponent<SliderControl>();
				if (component2 == null || component2.defaultValue != centerOfGravityPercent)
				{
					needsStore = true;
					jSON["centerOfGravityPercent"].AsFloat = centerOfGravityPercent;
				}
			}
			if (springSlider != null)
			{
				SliderControl component3 = springSlider.GetComponent<SliderControl>();
				if (component3 == null || component3.defaultValue != spring)
				{
					needsStore = true;
					jSON["spring"].AsFloat = spring;
				}
			}
			if (damperSlider != null)
			{
				SliderControl component4 = damperSlider.GetComponent<SliderControl>();
				if (component4 == null || component4.defaultValue != damper)
				{
					needsStore = true;
					jSON["damper"].AsFloat = damper;
				}
			}
			if (targetRotationXSlider != null)
			{
				SliderControl component5 = targetRotationXSlider.GetComponent<SliderControl>();
				if (component5 == null || component5.defaultValue != targetRotationX)
				{
					needsStore = true;
					jSON["targetRotationX"].AsFloat = targetRotationX;
				}
			}
			if (targetRotationYSlider != null)
			{
				SliderControl component6 = targetRotationYSlider.GetComponent<SliderControl>();
				if (component6 == null || component6.defaultValue != targetRotationY)
				{
					needsStore = true;
					jSON["targetRotationY"].AsFloat = targetRotationY;
				}
			}
			if (targetRotationZSlider != null)
			{
				SliderControl component7 = targetRotationZSlider.GetComponent<SliderControl>();
				if (component7 == null || component7.defaultValue != targetRotationZ)
				{
					needsStore = true;
					jSON["targetRotationZ"].AsFloat = targetRotationZ;
				}
			}
			if (positionSpringXSlider != null)
			{
				SliderControl component8 = positionSpringXSlider.GetComponent<SliderControl>();
				if (component8 == null || component8.defaultValue != positionSpringX)
				{
					needsStore = true;
					jSON["positionSpringX"].AsFloat = positionSpringX;
				}
			}
			if (positionSpringYSlider != null)
			{
				SliderControl component9 = positionSpringYSlider.GetComponent<SliderControl>();
				if (component9 == null || component9.defaultValue != positionSpringY)
				{
					needsStore = true;
					jSON["positionSpringY"].AsFloat = positionSpringY;
				}
			}
			if (positionSpringZSlider != null)
			{
				SliderControl component10 = positionSpringZSlider.GetComponent<SliderControl>();
				if (component10 == null || component10.defaultValue != positionSpringZ)
				{
					needsStore = true;
					jSON["positionSpringZ"].AsFloat = positionSpringZ;
				}
			}
			if (positionDamperXSlider != null)
			{
				SliderControl component11 = positionDamperXSlider.GetComponent<SliderControl>();
				if (component11 == null || component11.defaultValue != positionDamperX)
				{
					needsStore = true;
					jSON["positionDamperX"].AsFloat = positionDamperX;
				}
			}
			if (positionDamperYSlider != null)
			{
				SliderControl component12 = positionDamperYSlider.GetComponent<SliderControl>();
				if (component12 == null || component12.defaultValue != positionDamperY)
				{
					needsStore = true;
					jSON["positionDamperY"].AsFloat = positionDamperY;
				}
			}
			if (positionDamperZSlider != null)
			{
				SliderControl component13 = positionDamperZSlider.GetComponent<SliderControl>();
				if (component13 == null || component13.defaultValue != positionDamperZ)
				{
					needsStore = true;
					jSON["positionDamperZ"].AsFloat = positionDamperZ;
				}
			}
			if (smoothTargetRotationSpringSlider != null)
			{
				SliderControl component14 = smoothTargetRotationSpringSlider.GetComponent<SliderControl>();
				if (component14 == null || component14.defaultValue != smoothTargetRotationSpring)
				{
					needsStore = true;
					jSON["smoothTargetRotationSpring"].AsFloat = smoothTargetRotationSpring;
				}
			}
			if (smoothTargetRotationDamperSlider != null)
			{
				SliderControl component15 = smoothTargetRotationDamperSlider.GetComponent<SliderControl>();
				if (component15 == null || component15.defaultValue != smoothTargetRotationDamper)
				{
					needsStore = true;
					jSON["smoothTargetRotationDamper"].AsFloat = smoothTargetRotationDamper;
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
		if (jc["mass"] != null)
		{
			mass = jc["mass"].AsFloat;
		}
		else if (massSlider != null)
		{
			SliderControl component = massSlider.GetComponent<SliderControl>();
			if (component != null)
			{
				mass = component.defaultValue;
			}
		}
		if (jc["centerOfGravityPercent"] != null)
		{
			centerOfGravityPercent = jc["centerOfGravityPercent"].AsFloat;
		}
		else if (centerOfGravityPercentSlider != null)
		{
			SliderControl component2 = centerOfGravityPercentSlider.GetComponent<SliderControl>();
			if (component2 != null)
			{
				centerOfGravityPercent = component2.defaultValue;
			}
		}
		if (jc["spring"] != null)
		{
			spring = jc["spring"].AsFloat;
		}
		else if (springSlider != null)
		{
			SliderControl component3 = springSlider.GetComponent<SliderControl>();
			if (component3 != null)
			{
				spring = component3.defaultValue;
			}
		}
		if (jc["damper"] != null)
		{
			damper = jc["damper"].AsFloat;
		}
		else if (damperSlider != null)
		{
			SliderControl component4 = damperSlider.GetComponent<SliderControl>();
			if (component4 != null)
			{
				damper = component4.defaultValue;
			}
		}
		if (jc["targetRotationX"] != null)
		{
			targetRotationX = jc["targetRotationX"].AsFloat;
		}
		else if (targetRotationXSlider != null)
		{
			SliderControl component5 = targetRotationXSlider.GetComponent<SliderControl>();
			if (component5 != null)
			{
				targetRotationX = component5.defaultValue;
			}
		}
		if (jc["targetRotationY"] != null)
		{
			targetRotationY = jc["targetRotationY"].AsFloat;
		}
		else if (targetRotationYSlider != null)
		{
			SliderControl component6 = targetRotationYSlider.GetComponent<SliderControl>();
			if (component6 != null)
			{
				targetRotationY = component6.defaultValue;
			}
		}
		if (jc["targetRotationZ"] != null)
		{
			targetRotationZ = jc["targetRotationZ"].AsFloat;
		}
		else if (targetRotationZSlider != null)
		{
			SliderControl component7 = targetRotationZSlider.GetComponent<SliderControl>();
			if (component7 != null)
			{
				targetRotationZ = component7.defaultValue;
			}
		}
		if (jc["positionSpringX"] != null)
		{
			positionSpringX = jc["positionSpringX"].AsFloat;
		}
		else if (positionSpringXSlider != null)
		{
			SliderControl component8 = positionSpringXSlider.GetComponent<SliderControl>();
			if (component8 != null)
			{
				positionSpringX = component8.defaultValue;
			}
		}
		if (jc["positionSpringY"] != null)
		{
			positionSpringY = jc["positionSpringY"].AsFloat;
		}
		else if (positionSpringYSlider != null)
		{
			SliderControl component9 = positionSpringYSlider.GetComponent<SliderControl>();
			if (component9 != null)
			{
				positionSpringY = component9.defaultValue;
			}
		}
		if (jc["positionSpringZ"] != null)
		{
			positionSpringZ = jc["positionSpringZ"].AsFloat;
		}
		else if (positionSpringZSlider != null)
		{
			SliderControl component10 = positionSpringZSlider.GetComponent<SliderControl>();
			if (component10 != null)
			{
				positionSpringZ = component10.defaultValue;
			}
		}
		if (jc["positionDamperX"] != null)
		{
			positionDamperX = jc["positionDamperX"].AsFloat;
		}
		else if (positionDamperXSlider != null)
		{
			SliderControl component11 = positionDamperXSlider.GetComponent<SliderControl>();
			if (component11 != null)
			{
				positionDamperX = component11.defaultValue;
			}
		}
		if (jc["positionDamperY"] != null)
		{
			positionDamperY = jc["positionDamperY"].AsFloat;
		}
		else if (positionDamperYSlider != null)
		{
			SliderControl component12 = positionDamperYSlider.GetComponent<SliderControl>();
			if (component12 != null)
			{
				positionDamperY = component12.defaultValue;
			}
		}
		if (jc["positionDamperZ"] != null)
		{
			positionDamperZ = jc["positionDamperZ"].AsFloat;
		}
		else if (positionDamperZSlider != null)
		{
			SliderControl component13 = positionDamperZSlider.GetComponent<SliderControl>();
			if (component13 != null)
			{
				positionDamperZ = component13.defaultValue;
			}
		}
	}

	protected void SyncMass()
	{
		if (joint1 != null)
		{
			Rigidbody component = joint1.GetComponent<Rigidbody>();
			if (component != null && component.mass != _mass)
			{
				component.mass = _mass;
			}
		}
		if (joint2 != null)
		{
			Rigidbody component2 = joint2.GetComponent<Rigidbody>();
			if (component2 != null && component2.mass != _mass)
			{
				component2.mass = _mass;
			}
		}
	}

	protected void SyncCenterOfGravity()
	{
		if (!useSetCenterOfGravity)
		{
			return;
		}
		currentCenterOfGravity = Vector3.Lerp(lowCenterOfGravity, highCenterOfGravity, _centerOfGravityPercent);
		if (joint1 != null)
		{
			Rigidbody component = joint1.GetComponent<Rigidbody>();
			if (component != null && component.centerOfMass != currentCenterOfGravity)
			{
				component.centerOfMass = currentCenterOfGravity;
			}
		}
		if (joint2 != null)
		{
			Rigidbody component2 = joint2.GetComponent<Rigidbody>();
			if (useJoint1COGForJoint2)
			{
				currentCenterOfGravityJoint2 = currentCenterOfGravity;
			}
			else
			{
				currentCenterOfGravityJoint2 = Vector3.Lerp(lowCenterOfGravityJoint2, highCenterOfGravityJoint2, _centerOfGravityPercent);
			}
			if (component2 != null && component2.centerOfMass != currentCenterOfGravityJoint2)
			{
				component2.centerOfMass = currentCenterOfGravityJoint2;
			}
		}
	}

	protected void SyncJoint()
	{
		if (joint1 != null)
		{
			JointDrive slerpDrive = joint1.slerpDrive;
			slerpDrive.positionSpring = _spring;
			slerpDrive.positionDamper = _damper;
			joint1.slerpDrive = slerpDrive;
			slerpDrive = joint1.angularXDrive;
			slerpDrive.positionSpring = _spring;
			slerpDrive.positionDamper = _damper;
			joint1.angularXDrive = slerpDrive;
			slerpDrive = joint1.angularYZDrive;
			slerpDrive.positionSpring = _spring;
			slerpDrive.positionDamper = _damper;
			joint1.angularYZDrive = slerpDrive;
		}
		if (joint2 != null)
		{
			JointDrive slerpDrive2 = joint2.slerpDrive;
			slerpDrive2.positionSpring = _spring;
			slerpDrive2.positionDamper = _damper;
			joint2.slerpDrive = slerpDrive2;
			slerpDrive2 = joint2.angularXDrive;
			slerpDrive2.positionSpring = _spring;
			slerpDrive2.positionDamper = _damper;
			joint2.angularXDrive = slerpDrive2;
			slerpDrive2 = joint2.angularYZDrive;
			slerpDrive2.positionSpring = _spring;
			slerpDrive2.positionDamper = _damper;
			joint2.angularYZDrive = slerpDrive2;
		}
	}

	protected void SyncJointPositionXDrive()
	{
		if (joint1 != null)
		{
			JointDrive xDrive = joint1.xDrive;
			xDrive.positionSpring = _positionSpringX;
			xDrive.positionDamper = _positionDamperX;
			joint1.xDrive = xDrive;
		}
		if (joint2 != null)
		{
			JointDrive xDrive2 = joint2.xDrive;
			xDrive2.positionSpring = _positionSpringX;
			xDrive2.positionDamper = _positionDamperX;
			joint2.xDrive = xDrive2;
		}
	}

	protected void SyncJointPositionYDrive()
	{
		if (joint1 != null)
		{
			JointDrive yDrive = joint1.yDrive;
			yDrive.positionSpring = _positionSpringY;
			yDrive.positionDamper = _positionDamperY;
			joint1.yDrive = yDrive;
		}
		if (joint2 != null)
		{
			JointDrive yDrive2 = joint2.yDrive;
			yDrive2.positionSpring = _positionSpringY;
			yDrive2.positionDamper = _positionDamperY;
			joint2.yDrive = yDrive2;
		}
	}

	protected void SyncJointPositionZDrive()
	{
		if (joint1 != null)
		{
			JointDrive zDrive = joint1.zDrive;
			zDrive.positionSpring = _positionSpringZ;
			zDrive.positionDamper = _positionDamperZ;
			joint1.zDrive = zDrive;
		}
		if (joint2 != null)
		{
			JointDrive zDrive2 = joint2.zDrive;
			zDrive2.positionSpring = _positionSpringZ;
			zDrive2.positionDamper = _positionDamperZ;
			joint2.zDrive = zDrive2;
		}
	}

	protected void SetTargetRotation()
	{
		if (joint1 != null)
		{
			Quaternion targetRotation = Quaternion.Euler(smoothedJoint1TargetRotation);
			joint1.targetRotation = targetRotation;
		}
		if (joint2 != null)
		{
			Quaternion targetRotation2 = Quaternion.Euler(smoothedJoint2TargetRotation);
			joint2.targetRotation = targetRotation2;
		}
	}

	protected void SmoothTargetRotation()
	{
		if (joint1 != null)
		{
			smoothedJoint1TargetRotation += (smoothedJoint1TargetRotationVelocity = (1f - 0.5f * smoothTargetRotationDamper) * smoothedJoint1TargetRotationVelocity + 0.5f * smoothTargetRotationSpring * (setJoint1TargetRotation - smoothedJoint1TargetRotation));
		}
		if (joint2 != null)
		{
			smoothedJoint2TargetRotation += (smoothedJoint2TargetRotationVelocity = (1f - 0.5f * smoothTargetRotationDamper) * smoothedJoint2TargetRotationVelocity + 0.5f * smoothTargetRotationSpring * (setJoint2TargetRotation - smoothedJoint2TargetRotation));
		}
		SetTargetRotation();
	}

	public void SyncTargetRotation()
	{
		if (joint1 != null)
		{
			setJoint1TargetRotation.x = targetRotationX + additionalJoint1RotationX;
			setJoint1TargetRotation.y = targetRotationY + additionalJoint1RotationY;
			setJoint1TargetRotation.z = targetRotationZ + additionalJoint1RotationZ;
		}
		if (joint2 != null)
		{
			if (invertJoint2RotationX)
			{
				setJoint2TargetRotation.x = 0f - targetRotationX + additionalJoint2RotationX;
			}
			else
			{
				setJoint2TargetRotation.x = targetRotationX + additionalJoint2RotationX;
			}
			if (invertJoint2RotationY)
			{
				setJoint2TargetRotation.y = 0f - targetRotationY + additionalJoint2RotationY;
			}
			else
			{
				setJoint2TargetRotation.y = targetRotationY + additionalJoint2RotationY;
			}
			if (invertJoint2RotationZ)
			{
				setJoint2TargetRotation.z = 0f - targetRotationZ + additionalJoint2RotationZ;
			}
			else
			{
				setJoint2TargetRotation.z = targetRotationZ + additionalJoint2RotationZ;
			}
		}
		if (!useSmoothChanges)
		{
			smoothedJoint1TargetRotation = setJoint1TargetRotation;
			smoothedJoint2TargetRotation = setJoint2TargetRotation;
			SetTargetRotation();
		}
	}

	protected void InitUI()
	{
		if (massSlider != null)
		{
			massSlider.value = _mass;
			massSlider.onValueChanged.AddListener(delegate
			{
				mass = massSlider.value;
			});
			SliderControl component = massSlider.GetComponent<SliderControl>();
			if (component != null)
			{
				component.defaultValue = _mass;
			}
		}
		if (massSliderAlt != null)
		{
			massSliderAlt.value = _mass;
			massSliderAlt.onValueChanged.AddListener(delegate
			{
				mass = massSliderAlt.value;
			});
			SliderControl component2 = massSliderAlt.GetComponent<SliderControl>();
			if (component2 != null)
			{
				component2.defaultValue = _mass;
			}
		}
		if (centerOfGravityPercentSlider != null)
		{
			centerOfGravityPercentSlider.value = _centerOfGravityPercent;
			centerOfGravityPercentSlider.onValueChanged.AddListener(delegate
			{
				centerOfGravityPercent = centerOfGravityPercentSlider.value;
			});
			SliderControl component3 = centerOfGravityPercentSlider.GetComponent<SliderControl>();
			if (component3 != null)
			{
				component3.defaultValue = _centerOfGravityPercent;
			}
		}
		if (centerOfGravityPercentSliderAlt != null)
		{
			centerOfGravityPercentSliderAlt.value = _centerOfGravityPercent;
			centerOfGravityPercentSliderAlt.onValueChanged.AddListener(delegate
			{
				centerOfGravityPercent = centerOfGravityPercentSliderAlt.value;
			});
			SliderControl component4 = centerOfGravityPercentSliderAlt.GetComponent<SliderControl>();
			if (component4 != null)
			{
				component4.defaultValue = _centerOfGravityPercent;
			}
		}
		if (springSlider != null)
		{
			springSlider.value = _spring;
			springSlider.onValueChanged.AddListener(delegate
			{
				spring = springSlider.value;
			});
			SliderControl component5 = springSlider.GetComponent<SliderControl>();
			if (component5 != null)
			{
				component5.defaultValue = _spring;
			}
		}
		if (damperSlider != null)
		{
			damperSlider.value = _damper;
			damperSlider.onValueChanged.AddListener(delegate
			{
				damper = damperSlider.value;
			});
			SliderControl component6 = damperSlider.GetComponent<SliderControl>();
			if (component6 != null)
			{
				component6.defaultValue = _damper;
			}
		}
		if (targetRotationXSlider != null)
		{
			targetRotationXSlider.value = _targetRotationX;
			targetRotationXSlider.onValueChanged.AddListener(delegate
			{
				targetRotationX = targetRotationXSlider.value;
			});
			SliderControl component7 = targetRotationXSlider.GetComponent<SliderControl>();
			if (component7 != null)
			{
				component7.defaultValue = _targetRotationX;
			}
		}
		if (targetRotationXSliderAlt != null)
		{
			targetRotationXSliderAlt.value = _targetRotationX;
			targetRotationXSliderAlt.onValueChanged.AddListener(delegate
			{
				targetRotationX = targetRotationXSliderAlt.value;
			});
			SliderControl component8 = targetRotationXSliderAlt.GetComponent<SliderControl>();
			if (component8 != null)
			{
				component8.defaultValue = _targetRotationX;
			}
		}
		if (targetRotationYSlider != null)
		{
			targetRotationYSlider.value = _targetRotationY;
			targetRotationYSlider.onValueChanged.AddListener(delegate
			{
				targetRotationY = targetRotationYSlider.value;
			});
			SliderControl component9 = targetRotationYSlider.GetComponent<SliderControl>();
			if (component9 != null)
			{
				component9.defaultValue = _targetRotationY;
			}
		}
		if (targetRotationYSliderAlt != null)
		{
			targetRotationYSliderAlt.value = _targetRotationY;
			targetRotationYSliderAlt.onValueChanged.AddListener(delegate
			{
				targetRotationY = targetRotationYSliderAlt.value;
			});
			SliderControl component10 = targetRotationYSliderAlt.GetComponent<SliderControl>();
			if (component10 != null)
			{
				component10.defaultValue = _targetRotationY;
			}
		}
		if (targetRotationZSlider != null)
		{
			targetRotationZSlider.value = _targetRotationZ;
			targetRotationZSlider.onValueChanged.AddListener(delegate
			{
				targetRotationZ = targetRotationZSlider.value;
			});
			SliderControl component11 = targetRotationZSlider.GetComponent<SliderControl>();
			if (component11 != null)
			{
				component11.defaultValue = _targetRotationZ;
			}
		}
		if (targetRotationZSliderAlt != null)
		{
			targetRotationZSliderAlt.value = _targetRotationZ;
			targetRotationZSliderAlt.onValueChanged.AddListener(delegate
			{
				targetRotationZ = targetRotationZSliderAlt.value;
			});
			SliderControl component12 = targetRotationZSliderAlt.GetComponent<SliderControl>();
			if (component12 != null)
			{
				component12.defaultValue = _targetRotationZ;
			}
		}
		if (positionSpringXSlider != null)
		{
			positionSpringXSlider.value = _positionSpringX;
			positionSpringXSlider.onValueChanged.AddListener(delegate
			{
				positionSpringX = positionSpringXSlider.value;
			});
			SliderControl component13 = positionSpringXSlider.GetComponent<SliderControl>();
			if (component13 != null)
			{
				component13.defaultValue = _positionSpringX;
			}
		}
		if (positionDamperXSlider != null)
		{
			positionDamperXSlider.value = _positionDamperX;
			positionDamperXSlider.onValueChanged.AddListener(delegate
			{
				positionDamperX = positionDamperXSlider.value;
			});
			positionDamperX = positionDamperXSlider.value;
			SliderControl component14 = positionDamperXSlider.GetComponent<SliderControl>();
			if (component14 != null)
			{
				component14.defaultValue = _positionDamperX;
			}
		}
		if (positionSpringYSlider != null)
		{
			positionSpringYSlider.value = _positionSpringY;
			positionSpringYSlider.onValueChanged.AddListener(delegate
			{
				positionSpringY = positionSpringYSlider.value;
			});
			SliderControl component15 = positionSpringYSlider.GetComponent<SliderControl>();
			if (component15 != null)
			{
				component15.defaultValue = _positionSpringY;
			}
		}
		if (positionDamperYSlider != null)
		{
			positionDamperYSlider.value = _positionDamperY;
			positionDamperYSlider.onValueChanged.AddListener(delegate
			{
				positionDamperY = positionDamperYSlider.value;
			});
			SliderControl component16 = positionDamperYSlider.GetComponent<SliderControl>();
			if (component16 != null)
			{
				component16.defaultValue = _positionDamperY;
			}
		}
		if (positionSpringZSlider != null)
		{
			positionSpringZSlider.value = _positionSpringZ;
			positionSpringZSlider.onValueChanged.AddListener(delegate
			{
				positionSpringZ = positionSpringZSlider.value;
			});
			SliderControl component17 = positionSpringZSlider.GetComponent<SliderControl>();
			if (component17 != null)
			{
				component17.defaultValue = _positionSpringZ;
			}
		}
		if (positionDamperZSlider != null)
		{
			positionDamperZSlider.value = _positionDamperZ;
			positionDamperZSlider.onValueChanged.AddListener(delegate
			{
				positionDamperZ = positionDamperZSlider.value;
			});
			SliderControl component18 = positionDamperZSlider.GetComponent<SliderControl>();
			if (component18 != null)
			{
				component18.defaultValue = _positionDamperZ;
			}
		}
		if (smoothTargetRotationSpringSlider != null)
		{
			smoothTargetRotationSpringSlider.value = _smoothTargetRotationSpring;
			smoothTargetRotationSpringSlider.onValueChanged.AddListener(delegate
			{
				smoothTargetRotationSpring = smoothTargetRotationSpringSlider.value;
			});
			SliderControl component19 = smoothTargetRotationSpringSlider.GetComponent<SliderControl>();
			if (component19 != null)
			{
				component19.defaultValue = _smoothTargetRotationSpring;
			}
		}
		if (smoothTargetRotationDamperSlider != null)
		{
			smoothTargetRotationDamperSlider.value = _smoothTargetRotationDamper;
			smoothTargetRotationDamperSlider.onValueChanged.AddListener(delegate
			{
				smoothTargetRotationDamper = smoothTargetRotationDamperSlider.value;
			});
			SliderControl component20 = smoothTargetRotationDamperSlider.GetComponent<SliderControl>();
			if (component20 != null)
			{
				component20.defaultValue = _smoothTargetRotationDamper;
			}
		}
	}

	protected void SyncAll()
	{
		SyncMass();
		SyncCenterOfGravity();
		SyncJoint();
		SyncTargetRotation();
		SyncJointPositionXDrive();
		SyncJointPositionYDrive();
		SyncJointPositionZDrive();
	}

	private void Update()
	{
		if (useSmoothChanges)
		{
			SmoothTargetRotation();
		}
	}

	private void Start()
	{
		InitUI();
		SyncAll();
	}
}
