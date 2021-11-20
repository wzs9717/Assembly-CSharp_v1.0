using UnityEngine;

public class EyeHeight : MonoBehaviour
{
	public float heightOffset;

	private void Update()
	{
		if (EyeCenter.eyeCenter != null)
		{
			Vector3 position = base.transform.position;
			position.y = EyeCenter.eyeCenter.position.y + heightOffset;
			base.transform.position = position;
		}
	}
}
