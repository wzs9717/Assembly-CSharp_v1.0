using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TwoDPicker : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IEventSystemHandler
{
	public Image Selector;

	[SerializeField]
	protected float _xVal;

	[SerializeField]
	protected float _yVal;

	public float xVal
	{
		get
		{
			return _xVal;
		}
		set
		{
			_xVal = value;
			_xVal = Mathf.Clamp01(_xVal);
			SetSelectorPositionFromXYVal();
		}
	}

	public float yVal
	{
		get
		{
			return _yVal;
		}
		set
		{
			_yVal = value;
			_yVal = Mathf.Clamp01(_yVal);
			SetSelectorPositionFromXYVal();
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
	}

	public void OnDrag(PointerEventData data)
	{
		if (Selector != null)
		{
			SetDraggedPosition(data);
		}
	}

	protected void SetXYValFromSelectorPosition()
	{
		if (Selector != null)
		{
			RectTransform component = Selector.GetComponent<RectTransform>();
			RectTransform component2 = GetComponent<RectTransform>();
			if (component2 != null && component != null)
			{
				Vector2 anchoredPosition = component.anchoredPosition;
				xVal = anchoredPosition.x / component2.rect.width;
				yVal = anchoredPosition.y / component2.rect.height;
			}
		}
	}

	protected virtual void SetSelectorPositionFromXYVal()
	{
		if (Selector != null)
		{
			RectTransform component = Selector.GetComponent<RectTransform>();
			RectTransform component2 = GetComponent<RectTransform>();
			if (component2 != null && component != null)
			{
				Vector2 anchoredPosition = default(Vector2);
				anchoredPosition.x = component2.rect.width * _xVal;
				anchoredPosition.y = component2.rect.height * _yVal;
				component.anchoredPosition = anchoredPosition;
			}
		}
	}

	protected void SetDraggedPosition(PointerEventData data)
	{
		if (Selector != null)
		{
			RectTransform component = Selector.GetComponent<RectTransform>();
			RectTransform component2 = GetComponent<RectTransform>();
			if (component2 != null && component != null && RectTransformUtility.ScreenPointToWorldPointInRectangle(component2, data.position, data.pressEventCamera, out var worldPoint))
			{
				component.position = worldPoint;
				component.rotation = component2.rotation;
				SetXYValFromSelectorPosition();
			}
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
	}
}
