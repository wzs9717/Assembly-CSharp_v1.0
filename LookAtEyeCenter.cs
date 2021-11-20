using UnityEngine;

public class LookAtEyeCenter : MonoBehaviour
{
	public bool useEyeCenterUp;

	private void Update()
	{
		if (EyeCenter.eyeCenter != null)
		{
			if (useEyeCenterUp)
			{
				base.transform.LookAt(EyeCenter.eyeCenter, EyeCenter.eyeCenter.up);
			}
			else
			{
				base.transform.LookAt(EyeCenter.eyeCenter);
			}
		}
	}
}
