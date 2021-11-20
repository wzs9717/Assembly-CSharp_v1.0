using UnityEngine;

public class MoveAndRotateAsHUDAnchor : MonoBehaviour
{
	public Vector3 Offset;

	public HUDAnchor.AnchorNum anchorNum;

	public bool useSuperControllerWorldScale = true;

	[SerializeField]
	private bool _MoveAsEnabled = true;

	[SerializeField]
	private bool _RotateAsEnabled = true;

	private GameObject tracker;

	[SerializeField]
	private bool _lockPlayerPosition;

	private Vector3 _lastPosition;

	private Quaternion _lastRotation;

	[SerializeField]
	private bool _lockWorldPosition;

	[SerializeField]
	private bool _lookAtEyeCenter = true;

	public bool scaleAsAnchor;

	private Vector3 _startLocalPosition;

	private Quaternion _startLocalRotation;

	public bool MoveAsEnabled
	{
		get
		{
			return _MoveAsEnabled;
		}
		set
		{
			_MoveAsEnabled = value;
		}
	}

	public bool MoveAsDisabled
	{
		get
		{
			return !_MoveAsEnabled;
		}
		set
		{
			_MoveAsEnabled = !value;
		}
	}

	public bool RotateAsEnabled
	{
		get
		{
			return _RotateAsEnabled;
		}
		set
		{
			_RotateAsEnabled = value;
		}
	}

	public bool RotateAsDisabled
	{
		get
		{
			return !_RotateAsEnabled;
		}
		set
		{
			_RotateAsEnabled = !value;
		}
	}

	public bool lockPlayerPosition
	{
		get
		{
			return _lockPlayerPosition;
		}
		set
		{
			if (_lockPlayerPosition != value)
			{
				_lockPlayerPosition = value;
				if (_lockPlayerPosition && PlayerTransform.player != null)
				{
					tracker = new GameObject("tracker");
					tracker.transform.position = base.transform.position;
					tracker.transform.rotation = base.transform.rotation;
					tracker.transform.parent = PlayerTransform.player;
				}
				else if (tracker != null)
				{
					Object.DestroyImmediate(tracker);
					tracker = null;
				}
			}
		}
	}

	public bool lockWorldPosition
	{
		get
		{
			return _lockWorldPosition;
		}
		set
		{
			_lockWorldPosition = value;
		}
	}

	public bool lookAtEyeCenter
	{
		get
		{
			return _lookAtEyeCenter;
		}
		set
		{
			_lookAtEyeCenter = value;
		}
	}

	public void ResetLocalPositionAndRotation()
	{
		base.transform.localPosition = _startLocalPosition;
		base.transform.localRotation = _startLocalRotation;
		_lastPosition = base.transform.position;
		_lastRotation = base.transform.rotation;
		if (tracker != null)
		{
			tracker.transform.position = _lastPosition;
			tracker.transform.rotation = _lastRotation;
		}
	}

	private void moveAndRotate()
	{
		Transform anchorTransform = HUDAnchor.GetAnchorTransform(anchorNum);
		if (useSuperControllerWorldScale && SuperController.singleton != null)
		{
			if (scaleAsAnchor)
			{
				Vector3 localScale = SuperController.singleton.transform.localScale;
				localScale.x *= anchorTransform.localScale.x;
				localScale.y *= anchorTransform.localScale.y;
				localScale.z *= anchorTransform.localScale.z;
				base.transform.localScale = localScale;
			}
			else
			{
				base.transform.localScale = SuperController.singleton.transform.localScale;
			}
		}
		else if (scaleAsAnchor)
		{
			base.transform.localScale = anchorTransform.localScale;
		}
		if (_lockPlayerPosition)
		{
			if (tracker != null)
			{
				base.transform.position = tracker.transform.position;
			}
		}
		else if (_lockWorldPosition)
		{
			base.transform.position = _lastPosition;
		}
		else if (_MoveAsEnabled && anchorTransform != null)
		{
			base.transform.position = anchorTransform.position + Offset;
		}
		if (_lookAtEyeCenter)
		{
			if (EyeCenter.eyeCenter != null)
			{
				base.transform.LookAt(EyeCenter.eyeCenter);
			}
		}
		else if (_lockPlayerPosition)
		{
			if (tracker != null)
			{
				base.transform.rotation = tracker.transform.rotation;
			}
		}
		else if (_lockWorldPosition)
		{
			base.transform.rotation = _lastRotation;
		}
		else if (_RotateAsEnabled && anchorTransform != null)
		{
			base.transform.rotation = anchorTransform.rotation;
		}
		_lastPosition = base.transform.position;
		_lastRotation = base.transform.rotation;
	}

	private void Start()
	{
		_startLocalPosition = base.transform.localPosition;
		_startLocalRotation = base.transform.localRotation;
		_lastPosition = base.transform.position;
		_lastRotation = base.transform.rotation;
	}

	private void LateUpdate()
	{
		moveAndRotate();
	}
}
