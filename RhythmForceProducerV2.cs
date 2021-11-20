using System;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class RhythmForceProducerV2 : ForceProducerV2
{
	public enum RangeSelect
	{
		Low,
		Mid,
		High
	}

	public RhythmController controller;

	public RangeSelect rangeSelect;

	[SerializeField]
	private float _alternateBeatRatio = 1f;

	public Slider alternateBeatRatioSlider;

	[SerializeField]
	private float _threshold = 1f;

	public Slider thresholdSlider;

	[SerializeField]
	private float _burstLength = 1f;

	public Slider burstLengthSlider;

	[SerializeField]
	private float _minSpacing = 0.4f;

	public Slider minSpacingSlider;

	[SerializeField]
	private float _randomFactor = 0.1f;

	public Slider randomFactorSlider;

	public Material rhythmLineMaterial;

	public Material rawRhythmLineMaterial;

	public UIPopup rangeSelectPopup;

	private float minThreshold = 1f;

	private float flip = 1f;

	private bool timerOn;

	private float timer;

	private float forceTimer;

	private float maxOnset = 60f;

	public float peakOnset;

	private float oneOverMaxOnset;

	private float onsetMult = 2f;

	private float rawOnset;

	private float onset;

	private LineDrawer rhythmLineDrawer;

	private LineDrawer rawRhythmLineDrawer;

	private float rhythmLineLength;

	private float rawRhythmLineLength;

	public float alternateBeatRatio
	{
		get
		{
			return _alternateBeatRatio;
		}
		set
		{
			if (_alternateBeatRatio != value)
			{
				_alternateBeatRatio = value;
				SyncAlternateBeatRatioSlider();
			}
		}
	}

	public float threshold
	{
		get
		{
			return _threshold;
		}
		set
		{
			if (_threshold != value)
			{
				_threshold = value;
				SyncThresholdSlider();
			}
		}
	}

	public float burstLength
	{
		get
		{
			return _burstLength;
		}
		set
		{
			if (_burstLength != value)
			{
				_burstLength = value;
				SyncBurstLengthSlider();
			}
		}
	}

	public float minSpacing
	{
		get
		{
			return _minSpacing;
		}
		set
		{
			if (_minSpacing != value)
			{
				_minSpacing = value;
				SyncMinSpacingSlider();
			}
		}
	}

	public float randomFactor
	{
		get
		{
			return _randomFactor;
		}
		set
		{
			if (_randomFactor != value)
			{
				_randomFactor = value;
				SyncRandomFactorSlider();
			}
		}
	}

	public override JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true)
	{
		JSONClass jSON = base.GetJSON(includePhysical, includeAppearance);
		if (includePhysical)
		{
			if (rangeSelect != 0)
			{
				needsStore = true;
				jSON["range"] = rangeSelect.ToString();
			}
			if (alternateBeatRatioSlider != null)
			{
				SliderControl component = alternateBeatRatioSlider.GetComponent<SliderControl>();
				if (component == null || component.defaultValue != alternateBeatRatio)
				{
					needsStore = true;
					jSON["alternateBeatRatio"].AsFloat = alternateBeatRatio;
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
		if (jc["range"] != null)
		{
			SetRangeSelect(jc["range"]);
		}
		else
		{
			SetRangeSelect("Low");
		}
		if (jc["alternateBeatRatio"] != null)
		{
			alternateBeatRatio = jc["alternateBeatRatio"].AsFloat;
		}
		else if (alternateBeatRatioSlider != null)
		{
			SliderControl component = alternateBeatRatioSlider.GetComponent<SliderControl>();
			if (component != null)
			{
				alternateBeatRatio = component.defaultValue;
			}
		}
	}

	private void InitAlternateBeatRatioSlider()
	{
		if (alternateBeatRatioSlider != null)
		{
			alternateBeatRatioSlider.onValueChanged.AddListener(delegate
			{
				alternateBeatRatio = alternateBeatRatioSlider.value;
			});
			alternateBeatRatio = alternateBeatRatioSlider.value;
		}
	}

	private void SyncAlternateBeatRatioSlider()
	{
		if (alternateBeatRatioSlider != null)
		{
			alternateBeatRatioSlider.value = _alternateBeatRatio;
		}
	}

	private void InitThresholdSlider()
	{
		if (thresholdSlider != null)
		{
			thresholdSlider.onValueChanged.AddListener(delegate
			{
				threshold = thresholdSlider.value;
			});
			threshold = thresholdSlider.value;
		}
	}

	private void SyncThresholdSlider()
	{
		if (thresholdSlider != null)
		{
			thresholdSlider.value = _threshold;
		}
	}

	private void InitBurstLengthSlider()
	{
		if (burstLengthSlider != null)
		{
			burstLengthSlider.onValueChanged.AddListener(delegate
			{
				burstLength = burstLengthSlider.value;
			});
			burstLength = burstLengthSlider.value;
		}
	}

	private void SyncBurstLengthSlider()
	{
		if (burstLengthSlider != null)
		{
			burstLengthSlider.value = _burstLength;
		}
	}

	private void InitMinSpacingSlider()
	{
		if (minSpacingSlider != null)
		{
			minSpacingSlider.onValueChanged.AddListener(delegate
			{
				minSpacing = minSpacingSlider.value;
			});
			minSpacing = minSpacingSlider.value;
		}
	}

	private void SyncMinSpacingSlider()
	{
		if (minSpacingSlider != null)
		{
			minSpacingSlider.value = _minSpacing;
		}
	}

	private void InitRandomFactorSlider()
	{
		if (randomFactorSlider != null)
		{
			randomFactorSlider.onValueChanged.AddListener(delegate
			{
				randomFactor = randomFactorSlider.value;
			});
			randomFactor = randomFactorSlider.value;
		}
	}

	private void SyncRandomFactorSlider()
	{
		if (randomFactorSlider != null)
		{
			randomFactorSlider.value = _randomFactor;
		}
	}

	public void SetRangeSelect(string range)
	{
		try
		{
			rangeSelect = (RangeSelect)Enum.Parse(typeof(RangeSelect), range, ignoreCase: true);
			if (rangeSelectPopup != null)
			{
				rangeSelectPopup.currentValue = range;
			}
		}
		catch (ArgumentException)
		{
		}
	}

	protected override void InitUI()
	{
		base.InitUI();
		InitAlternateBeatRatioSlider();
		InitBurstLengthSlider();
		InitMinSpacingSlider();
		InitRandomFactorSlider();
		InitThresholdSlider();
		if ((bool)rangeSelectPopup)
		{
			UIPopup uIPopup = rangeSelectPopup;
			uIPopup.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup.onValueChangeHandlers, new UIPopup.OnValueChange(SetRangeSelect));
		}
		if ((bool)rhythmLineMaterial)
		{
			rhythmLineDrawer = new LineDrawer(rhythmLineMaterial);
		}
		if ((bool)rawRhythmLineMaterial)
		{
			rawRhythmLineDrawer = new LineDrawer(rawRhythmLineMaterial);
		}
	}

	protected override void Start()
	{
		base.Start();
		oneOverMaxOnset = 1f / maxOnset;
		if (controller == null)
		{
			controller = RhythmController.singleton;
		}
	}

	protected override void Update()
	{
		base.Update();
		forceTimer -= Time.deltaTime;
		if (forceTimer > 0f)
		{
			SetTargetForcePercent(flip * onset * oneOverMaxOnset);
			if (onset > peakOnset)
			{
				peakOnset = onset;
			}
		}
		else
		{
			SetTargetForcePercent(0f);
		}
		if (!(controller != null) || controller.rhythmTool == null || controller.low == null)
		{
			return;
		}
		float num;
		if (rangeSelect != 0)
		{
			num = ((rangeSelect != RangeSelect.Mid) ? (controller.high[controller.rhythmTool.CurrentFrame].onset * onsetMult) : (controller.mid[controller.rhythmTool.CurrentFrame].onset * onsetMult));
		}
		else if (controller.rhythmTool.CurrentFrame >= controller.low.Length)
		{
			Debug.LogError("Rhythm frame " + controller.rhythmTool.CurrentFrame + " is greater than analysis length " + controller.low.Length);
			num = 0f;
		}
		else
		{
			num = controller.low[controller.rhythmTool.CurrentFrame].onset * onsetMult;
		}
		if (num > maxOnset)
		{
			num = maxOnset;
		}
		if (num > minThreshold)
		{
			rawOnset = num;
			rawRhythmLineLength = rawOnset * linesScale * _forceFactor * oneOverMaxOnset;
		}
		if (timerOn)
		{
			timer -= Time.deltaTime;
			if (timer < 0f)
			{
				timerOn = false;
			}
		}
		else
		{
			onset = num;
			if (onset > threshold)
			{
				if (flip > 0f)
				{
					flip = 0f - _alternateBeatRatio;
				}
				else
				{
					flip = 1f;
				}
				timerOn = true;
				timer = _minSpacing;
				forceTimer = _burstLength;
				if (UnityEngine.Random.value < _randomFactor)
				{
					timer += _minSpacing;
					forceTimer += _burstLength;
				}
				rhythmLineLength = rawRhythmLineLength * flip;
			}
		}
		rawRhythmLineLength = Mathf.Lerp(rawRhythmLineLength, 0f, Time.deltaTime * 5f);
		if (on && receiver != null && drawLines)
		{
			Vector3 vector = AxisToVector(forceAxis);
			Vector3 vector2 = AxisToUpVector(forceAxis);
			Vector3 vector3 = base.transform.position + vector2 * (lineOffset + lineSpacing * 10f);
			if (rhythmLineDrawer != null)
			{
				rhythmLineDrawer.SetLinePoints(vector3, vector3 + vector * rhythmLineLength);
				rhythmLineDrawer.Draw(base.gameObject.layer);
			}
			vector3 += vector2 * lineSpacing;
			if (rawRhythmLineDrawer != null)
			{
				rawRhythmLineDrawer.SetLinePoints(vector3, vector3 + vector * (0f - rawRhythmLineLength));
				rawRhythmLineDrawer.Draw(base.gameObject.layer);
			}
		}
	}
}
