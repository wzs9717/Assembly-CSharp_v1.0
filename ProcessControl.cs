using System;
using System.Diagnostics;
using UnityEngine;

public class ProcessControl : MonoBehaviour
{
	[SerializeField]
	private UIPopup _prioritySelector;

	[SerializeField]
	private ProcessPriorityClass _processPriorityClass = ProcessPriorityClass.Normal;

	[SerializeField]
	private bool _debug;

	public UIPopup prioritySelector
	{
		get
		{
			return _prioritySelector;
		}
		set
		{
			if (_prioritySelector != value)
			{
				if (_prioritySelector != null)
				{
					UIPopup uIPopup = _prioritySelector;
					uIPopup.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Remove(uIPopup.onValueChangeHandlers, new UIPopup.OnValueChange(SetProcessPriorityClass));
				}
				_prioritySelector = value;
				InitSelector();
			}
		}
	}

	public ProcessPriorityClass processPriorityClass
	{
		get
		{
			return _processPriorityClass;
		}
		set
		{
			if (_processPriorityClass != value)
			{
				_processPriorityClass = value;
				SetInternalProcessPriorityClass();
				if (_prioritySelector != null)
				{
					_prioritySelector.currentValue = _processPriorityClass.ToString();
				}
			}
		}
	}

	public bool debug
	{
		get
		{
			return _debug;
		}
		set
		{
			if (_debug != value)
			{
				_debug = value;
			}
		}
	}

	private void InitSelector()
	{
		UIPopup uIPopup = _prioritySelector;
		uIPopup.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup.onValueChangeHandlers, new UIPopup.OnValueChange(SetProcessPriorityClass));
		_prioritySelector.currentValue = _processPriorityClass.ToString();
	}

	private void SetInternalProcessPriorityClass()
	{
		if (!Application.isEditor)
		{
			Process currentProcess = Process.GetCurrentProcess();
			if (currentProcess.PriorityClass != _processPriorityClass)
			{
				currentProcess.PriorityClass = _processPriorityClass;
			}
		}
	}

	public void SetProcessPriorityClass(string type)
	{
		processPriorityClass = (ProcessPriorityClass)Enum.Parse(typeof(ProcessPriorityClass), type);
	}

	private void Start()
	{
		SetInternalProcessPriorityClass();
		if (_prioritySelector != null)
		{
			InitSelector();
		}
	}
}
