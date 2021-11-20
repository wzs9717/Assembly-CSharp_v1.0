using UnityEngine;
using Valve.VR;

public class ViveTrackedControllers : MonoBehaviour
{
	public enum HapticAxis
	{
		X,
		Y,
		XY
	}

	public static ViveTrackedControllers singleton;

	public SteamVR_TrackedObject viveObjectLeft;

	public SteamVR_TrackedObject viveObjectRight;

	public float scrollClick = 0.25f;

	private GameObject viveObjectLeftModelGO;

	private GameObject viveObjectLeftCanvasGO;

	private GameObject viveObjectRightModelGO;

	private GameObject viveObjectRightCanvasGO;

	private float _lastLeftTriggerValue;

	private float _lastRightTriggerValue;

	private bool _leftTriggerFullClickThisFrame;

	private bool _leftTriggerFullUnclickThisFrame;

	private bool _rightTriggerFullClickThisFrame;

	private bool _rightTriggerFullUnclickThisFrame;

	private Vector2 _leftTouchDownPosition;

	private Vector2 _rightTouchDownPosition;

	private Vector2 _leftTouchPosition;

	private Vector2 _rightTouchPosition;

	private Vector2 _leftLastTouchPosition;

	private Vector2 _rightLastTouchPosition;

	private Vector2 _leftScrollPosition;

	private Vector2 _rightScrollPosition;

	public bool leftTouchedThisFrame
	{
		get
		{
			if (viveControllerLeft != null)
			{
				return viveControllerLeft.GetTouchDown(EVRButtonId.k_EButton_Axis0);
			}
			return false;
		}
	}

	public bool rightTouchedThisFrame
	{
		get
		{
			if (viveControllerRight != null)
			{
				return viveControllerRight.GetTouchDown(EVRButtonId.k_EButton_Axis0);
			}
			return false;
		}
	}

	public bool leftUntouchedThisFrame
	{
		get
		{
			if (viveControllerLeft != null)
			{
				return viveControllerLeft.GetTouchUp(EVRButtonId.k_EButton_Axis0);
			}
			return false;
		}
	}

	public bool rightUntouchedThisFrame
	{
		get
		{
			if (viveControllerRight != null)
			{
				return viveControllerRight.GetTouchUp(EVRButtonId.k_EButton_Axis0);
			}
			return false;
		}
	}

	public bool leftTouching
	{
		get
		{
			if (viveControllerLeft != null)
			{
				return viveControllerLeft.GetTouch(EVRButtonId.k_EButton_Axis0);
			}
			return false;
		}
	}

	public bool rightTouching
	{
		get
		{
			if (viveControllerRight != null)
			{
				return viveControllerRight.GetTouch(EVRButtonId.k_EButton_Axis0);
			}
			return false;
		}
	}

	public bool bothTouching
	{
		get
		{
			if (viveControllerRight != null && viveControllerLeft != null)
			{
				return viveControllerLeft.GetTouch(EVRButtonId.k_EButton_Axis0) && viveControllerRight.GetTouch(EVRButtonId.k_EButton_Axis0);
			}
			return false;
		}
	}

	public bool leftGrippedThisFrame
	{
		get
		{
			if (viveControllerLeft != null)
			{
				return viveControllerLeft.GetPressDown(EVRButtonId.k_EButton_Grip);
			}
			return false;
		}
	}

	public bool rightGrippedThisFrame
	{
		get
		{
			if (viveControllerRight != null)
			{
				return viveControllerRight.GetPressDown(EVRButtonId.k_EButton_Grip);
			}
			return false;
		}
	}

	public bool leftUngrippedThisFrame
	{
		get
		{
			if (viveControllerLeft != null)
			{
				return viveControllerLeft.GetPressUp(EVRButtonId.k_EButton_Grip);
			}
			return false;
		}
	}

	public bool rightUngrippedThisFrame
	{
		get
		{
			if (viveControllerRight != null)
			{
				return viveControllerRight.GetPressUp(EVRButtonId.k_EButton_Grip);
			}
			return false;
		}
	}

	public bool leftGripping
	{
		get
		{
			if (viveControllerLeft != null)
			{
				return viveControllerLeft.GetPress(EVRButtonId.k_EButton_Grip);
			}
			return false;
		}
	}

