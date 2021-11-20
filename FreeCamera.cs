using mset;
using UnityEngine;

public class FreeCamera : MonoBehaviour
{
	public float thetaSpeed = 250f;

	public float phiSpeed = 120f;

	public float moveSpeed = 10f;

	public float zoomSpeed = 30f;

	public float phiBoundMin = -89f;

	public float phiBoundMax = 89f;

	public bool useMoveBounds = true;

	public float moveBounds = 100f;

	public float rotateSmoothing = 0.5f;

	public float moveSmoothing = 0.7f;

	public float distance = 2f;

	private Vector2 euler;

	private Quaternion targetRot;

	private Vector3 targetLookAt;

	private float targetDist;

	private Vector3 distanceVec = new Vector3(0f, 0f, 0f);

	private Transform target;

	private Rect inputBounds;

	public Rect paramInputBounds = new Rect(0f, 0f, 1f, 1f);

	public bool usePivotPoint = true;

	public Vector3 pivotPoint = new Vector3(0f, 2f, 0f);

	public Transform pivotTransform;

	public void Start()
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		euler.x = eulerAngles.y;
		euler.y = eulerAngles.x;
		euler.y = Mathf.Repeat(euler.y + 180f, 360f) - 180f;
		GameObject gameObject = new GameObject("_FreeCameraTarget");
		gameObject.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
		target = gameObject.transform;
		if (usePivotPoint)
		{
			target.position = pivotPoint;
			targetDist = (base.transform.position - target.position).magnitude;
		}
		else if (pivotTransform != null)
		{
			usePivotPoint = true;
			Vector3 point = base.transform.worldToLocalMatrix.MultiplyPoint3x4(pivotTransform.position);
			point.x = 0f;
			point.y = 0f;
			targetDist = point.z;
			target.position = base.transform.localToWorldMatrix.MultiplyPoint3x4(point);
		}
		else
		{
			target.position = base.transform.position + base.transform.forward * distance;
			targetDist = distance;
		}
		targetRot = base.transform.rotation;
		targetLookAt = target.position;
	}

	public void Update()
	{
		inputBounds.x = (float)GetComponent<Camera>().pixelWidth * paramInputBounds.x;
		inputBounds.y = (float)GetComponent<Camera>().pixelHeight * paramInputBounds.y;
		inputBounds.width = (float)GetComponent<Camera>().pixelWidth * paramInputBounds.width;
		inputBounds.height = (float)GetComponent<Camera>().pixelHeight * paramInputBounds.height;
		if (!target || !inputBounds.Contains(Input.mousePosition))
		{
			return;
		}
		float axis = Input.GetAxis("Mouse X");
		float axis2 = Input.GetAxis("Mouse Y");
		bool flag = Input.GetMouseButton(0) || Input.touchCount == 1;
		bool flag2 = Input.GetMouseButton(1) || Input.touchCount == 2;
		bool flag3 = Input.GetMouseButton(2) || Input.touchCount == 3;
		bool flag4 = Input.touchCount >= 4;
		bool flag5 = flag;
		bool flag6 = flag4 || (flag && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)));
		bool flag7 = flag3 || (flag && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)));
		bool flag8 = flag2;
		if (flag6)
		{
			axis = axis * thetaSpeed * 0.02f;
			SkyManager skyManager = SkyManager.Get();
			if ((bool)skyManager && (bool)skyManager.GlobalSky)
			{
				skyManager.GlobalSky.transform.Rotate(new Vector3(0f, axis, 0f));
			}
		}
		else if (flag7)
		{
			axis = axis * moveSpeed * 0.005f * targetDist;
			axis2 = axis2 * moveSpeed * 0.005f * targetDist;
			targetLookAt -= base.transform.up * axis2 + base.transform.right * axis;
			if (useMoveBounds)
			{
				targetLookAt.x = Mathf.Clamp(targetLookAt.x, 0f - moveBounds, moveBounds);
				targetLookAt.y = Mathf.Clamp(targetLookAt.y, 0f - moveBounds, moveBounds);
				targetLookAt.z = Mathf.Clamp(targetLookAt.z, 0f - moveBounds, moveBounds);
			}
		}
		else if (flag8)
		{
			axis2 = axis2 * zoomSpeed * 0.005f * targetDist;
			targetDist += axis2;
			targetDist = Mathf.Max(0.1f, targetDist);
		}
		else if (flag5)
		{
			axis = axis * thetaSpeed * 0.02f;
			axis2 = axis2 * phiSpeed * 0.02f;
			euler.x += axis;
			euler.y -= axis2;
			euler.y = ClampAngle(euler.y, phiBoundMin, phiBoundMax);
			targetRot = Quaternion.Euler(euler.y, euler.x, 0f);
		}
		targetDist -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * 0.5f;
		targetDist = Mathf.Max(0.1f, targetDist);
	}

	public void FixedUpdate()
	{
		distance = moveSmoothing * targetDist + (1f - moveSmoothing) * distance;
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, targetRot, rotateSmoothing);
		target.position = Vector3.Lerp(target.position, targetLookAt, moveSmoothing);
		distanceVec.z = distance;
		base.transform.position = target.position - base.transform.rotation * distanceVec;
	}

	private static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}
}
