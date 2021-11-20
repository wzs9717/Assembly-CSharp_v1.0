using UnityEngine;

public class FollowPhysicallySingle : MonoBehaviour
{
	public enum Method
	{
		Force,
		Move
	}

	public enum ForcePosition
	{
		RBCenter,
		HingePoint
	}

	public static float GlobalForceMultiplier = 5f;

	public static float GlobalTorqueMultiplier = 5f;

	public bool useGlobalForceMultiplier = true;

	public bool useGlobalTorqueMultiplier = true;

	public bool on = true;

	public bool drivePosition = true;

	public bool driveRotation = true;

	public Transform follow;

	public Method moveMethod;

	public Method rotateMethod;

	public float PIDpresentFactorRot = 1f;

	public float PIDintegralFactorRot;

	public float PIDderivFactorRot = 0.1f;

	public float PIDpresentFactorPos = 1f;

	public float PIDintegralFactorPos;

	public float PIDderivFactorPos = 0.1f;

	public ForcePosition forcePosition;

	public Quaternion2Angles.RotationOrder rotationOrder;

	public float ForceMultiplier = 100f;

	public float TorqueMultiplier = 50f;

	public float TorqueMultiplier2 = 5f;

	public Vector3 DebugAngles;

	public Vector3 DebugAppliedForce;

	public Vector3 DebugAppliedTorque;

	public Vector3 DebugAppliedTorque2;

	public Vector3 DebugLastErrorRotation;

	[SerializeField]
	private float _ForcePercent = 1f;

	[SerializeField]
	private float _TorquePercent = 1f;

	public float MaxForce = 100f;

	public float MaxTorque = 50f;

	public float freezeMass = 100f;

	public bool useControlJointParams;

	public float controlledJointSpring = 0.005f;

	public float controlledJointMaxForce = 0.005f;

	public bool debugForce;

	public bool debugTorque;

	public Material lineMaterial;

	public Material rotationLineMaterial;

	public FreeControllerV3[] onIfAllFCV3sFollowing;

	private float VelocityVsDistancePower = 0.05f;

	private Rigidbody RB;

	private ConfigurableJoint CJ;

	private JointDrive startingJointDrive;

	private JointDrive controlledJointDrive;

	private Vector3 integralPosition;

	private Vector3 lastErrorPosition;

	private Vector3 integralRotation;

	private Vector3 lastErrorRotation;

	private Vector3 integralRotationForward;

	private Vector3 lastErrorRotationForward;

	private Vector3 previousPosition;

	private FreeControllerV3 followFCV3;

	private float startingMass;

	private bool startingUseGravity;

	private RigidbodyConstraints startingConstraints;

	private LineDrawer lineDrawer;

	private LineDrawer rotationLineDrawer;

	private bool usingStartingGravity;

	private bool usingStartingJointConditions;

	public float ForcePercent
	{
		get
		{
			return _ForcePercent;
		}
		set
		{
			_ForcePercent = value;
		}
	}

	public float TorquePercent
	{
		get
		{
			return _TorquePercent;
		}
		set
		{
			_TorquePercent = value;
		}
	}

	public float StaticGlobalForceMultipler
	{
		get
		{
			return GlobalForceMultiplier;
		}
		set
		{
			GlobalForceMultiplier = value;
		}
	}

	public float StaticGlobalTorqueMultipler
	{
		get
		{
			return GlobalTorqueMultiplier;
		}
		set
		{
			GlobalTorqueMultiplier = value;
		}
	}

	private void Start()
	{
		RB = base.transform.GetComponent<Rigidbody>();
		CJ = base.transform.GetComponent<ConfigurableJoint>();
		if ((bool)lineMaterial)
		{
			lineDrawer = new LineDrawer(lineMaterial);
		}
		if ((bool)rotationLineMaterial)
		{
			rotationLineDrawer = new LineDrawer(12, rotationLineMaterial);
		}
		if ((bool)CJ)
		{
			startingJointDrive = CJ.slerpDrive;
			controlledJointDrive = default(JointDrive);
			controlledJointDrive.positionSpring = controlledJointSpring;
			controlledJointDrive.positionDamper = startingJointDrive.positionDamper;
			controlledJointDrive.maximumForce = controlledJointMaxForce;
			usingStartingJointConditions = true;
		}
		if ((bool)follow)
		{
			previousPosition = follow.position;
			followFCV3 = follow.GetComponent<FreeControllerV3>();
		}
	}