	public bool rightGripping
	{
		get
		{
			if (viveControllerRight != null)
			{
				return viveControllerRight.GetPress(EVRButtonId.k_EButton_Grip);
			}
			return false;
		}
	}

	public bool leftTouchpadPressedThisFrame
	{
		get
		{
			if (viveControllerLeft != null)
			{
				return viveControllerLeft.GetPressDown(EVRButtonId.k_EButton_Axis0);
			}
			return false;
		}
	}

	public bool rightTouchpadPressedThisFrame
	{
		get
		{
			if (viveControllerRight != null)
			{
				return viveControllerRight.GetPressDown(EVRButtonId.k_EButton_Axis0);
			}
			return false;
		}
	}

	public bool leftTouchpadUnpressedThisFrame
	{
		get
		{
			if (viveControllerLeft != null)
			{
				return viveControllerLeft.GetPressUp(EVRButtonId.k_EButton_Axis0);
			}
			return false;
		}
	}

	public bool rightTouchpadUnpressedThisFrame
	{
		get
		{
			if (viveControllerRight != null)
			{
				return viveControllerRight.GetPressUp(EVRButtonId.k_EButton_Axis0);
			}
			return false;
		}
	}

	public bool leftTouchpadPressing
	{
		get
		{
			if (viveControllerLeft != null)
			{
				return viveControllerLeft.GetPress(EVRButtonId.k_EButton_Axis0);
			}
			return false;
		}
	}

	public bool rightTouchpadPressing
	{
		get
		{
			if (viveControllerRight != null)
			{
				return viveControllerRight.GetPress(EVRButtonId.k_EButton_Axis0);
			}
			return false;
		}
	}

	public bool leftTriggerPressing
	{
		get
		{
			if (viveControllerLeft != null)
			{
				return viveControllerLeft.GetPress(EVRButtonId.k_EButton_Axis1);
			}
			return false;
		}
	}

	public bool rightTriggerPressing
	{
		get
		{
			if (viveControllerRight != null)
			{
				return viveControllerRight.GetPress(EVRButtonId.k_EButton_Axis1);
			}
			return false;
		}
	}

	public float leftTriggerValue
	{
		get
		{
			if (viveControllerLeft != null)
			{
				return viveControllerLeft.GetAxis(EVRButtonId.k_EButton_Axis1).x;
			}
			return 0f;
		}
	}

	public float rightTriggerValue
	{
		get
		{
			if (viveControllerRight != null)
			{
				return viveControllerRight.GetAxis(EVRButtonId.k_EButton_Axis1).x;
			}
			return 0f;
		}
	}

	public bool leftTriggerFullClickThisFrame => _leftTriggerFullClickThisFrame;

	public bool leftTriggerFullUnclickThisFrame => _leftTriggerFullUnclickThisFrame;

	public bool rightTriggerFullClickThisFrame => _rightTriggerFullClickThisFrame;

	public bool rightTriggerFullUnclickThisFrame => _rightTriggerFullUnclickThisFrame;

	public Vector2 leftTouchDownPosition => _leftTouchDownPosition;

	public Vector2 rightTouchDownPosition => _rightTouchDownPosition;

	public Vector2 leftTouchPosition => _leftTouchPosition;

	public Vector2 rightTouchPosition => _rightTouchPosition;

	private SteamVR_Controller.Device viveControllerLeft
	{
		get
		{
			if (viveObjectLeft != null)
			{
				if (viveObjectLeftModelGO == null)
				{
					SteamVR_RenderModel componentInChildren = viveObjectLeft.GetComponentInChildren<SteamVR_RenderModel>();
					if (componentInChildren != null)
					{
						viveObjectLeftModelGO = componentInChildren.gameObject;
					}
				}
				if (viveObjectLeftCanvasGO == null)
				{
					Canvas componentInChildren2 = viveObjectLeft.GetComponentInChildren<Canvas>();
					if (componentInChildren2 != null)
					{
						viveObjectLeftCanvasGO = componentInChildren2.gameObject;
					}
				}
				if (viveObjectLeft.index != SteamVR_TrackedObject.EIndex.None)
				{
					return SteamVR_Controller.Input((int)viveObjectLeft.index);
				}
				return null;
			}
			return null;
		}
	}

