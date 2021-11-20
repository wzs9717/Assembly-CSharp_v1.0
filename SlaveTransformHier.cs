using System.Collections.Generic;
using UnityEngine;

public class SlaveTransformHier : MonoBehaviour
{
	public Transform sourceTree;

	private Dictionary<string, Transform> sourceTreeMap;

	private Dictionary<Transform, Transform> transformMap;

	private Transform findTransform(string tname)
	{
		Transform[] componentsInChildren = sourceTree.GetComponentsInChildren<Transform>();
		foreach (Transform transform in componentsInChildren)
		{
			if (transform.name == tname)
			{
				return transform;
			}
		}
		return null;
	}

	private void init()
	{
		if (!sourceTree)
		{
			return;
		}
		sourceTreeMap = new Dictionary<string, Transform>();
		Transform[] componentsInChildren = sourceTree.GetComponentsInChildren<Transform>();
		foreach (Transform transform in componentsInChildren)
		{
			if (!sourceTreeMap.ContainsKey(transform.name))
			{
				sourceTreeMap.Add(transform.name, transform);
			}
		}
		transformMap = new Dictionary<Transform, Transform>();
		Transform[] componentsInChildren2 = GetComponentsInChildren<Transform>();
		foreach (Transform transform2 in componentsInChildren2)
		{
			if (sourceTreeMap.TryGetValue(transform2.name, out var value))
			{
				transformMap.Add(value, transform2);
			}
		}
	}

	private void Start()
	{
		init();
	}

	private void Update()
	{
		if (transformMap == null)
		{
			return;
		}
		foreach (Transform key in transformMap.Keys)
		{
			if (transformMap.TryGetValue(key, out var value))
			{
				value.position = key.position;
				value.rotation = key.rotation;
			}
		}
	}
}
