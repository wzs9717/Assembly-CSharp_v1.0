using UnityEngine;

public static class Util
{
	public static void SpectrumMagnitude(float[] spectrum, float[] output)
	{
		for (int i = 0; i < output.Length; i++)
		{
			int num = i * 2 + 2;
			output[i] = Mathf.Sqrt(spectrum[num] * spectrum[num] + spectrum[num + 1] * spectrum[num + 1]);
		}
	}

	public static void SpectrumMagnitude(double[] spectrum, float[] output)
	{
		for (int i = 0; i < output.Length; i++)
		{
			int num = i * 2 + 2;
			float num2 = (float)spectrum[num];
			float num3 = (float)spectrum[num + 1];
			output[i] = Mathf.Sqrt(num2 * num2 + num3 * num3);
		}
	}

	public static void FloatsToDoubles(float[] input, double[] output)
	{
		for (int i = 0; i < input.Length; i++)
		{
			output[i] = input[i];
		}
	}
}
