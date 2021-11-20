using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Analysis
{
	public string name;

	private float thresholdMultiplier = 1.5f;

	private int thresholdWindowSize = 10;

	private Frame[] frames;

	public int start;

	public int end;

	public AnimationCurve weightCurve;

	private int totalFrames;

	public bool advancedAnalysis;

	public Frame[] Frames => frames;

	public void Init(int totalFrames, bool advancedAnalysis)
	{
		this.totalFrames = totalFrames;
		this.advancedAnalysis = advancedAnalysis;
		frames = new Frame[totalFrames];
		int num = 2047;
		if (end < start || start < 0 || end < 0 || start >= num || end > num)
		{
			Debug.LogError("Invalid range for analysis " + name + ". Range must be within Spectrum (fftWindowSize/2 - 1) and start cannot come after end.");
		}
	}

	public void Analyze(float[] spectrum, int index)
	{
		int num = 0;
		frames[index].magnitude = Sum(spectrum, start, end);
		frames[index].magnitudeSmooth = frames[index].magnitude;
		num = Mathf.Max(index - 10, 0);
		frames[num].flux = frames[num].magnitude - frames[Mathf.Max(num - 1, 0)].magnitude;
		Smooth(num, 10);
		num = Mathf.Max(index - 20, 0);
		frames[num].threshold = Threshold(frames, num, thresholdMultiplier, thresholdWindowSize);
		Smooth(num, 10);
		num = Mathf.Max(index - 30, 0);
		if (frames[num].flux > frames[num].threshold && frames[num].flux > frames[Mathf.Min(num + 1, frames.Length - 1)].flux && frames[num].flux > frames[Mathf.Max(num - 1, 0)].flux)
		{
			frames[num].onset = frames[num].flux;
		}
		num = Mathf.Max(index - 100, 0);
		Rank(num, 50);
	}

	public void DrawDebugLines(int index, int h)
	{
		for (int i = 0; i < 299 && i + 1 + index <= totalFrames - 1; i++)
		{
			Vector3 vector = new Vector3(i, frames[i + index].magnitude + (float)(h * 100), 0f);
			Vector3 vector2 = new Vector3(i + 1, frames[i + 1 + index].magnitude + (float)(h * 100), 0f);
			Debug.DrawLine(vector, vector2, Color.red);
			vector = new Vector3(i, frames[i + index].magnitudeSmooth + (float)(h * 100), 0f);
			vector2 = new Vector3(i + 1, frames[i + 1 + index].magnitudeSmooth + (float)(h * 100), 0f);
			Debug.DrawLine(vector, vector2, Color.red);
			vector = new Vector3(i, frames[i + index].flux + (float)(h * 100), 0f);
			vector2 = new Vector3(i + 1, frames[i + 1 + index].flux + (float)(h * 100), 0f);
			Debug.DrawLine(vector, vector2, Color.blue);
			vector = new Vector3(i, frames[i + index].threshold + (float)(h * 100), 0f);
			vector2 = new Vector3(i + 1, frames[i + 1 + index].threshold + (float)(h * 100), 0f);
			Debug.DrawLine(vector, vector2, Color.blue);
			vector = new Vector3(i, frames[i + index].onset + (float)(h * 100), 0f);
			vector2 = new Vector3(i + 1, frames[i + 1 + index].onset + (float)(h * 100), 0f);
			Debug.DrawLine(vector, vector2, Color.yellow);
			vector = new Vector3(i, -frames[i + index].onsetRank + h * 100, 0f);
			vector2 = new Vector3(i + 1, -frames[i + 1 + index].onsetRank + h * 100, 0f);
			Debug.DrawLine(vector, vector2, Color.white);
		}
	}

	private float Threshold(Frame[] input, int index, float multiplier, int windowSize)
	{
		int num = Mathf.Max(0, index - windowSize);
		int num2 = Mathf.Min(input.Length - 1, index + windowSize);
		float num3 = 0f;
		for (int i = num; i <= num2; i++)
		{
			num3 += Mathf.Abs(input[i].flux);
		}
		num3 /= (float)(num2 - num);
		return num3 * multiplier;
	}

	private float Sum(float[] input, int start, int end)
	{
		float num = 0f;
		for (int i = start; i < end; i++)
		{
			num = ((!advancedAnalysis) ? (num + input[i]) : (num + input[i] * weightCurve.Evaluate(i)));
		}
		return num;
	}

	private float Average(float[] input, int start, int end)
	{
		float num = 0f;
		for (int i = start; i < end; i++)
		{
			num += input[i];
		}
		return num / (float)(end - start);
	}

	private void Smooth(int index, int windowSize)
	{
		float num = 0f;
		for (int i = index - windowSize / 2; i < index + windowSize / 2; i++)
		{
			if (i > 0 && i < totalFrames)
			{
				num += frames[i].magnitudeSmooth;
			}
		}
		frames[index].magnitudeSmooth = num / (float)windowSize;
	}

	private void Rank(int index, int windowSize)
	{
		if (frames[index].onset == 0f)
		{
			return;
		}
		List<Frame> list = new List<Frame>();
		for (int i = 0; i < 5; i++)
		{
			int num = 0;
			int num2 = 0;
			for (int j = index - windowSize / 2; j < index - 1; j++)
			{
				if (j > 0 && j < totalFrames && frames[j].onset > 0f && !list.Contains(frames[j]))
				{
					num = j;
				}
			}
			for (int k = index + 1; k < index + windowSize / 2; k++)
			{
				if (k > 0 && k < totalFrames && frames[k].onset > 0f && !list.Contains(frames[k]))
				{
					num2 = k;
					break;
				}
			}
			if (frames[index].onset > frames[num].onset && frames[index].onset > frames[num2].onset)
			{
				frames[index].onsetRank = 5 - i;
				break;
			}
			if (frames[index].onset < frames[num].onset)
			{
				list.Add(frames[num]);
			}
			if (frames[index].onset < frames[num2].onset)
			{
				list.Add(frames[num2]);
			}
			if (num == 0 && num2 == 0)
			{
				frames[index].onsetRank = 5;
			}
		}
	}
}
