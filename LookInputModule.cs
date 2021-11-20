using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.VR;

public class LookInputModule : StandaloneInputModule
{
	public enum Mode
	{
		Pointer,
		Submit
	}

	private static LookInputModule _singleton;

	public bool disableStandaloneProcess;

	public string submitButtonName = "Fire1";

	public bool useEitherControllerButtons;

	public JoystickControl.Axis controlAxis = JoystickControl.Axis.Triggers;

	public JoystickControl.Axis discreteControlAxis = JoystickControl.Axis.DPadY;

	public bool invertDiscreteControlAxis = true;

	public bool useSmoothAxis = true;

	public float smoothAxisMultiplier = 0.01f;

	public float steppedAxisStepsPerSecond = 10f;

	private bool _guiRaycastHit;

	private bool _controlAxisUsed;

	private bool _buttonUsed;

	public Mode mode;

	public bool useLookDrag = true;

	public bool useLookDragSlider = true;

	public bool useLookDragScrollbar;

	public bool useCursor = true;

	public float normalCursorScale = 0.0005f;

	public bool scaleCursorWithDistance = true;

	public RectTransform cursor;

	public RectTransform cursorRight;

	public bool useSelectColor = true;

	public bool useSelectColorOnButton;

	public bool useSelectColorOnToggle;

	public Color selectColor = Color.blue;

	public bool ignoreInputsWhenLookAway = true;

	public bool deselectWhenLookAway;

	public RectTransform anchorForControlCopy;

	protected GameObject controlCopy;

	public Camera referenceCamera;

	private PointerEventData lookData;

	private Color currentSelectedNormalColor;

	private bool currentSelectedNormalColorValid;

	private Color currentSelectedHighlightedColor;

	private GameObject currentLook;

	private GameObject currentLookRight;

	private GameObject currentPressed;

	private GameObject currentPressedRight;

	private GameObject currentDragging;

	private GameObject currentDraggingRight;

	private float nextAxisActionTime;

	protected float axisAccumulation;

	protected bool axisOn;

	protected bool discreteAxisOn;

	public static LookInputModule singleton => _singleton;

	public bool guiRaycastHit => _guiRaycastHit;

	public bool controlAxisUsed => _controlAxisUsed;

	public bool buttonUsed => _buttonUsed;

	private PointerEventData GetLookPointerEventData()
	{
		Vector2 position = default(Vector2);
		if (referenceCamera != null)
		{
			position.x = referenceCamera.pixelWidth / 2;
			position.y = referenceCamera.pixelHeight / 2;
		}
		else
		{
			position.x = VRSettings.get_eyeTextureWidth() / 2;
			position.y = VRSettings.get_eyeTextureHeight() / 2;
		}
		if (lookData == null)
		{
			lookData = new PointerEventData(base.eventSystem);
		}
		lookData.Reset();
		lookData.delta = Vector2.zero;
		lookData.scrollDelta = Vector2.zero;
		lookData.position = position;
		m_RaycastResultCache.Clear();
		base.eventSystem.RaycastAll(lookData, m_RaycastResultCache);
		lookData.pointerCurrentRaycast = BaseInputModule.FindFirstRaycast(m_RaycastResultCache);
		if (lookData.pointerCurrentRaycast.gameObject != null)
		{
			_guiRaycastHit = true;
		}
		else
		{
			_guiRaycastHit = false;
		}
		m_RaycastResultCache.Clear();
		return lookData;
	}

	private void UpdateCursor(PointerEventData lookData, RectTransform curs)
	{
		if (!(curs != null))
		{
			return;
		}
		if (useCursor)
		{
			if (lookData.pointerEnter != null)
			{
				RectTransform component = lookData.pointerEnter.GetComponent<RectTransform>();
				if (RectTransformUtility.ScreenPointToWorldPointInRectangle(component, lookData.position, lookData.enterEventCamera, out var worldPoint))
				{
					curs.gameObject.SetActive(value: true);
					curs.position = worldPoint;
					curs.rotation = component.rotation;
					if (scaleCursorWithDistance)
					{
						float magnitude = (worldPoint - lookData.enterEventCamera.transform.position).magnitude;
						float num = magnitude * normalCursorScale;
						if (num < normalCursorScale)
						{
							num = normalCursorScale;
						}
						Vector3 localScale = default(Vector3);
						localScale.x = num;
						localScale.y = num;
						localScale.z = num;
						curs.localScale = localScale;
					}
				}
				else
				{
					curs.gameObject.SetActive(value: false);
				}
			}
			else
			{
				curs.gameObject.SetActive(value: false);
			}
		}
		else
		{
			curs.gameObject.SetActive(value: false);
		}
	}

