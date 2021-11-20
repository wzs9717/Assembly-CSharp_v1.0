using UnityEngine;

public class DAZMeshEyelidControl : MonoBehaviour
{
	[SerializeField]
	private DAZMorphBank _morphBank;

	public Transform leftEye;

	public Transform rightEye;

	public string LeftTopEyelidDownMorphName;

	public string RightTopEyelidDownMorphName;

	public string LeftBottomEyelidUpMorphName;

	public string RightBottomEyelidUpMorphName;

	public string LeftTopEyelidUpMorphName;

	public string RightTopEyelidUpMorphName;

	public string LeftBottomEyelidDownMorphName;

	public string RightBottomEyelidDownMorphName;

	public float blinkSpaceMin = 1f;

	public float blinkSpaceMax = 7f;

	public float blinkTimeMin = 0.1f;

	public float blinkTimeMax = 0.4f;

	public float blinkDownUpRatio = 0.4f;

	public float blinkBottomEyelidFactor = 0.5f;

	public float lookUpTopEyelidFactor = 3f;

	public float lookDownTopEyelidFactor = 1.5f;

	public float lookDownBottomEyelidFactor = 4f;

	private DAZMorph LeftTopEyelidDownMorph;

	private DAZMorph RightTopEyelidDownMorph;

	private DAZMorph LeftBottomEyelidUpMorph;

	private DAZMorph RightBottomEyelidUpMorph;

	private DAZMorph LeftTopEyelidUpMorph;

	private DAZMorph RightTopEyelidUpMorph;

	private DAZMorph LeftBottomEyelidDownMorph;

	private DAZMorph RightBottomEyelidDownMorph;

	private bool closed;

	private bool blinking;

	private float blinkStartTimer;

	public float blinkTime;

	public float currentWeight;

	public float leftEyeWeight;

	public float rightEyeWeight;

	public float targetWeight;

	public DAZMorphBank morphBank
	{
		get
		{
			return _morphBank;
		}
		set
		{
			if (_morphBank != value)
			{
				_morphBank = value;
				InitMorphs();
			}
		}
	}

	public void Close()
	{
		closed = true;
		BlinkClose();
	}

	public void Open()
	{
		closed = false;
		BlinkOpen();
	}

	public void Blink()
	{
		blinking = true;
		BlinkClose();
		blinkTime = Random.Range(blinkTimeMin, blinkTimeMax);
	}

	private void BlinkClose()
	{
		targetWeight = 1f;
	}

	private void BlinkOpen()
	{
		if (!closed)
		{
			targetWeight = 0f;
		}
	}

	private void InitMorphs()
	{
		if ((bool)_morphBank)
		{
			DAZMorph morph = _morphBank.GetMorph(LeftTopEyelidDownMorphName);
			if (morph != null)
			{
				LeftTopEyelidDownMorph = morph;
			}
			morph = _morphBank.GetMorph(RightTopEyelidDownMorphName);
			if (morph != null)
			{
				RightTopEyelidDownMorph = morph;
			}
			morph = _morphBank.GetMorph(LeftTopEyelidUpMorphName);
			if (morph != null)
			{
				LeftTopEyelidUpMorph = morph;
			}
			morph = _morphBank.GetMorph(RightTopEyelidUpMorphName);
			if (morph != null)
			{
				RightTopEyelidUpMorph = morph;
			}
			morph = _morphBank.GetMorph(LeftBottomEyelidDownMorphName);
			if (morph != null)
			{
				LeftBottomEyelidDownMorph = morph;
			}
			morph = _morphBank.GetMorph(RightBottomEyelidDownMorphName);
			if (morph != null)
			{
				RightBottomEyelidDownMorph = morph;
			}
			morph = _morphBank.GetMorph(LeftBottomEyelidUpMorphName);
			if (morph != null)
			{
				LeftBottomEyelidUpMorph = morph;
			}
			morph = _morphBank.GetMorph(RightBottomEyelidUpMorphName);
			if (morph != null)
			{
				RightBottomEyelidUpMorph = morph;
			}
		}
	}

