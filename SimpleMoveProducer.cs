using UnityEngine;

public class SimpleMoveProducer : MoveProducer
{
	protected enum State
	{
		Pause,
		Transition,
		Complete
	}

	public Vector3[] positions;

	public Quaternion[] rotations1;

	public float[] pauseTimes;

	public float transitionTime = 1f;

	public bool loop = true;

	public bool reverseAtEnd;

	protected bool reversed;

	protected int index;

	protected float pauseTimer;

	protected float transitionTimer;

	protected State currentState;

	protected override void Update()
	{
		if (positions == null || positions.Length <= 1)
		{
			return;
		}
		State state = currentState;
		if (currentState == State.Pause)
		{
			if (pauseTimer > 0f)
			{
				pauseTimer -= Time.deltaTime;
			}
			else
			{
				pauseTimer = 0f;
				if (reversed)
				{
					if (index == 0)
					{
						reversed = false;
						index++;
					}
					else
					{
						index--;
					}
					state = State.Transition;
					transitionTimer = transitionTime;
				}
				else if (index == positions.Length)
				{
					if (loop)
					{
						if (reverseAtEnd)
						{
							reversed = true;
							index--;
							transitionTimer = transitionTime;
						}
					}
					else
					{
						state = State.Complete;
					}
				}
				else
				{
					index++;
				}
			}
		}
		currentState = state;
	}
}
