using UnityEngine;

public class AdjustRotationTarget : MonoBehaviour
{
	public bool on = true;

	[SerializeField]
	protected float _xPercent;

	[SerializeField]
	protected float _yPercent;

	[SerializeField]
	protected float _zPercent;

	[SerializeField]
	protected float _percent;

	[SerializeField]
	protected Vector3 _lowTargetRotation;

	[SerializeField]
	protected Vector3 _zeroTargetRotation;

	[SerializeField]
	protected Vector3 _highTargetRotation;

	[SerializeField]
	protected Vector3 _currentTargetRotation;

	public float xPercent
	{
		get
		{
			return _xPercent;
		}
		set
		{
			if (_xPercent != value)
			{
				_xPercent = value;
				SetTargetRotationFromPercent();
				Adjust();
			}
		}
	}

	public float yPercent
	{
		get
		{
			return _yPercent;
		}
		set
		{
			if (_yPercent != value)
			{
				_yPercent = value;
				SetTargetRotationFromPercent();
				Adjust();
			}
		}
	}

	public float zPercent
	{
		get
		{
			return _zPercent;
		}
		set
		{
			if (_zPercent != value)
			{
				_zPercent = value;
				SetTargetRotationFromPercent();
				Adjust();
			}
		}
	}

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
				_xPercent = value;
				_yPercent = value;
				_zPercent = value;
				SetTargetRotationFromPercent();
				Adjust();
			}
		}
	}

	public Vector3 lowTargetRotation
	{
		get
		{
			return _lowTargetRotation;
		}
		set
		{
			if (_lowTargetRotation != value)
			{
				_lowTargetRotation = value;
				SetTargetRotationFromPercent();
				Adjust();
			}
		}
	}

	public Vector3 zeroTargetRotation
	{
		get
		{
			return _zeroTargetRotation;
		}
		set
		{
			if (_zeroTargetRotation != value)
			{
				_zeroTargetRotation = value;
				SetTargetRotationFromPercent();
				Adjust();
			}
		}
	}

	public Vector3 highTargetRotation
	{
		get
		{
			return _highTargetRotation;
		}
		set
		{
			if (_highTargetRotation != value)
			{
				_highTargetRotation = value;
				SetTargetRotationFromPercent();
				Adjust();
			}
		}
	}

	public Vector3 currentTargetRotation
	{
		get
		{
			return _currentTargetRotation;
		}
		set
		{
			if (_currentTargetRotation != value)
			{
				_currentTargetRotation = value;
				Adjust();
			}
		}
	}

	public float currentTargetRotationX
	{
		get
		{
			return _currentTargetRotation.x;
		}
		set
		{
			if (_currentTargetRotation.x != value)
			{
				Vector3 vector = _currentTargetRotation;
				vector.x = value;
				_currentTargetRotation = vector;
				Adjust();
			}
		}
	}

	public float currentTargetRotationNegativeX
	{
		get
		{
			return 0f - _currentTargetRotation.x;
		}
		set
		{
			if (_currentTargetRotation.x != 0f - value)
			{
				Vector3 vector = _currentTargetRotation;
				vector.x = 0f - value;
				_currentTargetRotation = vector;
				Adjust();
			}
		}
	}

	public float currentTargetRotationY
	{
		get
		{
			return _currentTargetRotation.y;
		}
		set
		{
			if (_currentTargetRotation.y != value)
			{
				Vector3 vector = _currentTargetRotation;
				vector.y = value;
				_currentTargetRotation = vector;
				Adjust();
			}
		}
	}

	public float currentTargetRotationNegativeY
	{
		get
		{
			return 0f - _currentTargetRotation.y;
		}
		set
		{
			if (_currentTargetRotation.y != 0f - value)
			{
				Vector3 vector = _currentTargetRotation;
				vector.y = 0f - value;
				_currentTargetRotation = vector;
				Adjust();
			}
		}
	}

	public float currentTargetRotationZ
	{
		get
		{
			return _currentTargetRotation.z;
		}
		set
		{
			if (_currentTargetRotation.z != value)
			{
				Vector3 vector = _currentTargetRotation;
				vector.z = value;
				_currentTargetRotation = vector;
				Adjust();
			}
		}
	}

	public float currentTargetRotationNegativeZ
	{
		get
		{
			return 0f - _currentTargetRotation.z;
		}
		set
		{
			if (_currentTargetRotation.z != 0f - value)
			{
				Vector3 vector = _currentTargetRotation;
				vector.z = 0f - value;
				_currentTargetRotation = vector;
				Adjust();
			}
		}
	}

	protected void SetTargetRotationFromPercent()
	{
		if (_xPercent < 0f)
		{
			_currentTargetRotation.x = Mathf.Lerp(_zeroTargetRotation.x, _lowTargetRotation.x, 0f - _xPercent);
		}
		else
		{
			_currentTargetRotation.x = Mathf.Lerp(_zeroTargetRotation.x, _highTargetRotation.x, _xPercent);
		}
		if (_yPercent < 0f)
		{
			_currentTargetRotation.y = Mathf.Lerp(_zeroTargetRotation.y, _lowTargetRotation.y, 0f - _yPercent);
		}
		else
		{
			_currentTargetRotation.y = Mathf.Lerp(_zeroTargetRotation.y, _highTargetRotation.y, _yPercent);
		}
		if (_zPercent < 0f)
		{
			_currentTargetRotation.z = Mathf.Lerp(_zeroTargetRotation.z, _lowTargetRotation.z, 0f - _zPercent);
		}
		else
		{
			_currentTargetRotation.z = Mathf.Lerp(_zeroTargetRotation.z, _highTargetRotation.z, _zPercent);
		}
	}

	public void Adjust()
	{
		if (on && Application.isPlaying)
		{
			Quaternion localRotation = Quaternion.Euler(_currentTargetRotation);
			base.transform.localRotation = localRotation;
		}
	}

	private void Start()
	{
		Adjust();
	}
}
