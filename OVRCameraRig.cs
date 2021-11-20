using System;
using UnityEngine;
using UnityEngine.VR;

[ExecuteInEditMode]
public class OVRCameraRig : MonoBehaviour
{
	public bool usePerEyeCameras;

	private bool _skipUpdate;

	private readonly string trackingSpaceName = "TrackingSpace";

	private readonly string trackerAnchorName = "TrackerAnchor";

	private readonly string eyeAnchorName = "EyeAnchor";

	private readonly string handAnchorName = "HandAnchor";

	private readonly string legacyEyeAnchorName = "Camera";

	private Camera _centerEyeCamera;

	private Camera _leftEyeCamera;

	private Camera _rightEyeCamera;

	public Camera leftEyeCamera => (!usePerEyeCameras) ? _centerEyeCamera : _leftEyeCamera;

	public Camera rightEyeCamera => (!usePerEyeCameras) ? _centerEyeCamera : _rightEyeCamera;

	public Transform trackingSpace { get; private set; }

	public Transform leftEyeAnchor { get; private set; }

	public Transform centerEyeAnchor { get; private set; }

	public Transform rightEyeAnchor { get; private set; }

	public Transform leftHandAnchor { get; private set; }

	public Transform rightHandAnchor { get; private set; }

	public Transform trackerAnchor { get; private set; }

	public event Action<OVRCameraRig> UpdatedAnchors;

	private void Awake()
	{
		_skipUpdate = true;
		EnsureGameObjectIntegrity();
	}

	private void Start()
	{
		UpdateAnchors();
	}

	private void FixedUpdate()
	{
		UpdateAnchors();
	}

	private void Update()
	{
		_skipUpdate = false;
	}

	private void UpdateAnchors()
	{
		EnsureGameObjectIntegrity();
		if (!Application.isPlaying)
		{
			return;
		}
		if (_skipUpdate)
		{
			centerEyeAnchor.FromOVRPose(OVRPose.identity, isLocal: true);
			leftEyeAnchor.FromOVRPose(OVRPose.identity, isLocal: true);
			rightEyeAnchor.FromOVRPose(OVRPose.identity, isLocal: true);
			return;
		}
		bool monoscopic = OVRManager.instance.monoscopic;
		OVRPose pose = OVRManager.tracker.GetPose();
		trackerAnchor.localRotation = pose.orientation;
		centerEyeAnchor.localRotation = InputTracking.GetLocalRotation((VRNode)2);
		leftEyeAnchor.localRotation = ((!monoscopic) ? InputTracking.GetLocalRotation((VRNode)0) : centerEyeAnchor.localRotation);
		rightEyeAnchor.localRotation = ((!monoscopic) ? InputTracking.GetLocalRotation((VRNode)1) : centerEyeAnchor.localRotation);
		leftHandAnchor.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
		rightHandAnchor.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
		trackerAnchor.localPosition = pose.position;
		centerEyeAnchor.localPosition = InputTracking.GetLocalPosition((VRNode)2);
		leftEyeAnchor.localPosition = ((!monoscopic) ? InputTracking.GetLocalPosition((VRNode)0) : centerEyeAnchor.localPosition);
		rightEyeAnchor.localPosition = ((!monoscopic) ? InputTracking.GetLocalPosition((VRNode)1) : centerEyeAnchor.localPosition);
		leftHandAnchor.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
		rightHandAnchor.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
		if (this.UpdatedAnchors != null)
		{
			this.UpdatedAnchors(this);
		}
	}

