using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class MoveProducer : JSONStorable
{
	[SerializeField]
	protected bool _on = true;

	public Toggle onToggle;

	[SerializeField]
	protected FreeControllerV3 _receiver;

	protected string freeControllerAtomUID;

	protected Vector3 _currentPosition;

	protected Quaternion _currentRotation;

	public UIPopup receiverAtomSelectionPopup;

	public UIPopup receiverSelectionPopup;

	public virtual bool on
	{
		get
		{
			return _on;
		}
		set
		{
			if (_on != value)
			{
				_on = value;
				SyncOnToggle();
			}
		}
	}

	public virtual FreeControllerV3 receiver
	{
		get
		{
			return _receiver;
		}
		set
		{
			_receiver = value;
		}
	}

	public virtual Vector3 currentPosition => _currentPosition;

	public virtual Quaternion currentRotation => _currentRotation;

	public override JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true)
	{
		JSONClass jSON = base.GetJSON(includePhysical, includeAppearance);
		if (includePhysical)
		{
			if (!on)
			{
				needsStore = true;
				jSON["on"].AsBool = on;
			}
			if (_receiver != null && _receiver.containingAtom != null)
			{
				needsStore = true;
				jSON["receiver"] = _receiver.containingAtom.uid + ":" + _receiver.name;
			}
		}
		return jSON;
	}

	public override void RestoreFromJSON(JSONClass jc, bool restorePhysical = true, bool restoreAppearance = true)
	{
		base.RestoreFromJSON(jc, restorePhysical, restoreAppearance);
		if (restorePhysical)
		{
			if (jc["on"] != null)
			{
				on = jc["on"].AsBool;
			}
			else
			{
				on = true;
			}
			if (jc["receiver"] != null)
			{
				SetReceiverByName(jc["receiver"]);
			}
			else
			{
				SetReceiverByName(string.Empty);
			}
		}
	}

	protected virtual void InitOnToggle()
	{
		if (onToggle != null)
		{
			onToggle.onValueChanged.AddListener(delegate
			{
				on = onToggle.isOn;
			});
			on = onToggle.isOn;
		}
	}

	protected virtual void SyncOnToggle()
	{
		if (onToggle != null)
		{
			onToggle.isOn = _on;
		}
	}

	public virtual void SetReceiverAtom(string atomUID)
	{
		if (!(SuperController.singleton != null))
		{
			return;
		}
		Atom atomByUid = SuperController.singleton.GetAtomByUid(atomUID);
		if (atomByUid != null)
		{
			freeControllerAtomUID = atomUID;
			List<string> freeControllerNamesInAtom = SuperController.singleton.GetFreeControllerNamesInAtom(freeControllerAtomUID);
			onFreeControllerNamesChanged(freeControllerNamesInAtom);
			if (receiverSelectionPopup != null)
			{
				receiverSelectionPopup.currentValue = "None";
			}
		}
		else
		{
			onFreeControllerNamesChanged(null);
		}
	}

	public virtual void SetReceiverObject(string objectName)
	{
		if (freeControllerAtomUID != null && SuperController.singleton != null)
		{
			FreeControllerV3 freeControllerV2 = (receiver = SuperController.singleton.FreeControllerNameToFreeController(freeControllerAtomUID + ":" + objectName));
		}
	}

	public void SetReceiverByName(string controllerName)
	{
		if (!(SuperController.singleton != null))
		{
			return;
		}
		FreeControllerV3 freeControllerV = SuperController.singleton.FreeControllerNameToFreeController(controllerName);
		if (freeControllerV != null)
		{
			if (receiverAtomSelectionPopup != null && freeControllerV.containingAtom != null)
			{
				receiverAtomSelectionPopup.currentValue = freeControllerV.containingAtom.uid;
			}
			if (receiverSelectionPopup != null)
			{
				receiverSelectionPopup.currentValue = freeControllerV.name;
			}
		}
		else
		{
			if (receiverAtomSelectionPopup != null)
			{
				receiverAtomSelectionPopup.currentValue = "None";
			}
			if (receiverSelectionPopup != null)
			{
				receiverSelectionPopup.currentValue = "None";
			}
		}
	}

	protected virtual void SetReceiverAtomNames()
	{
		if (!(receiverAtomSelectionPopup != null) || !(SuperController.singleton != null))
		{
			return;
		}
		List<string> atomUIDsWithFreeControllers = SuperController.singleton.GetAtomUIDsWithFreeControllers();
		if (atomUIDsWithFreeControllers == null)
		{
			receiverAtomSelectionPopup.numPopupValues = 1;
			receiverAtomSelectionPopup.setPopupValue(0, "None");
			return;
		}
		receiverAtomSelectionPopup.numPopupValues = atomUIDsWithFreeControllers.Count + 1;
		receiverAtomSelectionPopup.setPopupValue(0, "None");
		for (int i = 0; i < atomUIDsWithFreeControllers.Count; i++)
		{
			receiverAtomSelectionPopup.setPopupValue(i + 1, atomUIDsWithFreeControllers[i]);
		}
	}

	protected void onFreeControllerNamesChanged(List<string> controllerNames)
	{
		if (!(receiverSelectionPopup != null))
		{
			return;
		}
		if (controllerNames == null)
		{
			receiverSelectionPopup.numPopupValues = 1;
			receiverSelectionPopup.setPopupValue(0, "None");
			return;
		}
		receiverSelectionPopup.numPopupValues = controllerNames.Count + 1;
		receiverSelectionPopup.setPopupValue(0, "None");
		for (int i = 0; i < controllerNames.Count; i++)
		{
			receiverSelectionPopup.setPopupValue(i + 1, controllerNames[i]);
		}
	}

	public void SelectReceiver(FreeControllerV3 rcvr)
	{
		if (receiverAtomSelectionPopup != null && rcvr != null && rcvr.containingAtom != null)
		{
			receiverAtomSelectionPopup.currentValue = rcvr.containingAtom.uid;
		}
		if (receiverSelectionPopup != null && rcvr != null)
		{
			receiverSelectionPopup.currentValueNoCallback = rcvr.name;
		}
		receiver = rcvr;
	}

	public void SelectControllerFromScene()
	{
		SuperController.singleton.SelectModeControllers(SelectReceiver);
	}

	public virtual void InitUI()
	{
		InitOnToggle();
		if (receiverAtomSelectionPopup != null)
		{
			UIPopup uIPopup = receiverAtomSelectionPopup;
			uIPopup.onOpenPopupHandlers = (UIPopup.OnOpenPopup)Delegate.Combine(uIPopup.onOpenPopupHandlers, new UIPopup.OnOpenPopup(SetReceiverAtomNames));
			UIPopup uIPopup2 = receiverAtomSelectionPopup;
			uIPopup2.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup2.onValueChangeHandlers, new UIPopup.OnValueChange(SetReceiverAtom));
		}
		if (receiverSelectionPopup != null)
		{
			UIPopup uIPopup3 = receiverSelectionPopup;
			uIPopup3.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup3.onValueChangeHandlers, new UIPopup.OnValueChange(SetReceiverObject));
		}
	}

	protected virtual void Awake()
	{
		InitUI();
	}

	protected virtual void Start()
	{
		SetCurrentPositionAndRotation();
	}

	protected virtual void SetCurrentPositionAndRotation()
	{
		_currentPosition = base.transform.position;
		_currentRotation = base.transform.rotation;
	}

	protected virtual void UpdateTransform()
	{
		if (_on && (SuperController.singleton == null || !SuperController.singleton.freezeAnimation))
		{
			SetCurrentPositionAndRotation();
			if (_receiver != null)
			{
				_receiver.control.position = _currentPosition;
				_receiver.control.rotation = _currentRotation;
			}
		}
	}

	protected virtual void FixedUpdate()
	{
		UpdateTransform();
	}

	protected virtual void Update()
	{
		UpdateTransform();
	}
}
