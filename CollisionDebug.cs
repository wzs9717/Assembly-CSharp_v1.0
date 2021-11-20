using UnityEngine;

public class CollisionDebug : MonoBehaviour
{
	public bool on;

	public bool isDetectingCollisions;

	private Rigidbody rb;

	private void OnCollisionEnter(Collision c)
	{
		if (on)
		{
			Debug.Log("Collision between RB " + base.transform.name + " and collider " + c.collider.name + " in RB " + c.rigidbody.name);
			ContactPoint[] contacts = c.contacts;
			foreach (ContactPoint contactPoint in contacts)
			{
				MyDebug.DrawWireCube(contactPoint.point, 0.01f, Color.red);
			}
		}
	}

	private void OnCollisionStay(Collision c)
	{
		if (on)
		{
			ContactPoint[] contacts = c.contacts;
			foreach (ContactPoint contactPoint in contacts)
			{
				MyDebug.DrawWireCube(contactPoint.point, 0.01f, Color.red);
			}
		}
	}

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		isDetectingCollisions = rb.detectCollisions;
	}
}
