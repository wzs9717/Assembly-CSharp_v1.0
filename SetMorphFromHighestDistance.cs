using UnityEngine;

[ExecuteInEditMode]
public class SetMorphFromHighestDistance : MonoBehaviour
{
	public SkinnedMeshRenderer skin;

	public string morphName;

	public Transform t1;

	public Transform t2;

	public Transform refTransform;

	public float distanceLow;

	public float distanceHigh;

	public float morphLow;

	public float morphHigh;

	public float currentDistance1;

	public float currentDistance2;

	public float currentWeight;

	private void Start()
	{
	}

	private void setWeight()
	{
		if (!t1 || !t2 || !refTransform || !skin)
		{
			return;
		}
		currentDistance1 = Vector3.Magnitude(t1.position - refTransform.position);
		currentDistance2 = Vector3.Magnitude(t2.position - refTransform.position);
		float num = ((!(currentDistance1 > currentDistance2)) ? currentDistance2 : currentDistance1);
		float num2 = (num - distanceLow) / (distanceHigh - distanceLow);
		int blendShapeIndex = skin.sharedMesh.GetBlendShapeIndex(morphName);
		if (blendShapeIndex != -1)
		{
			currentWeight = morphLow + (morphHigh - morphLow) * num2;
			if (morphHigh > morphLow)
			{
				currentWeight = Mathf.Clamp(currentWeight, morphLow, morphHigh);
			}
			else
			{
				currentWeight = Mathf.Clamp(currentWeight, morphHigh, morphLow);
			}
			skin.SetBlendShapeWeight(blendShapeIndex, currentWeight);
		}
	}

	private void Update()
	{
		setWeight();
	}
}
