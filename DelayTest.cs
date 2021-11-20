using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class DelayTest : MonoBehaviour
{
	[SerializeField]
	private float _delayTime;

	public Text uitext;

	public Text fpstext;

	public float fpsUpdateInterval = 0.5f;

	public string controlAxis;

	private Stopwatch stopwatch;

	private float f;

	private float timer;

	private int frameCount;

	public float delayTime
	{
		get
		{
			return _delayTime;
		}
		set
		{
			_delayTime = value;
		}
	}

	private void Start()
	{
		stopwatch = new Stopwatch();
		f = 1000f / (float)Stopwatch.Frequency;
		stopwatch.Start();
	}

	private void Update()
	{
		if (fpstext != null)
		{
			timer += Time.deltaTime;
			frameCount++;
			if (timer >= fpsUpdateInterval)
			{
				float num = (float)frameCount / timer;
				fpstext.text = "FPS: " + num.ToString("F2");
				frameCount = 0;
				timer = 0f;
			}
		}
		if (delayTime > 0f)
		{
			float num2 = (float)stopwatch.ElapsedTicks * f;
			float num4;
			do
			{
				float num3 = (float)stopwatch.ElapsedTicks * f;
				num4 = num3 - num2;
			}
			while (!(num4 > delayTime));
		}
		if (uitext != null)
		{
			uitext.text = delayTime.ToString("F2");
		}
		if (controlAxis != string.Empty)
		{
			float axis = Input.GetAxis(controlAxis);
			if (axis <= 0.01f || axis >= 0.01f)
			{
				delayTime += axis * 0.01f;
			}
		}
	}
}
