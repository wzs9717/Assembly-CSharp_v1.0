using UnityEngine;

public class SpinPlanet : MonoBehaviour
{
	public float speed = 4f;

	private void Update()
	{
		base.transform.Rotate(Vector3.up, speed * Time.deltaTime);
	}
}
