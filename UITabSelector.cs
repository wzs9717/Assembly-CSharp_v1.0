using UnityEngine;
using UnityEngine.UI;

public class UITabSelector : MonoBehaviour
{
	public static UITabSelector activeTabSelector;

	public Color normalColor = Color.white;

	public Color activeColor = Color.blue;

	public Image tabBackground;

	public string startingTabName;

	protected bool activeTabChanged;

	private string FindActiveToggle()
	{
		foreach (Transform item in base.transform)
		{
			if (item.gameObject.activeSelf)
			{
				Toggle component = item.GetComponent<Toggle>();
				if (component != null && component.isOn)
				{
					return item.name;
				}
			}
		}
		return null;
	}

	public void SelectNextTab()
	{
		bool flag = false;
		foreach (Transform item in base.transform)
		{
			if (!item.gameObject.activeSelf)
			{
				continue;
			}
			Toggle component = item.GetComponent<Toggle>();
			if (component != null)
			{
				if (flag)
				{
					component.isOn = true;
					SetActiveTab(item.name);
					break;
				}
				if (component.isOn)
				{
					flag = true;
				}
			}
		}
	}

	public void SelectPreviousTab()
	{
		Toggle toggle = null;
		foreach (Transform item in base.transform)
		{
			if (!item.gameObject.activeSelf)
			{
				continue;
			}
			Toggle component = item.GetComponent<Toggle>();
			if (component != null)
			{
				if (component.isOn && toggle != null)
				{
					toggle.isOn = true;
					SetActiveTab(toggle.name);
					break;
				}
				toggle = component;
			}
		}
	}

	public void SetActiveTab(string name)
	{
		foreach (Transform item in base.transform)
		{
			UITab component = item.GetComponent<UITab>();
			if (component != null)
			{
				if (component.name == name)
				{
					component.gameObject.SetActive(value: true);
				}
				else
				{
					component.gameObject.SetActive(value: false);
				}
			}
		}
	}

	protected void ActiveTabChangedProcess()
	{
		if (!activeTabChanged)
		{
			return;
		}
		activeTabChanged = false;
		if (activeTabSelector != null && activeTabSelector.tabBackground != null)
		{
			activeTabSelector.tabBackground.color = normalColor;
		}
		if (base.gameObject.activeInHierarchy)
		{
			activeTabSelector = this;
			if (tabBackground != null)
			{
				tabBackground.color = activeColor;
			}
		}
		string text = FindActiveToggle();
		if (text != null)
		{
			SetActiveTab(text);
		}
	}

	public void ActiveTabChanged()
	{
		activeTabChanged = true;
	}

	private void Update()
	{
		ActiveTabChangedProcess();
	}

	private void Start()
	{
		SetActiveTab(startingTabName);
	}
}
