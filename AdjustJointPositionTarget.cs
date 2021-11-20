using UnityEngine;

public class AdjustJointPositionTarget : MonoBehaviour
{
	public bool on = true;

	[SerializeField]
	private float _percent;

	[SerializeField]
	private Vector3 _lowTargetPosition;

	[SerializeField]
	private Vector3 _zeroTargetPosition;

	[SerializeField]
	private Vector3 _highTargetPosition;

	[SerializeField]
	private Vector3 _currentTargetPosition;

	private ConfigurableJoint CJ;

	public float percent
	{
		get
		{
			return _percent;
		}
		set
		{
			if (_percent != value)
			{
				_percent = value;
				SetTargetPositionFromPercent();
				Adjust();
			}
		}
	}

	public Vector3 lowTargetPosition
	{
		get
		{
			return _lowTargetPosition;
		}
		set
		{
			if (_lowTargetPosition != value)
			{
				_lowTargetPosition = value;
				SetTargetPositionFromPercent();
				Adjust();
			}
		}
	}

	public Vector3 zeroTargetPosition
	{
		get
		{
			return _zeroTargetPosition;
		}
		set
		{
			if (_zeroTargetPosition != value)
			{
				_zeroTargetPosition = value;
				SetTargetPositionFromPercent();
				Adjust();
			}
		}
	}

	public Vector3 highTargetPosition
	{
		get
		{
			return _highTargetPosition;
		}
		set
		{
			if (_highTargetPosition != value)
			{
				_highTargetPosition = value;
				SetTargetPositionFromPercent();
				Adjust();
			}
		}
	}

	public Vector3 currentTargetPosition
	{
		get
		{
			return _currentTargetPosition;
		}
		set
		{
			if (_currentTargetPosition != value)
			{
				_currentTargetPosition = value;
				Adjust();
			}
		}
	}

	public float currentTargetPositionX
	{
		get
		{
			return _currentTargetPosition.x;
		}
		set
		{
			if (_currentTargetPosition.x != value)
			{
				Vector3 vector = _currentTargetPosition;
				vector.x = value;
				_currentTargetPosition = vector;
				Adjust();
			}
		}
	}

	public float currentTargetPositionNegativeX
	{
		get
		{
			return 0f - _currentTargetPosition.x;
		}
		set
		{
			if (_currentTargetPosition.x != 0f - value)
			{
				Vector3 vector = _currentTargetPosition;
				vector.x = 0f - value;
				_currentTargetPosition = vector;
				Adjust();
			}
		}
	}

	public float currentTargetPositionY
	{
		get
		{
			return _currentTargetPosition.y;
		}
		set
		{
			if (_currentTargetPosition.y != value)
			{
				Vector3 vector = _currentTargetPosition;
				vector.y = value;
				_currentTargetPosition = vector;
				Adjust();
			}
		}
	}

	public float currentTargetPositionNegativeY
	{
		get
		{
			return 0f - _currentTargetPosition.y;
		}
		set
		{
			if (_currentTargetPosition.y != 0f - value)
			{
				Vector3 vector = _currentTargetPosition;
				vector.y = 0f - value;
				_currentTargetPosition = vector;
				Adjust();
			}
		}
	}

	public float currentTargetPositionZ
	{
		get
		{
			return _currentTargetPosition.z;
		}
		set
		{
			if (_currentTargetPosition.z != value)
			{
				Vector3 vector = _currentTargetPosition;
				vector.z = value;
				_currentTargetPosition = vector;
				Adjust();
			}
		}
	}

	public float currentTargetPositionNegativeZ
	{
		get
		{
			return 0f - _currentTargetPosition.z;
		}
		set
		{
			if (_currentTargetPosition.z != 0f - value)
			{
				Vector3 vector = _currentTargetPosition;
				vector.z = 0f - value;
				_currentTargetPosition = vector;
				Adjust();
			}
		}
	}

	public ConfigurableJoint controlledJoint
	{
		get
		{
			if (CJ == null)
			{
				CJ = GetComponent<ConfigurableJoint>();
			}
			return CJ;
		}
	}

	private void SetTargetPositionFromPercent()
	{
		if (_percent < 0f)
		{
			_currentTargetPosition = Vector3.Lerp(_zeroTargetPosition, _lowTargetPosition, 0f - _percent);
		}
		else
		{
			_currentTargetPosition = Vector3.Lerp(_zeroTargetPosition, _highTargetPosition, _percent);
		}
	}

	private void Adjust()
	{
		if (on)
		{
			if (CJ == null)
			{
				CJ = GetComponent<ConfigurableJoint>();
			}
			if (CJ != null)
			{
				CJ.targetPosition = _currentTargetPosition;
			}
		}
	}
}