	private void applyTorque()
	{
		Vector3 vector = Vector3.Cross(base.transform.forward, follow.forward);
		Vector3 vector2 = Vector3.Cross(base.transform.up, follow.up);
		Vector3 vector3 = vector + vector2;
		Vector3 vector4 = vector3 * TorqueMultiplier * _TorquePercent;
		if (useGlobalTorqueMultiplier)
		{
			vector4 *= GlobalTorqueMultiplier;
		}
		Vector3 vector5 = Vector3.ClampMagnitude(vector4, MaxTorque);
		if (debugTorque)
		{
			DebugHUD.Msg(string.Concat(base.transform.name, " RAW tq: ", vector4, " clamped: ", vector5));
		}
		RB.AddTorque(vector5, ForceMode.Force);
		DebugAppliedTorque = vector5;
	}

	private void applyTorquePID()
	{
		float fixedDeltaTime = Time.fixedDeltaTime;
		Vector3 vector = Vector3.Cross(base.transform.forward, follow.forward);
		Vector3 vector2 = Vector3.Cross(base.transform.up, follow.up);
		Vector3 vector3 = vector + vector2;
		integralRotation += vector3 * fixedDeltaTime;
		Vector3 vector4 = (vector3 - lastErrorRotation) / fixedDeltaTime;
		lastErrorRotation = vector3;
		Vector3 vector5 = (vector3 * PIDpresentFactorRot + integralRotation * PIDintegralFactorRot + vector4 * PIDderivFactorRot) * TorqueMultiplier * _TorquePercent;
		if (useGlobalTorqueMultiplier)
		{
			vector5 *= GlobalTorqueMultiplier;
		}
		Vector3 vector6 = Vector3.ClampMagnitude(vector5, MaxTorque);
		if (debugTorque)
		{
			DebugHUD.Msg(string.Concat(base.transform.name, " RTq: ", vector5, " CTq: ", vector6, " A:", vector3, " D:", vector4));
		}
		RB.AddTorque(vector6, ForceMode.Force);
		DebugAppliedTorque = vector6;
	}

	private void applyTorquePID2()
	{
		float fixedDeltaTime = Time.fixedDeltaTime;
		Vector3 vector = Vector3.Cross(base.transform.up, follow.up);
		integralRotation += vector * fixedDeltaTime;
		Vector3 vector2 = (vector - lastErrorRotation) / fixedDeltaTime;
		Vector3 vector3 = lastErrorRotation;
		lastErrorRotation = vector;
		Vector3 vector4 = vector * PIDpresentFactorRot;
		Vector3 vector5 = integralRotation * PIDintegralFactorRot;
		Vector3 vector6 = vector2 * PIDderivFactorRot;
		Vector3 vector7 = vector6;
		Vector3 vector8 = (vector4 + vector5 + vector7) * TorqueMultiplier * _TorquePercent;
		if (useGlobalTorqueMultiplier)
		{
			vector8 *= GlobalTorqueMultiplier;
		}
		Vector3 vector9 = Vector3.ClampMagnitude(vector8, MaxTorque);
		if (debugTorque)
		{
			Debug.Log(base.transform.name + " alignup last error: " + vector3.ToString("F2") + " align:" + vector.ToString("F2") + " deriv:" + (vector - vector3).ToString("F2") + " aError " + vector4.ToString("F2") + " dError " + vector6.ToString("F2"));
		}
		RB.AddTorque(vector9, ForceMode.Force);
		DebugAppliedTorque = vector9;
		Vector3 vector10 = Vector3.Cross(base.transform.forward, follow.forward);
		integralRotationForward += vector10 * fixedDeltaTime;
		Vector3 vector11 = (vector10 - lastErrorRotationForward) / fixedDeltaTime;
		lastErrorRotationForward = vector10;
		Vector3 vector12 = vector10 * PIDpresentFactorRot;
		Vector3 vector13 = integralRotationForward * PIDintegralFactorRot;
		Vector3 vector14 = vector11 * PIDderivFactorRot;
		Vector3 vector15 = vector14;
		Vector3 vector16 = (vector12 + vector13 + vector15) * TorqueMultiplier2 * _TorquePercent;
		if (useGlobalTorqueMultiplier)
		{
			vector16 *= GlobalTorqueMultiplier;
		}
		Vector3 vector17 = Vector3.ClampMagnitude(vector16, MaxTorque);
		if (debugTorque)
		{
			Debug.Log(string.Concat(base.transform.name, " apply alignforward raw torque ", vector16, " clamped torque ", vector17));
		}
		RB.AddTorque(vector17, ForceMode.Force);
		DebugAppliedTorque2 = vector17;
	}