	private SteamVR_Controller.Device viveControllerRight
	{
		get
		{
			if (viveObjectRight != null)
			{
				if (viveObjectRightModelGO == null)
				{
					SteamVR_RenderModel componentInChildren = viveObjectRight.GetComponentInChildren<SteamVR_RenderModel>();
					if (componentInChildren != null)
					{
						viveObjectRightModelGO = componentInChildren.gameObject;
					}
				}
				if (viveObjectRightCanvasGO == null)
				{
					Canvas componentInChildren2 = viveObjectRight.GetComponentInChildren<Canvas>();
					if (componentInChildren2 != null)
					{
						viveObjectRightCanvasGO = componentInChildren2.gameObject;
					}
				}
				if (viveObjectRight.index != SteamVR_TrackedObject.EIndex.None)
				{
					return SteamVR_Controller.Input((int)viveObjectRight.index);
				}
				return null;
			}
			return null;
		}
	}

	public Vector2 GetLeftTouchDelta(bool hapticFeedback = true, HapticAxis hapticAxis = HapticAxis.X)
	{
		Vector2 result = default(Vector2);
		if (leftTouching)
		{
			result.x = _leftTouchPosition.x - _leftLastTouchPosition.x;
			result.y = _leftTouchPosition.y - _leftLastTouchPosition.y;
			if (hapticFeedback)
			{
				ushort durationMicroSec;
				switch (hapticAxis)
				{
				case HapticAxis.X:
					durationMicroSec = (ushort)(5000f * Mathf.Abs(result.x));
					break;
				case HapticAxis.Y:
					durationMicroSec = (ushort)(5000f * Mathf.Abs(result.y));
					break;
				default:
				{
					float num = Mathf.Abs(result.x) + Mathf.Abs(result.y);
					durationMicroSec = (ushort)(5000f * num);
					break;
				}
				}
				viveControllerLeft.TriggerHapticPulse(durationMicroSec);
			}
		}
		else
		{
			result.x = 0f;
			result.y = 0f;
		}
		return result;
	}

	public Vector2 GetRightTouchDelta(bool hapticFeedback = true, HapticAxis hapticAxis = HapticAxis.X)
	{
		Vector2 result = default(Vector2);
		if (rightTouching)
		{
			result.x = _rightTouchPosition.x - _rightLastTouchPosition.x;
			result.y = _rightTouchPosition.y - _rightLastTouchPosition.y;
			if (hapticFeedback)
			{
				ushort durationMicroSec;
				switch (hapticAxis)
				{
				case HapticAxis.X:
					durationMicroSec = (ushort)(5000f * Mathf.Abs(result.x));
					break;
				case HapticAxis.Y:
					durationMicroSec = (ushort)(5000f * Mathf.Abs(result.y));
					break;
				default:
				{
					float num = Mathf.Abs(result.x) + Mathf.Abs(result.y);
					durationMicroSec = (ushort)(5000f * num);
					break;
				}
				}
				viveControllerRight.TriggerHapticPulse(durationMicroSec);
			}
		}
		else
		{
			result.x = 0f;
			result.y = 0f;
		}
		return result;
	}

	private void ProcessViveControllers()
	{
		if (viveControllerLeft != null)
		{
			_leftTriggerFullClickThisFrame = false;
			_leftTriggerFullUnclickThisFrame = false;
			Vector2 axis = viveControllerLeft.GetAxis(EVRButtonId.k_EButton_Axis1);
			if (axis.x == 1f && _lastLeftTriggerValue != 1f)
			{
				_leftTriggerFullClickThisFrame = true;
			}
			if (_lastLeftTriggerValue == 1f && axis.x != 1f)
			{
				_leftTriggerFullUnclickThisFrame = true;
			}
			_lastLeftTriggerValue = axis.x;
			_leftLastTouchPosition = _leftTouchPosition;
			_leftTouchPosition = viveControllerLeft.GetAxis();
			if (leftTouchedThisFrame)
			{
				_leftLastTouchPosition = _leftTouchPosition;
				_leftTouchDownPosition = _leftTouchPosition;
				_leftScrollPosition = _leftTouchPosition;
			}
		}
		if (viveControllerRight != null)
		{
			_rightTriggerFullClickThisFrame = false;
			_rightTriggerFullUnclickThisFrame = false;
			Vector2 axis2 = viveControllerRight.GetAxis(EVRButtonId.k_EButton_Axis1);
			if (axis2.x == 1f && _lastRightTriggerValue != 1f)
			{
				_rightTriggerFullClickThisFrame = true;
			}
			if (_lastRightTriggerValue == 1f && axis2.x != 1f)
			{
				_rightTriggerFullUnclickThisFrame = true;
			}
			_lastRightTriggerValue = axis2.x;
			_rightLastTouchPosition = _rightTouchPosition;
			_rightTouchPosition = viveControllerRight.GetAxis();
			if (rightTouchedThisFrame)
			{
				_rightLastTouchPosition = _rightTouchPosition;
				_rightTouchDownPosition = _rightTouchPosition;
				_rightScrollPosition = _rightTouchPosition;
			}
		}
	}

