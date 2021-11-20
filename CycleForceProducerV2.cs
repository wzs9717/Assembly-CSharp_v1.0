using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class CycleForceProducerV2 : ForceProducerV2
{
	[SerializeField]
	private float _period = 1f;

	public Slider periodSlider;

	public Slider periodSliderAlt;

	[SerializeField]
	private float _periodRatio = 0.5f;

	public Slider periodRatioSlider;

	[SerializeField]
	private float _forceDuration = 1f;

	public Slider forceDurationSlider;

	[SerializeField]
	private bool _applyForceOnReturn = true;

	public Toggle applyForceOnReturnToggle;

	private float timer;

	private float forceTimer;

	private float flip;

	public override float forceFactor
	{
		get
		{
			return _forceFactor;
		}
		set
		{
			if (_forceFactor != value)
			{
				base.forceFactor = value;
				maxForce = value;
			}
		}
	}

	public override float torqueFactor
	{
		get
		{
			return _torqueFactor;
		}
		set
		{
			if (_torqueFactor != value)
			{
				base.torqueFactor = value;
				maxTorque = value;
			}
		}
	}

	public float period
	{
		get
		{
			return _period;
		}
		set
		{
			if (_period != value)
			{
				_period = value;
				SyncPeriodSlider();
			}
		}
	}

	public float periodRatio
	{
		get
		{
			return _periodRatio;
		}
		set
		{
			if (_periodRatio != value)
			{
				_periodRatio = value;
				SyncPeriodRatioSlider();
			}
		}
	}

	public float forceDuration
	{
		get
		{
			return _forceDuration;
		}
		set
		{
			if (_forceDuration != value)
			{
				_forceDuration = value;
				SyncForceDurationSlider();
			}
		}
	}

	public bool applyForceOnReturn
	{
		get
		{
			return _applyForceOnReturn;
		}
		set
		{
			if (_applyForceOnReturn != value)
			{
				_applyForceOnReturn = value;
				SyncApplyForceOnReturnToggle();
			}
		}
	}

	public override JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true)
	{
		JSONClass jSON = base.GetJSON(includePhysical, includeAppearance);
		if (includePhysical)
		{
			if (periodSlider != null)
			{
				SliderControl component = periodSlider.GetComponent<SliderControl>();
				if (component == null || component.defaultValue != period)
				{
					needsStore = true;
					jSON["period"].AsFloat = period;
				}
			}
			if (periodRatioSlider != null)
			{
				SliderControl component2 = periodRatioSlider.GetComponent<SliderControl>();
				if (component2 == null || component2.defaultValue != periodRatio)
				{
					needsStore = true;
					jSON["periodRatio"].AsFloat = periodRatio;
				}
			}
			if (forceDurationSlider != null)
			{
				SliderControl component3 = forceDurationSlider.GetComponent<SliderControl>();
				if (component3 == null || component3.defaultValue != forceDuration)
				{
					needsStore = true;
					jSON["forceDuration"].AsFloat = forceDuration;
				}
			}
			if (!applyForceOnReturn)
			{
				needsStore = true;
				jSON["applyForceOnReturn"].AsBool = applyForceOnReturn;
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
		if (jc["period"] != null)
		{
			period = jc["period"].AsFloat;
		}
		else if (periodSlider != null)
		{
			SliderControl component = periodSlider.GetComponent<SliderControl>();
			if (component != null)
			{
				period = component.defaultValue;
			}
		}
		if (jc["periodRatio"] != null)
		{
			periodRatio = jc["periodRatio"].AsFloat;
		}
		else if (periodRatioSlider != null)
		{
			SliderControl component2 = periodRatioSlider.GetComponent<SliderControl>();
			if (component2 != null)
			{
				periodRatio = component2.defaultValue;
			}
		}
		if (jc["forceDuration"] != null)
		{
			forceDuration = jc["forceDuration"].AsFloat;
		}
		else if (forceDurationSlider != null)
		{
			SliderControl component3 = forceDurationSlider.GetComponent<SliderControl>();
			if (component3 != null)
			{
				forceDuration = component3.defaultValue;
			}
		}
		if (jc["applyForceOnReturn"] != null)
		{
			applyForceOnReturn = jc["applyForceOnReturn"].AsBool;
		}
		else
		{
			applyForceOnReturn = true;
		}
	}

	private void InitPeriodSlider()
	{
		if (periodSliderAlt != null)
		{
			periodSliderAlt.onValueChanged.AddListener(delegate
			{
				period = periodSliderAlt.value;
			});
		}
		if (periodSlider != null)
		{
			periodSlider.onValueChanged.AddListener(delegate
			{
				period = periodSlider.value;
			});
			period = periodSlider.value;
		}
	}

	private void SyncPeriodSlider()
	{
		if (periodSlider != null)
		{
			periodSlider.value = _period;
		}
		if (periodSliderAlt != null)
		{
			periodSliderAlt.value = _period;
		}
	}

	private void InitPeriodRatioSlider()
	{
		if (periodRatioSlider != null)
		{
			periodRatioSlider.onValueChanged.AddListener(delegate
			{
				periodRatio = periodRatioSlider.value;
			});
			periodRatio = periodRatioSlider.value;
		}
	}

	private void SyncPeriodRatioSlider()
	{
		if (periodRatioSlider != null)
		{
			periodRatioSlider.value = _periodRatio;
		}
	}

	private void InitForceDurationSlider()
	{
		if (forceDurationSlider != null)
		{
			forceDurationSlider.onValueChanged.AddListener(delegate
			{
				forceDuration = forceDurationSlider.value;
			});
			forceDuration = forceDurationSlider.value;
		}
	}

	private void SyncForceDurationSlider()
	{
		if (forceDurationSlider != null)
		{
			forceDurationSlider.value = _forceDuration;
		}
	}

	private void InitApplyForceOnReturnToggle()
	{
		if (applyForceOnReturnToggle != null)
		{
			applyForceOnReturnToggle.onValueChanged.AddListener(delegate
			{
				applyForceOnReturn = applyForceOnReturnToggle.isOn;
			});
			applyForceOnReturn = applyForceOnReturnToggle.isOn;
		}
	}

	private void SyncApplyForceOnReturnToggle()
	{
		if (applyForceOnReturnToggle != null)
		{
			applyForceOnReturnToggle.isOn = _applyForceOnReturn;
		}
	}

	protected override void InitUI()
	{
		base.InitUI();
		InitApplyForceOnReturnToggle();
		InitForceDurationSlider();
		InitPeriodRatioSlider();
		InitPeriodSlider();
	}

	protected override void Start()
	{
		base.Start();
		flip = 1f;
	}

	protected override void Update()
	{
		base.Update();
		timer -= Time.deltaTime;
		forceTimer -= Time.deltaTime;
		if (timer < 0f)
		{
			if ((flip > 0f && _periodRatio != 1f) || _periodRatio == 0f)
			{
				if (_applyForceOnReturn)
				{
					flip = -1f;
				}
				else
				{
					flip = 0f;
				}
				timer = _period * (1f - _periodRatio);
				forceTimer = _forceDuration * _period;
			}
			else
			{
				flip = 1f;
				timer = _period * periodRatio;
				forceTimer = _forceDuration * _period;
			}
			SetTargetForcePercent(flip);
		}
		else if (forceTimer < 0f)
		{
			SetTargetForcePercent(0f);
		}
	}
}
