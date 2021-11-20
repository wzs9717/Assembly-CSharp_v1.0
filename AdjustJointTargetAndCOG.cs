using UnityEngine;

public class AdjustJointTargetAndCOG : MonoBehaviour
{
	public bool on = true;

	[SerializeField]
	private float _percent;

	public Vector3 currentCOG;

	public Vector3 lowCOG;

	public Vector3 highCOG;

	public Quaternion lowTargetRotation;

	public Quaternion highTargetRotation;

	private ConfigurableJoint CJ;

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

	public void SetPercentFromUISlider()
	{
	}

	private void Start()
	{
		CJ = GetComponent<ConfigurableJoint>();
		RB = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (!on)
		{
			return;
		}
		currentCOG = RB.centerOfMass;
		if (CJ != null)
		{
			Quaternion targetRotation = Quaternion.Lerp(lowTargetRotation, highTargetRotation, _percent);
			CJ.targetRotation = targetRotation;
		}
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
