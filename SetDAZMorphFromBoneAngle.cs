using UnityEngine;

public class SetDAZMorphFromBoneAngle : SetDAZMorph
{
	public enum axis
	{
		X,
		Y,
		Z
	}

	public DAZBone dazBone;

	public float angleLow;

	public float angleHigh = 20f;

	public axis angleAxis;

	public float currentAxisAngle;

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
		if (dazBone != null)
		{
			Vector3 anglesDegrees = dazBone.GetAnglesDegrees();
			switch (angleAxis)
			{
			case axis.X:
				currentAxisAngle = anglesDegrees.x;
				break;
			case axis.Y:
				currentAxisAngle = anglesDegrees.y;
				break;
			case axis.Z:
				currentAxisAngle = anglesDegrees.z;
				break;
			}
			float num = (currentAxisAngle * _multiplier - angleLow) / (angleHigh - angleLow);
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
