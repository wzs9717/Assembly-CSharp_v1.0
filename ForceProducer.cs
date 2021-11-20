using UnityEngine;

public class ForceProducer : MonoBehaviour
{
	public enum AxisName
	{
		X,
		Y,
		Z,
		NegX,
		NegY,
		NegZ
	}

	public bool on = true;

	public ForceReceiver receiver;

	public bool useForce = true;

	public AxisName ForceAxis;

	public bool useTorque = true;

	public AxisName TorqueAxis = AxisName.Z;

	public float ForceFactor = 200f;

	public float MaxForce = 5000f;

	public float TorqueFactor = 100f;

	public float MaxTorque = 1000f;

	public float ForceQuickness = 10f;

	public float TorqueQuickness = 10f;

	public Material linkLineMaterial;

	public Material forceLineMaterial;

	public Material targetForceLineMaterial;

	public Material rawForceLineMaterial;

	public bool drawLines = true;

	public float linesScale = 1E-05f;

	public float lineOffset = 0.1f;

	public float lineSpacing = 0.01f;

	public float targetForceMagnitude;

	protected Vector3 ForceDirection;

	protected Vector3 TorqueDirection;

	protected Vector3 currentForce;

	protected Vector3 rawForce;

	protected Vector3 targetForce;

	protected Vector3 currentTorque;

	protected Vector3 rawTorque;

	protected Vector3 targetTorque;

	protected LineDrawer linkLineDrawer;

	protected LineDrawer forceLineDrawer;

	protected LineDrawer targetForceLineDrawer;

	protected LineDrawer rawForceLineDrawer;

	protected Rigidbody RB;

	protected PlatformOrganizer platformOrganizer;

	protected bool wasInit;

	protected float inputMax = 100f;

	protected float torqueLineMult = 4f;

	protected float maxForceFactor = 400f;

	protected float maxMaxForce = 40000f;

	protected float maxForceQuickness = 20f;

	protected float maxTorqueFactor = 100f;

	protected float maxMaxTorque = 10000f;

	protected float maxTorqueQuickness = 20f;

	protected virtual void InitGUI()
	{
		wasInit = true;
		if (!(receiver != null))
		{
		}
	}

	protected virtual void Start()
	{
		platformOrganizer = GetComponentInParent<PlatformOrganizer>();
		if ((bool)linkLineMaterial)
		{
			linkLineDrawer = new LineDrawer(linkLineMaterial);
		}
		if ((bool)forceLineMaterial)
		{
			forceLineDrawer = new LineDrawer(2, forceLineMaterial);
		}
		if ((bool)targetForceLineMaterial)
		{
			targetForceLineDrawer = new LineDrawer(6, targetForceLineMaterial);
		}
		if ((bool)rawForceLineMaterial)
		{
			rawForceLineDrawer = new LineDrawer(6, rawForceLineMaterial);
		}
		if ((bool)receiver)
		{
			RB = receiver.GetComponent<Rigidbody>();
		}
	}

	public void SetReceiver(ForceReceiver rcvr)
	{
		if (wasInit)
		{
			receiver = rcvr;
			if (receiver != null)
			{
				RB = receiver.GetComponent<Rigidbody>();
			}
			else
			{
				RB = null;
			}
		}
	}

	public void SetForceReceiverFromPopupList()
	{
	}

	public void SetOnFromUIToggle()
	{
	}

	public void SetForceFactorFromUISlider()
	{
	}

	public void SetMaxForceFromUISlider()
	{
	}

	public void SetForceQuicknessFromUISlider()
	{
	}

	public void SetTorqueFactorFromUISlider()
	{
	}

	public void SetMaxTorqueFromUISlider()
	{
	}

	public void SetTorqueQuicknessFromUISlider()
	{
	}

	protected virtual void SetTargetForce(float force)
	{
		targetForceMagnitude = force;
		if (useForce)
		{
			rawForce = ForceDirection * force * ForceFactor;
			if (rawForce.magnitude > MaxForce)
			{
				targetForce = Vector3.ClampMagnitude(rawForce, MaxForce);
			}
			else
			{
				targetForce = rawForce;
			}
		}
		if (useTorque)
		{
			rawTorque = TorqueDirection * force * TorqueFactor;
			if (rawTorque.magnitude > MaxTorque)
			{
				targetTorque = Vector3.ClampMagnitude(rawTorque, MaxTorque);
			}
			else
			{
				targetTorque = rawTorque;
			}
		}
	}

	protected virtual void ApplyForce()
	{
		float num = Time.fixedDeltaTime / Time.timeScale;
		currentForce = Vector3.Lerp(currentForce, targetForce, num * ForceQuickness);
		currentTorque = Vector3.Lerp(currentTorque, targetTorque, num * TorqueQuickness);
		if ((bool)RB && on)
		{
			if (useForce)
			{
				RB.AddForce(currentForce * num * RB.mass);
			}
			if (useTorque)
			{
				RB.AddTorque(currentTorque * num * RB.mass);
			}
		}
	}

	protected Vector3 AxisToVector(AxisName axis)
	{
		return axis switch
		{
			AxisName.X => base.transform.right, 
			AxisName.NegX => -base.transform.right, 
			AxisName.Y => base.transform.up, 
			AxisName.NegY => -base.transform.up, 
			AxisName.Z => base.transform.forward, 
			AxisName.NegZ => -base.transform.forward, 
			_ => Vector3.zero, 
		};
	}

