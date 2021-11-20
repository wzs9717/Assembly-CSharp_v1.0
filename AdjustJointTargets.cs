using UnityEngine;

public class AdjustJointTargets : MonoBehaviour
{
	public bool on;

	[SerializeField]
	private float _percent;

	[SerializeField]
	private float _xPercent;

	[SerializeField]
	private float _yPercent;

	[SerializeField]
	private float _zPercent;

	private AdjustJointTarget[] ajts;

	public float percent
	{
		get
		{
			return _percent;
		}
		set
		{
			if (_percent == value)
			{
				return;
			}
			_percent = value;
			_xPercent = value;
			_yPercent = value;
			_zPercent = value;
			if (!on)
			{
				return;
			}
			if (ajts == null)
			{
				ajts = GetComponentsInChildren<AdjustJointTarget>(includeInactive: true);
			}
			if (ajts != null)
			{
				AdjustJointTarget[] array = ajts;
				foreach (AdjustJointTarget adjustJointTarget in array)
				{
					adjustJointTarget.percent = _percent;
				}
			}
		}
	}

	public float xPercent
	{
		get
		{
			return _xPercent;
		}
		set
		{
			if (_xPercent == value)
			{
				return;
			}
			_xPercent = value;
			if (!on)
			{
				return;
			}
			if (ajts == null)
			{
				ajts = GetComponentsInChildren<AdjustJointTarget>(includeInactive: true);
			}
			if (ajts != null)
			{
				AdjustJointTarget[] array = ajts;
				foreach (AdjustJointTarget adjustJointTarget in array)
				{
					adjustJointTarget.xPercent = _xPercent;
				}
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
			if (_yPercent == value)
			{
				return;
			}
			_yPercent = value;
			if (!on)
			{
				return;
			}
			if (ajts == null)
			{
				ajts = GetComponentsInChildren<AdjustJointTarget>(includeInactive: true);
			}
			if (ajts != null)
			{
				AdjustJointTarget[] array = ajts;
				foreach (AdjustJointTarget adjustJointTarget in array)
				{
					adjustJointTarget.yPercent = _yPercent;
				}
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
			if (_zPercent == value)
			{
				return;
			}
			_zPercent = value;
			if (!on)
			{
				return;
			}
			if (ajts == null)
			{
				ajts = GetComponentsInChildren<AdjustJointTarget>(includeInactive: true);
			}
			if (ajts != null)
			{
				AdjustJointTarget[] array = ajts;
				foreach (AdjustJointTarget adjustJointTarget in array)
				{
					adjustJointTarget.zPercent = _zPercent;
				}
			}
		}
	}

	public AdjustJointTarget[] controlledAdjustJointTargets
	{
		get
		{
			if (ajts == null)
			{
				ajts = GetComponentsInChildren<AdjustJointTarget>(includeInactive: true);
			}
			return ajts;
		}
	}

	public void ResyncTargets()
	{
		ajts = GetComponentsInChildren<AdjustJointTarget>(includeInactive: true);
		if (ajts != null)
		{
			AdjustJointTarget[] array = ajts;
			foreach (AdjustJointTarget adjustJointTarget in array)
			{
				adjustJointTarget.percent = _percent;
				adjustJointTarget.xPercent = _xPercent;
				adjustJointTarget.yPercent = _yPercent;
				adjustJointTarget.zPercent = _zPercent;
			}
		}
	}
}