	private void applyTorquePID3()
	{
		float fixedDeltaTime = Time.fixedDeltaTime;
		Quaternion q = follow.rotation * Quaternion.Inverse(base.transform.rotation);
		Vector3 angles = Quaternion2Angles.GetAngles(q, rotationOrder);
		Vector3 vector = angles * TorqueMultiplier * _TorquePercent;
		integralRotation += vector * fixedDeltaTime;
		Vector3 vector2 = (vector - lastErrorRotation) / fixedDeltaTime;
		lastErrorRotation = vector;
		Vector3 vector3 = (vector * PIDpresentFactorRot + integralRotation * PIDintegralFactorRot + vector2 * PIDderivFactorRot) * TorqueMultiplier * _TorquePercent;
		if (useGlobalTorqueMultiplier)
		{
			vector3 *= GlobalTorqueMultiplier;
		}
		Vector3 vector4 = Vector3.ClampMagnitude(vector3, MaxTorque);
		RB.AddTorque(vector4, ForceMode.Force);
		DebugAppliedTorque = vector4;
	}

	private void applyForce()
	{
		Vector3 position = follow.position;
		Vector3 velocity = RB.velocity;
		Vector3 vector = position - previousPosition;
		previousPosition = position;
		Vector3 vector2 = vector - velocity;
		Vector3 vector3 = vector2 * VelocityVsDistancePower;
		Vector3 vector4 = position - base.transform.position;
		Vector3 vector5 = vector4 * (1f - VelocityVsDistancePower);
		Vector3 vector6 = vector5 + vector3;
		Vector3 vector7 = vector6 * ForceMultiplier * _ForcePercent;
		if (useGlobalForceMultiplier)
		{
			vector7 *= GlobalForceMultiplier;
		}
		Vector3 vector8 = Vector3.ClampMagnitude(vector7, MaxForce);
		if (debugForce)
		{
			DebugHUD.Msg(string.Concat(base.transform.name, " RAW frc: ", vector7, " clamped: ", vector8));
		}
		if (forcePosition == ForcePosition.RBCenter)
		{
			RB.AddForce(vector8, ForceMode.Force);
		}
		else
		{
			RB.AddForceAtPosition(vector8, base.transform.position, ForceMode.Force);
		}
		DebugAppliedForce = vector8;
	}

	private void applyForcePID()
	{
		float fixedDeltaTime = Time.fixedDeltaTime;
		Vector3 vector = follow.position - base.transform.position;
		integralPosition += vector * fixedDeltaTime;
		Vector3 vector2 = (vector - lastErrorPosition) / fixedDeltaTime;
		lastErrorPosition = vector;
		Vector3 vector3 = (vector * PIDpresentFactorPos + integralPosition * PIDintegralFactorPos + vector2 * PIDderivFactorPos) * ForceMultiplier * _ForcePercent;
		if (useGlobalForceMultiplier)
		{
			vector3 *= GlobalForceMultiplier;
		}
		Vector3 vector4 = Vector3.ClampMagnitude(vector3, MaxForce);
		if (forcePosition == ForcePosition.RBCenter)
		{
			RB.AddForce(vector4, ForceMode.Force);
		}
		else
		{
			RB.AddForceAtPosition(vector4, base.transform.position, ForceMode.Force);
		}
		DebugAppliedForce = vector4;
	}

	private void setRotation()
	{
		RB.MoveRotation(follow.rotation);
	}

	private void setPosition()
	{
		RB.MovePosition(follow.position);
	}

	private void drawPositionLines()
	{
		lineDrawer.SetLinePoints(base.transform.position, follow.position);
		lineDrawer.Draw();
	}

