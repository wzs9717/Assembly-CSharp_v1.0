using System.Collections.Generic;
using UnityEngine;

public class HUDText : MonoBehaviour
{
	public float SplashTime = 2f;

	public int DisplayLines = 10;

	public bool StartHidden = true;

	public float UpdateInterval = 1f;

	private bool timerOn;

	private float timer;

	private float timer2;

	private MeshRenderer mr;

	private TextMesh tm;

	private bool hidden;

	private List<string> lines;

	private bool textNeedsUpdate;

	private void Start()
	{
		lines = new List<string>();
		mr = GetComponent<MeshRenderer>();
		tm = GetComponent<TextMesh>();
		if (StartHidden)
		{
			Hide();
		}
		else
		{
			Unhide();
		}
	}

	public void Splash()
	{
		if (hidden)
		{
			timerOn = true;
			timer = 2f;
			if (mr != null)
			{
				mr.enabled = true;
			}
		}
	}

	public void Hide()
	{
		if (mr != null)
		{
			mr.enabled = false;
			hidden = true;
		}
	}

	public void Unhide()
	{
		timerOn = false;
		if (mr != null)
		{
			mr.enabled = true;
			hidden = false;
		}
	}

	public string GetText()
	{
		if (tm != null)
		{
			return tm.text;
		}
		return string.Empty;
	}

	public void SetText(string text)
	{
		if (tm != null)
		{
			lines.Clear();
			lines.Add(text);
			tm.text = text;
		}
	}

	public void AppendTextLine(string text)
	{
		if (tm != null)
		{
			lines.Add(text);
			textNeedsUpdate = true;
		}
	}

	private void Update()
	{
		if (timerOn)
		{
			timer -= Time.deltaTime;
			if (timer < 0f)
			{
				Hide();
				timerOn = false;
			}
		}
		timer2 -= Time.deltaTime;
		if (!(timer2 < 0f))
		{
			return;
		}
		timer2 = UpdateInterval;
		if (textNeedsUpdate)
		{
			int num = lines.Count - DisplayLines;
			int count = lines.Count;
			if (num < 0)
			{
				num = 0;
			}
			tm.text = string.Empty;
			for (int i = num; i < count; i++)
			{
				TextMesh textMesh = tm;
				textMesh.text = textMesh.text + "\n" + lines[i];
			}
		}
	}
}
