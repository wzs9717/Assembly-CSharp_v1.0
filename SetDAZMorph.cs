using UnityEngine;

public class SetDAZMorph : MonoBehaviour
{
	[SerializeField]
	protected DAZMorphBank _morphBank;

	public string morph1Name;

	public float morph1Low;

	public float morph1High = 1f;

	public float currentMorph1Value;

	private float _morphPercent;

	protected DAZMorph morph1;

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

	public float morphPercent
	{
		get
		{
			return _morphPercent;
		}
		set
		{
			_morphPercent = value;
			if (morph1 != null)
			{
				currentMorph1Value = Mathf.Lerp(morph1Low, morph1High, _morphPercent);
				morph1.morphValue = currentMorph1Value;
			}
		}
	}

	public float morphRawValue
	{
		get
		{
			if (morph1 != null)
			{
				return morph1.morphValue;
			}
			return 0f;
		}
		set
		{
			if (morph1 != null)
			{
				currentMorph1Value = value;
				morph1.morphValue = value;
			}
		}
	}

	protected void InitMorphs()
	{
		morph1 = null;
		if (_morphBank != null)
		{
			morph1 = _morphBank.GetMorph(morph1Name);
		}
	}

	private void OnEnable()
	{
		InitMorphs();
	}

	private void Start()
	{
		InitMorphs();
	}

	private void OnDisable()
	{
		if (morph1 != null)
		{
			morph1.morphValue = 0f;
		}
	}
}
