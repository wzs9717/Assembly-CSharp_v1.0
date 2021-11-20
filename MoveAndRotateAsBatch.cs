using UnityEngine;

public class MoveAndRotateAsBatch : MonoBehaviour
{
	public enum UpdateTime
	{
		Normal,
		Late,
		Fixed
	}

	public UpdateTime updateTime;

	protected MoveAndRotateAs[] comps;

	protected void DoUpdate()
	{
		MoveAndRotateAs[] array = comps;
		foreach (MoveAndRotateAs moveAndRotateAs in array)
		{
			moveAndRotateAs.DoUpdate();
		}
	}

	private void Start()
	{
		comps = GetComponentsInChildren<MoveAndRotateAs>();
	}

	private void FixedUpdate()
	{
		if (updateTime == UpdateTime.Fixed)
		{
			DoUpdate();
		}
	}

	private void Update()
	{
		if (updateTime == UpdateTime.Normal)
		{
			DoUpdate();
		}
	}

	private void LateUpdate()
	{
		if (updateTime == UpdateTime.Late)
		{
			DoUpdate();
		}
	}
}