	public int GetLeftTouchScroll(bool hapticFeedback = true)
	{
		int num = 0;
		if (viveControllerLeft != null && leftTouching)
		{
			Vector2 axis = viveControllerLeft.GetAxis();
			float num2 = axis.x - _leftScrollPosition.x;
			if (num2 > 0f && num2 > scrollClick)
			{
				num = (int)(num2 / scrollClick);
				if (hapticFeedback)
				{
					ushort durationMicroSec = (ushort)(500 * num);
					viveControllerLeft.TriggerHapticPulse(durationMicroSec);
				}
				_leftScrollPosition = axis;
			}
			else if (num2 < 0f && 0f - num2 > scrollClick)
			{
				num = (int)(num2 / scrollClick);
				if (hapticFeedback)
				{
					ushort durationMicroSec2 = (ushort)(500 * -num);
					viveControllerLeft.TriggerHapticPulse(durationMicroSec2);
				}
				_leftScrollPosition = axis;
			}
		}
		return num;
	}

	public int GetRightTouchScroll(bool hapticFeedback = true)
	{
		int num = 0;
		if (viveControllerRight != null && rightTouching)
		{
			Vector2 axis = viveControllerRight.GetAxis();
			float num2 = axis.x - _rightScrollPosition.x;
			if (num2 > 0f && num2 > scrollClick)
			{
				num = (int)(num2 / scrollClick);
				if (hapticFeedback)
				{
					ushort durationMicroSec = (ushort)(500 * num);
					viveControllerRight.TriggerHapticPulse(durationMicroSec);
				}
				_rightScrollPosition = axis;
			}
			else if (num2 < 0f && 0f - num2 > scrollClick)
			{
				num = (int)(num2 / scrollClick);
				if (hapticFeedback)
				{
					ushort durationMicroSec2 = (ushort)(500 * -num);
					viveControllerRight.TriggerHapticPulse(durationMicroSec2);
				}
				_rightScrollPosition = axis;
			}
		}
		return num;
	}

	public void HideLeftController()
	{
		if (viveControllerLeft != null)
		{
			if (viveObjectLeftModelGO != null)
			{
				viveObjectLeftModelGO.SetActive(value: false);
			}
			if (viveObjectLeftCanvasGO != null)
			{
				viveObjectLeftCanvasGO.SetActive(value: false);
			}
		}
	}

	public void ShowLeftController()
	{
		if (viveControllerLeft != null)
		{
			if (viveObjectLeftModelGO != null)
			{
				viveObjectLeftModelGO.SetActive(value: true);
			}
			if (viveObjectLeftCanvasGO != null)
			{
				viveObjectLeftCanvasGO.SetActive(value: true);
			}
		}
	}

	public void HideRightController()
	{
		if (viveControllerRight != null)
		{
			if (viveObjectRightModelGO != null)
			{
				viveObjectRightModelGO.SetActive(value: false);
			}
			if (viveObjectRightCanvasGO != null)
			{
				viveObjectRightCanvasGO.SetActive(value: false);
			}
		}
	}

	public void ShowRightController()
	{
		if (viveControllerRight != null)
		{
			if (viveObjectRightModelGO != null)
			{
				viveObjectRightModelGO.SetActive(value: true);
			}
			if (viveObjectRightCanvasGO != null)
			{
				viveObjectRightCanvasGO.SetActive(value: true);
			}
		}
	}

	private void Update()
	{
		ProcessViveControllers();
	}

	private void Awake()
	{
		singleton = this;
	}
}
