using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateTabbedUI : MonoBehaviour
{
	public float elementHeight = 100f;

	public int numColumns = 1;

	public float columnPercentBuffer = 0.01f;

	public float rowBuffer = 10f;

	public Transform controlUIPrefab;

	public Transform tabUIPrefab;

	public Transform tabButtonUIPrefab;

	protected UITabSelector tabSelector;

	protected int numElementsPerColumn;

	protected int numElementsPerTab;

	protected int controlsCount;

	protected int controlsOnColumnCount;

	protected int controlsOnTabCount;

	protected int tabCount;

	protected int currentColumn;

	protected int currentTab;

	protected List<Vector2> controlPositions;

	protected ToggleGroup tg;

	protected Transform controlContainer;

	public void ClearChildren(Transform t)
	{
		foreach (Transform item in t)
		{
			Object.Destroy(item.gameObject);
		}
	}

	protected virtual Transform InstantiateControl(Transform parent, int index)
	{
		Vector2 anchoredPosition = controlPositions[index];
		Transform transform = Object.Instantiate(controlUIPrefab);
		transform.SetParent(parent, worldPositionStays: false);
		RectTransform component = transform.GetComponent<RectTransform>();
		component.anchoredPosition = anchoredPosition;
		Vector2 sizeDelta = component.sizeDelta;
		sizeDelta.y = elementHeight;
		component.sizeDelta = sizeDelta;
		float num = 1f / (float)numColumns;
		Vector2 anchorMin = default(Vector2);
		anchorMin.x = anchoredPosition.x * num + columnPercentBuffer;
		anchorMin.y = 1f;
		Vector2 anchorMax = default(Vector2);
		anchorMax.x = (anchoredPosition.x + 1f) * num - columnPercentBuffer;
		anchorMax.y = 1f;
		component.anchorMin = anchorMin;
		component.anchorMax = anchorMax;
		return transform;
	}

	private Transform CreateTabButton(bool on)
	{
		tabCount++;
		Transform transform = Object.Instantiate(tabButtonUIPrefab);
		transform.SetParent(base.transform, worldPositionStays: false);
		transform.name = tabCount.ToString();
		RectTransform component = transform.GetComponent<RectTransform>();
		Vector2 zero = Vector2.zero;
		zero.x = (float)tabCount * component.rect.width;
		zero.y = 0f - component.rect.height;
		Text[] componentsInChildren = transform.GetComponentsInChildren<Text>(includeInactive: true);
		if (componentsInChildren != null)
		{
			componentsInChildren[0].text = tabCount.ToString();
		}
		Toggle[] componentsInChildren2 = transform.GetComponentsInChildren<Toggle>(includeInactive: true);
		if (tg != null && componentsInChildren2 != null)
		{
			componentsInChildren2[0].isOn = on;
			componentsInChildren2[0].group = tg;
			string toggleName = transform.name;
			componentsInChildren2[0].onValueChanged.AddListener(delegate(bool arg0)
			{
				TabChange(toggleName, arg0);
				if (tabSelector != null)
				{
					tabSelector.ActiveTabChanged();
				}
			});
		}
		RectTransform component2 = transform.GetComponent<RectTransform>();
		component2.anchoredPosition = zero;
		return transform;
	}

	public virtual void TabChange(string name, bool on)
	{
		if (!on || !(controlContainer != null))
		{
			return;
		}
		ClearChildren(controlContainer);
		if (int.TryParse(name, out var result))
		{
			currentTab = result;
			int num = (result - 1) * numElementsPerTab;
			int num2 = num + numElementsPerTab;
			if (num2 > controlsCount)
			{
				num2 = controlsCount;
			}
			for (int i = num; i < num2; i++)
			{
				InstantiateControl(controlContainer, i);
			}
		}
	}

	protected void AllocateControl()
	{
		controlsCount++;
		if (controlsOnColumnCount == numElementsPerColumn)
		{
			currentColumn++;
			controlsOnColumnCount = 0;
		}
		if (controlsOnTabCount == numElementsPerTab)
		{
			CreateTabButton(on: false);
			controlsOnTabCount = 0;
			currentColumn = 0;
			controlsOnColumnCount = 0;
		}
		controlsOnTabCount++;
		controlsOnColumnCount++;
		Vector2 zero = Vector2.zero;
		zero.x = currentColumn;
		zero.y = (float)controlsOnColumnCount * (0f - elementHeight) - (float)(controlsOnColumnCount - 1) * rowBuffer + elementHeight * 0.5f;
		controlPositions.Add(zero);
	}

	public virtual void GenerateStart()
	{
		if (tabButtonUIPrefab != null && controlUIPrefab != null)
		{
			ClearChildren(base.transform);
			controlPositions = new List<Vector2>();
			tg = GetComponent<ToggleGroup>();
			controlsCount = 0;
			controlsOnTabCount = 0;
			controlsOnColumnCount = 0;
			currentColumn = 0;
			tabCount = 0;
			tabSelector = GetComponent<UITabSelector>();
			controlContainer = Object.Instantiate(tabUIPrefab);
			controlContainer.SetParent(base.transform, worldPositionStays: false);
			controlContainer.gameObject.SetActive(value: true);
			controlContainer.name = "ControlContainer";
			CreateTabButton(on: true);
			RectTransform component = controlContainer.GetComponent<RectTransform>();
			numElementsPerColumn = Mathf.FloorToInt(component.rect.height / (elementHeight + rowBuffer));
			numElementsPerTab = numElementsPerColumn * numColumns;
		}
	}

	public virtual void Generate()
	{
	}

	public virtual void GenerateFinish()
	{
		TabChange("1", on: true);
	}

	private void Start()
	{
		GenerateStart();
		Generate();
		GenerateFinish();
	}
}
