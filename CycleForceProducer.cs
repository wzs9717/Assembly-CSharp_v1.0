using UnityEngine;

public class CycleForceProducer : ForceProducer
{
	public float period = 1f;

	public float periodRatio = 0.5f;

	public float ForceDuration = 1f;

	public bool applyForceOnReturn = true;

	private float timer;

	private float forceTimer;

	private float flip;

	private float mult = 100f;

	protected override void Start()
	{
		base.Start();
		MaxForce = maxMaxForce;
		MaxTorque = maxMaxTorque;
		flip = mult;
	}

	public void SetCyclePeriodFromUISlider()
	{
	}

	public void SetCyclePeriodRatioFromUISlider()
	{
	}

	public void SetForceDurationFromUISlider()
	{
	}

	public void SetApplyForceOnReturnFromUIToggle()
	{
	}

	protected override void Update()
	{
		base.Update();
		timer -= Time.deltaTime;
		forceTimer -= Time.deltaTime;
		if (timer < 0f)
		{
			if (flip > 0f)
			{
				if (applyForceOnReturn)
				{
					flip = 0f - mult;
				}
				else
				{
					flip = 0f;
				}
				timer = period * periodRatio;
				forceTimer = ForceDuration * periodRatio;
			}
			else
			{
				flip = mult;
				timer = period * (1f - periodRatio);
				forceTimer = ForceDuration * (1f - periodRatio);
			}
			SetTargetForce(flip);
		}
		else if (forceTimer < 0f)
		{
			SetTargetForce(0f);
		}
	}
}
