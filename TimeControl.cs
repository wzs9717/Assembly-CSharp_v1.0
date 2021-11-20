using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class TimeControl : JSONStorable
{
	public enum State
	{
		Pause,
		Slow,
		Normal,
		Fast,
		Custom
	}

	public static TimeControl singleton;

	[SerializeField]
	private State _currentState = State.Normal;

	public bool compensateFixedTimestep;

	public float pauseScale;

	public float slowScale = 0.2f;

	public float normalScale = 1f;

	public float fastScale = 2f;

	public float lowScale;

	public float highScale = 2f;

	public Slider currentScaleSlider;

	public Slider currentScaleSliderAlt;

	[SerializeField]
	private float _currentScale = 1f;

	public State currentState
	{
		get
		{
			return _currentState;
		}
		set
		{
			_currentState = value;
			switch (_currentState)
			{
			case State.Fast:
				_currentScale = fastScale;
				break;
			case State.Normal:
				_currentScale = normalScale;
				break;
			case State.Pause:
				_currentScale = pauseScale;
				break;
			case State.Slow:
				_currentScale = slowScale;
				break;
			}
			SetScale();
		}
	}

	public float currentScale
	{
		get
		{
			return _currentScale;
		}
		set
		{
			_currentScale = value;
			if (_currentScale > highScale)
			{
				_currentScale = highScale;
			}
			else if (_currentScale < lowScale)
			{
				_currentScale = lowScale;
			}
			if (currentScaleSlider != null)
			{
				currentScaleSlider.value = _currentScale;
			}
			if (currentScaleSliderAlt != null)
			{
				currentScaleSliderAlt.value = _currentScale;
			}
			_currentState = State.Custom;
			SetScale();
		}
	}

	public bool SetSlow
	{
		get
		{
			return _currentState == State.Slow;
		}
		set
		{
			if (value)
			{
				currentState = State.Slow;
			}
			else
			{
				currentState = State.Normal;
			}
		}
	}

	public override JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true)
	{
		JSONClass jSON = base.GetJSON(includePhysical, includeAppearance);
		if (includePhysical && currentScaleSlider != null)
		{
			SliderControl component = currentScaleSlider.GetComponent<SliderControl>();
			if (component == null || component.defaultValue != currentScale)
			{
				needsStore = true;
				jSON["currentScale"].AsFloat = currentScale;
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
		if (jc["currentScale"] != null)
		{
			currentScale = jc["currentScale"].AsFloat;
		}
		else if (currentScaleSlider != null)
		{
			SliderControl component = currentScaleSlider.GetComponent<SliderControl>();
			if (component != null)
			{
				currentScale = component.defaultValue;
			}
		}
	}

	private void SetScale()
	{
		if (compensateFixedTimestep)
		{
			float num = _currentScale / Time.timeScale;
			Time.fixedDeltaTime *= num;
		}
		Time.timeScale = _currentScale;
	}

	private void InitUI()
	{
		if (currentScaleSlider != null)
		{
			currentScaleSlider.value = _currentScale;
			currentScaleSlider.onValueChanged.AddListener(delegate
			{
				currentScale = currentScaleSlider.value;
			});
			SliderControl component = currentScaleSlider.GetComponent<SliderControl>();
			if (component != null)
			{
				component.defaultValue = _currentScale;
			}
		}
		if (currentScaleSliderAlt != null)
		{
			currentScaleSliderAlt.value = _currentScale;
			currentScaleSliderAlt.onValueChanged.AddListener(delegate
			{
				currentScale = currentScaleSliderAlt.value;
			});
			SliderControl component2 = currentScaleSliderAlt.GetComponent<SliderControl>();
			if (component2 != null)
			{
				component2.defaultValue = _currentScale;
			}
		}
	}

	private void Awake()
	{
		singleton = this;
	}

	private void Start()
	{
		InitUI();
	}
}
