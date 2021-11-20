using UnityEngine;

public class DebugSMR : MonoBehaviour
{
	public int vertexIndex;

	private SkinnedMeshRenderer smr;

	private int reportedIndex;

	private void report()
	{
		reportedIndex = vertexIndex;
		BoneWeight boneWeight = smr.sharedMesh.boneWeights[vertexIndex];
		Debug.Log("Boneweight: bi0:" + boneWeight.boneIndex0 + " w:" + boneWeight.weight0 + " bi1:" + boneWeight.boneIndex1 + " w:" + boneWeight.weight1 + " bi2:" + boneWeight.boneIndex2 + " w:" + boneWeight.weight2 + " bi3:" + boneWeight.boneIndex3 + " w:" + boneWeight.weight3);
		Debug.Log("Bone names: bi0: " + smr.bones[boneWeight.boneIndex0].name + " bi1: " + smr.bones[boneWeight.boneIndex1].name + " bi2: " + smr.bones[boneWeight.boneIndex2].name + " bi3: " + smr.bones[boneWeight.boneIndex3].name);
	}

	private void Start()
	{
		smr = GetComponent<SkinnedMeshRenderer>();
		if ((bool)smr)
		{
			report();
		}
	}

	private void Update()
	{
		if (reportedIndex != vertexIndex)
		{
			report();
		}
	}
}
