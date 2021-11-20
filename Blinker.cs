using UnityEngine;

public class Blinker : MonoBehaviour
{
	public SkinnedMeshRenderer skin;

	public string LeftTopEyelidMorph;

	public string RightTopEyelidMorph;

	public string LeftBottomEyelidMorph;

	public string RightBottomEyelidMorph;

	public float blinkSpaceMin = 1f;

	public float blinkSpaceMax = 7f;

	public float blinkTimeMin = 0.1f;

	public float blinkTimeMax = 0.4f;

	public float blinkDownUpRatio = 0.4f;

	private int LeftTopEyelidIndex = -1;

	private int RightTopEyelidIndex = -1;

	private int LeftBottomEyelidIndex = -1;

	private int RightBottomEyelidIndex = -1;

	private bool closed;

	private bool blinking;

	private float blinkStartTimer;

	public float blinkTime;

	public float currentWeight;

	public float targetWeight;

	public void close()
	{
		closed = true;
		blinkClose();
	}

	public void open()
	{
		closed = false;
		blinkOpen();
	}

	private void setWeights()
	{
		if (LeftTopEyelidIndex != -1)
		{
			skin.SetBlendShapeWeight(LeftTopEyelidIndex, currentWeight);
		}
		if (RightTopEyelidIndex != -1)
		{
			skin.SetBlendShapeWeight(RightTopEyelidIndex, currentWeight);
		}
		if (LeftBottomEyelidIndex != -1)
		{
			skin.SetBlendShapeWeight(LeftBottomEyelidIndex, currentWeight);
		}
		if (RightBottomEyelidIndex != -1)
		{
			skin.SetBlendShapeWeight(RightBottomEyelidIndex, currentWeight);
		}
	}

	private void blinkClose()
	{
		targetWeight = 100f;
	}

	private void blinkOpen()
	{
		if (!closed)
		{
			targetWeight = 0f;
		}
	}

	public void blink()
	{
		blinking = true;
		blinkClose();
		blinkTime = Random.Range(blinkTimeMin, blinkTimeMax);
	}

	private void Start()
	{
		if ((bool)skin)
		{
			LeftTopEyelidIndex = skin.sharedMesh.GetBlendShapeIndex(LeftTopEyelidMorph);
			RightTopEyelidIndex = skin.sharedMesh.GetBlendShapeIndex(RightTopEyelidMorph);
			LeftBottomEyelidIndex = skin.sharedMesh.GetBlendShapeIndex(LeftBottomEyelidMorph);
			RightBottomEyelidIndex = skin.sharedMesh.GetBlendShapeIndex(RightBottomEyelidMorph);
		}
	}

	private void Update()
	{
		if (blinking)
		{
			if (currentWeight < 0f)
			{
				currentWeight = 0f;
				blinking = false;
			}
			else if (currentWeight > 100f)
			{
				currentWeight = 100f;
				blinkOpen();
			}
			else if (currentWeight > targetWeight)
			{
				currentWeight -= Time.deltaTime / (blinkTime * (1f - blinkDownUpRatio)) * 100f;
			}
			else
			{
				currentWeight += Time.deltaTime / (blinkTime * blinkDownUpRatio) * 100f;
			}
		}
		blinkStartTimer -= Time.deltaTime;
		if (blinkStartTimer < 0f)
		{
			blink();
			blinkStartTimer = Random.Range(blinkSpaceMin, blinkSpaceMax);
		}
		setWeights();
	}
}