	private void drawRotationLines()
	{
		float num = 0.02f;
		Vector3 vector = base.transform.forward * num;
		Vector3 vector2 = follow.forward * num;
		Vector3 position = base.transform.position;
		Vector3 point = position + vector;
		Vector3 point2 = position + vector2;
		Vector3 point3 = position + vector * 2f;
		Vector3 point4 = position + vector2 * 2f;
		Vector3 point5 = position + vector * 3f;
		Vector3 point6 = position + vector2 * 3f;
		Vector3 vector3 = position + vector * 4f;
		Vector3 point7 = position + vector2 * 4f;
		rotationLineDrawer.SetLinePoints(0, position, vector3);
		rotationLineDrawer.SetLinePoints(1, position, point7);
		rotationLineDrawer.SetLinePoints(2, point, point2);
		rotationLineDrawer.SetLinePoints(3, point3, point4);
		rotationLineDrawer.SetLinePoints(4, point5, point6);
		rotationLineDrawer.SetLinePoints(5, vector3, point7);
		rotationLineDrawer.Draw();
		Vector3 vector4 = base.transform.up * num;
		Vector3 vector5 = follow.up * num;
		Vector3 position2 = base.transform.position;
		Vector3 point8 = position2 + vector4;
		Vector3 point9 = position2 + vector5;
		Vector3 point10 = position2 + vector4 * 2f;
		Vector3 point11 = position2 + vector5 * 2f;
		Vector3 point12 = position2 + vector4 * 3f;
		Vector3 point13 = position2 + vector5 * 3f;
		Vector3 vector6 = position2 + vector4 * 4f;
		Vector3 point14 = position2 + vector5 * 4f;
		rotationLineDrawer.SetLinePoints(6, position2, vector6);
		rotationLineDrawer.SetLinePoints(7, position2, point14);
		rotationLineDrawer.SetLinePoints(8, point8, point9);
		rotationLineDrawer.SetLinePoints(9, point10, point11);
		rotationLineDrawer.SetLinePoints(10, point12, point13);
		rotationLineDrawer.SetLinePoints(11, vector6, point14);
		rotationLineDrawer.Draw();
	}

	private void Update()
	{
		if (lineDrawer != null && drivePosition && RB != null && follow != null && followFCV3 != null && followFCV3.isPositionOn && !followFCV3.hidden)
		{
			drawPositionLines();
		}
		if (rotationLineDrawer != null && driveRotation && RB != null && follow != null && followFCV3 != null && followFCV3.isRotationOn && !followFCV3.hidden)
		{
			drawRotationLines();
		}
	}

	private void FixedUpdate()
	{
		if (onIfAllFCV3sFollowing != null && onIfAllFCV3sFollowing.Length > 0)
		{
			on = true;
			bool flag = true;
			FreeControllerV3[] array = onIfAllFCV3sFollowing;
			foreach (FreeControllerV3 freeControllerV in array)
			{
				if (freeControllerV.currentRotationState != FreeControllerV3.RotationState.Following || freeControllerV.currentPositionState != FreeControllerV3.PositionState.Following)
				{
					on = false;
				}
				if (!freeControllerV.hidden)
				{
					flag = false;
				}
			}
			if (on && !flag)
			{
				if (lineDrawer != null)
				{
					drawPositionLines();
				}
				if (rotationLineDrawer != null)
				{
					drawRotationLines();
				}
			}
		}
		bool flag2 = false;
		if (on)
		{
			if (driveRotation && RB != null && follow != null && (followFCV3 == null || followFCV3.isRotationOn))
			{
				if (rotateMethod == Method.Move)
				{
					setRotation();
				}
				else if (rotateMethod == Method.Force)
				{
					flag2 = true;
					applyTorquePID3();
				}
			}
			if (drivePosition && RB != null && follow != null && (followFCV3 == null || followFCV3.isPositionOn))
			{
				if (moveMethod == Method.Move)
				{
					setPosition();
				}
				else if (moveMethod == Method.Force)
				{
					applyForcePID();
				}
			}
		}
		if (useControlJointParams && (bool)CJ && flag2 && usingStartingJointConditions)
		{
			usingStartingJointConditions = false;
			CJ.slerpDrive = controlledJointDrive;
		}
		if ((bool)CJ && !flag2 && !usingStartingJointConditions)
		{
			usingStartingJointConditions = true;
			CJ.slerpDrive = startingJointDrive;
		}
	}
}
