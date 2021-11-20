using System;
using UnityEngine;

[Serializable]
public class RhythmTool
{
	private AudioSource audioSource;

	public const int fftWindowSize = 4096;

	public const int frameSpacing = 1500;

	[SerializeField]
	private int totalFrames;

	[SerializeField]
	private int currentFrame;

	[SerializeField]
	private float interpolation;

	[SerializeField]
	private int lastFrame;

	[SerializeField]
	private int lead = 300;

	[SerializeField]
	private bool isDone;

	[SerializeField]
	private bool advancedAnalyses;

	[AnalysisList]
	public Analysis[] analyses;

	private bool preCalculate;

	private bool initialized;

	private int totalSamples;

	private int sampleIndex;

	private float[] floatSamples;

	private double[] doubleSamples;

	private float[] magnitude;

	private LomontFFT fft;

	public int TotalFrames => totalFrames;

	public int CurrentFrame => currentFrame;

	public float Interpolation => interpolation;

	public int LastFrame => lastFrame;

	public int Lead
	{
		get
		{
			return lead;
		}
		set
		{
			lead = Mathf.Max(lead, 40);
		}
	}

	public bool IsDone => isDone;

	public bool Initialized => initialized;

	public AudioSource Init(MonoBehaviour script, bool preCalculate)
	{
		this.preCalculate = preCalculate;
		floatSamples = new float[4096];
		doubleSamples = new double[4096];
		magnitude = new float[2047];
		fft = new LomontFFT();
		audioSource = script.gameObject.GetComponent<AudioSource>();
		if (audioSource == null)
		{
			audioSource = script.gameObject.AddComponent<AudioSource>();
		}
		if (!advancedAnalyses)
		{
			analyses = new Analysis[3];
			Analysis analysis = new Analysis();
			analysis.start = 0;
			analysis.end = 12;
			analysis.name = "Low";
			analyses[0] = analysis;
			analysis = new Analysis();
			analysis.start = 30;
			analysis.end = 200;
			analysis.name = "Mid";
			analyses[1] = analysis;
			analysis = new Analysis();
			analysis.start = 300;
			analysis.end = 550;
			analysis.name = "High";
			analyses[2] = analysis;
		}
		if (analyses.Length <= 0)
		{
			Debug.LogWarning("No analysis configured");
			return null;
		}
		initialized = true;
		return audioSource;
	}

	public bool NewSong(string songPath)
	{
		if (!initialized)
		{
			return false;
		}
		audioSource.Stop();
		audioSource.clip = Mp3Importer.Import(songPath);
		totalSamples = audioSource.clip.samples;
		totalFrames = totalSamples / 1500;
		currentFrame = 0;
		Analysis[] array = analyses;
		foreach (Analysis analysis in array)
		{
			analysis.Init(totalFrames, advancedAnalyses);
		}
		isDone = false;
		lastFrame = 0;
		initialized = true;
		return true;
	}

	public bool NewSong(AudioClip audioClip)
	{
		if (!initialized)
		{
			return false;
		}
		audioSource.Stop();
		audioSource.clip = audioClip;
		totalSamples = audioSource.clip.samples;
		totalFrames = totalSamples / 1500;
		currentFrame = 0;
		Analysis[] array = analyses;
		foreach (Analysis analysis in array)
		{
			analysis.Init(totalFrames, advancedAnalyses);
		}
		isDone = false;
		lastFrame = 0;
		initialized = true;
		return true;
	}

	public void Play()
	{
		if (audioSource != null)
		{
			audioSource.Play();
		}
	}

	public void Stop()
	{
		if (audioSource != null)
		{
			audioSource.Stop();
		}
	}

	private void EndOfAnalysis()
	{
		if (preCalculate)
		{
		}
		isDone = true;
	}

	public void Update()
	{
		if (!initialized)
		{
			Debug.LogWarning("RhythmTool not initialized");
			return;
		}
		sampleIndex = audioSource.timeSamples;
		float num = (float)sampleIndex / (float)totalSamples;
		currentFrame = (int)(num * (float)totalFrames);
		interpolation = num * (float)totalFrames;
		interpolation -= currentFrame;
		if (isDone)
		{
			return;
		}
		if (preCalculate)
		{
			lead += 500;
		}
		lead = Mathf.Max(40, lead);
		for (int i = lastFrame + 1; i < currentFrame + lead; i++)
		{
			if (i >= totalFrames)
			{
				EndOfAnalysis();
				break;
			}
			audioSource.clip.GetData(floatSamples, i * 1500);
			Util.FloatsToDoubles(floatSamples, doubleSamples);
			fft.RealFFT(doubleSamples, forward: true);
			Util.SpectrumMagnitude(doubleSamples, magnitude);
			Analysis[] array = analyses;
			foreach (Analysis analysis in array)
			{
				analysis.Analyze(magnitude, i);
			}
			lastFrame = i;
		}
	}

	public Frame[] GetResults(string name)
	{
		Analysis[] array = analyses;
		foreach (Analysis analysis in array)
		{
			if (analysis.name == name)
			{
				return analysis.Frames;
			}
		}
		Debug.LogWarning("Analysis " + name + " not found.");
		return null;
	}

	public void DrawDebugLines()
	{
		audioSource.clip.GetData(floatSamples, currentFrame * 1500);
		Util.FloatsToDoubles(floatSamples, doubleSamples);
		fft.RealFFT(doubleSamples, forward: true);
		Util.SpectrumMagnitude(doubleSamples, magnitude);
		for (int i = 0; i < magnitude.Length - 1; i++)
		{
			Vector3 start = new Vector3(i * -1, magnitude[i] * (1f + (float)i * 0.05f), 0f);
			Vector3 end = new Vector3((i + 1) * -1, magnitude[i + 1] * (1f + (float)i * 0.05f), 0f);
			Debug.DrawLine(start, end, Color.green);
		}
		for (int j = 0; j < analyses.Length; j++)
		{
			analyses[j].DrawDebugLines(currentFrame, j);
		}
	}
}
