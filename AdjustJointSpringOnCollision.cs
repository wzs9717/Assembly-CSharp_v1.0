using UnityEngine;

public class AdjustJointSpringOnCollision : MonoBehaviour
{
	public float springForceOnCollision;

	public float delay;

	private ConfigurableJoint cj;

	private JointDrive saveJointDrive;

	private bool timerOn;

	private float timer;

	private void OnCollisionEnter()
	{
		if ((bool)cj)
		{
			timerOn = false;
			JointDrive xDrive = cj.xDrive;
			xDrive.positionSpring = springForceOnCollision;
			cj.xDrive = xDrive;
		}
	}

	private void restoreDrive()
	{
		cj.xDrive = saveJointDrive;
	}

	private void OnCollisionExit()
	{
		if ((bool)cj)
		{
			timerOn = true;
			timer = delay;
		}
	}

	private void Start()
	{
		cj = GetComponent<ConfigurableJoint>();
		if ((bool)cj)
		{
			saveJointDrive = cj.xDrive;
		}
	}

	private void Update()
	{
		if (timerOn)
		{
			timer -= Time.deltaTime;
			if (timer < 0f)
			{
				timerOn = false;
				restoreDrive();
			}
		}
	}
}
