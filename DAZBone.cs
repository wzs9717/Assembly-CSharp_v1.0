using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class DAZBone : JSONStorable
{
	protected const float geoScale = 0.01f;

	public bool isRoot;

	[SerializeField]
	private string _id;

	[SerializeField]
	private Vector3 _worldPosition;

	[SerializeField]
	private Vector3 _maleWorldPosition;

	[SerializeField]
	private Vector3 _worldOrientation;

	[SerializeField]
	private Vector3 _maleWorldOrientation;

	[SerializeField]
	private Vector3 _morphedWorldPosition;

	[SerializeField]
	private Vector3 _morphedWorldOrientation;

	[SerializeField]
	private Matrix4x4 _morphedLocalToWorldMatrix;

	[SerializeField]
	private Matrix4x4 _morphedWorldToLocalMatrix;

	private Matrix4x4 _changeFromOriginalMatrix;

	[SerializeField]
	private Quaternion2Angles.RotationOrder _maleRotationOrder;

	[SerializeField]
	private Quaternion2Angles.RotationOrder _rotationOrder;

	[SerializeField]
	private Dictionary<string, Vector3> _morphOffsets;

	[SerializeField]
	private Dictionary<string, Vector3> _morphOrientationOffsets;

	private Vector3 _currentAnglesRadians;

	private Vector3 _currentAngles;

	public Vector3 presetLocalTranslation;

	public Vector3 presetLocalRotation;

	private Vector3 _startingLocalPosition;

	private Quaternion _startingLocalRotation;

	public DAZBones dazBones;

	public bool disableMorph;

	private Vector3 saveBonePosition;

	private Quaternion saveBoneRotation;

	private Rigidbody saveConnectedBody;

	private Vector3 zeroVector = Vector3.zero;

	private Transform saveParent;

	private bool transformDirty;

	protected bool didDetachJoint;

	protected bool wasInit;

	public string id
	{
		get
		{
			return _id;
		}
		set
		{
			_id = value;
		}
	}

	public Vector3 worldPosition
	{
		get
		{
			if (dazBones != null && dazBones.isMale)
			{
				return _maleWorldPosition;
			}
			return _worldPosition;
		}
	}

	public Vector3 importWorldPosition => _worldPosition;

	public Vector3 maleWorldPosition => _maleWorldPosition;

	public Vector3 worldOrientation
	{
		get
		{
			if (dazBones != null && dazBones.isMale)
			{
				return _maleWorldOrientation;
			}
			return _worldOrientation;
		}
	}

	public Vector3 importWorldOrientation => _worldOrientation;

	public Vector3 maleWorldOrientation => _maleWorldOrientation;

	public Vector3 morphedWorldPosition => _morphedWorldPosition;

	public Vector3 morphedWorldOrientation => _morphedWorldOrientation;

	public Matrix4x4 morphedLocalToWorldMatrix => _morphedLocalToWorldMatrix;

	public Matrix4x4 morphedWorldToLocalMatrix => _morphedWorldToLocalMatrix;

	public Matrix4x4 changeFromOriginalMatrix => _changeFromOriginalMatrix;

	public Quaternion2Angles.RotationOrder rotationOrder
	{
		get
		{
			if (dazBones != null && dazBones.isMale)
			{
				return _maleRotationOrder;
			}
			return _rotationOrder;
		}
	}

	public Dictionary<string, Vector3> morphOffsets => _morphOffsets;

	public Dictionary<string, Vector3> morphOrientationOffsets => _morphOrientationOffsets;

	public Vector3 currentAngles => _currentAngles;

	public Vector3 startingLocalPosition => _startingLocalPosition;

	public Quaternion startingLocalRotation => _startingLocalRotation;

	public bool isTransformDirty => transformDirty;

	public override JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true)
	{
		JSONClass jSON = base.GetJSON(includePhysical, includeAppearance);
		if (includePhysical)
		{
			needsStore = true;
			if (isRoot)
			{
				Vector3 position = base.transform.position;
				jSON["rootPosition"]["x"].AsFloat = position.x;
				jSON["rootPosition"]["y"].AsFloat = position.y;
				jSON["rootPosition"]["z"].AsFloat = position.z;
				Vector3 eulerAngles = base.transform.eulerAngles;
				jSON["rootRotation"]["x"].AsFloat = eulerAngles.x;
				jSON["rootRotation"]["y"].AsFloat = eulerAngles.y;
				jSON["rootRotation"]["z"].AsFloat = eulerAngles.z;
			}
			else
			{
				Vector3 position = base.transform.localPosition;
				jSON["position"]["x"].AsFloat = position.x;
				jSON["position"]["y"].AsFloat = position.y;
				jSON["position"]["z"].AsFloat = position.z;
				Vector3 eulerAngles = base.transform.localEulerAngles;
				jSON["rotation"]["x"].AsFloat = eulerAngles.x;
				jSON["rotation"]["y"].AsFloat = eulerAngles.y;
				jSON["rotation"]["z"].AsFloat = eulerAngles.z;
			}
		}
		return jSON;
	}

	public override void RestoreFromJSON(JSONClass jc, bool restorePhysical = true, bool restoreAppearance = true)
	{
		Init();
		base.RestoreFromJSON(jc, restorePhysical, restoreAppearance);
		if (!restorePhysical)
		{
			return;
		}
		if (isRoot && jc["rootPosition"] != null)
		{
			Vector3 position = base.transform.position;
			if (jc["rootPosition"]["x"] != null)
			{
				position.x = jc["rootPosition"]["x"].AsFloat;
			}
			if (jc["rootPosition"]["y"] != null)
			{
				position.y = jc["rootPosition"]["y"].AsFloat;
			}
			if (jc["rootPosition"]["z"] != null)
			{
				position.z = jc["rootPosition"]["z"].AsFloat;
			}
			base.transform.position = position;
		}
		else if (jc["position"] != null)
		{
			Vector3 localPosition = base.transform.localPosition;
			if (jc["position"]["x"] != null)
			{
				localPosition.x = jc["position"]["x"].AsFloat;
			}
			if (jc["position"]["y"] != null)
			{
				localPosition.y = jc["position"]["y"].AsFloat;
			}
			if (jc["position"]["z"] != null)
			{
				localPosition.z = jc["position"]["z"].AsFloat;
			}
			base.transform.localPosition = localPosition;
		}
		else
		{
			base.transform.localPosition = _startingLocalPosition;
		}
		if (isRoot && jc["rootRotation"] != null)
		{
			Vector3 eulerAngles = base.transform.eulerAngles;
			if (jc["rootRotation"]["x"] != null)
			{
				eulerAngles.x = jc["rootRotation"]["x"].AsFloat;
			}
			if (jc["rootRotation"]["y"] != null)
			{
				eulerAngles.y = jc["rootRotation"]["y"].AsFloat;
			}
			if (jc["rootRotation"]["z"] != null)
			{
				eulerAngles.z = jc["rootRotation"]["z"].AsFloat;
			}
			base.transform.eulerAngles = eulerAngles;
		}
		else if (jc["rotation"] != null)
		{
			Vector3 localEulerAngles = base.transform.localEulerAngles;
			if (jc["rotation"]["x"] != null)
			{
				localEulerAngles.x = jc["rotation"]["x"].AsFloat;
			}
			if (jc["rotation"]["y"] != null)
			{
				localEulerAngles.y = jc["rotation"]["y"].AsFloat;
			}
			if (jc["rotation"]["z"] != null)
			{
				localEulerAngles.z = jc["rotation"]["z"].AsFloat;
			}
			base.transform.localEulerAngles = localEulerAngles;
		}
		else
		{
			base.transform.localRotation = _startingLocalRotation;
		}
	}

	public void ImportNode(JSONNode jn, bool isMale)
	{
		_id = jn["id"];
		foreach (JSONNode item in jn["center_point"].AsArray)
		{
			switch ((string)item["id"])
			{
			case "x":
				if (isMale)
				{
					_maleWorldPosition.x = item["value"].AsFloat * -0.01f;
				}
				else
				{
					_worldPosition.x = item["value"].AsFloat * -0.01f;
				}
				break;
			case "y":
				if (isMale)
				{
					_maleWorldPosition.y = item["value"].AsFloat * 0.01f;
				}
				else
				{
					_worldPosition.y = item["value"].AsFloat * 0.01f;
				}
				break;
			case "z":
				if (isMale)
				{
					_maleWorldPosition.z = item["value"].AsFloat * 0.01f;
				}
				else
				{
					_worldPosition.z = item["value"].AsFloat * 0.01f;
				}
				break;
			}
		}
		foreach (JSONNode item2 in jn["orientation"].AsArray)
		{
			switch ((string)item2["id"])
			{
			case "x":
				if (isMale)
				{
					_maleWorldOrientation.x = item2["value"].AsFloat;
				}
				else
				{
					_worldOrientation.x = item2["value"].AsFloat;
				}
				break;
			case "y":
				if (isMale)
				{
					_maleWorldOrientation.y = 0f - item2["value"].AsFloat;
				}
				else
				{
					_worldOrientation.y = 0f - item2["value"].AsFloat;
				}
				break;
			case "z":
				if (isMale)
				{
					_maleWorldOrientation.z = 0f - item2["value"].AsFloat;
				}
				else
				{
					_worldOrientation.z = 0f - item2["value"].AsFloat;
				}
				break;
			}
		}
		string text = jn["rotation_order"];
		Quaternion2Angles.RotationOrder maleRotationOrder;
		switch (text)
		{
		case "XYZ":
			maleRotationOrder = Quaternion2Angles.RotationOrder.ZYX;
			break;
		case "XZY":
			maleRotationOrder = Quaternion2Angles.RotationOrder.YZX;
			break;
		case "YXZ":
			maleRotationOrder = Quaternion2Angles.RotationOrder.ZXY;
			break;
		case "YZX":
			maleRotationOrder = Quaternion2Angles.RotationOrder.XZY;
			break;
		case "ZXY":
			maleRotationOrder = Quaternion2Angles.RotationOrder.YXZ;
			break;
		case "ZYX":
			maleRotationOrder = Quaternion2Angles.RotationOrder.XYZ;
			break;
		default:
			Debug.LogError("Bad rotation order in json: " + text);
			maleRotationOrder = Quaternion2Angles.RotationOrder.XYZ;
			break;
		}
		if (isMale)
		{
			_maleRotationOrder = maleRotationOrder;
		}
		else
		{
			_rotationOrder = maleRotationOrder;
		}
		SetTransformToImportValues();
	}

	public void Rotate(Vector3 rotationToUse)
	{
		switch (rotationOrder)
		{
		case Quaternion2Angles.RotationOrder.XYZ:
			base.transform.Rotate(rotationToUse.x, 0f, 0f);
			base.transform.Rotate(0f, rotationToUse.y, 0f);
			base.transform.Rotate(0f, 0f, rotationToUse.z);
			break;
		case Quaternion2Angles.RotationOrder.XZY:
			base.transform.Rotate(rotationToUse.x, 0f, 0f);
			base.transform.Rotate(0f, 0f, rotationToUse.z);
			base.transform.Rotate(0f, rotationToUse.y, 0f);
			break;
		case Quaternion2Angles.RotationOrder.YXZ:
			base.transform.Rotate(0f, rotationToUse.y, 0f);
			base.transform.Rotate(rotationToUse.x, 0f, 0f);
			base.transform.Rotate(0f, 0f, rotationToUse.z);
			break;
		case Quaternion2Angles.RotationOrder.YZX:
			base.transform.Rotate(0f, rotationToUse.y, 0f);
			base.transform.Rotate(0f, 0f, rotationToUse.z);
			base.transform.Rotate(rotationToUse.x, 0f, 0f);
			break;
		case Quaternion2Angles.RotationOrder.ZXY:
			base.transform.Rotate(0f, 0f, rotationToUse.z);
			base.transform.Rotate(rotationToUse.x, 0f, 0f);
			base.transform.Rotate(0f, rotationToUse.y, 0f);
			break;
		case Quaternion2Angles.RotationOrder.ZYX:
			base.transform.Rotate(0f, 0f, rotationToUse.z);
			base.transform.Rotate(0f, rotationToUse.y, 0f);
			base.transform.Rotate(rotationToUse.x, 0f, 0f);
			break;
		}
	}

	public void ApplyOffsetTransform()
	{
		if (dazBones != null)
		{
			base.transform.position += dazBones.transform.position;
		}
	}

	public void SetTransformToImportValues()
	{
		if (!Application.isPlaying)
		{
			base.transform.position = worldPosition;
			base.transform.rotation = Quaternion.Euler(worldOrientation);
			ApplyOffsetTransform();
		}
	}

	public void ApplyPresetLocalTransforms()
	{
		base.transform.localPosition += presetLocalTranslation;
		Rotate(presetLocalRotation);
	}

	public void SetImportValuesToTransform()
	{
		if (!Application.isPlaying)
		{
			if (dazBones != null && dazBones.isMale)
			{
				_maleWorldPosition = base.transform.position;
				_maleWorldOrientation = base.transform.eulerAngles;
			}
			else
			{
				_worldPosition = base.transform.position;
				_worldOrientation = base.transform.eulerAngles;
			}
		}
	}

	public void SetMorphedTransform(bool useScale, float globalScale)
	{
		transformDirty = false;
		if (disableMorph)
		{
			return;
		}
		_morphedWorldPosition = worldPosition;
		InitMorphOffsets();
		foreach (string key in _morphOffsets.Keys)
		{
			if (_morphOffsets.TryGetValue(key, out var value))
			{
				_morphedWorldPosition += value;
			}
		}
		base.transform.position = _morphedWorldPosition;
		_morphedWorldOrientation = worldOrientation;
		foreach (string key2 in _morphOrientationOffsets.Keys)
		{
			if (_morphOrientationOffsets.TryGetValue(key2, out var value2))
			{
				_morphedWorldOrientation += value2;
			}
		}
		base.transform.rotation = Quaternion.Euler(_morphedWorldOrientation);
		_morphedLocalToWorldMatrix = base.transform.localToWorldMatrix;
		_morphedWorldToLocalMatrix = base.transform.worldToLocalMatrix;
		if (useScale)
		{
			base.transform.position *= globalScale;
		}
	}

	public void SaveTransform()
	{
		saveBonePosition = base.transform.position;
		saveBoneRotation = base.transform.rotation;
	}

	public void RestoreTransform()
	{
		if (disableMorph)
		{
			return;
		}
		ConfigurableJoint component = GetComponent<ConfigurableJoint>();
		if (component != null)
		{
			base.transform.position = saveBonePosition;
			base.transform.rotation = saveBoneRotation;
			JointPositionHardLimit component2 = GetComponent<JointPositionHardLimit>();
			if (component2 != null && component2.useOffsetPosition)
			{
				component2.SetTargetPositionFromPercent();
			}
		}
		else
		{
			AdjustRotationTarget component3 = GetComponent<AdjustRotationTarget>();
			if (component3 != null)
			{
				component3.Adjust();
			}
		}
	}

	public void SaveAndDetachParent()
	{
		if (!disableMorph)
		{
			saveParent = base.transform.parent;
			base.transform.parent = null;
		}
	}

	public void RestoreParent()
	{
		if (!disableMorph)
		{
			base.transform.parent = saveParent;
			_startingLocalRotation = base.transform.localRotation;
			if (!isRoot)
			{
				_startingLocalPosition = base.transform.localPosition;
			}
			ResetScale();
		}
	}

	public void ResetScale()
	{
		base.transform.localScale = Vector3.one;
	}

	public void DetachJoint()
	{
		if (!disableMorph)
		{
			ConfigurableJoint component = GetComponent<ConfigurableJoint>();
			if (component != null && component.connectedBody != null && !component.connectedBody.GetComponent<FreeControllerV3>())
			{
				didDetachJoint = true;
				saveConnectedBody = component.connectedBody;
				component.connectedBody = null;
			}
		}
	}

	public void AttachJoint()
	{
		ConfigurableJoint component = GetComponent<ConfigurableJoint>();
		if (didDetachJoint)
		{
			didDetachJoint = false;
			component.connectedBody = saveConnectedBody;
			Vector3 localPosition = base.transform.localPosition;
			if (component.connectedAnchor != localPosition)
			{
				component.connectedAnchor = localPosition;
			}
			JointPositionHardLimit component2 = GetComponent<JointPositionHardLimit>();
			if (component2 != null)
			{
				component2.startAnchor = component.connectedAnchor;
				component2.startRotation = base.transform.localRotation;
			}
		}
	}

	private void InitMorphOffsets()
	{
		if (_morphOffsets == null)
		{
			_morphOffsets = new Dictionary<string, Vector3>();
		}
		if (_morphOrientationOffsets == null)
		{
			_morphOrientationOffsets = new Dictionary<string, Vector3>();
		}
	}

	public void SetBoneXOffset(string morphName, float xoffset)
	{
		InitMorphOffsets();
		Vector3 value = zeroVector;
		if (_morphOffsets.TryGetValue(morphName, out value))
		{
			value.x = xoffset;
			_morphOffsets.Remove(morphName);
			if (value != zeroVector)
			{
				_morphOffsets.Add(morphName, value);
			}
		}
		else
		{
			value.x = xoffset;
			if (value != zeroVector)
			{
				_morphOffsets.Add(morphName, value);
			}
		}
		transformDirty = true;
	}

	public void SetBoneYOffset(string morphName, float yoffset)
	{
		InitMorphOffsets();
		Vector3 value = zeroVector;
		if (_morphOffsets.TryGetValue(morphName, out value))
		{
			value.y = yoffset;
			_morphOffsets.Remove(morphName);
			if (value != zeroVector)
			{
				_morphOffsets.Add(morphName, value);
			}
		}
		else
		{
			value.y = yoffset;
			if (value != zeroVector)
			{
				_morphOffsets.Add(morphName, value);
			}
		}
		transformDirty = true;
	}

	public void SetBoneZOffset(string morphName, float zoffset)
	{
		InitMorphOffsets();
		Vector3 value = zeroVector;
		if (_morphOffsets.TryGetValue(morphName, out value))
		{
			value.z = zoffset;
			_morphOffsets.Remove(morphName);
			if (value != zeroVector)
			{
				_morphOffsets.Add(morphName, value);
			}
		}
		else
		{
			value.z = zoffset;
			if (value != zeroVector)
			{
				_morphOffsets.Add(morphName, value);
			}
		}
		transformDirty = true;
	}

	public void SetBoneOrientationXOffset(string morphName, float xoffset)
	{
		InitMorphOffsets();
		Vector3 value = zeroVector;
		if (_morphOrientationOffsets.TryGetValue(morphName, out value))
		{
			value.x = xoffset;
			_morphOrientationOffsets.Remove(morphName);
			if (value != zeroVector)
			{
				_morphOrientationOffsets.Add(morphName, value);
			}
		}
		else
		{
			value.x = xoffset;
			if (value != zeroVector)
			{
				_morphOrientationOffsets.Add(morphName, value);
			}
		}
		transformDirty = true;
	}

	public void SetBoneOrientationYOffset(string morphName, float yoffset)
	{
		InitMorphOffsets();
		Vector3 value = zeroVector;
		if (_morphOrientationOffsets.TryGetValue(morphName, out value))
		{
			value.y = yoffset;
			_morphOrientationOffsets.Remove(morphName);
			if (value != zeroVector)
			{
				_morphOrientationOffsets.Add(morphName, value);
			}
		}
		else
		{
			value.y = yoffset;
			if (value != zeroVector)
			{
				_morphOrientationOffsets.Add(morphName, value);
			}
		}
		transformDirty = true;
	}

	public void SetBoneOrientationZOffset(string morphName, float zoffset)
	{
		InitMorphOffsets();
		Vector3 value = zeroVector;
		if (_morphOrientationOffsets.TryGetValue(morphName, out value))
		{
			value.z = zoffset;
			_morphOrientationOffsets.Remove(morphName);
			if (value != zeroVector)
			{
				_morphOrientationOffsets.Add(morphName, value);
			}
		}
		else
		{
			value.z = zoffset;
			if (value != zeroVector)
			{
				_morphOrientationOffsets.Add(morphName, value);
			}
		}
		transformDirty = true;
	}

	public void SetBoneScaleX(string morphName, float xscale)
	{
	}

	public Vector3 GetAngles()
	{
		return _currentAnglesRadians;
	}

	public Vector3 GetAnglesDegrees()
	{
		return _currentAngles;
	}

	public void Init()
	{
		if (!wasInit)
		{
			wasInit = true;
			_changeFromOriginalMatrix = base.transform.localToWorldMatrix * _morphedWorldToLocalMatrix;
			_startingLocalPosition = base.transform.localPosition;
			_startingLocalRotation = base.transform.localRotation;
		}
	}

	private void Awake()
	{
		Init();
	}

	private void Update()
	{
		_changeFromOriginalMatrix = base.transform.localToWorldMatrix * _morphedWorldToLocalMatrix;
		_currentAnglesRadians = Quaternion2Angles.GetAngles(Quaternion.Inverse(_startingLocalRotation) * base.transform.localRotation, rotationOrder);
		_currentAngles = _currentAnglesRadians * 57.29578f;
	}
}
