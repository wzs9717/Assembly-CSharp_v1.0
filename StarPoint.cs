using UnityEngine;

public class StarPoint
{
	public Vector3 position;

	public float noise;

	public float xRotation;

	public float yRotation;

	public StarPoint(Vector3 position, float noise, float xRotation, float yRotation)
	{
		this.position = position;
		this.noise = noise;
		this.xRotation = xRotation;
		this.yRotation = yRotation;
	}
}
