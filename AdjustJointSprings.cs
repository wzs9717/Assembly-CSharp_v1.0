using UnityEngine;

public class AdjustJointSprings : MonoBehaviour
{
	public bool on;

	[SerializeField]
	private float _percent;

	[SerializeField]
	private float _defaultPercent;

	private AdjustJointSpring[] ajss;

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

	public AdjustJointSpring[] controlledAdjustJointSprings
	{
		get
		{
			if (ajss == null)
			{
				ajss = GetComponentsInChildren<AdjustJointSpring>(includeInactive: true);
			}
			return ajss;
		}
	}

	public void SetDefaultPercent()
	{
		percent = _defaultPercent;
	}

	public void ResyncSprings()
	{
		ajss = GetComponentsInChildren<AdjustJointSpring>(includeInactive: true);
		Adjust();
	}

	private void Adjust()
	{
		if (!on)
		{
			return;
		}
		if (ajss == null)
		{
			ajss = GetComponentsInChildren<AdjustJointSpring>(includeInactive: true);
		}
		if (ajss != null)
		{
			AdjustJointSpring[] array = ajss;
			foreach (AdjustJointSpring adjustJointSpring in array)
			{
				adjustJointSpring.percent = percent;
			}
		}
	}
}
