using UnityEngine;

public class SetDAZMorphFromDistance : SetDAZMorph
{
	public Transform transform1;

	public Transform transform2;

	public float distanceLow;

	public float distanceHigh = 0.1f;

	public float currentDistance;

	public bool lerpOverTime;

	public float lerpRate = 10f;

	private void Update()
	{
		if (transform1 != null && transform2 != null && morph1 != null)
		{
			currentDistance = (transform1.position - transform2.position).magnitude / base.transform.lossyScale.x;
			float num = Mathf.Clamp(currentDistance, distanceLow, distanceHigh) - distanceLow;
			float t = num / (distanceHigh - distanceLow);
			float b = Mathf.Lerp(morph1Low, morph1High, t);
			if (lerpOverTime)
			{
				currentMorph1Value = Mathf.Lerp(currentMorph1Value, b, lerpRate * Time.deltaTime / Time.timeScale);
			}
			else
			{
				currentMorph1Value = b;
			}
			morph1.morphValue = currentMorph1Value;
		}
	}
}
