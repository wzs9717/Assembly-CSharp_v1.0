using System.Collections.Generic;
using UnityEngine;

public class RebindSMR : MonoBehaviour
{
	public Transform newRoot;

	private SkinnedMeshRenderer smr;

	private void Start()
	{
		smr = GetComponent<SkinnedMeshRenderer>();
		if (!(newRoot != null) || !(smr != null))
		{
			return;
		}
		Transform[] array = new Transform[smr.bones.Length];
		Dictionary<string, Transform> dictionary = new Dictionary<string, Transform>();
		Transform[] componentsInChildren = newRoot.GetComponentsInChildren<Transform>();
		foreach (Transform transform in componentsInChildren)
		{
			if (!dictionary.ContainsKey(transform.name))
			{
				dictionary.Add(transform.name, transform);
			}
		}
		for (int j = 0; j < smr.bones.Length; j++)
		{
			Transform transform2 = smr.bones[j];
			if (dictionary.TryGetValue(transform2.name, out var value))
			{
				array[j] = value;
			}
		}
		smr.bones = array;
		if (dictionary.TryGetValue(smr.rootBone.name, out var value2))
		{
			smr.rootBone = value2;
		}
	}
}
