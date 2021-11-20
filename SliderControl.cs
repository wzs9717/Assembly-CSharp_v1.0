using UnityEngine;
using UnityEngine.UI;

public class SliderControl : MonoBehaviour
{
	public Slider slider;

	public float defaultValue;

	public bool disableLookDrag;

	public void setSliderToMinimum()
	{
		if (slider != null)
		{
			slider.value = slider.minValue;
		}
	}

	public void setSliderToMaximum()
	{
		if (slider != null)
		{
			slider.value = slider.maxValue;
		}
	}

	public void setSliderToValue(float value)
	{
		if (slider != null)
		{
			slider.value = value;
		}
	}

	public void setSliderToDefaultValue()
	{
		if (slider != null)
		{
			slider.value = defaultValue;
		}
	}
}
