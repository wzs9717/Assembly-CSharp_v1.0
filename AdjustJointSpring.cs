using UnityEngine;

public class AdjustJointSpring : MonoBehaviour
{
	public bool on = true;

	[SerializeField]
	private float _percent;

	[SerializeField]
	private float _lowSpring;

	[SerializeField]
	private float _defaultPercent;

	[SerializeField]
	private float _highSpring;

	[SerializeField]
	private float _yzMultiplier = 1f;

	[SerializeField]
	private float _currentXSpring;

	[SerializeField]
	private float _currentYZSpring;

	[SerializeField]
	private float _currentSpring;

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
				SetSpringVarsFromPercent();
				Adjust();
			}
		}
	}

	public float lowSpring
	{
		get
		{
			return _lowSpring;
		}
		set
		{
			if (_lowSpring != value)
			{
				_lowSpring = value;
				SetSpringVarsFromPercent();
				Adjust();
			}
		}
	}

	public float defaultPercent
	{
		get
		{
			return _defaultPercent;
		}
		set
		{
			if (_defaultPercent != value)
			{
				_defaultPercent = value;
			}
		}
	}

	public float highSpring
	{
		get
		{
			return _highSpring;
		}
		set
		{
			if (_highSpring != value)
			{
				_highSpring = value;
				SetSpringVarsFromPercent();
				Adjust();
			}
		}
	}

	public float yzMultiplier
	{
		get
		{
			return _yzMultiplier;
		}
		set
		{
			if (_yzMultiplier != value)
			{
				_yzMultiplier = value;
				SetSpringVarsFromPercent();
				Adjust();
			}
		}
	}

	public float currentXSpring
	{
		get
		{
			return _currentXSpring;
		}
		set
		{
			if (_currentXSpring != value)
			{
				_currentXSpring = value;
				Adjust();
			}
		}
	}

	public float currentYZSpring
	{
		get
		{
			return _currentYZSpring;
		}
		set
		{
			if (_currentYZSpring != value)
			{
				_currentYZSpring = value;
				Adjust();
			}
		}
	}

	public float currentSpring
	{
		get
		{
			return _currentSpring;
		}
		set
		{
			if (_currentSpring != value)
			{
				_currentSpring = value;
				_currentXSpring = value;
				_currentYZSpring = value * _yzMultiplier;
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

	private void SetSpringVarsFromPercent()
	{
		_currentSpring = (highSpring - lowSpring) * _percent + lowSpring;
		_currentXSpring = _currentSpring;
		_currentYZSpring = _currentSpring * _yzMultiplier;
	}

	public void SetDefaultPercent()
	{
		percent = _defaultPercent;
	}

	private void Adjust()
	{
		if (!on)
		{
			return;
		}
		if (CJ == null)
		{
			CJ = GetComponent<ConfigurableJoint>();
		}
		if (CJ != null)
		{
			if (CJ.slerpDrive.positionSpring != _currentSpring)
			{
				JointDrive slerpDrive = CJ.slerpDrive;
				slerpDrive.positionSpring = _currentSpring;
				CJ.slerpDrive = slerpDrive;
			}
			if (CJ.angularXDrive.positionSpring != _currentXSpring)
			{
				JointDrive angularXDrive = CJ.angularXDrive;
				angularXDrive.positionSpring = _currentXSpring;
				CJ.angularXDrive = angularXDrive;
			}
			if (CJ.angularYZDrive.positionSpring != _currentYZSpring)
			{
				JointDrive angularYZDrive = CJ.angularYZDrive;
				angularYZDrive.positionSpring = _currentYZSpring;
				CJ.angularYZDrive = angularYZDrive;
			}
			Rigidbody component = GetComponent<Rigidbody>();
			component.WakeUp();
		}
	}
}
