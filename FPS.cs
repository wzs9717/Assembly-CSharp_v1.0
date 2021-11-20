using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour
{
	public string fps;

	public float UpdateIntervalInMilliseconds = 500f;

	public Text text;

	private float TimeLeft;

	private float Accum;

	private int Frames;

	private float lastUpdateTime;

	private void UpdateFPS()
	{
		float elapsedMilliseconds = GlobalStopwatch.GetElapsedMilliseconds();
		float num = elapsedMilliseconds - lastUpdateTime;
		TimeLeft -= num;
		Accum += 1000f / num;
		lastUpdateTime = elapsedMilliseconds;
		Frames++;
		if ((double)TimeLeft <= 0.0 && Frames != 0)
		{
			float num2 = Accum / (float)Frames;
			fps = $"FPS: {num2:F2}";
			if ((bool)text)
			{
				text.text = fps;
			}
			TimeLeft += UpdateIntervalInMilliseconds;
			Accum = 0f;
			Frames = 0;
		}
	}

	private void Update()
	{
		UpdateFPS();
	}
}
