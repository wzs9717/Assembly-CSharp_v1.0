using System.Diagnostics;
using UnityEngine;

public class GlobalStopwatch : MonoBehaviour
{
	public static GlobalStopwatch singleton;

	private Stopwatch stopwatch;

	private float f;

	public static float GetElapsedMilliseconds()
	{
		if (singleton != null)
		{
			return (float)singleton.stopwatch.ElapsedTicks * singleton.f;
		}
		return 0f;
	}

	private void Awake()
	{
		singleton = this;
		stopwatch = new Stopwatch();
		stopwatch.Start();
		f = 1000f / (float)Stopwatch.Frequency;
	}

	private void OnDestroy()
	{
		if (stopwatch != null)
		{
			stopwatch.Stop();
		}
	}
}