	protected Vector3 AxisToUpVector(AxisName axis)
	{
		return axis switch
		{
			AxisName.X => base.transform.up, 
			AxisName.NegX => base.transform.up, 
			AxisName.Y => base.transform.forward, 
			AxisName.NegY => base.transform.forward, 
			AxisName.Z => base.transform.up, 
			AxisName.NegZ => base.transform.up, 
			_ => Vector3.zero, 
		};
	}

	protected Vector3 getDrawTorque(Vector3 trq)
	{
		return TorqueAxis switch
		{
			AxisName.X => Quaternion.FromToRotation(-base.transform.right, AxisToVector(ForceAxis)), 
			AxisName.NegX => Quaternion.FromToRotation(-base.transform.right, AxisToVector(ForceAxis)), 
			AxisName.Y => Quaternion.FromToRotation(base.transform.up, AxisToVector(ForceAxis)), 
			AxisName.NegY => Quaternion.FromToRotation(-base.transform.up, AxisToVector(ForceAxis)), 
			AxisName.Z => Quaternion.FromToRotation(base.transform.forward, AxisToVector(ForceAxis)), 
			AxisName.NegZ => Quaternion.FromToRotation(-base.transform.forward, AxisToVector(ForceAxis)), 
			_ => Quaternion.identity, 
		} * trq;
	}

	protected virtual void FixedUpdate()
	{
		ApplyForce();
	}

	protected virtual void Update()
	{
		if (!wasInit)
		{
			InitGUI();
		}
		if (useForce)
		{
			ForceDirection = AxisToVector(ForceAxis);
		}
		if (useTorque)
		{
			TorqueDirection = AxisToVector(TorqueAxis);
		}
		if (on && receiver != null && drawLines)
		{
			Vector3 vector = AxisToVector(ForceAxis);
			Vector3 drawTorque = getDrawTorque(AxisToVector(TorqueAxis));
			Vector3 vector2 = AxisToUpVector(ForceAxis);
			if (linkLineDrawer != null)
			{
				linkLineDrawer.SetLinePoints(base.transform.position, receiver.transform.position);
				linkLineDrawer.Draw();
			}
			if (forceLineDrawer != null)
			{
				Vector3 vector3 = base.transform.position + vector2 * lineOffset;
				forceLineDrawer.SetLinePoints(0, vector3, vector3 + currentForce * linesScale);
				vector3 += vector2 * lineSpacing * 5f;
				Vector3 drawTorque2 = getDrawTorque(currentTorque);
				forceLineDrawer.SetLinePoints(1, vector3, vector3 + drawTorque2 * linesScale * torqueLineMult);
				targetForceLineDrawer.Draw();
				forceLineDrawer.Draw();
			}
			if (targetForceLineDrawer != null)
			{
				Vector3 vector4 = base.transform.position + vector2 * (lineOffset + lineSpacing);
				targetForceLineDrawer.SetLinePoints(0, vector4, vector4 + targetForce * linesScale);
				Vector3 vector5 = vector * MaxForce * linesScale;
				Vector3 vector6 = vector4 + vector5;
				targetForceLineDrawer.SetLinePoints(1, vector6 - vector2 * lineSpacing, vector6 + vector2 * lineSpacing);
				vector6 = vector4 - vector5;
				targetForceLineDrawer.SetLinePoints(2, vector6 - vector2 * lineSpacing, vector6 + vector2 * lineSpacing);
				vector4 += vector2 * lineSpacing * 5f;
				Vector3 drawTorque3 = getDrawTorque(targetTorque);
				targetForceLineDrawer.SetLinePoints(3, vector4, vector4 + drawTorque3 * linesScale * torqueLineMult);
				vector5 = drawTorque * MaxTorque * linesScale * torqueLineMult;
				vector6 = vector4 + vector5;
				targetForceLineDrawer.SetLinePoints(4, vector6 - vector2 * lineSpacing, vector6 + vector2 * lineSpacing);
				vector6 = vector4 - vector5;
				targetForceLineDrawer.SetLinePoints(5, vector6 - vector2 * lineSpacing, vector6 + vector2 * lineSpacing);
				targetForceLineDrawer.Draw();
			}
			if (rawForceLineDrawer != null)
			{
				Vector3 vector7 = base.transform.position + vector2 * (lineOffset + lineSpacing * 2f);
				rawForceLineDrawer.SetLinePoints(0, vector7, vector7 + rawForce * linesScale);
				Vector3 vector8 = vector * ForceFactor * inputMax * linesScale;
				Vector3 vector9 = vector7 + vector8;
				rawForceLineDrawer.SetLinePoints(1, vector9 - vector2 * lineSpacing, vector9 + vector2 * lineSpacing);
				vector9 = vector7 - vector8;
				rawForceLineDrawer.SetLinePoints(2, vector9 - vector2 * lineSpacing, vector9 + vector2 * lineSpacing);
				vector7 += vector2 * lineSpacing * 5f;
				Vector3 drawTorque4 = getDrawTorque(rawTorque);
				rawForceLineDrawer.SetLinePoints(3, vector7, vector7 + drawTorque4 * linesScale * torqueLineMult);
				vector8 = drawTorque * TorqueFactor * inputMax * linesScale * torqueLineMult;
				vector9 = vector7 + vector8;
				rawForceLineDrawer.SetLinePoints(4, vector9 - vector2 * lineSpacing, vector9 + vector2 * lineSpacing);
				vector9 = vector7 - vector8;
				rawForceLineDrawer.SetLinePoints(5, vector9 - vector2 * lineSpacing, vector9 + vector2 * lineSpacing);
				rawForceLineDrawer.Draw();
			}
		}
	}
}
