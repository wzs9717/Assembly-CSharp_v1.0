using UnityEngine;

public class EyeCenter : MonoBehaviour
{
	public static Transform eyeCenter;

	private void Start()
	{
		eyeCenter = base.transform;
	}
}
