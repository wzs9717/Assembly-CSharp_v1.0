using System;
using UnityEngine;

public class ShaderLODControl : MonoBehaviour
{
	public enum ShaderLOD
	{
		Low = 250,
		Medium = 400,
		High = 600
	}

	[SerializeField]
	private UIPopup _shaderLODSelector;

	[SerializeField]
	private ShaderLOD _shaderLOD = ShaderLOD.High;

	public UIPopup shaderLODSelector
	{
		get
		{
			return _shaderLODSelector;
		}
		set
		{
			if (_shaderLODSelector != value)
			{
				if (_shaderLODSelector != null)
				{
					UIPopup uIPopup = _shaderLODSelector;
					uIPopup.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Remove(uIPopup.onValueChangeHandlers, new UIPopup.OnValueChange(SetShaderLOD));
				}
				_shaderLODSelector = value;
				InitSelector();
			}
		}
	}

	public ShaderLOD shaderLOD
	{
		get
		{
			return _shaderLOD;
		}
		set
		{
			if (_shaderLOD != value)
			{
				_shaderLOD = value;
				SetInternalShaderLOD();
				if (_shaderLODSelector != null)
				{
					_shaderLODSelector.currentValue = _shaderLOD.ToString();
				}
			}
		}
	}

	private void InitSelector()
	{
		UIPopup uIPopup = _shaderLODSelector;
		uIPopup.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup.onValueChangeHandlers, new UIPopup.OnValueChange(SetShaderLOD));
		_shaderLODSelector.currentValue = _shaderLOD.ToString();
	}

	private void SetInternalShaderLOD()
	{
		Shader.globalMaximumLOD = (int)_shaderLOD;
	}

	public void SetShaderLOD(string lod)
	{
		shaderLOD = (ShaderLOD)Enum.Parse(typeof(ShaderLOD), lod);
	}

	private void Start()
	{
		SetInternalShaderLOD();
		if (_shaderLODSelector != null)
		{
			InitSelector();
		}
	}
}
