using UnityEngine;

public class ReportCenterOfMass : MonoBehaviour
{
	public bool report;

	private Rigidbody rb;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (report && (bool)rb)
		{
			Debug.Log(base.name + " center of mass: " + rb.centerOfMass.ToString("F3"));
		}
	}
}
