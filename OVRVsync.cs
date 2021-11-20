using UnityEngine;
using UnityEngine.UI;

public class OVRVsync : MonoBehaviour
{
	public static OVRVsync singleton;

	public bool keyEnabled;

	public KeyCode vsyncToggleKey = KeyCode.V;

	public Toggle toggle;

	private bool _VSYNCOn = true;

	public bool VSYNCOn
	{
		get
		{
			return _VSYNCOn;
		}
		set
		{
			if (_VSYNCOn == value)
			{
				return;
			}
			_VSYNCOn = value;
			if (_VSYNCOn)
			{
				QualitySettings.vSyncCount = 1;
				Debug.Log("Vsync ON");
				if (toggle != null)
				{
					toggle.isOn = true;
				}
			}
			else
			{
				Debug.Log("Vsync OFF");
				QualitySettings.vSyncCount = 0;
				if (toggle != null)
				{
					toggle.isOn = false;
				}
			}
		}
	}

	public void ToggleVsync()
	{
		VSYNCOn = !VSYNCOn;
	}

	private void Update()
	{
		singleton = this;
		if (keyEnabled && Input.GetKeyDown(vsyncToggleKey))
		{
			ToggleVsync();
		}
	}
}
