using UnityEngine;

public class RhythmController : MonoBehaviour
{
	public static RhythmController singleton;

	public AudioClip[] audioClip;

	public RhythmTool rhythmTool;

	public Frame[] low;

	public Frame[] mid;

	public Frame[] high;

	private int index;

	private void Awake()
	{
		singleton = this;
	}

	private void Start()
	{
		rhythmTool.Init(this, preCalculate: false);
		if (audioClip != null)
		{
			index = 0;
			StartSong();
		}
	}

	private void StartSong()
	{
		rhythmTool.NewSong(audioClip[index]);
		rhythmTool.Play();
		low = rhythmTool.GetResults("Low");
		mid = rhythmTool.GetResults("Mid");
		high = rhythmTool.GetResults("High");
	}

	private void Update()
	{
		rhythmTool.Update();
		if (rhythmTool.CurrentFrame == rhythmTool.TotalFrames - 1)
		{
			index++;
			if (index >= audioClip.Length)
			{
				index = 0;
			}
			StartSong();
		}
	}
}
