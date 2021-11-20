using System;
using UnityEngine;

public class Oscillate : MonoBehaviour
{
	public Vector3 amplitude;

	public Vector3 speed;

	public Vector3 clamp;

	public Vector3 basePosition;

	private void Start()
	{
		basePosition = base.transform.position;
	}

	private void LateUpdate()
	{
		Vector3 vector = Time.time * speed * 2f * (float)Math.PI;
		Vector3 vector2 = new Vector3(amplitude.x * Mathf.Sin(vector.x), amplitude.y * Mathf.Sin(vector.y), amplitude.z * Mathf.Sin(vector.z));
		vector2.x = Mathf.Clamp(vector2.x, 0f - clamp.x, clamp.x);
		vector2.y = Mathf.Clamp(vector2.y, 0f - clamp.y, clamp.y);
		vector2.z = Mathf.Clamp(vector2.z, 0f - clamp.z, clamp.z);
		base.transform.position = basePosition + vector2;
	}
}
