using UnityEngine;

public class SetDAZMorphFromAverageBoneAngle : SetDAZMorph
{
	public enum axis
	{
		X,
		Y,
		Z,
		NegX,
		NegY,
		NegZ
	}

	public DAZBone dazBone1;

	public DAZBone dazBone2;

	public float angleLow;

	public float angleHigh = 20f;

	public axis angleAxis1;

	public axis angleAxis2;

	public float currentAxisAngle1;

	public float currentAxisAngle2;

	public float _multiplier = 1f;

	public bool clampMorphValue = true;

	public float multiplier
	{
		get
		{
			return _multiplier;
		}
		set
		{
			_multiplier = value;
		}
	}

	private void Update()
	{
		if (dazBone1 != null && dazBone2 != null)
		{
			Vector3 anglesDegrees = dazBone1.GetAnglesDegrees();
			Vector3 anglesDegrees2 = dazBone2.GetAnglesDegrees();
			switch (angleAxis1)
			{
			case axis.X:
				currentAxisAngle1 = anglesDegrees.x;
				break;
			case axis.Y:
				currentAxisAngle1 = anglesDegrees.y;
				break;
			case axis.Z:
				currentAxisAngle1 = anglesDegrees.z;
				break;
			case axis.NegX:
				currentAxisAngle1 = 0f - anglesDegrees.x;
				break;
			case axis.NegY:
				currentAxisAngle1 = 0f - anglesDegrees.y;
				break;
			case axis.NegZ:
				currentAxisAngle1 = 0f - anglesDegrees.z;
				break;
			}
			switch (angleAxis2)
			{
			case axis.X:
				currentAxisAngle2 = anglesDegrees2.x;
				break;
			case axis.Y:
				currentAxisAngle2 = anglesDegrees2.y;
				break;
			case axis.Z:
				currentAxisAngle2 = anglesDegrees2.z;
				break;
			case axis.NegX:
				currentAxisAngle2 = 0f - anglesDegrees2.x;
				break;
			case axis.NegY:
				currentAxisAngle2 = 0f - anglesDegrees2.y;
				break;
			case axis.NegZ:
				currentAxisAngle2 = 0f - anglesDegrees2.z;
				break;
			}
			float num = ((currentAxisAngle1 + currentAxisAngle2) * 0.5f * _multiplier - angleLow) / (angleHigh - angleLow);
			float num2 = num;
			if (clampMorphValue)
			{
				num2 = Mathf.Clamp(num, 0f, 1f);
			}
			if (morph1 != null)
			{
				currentMorph1Value = morph1Low + (morph1High - morph1Low) * num2;
				morph1.morphValue = currentMorph1Value;
			}
		}
	}
}
