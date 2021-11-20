using UnityEngine;

public class AdjustJointPositionTargets : MonoBehaviour
{
	public bool on = true;

	[SerializeField]
	protected float _percent;

	protected AdjustJointPositionTarget[] ajpts;

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

	public AdjustJointPositionTarget[] controlledAdjustJointPositionTargets
	{
		get
		{
			if (ajpts == null)
			{
				ajpts = GetComponentsInChildren<AdjustJointPositionTarget>(includeInactive: true);
			}
			return ajpts;
		}
	}

	public void ResyncTargets()
	{
		ajpts = GetComponentsInChildren<AdjustJointPositionTarget>(includeInactive: true);
		Adjust();
	}

	protected virtual void Adjust()
	{
		if (!on)
		{
			return;
		}
		if (ajpts == null)
		{
			ajpts = GetComponentsInChildren<AdjustJointPositionTarget>(includeInactive: true);
		}
		if (ajpts != null)
		{
			AdjustJointPositionTarget[] array = ajpts;
			foreach (AdjustJointPositionTarget adjustJointPositionTarget in array)
			{
				adjustJointPositionTarget.percent = percent;
			}
		}
	}
}
