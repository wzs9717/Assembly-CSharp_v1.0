using System;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class Atom : MonoBehaviour
{
	public Text idText;

	public Text idTextAlt;

	public string category;

	public string type;

	public Toggle onToggle;

	public Toggle onToggleAlt;

	public Button resetButton;

	public Button resetPhysicalButton;

	public Button resetAppearanceButton;

	public Button removeButton;

	public Button saveAppearancePresetButton;

	public Button savePhysicalPresetButton;

	public Button loadAppearancePresetButton;

	public Button loadPhysicalPresetButton;

	public Transform[] onToggleObjects;

	[SerializeField]
	protected bool _useRigidbodyInterpolation = true;

	protected bool _startingOn;

	[SerializeField]
	protected bool _on = true;

	public Toggle collisionEnabledToggle;

	private bool _startingCollisionEnabled;

	[SerializeField]
	protected bool _collisionEnabled = true;

	[SerializeField]
	private Atom _parentAtom;

	public UIPopup parentAtomSelectionPopup;

	public Transform reParentObject;

	public Transform childAtomContainer;

	[SerializeField]
	private string _uid;

	public Text descriptionText;

	public Text descriptionTextAlt;

	[SerializeField]
	private string _description;

	private Transform[] masterControllerCorners;

	[SerializeField]
	private FreeControllerV3 _masterController;

	public float extentPadding = 0.3f;

	public bool alwaysShowExtents = true;

	private float extentLowX;

	private float extentHighX;

	private float extentLowY;

	private float extentHighY;

	private float extentLowZ;

	private float extentHighZ;

	private Vector3 extentlll;

	private Vector3 extentllh;

	private Vector3 extentlhl;

	private Vector3 extentlhh;

	private Vector3 extenthll;

	private Vector3 extenthlh;

	private Vector3 extenthhl;

	private Vector3 extenthhh;

	private bool wasInit;

	private Vector3 reParentObjectStartingPosition;

	private Quaternion reParentObjectStartingRotation;

	private Vector3 childAtomContainerStartingPosition;

	private Quaternion childAtomContainerStartingRotation;

	private List<JSONStorable> _storables;

	private Dictionary<string, JSONStorable> _storableById;

	protected JSONClass lastRestoredData;

	protected bool lastRestorePhysical;

	protected bool lastRestoreAppearance;

	protected bool saveIncludePhysical;

	protected bool saveIncludeAppearance;

	protected string loadedName;

	protected string loadedPhysicalName;

	protected string loadedAppearanceName;

	private ForceReceiver[] _forceReceivers;

	private ForceProducerV2[] _forceProducers;

	private FreeControllerV3[] _freeControllers;

	private Rigidbody[] _rigidbodies;

	private Rigidbody[] _linkableRigidbodies;

	private Rigidbody[] _realRigidbodies;

	private AnimationPattern[] _animationPatterns;

	private AnimationStep[] _animationSteps;

	private Animator[] _animators;

	private Canvas[] _canvases;

	private DAZPhysicsMesh[] _dazPhysicsMeshes;

	private AutoCollider[] _autoColliders;

	public bool useRigidbodyInterpolation
	{
		get
		{
			return _useRigidbodyInterpolation;
		}
		set
		{
			if (_useRigidbodyInterpolation != value)
			{
				_useRigidbodyInterpolation = value;
				if (Application.isPlaying)
				{
					SyncRigidbodyInterpolation();
				}
			}
		}
	}

	public bool on
	{
		get
		{
			return _on;
		}
		set
		{
			if (_on == value)
			{
				return;
			}
			_on = value;
			if (onToggle != null)
			{
				onToggle.isOn = _on;
			}
			if (onToggleAlt != null)
			{
				onToggleAlt.isOn = _on;
			}
			if (onToggleObjects != null)
			{
				Transform[] array = onToggleObjects;
				foreach (Transform transform in array)
				{
					transform.gameObject.SetActive(_on);
				}
			}
			SyncRigidbodyInterpolation();
		}
	}

	public bool collisionEnabled
	{
		get
		{
			return _collisionEnabled;
		}
		set
		{
			if (_collisionEnabled != value)
			{
				_collisionEnabled = value;
				SyncCollisionEnabled();
			}
		}
	}

	public Atom parentAtom
	{
		get
		{
			return _parentAtom;
		}
		set
		{
			if (!(_parentAtom != value))
			{
				return;
			}
			_parentAtom = value;
			if (!(reParentObject != null))
			{
				return;
			}
			if (_parentAtom != null)
			{
				if (_parentAtom.childAtomContainer != null)
				{
					reParentObject.parent = _parentAtom.childAtomContainer;
				}
				else
				{
					reParentObject.parent = _parentAtom.transform;
				}
			}
			else
			{
				reParentObject.parent = base.transform;
			}
		}
	}

	public string uid
	{
		get
		{
			return _uid;
		}
		set
		{
			_uid = value;
			if (idText != null)
			{
				idText.text = _uid;
			}
			if (idTextAlt != null)
			{
				idTextAlt.text = _uid;
			}
		}
	}

	public string description
	{
		get
		{
			return _description;
		}
		set
		{
			_description = value;
			SyncDescriptionText();
		}
	}

	public FreeControllerV3 masterController
	{
		get
		{
			return _masterController;
		}
		set
		{
			if (_masterController != value)
			{
				_masterController = value;
				SyncMasterControllerCorners();
			}
		}
	}

	public ForceReceiver[] forceReceivers
	{
		get
		{
			Init();
			return _forceReceivers;
		}
	}

	public ForceProducerV2[] forceProducers
	{
		get
		{
			Init();
			return _forceProducers;
		}
	}

	public FreeControllerV3[] freeControllers
	{
		get
		{
			Init();
			return _freeControllers;
		}
	}

	public Rigidbody[] rigidbodies
	{
		get
		{
			Init();
			return _rigidbodies;
		}
	}

	public Rigidbody[] linkableRigidbodies
	{
		get
		{
			Init();
			return _linkableRigidbodies;
		}
	}

	public Rigidbody[] realRigidbodies
	{
		get
		{
			Init();
			return _realRigidbodies;
		}
	}

	public AnimationPattern[] animationPatterns
	{
		get
		{
			Init();
			return _animationPatterns;
		}
	}

	public AnimationStep[] animationSteps
	{
		get
		{
			Init();
			return _animationSteps;
		}
	}

	public Animator[] animators
	{
		get
		{
			Init();
			return _animators;
		}
	}

	public Canvas[] canvases
	{
		get
		{
			Init();
			return _canvases;
		}
	}

	public DAZPhysicsMesh[] dazPhysicsMeshes
	{
		get
		{
			Init();
			return _dazPhysicsMeshes;
		}
	}

	public AutoCollider[] autoColliders
	{
		get
		{
			Init();
			return _autoColliders;
		}
	}

	protected void SyncRigidbodyInterpolation()
	{
		if (_realRigidbodies != null)
		{
			Rigidbody[] array = _realRigidbodies;
			foreach (Rigidbody rigidbody in array)
			{
				RigidbodyAttributes component = rigidbody.GetComponent<RigidbodyAttributes>();
				if (component != null)
				{
					component.useInterpolation = _on && _useRigidbodyInterpolation;
				}
				else if (_on && _useRigidbodyInterpolation)
				{
					if (!rigidbody.isKinematic)
					{
						rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
					}
				}
				else
				{
					rigidbody.interpolation = RigidbodyInterpolation.None;
				}
			}
		}
		if (_dazPhysicsMeshes != null)
		{
			DAZPhysicsMesh[] array2 = _dazPhysicsMeshes;
			foreach (DAZPhysicsMesh dAZPhysicsMesh in array2)
			{
				dAZPhysicsMesh.useInterpolation = useRigidbodyInterpolation;
			}
		}
		if (_autoColliders != null)
		{
			AutoCollider[] array3 = _autoColliders;
			foreach (AutoCollider autoCollider in array3)
			{
				autoCollider.useInterpolation = useRigidbodyInterpolation;
			}
		}
	}

	protected void SyncCollisionEnabled()
	{
		if (_realRigidbodies != null)
		{
			Rigidbody[] array = _realRigidbodies;
			foreach (Rigidbody rigidbody in array)
			{
				rigidbody.detectCollisions = _collisionEnabled;
			}
		}
		if (_dazPhysicsMeshes != null)
		{
			DAZPhysicsMesh[] array2 = _dazPhysicsMeshes;
			foreach (DAZPhysicsMesh dAZPhysicsMesh in array2)
			{
				dAZPhysicsMesh.collisionEnabled = _collisionEnabled;
			}
		}
		if (_autoColliders != null)
		{
			AutoCollider[] array3 = _autoColliders;
			foreach (AutoCollider autoCollider in array3)
			{
				autoCollider.collisionEnabled = _collisionEnabled;
			}
		}
		if (collisionEnabledToggle != null)
		{
			collisionEnabledToggle.isOn = _collisionEnabled;
		}
	}

	protected void SyncDescriptionText()
	{
		if (descriptionText != null)
		{
			descriptionText.text = _description;
		}
		if (descriptionTextAlt != null)
		{
			descriptionTextAlt.text = _description;
		}
	}

	private void SyncMasterControllerCorners()
	{
		List<Transform> list = new List<Transform>();
		if (_masterController != null)
		{
			foreach (Transform item in _masterController.transform)
			{
				list.Add(item);
			}
		}
		masterControllerCorners = list.ToArray();
	}

	private void walkAndGetComponents(Transform t, List<ForceReceiver> receivers, List<ForceProducerV2> producers, List<FreeControllerV3> controllers, List<Rigidbody> rbs, List<Rigidbody> linkablerbs, List<Rigidbody> realrbs, List<AnimationPattern> ans, List<AnimationStep> asts, List<Animator> anms, List<JSONStorable> jss, List<DAZPhysicsMesh> dpms, List<DAZCharacter> dcs, List<Canvas> cvs, List<AutoCollider> acs, bool insideAutoCollider)
	{
		Rigidbody component = t.GetComponent<Rigidbody>();
		if (component != null)
		{
			rbs.Add(component);
		}
		ForceReceiver component2 = t.GetComponent<ForceReceiver>();
		if (component2 != null)
		{
			receivers.Add(component2);
		}
		ForceProducerV2 component3 = t.GetComponent<ForceProducerV2>();
		if (component3 != null)
		{
			producers.Add(component3);
		}
		FreeControllerV3 component4 = t.GetComponent<FreeControllerV3>();
		if (component4 != null)
		{
			controllers.Add(component4);
		}
		DAZPhysicsMesh component5 = t.GetComponent<DAZPhysicsMesh>();
		if (component5 != null)
		{
			dpms.Add(component5);
		}
		DAZCharacter component6 = t.GetComponent<DAZCharacter>();
		if (component6 != null)
		{
			dcs.Add(component6);
		}
		if (component != null && component4 == null && !insideAutoCollider)
		{
			realrbs.Add(component);
		}
		if (component != null && (component2 != null || component4 != null))
		{
			linkablerbs.Add(component);
		}
		AnimationPattern component7 = t.GetComponent<AnimationPattern>();
		if (component7 != null)
		{
			ans.Add(component7);
		}
		AnimationStep component8 = t.GetComponent<AnimationStep>();
		if (component8 != null)
		{
			asts.Add(component8);
		}
		Animator component9 = t.GetComponent<Animator>();
		if (component9 != null)
		{
			anms.Add(component9);
		}
		Canvas component10 = t.GetComponent<Canvas>();
		if (component10 != null)
		{
			cvs.Add(component10);
		}
		JSONStorable[] components = t.GetComponents<JSONStorable>();
		if (components != null)
		{
			JSONStorable[] array = components;
			foreach (JSONStorable item in array)
			{
				jss.Add(item);
			}
		}
		AutoCollider component11 = t.GetComponent<AutoCollider>();
		bool flag = false;
		if (component11 != null)
		{
			flag = true;
			acs.Add(component11);
		}
		foreach (Transform item2 in t)
		{
			if (!item2.GetComponent<Atom>())
			{
				walkAndGetComponents(item2, receivers, producers, controllers, rbs, linkablerbs, realrbs, ans, asts, anms, jss, dpms, dcs, cvs, acs, insideAutoCollider || flag);
			}
		}
	}

	private void Init()
	// !!
	{
		if (wasInit)
		{
			return;
		}
		wasInit = true;
		List<ForceReceiver> list = new List<ForceReceiver>();
		List<ForceProducerV2> list2 = new List<ForceProducerV2>();
		List<FreeControllerV3> list3 = new List<FreeControllerV3>();
		List<Rigidbody> list4 = new List<Rigidbody>();
		List<Rigidbody> list5 = new List<Rigidbody>();
		List<Rigidbody> list6 = new List<Rigidbody>();
		List<AnimationPattern> list7 = new List<AnimationPattern>();
		List<AnimationStep> list8 = new List<AnimationStep>();
		List<Animator> list9 = new List<Animator>();
		List<Canvas> list10 = new List<Canvas>();
		List<JSONStorable> list11 = new List<JSONStorable>();
		List<DAZPhysicsMesh> list12 = new List<DAZPhysicsMesh>();
		List<DAZCharacter> list13 = new List<DAZCharacter>();
		List<AutoCollider> list14 = new List<AutoCollider>();
		walkAndGetComponents(base.transform, list, list2, list3, list4, list5, list6, list7, list8, list9, list11, list12, list13, list10, list14, insideAutoCollider: false);
		_forceReceivers = list.ToArray();
		_forceProducers = list2.ToArray();
		_freeControllers = list3.ToArray();
		_rigidbodies = list4.ToArray();
		_linkableRigidbodies = list5.ToArray();
		_realRigidbodies = list6.ToArray();
		_animationPatterns = list7.ToArray();
		_animationSteps = list8.ToArray();
		_animators = list9.ToArray();
		_canvases = list10.ToArray();
		_storables = list11;
		_dazPhysicsMeshes = list12.ToArray();
		_autoColliders = list14.ToArray();
		_storableById = new Dictionary<string, JSONStorable>();
		foreach (JSONStorable storable in _storables)
		{
			if (!storable.exclude)
			{
				if (_storableById.ContainsKey(storable.storeId))
				{
					Debug.LogError("Found duplicate storable uid " + storable.storeId + " in atom " + uid);
				}
				else
				{
					_storableById.Add(storable.storeId, storable);
				}
			}
		}
		AnimationPattern[] array = _animationPatterns;
		foreach (AnimationPattern animationPattern in array)
		{
			animationPattern.containingAtom = this;
		}
		foreach (DAZCharacter item in list13)
		{
			item.containingAtom = this;
		}
		AnimationStep[] array2 = _animationSteps;
		foreach (AnimationStep animationStep in array2)
		{
			animationStep.containingAtom = this;
		}
		FreeControllerV3[] array3 = _freeControllers;
		foreach (FreeControllerV3 freeControllerV in array3)
		{
			freeControllerV.containingAtom = this;
		}
		ForceReceiver[] array4 = _forceReceivers;
		foreach (ForceReceiver forceReceiver in array4)
		{
			forceReceiver.containingAtom = this;
		}
		ForceProducerV2[] array5 = _forceProducers;
		foreach (ForceProducerV2 forceProducerV in array5)
		{
			forceProducerV.containingAtom = this;
		}
		SyncRigidbodyInterpolation();
		SyncCollisionEnabled();
		SyncDescriptionText();
	}

	public void Store(JSONArray atoms, bool includePhysical = true, bool includeAppearance = true)
	{
		JSONClass jSONClass = new JSONClass();
		jSONClass["id"] = uid;
		if (onToggle != null && includePhysical)
		{
			jSONClass["on"].AsBool = onToggle.isOn;
		}
		if (collisionEnabledToggle != null && includePhysical)
		{
			jSONClass["collisionEnabled"].AsBool = collisionEnabledToggle.isOn;
		}
		if (type != null)
		{
			jSONClass["type"] = type;
		}
		else
		{
			Debug.LogWarning("Atom " + uid + " does not have a type set");
		}
		if (parentAtom != null)
		{
			jSONClass["parentAtom"] = parentAtom.uid;
		}
		if (reParentObject != null && includePhysical)
		{
			Vector3 position = reParentObject.position;
			jSONClass["position"]["x"].AsFloat = position.x;
			jSONClass["position"]["y"].AsFloat = position.y;
			jSONClass["position"]["z"].AsFloat = position.z;
			Vector3 eulerAngles = reParentObject.eulerAngles;
			jSONClass["rotation"]["x"].AsFloat = eulerAngles.x;
			jSONClass["rotation"]["y"].AsFloat = eulerAngles.y;
			jSONClass["rotation"]["z"].AsFloat = eulerAngles.z;
		}
		if (childAtomContainer != null && includePhysical)
		{
			Vector3 position2 = childAtomContainer.position;
			jSONClass["containerPosition"]["x"].AsFloat = position2.x;
			jSONClass["containerPosition"]["y"].AsFloat = position2.y;
			jSONClass["containerPosition"]["z"].AsFloat = position2.z;
			Vector3 eulerAngles2 = childAtomContainer.eulerAngles;
			jSONClass["containerRotation"]["x"].AsFloat = eulerAngles2.x;
			jSONClass["containerRotation"]["y"].AsFloat = eulerAngles2.y;
			jSONClass["containerRotation"]["z"].AsFloat = eulerAngles2.z;
		}
		atoms.Add(jSONClass);
		jSONClass["storables"] = new JSONArray();
		foreach (JSONStorable storable in _storables)
		{
			if (!storable.exclude && (!storable.onlyStoreIfActive || storable.gameObject.activeInHierarchy))
			{
				JSONClass jSON = storable.GetJSON(includePhysical, includeAppearance);
				if (storable.needsStore)
				{
					jSONClass["storables"].Add(jSON);
				}
			}
		}
	}

	public void RestoreForceInitialize()
	{
		Init();
		on = true;
	}

	public void RestoreStartingOn()
	{
		on = _startingOn;
	}

	public void RestoreTransform(JSONClass jc)
	{
		Init();
		if (reParentObject != null)
		{
			if (jc["position"] != null)
			{
				Vector3 position = reParentObject.position;
				if (jc["position"]["x"] != null)
				{
					position.x = jc["position"]["x"].AsFloat;
				}
				if (jc["position"]["y"] != null)
				{
					position.y = jc["position"]["y"].AsFloat;
				}
				if (jc["position"]["z"] != null)
				{
					position.z = jc["position"]["z"].AsFloat;
				}
				reParentObject.position = position;
			}
			else
			{
				reParentObject.position = reParentObjectStartingPosition;
			}
			if (jc["rotation"] != null)
			{
				Vector3 eulerAngles = reParentObject.eulerAngles;
				if (jc["rotation"]["x"] != null)
				{
					eulerAngles.x = jc["rotation"]["x"].AsFloat;
				}
				if (jc["rotation"]["y"] != null)
				{
					eulerAngles.y = jc["rotation"]["y"].AsFloat;
				}
				if (jc["rotation"]["z"] != null)
				{
					eulerAngles.z = jc["rotation"]["z"].AsFloat;
				}
				reParentObject.eulerAngles = eulerAngles;
			}
			else
			{
				reParentObject.rotation = reParentObjectStartingRotation;
			}
		}
		if (!(childAtomContainer != null))
		{
			return;
		}
		if (jc["containerPosition"] != null)
		{
			Vector3 position2 = childAtomContainer.position;
			if (jc["containerPosition"]["x"] != null)
			{
				position2.x = jc["containerPosition"]["x"].AsFloat;
			}
			if (jc["containerPosition"]["y"] != null)
			{
				position2.y = jc["containerPosition"]["y"].AsFloat;
			}
			if (jc["containerPosition"]["z"] != null)
			{
				position2.z = jc["containerPosition"]["z"].AsFloat;
			}
			childAtomContainer.position = position2;
		}
		else
		{
			childAtomContainer.position = childAtomContainerStartingPosition;
		}
		if (jc["containerRotation"] != null)
		{
			Vector3 eulerAngles2 = childAtomContainer.eulerAngles;
			if (jc["containerRotation"]["x"] != null)
			{
				eulerAngles2.x = jc["containerRotation"]["x"].AsFloat;
			}
			if (jc["containerRotation"]["y"] != null)
			{
				eulerAngles2.y = jc["containerRotation"]["y"].AsFloat;
			}
			if (jc["containerRotation"]["z"] != null)
			{
				eulerAngles2.z = jc["containerRotation"]["z"].AsFloat;
			}
			childAtomContainer.eulerAngles = eulerAngles2;
		}
		else
		{
			childAtomContainer.rotation = childAtomContainerStartingRotation;
		}
	}

	public void ClearParentAtom()
	{
		SelectAtomParent(null);
	}

	public void RestoreParentAtom(JSONClass jc)
	{
		if (jc["parentAtom"] != null)
		{
			Atom atomByUid = SuperController.singleton.GetAtomByUid(jc["parentAtom"]);
			SelectAtomParent(atomByUid);
		}
		else
		{
			SelectAtomParent(null);
		}
	}

	public void RestoreFromLast(JSONStorable js)
	{
		if (!(lastRestoredData != null))
		{
			return;
		}
		bool flag = false;
		foreach (JSONClass item in lastRestoredData["storables"].AsArray)
		{
			string text = item["id"];
			if (text == js.storeId)
			{
				flag = true;
				js.RestoreFromJSON(item, lastRestorePhysical, lastRestoreAppearance);
				js.LateRestoreFromJSON(item, lastRestorePhysical, lastRestoreAppearance);
				break;
			}
		}
		if (!flag)
		{
			Debug.LogWarning("Could not find data for store " + js.storeId + " from last restore");
		}
	}

	public void Restore(JSONClass jc, bool restorePhysical = true, bool restoreAppearance = true, bool restoreCore = true)
	{
		Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
		lastRestoredData = jc;
		lastRestoreAppearance = restoreAppearance;
		lastRestorePhysical = restorePhysical;
		if (restoreCore && onToggle != null)
		{
			if (jc["on"] != null)
			{
				onToggle.isOn = jc["on"].AsBool;
			}
			else
			{
				onToggle.isOn = _startingOn;
			}
		}
		foreach (JSONClass item in jc["storables"].AsArray)
		{
			if (_storableById.TryGetValue(item["id"], out var value))
			{
				value.RestoreFromJSON(item, restorePhysical, restoreAppearance);
				if (!dictionary.ContainsKey(item["id"]))
				{
					dictionary.Add(item["id"], value: true);
				}
			}
		}
		foreach (JSONStorable storable in _storables)
		{
			if (!storable.exclude && !dictionary.ContainsKey(storable.storeId))
			{
				JSONClass jc2 = new JSONClass();
				storable.RestoreFromJSON(jc2, restorePhysical, restoreAppearance);
			}
		}
	}

	public void LateRestore(JSONClass jc, bool restorePhysical = true, bool restoreAppearance = true, bool restoreCore = true)
	{
		Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
		if (restoreCore && collisionEnabledToggle != null)
		{
			if (jc["collisionEnabled"] != null)
			{
				collisionEnabledToggle.isOn = jc["collisionEnabled"].AsBool;
			}
			else
			{
				collisionEnabledToggle.isOn = _startingCollisionEnabled;
			}
		}
		foreach (JSONClass item in jc["storables"].AsArray)
		{
			if (_storableById.TryGetValue(item["id"], out var value))
			{
				value.LateRestoreFromJSON(item, restorePhysical, restoreAppearance);
				if (!dictionary.ContainsKey(item["id"]))
				{
					dictionary.Add(item["id"], value: true);
				}
			}
		}
		foreach (JSONStorable storable in _storables)
		{
			if (!storable.exclude && !dictionary.ContainsKey(storable.storeId))
			{
				JSONClass jc2 = new JSONClass();
				storable.LateRestoreFromJSON(jc2, restorePhysical, restoreAppearance);
			}
		}
	}

	public void PreRestore()
	{
		foreach (JSONStorable storable in _storables)
		{
			storable.PreRestore();
		}
	}

	public void PostRestore()
	{
		foreach (JSONStorable storable in _storables)
		{
			storable.PostRestore();
		}
	}

	public void Remove()
	{
		SuperController.singleton.RemoveAtom(this);
	}

	public void Reset()
	{
		JSONClass jc = new JSONClass();
		loadedName = null;
		loadedPhysicalName = null;
		loadedAppearanceName = null;
		PreRestore();
		RestoreTransform(jc);
		RestoreParentAtom(jc);
		Restore(jc);
		LateRestore(jc);
		PostRestore();
	}

	public void ResetPhysical()
	{
		JSONClass jc = new JSONClass();
		loadedName = null;
		loadedPhysicalName = null;
		loadedAppearanceName = null;
		PreRestore();
		RestoreTransform(jc);
		RestoreParentAtom(jc);
		Restore(jc, restorePhysical: true, restoreAppearance: false, restoreCore: false);
		LateRestore(jc, restorePhysical: true, restoreAppearance: false, restoreCore: false);
		PostRestore();
	}

	public void ResetAppearance()
	{
		JSONClass jc = new JSONClass();
		loadedName = null;
		loadedPhysicalName = null;
		loadedAppearanceName = null;
		PreRestore();
		Restore(jc, restorePhysical: false, restoreAppearance: true, restoreCore: false);
		LateRestore(jc, restorePhysical: false, restoreAppearance: true, restoreCore: false);
		PostRestore();
	}

	public void SavePresetDialog(bool includePhysical = false, bool includeAppearance = false)
	{
		saveIncludePhysical = includePhysical;
		saveIncludeAppearance = includeAppearance;
		MultiButtonPanel multiButtonPanel = SuperController.singleton.multiButtonPanel;
		if (multiButtonPanel != null)
		{
			multiButtonPanel.SetButton1Text("Save New");
			multiButtonPanel.showButton1 = true;
			string text = null;
			if (includePhysical && includeAppearance)
			{
				text = loadedName;
			}
			else if (includePhysical)
			{
				text = loadedPhysicalName;
			}
			else if (includeAppearance)
			{
				text = loadedAppearanceName;
			}
			if (text != null && text != string.Empty)
			{
				multiButtonPanel.SetButton2Text("Overwrite Current");
				multiButtonPanel.showButton2 = true;
			}
			else
			{
				multiButtonPanel.showButton2 = false;
			}
			multiButtonPanel.SetButton3Text("Cancel");
			multiButtonPanel.showButton3 = true;
			multiButtonPanel.buttonCallback = SaveConfirm;
			SuperController.singleton.activeUI = SuperController.ActiveUI.MultiButtonPanel;
		}
	}

	public void SaveConfirm(string option)
	{
		if (SuperController.singleton.lastActiveUI == SuperController.ActiveUI.MultiButtonPanel)
		{
			SuperController.singleton.activeUI = SuperController.ActiveUI.None;
		}
		else
		{
			SuperController.singleton.activeUI = SuperController.singleton.lastActiveUI;
		}
		MultiButtonPanel multiButtonPanel = SuperController.singleton.multiButtonPanel;
		multiButtonPanel.gameObject.SetActive(value: false);
		multiButtonPanel.buttonCallback = null;
		string text = null;
		if (option == "Save New")
		{
			int num = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
			if (saveIncludePhysical && saveIncludeAppearance)
			{
				text = (loadedName = SuperController.singleton.savesDir + type + "/full/" + num + ".json");
			}
			else if (saveIncludePhysical)
			{
				text = (loadedPhysicalName = SuperController.singleton.savesDir + type + "/pose/" + num + ".json");
			}
			else if (saveIncludeAppearance)
			{
				text = (loadedAppearanceName = SuperController.singleton.savesDir + type + "/appearance/" + num + ".json");
			}
		}
		else if (option == "Overwrite Current")
		{
			if (saveIncludePhysical && saveIncludeAppearance)
			{
				text = loadedName;
			}
			else if (saveIncludePhysical)
			{
				text = loadedPhysicalName;
			}
			else if (saveIncludeAppearance)
			{
				text = loadedAppearanceName;
			}
		}
		if (text != null && text != string.Empty)
		{
			SuperController.singleton.Save(text, this, saveIncludePhysical, saveIncludeAppearance);
		}
	}

	public void SavePreset(string saveName = "savefile", bool includePhysical = false)
	{
		SuperController.singleton.Save(saveName, this, includePhysical);
	}

	public void LoadPhysicalPresetDialog()
	{
		string text = SuperController.singleton.savesDir + type + "/pose";
		Directory.CreateDirectory(text);
		SuperController.singleton.fileBrowserUI.defaultPath = text;
		SuperController.singleton.fileBrowserUI.Show(LoadPhysicalPreset);
	}

	public void LoadPhysicalPreset(string saveName = "savefile")
	{
		if (!(saveName != string.Empty))
		{
			return;
		}
		StreamReader streamReader = new StreamReader(saveName);
		string aJSON = streamReader.ReadToEnd();
		streamReader.Close();
		JSONNode jSONNode = JSON.Parse(aJSON);
		JSONArray asArray = jSONNode["atoms"].AsArray;
		loadedPhysicalName = saveName;
		JSONClass asObject = asArray[0].AsObject;
		if (asObject != null)
		{
			string text = asObject["type"];
			if (text == type)
			{
				PreRestore();
				RestoreTransform(asObject);
				Restore(asObject, restorePhysical: true, restoreAppearance: false, restoreCore: false);
				LateRestore(asObject, restorePhysical: true, restoreAppearance: false, restoreCore: false);
				PostRestore();
			}
		}
	}

	public void LoadAppearancePresetDialog()
	{
		string text = SuperController.singleton.savesDir + type + "/appearance";
		Directory.CreateDirectory(text);
		SuperController.singleton.fileBrowserUI.defaultPath = text;
		SuperController.singleton.fileBrowserUI.Show(LoadAppearancePreset);
	}

	public void LoadAppearancePreset(string saveName = "savefile")
	{
		if (!(saveName != string.Empty))
		{
			return;
		}
		StreamReader streamReader = new StreamReader(saveName);
		string aJSON = streamReader.ReadToEnd();
		streamReader.Close();
		JSONNode jSONNode = JSON.Parse(aJSON);
		JSONArray asArray = jSONNode["atoms"].AsArray;
		loadedAppearanceName = saveName;
		JSONClass asObject = asArray[0].AsObject;
		if (asObject != null)
		{
			string text = asObject["type"];
			if (text == type)
			{
				Restore(asObject, restorePhysical: false, restoreAppearance: true, restoreCore: false);
				LateRestore(asObject, restorePhysical: false, restoreAppearance: true, restoreCore: false);
			}
		}
	}

	public void SetParentAtomSelectPopupValues()
	{
		if (!(parentAtomSelectionPopup != null) || !(SuperController.singleton != null))
		{
			return;
		}
		List<string> atomUIDs = SuperController.singleton.GetAtomUIDs();
		if (atomUIDs == null)
		{
			parentAtomSelectionPopup.numPopupValues = 1;
			parentAtomSelectionPopup.setPopupValue(0, "None");
			return;
		}
		
		parentAtomSelectionPopup.numPopupValues = atomUIDs.Count + 1;
		parentAtomSelectionPopup.setPopupValue(0, "None");
		for (int i = 0; i < atomUIDs.Count; i++)
		{
			parentAtomSelectionPopup.setPopupValue(i + 1, atomUIDs[i]);
		}
	}

	public bool RegisterAdditionalStorable(JSONStorable js)
	{
		if (js != null && !js.exclude)
		{
			if (!_storableById.ContainsKey(js.storeId))
			{
				_storables.Add(js);
				_storableById.Add(js.storeId, js);
				return true;
			}
			Debug.LogError("Found duplicate storable uid " + js.storeId + " in atom " + uid);
		}
		return false;
	}

	public void UnregisterAdditionalStorable(JSONStorable js)
	{
		if (js != null && _storableById.ContainsKey(js.storeId))
		{
			_storableById.Remove(js.storeId);
		}
	}

	public virtual void SetParentAtom(string atomUID)
	{
		if (SuperController.singleton != null)
		{
			Atom atom = (parentAtom = SuperController.singleton.GetAtomByUid(atomUID));
		}
	}

	public void SelectAtomParent(Atom a)
	{
		if (parentAtomSelectionPopup != null)
		{
			if (a == null)
			{
				parentAtomSelectionPopup.currentValue = "None";
			}
			else
			{
				parentAtomSelectionPopup.currentValue = a.uid;
			}
		}
		parentAtom = a;
	}

	public void SelectAtomParentFromScene()
	{
		SuperController.singleton.SelectModeAtom(SelectAtomParent);
	}

	protected void InitUI()
	{
		if (parentAtomSelectionPopup != null)
		{
			UIPopup uIPopup = parentAtomSelectionPopup;
			uIPopup.onOpenPopupHandlers = (UIPopup.OnOpenPopup)Delegate.Combine(uIPopup.onOpenPopupHandlers, new UIPopup.OnOpenPopup(SetParentAtomSelectPopupValues));
			UIPopup uIPopup2 = parentAtomSelectionPopup;
			uIPopup2.onValueChangeHandlers = (UIPopup.OnValueChange)Delegate.Combine(uIPopup2.onValueChangeHandlers, new UIPopup.OnValueChange(SetParentAtom));
		}
		if (onToggle != null)
		{
			onToggle.isOn = on;
			onToggle.onValueChanged.AddListener(delegate
			{
				on = onToggle.isOn;
			});
		}
		if (onToggleAlt != null)
		{
			onToggleAlt.isOn = on;
			onToggleAlt.onValueChanged.AddListener(delegate
			{
				on = onToggleAlt.isOn;
			});
		}
		if (collisionEnabledToggle != null)
		{
			collisionEnabledToggle.isOn = collisionEnabled;
			collisionEnabledToggle.onValueChanged.AddListener(delegate
			{
				collisionEnabled = collisionEnabledToggle.isOn;
			});
		}
		if (resetButton != null)
		{
			resetButton.onClick.AddListener(delegate
			{
				Reset();
			});
		}
		if (resetPhysicalButton != null)
		{
			resetPhysicalButton.onClick.AddListener(delegate
			{
				ResetPhysical();
			});
		}
		if (resetAppearanceButton != null)
		{
			resetAppearanceButton.onClick.AddListener(delegate
			{
				ResetAppearance();
			});
		}
		if (removeButton != null)
		{
			removeButton.onClick.AddListener(delegate
			{
				Remove();
			});
		}
		if (saveAppearancePresetButton != null)
		{
			saveAppearancePresetButton.onClick.AddListener(delegate
			{
				SavePresetDialog(includePhysical: false, includeAppearance: true);
			});
		}
		if (savePhysicalPresetButton != null)
		{
			savePhysicalPresetButton.onClick.AddListener(delegate
			{
				SavePresetDialog(includePhysical: true);
			});
		}
		if (loadAppearancePresetButton != null)
		{
			loadAppearancePresetButton.onClick.AddListener(delegate
			{
				LoadAppearancePresetDialog();
			});
		}
		if (loadPhysicalPresetButton != null)
		{
			loadPhysicalPresetButton.onClick.AddListener(delegate
			{
				LoadPhysicalPresetDialog();
			});
		}
	}

	private void Awake()
	{
		_startingOn = _on;
		_startingCollisionEnabled = _collisionEnabled;
		InitUI();
	}

	private void Start()
	{
		if (reParentObject != null)
		{
			reParentObjectStartingPosition = reParentObject.position;
			reParentObjectStartingRotation = reParentObject.rotation;
		}
		if (childAtomContainer != null)
		{
			childAtomContainerStartingPosition = childAtomContainer.position;
			childAtomContainerStartingRotation = childAtomContainer.rotation;
		}
		SyncMasterControllerCorners();
	}

	private void Update()
	{
		if (!(_masterController != null))
		{
			return;
		}
		Init();
		if (masterControllerCorners == null || masterControllerCorners.Length < 8)
		{
			return;
		}
		if (_freeControllers.Length > 1 || (alwaysShowExtents && _freeControllers.Length > 0))
		{
			Vector3 position = _freeControllers[0].transform.position;
			extentLowX = position.x;
			extentHighX = position.x;
			extentLowY = position.y;
			extentHighY = position.y;
			extentLowZ = position.z;
			extentHighZ = position.z;
			FreeControllerV3[] array = _freeControllers;
			foreach (FreeControllerV3 freeControllerV in array)
			{
				if (freeControllerV != _masterController)
				{
					position = freeControllerV.transform.position;
					if (position.x > extentHighX)
					{
						extentHighX = position.x;
					}
					else if (position.x < extentLowX)
					{
						extentLowX = position.x;
					}
					if (position.y > extentHighY)
					{
						extentHighY = position.y;
					}
					else if (position.y < extentLowY)
					{
						extentLowY = position.y;
					}
					if (position.z > extentHighZ)
					{
						extentHighZ = position.z;
					}
					else if (position.z < extentLowZ)
					{
						extentLowZ = position.z;
					}
				}
			}
			extentLowX -= extentPadding;
			extentLowY -= extentPadding;
			extentLowZ -= extentPadding;
			extentHighX += extentPadding;
			extentHighY += extentPadding;
			extentHighZ += extentPadding;
			extentlll.x = extentLowX;
			extentlll.y = extentLowY;
			extentlll.z = extentLowZ;
			extentllh.x = extentLowX;
			extentllh.y = extentLowY;
			extentllh.z = extentHighZ;
			extentlhl.x = extentLowX;
			extentlhl.y = extentHighY;
			extentlhl.z = extentLowZ;
			extentlhh.x = extentLowX;
			extentlhh.y = extentHighY;
			extentlhh.z = extentHighZ;
			extenthll.x = extentHighX;
			extenthll.y = extentLowY;
			extenthll.z = extentLowZ;
			extenthlh.x = extentHighX;
			extenthlh.y = extentLowY;
			extenthlh.z = extentHighZ;
			extenthhl.x = extentHighX;
			extenthhl.y = extentHighY;
			extenthhl.z = extentLowZ;
			extenthhh.x = extentHighX;
			extenthhh.y = extentHighY;
			extenthhh.z = extentHighZ;
			masterControllerCorners[0].position = extentlll;
			masterControllerCorners[1].position = extentllh;
			masterControllerCorners[2].position = extentlhl;
			masterControllerCorners[3].position = extentlhh;
			masterControllerCorners[4].position = extenthll;
			masterControllerCorners[5].position = extenthlh;
			masterControllerCorners[6].position = extenthhl;
			masterControllerCorners[7].position = extenthhh;
			Transform[] array2 = masterControllerCorners;
			foreach (Transform transform in array2)
			{
				transform.gameObject.SetActive(value: true);
			}
		}
		else
		{
			Transform[] array3 = masterControllerCorners;
			foreach (Transform transform2 in array3)
			{
				transform2.gameObject.SetActive(value: false);
			}
		}
	}
}
