using UnityEngine;

public class AdjustJointHardPositionTargets : MonoBehaviour
{
	public bool on = true;

	[SerializeField]
	protected float _percent;

	protected JointPositionHardLimit[] jphls;

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
				Adjust();
			}
		}
	}

	public JointPositionHardLimit[] controlledJointPositionHardLimits
	{
		get
		{
			if (jphls == null)
			{
				jphls = GetComponentsInChildren<JointPositionHardLimit>(includeInactive: true);
			}
			return jphls;
		}
	}

	public void ResyncTargets()
	{
		jphls = GetComponentsInChildren<JointPositionHardLimit>(includeInactive: true);
		Adjust();
	}

	protected virtual void Adjust()
	{
		if (!on)
		{
			return;
		}
		if (jphls == null)
		{
			jphls = GetComponentsInChildren<JointPositionHardLimit>(includeInactive: true);
		}
		if (jphls != null)
		{
			JointPositionHardLimit[] array = jphls;
			foreach (JointPositionHardLimit jointPositionHardLimit in array)
			{
				jointPositionHardLimit.percent = percent;
			}
		}
	}
}