	private void SetSelectedColor(GameObject go)
	{
		if (!useSelectColor)
		{
			return;
		}
		if (!useSelectColorOnButton && (bool)go.GetComponent<Button>())
		{
			currentSelectedNormalColorValid = false;
			return;
		}
		if (!useSelectColorOnToggle && (bool)go.GetComponent<Toggle>())
		{
			currentSelectedNormalColorValid = false;
			return;
		}
		Selectable component = go.GetComponent<Selectable>();
		if (component != null)
		{
			ColorBlock colors = component.colors;
			currentSelectedNormalColor = colors.normalColor;
			currentSelectedNormalColorValid = true;
			currentSelectedHighlightedColor = colors.highlightedColor;
			colors.normalColor = selectColor;
			colors.highlightedColor = selectColor;
			component.colors = colors;
		}
	}

	private void RestoreColor(GameObject go)
	{
		if (useSelectColor && currentSelectedNormalColorValid)
		{
			Selectable component = go.GetComponent<Selectable>();
			if (component != null)
			{
				ColorBlock colors = component.colors;
				colors.normalColor = currentSelectedNormalColor;
				colors.highlightedColor = currentSelectedHighlightedColor;
				component.colors = colors;
			}
		}
	}

	public new void ClearSelection()
	{
		if ((bool)base.eventSystem.currentSelectedGameObject)
		{
			if (controlCopy != null)
			{
				Object.DestroyImmediate(controlCopy);
				controlCopy = null;
			}
			RestoreColor(base.eventSystem.currentSelectedGameObject);
			base.eventSystem.SetSelectedGameObject(null);
		}
	}

	private void Select(GameObject go)
	{
		ClearSelection();
		if (!ExecuteEvents.GetEventHandler<ISelectHandler>(go))
		{
			return;
		}
		if (anchorForControlCopy != null)
		{
			Slider component = go.GetComponent<Slider>();
			UIPopup component2 = go.GetComponent<UIPopup>();
			Transform transform = null;
			if (component != null)
			{
				transform = component.transform.parent;
			}
			else if (component2 != null)
			{
				transform = component2.transform;
			}
			if (transform != null)
			{
				RectTransform component3 = transform.GetComponent<RectTransform>();
				Rect rect = component3.rect;
				Vector2 sizeDelta = default(Vector2);
				sizeDelta.x = rect.width;
				sizeDelta.y = rect.height;
				anchorForControlCopy.sizeDelta = sizeDelta;
				controlCopy = Object.Instantiate(transform.gameObject);
				controlCopy.transform.SetParent(anchorForControlCopy);
				controlCopy.transform.localRotation = Quaternion.identity;
				controlCopy.transform.localPosition = Vector3.zero;
				controlCopy.transform.localScale = Vector3.one;
				RectTransform component4 = controlCopy.GetComponent<RectTransform>();
				component4.offsetMin = component3.offsetMin;
				component4.offsetMax = component3.offsetMax;
				component4.anchoredPosition = Vector3.zero;
				if (component != null)
				{
					Slider componentInChildren = controlCopy.GetComponentInChildren<Slider>();
					if (componentInChildren != null)
					{
						SliderTrack sliderTrack = componentInChildren.gameObject.AddComponent<SliderTrack>();
						sliderTrack.master = component;
					}
				}
				else if (component2 != null)
				{
					UIPopup component5 = controlCopy.GetComponent<UIPopup>();
					if (component5 != null)
					{
						UIPopupTrack uIPopupTrack = component5.gameObject.AddComponent<UIPopupTrack>();
						uIPopupTrack.master = component2;
					}
				}
			}
		}
		SetSelectedColor(go);
		base.eventSystem.SetSelectedGameObject(go);
	}

