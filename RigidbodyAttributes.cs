using UnityEngine;

[ExecuteInEditMode]
public class RigidbodyAttributes : MonoBehaviour
{
	private Rigidbody rb;

	[SerializeField]
	private bool _useOverrideTensor;

	private Vector3 _originalTensor;

	private Quaternion _originalTensorRotation;

	[SerializeField]
	private Vector3 _inertiaTensor = Vector3.one;

	[SerializeField]
	private bool _useInterpolation;

	public bool useOverrideTensor
	{
		get
		{
			return _useOverrideTensor;
		}
		set
		{
			if (_useOverrideTensor == value)
			{
				return;
			}
			_useOverrideTensor = value;
			if (rb != null)
			{
				if (_useOverrideTensor)
				{
					_originalTensor = rb.inertiaTensor;
					_originalTensorRotation = rb.inertiaTensorRotation;
					rb.inertiaTensor = _inertiaTensor;
				}
				else
				{
					rb.ResetInertiaTensor();
				}
			}
		}
	}

	public Vector3 originalTensor => _originalTensor;

	public Quaternion originalTensorRotation => _originalTensorRotation;

	public Vector3 currentTensor
	{
		get
		{
			if (rb != null)
			{
				return rb.inertiaTensor;
			}
			return Vector3.zero;
		}
	}

	public Vector3 inertiaTensor
	{
		get
		{
			return _inertiaTensor;
		}
		set
		{
			if (_inertiaTensor != value)
			{
				_inertiaTensor = value;
				SetTensor();
			}
		}
	}

	public float maxDepenetrationVelocity
	{
		get
		{
			if (rb != null)
			{
				return rb.maxDepenetrationVelocity;
			}
			return -1f;
		}
	}

	public float maxAngularVelocity
	{
		get
		{
			if (rb != null)
			{
				return rb.maxAngularVelocity;
			}
			return -1f;
		}
	}

	public bool useInterpolation
	{
		get
		{
			return _useInterpolation;
		}
		set
		{
			if (_useInterpolation != value)
			{
				_useInterpolation = value;
				SyncInterpolation();
			}
		}
	}

	private void SetTensor()
	{
		if (rb != null && _useOverrideTensor)
		{
			rb.inertiaTensor = _inertiaTensor;
		}
	}

	private void SyncInterpolation()
	{
		if (rb != null)
		{
			if (Application.isPlaying && base.isActiveAndEnabled && _useInterpolation)
			{
				rb.interpolation = RigidbodyInterpolation.Interpolate;
			}
			else
			{
				rb.interpolation = RigidbodyInterpolation.None;
			}
		}
	}

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		SetTensor();
	}

	private void OnEnable()
	{
		rb = GetComponent<Rigidbody>();
		SyncInterpolation();
	}

	private void OnDisable()
	{
		SyncInterpolation();
	}
}
