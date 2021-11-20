using System;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class AnimationStep : CubicBezierPoint
{
	public enum CurveType
	{
		Linear,
		EaseInOut
	}

	public Atom containingAtom;

	public AnimationPattern animationParent;

	[SerializeField]
	protected int _stepNumber;

	public Text stepNameText;

	public Slider transitionToTimeSlider;

	[SerializeField]
	protected float _transitionToTime = 1f;

	public UIPopup curveTypePopup;

	[SerializeField]
	protected CurveType _curveType;

	public AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public int stepNumber
	{
		get
		{
			return _stepNumber;
		}
		set
		{
			if (_stepNumber == value)
			{
				return;
			}
			_stepNumber = value;
			if (stepNameText != null)
			{
				string text = string.Empty;
				if (animationParent != null)
				{
					text = animationParent.uid;
				}
				stepNameText.text = text + " Step " + _stepNumber;
			}
		}
	}

	public float transitionToTime
	{
		get
		{
			return _transitionToTime;
		}
		set
		{
			if (_transitionToTime != value)
			{
				_transitionToTime = value;
				if (transitionToTimeSlider != null)
				{
					transitionToTimeSlider.value = _transitionToTime;
				}
			}
		}
	}

	public CurveType curveType
	{
		get
		{
			return _curveType;
		}
		set
		{
			if (_curveType != value)
			{
				_curveType = value;
				if (curveTypePopup != null)
				{
					curveTypePopup.currentValue = _curveType.ToString();
				}
				switch (_curveType)
				{
				case CurveType.Linear:
					curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
					break;
				case CurveType.EaseInOut:
					curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
					break;
				}
			}
		}
	}

	public override JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true)
	{
		JSONClass jSON = base.GetJSON(includePhysical, includeAppearance);
		if (includePhysical)
		{
			if (transitionToTimeSlider != null)
			{
				SliderControl component = transitionToTimeSlider.GetComponent<SliderControl>();
				if (component == null || component.defaultValue != _transitionToTime)
				{
					needsStore = true;
					jSON["transitionToTime"].AsFloat = _transitionToTime;
				}
			}
			if (_curveType != 0)
			{
				jSON["curveType"] = _curveType.ToString();
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
		if (jc["transitionToTime"] != null)
		{
			transitionToTime = jc["transitionToTime"].AsFloat;
		}
		else if (transitionToTimeSlider != null)
		{
			SliderControl component = transitionToTimeSlider.GetComponent<SliderControl>();
			if (component != null)
			{
				transitionToTime = component.defaultValue;
			}
		}
		if (jc["curveType"] != null)
		{
			SetCurveType(jc["curveType"]);
		}
	}

	public void SetCurveType(string type)
	{
		try
		{
			CurveType curveType2 = (this.curveType = (CurveType)Enum.Parse(typeof(CurveType), type));
		}
		catch (ArgumentException)
		{
			Debug.LogError("Attempted to set curve type to " + type + " which is not a valid curve type");
		}
	}

	public void CreateStepBefore()
	{
		if (animationParent != null)
		{
			animationParent.CreateStepBeforeStep(this);
		}
	}

	public void CreateStepAfter()
	{
		if (animationParent != null)
		{
			animationParent.CreateStepAfterStep(this);
		}
	}

	public void DestroyStep()
	{
		if (animationParent != null)
		{
			animationParent.DestroyStep(this);
		}
		else if (containingAtom != null)
		{
			SuperController.singleton.RemoveAtom(containingAtom);
		}
	}

	public void AlignPositionToRoot()
	{
		if (animationParent != null)
		{
			base.transform.position = animationParent.transform.position;
		}
	}

	public void AlignRotationToRoot()
	{
		if (animationParent != null)
		{
			base.transform.rotation = animationParent.transform.rotation;
		}
	}

	protected void InitUI()
	{
		if (transitionToTimeSlider != null)
		{
			transitionToTimeSlider.onValueChanged.AddListener(delegate
			{
				transitionToTime = transitionToTimeSlider.value;
			});
		}
		if (curveTypePopup != null)
		{
			UIPopup uIPopup = curveTypePopup;
			uIPopup.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup.onValueChangeHandlers, new UIPopup.OnValueChange(SetCurveType));
		}
	}

	private void Awake()
	{
		InitUI();
	}
}
