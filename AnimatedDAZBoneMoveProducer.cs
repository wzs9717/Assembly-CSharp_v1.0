using UnityEngine;

public class AnimatedDAZBoneMoveProducer : MoveProducer
{
	public bool useOrientationOffset = true;

	protected DAZBone dazBone;

	protected Transform orientationTransform;

	protected override void Start()
	{
		base.Start();
		if (_receiver != null && _receiver.followWhenOff != null)
		{
			dazBone = _receiver.followWhenOff.GetComponent<DAZBone>();
			if (dazBone != null)
			{
				GameObject gameObject = new GameObject(base.name + "_orientation");
				orientationTransform = gameObject.transform;
				orientationTransform.SetParent(base.transform);
				orientationTransform.localPosition = Vector3.zero;
				dazBone.Init();
				orientationTransform.localRotation = dazBone.startingLocalRotation;
			}
		}
	}

	protected override void SetCurrentPositionAndRotation()
	{
		if (useOrientationOffset && orientationTransform != null)
		{
			_currentPosition = orientationTransform.position;
			_currentRotation = orientationTransform.rotation;
		}
		else
		{
			_currentPosition = base.transform.position;
			_currentRotation = base.transform.rotation;
		}
	}
}
