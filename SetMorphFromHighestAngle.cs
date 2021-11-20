using UnityEngine;

[ExecuteInEditMode]
public class SetMorphFromHighestAngle : MonoBehaviour
{
	public SkinnedMeshRenderer skin;

	public bool resetStart;

	public string morphName;

	public Transform t1;

	public Transform t2;

	public float angleLow;

	public float angleHigh;

	public float morphLow;

	public float morphHigh;

	public float currentAngle1;

	public float currentAngle2;

	public float currentWeight;

	private Quaternion originalT1Rotation;

	private Quaternion originalT2Rotation;

	private void Start()
	{
		originalT1Rotation = t1.localRotation;
		originalT2Rotation = t2.localRotation;
	}

	private void setWeight()
	{
		if (!t1 || !t2 || !skin)
		{
			return;
		}
		currentAngle1 = Quaternion.Angle(originalT1Rotation, t1.localRotation);
		currentAngle2 = Quaternion.Angle(originalT2Rotation, t2.localRotation);
		float num = ((!(currentAngle1 > currentAngle2)) ? currentAngle2 : currentAngle1);
		float num2 = (num - angleLow) / (angleHigh - angleLow);
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
		if (resetStart)
		{
			resetStart = false;
			Start();
		}
		setWeight();
	}
}
