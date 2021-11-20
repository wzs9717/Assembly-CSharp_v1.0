using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ZeroMorphs : MonoBehaviour
{
	public bool reinit;

	public SkinnedMeshRenderer skin;

	public string[] morphs;

	private Dictionary<int, float> startingVals;

	private bool zero;

	private bool wasInit;

	private void OnEnable()
	{
		if ((bool)skin)
		{
			zero = true;
		}
	}

	private void OnDisable()
	{
		if (!skin || startingVals == null)
		{
			return;
		}
		zero = false;
		foreach (int key in startingVals.Keys)
		{
			if (startingVals.TryGetValue(key, out var value))
			{
				skin.SetBlendShapeWeight(key, value);
			}
		}
	}

	private void Init()
	{
		wasInit = true;
		startingVals = new Dictionary<int, float>();
		if (!skin)
		{
			return;
		}
		string[] array = morphs;
		foreach (string blendShapeName in array)
		{
			int blendShapeIndex = skin.sharedMesh.GetBlendShapeIndex(blendShapeName);
			if (blendShapeIndex != -1)
			{
				startingVals.Add(blendShapeIndex, skin.GetBlendShapeWeight(blendShapeIndex));
			}
		}
	}

	private void Start()
	{
		Init();
	}

	private void zeroMorphs()
	{
		if (!skin || !zero || startingVals == null)
		{
			return;
		}
		foreach (int key in startingVals.Keys)
		{
			skin.SetBlendShapeWeight(key, 0f);
		}
	}

	private void Update()
	{
		if (reinit)
		{
			reinit = false;
			wasInit = false;
		}
		if (!wasInit)
		{
			Init();
		}
		zeroMorphs();
	}

	private void LateUpdate()
	{
		zeroMorphs();
	}
}
