using UnityEngine;

public class EyesControl : MonoBehaviour
{
	public enum LookMode
	{
		Fixed,
		Closest,
		RandomAll,
		RandomList
	}

	[SerializeField]
	protected LookMode _currentLookMode;

	public LookAtWithLimits leftLookAtWithLimits;

	public LookAtWithLimits rightLookAtWithLimits;

	[SerializeField]
	protected LookAtTarget _lookAt;

	public LookMode currentLookMode
	{
		get
		{
			return _currentLookMode;
		}
		set
		{
			if (_currentLookMode != value)
			{
				_currentLookMode = value;
			}
		}
	}

	public LookAtTarget lookAt
	{
		get
		{
			return _lookAt;
		}
		set
		{
			if (!(_lookAt != value))
			{
				return;
			}
			_lookAt = value;
			if (leftLookAtWithLimits != null)
			{
				if (_lookAt == null)
				{
					leftLookAtWithLimits.target = null;
				}
				else
				{
					leftLookAtWithLimits.target = _lookAt.transform;
				}
			}
			if (rightLookAtWithLimits != null)
			{
				if (_lookAt == null)
				{
					rightLookAtWithLimits.target = null;
				}
				else
				{
					rightLookAtWithLimits.target = _lookAt.transform;
				}
			}
		}
	}

	private void Update()
	{
	}
}
