using UnityEngine;
using UnityEngine.EventSystems;

public class BasicLookInputModule : BaseInputModule
{
	public const int kLookId = -3;

	public string submitButtonName = "ButtonA";

	public string controlAxisName = "RightStickX";

	private PointerEventData lookData;

	private PointerEventData GetLookPointerEventData()
	{
		Vector2 position = default(Vector2);
		position.x = Screen.width / 2;
		position.y = Screen.height / 2;
		if (lookData == null)
		{
			lookData = new PointerEventData(base.eventSystem);
		}
		lookData.Reset();
		lookData.delta = Vector2.zero;
		lookData.position = position;
		lookData.scrollDelta = Vector2.zero;
		base.eventSystem.RaycastAll(lookData, m_RaycastResultCache);
		lookData.pointerCurrentRaycast = BaseInputModule.FindFirstRaycast(m_RaycastResultCache);
		m_RaycastResultCache.Clear();
		return lookData;
	}

	private bool SendUpdateEventToSelectedObject()
	{
		if (base.eventSystem.currentSelectedGameObject == null)
		{
			return false;
		}
		BaseEventData baseEventData = GetBaseEventData();
		ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.updateSelectedHandler);
		return baseEventData.used;
	}

	public override void Process()
	{
		SendUpdateEventToSelectedObject();
		PointerEventData lookPointerEventData = GetLookPointerEventData();
		HandlePointerExitAndEnter(lookPointerEventData, lookPointerEventData.pointerCurrentRaycast.gameObject);
		if (Input.GetButtonDown(submitButtonName))
		{
			base.eventSystem.SetSelectedGameObject(null);
			if (lookPointerEventData.pointerCurrentRaycast.gameObject != null)
			{
				GameObject root = lookPointerEventData.pointerCurrentRaycast.gameObject;
				GameObject gameObject = ExecuteEvents.ExecuteHierarchy(root, lookPointerEventData, ExecuteEvents.submitHandler);
				if (gameObject == null)
				{
					gameObject = ExecuteEvents.ExecuteHierarchy(root, lookPointerEventData, ExecuteEvents.selectHandler);
				}
				if (gameObject != null)
				{
					Debug.Log("Selected " + gameObject.name);
					base.eventSystem.SetSelectedGameObject(gameObject);
				}
			}
		}
		if ((bool)base.eventSystem.currentSelectedGameObject && controlAxisName != null && controlAxisName != string.Empty)
		{
			float axis = Input.GetAxis(controlAxisName);
			if (axis > 0.01f || axis < -0.01f)
			{
				AxisEventData axisEventData = GetAxisEventData(axis, 0f, 0f);
				ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
			}
		}
	}
}
