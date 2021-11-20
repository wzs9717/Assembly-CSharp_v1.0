using UnityEngine;

public class OVRPlatformMenu : MonoBehaviour
{
	public enum eHandler
	{
		ShowConfirmQuit
	}

	private enum eBackButtonAction
	{
		NONE,
		DOUBLE_TAP,
		SHORT_PRESS
	}

	public KeyCode keyCode = KeyCode.Escape;

	public eHandler shortPressHandler;

	private float doubleTapDelay = 0.25f;

	private float shortPressDelay = 0.25f;

	private float longPressDelay = 0.75f;

	private int downCount;

	private int upCount;

	private float initialDownTime = -1f;

	private eBackButtonAction ResetAndSendAction(eBackButtonAction action)
	{
		MonoBehaviour.print(string.Concat("ResetAndSendAction( ", action, " );"));
		downCount = 0;
		upCount = 0;
		initialDownTime = -1f;
		return action;
	}

	private eBackButtonAction HandleBackButtonState()
	{
		if (Input.GetKeyDown(keyCode))
		{
			downCount++;
			if (downCount == 1)
			{
				initialDownTime = Time.realtimeSinceStartup;
			}
		}
		else if (downCount > 0)
		{
			if (Input.GetKey(keyCode))
			{
				if (downCount <= upCount)
				{
					downCount++;
				}
				float num = Time.realtimeSinceStartup - initialDownTime;
				if (num > longPressDelay)
				{
					return ResetAndSendAction(eBackButtonAction.NONE);
				}
			}
			else if (initialDownTime >= 0f)
			{
				if (upCount < downCount)
				{
					upCount++;
				}
				float num2 = Time.realtimeSinceStartup - initialDownTime;
				if (num2 < doubleTapDelay)
				{
					if (downCount == 2 && upCount == 2)
					{
						return ResetAndSendAction(eBackButtonAction.DOUBLE_TAP);
					}
				}
				else if (num2 > shortPressDelay && num2 < longPressDelay)
				{
					if (downCount == 1 && upCount == 1)
					{
						return ResetAndSendAction(eBackButtonAction.SHORT_PRESS);
					}
				}
				else if (num2 > longPressDelay)
				{
					return ResetAndSendAction(eBackButtonAction.NONE);
				}
			}
		}
		return eBackButtonAction.NONE;
	}

	private void Awake()
	{
		if (!OVRManager.isHmdPresent)
		{
			base.enabled = false;
		}
	}

	private void OnApplicationFocus(bool focusState)
	{
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (!pauseStatus)
		{
			Input.ResetInputAxes();
		}
	}

	private void ShowConfirmQuitMenu()
	{
	}

	private void DoHandler(eHandler handler)
	{
		if (handler == eHandler.ShowConfirmQuit)
		{
			ShowConfirmQuitMenu();
		}
	}

	private void Update()
	{
	}
}
