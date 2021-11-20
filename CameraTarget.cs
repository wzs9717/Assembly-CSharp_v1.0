using UnityEngine;

public class CameraTarget : MonoBehaviour
{
	public enum CameraLocation
	{
		None,
		Left,
		Right,
		Center
	}

	public static CameraTarget centerTarget;

	public static CameraTarget leftTarget;

	public static CameraTarget rightTarget;

	public CameraLocation cameraLocation;

	public Camera targetCamera;

	public Matrix4x4 worldToCameraMatrix;

	public Matrix4x4 projectionMatrix;

	private void OnPreRender()
	{
		if (targetCamera != null)
		{
			worldToCameraMatrix = targetCamera.worldToCameraMatrix;
			projectionMatrix = targetCamera.projectionMatrix;
		}
	}

	public void FindCamera()
	{
		targetCamera = GetComponent<Camera>();
		if (targetCamera == null)
		{
			targetCamera = base.transform.parent.GetComponent<Camera>();
		}
	}

	private void Update()
	{
		switch (cameraLocation)
		{
		case CameraLocation.Center:
			centerTarget = this;
			break;
		case CameraLocation.Left:
			leftTarget = this;
			break;
		case CameraLocation.Right:
			rightTarget = this;
			break;
		}
	}
}
