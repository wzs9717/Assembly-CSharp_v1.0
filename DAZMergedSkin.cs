using UnityEngine;

public class DAZMergedSkin : DAZSkin
{
	public void Merge()
	{
		DAZMergedMesh dAZMergedMesh = (DAZMergedMesh)(dazMesh = GetComponent<DAZMergedMesh>());
		if (dazMesh == null)
		{
			Debug.LogError("Can't merge because no DAZMergedMesh component found");
			return;
		}
		string text = dAZMergedMesh.targetMesh.geometryId;
		string text2 = dAZMergedMesh.graftMesh.geometryId;
		geometryId = text + ":" + text2;
		string text3 = null;
		bool has2ndGraft = dAZMergedMesh.has2ndGraft;
		if (has2ndGraft)
		{
			text3 = dAZMergedMesh.graft2Mesh.geometryId;
			geometryId = geometryId + ":" + text3;
		}
		DAZSkin[] components = GetComponents<DAZSkin>();
		if (components == null)
		{
			Debug.LogError("Can't merge because no DAZSkin components found");
			return;
		}
		DAZSkin dAZSkin = null;
		DAZSkin dAZSkin2 = null;
		DAZSkin dAZSkin3 = null;
		DAZSkin[] array = components;
		foreach (DAZSkin dAZSkin4 in array)
		{
			if (dAZSkin4.geometryId == text)
			{
				dAZSkin = dAZSkin4;
			}
			else if (dAZSkin4.geometryId == text2)
			{
				dAZSkin2 = dAZSkin4;
			}
			else if (has2ndGraft && dAZSkin4.geometryId == text3)
			{
				dAZSkin3 = dAZSkin4;
			}
		}
		if (dAZSkin == null || dAZSkin2 == null)
		{
			Debug.LogError("Could not find both target and graft skin to merge");
			return;
		}
		int num = dAZSkin.numBones;
		int num2 = dAZSkin2.numBones;
		_numBones = dAZSkin.numBones + num2;
		if (has2ndGraft)
		{
			_numBones += dAZSkin3.numBones;
		}
		nodes = new DAZNode[_numBones];
		for (int j = 0; j < dAZSkin.numBones; j++)
		{
			nodes[j] = dAZSkin.nodes[j];
		}
		int startGraftVertIndex = dAZMergedMesh.startGraftVertIndex;
		for (int k = 0; k < dAZSkin2.numBones; k++)
		{
			DAZNode dAZNode = dAZSkin2.nodes[k];
			int num3 = num + k;
			nodes[num3] = new DAZNode();
			nodes[num3].name = dAZNode.name;
			nodes[num3].rotationOrder = dAZNode.rotationOrder;
			nodes[num3].bulgeFactors = dAZNode.bulgeFactors;
			DAZMeshVertexWeights[] weights = dAZNode.weights;
			DAZMeshVertexWeights[] array2 = new DAZMeshVertexWeights[weights.Length];
			nodes[num3].weights = array2;
			for (int l = 0; l < array2.Length; l++)
			{
				array2[l] = new DAZMeshVertexWeights();
				array2[l].vertex = weights[l].vertex + startGraftVertIndex;
				array2[l].weight = weights[l].weight;
				array2[l].xweight = weights[l].xweight;
				array2[l].yweight = weights[l].yweight;
				array2[l].zweight = weights[l].zweight;
				array2[l].xleftbulge = weights[l].xleftbulge;
				array2[l].xrightbulge = weights[l].xrightbulge;
				array2[l].yleftbulge = weights[l].yleftbulge;
				array2[l].yrightbulge = weights[l].yrightbulge;
				array2[l].zleftbulge = weights[l].zleftbulge;
				array2[l].zrightbulge = weights[l].zrightbulge;
			}
		}
		if (!has2ndGraft)
		{
			return;
		}
		int startGraft2VertIndex = dAZMergedMesh.startGraft2VertIndex;
		for (int m = 0; m < dAZSkin3.numBones; m++)
		{
			DAZNode dAZNode2 = dAZSkin3.nodes[m];
			int num4 = num + num2 + m;
			nodes[num4] = new DAZNode();
			nodes[num4].name = dAZNode2.name;
			nodes[num4].rotationOrder = dAZNode2.rotationOrder;
			nodes[num4].bulgeFactors = dAZNode2.bulgeFactors;
			DAZMeshVertexWeights[] weights2 = dAZNode2.weights;
			DAZMeshVertexWeights[] array3 = new DAZMeshVertexWeights[weights2.Length];
			nodes[num4].weights = array3;
			for (int n = 0; n < array3.Length; n++)
			{
				array3[n] = new DAZMeshVertexWeights();
				array3[n].vertex = weights2[n].vertex + startGraft2VertIndex;
				array3[n].weight = weights2[n].weight;
				array3[n].xweight = weights2[n].xweight;
				array3[n].yweight = weights2[n].yweight;
				array3[n].zweight = weights2[n].zweight;
				array3[n].xleftbulge = weights2[n].xleftbulge;
				array3[n].xrightbulge = weights2[n].xrightbulge;
				array3[n].yleftbulge = weights2[n].yleftbulge;
				array3[n].yrightbulge = weights2[n].yrightbulge;
				array3[n].zleftbulge = weights2[n].zleftbulge;
				array3[n].zrightbulge = weights2[n].zrightbulge;
			}
		}
	}
}
