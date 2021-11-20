using UnityEngine;

public class UIPopupTrack : MonoBehaviour
{
	public UIPopup master;

	protected UIPopup slave;

	private void Update()
	{
		if (master != null && slave != null)
		{
			slave.currentValue = master.currentValue;
		}
	}

	private void Start()
	{
		slave = GetComponent<UIPopup>();
	}
}
