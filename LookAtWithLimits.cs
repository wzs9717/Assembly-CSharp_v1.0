using UnityEngine;

public class LookAtWithLimits : MonoBehaviour
{
	public bool on = true;

	public CameraTarget.CameraLocation lookAtCameraLocation;

	public Transform target;

	public UpdateTime updateTime;

	public OrientAxis UpAxis = OrientAxis.Y;

	public OrientAxis ForwardAxis = OrientAxis.Z;

	public OrientAxis RightAxis;

	public float MaxRight;

	public float MaxLeft;

	public float MaxUp;

	public float MaxDown;

	public float MinEngageDistance;

	public float smoothFactor = 10f;

	public bool debug;

	private float lastRightLeft;

	private float lastUpDown;

	private Quaternion startRot;

	private void Start()
	{
		startRot = base.transform.localRotation;
	}

	private Vector3 GetAxisVector(OrientAxis oa)
	{
		return oa switch
		{
			OrientAxis.X => base.transform.right, 
			OrientAxis.NegX => -base.transform.right, 
			OrientAxis.Y => base.transform.up, 
			OrientAxis.NegY => -base.transform.up, 
			OrientAxis.Z => base.transform.forward, 
			_ => -base.transform.forward, 
		};
	}

	private void LookAt()
	{
		base.transform.localRotation = startRot;
		if (target == null && lookAtCameraLocation == CameraTarget.CameraLocation.None)
		{
			return;
		}
		if (lookAtCameraLocation != 0)
		{
			switch (lookAtCameraLocation)
			{
			case CameraTarget.CameraLocation.Center:
				target = CameraTarget.centerTarget.transform;
				break;
			case CameraTarget.CameraLocation.Left:
				target = CameraTarget.leftTarget.transform;
				break;
			case CameraTarget.CameraLocation.Right:
				target = CameraTarget.rightTarget.transform;
				break;
			}
		}
		Vector3 vector = target.position - base.transform.position;
		Vector3 axisVector = GetAxisVector(UpAxis);
		Vector3 axisVector2 = GetAxisVector(ForwardAxis);
		Vector3 axisVector3 = GetAxisVector(RightAxis);
		float magnitude = vector.magnitude;
		if (magnitude < MinEngageDistance)
		{
			vector = axisVector2;
		}
		if (debug)
		{
			Debug.DrawRay(base.transform.position, axisVector, Color.green);
			Debug.DrawRay(base.transform.position, axisVector2, Color.blue);
			Debug.DrawRay(base.transform.position, axisVector3, Color.red);
		}
		float num = Vector3.Angle(axisVector2, vector);
		float num2 = 0f;
		Vector3 vector2 = Vector3.zero;
		if (on)
		{
			float num3 = Vector3.Dot(vector, axisVector);
			Vector3 vector3 = axisVector * num3;
			vector2 = vector - vector3;
			if (debug)
			{
				Debug.DrawRay(base.transform.position, vector2, Color.cyan);
			}
			float num4 = Vector3.Angle(axisVector2, vector2);
			float num5 = Vector3.Dot(vector2, axisVector3);
			if (debug)
			{
				MonoBehaviour.print("fullangle " + num + " left/right angle " + num4 + " Dir is " + num5);
			}
			if (num5 < 0f)
			{
				if (num4 > MaxLeft)
				{
					num4 = MaxLeft;
				}
				num2 = Mathf.Lerp(lastRightLeft, 0f - num4, Time.deltaTime * smoothFactor);
			}
			else
			{
				if (num4 > MaxRight)
				{
					num4 = MaxRight;
				}
				num2 = Mathf.Lerp(lastRightLeft, num4, Time.deltaTime * smoothFactor);
			}
		}
		else
		{
			num2 = Mathf.Lerp(lastRightLeft, 0f, Time.deltaTime * smoothFactor);
		}
		if (float.IsNaN(num2))
		{
			Debug.LogError("left right move angle is NaN for " + base.name);
		}
		else
		{
			lastRightLeft = num2;
			if (UpAxis == OrientAxis.X)
			{
				base.transform.Rotate(num2, 0f, 0f);
			}
			else if (UpAxis == OrientAxis.NegX)
			{
				base.transform.Rotate(0f - num2, 0f, 0f);
			}
			else if (UpAxis == OrientAxis.Y)
			{
				base.transform.Rotate(0f, num2, 0f);
			}
			else if (UpAxis == OrientAxis.NegY)
			{
				base.transform.Rotate(0f, 0f - num2, 0f);
			}
			else if (UpAxis == OrientAxis.Z)
			{
				base.transform.Rotate(0f, 0f, num2);
			}
			else if (UpAxis == OrientAxis.NegZ)
			{
				base.transform.Rotate(0f, 0f, 0f - num2);
			}
		}
		if (on)
		{
			Vector3 vector4 = Vector3.Cross(axisVector, vector2);
			if (debug)
			{
				Debug.DrawRay(base.transform.position, vector4, Color.gray);
			}
			float num6 = Vector3.Dot(vector, vector4);
			Vector3 vector5 = vector4 * num6;
			Vector3 vector6 = vector - vector5;
			if (debug)
			{
				Debug.DrawRay(base.transform.position, vector6, Color.magenta);
			}
			float num7 = Vector3.Angle(vector2, vector6);
			float num8 = Vector3.Dot(vector6, axisVector);
			if (debug)
			{
				MonoBehaviour.print("fullangle " + num + " up/down angle " + num7 + " Dir is " + num8);
			}
			if (num8 < 0f)
			{
				if (num7 > MaxDown)
				{
					num7 = MaxDown;
				}
				num2 = Mathf.Lerp(lastUpDown, num7, Time.deltaTime * smoothFactor);
			}
			else
			{
				if (num7 > MaxUp)
				{
					num7 = MaxUp;
				}
				num2 = Mathf.Lerp(lastUpDown, 0f - num7, Time.deltaTime * smoothFactor);
			}
		}
		else
		{
			num2 = Mathf.Lerp(lastUpDown, 0f, Time.deltaTime * smoothFactor);
		}
		if (float.IsNaN(num2))
		{
			Debug.LogError("up down move angle is NaN for " + base.name);
			return;
		}
		lastUpDown = num2;
		if (RightAxis == OrientAxis.X)
		{
			base.transform.Rotate(num2, 0f, 0f);
		}
		else if (RightAxis == OrientAxis.NegX)
		{
			base.transform.Rotate(0f - num2, 0f, 0f);
		}
		else if (RightAxis == OrientAxis.Y)
		{
			base.transform.Rotate(0f, num2, 0f);
		}
		else if (RightAxis == OrientAxis.NegY)
		{
			base.transform.Rotate(0f, 0f - num2, 0f);
		}
		else if (RightAxis == OrientAxis.Z)
		{
			base.transform.Rotate(0f, 0f, num2);
		}
		else if (RightAxis == OrientAxis.NegZ)
		{
			base.transform.Rotate(0f, 0f, 0f - num2);
		}
	}

	private void Update()
	{
		if (updateTime == UpdateTime.Update)
		{
			LookAt();
		}
	}

	private void LateUpdate()
	{
		if (updateTime == UpdateTime.LateUpdate)
		{
			LookAt();
		}
	}

	private void FixedUpdate()
	{
		if (updateTime == UpdateTime.FixedUpdate)
		{
			LookAt();
		}
	}
}
