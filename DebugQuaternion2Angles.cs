using UnityEngine;

public class DebugQuaternion2Angles : MonoBehaviour
{
	public enum DisplayType
	{
		Radians,
		Degrees
	}

	public Quaternion2Angles.RotationOrder rotationOrder;

	public DisplayType displayType = DisplayType.Degrees;

	public float xrotset;

	public float yrotset;

	public float zrotset;

	public bool set;

	public Transform setOther;

	public float xrot;

	public float yrot;

	public float zrot;

	public float qx;

	public float qy;

	public float qz;

	public float qw;

	public float ex;

	public float ey;

	public float ez;

	private void rotateTransform(Transform t, float xr, float yr, float zr)
	{
		t.localRotation = Quaternion.identity;
		switch (rotationOrder)
		{
		case Quaternion2Angles.RotationOrder.XYZ:
			t.Rotate(xr, 0f, 0f);
			t.Rotate(0f, yr, 0f);
			t.Rotate(0f, 0f, zr);
			break;
		case Quaternion2Angles.RotationOrder.XZY:
			t.Rotate(xr, 0f, 0f);
			t.Rotate(0f, 0f, zr);
			t.Rotate(0f, yr, 0f);
			break;
		case Quaternion2Angles.RotationOrder.YXZ:
			t.Rotate(0f, yr, 0f);
			t.Rotate(xr, 0f, 0f);
			t.Rotate(0f, 0f, zr);
			break;
		case Quaternion2Angles.RotationOrder.YZX:
			t.Rotate(0f, yr, 0f);
			t.Rotate(0f, 0f, zr);
			t.Rotate(xr, 0f, 0f);
			break;
		case Quaternion2Angles.RotationOrder.ZXY:
			t.Rotate(0f, 0f, zr);
			t.Rotate(xr, 0f, 0f);
			t.Rotate(0f, yr, 0f);
			break;
		case Quaternion2Angles.RotationOrder.ZYX:
			t.Rotate(0f, 0f, zr);
			t.Rotate(0f, yr, 0f);
			t.Rotate(xr, 0f, 0f);
			break;
		}
	}

	private void LateUpdate()
	{
		if (set)
		{
			set = false;
			rotateTransform(base.transform, xrotset, yrotset, zrotset);
		}
		Quaternion localRotation = base.transform.localRotation;
		qx = localRotation.x;
		qy = localRotation.y;
		qz = localRotation.z;
		qw = localRotation.w;
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		if (localEulerAngles.x > 180f)
		{
			localEulerAngles.x -= 360f;
		}
		if (localEulerAngles.y > 180f)
		{
			localEulerAngles.y -= 360f;
		}
		if (localEulerAngles.z > 180f)
		{
			localEulerAngles.z -= 360f;
		}
		ex = localEulerAngles.x;
		ey = localEulerAngles.y;
		ez = localEulerAngles.z;
		Vector3 angles = Quaternion2Angles.GetAngles(localRotation, rotationOrder);
		if (displayType == DisplayType.Degrees)
		{
			angles *= 57.29578f;
		}
		xrot = angles.x;
		yrot = angles.y;
		zrot = angles.z;
		if ((bool)setOther)
		{
			rotateTransform(setOther, xrot, yrot, zrot);
		}
	}
}
