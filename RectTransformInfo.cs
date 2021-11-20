using UnityEngine;

[ExecuteInEditMode]
public class RectTransformInfo : MonoBehaviour
{
	public Vector2 anchoredPosition;

	public Vector2 anchorMin;

	public Vector2 anchorMax;

	public Vector2 offsetMin;

	public Vector2 offsetMax;

	public Vector2 pivot;

	public Rect rect;

	public Vector2 sizeDelta;

	private void Update()
	{
		RectTransform component = GetComponent<RectTransform>();
		if (component != null)
		{
			anchoredPosition = component.anchoredPosition;
			anchorMin = component.anchorMin;
			anchorMax = component.anchorMax;
			offsetMin = component.offsetMin;
			offsetMax = component.offsetMax;
			pivot = component.pivot;
			rect = component.rect;
			sizeDelta = component.sizeDelta;
		}
	}
}