	public void EnsureGameObjectIntegrity()
	{
		if (trackingSpace == null)
		{
			trackingSpace = ConfigureRootAnchor(trackingSpaceName);
		}
		if (leftEyeAnchor == null)
		{
			leftEyeAnchor = ConfigureEyeAnchor(trackingSpace, (VRNode)0);
		}
		if (centerEyeAnchor == null)
		{
			centerEyeAnchor = ConfigureEyeAnchor(trackingSpace, (VRNode)2);
		}
		if (rightEyeAnchor == null)
		{
			rightEyeAnchor = ConfigureEyeAnchor(trackingSpace, (VRNode)1);
		}
		if (leftHandAnchor == null)
		{
			leftHandAnchor = ConfigureHandAnchor(trackingSpace, OVRPlugin.Node.HandLeft);
		}
		if (rightHandAnchor == null)
		{
			rightHandAnchor = ConfigureHandAnchor(trackingSpace, OVRPlugin.Node.HandRight);
		}
		if (trackerAnchor == null)
		{
			trackerAnchor = ConfigureTrackerAnchor(trackingSpace);
		}
		if (_centerEyeCamera == null || _leftEyeCamera == null || _rightEyeCamera == null)
		{
			_centerEyeCamera = centerEyeAnchor.GetComponent<Camera>();
			_leftEyeCamera = leftEyeAnchor.GetComponent<Camera>();
			_rightEyeCamera = rightEyeAnchor.GetComponent<Camera>();
			if (_centerEyeCamera == null)
			{
				_centerEyeCamera = centerEyeAnchor.gameObject.AddComponent<Camera>();
				_centerEyeCamera.tag = "MainCamera";
			}
			if (_leftEyeCamera == null)
			{
				_leftEyeCamera = leftEyeAnchor.gameObject.AddComponent<Camera>();
				_leftEyeCamera.tag = "MainCamera";
			}
			if (_rightEyeCamera == null)
			{
				_rightEyeCamera = rightEyeAnchor.gameObject.AddComponent<Camera>();
				_rightEyeCamera.tag = "MainCamera";
			}
			_centerEyeCamera.stereoTargetEye = StereoTargetEyeMask.Both;
			_leftEyeCamera.stereoTargetEye = StereoTargetEyeMask.Left;
			_rightEyeCamera.stereoTargetEye = StereoTargetEyeMask.Right;
		}
		if (_centerEyeCamera.enabled == usePerEyeCameras || _leftEyeCamera.enabled == !usePerEyeCameras || _rightEyeCamera.enabled == !usePerEyeCameras)
		{
			_skipUpdate = true;
		}
		_centerEyeCamera.enabled = !usePerEyeCameras;
		_leftEyeCamera.enabled = usePerEyeCameras;
		_rightEyeCamera.enabled = usePerEyeCameras;
	}

	private Transform ConfigureRootAnchor(string name)
	{
		Transform transform = base.transform.Find(name);
		if (transform == null)
		{
			transform = new GameObject(name).transform;
		}
		transform.parent = base.transform;
		transform.localScale = Vector3.one;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		return transform;
	}

	private Transform ConfigureEyeAnchor(Transform root, VRNode eye)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		string text = (((int)eye == 2) ? "Center" : (((int)eye != 0) ? "Right" : "Left"));
		string text2 = text + eyeAnchorName;
		Transform transform = base.transform.Find(root.name + "/" + text2);
		if (transform == null)
		{
			transform = base.transform.Find(text2);
		}
		if (transform == null)
		{
			string text3 = legacyEyeAnchorName + ((object)(VRNode)(ref eye)).ToString();
			transform = base.transform.Find(text3);
		}
		if (transform == null)
		{
			transform = new GameObject(text2).transform;
		}
		transform.name = text2;
		transform.parent = root;
		transform.localScale = Vector3.one;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		return transform;
	}

	private Transform ConfigureHandAnchor(Transform root, OVRPlugin.Node hand)
	{
		string text = ((hand != OVRPlugin.Node.HandLeft) ? "Right" : "Left");
		string text2 = text + handAnchorName;
		Transform transform = base.transform.Find(root.name + "/" + text2);
		if (transform == null)
		{
			transform = base.transform.Find(text2);
		}
		if (transform == null)
		{
			transform = new GameObject(text2).transform;
		}
		transform.name = text2;
		transform.parent = root;
		transform.localScale = Vector3.one;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		return transform;
	}

	private Transform ConfigureTrackerAnchor(Transform root)
	{
		string text = trackerAnchorName;
		Transform transform = base.transform.Find(root.name + "/" + text);
		if (transform == null)
		{
			transform = new GameObject(text).transform;
		}
		transform.parent = root;
		transform.localScale = Vector3.one;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		return transform;
	}
}
