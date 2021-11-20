using UnityEngine;

[ExecuteInEditMode]
public class SetMorphFromAngle : MonoBehaviour
{
	public SkinnedMeshRenderer skin;

	public string morphName;

	public Transform t1;

	public Transform t2;

	public float angleLow;

	public float angleHigh;

	public float morphLow;

	public float morphHigh;

	public float currentAngle;

	public float currentWeight;

	private void Start()
	{
	}

	private void setWeight()
	{
		if (!t1 || !t2 || !skin)
		{
			return;
		}
		currentAngle = Quaternion.Angle(t1.rotation, t2.rotation);
		float num = (currentAngle - angleLow) / (angleHigh - angleLow);
		int blendShapeIndex = skin.sharedMesh.GetBlendShapeIndex(morphName);
		if (blendShapeIndex != -1)
		{
			currentWeight = morphLow + (morphHigh - morphLow) * num;
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
