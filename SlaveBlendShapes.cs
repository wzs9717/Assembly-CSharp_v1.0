using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[ExecuteInEditMode]
public class SlaveBlendShapes : MonoBehaviour
{
	public SkinnedMeshRenderer source;

	public string[] exclude;

	public bool reinit;

	private SkinnedMeshRenderer dest;

	private Dictionary<string, bool> excludeMap;

	private Dictionary<int, int> shapeMap;

	private int shapeCount;

	private bool wasInit;

	private void Start()
	{
		init();
	}

	private void init()
	{
		if (wasInit)
		{
			return;
		}
		wasInit = true;
		dest = GetComponent<SkinnedMeshRenderer>();
		shapeMap = new Dictionary<int, int>();
		excludeMap = new Dictionary<string, bool>();
		if (exclude != null)
		{
			string[] array = exclude;
			foreach (string key in array)
			{
				excludeMap.Add(key, value: true);
			}
		}
		if (!dest)
		{
			return;
		}
		shapeCount = dest.sharedMesh.blendShapeCount;
		if (!source)
		{
			return;
		}
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		for (int j = 0; j < source.sharedMesh.blendShapeCount; j++)
		{
			string blendShapeName = source.sharedMesh.GetBlendShapeName(j);
			string key2 = Regex.Replace(blendShapeName, "^.*\\.", string.Empty);
			dictionary.Add(key2, j);
		}
		for (int k = 0; k < shapeCount; k++)
		{
			string blendShapeName2 = dest.sharedMesh.GetBlendShapeName(k);
			string key3 = Regex.Replace(blendShapeName2, "^.*\\.", string.Empty);
			if (dictionary.TryGetValue(key3, out var value) && !excludeMap.TryGetValue(key3, out var _))
			{
				shapeMap.Add(k, value);
			}
		}
	}

	private void setWeights()
	{
		if (!source || !dest || shapeMap == null)
		{
			return;
		}
		for (int i = 0; i < shapeCount; i++)
		{
			if (shapeMap.TryGetValue(i, out var value))
			{
				dest.SetBlendShapeWeight(i, source.GetBlendShapeWeight(value));
			}
		}
	}

	private void Update()
	{
		if (reinit)
		{
			wasInit = false;
			reinit = false;
		}
		init();
		setWeights();
	}
}
