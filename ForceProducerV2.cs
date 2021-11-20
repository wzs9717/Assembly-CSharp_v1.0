using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class ForceProducerV2 : JSONStorable
{
	public enum AxisName
	{
		X,
		Y,
		Z,
		NegX,
		NegY,
		NegZ
	}

	public Atom containingAtom;

	[SerializeField]
	protected bool _on = true;

	public Toggle onToggle;

	[SerializeField]
	protected ForceReceiver _receiver;

	public AxisName forceAxis;

	public AxisName torqueAxis = AxisName.Z;

	[SerializeField]
	protected bool _useForce = true;

	public Toggle useForceToggle;

	[SerializeField]
	protected bool _useTorque = true;

	public Toggle useTorqueToggle;

	[SerializeField]
	protected float _forceFactor = 200f;

	public Slider forceFactorSlider;

	public Slider forceFactorSliderAlt;

	[SerializeField]
	protected float _maxForce = 5000f;

	public Slider maxForceSlider;

	[SerializeField]
	protected float _torqueFactor = 100f;

	public Slider torqueFactorSlider;

	public Slider torqueFactorSliderAlt;

	[SerializeField]
	protected float _maxTorque = 1000f;

	public Slider maxTorqueSlider;

	[SerializeField]
	protected float _forceQuickness = 10f;

	public Slider forceQuicknessSlider;

	public Slider forceQuicknessSliderAlt;

	[SerializeField]
	protected float _torqueQuickness = 10f;

	public Slider torqueQuicknessSlider;

	public Slider torqueQuicknessSliderAlt;

	public Material linkLineMaterial;

	public Material forceLineMaterial;

	public Material targetForceLineMaterial;

	public Material rawForceLineMaterial;

	public bool drawLines = true;

	public float linesScale = 0.001f;

	public float lineOffset = 0.1f;

	public float lineSpacing = 0.01f;

	public UIPopup receiverAtomSelectionPopup;

	public UIPopup receiverSelectionPopup;

	public UIPopup receiverAtomAndReceiverSelectionPopup;

	public float targetForcePercent;

	public Vector3 appliedForce;

	public Vector3 appliedTorque;

	protected Vector3 forceDirection;

	protected Vector3 torqueDirection;

	protected Vector3 currentForce;

	protected Vector3 rawForce;

	protected Vector3 targetForce;

	protected Vector3 currentTorque;

	public Vector3 rawTorque;

	public Vector3 targetTorque;

	protected LineDrawer linkLineDrawer;

	protected LineDrawer forceLineDrawer;

	protected LineDrawer targetForceLineDrawer;

	protected LineDrawer rawForceLineDrawer;

	protected Rigidbody RB;

	protected float torqueLineMult = 10f;

	protected string receiverAtomUID;

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

	public virtual ForceReceiver receiver
	{
		get
		{
			return _receiver;
		}
		set
		{
			_receiver = value;
			if (_receiver != null)
			{
				RB = _receiver.GetComponent<Rigidbody>();
			}
			else
			{
				RB = null;
			}
		}
	}

	public virtual bool useForce
	{
		get
		{
			return _useForce;
		}
		set
		{
			if (_useForce != value)
			{
				_useForce = value;
				SyncUseForceToggle();
			}
		}
	}

	public virtual bool useTorque
	{
		get
		{
			return _useTorque;
		}
		set
		{
			if (_useTorque != value)
			{
				_useTorque = value;
				SyncUseTorqueToggle();
			}
		}
	}

	public virtual float forceFactor
	{
		get
		{
			return _forceFactor;
		}
		set
		{
			if (_forceFactor != value)
			{
				_forceFactor = value;
				SyncForceFactorSlider();
			}
		}
	}

	public virtual float maxForce
	{
		get
		{
			return _maxForce;
		}
		set
		{
			if (_maxForce != value)
			{
				_maxForce = value;
				SyncMaxForceSlider();
			}
		}
	}

	public virtual float torqueFactor
	{
		get
		{
			return _torqueFactor;
		}
		set
		{
			if (_torqueFactor != value)
			{
				_torqueFactor = value;
				SyncTorqueFactorSlider();
			}
		}
	}

	public virtual float maxTorque
	{
		get
		{
			return _maxTorque;
		}
		set
		{
			if (_maxTorque != value)
			{
				_maxTorque = value;
				SyncMaxTorqueSlider();
			}
		}
	}

	public virtual float forceQuickness
	{
		get
		{
			return _forceQuickness;
		}
		set
		{
			if (_forceQuickness != value)
			{
				_forceQuickness = value;
				SyncForceQuicknessSlider();
			}
		}
	}

	public virtual float torqueQuickness
	{
		get
		{
			return _torqueQuickness;
		}
		set
		{
			if (_torqueQuickness != value)
			{
				_torqueQuickness = value;
				SyncTorqueQuicknessSlider();
			}
		}
	}

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
			if (forceFactorSlider != null)
			{
				SliderControl component = forceFactorSlider.GetComponent<SliderControl>();
				if (component == null || component.defaultValue != forceFactor)
				{
					needsStore = true;
					jSON["forceFactor"].AsFloat = forceFactor;
				}
			}
			if (maxForceSlider != null)
			{
				SliderControl component2 = maxForceSlider.GetComponent<SliderControl>();
				if (component2 == null || component2.defaultValue != maxForce)
				{
					needsStore = true;
					jSON["maxForce"].AsFloat = maxForce;
				}
			}
			if (forceQuicknessSlider != null)
			{
				SliderControl component3 = forceQuicknessSlider.GetComponent<SliderControl>();
				if (component3 == null || component3.defaultValue != forceQuickness)
				{
					needsStore = true;
					jSON["forceQuickness"].AsFloat = forceQuickness;
				}
			}
			if (torqueFactorSlider != null)
			{
				SliderControl component4 = torqueFactorSlider.GetComponent<SliderControl>();
				if (component4 == null || component4.defaultValue != torqueFactor)
				{
					needsStore = true;
					jSON["torqueFactor"].AsFloat = torqueFactor;
				}
			}
			if (maxTorqueSlider != null)
			{
				SliderControl component5 = maxTorqueSlider.GetComponent<SliderControl>();
				if (component5 == null || component5.defaultValue != maxTorque)
				{
					needsStore = true;
					jSON["maxTorque"].AsFloat = maxTorque;
				}
			}
			if (torqueQuicknessSlider != null)
			{
				SliderControl component6 = torqueQuicknessSlider.GetComponent<SliderControl>();
				if (component6 == null || component6.defaultValue != torqueQuickness)
				{
					needsStore = true;
					jSON["torqueQuickness"].AsFloat = torqueQuickness;
				}
			}
		}
		return jSON;
	}

	public override void RestoreFromJSON(JSONClass jc, bool restorePhysical = true, bool restoreAppearance = true)
	{
		base.RestoreFromJSON(jc, restorePhysical, restoreAppearance);
		if (!restorePhysical)
		{
			return;
		}
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
			SetForceReceiver(jc["receiver"]);
		}
		else
		{
			SetForceReceiver(string.Empty);
		}
		if (jc["forceFactor"] != null)
		{
			forceFactor = jc["forceFactor"].AsFloat;
		}
		else if (forceFactorSlider != null)
		{
			SliderControl component = forceFactorSlider.GetComponent<SliderControl>();
			if (component != null)
			{
				forceFactor = component.defaultValue;
			}
		}
		if (jc["maxForce"] != null)
		{
			maxForce = jc["maxForce"].AsFloat;
		}
		else if (maxForceSlider != null)
		{
			SliderControl component2 = maxForceSlider.GetComponent<SliderControl>();
			if (component2 != null)
			{
				maxForce = component2.defaultValue;
			}
		}
		if (jc["forceQuickness"] != null)
		{
			forceQuickness = jc["forceQuickness"].AsFloat;
		}
		else if (forceQuicknessSlider != null)
		{
			SliderControl component3 = forceQuicknessSlider.GetComponent<SliderControl>();
			if (component3 != null)
			{
				forceQuickness = component3.defaultValue;
			}
		}
		if (jc["torqueFactor"] != null)
		{
			torqueFactor = jc["torqueFactor"].AsFloat;
		}
		else if (torqueFactorSlider != null)
		{
			SliderControl component4 = torqueFactorSlider.GetComponent<SliderControl>();
			if (component4 != null)
			{
				torqueFactor = component4.defaultValue;
			}
		}
		if (jc["maxTorque"] != null)
		{
			maxTorque = jc["maxTorque"].AsFloat;
		}
		else if (maxTorqueSlider != null)
		{
			SliderControl component5 = maxTorqueSlider.GetComponent<SliderControl>();
			if (component5 != null)
			{
				maxTorque = component5.defaultValue;
			}
		}
		if (jc["torqueQuickness"] != null)
		{
			torqueQuickness = jc["torqueQuickness"].AsFloat;
		}
		else if (torqueQuicknessSlider != null)
		{
			SliderControl component6 = torqueQuicknessSlider.GetComponent<SliderControl>();
			if (component6 != null)
			{
				torqueQuickness = component6.defaultValue;
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

	protected virtual void InitUseForceToggle()
	{
		if (useForceToggle != null)
		{
			useForceToggle.onValueChanged.AddListener(delegate
			{
				useForce = useForceToggle.isOn;
			});
			useForce = useForceToggle.isOn;
		}
	}

	protected virtual void SyncUseForceToggle()
	{
		if (useForceToggle != null)
		{
			useForceToggle.isOn = _useForce;
		}
	}

	protected virtual void InitUseTorqueToggle()
	{
		if (useTorqueToggle != null)
		{
			useTorqueToggle.onValueChanged.AddListener(delegate
			{
				useTorque = useTorqueToggle.isOn;
			});
			useTorque = useTorqueToggle.isOn;
		}
	}

	protected virtual void SyncUseTorqueToggle()
	{
		if (useTorqueToggle != null)
		{
			useTorqueToggle.isOn = _useTorque;
		}
	}

	protected virtual void InitForceFactorSlider()
	{
		if (forceFactorSliderAlt != null)
		{
			forceFactorSliderAlt.onValueChanged.AddListener(delegate
			{
				forceFactor = forceFactorSliderAlt.value;
			});
		}
		if (forceFactorSlider != null)
		{
			forceFactorSlider.onValueChanged.AddListener(delegate
			{
				forceFactor = forceFactorSlider.value;
			});
			forceFactor = forceFactorSlider.value;
		}
	}

	protected virtual void SyncForceFactorSlider()
	{
		if (forceFactorSlider != null)
		{
			forceFactorSlider.value = _forceFactor;
		}
		if (forceFactorSliderAlt != null)
		{
			forceFactorSliderAlt.value = _forceFactor;
		}
	}

	protected virtual void InitMaxForceSlider()
	{
		if (maxForceSlider != null)
		{
			maxForceSlider.onValueChanged.AddListener(delegate
			{
				maxForce = maxForceSlider.value;
			});
			maxForce = maxForceSlider.value;
		}
	}

	protected virtual void SyncMaxForceSlider()
	{
		if (maxForceSlider != null)
		{
			maxForceSlider.value = _maxForce;
		}
	}

	protected virtual void InitTorqueFactorSlider()
	{
		if (torqueFactorSliderAlt != null)
		{
			torqueFactorSliderAlt.onValueChanged.AddListener(delegate
			{
				torqueFactor = torqueFactorSliderAlt.value;
			});
		}
		if (torqueFactorSlider != null)
		{
			torqueFactorSlider.onValueChanged.AddListener(delegate
			{
				torqueFactor = torqueFactorSlider.value;
			});
			torqueFactor = torqueFactorSlider.value;
		}
	}

	protected virtual void SyncTorqueFactorSlider()
	{
		if (torqueFactorSlider != null)
		{
			torqueFactorSlider.value = _torqueFactor;
		}
		if (torqueFactorSliderAlt != null)
		{
			torqueFactorSliderAlt.value = _torqueFactor;
		}
	}

	protected virtual void InitMaxTorqueSlider()
	{
		if (maxTorqueSlider != null)
		{
			maxTorqueSlider.onValueChanged.AddListener(delegate
			{
				maxTorque = maxTorqueSlider.value;
			});
			maxTorque = maxTorqueSlider.value;
		}
	}

	protected virtual void SyncMaxTorqueSlider()
	{
		if (maxTorqueSlider != null)
		{
			maxTorqueSlider.value = _maxTorque;
		}
	}

	protected virtual void InitForceQuicknessSlider()
	{
		if (forceQuicknessSliderAlt != null)
		{
			forceQuicknessSliderAlt.onValueChanged.AddListener(delegate
			{
				forceQuickness = forceQuicknessSliderAlt.value;
			});
		}
		if (forceQuicknessSlider != null)
		{
			forceQuicknessSlider.onValueChanged.AddListener(delegate
			{
				forceQuickness = forceQuicknessSlider.value;
			});
			forceQuickness = forceQuicknessSlider.value;
		}
	}

	protected virtual void SyncForceQuicknessSlider()
	{
		if (forceQuicknessSlider != null)
		{
			forceQuicknessSlider.value = _forceQuickness;
		}
		if (forceQuicknessSliderAlt != null)
		{
			forceQuicknessSliderAlt.value = _forceQuickness;
		}
	}

	protected virtual void InitTorqueQuicknessSlider()
	{
		if (torqueQuicknessSliderAlt != null)
		{
			torqueQuicknessSliderAlt.onValueChanged.AddListener(delegate
			{
				torqueQuickness = torqueQuicknessSliderAlt.value;
			});
		}
		if (torqueQuicknessSlider != null)
		{
			torqueQuicknessSlider.onValueChanged.AddListener(delegate
			{
				torqueQuickness = torqueQuicknessSlider.value;
			});
			torqueQuickness = torqueQuicknessSlider.value;
		}
	}

	protected virtual void SyncTorqueQuicknessSlider()
	{
		if (torqueQuicknessSlider != null)
		{
			torqueQuicknessSlider.value = _torqueQuickness;
		}
		if (torqueQuicknessSliderAlt != null)
		{
			torqueQuicknessSliderAlt.value = _torqueQuickness;
		}
	}

	protected virtual void SetReceiverAtomNames()
	{
		if (!(receiverAtomSelectionPopup != null) || !(SuperController.singleton != null))
		{
			return;
		}
		List<string> atomUIDsWithForceReceivers = SuperController.singleton.GetAtomUIDsWithForceReceivers();
		if (atomUIDsWithForceReceivers == null)
		{
			receiverAtomSelectionPopup.numPopupValues = 1;
			receiverAtomSelectionPopup.setPopupValue(0, "None");
			return;
		}
		receiverAtomSelectionPopup.numPopupValues = atomUIDsWithForceReceivers.Count + 1;
		receiverAtomSelectionPopup.setPopupValue(0, "None");
		for (int i = 0; i < atomUIDsWithForceReceivers.Count; i++)
		{
			receiverAtomSelectionPopup.setPopupValue(i + 1, atomUIDsWithForceReceivers[i]);
		}
	}

	protected virtual void SetReceiverAtomAndReceiverNames()
	{
		if (!(receiverAtomAndReceiverSelectionPopup != null) || !(SuperController.singleton != null))
		{
			return;
		}
		string[] forceReceiverNames = SuperController.singleton.forceReceiverNames;
		if (forceReceiverNames == null)
		{
			receiverAtomAndReceiverSelectionPopup.numPopupValues = 1;
			receiverAtomAndReceiverSelectionPopup.setPopupValue(0, "None");
			return;
		}
		receiverAtomAndReceiverSelectionPopup.numPopupValues = forceReceiverNames.Length + 1;
		receiverAtomAndReceiverSelectionPopup.setPopupValue(0, "None");
		for (int i = 0; i < forceReceiverNames.Length; i++)
		{
			receiverAtomAndReceiverSelectionPopup.setPopupValue(i + 1, forceReceiverNames[i]);
		}
	}

	protected virtual void onReceiverNamesChanged(List<string> rcvrNames)
	{
		if (!(receiverSelectionPopup != null))
		{
			return;
		}
		if (rcvrNames == null)
		{
			receiverSelectionPopup.numPopupValues = 1;
			receiverSelectionPopup.setPopupValue(0, "None");
			return;
		}
		receiverSelectionPopup.numPopupValues = rcvrNames.Count + 1;
		receiverSelectionPopup.setPopupValue(0, "None");
		for (int i = 0; i < rcvrNames.Count; i++)
		{
			receiverSelectionPopup.setPopupValue(i + 1, rcvrNames[i]);
		}
	}

	protected virtual void InitUI()
	{
		InitOnToggle();
		InitUseForceToggle();
		InitUseTorqueToggle();
		InitForceFactorSlider();
		InitTorqueFactorSlider();
		InitMaxForceSlider();
		InitMaxTorqueSlider();
		InitForceQuicknessSlider();
		InitTorqueQuicknessSlider();
		if (receiverAtomAndReceiverSelectionPopup != null)
		{
			UIPopup uIPopup = receiverAtomAndReceiverSelectionPopup;
			uIPopup.onOpenPopupHandlers = (UIPopup.OnOpenPopup)Delegate.Combine(uIPopup.onOpenPopupHandlers, new UIPopup.OnOpenPopup(SetReceiverAtomAndReceiverNames));
			UIPopup uIPopup2 = receiverAtomAndReceiverSelectionPopup;
			uIPopup2.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup2.onValueChangeHandlers, new UIPopup.OnValueChange(SetForceReceiver));
		}
		if (receiverAtomSelectionPopup != null)
		{
			UIPopup uIPopup3 = receiverAtomSelectionPopup;
			uIPopup3.onOpenPopupHandlers = (UIPopup.OnOpenPopup)Delegate.Combine(uIPopup3.onOpenPopupHandlers, new UIPopup.OnOpenPopup(SetReceiverAtomNames));
			UIPopup uIPopup4 = receiverAtomSelectionPopup;
			uIPopup4.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup4.onValueChangeHandlers, new UIPopup.OnValueChange(SetForceReceiverAtom));
		}
		if (receiverSelectionPopup != null)
		{
			UIPopup uIPopup5 = receiverSelectionPopup;
			uIPopup5.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup5.onValueChangeHandlers, new UIPopup.OnValueChange(SetForceReceiverObject));
		}
		if ((bool)linkLineMaterial)
		{
			linkLineDrawer = new LineDrawer(linkLineMaterial);
		}
		if ((bool)forceLineMaterial)
		{
			forceLineDrawer = new LineDrawer(2, forceLineMaterial);
		}
		if ((bool)targetForceLineMaterial)
		{
			targetForceLineDrawer = new LineDrawer(6, targetForceLineMaterial);
		}
		if ((bool)rawForceLineMaterial)
		{
			rawForceLineDrawer = new LineDrawer(6, rawForceLineMaterial);
		}
	}

	protected virtual void Awake()
	{
		InitUI();
	}

	protected virtual void Start()
	{
		if ((bool)_receiver)
		{
			RB = _receiver.GetComponent<Rigidbody>();
		}
	}

	public virtual void SetForceReceiverAtom(string atomUID)
	{
		if (SuperController.singleton != null)
		{
			Atom atomByUid = SuperController.singleton.GetAtomByUid(atomUID);
			if (atomByUid != null)
			{
				receiverAtomUID = atomUID;
				List<string> forceReceiverNamesInAtom = SuperController.singleton.GetForceReceiverNamesInAtom(receiverAtomUID);
				onReceiverNamesChanged(forceReceiverNamesInAtom);
				receiverSelectionPopup.currentValue = "None";
			}
			else
			{
				onReceiverNamesChanged(null);
			}
		}
	}

	public virtual void SetForceReceiverObject(string objectName)
	{
		if (receiverAtomUID != null && SuperController.singleton != null)
		{
			receiver = SuperController.singleton.ReceiverNameToForceReceiver(receiverAtomUID + ":" + objectName);
		}
	}

	public virtual void SetForceReceiver(string receiverName)
	{
		if (!(SuperController.singleton != null))
		{
			return;
		}
		ForceReceiver forceReceiver = SuperController.singleton.ReceiverNameToForceReceiver(receiverName);
		if (forceReceiver != null)
		{
			if (receiverAtomSelectionPopup != null && forceReceiver.containingAtom != null)
			{
				receiverAtomSelectionPopup.currentValue = forceReceiver.containingAtom.uid;
			}
			if (receiverSelectionPopup != null)
			{
				receiverSelectionPopup.currentValue = forceReceiver.name;
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
		receiver = forceReceiver;
	}

	public void SelectForceReceiver(ForceReceiver rcvr)
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

	public void SelectForceReceiverFromScene()
	{
		SuperController.singleton.SelectModeForceReceivers(SelectForceReceiver);
	}

	protected virtual void SetTargetForcePercent(float forcePercent)
	{
		targetForcePercent = Mathf.Clamp(forcePercent, -1f, 1f);
		if (useForce)
		{
			rawForce = forceDirection * targetForcePercent * _forceFactor;
			if (rawForce.magnitude > _maxForce)
			{
				targetForce = Vector3.ClampMagnitude(rawForce, _maxForce);
			}
			else
			{
				targetForce = rawForce;
			}
		}
		if (useTorque)
		{
			rawTorque = torqueDirection * targetForcePercent * _torqueFactor;
			if (rawTorque.magnitude > _maxTorque)
			{
				targetTorque = Vector3.ClampMagnitude(rawTorque, _maxTorque);
			}
			else
			{
				targetTorque = rawTorque;
			}
		}
	}

	protected virtual void ApplyForce()
	{
		float fixedDeltaTime = Time.fixedDeltaTime;
		float num = ((!(TimeControl.singleton != null) || !TimeControl.singleton.compensateFixedTimestep) ? 1f : (1f / Time.timeScale));
		currentForce = Vector3.Lerp(currentForce, targetForce, fixedDeltaTime * _forceQuickness);
		currentTorque = Vector3.Lerp(currentTorque, targetTorque, fixedDeltaTime * _torqueQuickness);
		if ((bool)RB && on && (!SuperController.singleton || !SuperController.singleton.freezeAnimation))
		{
			if (useForce)
			{
				appliedForce = currentForce * num;
				RB.AddForce(appliedForce, ForceMode.Force);
			}
			if (useTorque)
			{
				appliedTorque = currentTorque * num;
				RB.AddTorque(appliedTorque, ForceMode.Force);
				RB.maxAngularVelocity = 20f;
			}
		}
	}

	protected virtual Vector3 AxisToVector(AxisName axis)
	{
		return axis switch
		{
			AxisName.X => base.transform.right, 
			AxisName.NegX => -base.transform.right, 
			AxisName.Y => base.transform.up, 
			AxisName.NegY => -base.transform.up, 
			AxisName.Z => base.transform.forward, 
			AxisName.NegZ => -base.transform.forward, 
			_ => Vector3.zero, 
		};
	}

	protected virtual Vector3 AxisToUpVector(AxisName axis)
	{
		return axis switch
		{
			AxisName.X => base.transform.up, 
			AxisName.NegX => base.transform.up, 
			AxisName.Y => base.transform.forward, 
			AxisName.NegY => base.transform.forward, 
			AxisName.Z => base.transform.up, 
			AxisName.NegZ => base.transform.up, 
			_ => Vector3.zero, 
		};
	}

	protected virtual Vector3 getDrawTorque(Vector3 trq)
	{
		return torqueAxis switch
		{
			AxisName.X => Quaternion.FromToRotation(-base.transform.right, AxisToVector(forceAxis)), 
			AxisName.NegX => Quaternion.FromToRotation(-base.transform.right, AxisToVector(forceAxis)), 
			AxisName.Y => Quaternion.FromToRotation(base.transform.up, AxisToVector(forceAxis)), 
			AxisName.NegY => Quaternion.FromToRotation(-base.transform.up, AxisToVector(forceAxis)), 
			AxisName.Z => Quaternion.FromToRotation(base.transform.forward, AxisToVector(forceAxis)), 
			AxisName.NegZ => Quaternion.FromToRotation(-base.transform.forward, AxisToVector(forceAxis)), 
			_ => Quaternion.identity, 
		} * trq;
	}

	protected virtual void FixedUpdate()
	{
		ApplyForce();
	}

	protected virtual void Update()
	{
		if (useForce)
		{
			forceDirection = AxisToVector(forceAxis);
		}
		if (useTorque)
		{
			torqueDirection = AxisToVector(torqueAxis);
		}
		if (_on && _receiver != null && drawLines)
		{
			Vector3 vector = AxisToVector(forceAxis);
			Vector3 drawTorque = getDrawTorque(AxisToVector(torqueAxis));
			Vector3 vector2 = AxisToUpVector(forceAxis);
			if (linkLineDrawer != null)
			{
				linkLineDrawer.SetLinePoints(base.transform.position, receiver.transform.position);
				linkLineDrawer.Draw(base.gameObject.layer);
			}
			if (forceLineDrawer != null)
			{
				Vector3 vector3 = base.transform.position + vector2 * lineOffset;
				forceLineDrawer.SetLinePoints(0, vector3, vector3 + currentForce * linesScale);
				vector3 += vector2 * lineSpacing * 5f;
				Vector3 drawTorque2 = getDrawTorque(currentTorque);
				forceLineDrawer.SetLinePoints(1, vector3, vector3 + drawTorque2 * linesScale * torqueLineMult);
				targetForceLineDrawer.Draw(base.gameObject.layer);
				forceLineDrawer.Draw(base.gameObject.layer);
			}
			if (targetForceLineDrawer != null)
			{
				Vector3 vector4 = base.transform.position + vector2 * (lineOffset + lineSpacing);
				targetForceLineDrawer.SetLinePoints(0, vector4, vector4 + targetForce * linesScale);
				Vector3 vector5 = vector * _maxForce * linesScale;
				Vector3 vector6 = vector4 + vector5;
				targetForceLineDrawer.SetLinePoints(1, vector6 - vector2 * lineSpacing, vector6 + vector2 * lineSpacing);
				vector6 = vector4 - vector5;
				targetForceLineDrawer.SetLinePoints(2, vector6 - vector2 * lineSpacing, vector6 + vector2 * lineSpacing);
				vector4 += vector2 * lineSpacing * 5f;
				Vector3 drawTorque3 = getDrawTorque(targetTorque);
				targetForceLineDrawer.SetLinePoints(3, vector4, vector4 + drawTorque3 * linesScale * torqueLineMult);
				vector5 = drawTorque * _maxTorque * linesScale * torqueLineMult;
				vector6 = vector4 + vector5;
				targetForceLineDrawer.SetLinePoints(4, vector6 - vector2 * lineSpacing, vector6 + vector2 * lineSpacing);
				vector6 = vector4 - vector5;
				targetForceLineDrawer.SetLinePoints(5, vector6 - vector2 * lineSpacing, vector6 + vector2 * lineSpacing);
				targetForceLineDrawer.Draw(base.gameObject.layer);
			}
			if (rawForceLineDrawer != null)
			{
				Vector3 vector7 = base.transform.position + vector2 * (lineOffset + lineSpacing * 2f);
				rawForceLineDrawer.SetLinePoints(0, vector7, vector7 + rawForce * linesScale);
				Vector3 vector8 = vector * _forceFactor * linesScale;
				Vector3 vector9 = vector7 + vector8;
				rawForceLineDrawer.SetLinePoints(1, vector9 - vector2 * lineSpacing, vector9 + vector2 * lineSpacing);
				vector9 = vector7 - vector8;
				rawForceLineDrawer.SetLinePoints(2, vector9 - vector2 * lineSpacing, vector9 + vector2 * lineSpacing);
				vector7 += vector2 * lineSpacing * 5f;
				Vector3 drawTorque4 = getDrawTorque(rawTorque);
				rawForceLineDrawer.SetLinePoints(3, vector7, vector7 + drawTorque4 * linesScale * torqueLineMult);
				vector8 = drawTorque * _torqueFactor * linesScale * torqueLineMult;
				vector9 = vector7 + vector8;
				rawForceLineDrawer.SetLinePoints(4, vector9 - vector2 * lineSpacing, vector9 + vector2 * lineSpacing);
				vector9 = vector7 - vector8;
				rawForceLineDrawer.SetLinePoints(5, vector9 - vector2 * lineSpacing, vector9 + vector2 * lineSpacing);
				rawForceLineDrawer.Draw(base.gameObject.layer);
			}
		}
	}
}
