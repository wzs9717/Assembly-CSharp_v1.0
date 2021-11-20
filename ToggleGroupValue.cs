using UnityEngine;
using UnityEngine.UI;

public class ToggleGroupValue : MonoBehaviour
{
	public delegate void OnToggleChanged(string activeToggle);

	private string _activeToggleName;

	private bool disableCallback;

	private Toggle[] toggles;

	public OnToggleChanged onToggleChangedHandlers;

	public string activeToggleName
	{
		get
		{
			return _activeToggleName;
		}
		set
		{
			if (!(_activeToggleName != value))
			{
				return;
			}
			Init();
			bool flag = false;
			disableCallback = true;
			Toggle[] array = toggles;
			foreach (Toggle toggle in array)
			{
				if (toggle.name == value)
				{
					flag = true;
					toggle.isOn = true;
					_activeToggleName = value;
				}
				else
				{
					toggle.isOn = false;
				}
			}
			if (flag && onToggleChangedHandlers != null)
			{
				onToggleChangedHandlers(_activeToggleName);
			}
			disableCallback = false;
		}
	}

	private void Init()
	{
		if (toggles != null)
		{
			return;
		}
		ToggleGroup component = GetComponent<ToggleGroup>();
		toggles = GetComponentsInChildren<Toggle>(includeInactive: true);
		if (!(component != null) || toggles == null)
		{
			return;
		}
		Toggle[] array = toggles;
		foreach (Toggle toggle in array)
		{
			if (toggle.group != component)
			{
				toggle.group = component;
			}
		}
	}

	private void Start()
	{
		Init();
	}

	public void ToggleChanged()
	{
		Init();
		if (toggles == null || disableCallback)
		{
			return;
		}
		Toggle[] array = toggles;
		foreach (Toggle toggle in array)
		{
			if (toggle.isOn && _activeToggleName != toggle.name)
			{
				_activeToggleName = toggle.name;
				if (onToggleChangedHandlers != null)
				{
					onToggleChangedHandlers(_activeToggleName);
				}
			}
		}
	}
}
