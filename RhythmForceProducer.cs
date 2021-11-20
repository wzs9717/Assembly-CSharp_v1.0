using UnityEngine;

public class RhythmForceProducer : ForceProducer
{
	public enum RangeSelect
	{
		low,
		mid,
		high
	}

	public RhythmController controller;

	public RangeSelect rangeSelect;

	public float alternateBeatRatio = 1f;

	public float threshold = 1f;

	public float BurstLength = 1f;

	public float MinSpacing = 0.4f;

	public float RandomFactor = 0.1f;

	public Material rhythmLineMaterial;

	public Material rawRhythmLineMaterial;

	private float minThreshold = 1f;

	private float flip = 1f;

	private bool timerOn;

	private float timer;

	private float forceTimer;

	private float maxOnset = 100f;

	private float onsetMult = 2f;

	private float rawOnset;

	private float onset;

	private LineDrawer rhythmLineDrawer;

	private LineDrawer rawRhythmLineDrawer;

	private float rhythmLineLength;

	private float rawRhythmLineLength;

	protected override void Start()
	{
		base.Start();
		if ((bool)rhythmLineMaterial)
		{
			rhythmLineDrawer = new LineDrawer(rhythmLineMaterial);
		}
		if ((bool)rawRhythmLineMaterial)
		{
			rawRhythmLineDrawer = new LineDrawer(rawRhythmLineMaterial);
		}
	}

	protected override void Update()
	{
		base.Update();
		forceTimer -= Time.deltaTime;
		if (forceTimer > 0f)
		{
			SetTargetForce(flip * onset);
		}
		else
		{
			SetTargetForce(0f);
		}
		float num = ((rangeSelect == RangeSelect.low) ? (controller.low[controller.rhythmTool.CurrentFrame].onset * onsetMult) : ((rangeSelect != RangeSelect.mid) ? (controller.high[controller.rhythmTool.CurrentFrame].onset * onsetMult) : (controller.mid[controller.rhythmTool.CurrentFrame].onset * onsetMult)));
		if (num > maxOnset)
		{
			num = maxOnset;
		}
		if (num > minThreshold)
		{
			rawOnset = num;
			rawRhythmLineLength = rawOnset * linesScale * ForceFactor;
		}
		if (timerOn)
		{
			timer -= Time.deltaTime;
			if (timer < 0f)
			{
				timerOn = false;
				if (flip > 0f)
				{
					flip = 0f - alternateBeatRatio;
				}
				else
				{
					flip = 1f;
				}
			}
		}
		else if (controller != null && controller.rhythmTool != null && controller.low != null)
		{
			onset = num;
			if (onset > threshold)
			{
				timerOn = true;
				timer = MinSpacing;
				forceTimer = BurstLength;
				if (Random.value < RandomFactor)
				{
					timer += MinSpacing;
					forceTimer += BurstLength;
				}
				rhythmLineLength = rawRhythmLineLength * flip;
			}
		}
		rawRhythmLineLength = Mathf.Lerp(rawRhythmLineLength, 0f, Time.deltaTime * 5f);
		if (on && receiver != null && drawLines)
		{
			Vector3 vector = AxisToVector(ForceAxis);
			Vector3 vector2 = AxisToUpVector(ForceAxis);
			Vector3 vector3 = base.transform.position + vector2 * (lineOffset + lineSpacing * 10f);
			if (rhythmLineDrawer != null)
			{
				rhythmLineDrawer.SetLinePoints(vector3, vector3 + vector * rhythmLineLength);
				rhythmLineDrawer.Draw();
			}
			vector3 += vector2 * lineSpacing;
			if (rawRhythmLineDrawer != null)
			{
				rawRhythmLineDrawer.SetLinePoints(vector3, vector3 + vector * (0f - rawRhythmLineLength));
				rawRhythmLineDrawer.Draw();
			}
		}
	}
}
