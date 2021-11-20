using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PhysicsSimulator : MonoBehaviour
{
	protected bool _pauseSimulation;

	protected List<AsyncFlag> waitResumeSimulationFlags;

	protected int delayResumeSimulation;

	protected bool pauseSimulation
	{
		get
		{
			return _pauseSimulation;
		}
		set
		{
			if (_pauseSimulation != value)
			{
				_pauseSimulation = value;
				SyncPauseSimulation();
			}
		}
	}

	protected virtual void SyncPauseSimulation()
	{
	}

	protected void DelayResumeSimulation(int count)
	{
		if (count > delayResumeSimulation)
		{
			delayResumeSimulation = count;
		}
	}

	protected void CheckResumeSimulation()
	{
		if (delayResumeSimulation >= 0)
		{
			delayResumeSimulation--;
		}
		if (waitResumeSimulationFlags == null)
		{
			waitResumeSimulationFlags = new List<AsyncFlag>();
		}
		bool flag = false;
		if (waitResumeSimulationFlags.Count > 0)
		{
			List<AsyncFlag> list = new List<AsyncFlag>();
			foreach (AsyncFlag waitResumeSimulationFlag in waitResumeSimulationFlags)
			{
				if (waitResumeSimulationFlag.flag)
				{
					list.Add(waitResumeSimulationFlag);
					flag = true;
				}
			}
			foreach (AsyncFlag item in list)
			{
				waitResumeSimulationFlags.Remove(item);
			}
		}
		if (delayResumeSimulation > 0 || waitResumeSimulationFlags.Count > 0)
		{
			pauseSimulation = true;
		}
		else if (delayResumeSimulation == 0 || flag)
		{
			pauseSimulation = false;
		}
	}

	public bool IsSimulationPaused()
	{
		return _pauseSimulation;
	}

	public void PauseSimulation()
	{
		pauseSimulation = true;
	}

	public void PauseSimulation(AsyncFlag waitFor)
	{
		if (waitResumeSimulationFlags == null)
		{
			waitResumeSimulationFlags = new List<AsyncFlag>();
		}
		waitResumeSimulationFlags.Add(waitFor);
		pauseSimulation = true;
	}

	public void PauseSimulation(int numFrames)
	{
		DelayResumeSimulation(numFrames);
		pauseSimulation = true;
	}

	protected virtual void Update()
	{
		if (Application.isPlaying)
		{
			CheckResumeSimulation();
		}
	}
}
