using System.Collections.Generic;
using UnityEngine;

public class DAZBones : MonoBehaviour
{
	private Dictionary<string, DAZBone> boneNameToDAZBone;

	private Dictionary<string, DAZBone> boneIdToDAZBone;

	private DAZBone[] dazBones;

	public bool useScale;

	private bool _isMale;

	[SerializeField]
	private Dictionary<string, float> _morphGeneralScales;

	private float _currentGeneralScale;

	private bool _wasInit;

	public bool isMale
	{
		get
		{
			return _isMale;
		}
		set
		{
			if (_isMale != value)
			{
				_isMale = value;
				SetMorphedTransform();
			}
		}
	}

	public Dictionary<string, float> morphGeneralScales => _morphGeneralScales;

	public float currentGeneralScale => _currentGeneralScale;

	public bool wasInit => _wasInit;

	public void SetGeneralScale(string morphName, float scale)
	{
		if (_morphGeneralScales == null)
		{
			_morphGeneralScales = new Dictionary<string, float>();
		}
		if (_morphGeneralScales.TryGetValue(morphName, out var _))
		{
			_morphGeneralScales.Remove(morphName);
		}
		if (scale != 0f)
		{
			_morphGeneralScales.Add(morphName, scale);
		}
		_currentGeneralScale = 0f;
		foreach (float value2 in _morphGeneralScales.Values)
		{
			float num = value2;
			_currentGeneralScale += num;
		}
		SetMorphedTransform();
	}

	public DAZBone GetDAZBone(string boneName)
	{
		Init();
		if (boneNameToDAZBone != null)
		{
			if (boneNameToDAZBone.TryGetValue(boneName, out var value))
			{
				return value;
			}
			return null;
		}
		return null;
	}

	public DAZBone GetDAZBoneById(string boneId)
	{
		Init();
		if (boneIdToDAZBone != null)
		{
			if (boneIdToDAZBone.TryGetValue(boneId, out var value))
			{
				return value;
			}
			return null;
		}
		return null;
	}

	public void Reset()
	{
		_wasInit = false;
		boneNameToDAZBone = null;
		boneIdToDAZBone = null;
		Init();
	}

	private void InitBonesRecursive(Transform t)
	{
		foreach (Transform item in t)
		{
			DAZBones component = item.GetComponent<DAZBones>();
			if (!(component == null))
			{
				continue;
			}
			DAZBone component2 = item.GetComponent<DAZBone>();
			if (component2 != null)
			{
				component2.Init();
				if (boneNameToDAZBone.ContainsKey(component2.name))
				{
					Debug.LogError("Found duplicate bone " + component2.name);
				}
				else
				{
					boneNameToDAZBone.Add(component2.name, component2);
				}
				if (boneIdToDAZBone.ContainsKey(component2.id))
				{
					Debug.LogError("Found duplicate bone id " + component2.id);
				}
				else
				{
					boneIdToDAZBone.Add(component2.id, component2);
				}
				InitBonesRecursive(component2.transform);
			}
		}
	}

	public void Init()
	{
		if (!_wasInit || boneNameToDAZBone == null)
		{
			_wasInit = true;
			boneNameToDAZBone = new Dictionary<string, DAZBone>();
			boneIdToDAZBone = new Dictionary<string, DAZBone>();
			InitBonesRecursive(base.transform);
			dazBones = new DAZBone[boneNameToDAZBone.Count];
			boneNameToDAZBone.Values.CopyTo(dazBones, 0);
			SetMorphedTransform();
		}
	}

	public void SetTransformsToImportValues()
	{
		if (dazBones != null)
		{
			DAZBone[] array = dazBones;
			foreach (DAZBone dAZBone in array)
			{
				dAZBone.SetTransformToImportValues();
			}
		}
	}

	public void SetMorphedTransform()
	{
		float x = base.transform.lossyScale.x;
		if (dazBones != null)
		{
			if (Application.isPlaying)
			{
				DAZBone[] array = dazBones;
				foreach (DAZBone dAZBone in array)
				{
					dAZBone.SaveTransform();
				}
			}
			DAZBone[] array2 = dazBones;
			foreach (DAZBone dAZBone2 in array2)
			{
				dAZBone2.dazBones = this;
				dAZBone2.DetachJoint();
				dAZBone2.SaveAndDetachParent();
			}
			DAZBone[] array3 = dazBones;
			foreach (DAZBone dAZBone3 in array3)
			{
				dAZBone3.ResetScale();
			}
			DAZBone[] array4 = dazBones;
			foreach (DAZBone dAZBone4 in array4)
			{
				dAZBone4.SetMorphedTransform(useScale, x);
			}
			if (!Application.isPlaying)
			{
				DAZBone[] array5 = dazBones;
				foreach (DAZBone dAZBone5 in array5)
				{
					dAZBone5.ApplyOffsetTransform();
				}
			}
			DAZBone[] array6 = dazBones;
			foreach (DAZBone dAZBone6 in array6)
			{
				dAZBone6.RestoreParent();
				dAZBone6.AttachJoint();
			}
			if (Application.isPlaying)
			{
				DAZBone[] array7 = dazBones;
				foreach (DAZBone dAZBone7 in array7)
				{
					dAZBone7.RestoreTransform();
				}
			}
			else
			{
				DAZBone[] array8 = dazBones;
				foreach (DAZBone dAZBone8 in array8)
				{
					dAZBone8.ApplyPresetLocalTransforms();
				}
			}
		}
		else
		{
			Debug.LogWarning("SetMorphedTransform called when bones were not init");
		}
	}

	private void Start()
	{
		Init();
	}
}
