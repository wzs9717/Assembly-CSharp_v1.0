using UnityEngine;

public class AdjustJointPositionDrive : MonoBehaviour
{
	public bool on = true;

	[SerializeField]
	private float _XDriveSpring;

	[SerializeField]
	private float _XDriveDamper;

	[SerializeField]
	private float _YDriveSpring;

	[SerializeField]
	private float _YDriveDamper;

	[SerializeField]
	private float _ZDriveSpring;

	[SerializeField]
	private float _ZDriveDamper;

	private ConfigurableJoint CJ;

	public float XDriveSpring
	{
		get
		{
			return _XDriveSpring;
		}
		set
		{
			if (_XDriveSpring != value)
			{
				_XDriveSpring = value;
				Adjust();
			}
		}
	}

	public float XDriveDamper
	{
		get
		{
			return _XDriveDamper;
		}
		set
		{
			if (_XDriveDamper != value)
			{
				_XDriveDamper = value;
				Adjust();
			}
		}
	}

	public float YDriveSpring
	{
		get
		{
			return _YDriveSpring;
		}
		set
		{
			if (_YDriveSpring != value)
			{
				_YDriveSpring = value;
				Adjust();
			}
		}
	}

	public float YDriveDamper
	{
		get
		{
			return _YDriveDamper;
		}
		set
		{
			if (_YDriveDamper != value)
			{
				_YDriveDamper = value;
				Adjust();
			}
		}
	}

	public float ZDriveSpring
	{
		get
		{
			return _ZDriveSpring;
		}
		set
		{
			if (_ZDriveSpring != value)
			{
				_ZDriveSpring = value;
				Adjust();
			}
		}
	}

	public float ZDriveDamper
	{
		get
		{
			return _ZDriveDamper;
		}
		set
		{
			if (_ZDriveDamper != value)
			{
				_ZDriveDamper = value;
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
			if (CJ.xDrive.positionSpring != _XDriveSpring || CJ.xDrive.positionDamper != _XDriveDamper)
			{
				JointDrive xDrive = CJ.xDrive;
				xDrive.positionSpring = _XDriveSpring;
				xDrive.positionDamper = _XDriveDamper;
				CJ.xDrive = xDrive;
			}
			if (CJ.yDrive.positionSpring != _YDriveSpring || CJ.yDrive.positionDamper != _YDriveDamper)
			{
				JointDrive yDrive = CJ.yDrive;
				yDrive.positionSpring = _YDriveSpring;
				yDrive.positionDamper = _YDriveDamper;
				CJ.yDrive = yDrive;
			}
			if (CJ.zDrive.positionSpring != _ZDriveSpring || CJ.zDrive.positionDamper != _ZDriveDamper)
			{
				JointDrive zDrive = CJ.zDrive;
				zDrive.positionSpring = _ZDriveSpring;
				zDrive.positionDamper = _ZDriveDamper;
				CJ.zDrive = zDrive;
			}
		}
	}
}
