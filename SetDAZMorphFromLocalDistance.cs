using UnityEngine;

public class SetDAZMorphFromLocalDistance : SetDAZMorph
{
	public enum axis
	{
		X,
		Y,
		Z
	}

	public Transform movingTransform;

	public axis distanceAxis;

	public float distanceLow;

	public float distanceHigh = 0.1f;

	public float currentDistance;

	private Vector3 startingLocalPosition;

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
		if (movingTransform != null && morph1 != null)
		{
			switch (distanceAxis)
			{
			case axis.X:
				currentDistance = movingTransform.localPosition.x - startingLocalPosition.x;
				break;
			case axis.Y:
				currentDistance = movingTransform.localPosition.y - startingLocalPosition.y;
				break;
			case axis.Z:
				currentDistance = movingTransform.localPosition.z - startingLocalPosition.z;
				break;
			}
			float num = (currentDistance * _multiplier - distanceLow) / (distanceHigh - distanceLow);
			float num2 = num;
			if (clampMorphValue)
			{
				num2 = Mathf.Clamp(num, 0f, 1f);
			}
			currentMorph1Value = morph1Low + (morph1High - morph1Low) * num2;
			morph1.morphValue = currentMorph1Value;
		}
	}

	private void Start()
	{
		startingLocalPosition = movingTransform.localPosition;
	}
}
