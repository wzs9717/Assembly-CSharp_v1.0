using System.Collections.Generic;
using UnityEngine;

public class DAZMorphSet : MonoBehaviour
{
	public string displayName;

	public Transform DAZMeshInitTransform;

	public Transform DAZMeshApplyTransform;

	public string[] morphNames;

	public float[] morphValues;

	public float[] morphStartValues;

	public string[] untrackedMorphNames;

	public float[] untrackedMorphValues;

	public void InitSet()
	{
		if (!(DAZMeshInitTransform != null))
		{
			return;
		}
		DAZMesh[] components = DAZMeshInitTransform.GetComponents<DAZMesh>();
		List<string> list = new List<string>();
		List<float> list2 = new List<float>();
		List<float> list3 = new List<float>();
		List<string> list4 = new List<string>();
		List<float> list5 = new List<float>();
		DAZMesh[] array = components;
		foreach (DAZMesh dAZMesh in array)
		{
			if (!(dAZMesh.morphBank != null))
			{
				continue;
			}
			foreach (DAZMorph morph in dAZMesh.morphBank.morphs)
			{
				if (morph.morphValue != 0f)
				{
					if (morph.visible)
					{
						list.Add(morph.morphName);
						list2.Add(morph.morphValue);
						list3.Add(morph.startValue);
					}
					else
					{
						list4.Add(morph.morphName);
						list5.Add(morph.morphValue);
					}
				}
			}
		}
		morphNames = list.ToArray();
		morphValues = list2.ToArray();
		morphStartValues = list3.ToArray();
		untrackedMorphNames = list4.ToArray();
		untrackedMorphValues = list5.ToArray();
	}

	public void ApplySet()
	{
		if (!(DAZMeshApplyTransform != null) || morphNames == null)
		{
			return;
		}
		DAZMesh[] components = DAZMeshApplyTransform.GetComponents<DAZMesh>();
		DAZMesh[] array = components;
		foreach (DAZMesh dAZMesh in array)
		{
			if (!(dAZMesh.morphBank != null))
			{
				continue;
			}
			foreach (DAZMorph morph2 in dAZMesh.morphBank.morphs)
			{
				if (morph2.visible)
				{
					morph2.morphValue = 0f;
					morph2.startValue = 0f;
				}
			}
		}
		for (int j = 0; j < morphNames.Length; j++)
		{
			DAZMesh[] array2 = components;
			foreach (DAZMesh dAZMesh2 in array2)
			{
				if (dAZMesh2.morphBank != null)
				{
					DAZMorph morph = dAZMesh2.morphBank.GetMorph(morphNames[j]);
					if (morph != null)
					{
						morph.morphValue = morphValues[j];
						morph.startValue = morphStartValues[j];
					}
				}
			}
		}
		DAZMesh[] array3 = components;
		foreach (DAZMesh dAZMesh3 in array3)
		{
			if (dAZMesh3.morphBank != null)
			{
				dAZMesh3.morphBank.ApplyMorphsImmediate();
			}
		}
	}
}
