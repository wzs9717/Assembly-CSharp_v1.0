using UnityEngine;

public class FreeControllerV3Link : MonoBehaviour
{
	public FreeControllerV3 linkedController;

	private void Update()
	{
		if ((bool)linkedController && linkedController.followWhenOff != null)
		{
			base.transform.position = linkedController.followWhenOff.position;
		}
	}
}
