using UnityEngine;

public class DebugHUD : MonoBehaviour
{
	public static DebugHUD singleton;

	public Transform DebugText;

	public float alertDisplayTime = 0.1f;

	public Transform alert1Object;

	public Transform alert2Object;

	public Transform alert3Object;

	protected float alert1Timer;

	protected float alert2Timer;

	protected float alert3Timer;

	private static HUDText DebugT;

	public static void Msg(string msg)
	{
		if (DebugT != null)
		{
			DebugT.AppendTextLine(msg);
			DebugT.Splash();
		}
	}

	public static void Alert1()
	{
		if (singleton != null)
		{
			singleton.alert1Timer = singleton.alertDisplayTime;
			if (singleton.alert1Object != null)
			{
				singleton.alert1Object.gameObject.SetActive(value: true);
			}
		}
	}

	public static void Alert2()
	{
		if (singleton != null)
		{
			singleton.alert2Timer = singleton.alertDisplayTime;
			if (singleton.alert2Object != null)
			{
				singleton.alert2Object.gameObject.SetActive(value: true);
			}
		}
	}

	public static void Alert3()
	{
		if (singleton != null)
		{
			singleton.alert3Timer = singleton.alertDisplayTime;
			if (singleton.alert3Object != null)
			{
				singleton.alert3Object.gameObject.SetActive(value: true);
			}
		}
	}

	private void Start()
	{
		singleton = this;
		if (DebugText != null)
		{
			DebugT = DebugText.GetComponent<HUDText>();
		}
	}

	private void Update()
	{
		if (alert1Timer >= 0f)
		{
			alert1Timer -= Time.deltaTime;
		}
		else if (alert1Timer < 0f && alert1Object != null && alert1Object.gameObject.activeSelf)
		{
			alert1Object.gameObject.SetActive(value: false);
		}
		if (alert2Timer >= 0f)
		{
			alert2Timer -= Time.deltaTime;
		}
		else if (alert2Timer < 0f && alert2Object != null && alert2Object.gameObject.activeSelf)
		{
			alert2Object.gameObject.SetActive(value: false);
		}
		if (alert3Timer >= 0f)
		{
			alert3Timer -= Time.deltaTime;
		}
		else if (alert3Timer < 0f && alert3Object != null && alert3Object.gameObject.activeSelf)
		{
			alert3Object.gameObject.SetActive(value: false);
		}
	}
}
