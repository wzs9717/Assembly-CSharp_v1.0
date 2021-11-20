using System;

[Serializable]
public class DAZNode
{
	public string name;

	public Quaternion2Angles.RotationOrder rotationOrder;

	public DAZMeshVertexWeights[] weights;

	public int[] fullyWeightedVertices;

	public DAZBulgeFactors bulgeFactors;
}
