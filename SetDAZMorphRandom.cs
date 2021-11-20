using UnityEngine;

public class SetDAZMorphRandom : SetDAZMorph
{
	public float frequency = 2f;

	public float frequencyRandomness = 1f;

	public float lerpUpFactor = 1f;

	public float lerpDownFactor = 1f;

	public float currentValue;

	public float targetValue;

	private float countdown;

	private void Update()
	{
		if (countdown < 0f)
		{
			countdown = Random.Range(frequency - frequencyRandomness, frequency + frequencyRandomness);
			targetValue = Random.Range(0f, 1f);
		}
		else
		{
			countdown -= Time.deltaTime;
		}
		if (currentValue < targetValue)
		{
			currentValue = Mathf.Lerp(currentValue, targetValue, Time.deltaTime * lerpUpFactor);
		}
		else
		{
			currentValue = Mathf.Lerp(currentValue, targetValue, Time.deltaTime * lerpDownFactor);
		}
		if (morph1 != null)
		{
			currentMorph1Value = Mathf.Lerp(morph1Low, morph1High, currentValue);
			morph1.morphValue = currentMorph1Value;
		}
	}
}
