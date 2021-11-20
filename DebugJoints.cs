using UnityEngine;

[ExecuteInEditMode]
public class DebugJoints : MonoBehaviour
{
	private bool _showJoints;

	private DebugJoint[] debugJoints;

	public bool showJoints
	{
		get
		{
			return _showJoints;
		}
		set
		{
			if (_showJoints != value)
			{
				_showJoints = value;
				SyncJoints();
			}
		}
	}

	private void SyncJoints()
	{
		if (debugJoints == null)
		{
			return;
		}
		DebugJoint[] array = debugJoints;
		foreach (DebugJoint debugJoint in array)
		{
			if (_showJoints)
			{
				debugJoint.gameObject.SetActive(value: true);
			}
			else
			{
				debugJoint.gameObject.SetActive(value: false);
			}
		}
	}

	public void InitJoints()
	{
		debugJoints = GetComponentsInChildren<DebugJoint>(includeInactive: true);
	}

	private void Start()
	{
		InitJoints();
		SyncJoints();
	}
}
