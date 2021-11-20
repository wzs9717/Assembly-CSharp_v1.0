using System;

[Serializable]
public class DAZSkinV2Node
{
	public string url;

	public string id;

	public string name;

	public Quaternion2Angles.RotationOrder rotationOrder;

	public DAZSkinV2VertexWeights[] weights;

	public DAZSkinV2GeneralVertexWeights[] generalWeights;

	public int[] fullyWeightedVertices;

	public DAZSkinV2BulgeFactors bulgeFactors;
}
