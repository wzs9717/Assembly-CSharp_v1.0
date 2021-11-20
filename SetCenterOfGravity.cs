using UnityEngine;

public class SetCenterOfGravity : MonoBehaviour
{
	public Vector3 centerOfGravity;

	public bool liveUpdate;

	private Rigidbody rb;

	private void setCOG()
	{
		rb.centerOfMass = centerOfGravity;
	}

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		if ((bool)rb)
		{
			setCOG();
		}
	}

	private void Update()
	{
		if (liveUpdate && (bool)rb && rb.centerOfMass != centerOfGravity)
		{
			setCOG();
		}
	}
}
