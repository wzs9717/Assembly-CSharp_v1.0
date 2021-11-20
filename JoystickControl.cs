using UnityEngine;

public class JoystickControl : MonoBehaviour
{
	public enum JoystickType
	{
		X360,
		XBONE
	}

	public enum Axis
	{
		None,
		LeftStickX,
		LeftStickY,
		RightStickX,
		RightStickY,
		Triggers,
		DPadX,
		DPadY
	}

	public static JoystickControl singleton;

	public JoystickType joystickType;

	public bool on = true;

	public string leftStickXAxisName = "LeftStickX";

	public string leftStickYAxisName = "LeftStickY";

	public string rightStickXAxisName = "RightStickX";

	public string rightStickYAxisName = "RightStickY";

	public string triggersAxisName = "Triggers";

	public string xboneTriggerRightAxisName = "Axis6";

	public string xboneDPadXAxisName = "Axis7";

	public string xboneDPadYAxisName = "Axis8";

	public string x360DPadXAxisName = "Axis6";

	public string x360DPadYAxisName = "Axis7";

	public static float GetAxis(Axis axis)
	{
		if (singleton != null && singleton.on && (SuperController.singleton == null || SuperController.singleton.viveControllerLeft == null))
		{
			switch (axis)
			{
			case Axis.LeftStickX:
				if (OVRManager.isHmdPresent)
				{
					return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x;
				}
				return Input.GetAxis(singleton.leftStickXAxisName);
			case Axis.LeftStickY:
				if (OVRManager.isHmdPresent)
				{
					return 0f - OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y;
				}
				return Input.GetAxis(singleton.leftStickYAxisName);
			case Axis.RightStickX:
				if (OVRManager.isHmdPresent)
				{
					return OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x;
				}
				return Input.GetAxis(singleton.rightStickXAxisName);
			case Axis.RightStickY:
				if (OVRManager.isHmdPresent)
				{
					return 0f - OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y;
				}
				return Input.GetAxis(singleton.rightStickYAxisName);
			case Axis.Triggers:
				if (singleton.joystickType == JoystickType.X360)
				{
					return Input.GetAxis(singleton.triggersAxisName);
				}
				if (singleton.joystickType == JoystickType.XBONE)
				{
					float num;
					if (OVRManager.isHmdPresent)
					{
						num = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);
						float num2 = OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger);
						num -= num2;
					}
					else
					{
						num = 0f;
					}
					return num * 0.5f;
				}
				break;
			case Axis.DPadX:
				if (singleton.joystickType == JoystickType.X360)
				{
					return Input.GetAxis(singleton.x360DPadXAxisName);
				}
				if (singleton.joystickType == JoystickType.XBONE)
				{
					return Input.GetAxis(singleton.xboneDPadXAxisName);
				}
				break;
			case Axis.DPadY:
				if (singleton.joystickType == JoystickType.X360)
				{
					return Input.GetAxis(singleton.x360DPadYAxisName);
				}
				if (singleton.joystickType == JoystickType.XBONE)
				{
					return Input.GetAxis(singleton.xboneDPadYAxisName);
				}
				break;
			}
		}
		return 0f;
	}

	public static bool GetButtonDown(string buttonName)
	{
		if (singleton != null && singleton.on && (SuperController.singleton == null || SuperController.singleton.viveControllerLeft == null))
		{
			return Input.GetButtonDown(buttonName);
		}
		return false;
	}

	public static bool GetButtonUp(string buttonName)
	{
		if (singleton != null && singleton.on && (SuperController.singleton == null || SuperController.singleton.viveControllerLeft == null))
		{
			return Input.GetButtonUp(buttonName);
		}
		return false;
	}

	public static bool GetButton(string buttonName)
	{
		if (singleton != null && singleton.on && (SuperController.singleton == null || SuperController.singleton.viveControllerLeft == null))
		{
			return Input.GetButton(buttonName);
		}
		return false;
	}

	private void Awake()
	{
		singleton = this;
	}
}
