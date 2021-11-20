using UnityEngine;

[ExecuteInEditMode]
public class SetMorphFromDistance : MonoBehaviour
{
	public SkinnedMeshRenderer skin;

	public bool resetStart;

	public string morphName;

	public float distanceLow;

	public float distanceHigh;

	public float morphLow;

	public float morphHigh;

	public float currentDistance;

	public float currentWeight;

	private Vector3 startLocalPosition;

	private void Start()
	{
		startLocalPosition = base.transform.localPosition;
	}

	private void setWeight()
	{
		if ((bool)skin)
		{
			currentDistance = Vector3.Magnitude(base.transform.localPosition - startLocalPosition);
			float num = (currentDistance - distanceLow) / (distanceHigh - distanceLow);
			int blendShapeIndex = skin.sharedMesh.GetBlendShapeIndex(morphName);
			if (blendShapeIndex != -1)
			{
				currentWeight = morphLow + (morphHigh - morphLow) * num;
				currentWeight = Mathf.Clamp(currentWeight, morphLow, morphHigh);
				skin.SetBlendShapeWeight(blendShapeIndex, currentWeight);
			}
		}
	}

	private void Update()
	{
		if (resetStart)
		{
			resetStart = false;
			Start();
		}
		setWeight();
	}
}
