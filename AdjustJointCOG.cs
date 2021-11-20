using UnityEngine;

public class AdjustJointCOG : MonoBehaviour
{
	public bool on = true;

	[SerializeField]
	private float _percent;

	public Vector3 currentCOG;

	public Vector3 lowCOG;

	public Vector3 highCOG;

	private Rigidbody RB;

	public float percent
	{
		get
		{
			return _percent;
		}
		set
		{
			_percent = value;
		}
	}

	private void Start()
	{
		RB = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (!on)
		{
			return;
		}
		currentCOG = RB.centerOfMass;
		if (RB != null)
		{
			Vector3 vector = Vector3.Lerp(lowCOG, highCOG, _percent);
			if (RB.centerOfMass != vector)
			{
				RB.centerOfMass = vector;
			}
		}
	}
}
