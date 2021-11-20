using UnityEngine;

public class DAZMergedSkinV2 : DAZSkinV2
{
	public void Merge()
	{
		DAZMergedMesh dAZMergedMesh = (DAZMergedMesh)(dazMesh = GetComponent<DAZMergedMesh>());
		if (dazMesh == null)
		{
			Debug.LogError("Could not merge because no DAZMergedMesh component found");
			return;
		}
		string text = dAZMergedMesh.targetMesh.sceneGeometryId;
		string text2 = dAZMergedMesh.graftMesh.sceneGeometryId;
		sceneGeometryId = text + ":" + text2;
		string text3 = null;
		bool has2ndGraft = dAZMergedMesh.has2ndGraft;
		if (has2ndGraft)
		{
			text3 = dAZMergedMesh.graft2Mesh.sceneGeometryId;
			sceneGeometryId = sceneGeometryId + ":" + text3;
		}
		DAZSkinV2[] components = GetComponents<DAZSkinV2>();
		if (components == null)
		{
			Debug.LogError("Can't merge because no DAZSkin components found");
			return;
		}
		DAZSkinV2 dAZSkinV = null;
		DAZSkinV2 dAZSkinV2 = null;
		DAZSkinV2 dAZSkinV3 = null;
		_hasGeneralWeights = true;
		DAZSkinV2[] array = components;
		foreach (DAZSkinV2 dAZSkinV4 in array)
		{
			if (dAZSkinV4.sceneGeometryId == text)
			{
				dAZSkinV = dAZSkinV4;
				dAZSkinV.skin = false;
				dAZSkinV.draw = false;
				if (!dAZSkinV.hasGeneralWeights)
				{
					_hasGeneralWeights = false;
				}
			}
			else if (dAZSkinV4.sceneGeometryId == text2)
			{
				dAZSkinV2 = dAZSkinV4;
				dAZSkinV2.skin = false;
				dAZSkinV2.draw = false;
				if (!dAZSkinV2.hasGeneralWeights)
				{
					_hasGeneralWeights = false;
				}
			}
			else if (has2ndGraft && dAZSkinV4.sceneGeometryId == text3)
			{
				dAZSkinV3 = dAZSkinV4;
				dAZSkinV3.skin = false;
				dAZSkinV3.draw = false;
				if (!dAZSkinV3.hasGeneralWeights)
				{
					_hasGeneralWeights = false;
				}
			}
		}
		if (dAZSkinV == null || dAZSkinV2 == null)
		{
			Debug.LogError("Could not find both target and graft skin to merge");
			return;
		}
		int num = dAZSkinV.numBones;
		int num2 = dAZSkinV2.numBones;
		_numBones = dAZSkinV.numBones + num2;
		if (has2ndGraft)
		{
			_numBones += dAZSkinV3.numBones;
		}
		nodes = new DAZSkinV2Node[_numBones];
		int num3 = dAZMergedMesh.numGraftBaseVertices + dAZMergedMesh.numGraft2BaseVertices;
		for (int j = 0; j < dAZSkinV.numBones; j++)
		{
			DAZSkinV2Node dAZSkinV2Node = dAZSkinV.nodes[j];
			nodes[j] = new DAZSkinV2Node();
			nodes[j].name = dAZSkinV2Node.name;
			nodes[j].rotationOrder = dAZSkinV2Node.rotationOrder;
			nodes[j].bulgeFactors = dAZSkinV2Node.bulgeFactors;
			DAZSkinV2VertexWeights[] weights = dAZSkinV2Node.weights;
			DAZSkinV2VertexWeights[] array2 = new DAZSkinV2VertexWeights[weights.Length];
			nodes[j].weights = array2;
			for (int k = 0; k < array2.Length; k++)
			{
				array2[k] = new DAZSkinV2VertexWeights();
				if (weights[k].vertex >= dAZMergedMesh.numTargetBaseVertices)
				{
					array2[k].vertex = weights[k].vertex + num3;
				}
				else
				{
					array2[k].vertex = weights[k].vertex;
				}
				array2[k].xweight = weights[k].xweight;
				array2[k].yweight = weights[k].yweight;
				array2[k].zweight = weights[k].zweight;
				array2[k].xleftbulge = weights[k].xleftbulge;
				array2[k].xrightbulge = weights[k].xrightbulge;
				array2[k].yleftbulge = weights[k].yleftbulge;
				array2[k].yrightbulge = weights[k].yrightbulge;
				array2[k].zleftbulge = weights[k].zleftbulge;
				array2[k].zrightbulge = weights[k].zrightbulge;
			}
			int[] fullyWeightedVertices = dAZSkinV2Node.fullyWeightedVertices;
			int[] array3 = new int[fullyWeightedVertices.Length];
			nodes[j].fullyWeightedVertices = array3;
			for (int l = 0; l < array3.Length; l++)
			{
				if (fullyWeightedVertices[l] >= dAZMergedMesh.numTargetBaseVertices)
				{
					array3[l] = fullyWeightedVertices[l] + num3;
				}
				else
				{
					array3[l] = fullyWeightedVertices[l];
				}
			}
			DAZSkinV2GeneralVertexWeights[] generalWeights = dAZSkinV2Node.generalWeights;
			DAZSkinV2GeneralVertexWeights[] array4 = new DAZSkinV2GeneralVertexWeights[generalWeights.Length];
			nodes[j].generalWeights = array4;
			for (int m = 0; m < array4.Length; m++)
			{
				array4[m] = new DAZSkinV2GeneralVertexWeights();
				if (generalWeights[m].vertex >= dAZMergedMesh.numTargetBaseVertices)
				{
					array4[m].vertex = generalWeights[m].vertex + num3;
				}
				else
				{
					array4[m].vertex = generalWeights[m].vertex;
				}
				array4[m].weight = generalWeights[m].weight;
			}
		}
		int startGraftVertIndex = dAZMergedMesh.startGraftVertIndex;
		int num4 = dAZMergedMesh.numTargetUVVertices + dAZMergedMesh.numGraft2BaseVertices;
		for (int n = 0; n < dAZSkinV2.numBones; n++)
		{
			DAZSkinV2Node dAZSkinV2Node2 = dAZSkinV2.nodes[n];
			int num5 = num + n;
			nodes[num5] = new DAZSkinV2Node();
			nodes[num5].name = dAZSkinV2Node2.name;
			nodes[num5].rotationOrder = dAZSkinV2Node2.rotationOrder;
			nodes[num5].bulgeFactors = dAZSkinV2Node2.bulgeFactors;
			DAZSkinV2VertexWeights[] weights2 = dAZSkinV2Node2.weights;
			DAZSkinV2VertexWeights[] array5 = new DAZSkinV2VertexWeights[weights2.Length];
			nodes[num5].weights = array5;
			for (int num6 = 0; num6 < array5.Length; num6++)
			{
				array5[num6] = new DAZSkinV2VertexWeights();
				if (weights2[num6].vertex >= dAZMergedMesh.numGraftBaseVertices)
				{
					array5[num6].vertex = weights2[num6].vertex + num4;
				}
				else
				{
					array5[num6].vertex = weights2[num6].vertex + startGraftVertIndex;
				}
				array5[num6].xweight = weights2[num6].xweight;
				array5[num6].yweight = weights2[num6].yweight;
				array5[num6].zweight = weights2[num6].zweight;
				array5[num6].xleftbulge = weights2[num6].xleftbulge;
				array5[num6].xrightbulge = weights2[num6].xrightbulge;
				array5[num6].yleftbulge = weights2[num6].yleftbulge;
				array5[num6].yrightbulge = weights2[num6].yrightbulge;
				array5[num6].zleftbulge = weights2[num6].zleftbulge;
				array5[num6].zrightbulge = weights2[num6].zrightbulge;
			}
			int[] fullyWeightedVertices2 = dAZSkinV2Node2.fullyWeightedVertices;
			int[] array6 = new int[fullyWeightedVertices2.Length];
			nodes[num5].fullyWeightedVertices = array6;
			for (int num7 = 0; num7 < array6.Length; num7++)
			{
				if (fullyWeightedVertices2[num7] >= dAZMergedMesh.numGraftBaseVertices)
				{
					array6[num7] = fullyWeightedVertices2[num7] + num4;
				}
				else
				{
					array6[num7] = fullyWeightedVertices2[num7] + startGraftVertIndex;
				}
			}
			DAZSkinV2GeneralVertexWeights[] generalWeights2 = dAZSkinV2Node2.generalWeights;
			DAZSkinV2GeneralVertexWeights[] array7 = new DAZSkinV2GeneralVertexWeights[generalWeights2.Length];
			nodes[num5].generalWeights = array7;
			for (int num8 = 0; num8 < array7.Length; num8++)
			{
				array7[num8] = new DAZSkinV2GeneralVertexWeights();
				if (generalWeights2[num8].vertex >= dAZMergedMesh.numGraftBaseVertices)
				{
					array7[num8].vertex = generalWeights2[num8].vertex + num4;
				}
				else
				{
					array7[num8].vertex = generalWeights2[num8].vertex + startGraftVertIndex;
				}
				array7[num8].weight = generalWeights2[num8].weight;
			}
		}
		if (!has2ndGraft)
		{
			return;
		}
		int startGraft2VertIndex = dAZMergedMesh.startGraft2VertIndex;
		int num9 = dAZMergedMesh.numTargetUVVertices + dAZMergedMesh.numGraftUVVertices;
		for (int num10 = 0; num10 < dAZSkinV3.numBones; num10++)
		{
			DAZSkinV2Node dAZSkinV2Node3 = dAZSkinV3.nodes[num10];
			int num11 = num + num2 + num10;
			nodes[num11] = new DAZSkinV2Node();
			nodes[num11].name = dAZSkinV2Node3.name;
			nodes[num11].rotationOrder = dAZSkinV2Node3.rotationOrder;
			nodes[num11].bulgeFactors = dAZSkinV2Node3.bulgeFactors;
			DAZSkinV2VertexWeights[] weights3 = dAZSkinV2Node3.weights;
			DAZSkinV2VertexWeights[] array8 = new DAZSkinV2VertexWeights[weights3.Length];
			nodes[num11].weights = array8;
			for (int num12 = 0; num12 < array8.Length; num12++)
			{
				array8[num12] = new DAZSkinV2VertexWeights();
				if (weights3[num12].vertex >= dAZMergedMesh.numGraft2BaseVertices)
				{
					array8[num12].vertex = weights3[num12].vertex + num9;
				}
				else
				{
					array8[num12].vertex = weights3[num12].vertex + startGraft2VertIndex;
				}
				array8[num12].xweight = weights3[num12].xweight;
				array8[num12].yweight = weights3[num12].yweight;
				array8[num12].zweight = weights3[num12].zweight;
				array8[num12].xleftbulge = weights3[num12].xleftbulge;
				array8[num12].xrightbulge = weights3[num12].xrightbulge;
				array8[num12].yleftbulge = weights3[num12].yleftbulge;
				array8[num12].yrightbulge = weights3[num12].yrightbulge;
				array8[num12].zleftbulge = weights3[num12].zleftbulge;
				array8[num12].zrightbulge = weights3[num12].zrightbulge;
			}
			int[] fullyWeightedVertices3 = dAZSkinV2Node3.fullyWeightedVertices;
			int[] array9 = new int[fullyWeightedVertices3.Length];
			nodes[num11].fullyWeightedVertices = array9;
			for (int num13 = 0; num13 < array9.Length; num13++)
			{
				if (fullyWeightedVertices3[num13] >= dAZMergedMesh.numGraft2BaseVertices)
				{
					array9[num13] = fullyWeightedVertices3[num13] + num9;
				}
				else
				{
					array9[num13] = fullyWeightedVertices3[num13] + startGraft2VertIndex;
				}
			}
			DAZSkinV2GeneralVertexWeights[] generalWeights3 = dAZSkinV2Node3.generalWeights;
			DAZSkinV2GeneralVertexWeights[] array10 = new DAZSkinV2GeneralVertexWeights[generalWeights3.Length];
			nodes[num11].generalWeights = array10;
			for (int num14 = 0; num14 < array10.Length; num14++)
			{
				array10[num14] = new DAZSkinV2GeneralVertexWeights();
				if (generalWeights3[num14].vertex >= dAZMergedMesh.numGraft2BaseVertices)
				{
					array10[num14].vertex = generalWeights3[num14].vertex + num9;
				}
				else
				{
					array10[num14].vertex = generalWeights3[num14].vertex + startGraft2VertIndex;
				}
				array10[num14].weight = generalWeights3[num14].weight;
			}
		}
	}
}
