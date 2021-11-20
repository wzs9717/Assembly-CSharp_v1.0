using UnityEngine;

public class InterpolateRigidbodies : MonoBehaviour
{
	[SerializeField]
	private bool _on;

	public bool setOnStart;

	public bool on
	{
		get
		{
			return _on;
		}
		set
		{
			if (_on == value)
			{
				return;
			}
			if (value)
			{
				if (Application.isPlaying)
				{
					_on = value;
					WalkAndSetInterpolate(base.transform);
				}
				else
				{
					Debug.LogWarning("Interpolation on rigidbodies should only be turned on at runtime to prevent major joint issues");
				}
			}
			else
			{
				_on = value;
				WalkAndSetInterpolate(base.transform);
			}
		}
	}

	private void WalkAndSetInterpolate(Transform t)
	{
		Rigidbody component = t.GetComponent<Rigidbody>();
		if (component != null)
		{
			if (_on)
			{
				if (!component.isKinematic)
				{
					component.interpolation = RigidbodyInterpolation.Interpolate;
				}
			}
			else
			{
				component.interpolation = RigidbodyInterpolation.None;
			}
		}
		foreach (Transform item in t)
		{
			WalkAndSetInterpolate(item);
		}
	}

	private void Start()
	{
		if (setOnStart)
		{
			on = true;
		}
	}
}
