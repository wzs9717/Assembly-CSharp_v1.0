using UnityEngine;

public class AdjustJointDamper : MonoBehaviour
{
	public bool on = true;

	[SerializeField]
	private float _percent;

	[SerializeField]
	private float _lowDamper;

	[SerializeField]
	private float _defaultPercent;

	[SerializeField]
	private float _highDamper;

	[SerializeField]
	private float _yzMultiplier = 1f;

	[SerializeField]
	private float _currentDamper;

	[SerializeField]
	private float _currentXDamper;

	[SerializeField]
	private float _currentYZDamper;

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
				SetDamperVarsFromPercent();
				Adjust();
			}
		}
	}

	public float lowDamper
	{
		get
		{
			return _lowDamper;
		}
		set
		{
			if (_lowDamper != value)
			{
				_lowDamper = value;
				SetDamperVarsFromPercent();
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

	public float highDamper
	{
		get
		{
			return _highDamper;
		}
		set
		{
			if (_highDamper != value)
			{
				_highDamper = value;
				SetDamperVarsFromPercent();
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
				SetDamperVarsFromPercent();
				Adjust();
			}
		}
	}

	public float currentDamper
	{
		get
		{
			return _currentDamper;
		}
		set
		{
			if (_currentDamper != value)
			{
				_currentDamper = value;
				_currentXDamper = value;
				_currentYZDamper = value * _yzMultiplier;
				Adjust();
			}
		}
	}

	public float currentXDamper
	{
		get
		{
			return _currentXDamper;
		}
		set
		{
			if (_currentXDamper != value)
			{
				_currentXDamper = value;
				Adjust();
			}
		}
	}

	public float currentYZDamper
	{
		get
		{
			return _currentYZDamper;
		}
		set
		{
			if (_currentYZDamper != value)
			{
				_currentYZDamper = value;
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

	private void SetDamperVarsFromPercent()
	{
		_currentDamper = (highDamper - lowDamper) * _percent + lowDamper;
		_currentXDamper = _currentDamper;
		_currentYZDamper = _currentDamper * _yzMultiplier;
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
			if (CJ.slerpDrive.positionDamper != _currentDamper)
			{
				JointDrive slerpDrive = CJ.slerpDrive;
				slerpDrive.positionDamper = _currentDamper;
				CJ.slerpDrive = slerpDrive;
			}
			if (CJ.angularXDrive.positionDamper != _currentXDamper)
			{
				JointDrive angularXDrive = CJ.angularXDrive;
				angularXDrive.positionDamper = _currentXDamper;
				CJ.angularXDrive = angularXDrive;
			}
			if (CJ.angularYZDrive.positionDamper != _currentYZDamper)
			{
				JointDrive angularYZDrive = CJ.angularYZDrive;
				angularYZDrive.positionDamper = _currentYZDamper;
				CJ.angularYZDrive = angularYZDrive;
			}
		}
	}
}
