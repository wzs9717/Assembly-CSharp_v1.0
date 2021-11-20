using UnityEngine;
using UnityEngine.UI;

public class AllJointsController : MonoBehaviour
{
	public FreeControllerV3GUI[] freeControllerGUIObjects;

	public Slider springPercentSlider;

	public Slider damperPercentSlider;

	public void SetAllJointsControlPositionAndRotation()
	{
		if (freeControllerGUIObjects == null)
		{
			return;
		}
		FreeControllerV3GUI[] array = freeControllerGUIObjects;
		foreach (FreeControllerV3GUI freeControllerV3GUI in array)
		{
			if (freeControllerV3GUI.positionToggleGroup != null)
			{
				freeControllerV3GUI.positionToggleGroup.activeToggleName = "On";
			}
			if (freeControllerV3GUI.rotationToggleGroup != null)
			{
				freeControllerV3GUI.rotationToggleGroup.activeToggleName = "On";
			}
		}
	}

	public void SetAllJointsControlPosition()
	{
		if (freeControllerGUIObjects == null)
		{
			return;
		}
		FreeControllerV3GUI[] array = freeControllerGUIObjects;
		foreach (FreeControllerV3GUI freeControllerV3GUI in array)
		{
			if (freeControllerV3GUI.positionToggleGroup != null)
			{
				freeControllerV3GUI.positionToggleGroup.activeToggleName = "On";
			}
			if (freeControllerV3GUI.rotationToggleGroup != null)
			{
				freeControllerV3GUI.rotationToggleGroup.activeToggleName = "Off";
			}
		}
	}

	public void SetAllJointsControlOff()
	{
		if (freeControllerGUIObjects == null)
		{
			return;
		}
		FreeControllerV3GUI[] array = freeControllerGUIObjects;
		foreach (FreeControllerV3GUI freeControllerV3GUI in array)
		{
			if (freeControllerV3GUI.positionToggleGroup != null)
			{
				freeControllerV3GUI.positionToggleGroup.activeToggleName = "Off";
			}
			if (freeControllerV3GUI.rotationToggleGroup != null)
			{
				freeControllerV3GUI.rotationToggleGroup.activeToggleName = "Off";
			}
		}
	}

	public void SetAllJointsMaxHoldSpring()
	{
		if (freeControllerGUIObjects == null)
		{
			return;
		}
		FreeControllerV3GUI[] array = freeControllerGUIObjects;
		foreach (FreeControllerV3GUI freeControllerV3GUI in array)
		{
			if (freeControllerV3GUI.holdPositionSpringSlider != null)
			{
				freeControllerV3GUI.holdPositionSpringSlider.value = freeControllerV3GUI.holdPositionSpringSlider.maxValue;
			}
			if (freeControllerV3GUI.holdRotationSpringSlider != null)
			{
				freeControllerV3GUI.holdRotationSpringSlider.value = freeControllerV3GUI.holdRotationSpringSlider.maxValue;
			}
		}
	}

	public void SetAllJointsPercentHoldSpring()
	{
		if (freeControllerGUIObjects == null || !(springPercentSlider != null))
		{
			return;
		}
		float value = springPercentSlider.value;
		FreeControllerV3GUI[] array = freeControllerGUIObjects;
		foreach (FreeControllerV3GUI freeControllerV3GUI in array)
		{
			if (freeControllerV3GUI.holdPositionSpringSlider != null)
			{
				freeControllerV3GUI.holdPositionSpringSlider.value = (freeControllerV3GUI.holdPositionSpringSlider.maxValue - freeControllerV3GUI.holdPositionSpringSlider.minValue) * value + freeControllerV3GUI.holdPositionSpringSlider.minValue;
			}
			if (freeControllerV3GUI.holdRotationSpringSlider != null)
			{
				freeControllerV3GUI.holdRotationSpringSlider.value = (freeControllerV3GUI.holdRotationSpringSlider.maxValue - freeControllerV3GUI.holdRotationSpringSlider.minValue) * value + freeControllerV3GUI.holdRotationSpringSlider.minValue;
			}
		}
	}

	public void SetAllJointsMinHoldSpring()
	{
		if (freeControllerGUIObjects == null)
		{
			return;
		}
		FreeControllerV3GUI[] array = freeControllerGUIObjects;
		foreach (FreeControllerV3GUI freeControllerV3GUI in array)
		{
			if (freeControllerV3GUI.holdPositionSpringSlider != null)
			{
				freeControllerV3GUI.holdPositionSpringSlider.value = freeControllerV3GUI.holdPositionSpringSlider.minValue;
			}
			if (freeControllerV3GUI.holdRotationSpringSlider != null)
			{
				freeControllerV3GUI.holdRotationSpringSlider.value = freeControllerV3GUI.holdRotationSpringSlider.minValue;
			}
		}
	}

	public void SetAllJointsMaxHoldDamper()
	{
		if (freeControllerGUIObjects == null)
		{
			return;
		}
		FreeControllerV3GUI[] array = freeControllerGUIObjects;
		foreach (FreeControllerV3GUI freeControllerV3GUI in array)
		{
			if (freeControllerV3GUI.holdPositionDamperSlider != null)
			{
				freeControllerV3GUI.holdPositionDamperSlider.value = freeControllerV3GUI.holdPositionDamperSlider.maxValue;
			}
			if (freeControllerV3GUI.holdRotationDamperSlider != null)
			{
				freeControllerV3GUI.holdRotationDamperSlider.value = freeControllerV3GUI.holdRotationDamperSlider.maxValue;
			}
		}
	}

	public void SetAllJointsPercentHoldDamper()
	{
		if (freeControllerGUIObjects == null || !damperPercentSlider)
		{
			return;
		}
		float value = damperPercentSlider.value;
		FreeControllerV3GUI[] array = freeControllerGUIObjects;
		foreach (FreeControllerV3GUI freeControllerV3GUI in array)
		{
			if (freeControllerV3GUI.holdPositionDamperSlider != null)
			{
				freeControllerV3GUI.holdPositionDamperSlider.value = (freeControllerV3GUI.holdPositionDamperSlider.maxValue - freeControllerV3GUI.holdPositionDamperSlider.minValue) * value + freeControllerV3GUI.holdPositionDamperSlider.minValue;
			}
			if (freeControllerV3GUI.holdRotationDamperSlider != null)
			{
				freeControllerV3GUI.holdRotationDamperSlider.value = (freeControllerV3GUI.holdRotationDamperSlider.maxValue - freeControllerV3GUI.holdRotationDamperSlider.minValue) * value + freeControllerV3GUI.holdRotationDamperSlider.minValue;
			}
		}
	}

	public void SetAllJointsMinHoldDamper()
	{
		if (freeControllerGUIObjects == null)
		{
			return;
		}
		FreeControllerV3GUI[] array = freeControllerGUIObjects;
		foreach (FreeControllerV3GUI freeControllerV3GUI in array)
		{
			if (freeControllerV3GUI.holdPositionDamperSlider != null)
			{
				freeControllerV3GUI.holdPositionDamperSlider.value = freeControllerV3GUI.holdPositionDamperSlider.minValue;
			}
			if (freeControllerV3GUI.holdRotationDamperSlider != null)
			{
				freeControllerV3GUI.holdRotationDamperSlider.value = freeControllerV3GUI.holdRotationDamperSlider.minValue;
			}
		}
	}

	private void Start()
	{
	}
}
