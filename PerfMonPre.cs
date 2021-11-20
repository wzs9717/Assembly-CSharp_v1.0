using UnityEngine;

public class PerfMonPre : MonoBehaviour
{
	public static float frameStartTime;

	public static float physicsStartTime;

	public static float updateStartTime;

	public static float lastPhysicsTime;

	public static float physicsTime;

	protected bool updated;

	private void FixedUpdate()
	{
		frameStartTime = GlobalStopwatch.GetElapsedMilliseconds();
		physicsStartTime = frameStartTime;
		updated = true;
	}

	private void Update()
	{
		updateStartTime = GlobalStopwatch.GetElapsedMilliseconds();
		if (!updated)
		{
			physicsTime = 0f;
			frameStartTime = updateStartTime;
		}
		else
		{
			physicsTime = updateStartTime - physicsStartTime;
			lastPhysicsTime = physicsTime;
		}
		updated = false;
	}
}
