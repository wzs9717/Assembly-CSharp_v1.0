using System.Collections.Generic;
using UnityEngine;

public class AutoColliderBatchUpdater : MonoBehaviour
{
	protected AutoCollider[] _autoColliders;

	public bool clumpUpdate;

	public DAZSkinV2 skin;

	protected bool _on = true;

	private bool _pauseSimulation;

	protected List<AsyncFlag> waitResumeSimulationFlags;

	protected int delayResumeSimulation;

	public DAZMorphBank morphBank1ForResizeTrigger;

	public DAZMorphBank morphBank2ForResizeTrigger;

	protected bool morphsChanged;

	public bool on
	{
		get
		{
			return _on;
		}
		set
		{
			if (_on != value)
			{
				_on = value;
			}
		}
	}

	public bool pauseSimulation
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
			}
		}
	}

	public void UpdateAutoColliders()
	{
		AutoCollider[] componentsInChildren = GetComponentsInChildren<AutoCollider>();
		List<AutoCollider> list = new List<AutoCollider>();
		AutoCollider[] array = componentsInChildren;
		foreach (AutoCollider autoCollider in array)
		{
			if (autoCollider.allowBatchUpdate)
			{
				list.Add(autoCollider);
			}
		}
		_autoColliders = list.ToArray();
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
			AutoCollider[] autoColliders = _autoColliders;
			foreach (AutoCollider autoCollider in autoColliders)
			{
				autoCollider.ResetJoints();
				autoCollider.pauseSimulation = false;
			}
			pauseSimulation = false;
		}
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

	protected void UpdateAnchors()
	{
		if (_autoColliders == null || _autoColliders.Length <= 0 || !(skin != null))
		{
			return;
		}
		Vector3[] rawSkinnedVerts = skin.rawSkinnedVerts;
		AutoCollider[] autoColliders = _autoColliders;
		foreach (AutoCollider autoCollider in autoColliders)
		{
			if (autoCollider.centerJoint)
			{
				if (autoCollider.lookAtOption == AutoCollider.LookAtOption.Opposite && autoCollider.oppositeVertex != -1)
				{
					if (autoCollider.joint != null)
					{
						autoCollider.anchorTarget = (rawSkinnedVerts[autoCollider.targetVertex] + rawSkinnedVerts[autoCollider.oppositeVertex]) * 0.5f;
						autoCollider.joint.connectedAnchor = autoCollider.backForceRigidbody.transform.InverseTransformPoint(autoCollider.anchorTarget);
					}
				}
				else if (autoCollider.lookAtOption == AutoCollider.LookAtOption.AnchorCenters && autoCollider.anchorVertex1 != -1 && autoCollider.anchorVertex2 != -1 && autoCollider.joint != null)
				{
					autoCollider.anchorTarget = (rawSkinnedVerts[autoCollider.anchorVertex1] + rawSkinnedVerts[autoCollider.anchorVertex2]) * 0.5f;
					autoCollider.joint.connectedAnchor = autoCollider.backForceRigidbody.transform.InverseTransformPoint(autoCollider.anchorTarget);
				}
			}
			else if (autoCollider.joint != null)
			{
				autoCollider.anchorTarget = rawSkinnedVerts[autoCollider.targetVertex];
				autoCollider.joint.connectedAnchor = autoCollider.backForceRigidbody.transform.InverseTransformPoint(autoCollider.anchorTarget);
			}
			if (autoCollider.debug)
			{
				MyDebug.DrawWireCube(autoCollider.anchorTarget, 0.005f, Color.blue);
			}
		}
	}

	private void OnEnable()
	{
		UpdateAutoColliders();
		AutoCollider[] autoColliders = _autoColliders;
		foreach (AutoCollider autoCollider in autoColliders)
		{
			autoCollider.enabled = false;
		}
	}

	private void OnDisable()
	{
		AutoCollider[] autoColliders = _autoColliders;
		foreach (AutoCollider autoCollider in autoColliders)
		{
			autoCollider.enabled = true;
		}
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			CheckResumeSimulation();
		}
		if (skin != null && (skin.dazMesh.visibleVerticesChangedLastFrame || skin.dazMesh.visibleVerticesChangedThisFrame))
		{
			morphsChanged = true;
		}
	}

	private void FixedUpdate()
	{
		if (!_on || !clumpUpdate || pauseSimulation)
		{
			return;
		}
		UpdateAnchors();
		if (morphsChanged)
		{
			morphsChanged = false;
			AutoCollider[] autoColliders = _autoColliders;
			foreach (AutoCollider autoCollider in autoColliders)
			{
				autoCollider.AutoColliderSizeSet();
			}
		}
	}
}
