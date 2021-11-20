using System;
using UnityEngine;

public class QualityControl : MonoBehaviour
{
	[SerializeField]
	private UIPopup _msaaSelector;

	[SerializeField]
	private int _msaaLevel = 8;

	[SerializeField]
	private UIPopup _pixelLightCountSelector;

	[SerializeField]
	private int _pixelLightCount = 4;

	public UIPopup msaaSelector
	{
		get
		{
			return _msaaSelector;
		}
		set
		{
			if (_msaaSelector != value)
			{
				if (_msaaSelector != null)
				{
					UIPopup uIPopup = _msaaSelector;
					uIPopup.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Remove(uIPopup.onValueChangeHandlers, new UIPopup.OnValueChange(SetMsaaFromString));
				}
				_msaaSelector = value;
				InitMsaaSelector();
			}
		}
	}

	public int msaaLevel
	{
		get
		{
			return _msaaLevel;
		}
		set
		{
			if (_msaaLevel != value && (value == 0 || value == 2 || value == 4 || value == 8))
			{
				_msaaLevel = value;
				SetMsaaLevel();
				SetMsaaSelectorCurrentValue();
			}
		}
	}

	public UIPopup pixelLightCountSelector
	{
		get
		{
			return _pixelLightCountSelector;
		}
		set
		{
			if (_pixelLightCountSelector != value)
			{
				if (_pixelLightCountSelector != null)
				{
					UIPopup uIPopup = _pixelLightCountSelector;
					uIPopup.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Remove(uIPopup.onValueChangeHandlers, new UIPopup.OnValueChange(SetPixelLightCountFromString));
				}
				_pixelLightCountSelector = value;
				InitPixelLightCountSelector();
			}
		}
	}

	public int pixelLightCount
	{
		get
		{
			return _pixelLightCount;
		}
		set
		{
			if (_pixelLightCount != value)
			{
				_pixelLightCount = value;
				SetPixelLightCount();
				SetPixelLightCountSelectorCurrentValue();
			}
		}
	}

	private void SetMsaaSelectorCurrentValue()
	{
		if (_msaaSelector != null)
		{
			switch (_msaaLevel)
			{
			case 0:
				_msaaSelector.currentValue = "Off";
				break;
			case 2:
				_msaaSelector.currentValue = "2X";
				break;
			case 4:
				_msaaSelector.currentValue = "4X";
				break;
			case 8:
				_msaaSelector.currentValue = "8X";
				break;
			}
		}
	}

	private void InitMsaaSelector()
	{
		if (_msaaSelector != null)
		{
			UIPopup uIPopup = _msaaSelector;
			uIPopup.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup.onValueChangeHandlers, new UIPopup.OnValueChange(SetMsaaFromString));
			SetMsaaSelectorCurrentValue();
		}
	}

	private void SetMsaaLevel()
	{
		if (_msaaLevel == 0 || _msaaLevel == 2 || _msaaLevel == 4 || _msaaLevel == 8)
		{
			QualitySettings.antiAliasing = _msaaLevel;
		}
	}

	public void SetMsaaFromString(string levelString)
	{
		switch (levelString)
		{
		case "Off":
			msaaLevel = 0;
			break;
		case "2X":
			msaaLevel = 2;
			break;
		case "4X":
			msaaLevel = 4;
			break;
		case "8X":
			msaaLevel = 8;
			break;
		}
	}

	private void SetPixelLightCountSelectorCurrentValue()
	{
		if (_pixelLightCountSelector != null)
		{
			_pixelLightCountSelector.currentValue = _pixelLightCount.ToString();
		}
	}

	private void InitPixelLightCountSelector()
	{
		if (_pixelLightCountSelector != null)
		{
			UIPopup uIPopup = _pixelLightCountSelector;
			uIPopup.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup.onValueChangeHandlers, new UIPopup.OnValueChange(SetPixelLightCountFromString));
			SetPixelLightCountSelectorCurrentValue();
		}
	}

	private void SetPixelLightCount()
	{
		QualitySettings.pixelLightCount = _pixelLightCount;
	}

	public void SetPixelLightCountFromString(string lightCount)
	{
		pixelLightCount = int.Parse(lightCount);
	}

	private void Start()
	{
		SetMsaaLevel();
		if (_msaaSelector != null)
		{
			InitMsaaSelector();
		}
		SetPixelLightCount();
		if (_pixelLightCountSelector != null)
		{
			InitPixelLightCountSelector();
		}
	}
}
