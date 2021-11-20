using UnityEngine;

public class ShaderSoftShadowControl : MonoBehaviour
{
	[SerializeField]
	protected float _shadowFilterLevel = 9f;

	[SerializeField]
	protected float _shadowPointKernel = 0.15f;

	[SerializeField]
	protected float _shadowPointBiasBase = 0.01f;

	[SerializeField]
	protected float _shadowPointBiasScale = 0.01f;

	public float shadowFilterLevel
	{
		get
		{
			return _shadowFilterLevel;
		}
		set
		{
			if (_shadowFilterLevel != value)
			{
				_shadowFilterLevel = value;
				int value2 = (int)_shadowFilterLevel;
				Shader.SetGlobalInt("_ShadowFilterLevel", value2);
			}
		}
	}

	public float shadowPointKernel
	{
		get
		{
			return _shadowPointKernel;
		}
		set
		{
			if (_shadowPointKernel != value)
			{
				_shadowPointKernel = value;
				Shader.SetGlobalFloat("_ShadowPointKernel", _shadowPointKernel);
			}
		}
	}

	public float shadowPointBiasBase
	{
		get
		{
			return _shadowPointBiasBase;
		}
		set
		{
			if (_shadowPointBiasBase != value)
			{
				_shadowPointBiasBase = value;
				Shader.SetGlobalFloat("_ShadowPointBiasBase", _shadowPointBiasBase);
			}
		}
	}

	public float shadowPointBiasScale
	{
		get
		{
			return _shadowPointBiasScale;
		}
		set
		{
			if (_shadowPointBiasScale != value)
			{
				_shadowPointBiasScale = value;
				Shader.SetGlobalFloat("_ShadowPointBiasScale", _shadowPointBiasScale);
			}
		}
	}
}