	public static void SelectGameObject(GameObject go)
	{
		if (_singleton != null)
		{
			_singleton.Select(go);
		}
	}

	private new bool SendUpdateEventToSelectedObject()
	{
		if (base.eventSystem.currentSelectedGameObject == null)
		{
			return false;
		}
		BaseEventData baseEventData = GetBaseEventData();
		ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.updateSelectedHandler);
		return baseEventData.used;
	}

	public bool AxisControllerSelected()
	{
		if (base.eventSystem.currentSelectedGameObject == null)
		{
			return false;
		}
		Slider component = base.eventSystem.currentSelectedGameObject.GetComponent<Slider>();
		if (component != null)
		{
			return true;
		}
		Scrollbar component2 = base.eventSystem.currentSelectedGameObject.GetComponent<Scrollbar>();
		if (component2 != null)
		{
			return true;
		}
		UIPopupButton component3 = base.eventSystem.currentSelectedGameObject.GetComponent<UIPopupButton>();
		UIPopup uIPopup = ((!(component3 != null)) ? base.eventSystem.currentSelectedGameObject.GetComponent<UIPopup>() : component3.popupParent);
		if (uIPopup != null)
		{
			return true;
		}
		return false;
	}

	protected void HandleAxis(float newVal)
	{
		if (base.eventSystem.currentSelectedGameObject != null)
		{
			if (useSmoothAxis)
			{
				Slider component = base.eventSystem.currentSelectedGameObject.GetComponent<Slider>();
				if (component != null)
				{
					float num = component.maxValue - component.minValue;
					component.value += newVal * smoothAxisMultiplier * num;
					_controlAxisUsed = true;
				}
				else
				{
					Scrollbar component2 = base.eventSystem.currentSelectedGameObject.GetComponent<Scrollbar>();
					if (component2 != null)
					{
						component2.value += newVal * smoothAxisMultiplier;
						_controlAxisUsed = true;
					}
					else
					{
						UIPopupButton component3 = base.eventSystem.currentSelectedGameObject.GetComponent<UIPopupButton>();
						UIPopup uIPopup = ((!(component3 != null)) ? base.eventSystem.currentSelectedGameObject.GetComponent<UIPopup>() : component3.popupParent);
						if (uIPopup != null)
						{
							_controlAxisUsed = true;
							if (axisAccumulation > 0.3f)
							{
								uIPopup.SetNextValue();
								axisAccumulation = 0f;
							}
							else if (axisAccumulation < -0.3f)
							{
								uIPopup.SetPreviousValue();
								axisAccumulation = 0f;
							}
						}
						else
						{
							_controlAxisUsed = false;
						}
					}
				}
			}
			else
			{
				_controlAxisUsed = true;
				float unscaledTime = Time.unscaledTime;
				if (unscaledTime > nextAxisActionTime)
				{
					nextAxisActionTime = unscaledTime + 1f / steppedAxisStepsPerSecond;
					AxisEventData axisEventData = GetAxisEventData(newVal, 0f, 0f);
					if (!ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler))
					{
						_controlAxisUsed = false;
					}
				}
			}
		}
		if (useSmoothAxis && !_controlAxisUsed && UITabSelector.activeTabSelector != null)
		{
			_controlAxisUsed = true;
			if (axisAccumulation > 0.6f)
			{
				UITabSelector.activeTabSelector.SelectNextTab();
				axisAccumulation = 0f;
			}
			else if (axisAccumulation < -0.6f)
			{
				UITabSelector.activeTabSelector.SelectPreviousTab();
				axisAccumulation = 0f;
			}
		}
	}

	protected bool GetSubmitLeftButtonDown()
	{
		if (OVRManager.isHmdPresent)
		{
			return OVRInput.GetDown(OVRInput.Button.Three, OVRInput.Controller.Touch);
		}
		return ViveTrackedControllers.singleton != null && ViveTrackedControllers.singleton.leftTouchpadPressedThisFrame;
	}

	protected bool GetSubmitLeftButtonUp()
	{
		if (OVRManager.isHmdPresent)
		{
			return OVRInput.GetUp(OVRInput.Button.Three, OVRInput.Controller.Touch);
		}
		return ViveTrackedControllers.singleton != null && ViveTrackedControllers.singleton.leftTouchpadUnpressedThisFrame;
	}

	protected bool GetSubmitRightButtonDown()
	{
		if (OVRManager.isHmdPresent)
		{
			return OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.Touch);
		}
		return ViveTrackedControllers.singleton != null && ViveTrackedControllers.singleton.rightTouchpadPressedThisFrame;
	}

	protected bool GetSubmitRightButtonUp()
	{
		if (OVRManager.isHmdPresent)
		{
			return OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.Touch);
		}
		return ViveTrackedControllers.singleton != null && ViveTrackedControllers.singleton.rightTouchpadUnpressedThisFrame;
	}

	public void ProcessMain()
	{
		SendUpdateEventToSelectedObject();
		PointerEventData lookPointerEventData = GetLookPointerEventData();
		currentLook = lookPointerEventData.pointerCurrentRaycast.gameObject;
		if (deselectWhenLookAway && currentLook == null && currentLookRight == null)
		{
			ClearSelection();
		}
		HandlePointerExitAndEnter(lookPointerEventData, currentLook);
		UpdateCursor(lookPointerEventData, cursor);
		if (!ignoreInputsWhenLookAway || (ignoreInputsWhenLookAway && currentLook != null))
		{
			_buttonUsed = false;
			if (GetSubmitLeftButtonDown() || (useEitherControllerButtons && GetSubmitRightButtonDown()))
			{
				ClearSelection();
				lookPointerEventData.pressPosition = lookPointerEventData.position;
				lookPointerEventData.pointerPressRaycast = lookPointerEventData.pointerCurrentRaycast;
				lookPointerEventData.pointerPress = null;
				if (currentLook != null)
				{
					currentPressed = currentLook;
					GameObject gameObject = null;
					if (mode == Mode.Pointer)
					{
						gameObject = ExecuteEvents.ExecuteHierarchy(currentPressed, lookPointerEventData, ExecuteEvents.pointerDownHandler);
						if (gameObject == null)
						{
							gameObject = ExecuteEvents.ExecuteHierarchy(currentPressed, lookPointerEventData, ExecuteEvents.pointerClickHandler);
							if (gameObject != null)
							{
								currentPressed = gameObject;
							}
						}
						else
						{
							currentPressed = gameObject;
							ExecuteEvents.Execute(gameObject, lookPointerEventData, ExecuteEvents.pointerClickHandler);
						}
					}
					else if (mode == Mode.Submit)
					{
						gameObject = ExecuteEvents.ExecuteHierarchy(currentPressed, lookPointerEventData, ExecuteEvents.submitHandler);
						if (gameObject == null)
						{
							gameObject = ExecuteEvents.ExecuteHierarchy(currentPressed, lookPointerEventData, ExecuteEvents.selectHandler);
						}
					}
					if (gameObject != null)
					{
						lookPointerEventData.pointerPress = gameObject;
						currentPressed = gameObject;
						Select(currentPressed);
						_buttonUsed = true;
					}
					if (mode == Mode.Pointer && currentPressed != null)
					{
						if (useLookDrag)
						{
							bool flag = true;
							if (!useLookDragSlider && (bool)currentPressed.GetComponent<Slider>())
							{
								flag = false;
							}
							else if (!useLookDragScrollbar && (bool)currentPressed.GetComponent<Scrollbar>())
							{
								flag = false;
								if (ExecuteEvents.Execute(currentPressed, lookPointerEventData, ExecuteEvents.beginDragHandler))
								{
									ExecuteEvents.Execute(currentPressed, lookPointerEventData, ExecuteEvents.endDragHandler);
								}
							}
							SliderControl component = currentPressed.GetComponent<SliderControl>();
							if (component != null && component.disableLookDrag)
							{
								flag = false;
							}
							if (flag)
							{
								ExecuteEvents.Execute(currentPressed, lookPointerEventData, ExecuteEvents.beginDragHandler);
								lookPointerEventData.pointerDrag = currentPressed;
								currentDragging = currentPressed;
							}
						}
						else if ((bool)currentPressed.GetComponent<Scrollbar>() && ExecuteEvents.Execute(currentPressed, lookPointerEventData, ExecuteEvents.beginDragHandler))
						{
							ExecuteEvents.Execute(currentPressed, lookPointerEventData, ExecuteEvents.endDragHandler);
						}
					}
				}
			}
		}
		if (GetSubmitLeftButtonUp() || (useEitherControllerButtons && GetSubmitRightButtonUp()))
		{
			if ((bool)currentDragging)
			{
				ExecuteEvents.Execute(currentDragging, lookPointerEventData, ExecuteEvents.endDragHandler);
				if (currentLook != null)
				{
					ExecuteEvents.ExecuteHierarchy(currentLook, lookPointerEventData, ExecuteEvents.dropHandler);
				}
				lookPointerEventData.pointerDrag = null;
				currentDragging = null;
			}
			if ((bool)currentPressed)
			{
				ExecuteEvents.Execute(currentPressed, lookPointerEventData, ExecuteEvents.pointerUpHandler);
				lookPointerEventData.rawPointerPress = null;
				lookPointerEventData.pointerPress = null;
				currentPressed = null;
			}
		}
		if (currentDragging != null)
		{
			ExecuteEvents.Execute(currentDragging, lookPointerEventData, ExecuteEvents.dragHandler);
		}
		if (ignoreInputsWhenLookAway && (!ignoreInputsWhenLookAway || !(currentLook != null)))
		{
			return;
		}
		_controlAxisUsed = false;
		if ((bool)base.eventSystem.currentSelectedGameObject || (bool)UITabSelector.activeTabSelector)
		{
			_controlAxisUsed = true;
			float axis = JoystickControl.GetAxis(controlAxis);
			if (axis > 0.01f || axis < -0.01f)
			{
				if (!axisOn)
				{
					axisAccumulation = Mathf.Sign(axis);
				}
				else
				{
					axisAccumulation += axis * Time.deltaTime;
				}
				axisOn = true;
				HandleAxis(axis);
			}
			else
			{
				axisOn = false;
			}
			axis = JoystickControl.GetAxis(discreteControlAxis);
			if (axis > 0.01f || axis < -0.01f)
			{
				if (invertDiscreteControlAxis)
				{
					axis = 0f - axis;
				}
				if (!discreteAxisOn)
				{
					axisAccumulation = Mathf.Sign(axis);
				}
				else
				{
					axisAccumulation += axis * Time.deltaTime;
				}
				discreteAxisOn = true;
				HandleAxis(axis);
			}
			else
			{
				discreteAxisOn = false;
			}
			if (ViveTrackedControllers.singleton != null)
			{
				axis = ViveTrackedControllers.singleton.GetLeftTouchDelta().x;
				if (axis > 0.0001f || axis < -0.0001f)
				{
					axis /= smoothAxisMultiplier * 10f;
					axisAccumulation += axis * 0.3f;
					HandleAxis(axis);
				}
			}
		}
		else
		{
			axisAccumulation = 0f;
		}
	}

	public override void Process()
	{
		_singleton = this;
		if (!disableStandaloneProcess)
		{
			base.Process();
		}
	}

	public void ProcessRight()
	{
		_singleton = this;
		SendUpdateEventToSelectedObject();
		PointerEventData lookPointerEventData = GetLookPointerEventData();
		currentLookRight = lookPointerEventData.pointerCurrentRaycast.gameObject;
		HandlePointerExitAndEnter(lookPointerEventData, currentLookRight);
		UpdateCursor(lookPointerEventData, cursorRight);
		if ((!ignoreInputsWhenLookAway || (ignoreInputsWhenLookAway && currentLookRight != null)) && GetSubmitRightButtonDown())
		{
			ClearSelection();
			lookPointerEventData.pressPosition = lookPointerEventData.position;
			lookPointerEventData.pointerPressRaycast = lookPointerEventData.pointerCurrentRaycast;
			lookPointerEventData.pointerPress = null;
			if (currentLookRight != null)
			{
				currentPressedRight = currentLookRight;
				GameObject gameObject = null;
				if (mode == Mode.Pointer)
				{
					gameObject = ExecuteEvents.ExecuteHierarchy(currentPressedRight, lookPointerEventData, ExecuteEvents.pointerDownHandler);
					if (gameObject == null)
					{
						gameObject = ExecuteEvents.ExecuteHierarchy(currentPressedRight, lookPointerEventData, ExecuteEvents.pointerClickHandler);
						if (gameObject != null)
						{
							currentPressedRight = gameObject;
						}
					}
					else
					{
						currentPressedRight = gameObject;
						ExecuteEvents.Execute(gameObject, lookPointerEventData, ExecuteEvents.pointerClickHandler);
					}
				}
				else if (mode == Mode.Submit)
				{
					gameObject = ExecuteEvents.ExecuteHierarchy(currentPressedRight, lookPointerEventData, ExecuteEvents.submitHandler);
					if (gameObject == null)
					{
						gameObject = ExecuteEvents.ExecuteHierarchy(currentPressedRight, lookPointerEventData, ExecuteEvents.selectHandler);
					}
				}
				if (gameObject != null)
				{
					lookPointerEventData.pointerPress = gameObject;
					currentPressedRight = gameObject;
					Select(currentPressedRight);
					_buttonUsed = true;
				}
				if (mode == Mode.Pointer && currentPressedRight != null)
				{
					if (useLookDrag)
					{
						bool flag = true;
						if (!useLookDragSlider && (bool)currentPressedRight.GetComponent<Slider>())
						{
							flag = false;
						}
						else if (!useLookDragScrollbar && (bool)currentPressedRight.GetComponent<Scrollbar>())
						{
							flag = false;
							if (ExecuteEvents.Execute(currentPressedRight, lookPointerEventData, ExecuteEvents.beginDragHandler))
							{
								ExecuteEvents.Execute(currentPressedRight, lookPointerEventData, ExecuteEvents.endDragHandler);
							}
						}
						SliderControl component = currentPressedRight.GetComponent<SliderControl>();
						if (component != null && component.disableLookDrag)
						{
							flag = false;
						}
						if (flag)
						{
							ExecuteEvents.Execute(currentPressedRight, lookPointerEventData, ExecuteEvents.beginDragHandler);
							lookPointerEventData.pointerDrag = currentPressedRight;
							currentDraggingRight = currentPressedRight;
						}
					}
					else if ((bool)currentPressedRight.GetComponent<Scrollbar>() && ExecuteEvents.Execute(currentPressedRight, lookPointerEventData, ExecuteEvents.beginDragHandler))
					{
						ExecuteEvents.Execute(currentPressedRight, lookPointerEventData, ExecuteEvents.endDragHandler);
					}
				}
			}
		}
		if (GetSubmitRightButtonUp())
		{
			if ((bool)currentDraggingRight)
			{
				ExecuteEvents.Execute(currentDraggingRight, lookPointerEventData, ExecuteEvents.endDragHandler);
				if (currentLookRight != null)
				{
					ExecuteEvents.ExecuteHierarchy(currentLookRight, lookPointerEventData, ExecuteEvents.dropHandler);
				}
				lookPointerEventData.pointerDrag = null;
				currentDraggingRight = null;
			}
			if ((bool)currentPressedRight)
			{
				ExecuteEvents.Execute(currentPressedRight, lookPointerEventData, ExecuteEvents.pointerUpHandler);
				lookPointerEventData.rawPointerPress = null;
				lookPointerEventData.pointerPress = null;
				currentPressedRight = null;
			}
		}
		if (currentDraggingRight != null)
		{
			ExecuteEvents.Execute(currentDraggingRight, lookPointerEventData, ExecuteEvents.dragHandler);
		}
	}
}
