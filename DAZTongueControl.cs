using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class DAZTongueControl : JSONStorable
{
	public AdjustJointPositionTargets tongueInOutPositionTargets;

	public AdjustJointHardPositionTargets tongueInOutHardPositionTargets;

	public SetDAZMorph tongueInOutSetDAZMorph;

	public Slider tongueInOutSlider;

	[SerializeField]
	public float _tongueInOut;

	public AdjustRotationTarget tongueUpDownTarget;

	public Slider tongueUpDownSlider;

	[SerializeField]
	public float _tongueUpDown;

	public AdjustJointTargets tongueRotationTargets;

	public Slider tongueUpDownCurlSlider;

	[SerializeField]
	public float _tongueUpDownCurl;

	public Slider tongueRightLeftCurlSlider;

	[SerializeField]
	public float _tongueRightLeftCurl;

	public Slider tongueTwistSlider;

	[SerializeField]
	public float _tongueTwist;

	public Rigidbody[] tongueCollisionRigidbodies;

	public Toggle tongueCollisionToggle;

	[SerializeField]
	public bool _tongueCollision = true;

	public float tongueInOut
	{
		get
		{
			return _tongueInOut;
		}
		set
		{
			if (_tongueInOut != value)
			{
				_tongueInOut = value;
				if (tongueInOutSlider != null)
				{
					tongueInOutSlider.value = value;
				}
				if (tongueInOutPositionTargets != null)
				{
					tongueInOutPositionTargets.percent = value;
				}
				if (tongueInOutHardPositionTargets != null)
				{
					tongueInOutHardPositionTargets.percent = value;
				}
				if (tongueInOutSetDAZMorph != null)
				{
					tongueInOutSetDAZMorph.morphPercent = value;
				}
			}
		}
	}

	public float tongueUpDown
	{
		get
		{
			return _tongueUpDown;
		}
		set
		{
			if (_tongueUpDown != value)
			{
				_tongueUpDown = value;
				if (tongueUpDownSlider != null)
				{
					tongueUpDownSlider.value = value;
				}
				if (tongueUpDownTarget != null)
				{
					tongueUpDownTarget.currentTargetRotationX = value;
				}
			}
		}
	}

	public float tongueUpDownCurl
	{
		get
		{
			return _tongueUpDownCurl;
		}
		set
		{
			if (_tongueUpDownCurl != value)
			{
				_tongueUpDownCurl = value;
				if (tongueUpDownCurlSlider != null)
				{
					tongueUpDownCurlSlider.value = value;
				}
				if (tongueRotationTargets != null)
				{
					tongueRotationTargets.xPercent = value;
				}
			}
		}
	}

	public float tongueRightLeftCurl
	{
		get
		{
			return _tongueRightLeftCurl;
		}
		set
		{
			if (_tongueRightLeftCurl != value)
			{
				_tongueRightLeftCurl = value;
				if (tongueRightLeftCurlSlider != null)
				{
					tongueRightLeftCurlSlider.value = value;
				}
				if (tongueRotationTargets != null)
				{
					tongueRotationTargets.yPercent = value;
				}
			}
		}
	}

	public float tongueTwist
	{
		get
		{
			return _tongueTwist;
		}
		set
		{
			if (_tongueTwist != value)
			{
				_tongueTwist = value;
				if (tongueTwistSlider != null)
				{
					tongueTwistSlider.value = value;
				}
				if (tongueRotationTargets != null)
				{
					tongueRotationTargets.zPercent = value;
				}
			}
		}
	}

	public bool tongueCollision
	{
		get
		{
			return _tongueCollision;
		}
		set
		{
			if (_tongueCollision == value)
			{
				return;
			}
			_tongueCollision = value;
			if (tongueCollisionToggle != null)
			{
				tongueCollisionToggle.isOn = value;
			}
			if (tongueCollisionRigidbodies != null)
			{
				Rigidbody[] array = tongueCollisionRigidbodies;
				foreach (Rigidbody rigidbody in array)
				{
					rigidbody.detectCollisions = value;
				}
			}
		}
	}

	public override JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true)
	{
		JSONClass jSON = base.GetJSON(includePhysical, includeAppearance);
		if (includePhysical)
		{
			if (tongueInOutSlider != null)
			{
				SliderControl component = tongueInOutSlider.GetComponent<SliderControl>();
				if (component == null || component.defaultValue != tongueInOut)
				{
					needsStore = true;
					jSON["tongueInOut"].AsFloat = tongueInOut;
				}
			}
			if (tongueUpDownSlider != null)
			{
				SliderControl component2 = tongueUpDownSlider.GetComponent<SliderControl>();
				if (component2 == null || component2.defaultValue != tongueUpDown)
				{
					needsStore = true;
					jSON["tongueUpDown"].AsFloat = tongueUpDown;
				}
			}
			if (tongueUpDownCurlSlider != null)
			{
				SliderControl component3 = tongueUpDownCurlSlider.GetComponent<SliderControl>();
				if (component3 == null || component3.defaultValue != tongueUpDownCurl)
				{
					needsStore = true;
					jSON["tongueUpDownCurl"].AsFloat = tongueUpDownCurl;
				}
			}
			if (tongueRightLeftCurlSlider != null)
			{
				SliderControl component4 = tongueRightLeftCurlSlider.GetComponent<SliderControl>();
				if (component4 == null || component4.defaultValue != tongueRightLeftCurl)
				{
					needsStore = true;
					jSON["tongueRightLeftCurl"].AsFloat = tongueRightLeftCurl;
				}
			}
			if (tongueTwistSlider != null)
			{
				SliderControl component5 = tongueTwistSlider.GetComponent<SliderControl>();
				if (component5 == null || component5.defaultValue != tongueTwist)
				{
					needsStore = true;
					jSON["tongueTwist"].AsFloat = tongueTwist;
				}
			}
			if (!tongueCollision)
			{
				needsStore = true;
				jSON["tongueCollision"].AsBool = tongueCollision;
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
		if (jc["tongueInOut"] != null)
		{
			tongueInOut = jc["tongueInOut"].AsFloat;
		}
		else if (tongueInOutSlider != null)
		{
			SliderControl component = tongueInOutSlider.GetComponent<SliderControl>();
			if (component != null)
			{
				tongueInOut = component.defaultValue;
			}
		}
		if (jc["tongueUpDown"] != null)
		{
			tongueUpDown = jc["tongueUpDown"].AsFloat;
		}
		else if (tongueUpDownSlider != null)
		{
			SliderControl component2 = tongueUpDownSlider.GetComponent<SliderControl>();
			if (component2 != null)
			{
				tongueUpDown = component2.defaultValue;
			}
		}
		if (jc["tongueUpDownCurl"] != null)
		{
			tongueUpDownCurl = jc["tongueUpDownCurl"].AsFloat;
		}
		else if (tongueUpDownCurlSlider != null)
		{
			SliderControl component3 = tongueUpDownCurlSlider.GetComponent<SliderControl>();
			if (component3 != null)
			{
				tongueUpDownCurl = component3.defaultValue;
			}
		}
		if (jc["tongueRightLeftCurl"] != null)
		{
			tongueRightLeftCurl = jc["tongueRightLeftCurl"].AsFloat;
		}
		else if (tongueRightLeftCurlSlider != null)
		{
			SliderControl component4 = tongueRightLeftCurlSlider.GetComponent<SliderControl>();
			if (component4 != null)
			{
				tongueRightLeftCurl = component4.defaultValue;
			}
		}
		if (jc["tongueTwist"] != null)
		{
			tongueTwist = jc["tongueTwist"].AsFloat;
		}
		else if (tongueTwistSlider != null)
		{
			SliderControl component5 = tongueTwistSlider.GetComponent<SliderControl>();
			if (component5 != null)
			{
				tongueTwist = component5.defaultValue;
			}
		}
		if (jc["tongueCollision"] != null)
		{
			tongueCollision = jc["tongueCollision"].AsBool;
		}
		else
		{
			tongueCollision = true;
		}
	}

	protected void InitUI()
	{
		if (tongueInOutSlider != null)
		{
			tongueInOutSlider.onValueChanged.AddListener(delegate
			{
				tongueInOut = tongueInOutSlider.value;
			});
			tongueInOut = tongueInOutSlider.value;
		}
		if (tongueUpDownSlider != null)
		{
			tongueUpDownSlider.onValueChanged.AddListener(delegate
			{
				tongueUpDown = tongueUpDownSlider.value;
			});
			tongueUpDown = tongueUpDownSlider.value;
		}
		if (tongueUpDownCurlSlider != null)
		{
			tongueUpDownCurlSlider.onValueChanged.AddListener(delegate
			{
				tongueUpDownCurl = tongueUpDownCurlSlider.value;
			});
			tongueUpDownCurl = tongueUpDownCurlSlider.value;
		}
		if (tongueRightLeftCurlSlider != null)
		{
			tongueRightLeftCurlSlider.onValueChanged.AddListener(delegate
			{
				tongueRightLeftCurl = tongueRightLeftCurlSlider.value;
			});
			tongueRightLeftCurl = tongueRightLeftCurlSlider.value;
		}
		if (tongueTwistSlider != null)
		{
			tongueTwistSlider.onValueChanged.AddListener(delegate
			{
				tongueTwist = tongueTwistSlider.value;
			});
			tongueTwist = tongueTwistSlider.value;
		}
		if (tongueCollisionToggle != null)
		{
			tongueCollisionToggle.onValueChanged.AddListener(delegate
			{
				tongueCollision = tongueCollisionToggle.isOn;
			});
			tongueCollision = tongueCollisionToggle.isOn;
		}
	}

	private void Start()
	{
		InitUI();
	}
}