	private void Start()
	{
		InitMorphs();
	}

	private void UpdateBlink()
	{
		if (blinking)
		{
			if (currentWeight > targetWeight)
			{
				currentWeight -= Time.deltaTime / (blinkTime * (1f - blinkDownUpRatio));
			}
			else
			{
				currentWeight += Time.deltaTime / (blinkTime * blinkDownUpRatio);
			}
			if (currentWeight < 0f)
			{
				currentWeight = 0f;
				blinking = false;
			}
			else if (currentWeight > 1f)
			{
				currentWeight = 1f;
				BlinkOpen();
			}
		}
		blinkStartTimer -= Time.deltaTime;
		if (blinkStartTimer < 0f)
		{
			Blink();
			blinkStartTimer = Random.Range(blinkSpaceMin, blinkSpaceMax);
		}
	}

	private void UpdateWeights()
	{
		leftEyeWeight = currentWeight;
		if (leftEye != null)
		{
			float x = Quaternion2Angles.GetAngles(leftEye.localRotation, Quaternion2Angles.RotationOrder.ZYX).x;
			if (x > 0f)
			{
				float num = x * lookDownTopEyelidFactor;
				if (leftEyeWeight < num)
				{
					leftEyeWeight = num;
				}
				if (LeftBottomEyelidDownMorph != null)
				{
					LeftBottomEyelidDownMorph.morphValue = x * lookDownBottomEyelidFactor;
				}
				if (LeftTopEyelidUpMorph != null)
				{
					LeftTopEyelidUpMorph.morphValue = 0f;
				}
			}
			else
			{
				if (LeftBottomEyelidDownMorph != null)
				{
					LeftBottomEyelidDownMorph.morphValue = 0f;
				}
				if (LeftTopEyelidUpMorph != null)
				{
					LeftTopEyelidUpMorph.morphValue = (0f - x) * lookUpTopEyelidFactor;
				}
			}
		}
		if (LeftTopEyelidDownMorph != null)
		{
			LeftTopEyelidDownMorph.morphValue = leftEyeWeight;
		}
		rightEyeWeight = currentWeight;
		if (rightEye != null)
		{
			float x2 = Quaternion2Angles.GetAngles(rightEye.localRotation, Quaternion2Angles.RotationOrder.ZYX).x;
			if (x2 > 0f)
			{
				float num2 = x2 * lookDownTopEyelidFactor;
				if (rightEyeWeight < num2)
				{
					rightEyeWeight = num2;
				}
				if (RightBottomEyelidDownMorph != null)
				{
					RightBottomEyelidDownMorph.morphValue = x2 * lookDownBottomEyelidFactor;
				}
				if (RightTopEyelidUpMorph != null)
				{
					RightTopEyelidUpMorph.morphValue = 0f;
				}
			}
			else
			{
				if (RightBottomEyelidDownMorph != null)
				{
					RightBottomEyelidDownMorph.morphValue = 0f;
				}
				if (RightTopEyelidUpMorph != null)
				{
					RightTopEyelidUpMorph.morphValue = (0f - x2) * lookUpTopEyelidFactor;
				}
			}
		}
		if (RightTopEyelidDownMorph != null)
		{
			RightTopEyelidDownMorph.morphValue = rightEyeWeight;
		}
		if (LeftBottomEyelidUpMorph != null)
		{
			LeftBottomEyelidUpMorph.morphValue = currentWeight * blinkBottomEyelidFactor;
		}
		if (RightBottomEyelidUpMorph != null)
		{
			RightBottomEyelidUpMorph.morphValue = currentWeight * blinkBottomEyelidFactor;
		}
	}

	private void Update()
	{
		UpdateBlink();
		UpdateWeights();
	}
}
