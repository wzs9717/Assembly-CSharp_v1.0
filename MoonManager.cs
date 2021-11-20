using UnityEngine;

public class MoonManager : MonoBehaviour
{
	[Range(0f, 1f)]
	public float moonHeight;

	[Range(0f, 1f)]
	public float moonRotation;

	private float currentMoonHeight;

	private float currentMoonRotation;

	private void Update()
	{
	}

	public void UpdateMoonData()
	{
	}
}
