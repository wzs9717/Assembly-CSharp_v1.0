using System;
using SimpleJSON;
using UnityEngine.UI;

public class SyncForceProducerV2 : ForceProducerV2
{
	public ForceProducerV2 syncProducer;

	public UIPopup syncProducerSelectionPopup;

	public Toggle autoSyncToggle;

	protected bool _autoSync;

	public bool autoSync
	{
		get
		{
			return _autoSync;
		}
		set
		{
			if (_autoSync != value)
			{
				_autoSync = value;
				if (autoSyncToggle != null)
				{
					autoSyncToggle.isOn = value;
				}
			}
		}
	}

	public override JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true)
	{
		JSONClass jSON = base.GetJSON(includePhysical, includeAppearance);
		if (includePhysical)
		{
			if (syncProducer != null && syncProducer.containingAtom != null)
			{
				needsStore = true;
				jSON["syncTo"] = syncProducer.containingAtom.uid + ":" + syncProducer.name;
			}
			if (autoSync)
			{
				needsStore = true;
				jSON["autoSync"].AsBool = autoSync;
			}
		}
		return jSON;
	}

	public override void RestoreFromJSON(JSONClass jc, bool restorePhysical = true, bool restoreAppearance = true)
	{
		base.RestoreFromJSON(jc, restorePhysical, restoreAppearance);
		if (restorePhysical)
		{
			if (jc["syncTo"] != null)
			{
				SetSyncProducer(jc["syncTo"]);
			}
			else
			{
				SetSyncProducer(string.Empty);
			}
			if (jc["autoSync"] != null)
			{
				autoSync = jc["autoSync"].AsBool;
			}
			else
			{
				autoSync = false;
			}
		}
	}

	private void InitAutoSyncToggle()
	{
		if (autoSyncToggle != null)
		{
			autoSyncToggle.onValueChanged.AddListener(delegate
			{
				autoSync = autoSyncToggle.isOn;
			});
			autoSync = autoSyncToggle.isOn;
		}
	}

	protected virtual void SetProducerNames()
	{
		if (!(syncProducerSelectionPopup != null) || !(SuperController.singleton != null))
		{
			return;
		}
		string[] forceProducerNames = SuperController.singleton.forceProducerNames;
		if (forceProducerNames == null)
		{
			syncProducerSelectionPopup.numPopupValues = 1;
			syncProducerSelectionPopup.setPopupValue(0, "None");
			return;
		}
		syncProducerSelectionPopup.numPopupValues = forceProducerNames.Length + 1;
		syncProducerSelectionPopup.setPopupValue(0, "None");
		for (int i = 0; i < forceProducerNames.Length; i++)
		{
			syncProducerSelectionPopup.setPopupValue(i + 1, forceProducerNames[i]);
		}
	}

	public void SetSyncProducer(string producerName)
	{
		if (SuperController.singleton != null)
		{
			ForceProducerV2 forceProducerV = (syncProducer = SuperController.singleton.ProducerNameToForceProducer(producerName));
			if (syncProducerSelectionPopup != null)
			{
				syncProducerSelectionPopup.currentValue = producerName;
			}
		}
	}

	public void SyncAllParameters()
	{
		if (syncProducer != null)
		{
			forceFactor = syncProducer.forceFactor;
			forceQuickness = syncProducer.forceQuickness;
			maxForce = syncProducer.maxForce;
			torqueFactor = syncProducer.torqueFactor;
			torqueQuickness = syncProducer.torqueQuickness;
			maxTorque = syncProducer.maxTorque;
		}
	}

	protected override void InitUI()
	{
		base.InitUI();
		InitAutoSyncToggle();
		if (syncProducerSelectionPopup != null)
		{
			UIPopup uIPopup = syncProducerSelectionPopup;
			uIPopup.onOpenPopupHandlers = (UIPopup.OnOpenPopup)Delegate.Combine(uIPopup.onOpenPopupHandlers, new UIPopup.OnOpenPopup(SetProducerNames));
			UIPopup uIPopup2 = syncProducerSelectionPopup;
			uIPopup2.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup2.onValueChangeHandlers, new UIPopup.OnValueChange(SetSyncProducer));
		}
	}

	protected override void Update()
	{
		if (syncProducer != null)
		{
			SetTargetForcePercent(syncProducer.targetForcePercent);
		}
		if (autoSync)
		{
			SyncAllParameters();
		}
		base.Update();
	}

	protected override void Start()
	{
		base.Start();
	}
}
