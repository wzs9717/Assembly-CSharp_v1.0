using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class OVRGrabber : MonoBehaviour
{
	public float grabBegin = 0.55f;

	public float grabEnd = 0.35f;

	[SerializeField]
	protected bool m_parentHeldObject;

	[SerializeField]
	protected Transform m_gripTransform;

	[SerializeField]
	protected Collider[] m_grabVolumes;

	[SerializeField]
	protected OVRInput.Controller m_controller;

	[SerializeField]
	protected Transform m_parentTransform;

	protected bool m_grabVolumeEnabled = true;

	protected Vector3 m_lastPos;

	protected Quaternion m_lastRot;

	protected Quaternion m_anchorOffsetRotation;

	protected Vector3 m_anchorOffsetPosition;

	protected float m_prevFlex;

	protected OVRGrabbable m_grabbedObj;

	private Vector3 m_grabbedObjectPosOff;

	private Quaternion m_grabbedObjectRotOff;

	protected Dictionary<OVRGrabbable, int> m_grabCandidates = new Dictionary<OVRGrabbable, int>();

	public OVRGrabbable grabbedObject => m_grabbedObj;

	public void ForceRelease(OVRGrabbable grabbable)
	{
		if (m_grabbedObj != null && m_grabbedObj == grabbable)
		{
			GrabEnd();
		}
	}

	private void Awake()
	{
		m_anchorOffsetPosition = base.transform.localPosition;
		m_anchorOffsetRotation = base.transform.localRotation;
	}

	private void Start()
	{
		m_lastPos = base.transform.position;
		m_lastRot = base.transform.rotation;
		if (m_parentTransform == null)
		{
			if (base.gameObject.transform.parent != null)
			{
				m_parentTransform = base.gameObject.transform.parent.transform;
				return;
			}
			m_parentTransform = new GameObject().transform;
			m_parentTransform.position = Vector3.zero;
			m_parentTransform.rotation = Quaternion.identity;
		}
	}

	private void FixedUpdate()
	{
		Vector3 localControllerPosition = OVRInput.GetLocalControllerPosition(m_controller);
		Quaternion localControllerRotation = OVRInput.GetLocalControllerRotation(m_controller);
		Vector3 vector = m_parentTransform.TransformPoint(m_anchorOffsetPosition + localControllerPosition);
		Quaternion rot = m_parentTransform.rotation * localControllerRotation * m_anchorOffsetRotation;
		GetComponent<Rigidbody>().MovePosition(vector);
		GetComponent<Rigidbody>().MoveRotation(rot);
		if (!m_parentHeldObject)
		{
			MoveGrabbedObject(vector, rot);
		}
		m_lastPos = base.transform.position;
		m_lastRot = base.transform.rotation;
		float prevFlex = m_prevFlex;
		m_prevFlex = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller);
		CheckForGrabOrRelease(prevFlex);
	}

	private void OnDestroy()
	{
		if (m_grabbedObj != null)
		{
			GrabEnd();
		}
	}

	private void OnTriggerEnter(Collider otherCollider)
	{
		OVRGrabbable oVRGrabbable = otherCollider.GetComponent<OVRGrabbable>() ?? otherCollider.GetComponentInParent<OVRGrabbable>();
		if (!(oVRGrabbable == null))
		{
			int value = 0;
			m_grabCandidates.TryGetValue(oVRGrabbable, out value);
			m_grabCandidates[oVRGrabbable] = value + 1;
		}
	}

	private void OnTriggerExit(Collider otherCollider)
	{
		OVRGrabbable oVRGrabbable = otherCollider.GetComponent<OVRGrabbable>() ?? otherCollider.GetComponentInParent<OVRGrabbable>();
		if (oVRGrabbable == null)
		{
			return;
		}
		int value = 0;
		if (m_grabCandidates.TryGetValue(oVRGrabbable, out value))
		{
			if (value > 1)
			{
				m_grabCandidates[oVRGrabbable] = value - 1;
			}
			else
			{
				m_grabCandidates.Remove(oVRGrabbable);
			}
		}
	}

	protected void CheckForGrabOrRelease(float prevFlex)
	{
		if (m_prevFlex >= grabBegin && prevFlex < grabBegin)
		{
			GrabBegin();
		}
		else if (m_prevFlex <= grabEnd && prevFlex > grabEnd)
		{
			GrabEnd();
		}
	}

	protected void GrabBegin()
	{
		float num = float.MaxValue;
		OVRGrabbable oVRGrabbable = null;
		Collider grabPoint = null;
		foreach (OVRGrabbable key in m_grabCandidates.Keys)
		{
			if (key.isGrabbed && !key.allowOffhandGrab)
			{
				continue;
			}
			for (int i = 0; i < key.grabPoints.Length; i++)
			{
				Collider collider = key.grabPoints[i];
				Vector3 vector = collider.ClosestPointOnBounds(m_gripTransform.position);
				float sqrMagnitude = (m_gripTransform.position - vector).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					oVRGrabbable = key;
					grabPoint = collider;
				}
			}
		}
		GrabVolumeEnable(enabled: false);
		if (!(oVRGrabbable != null))
		{
			return;
		}
		if (oVRGrabbable.isGrabbed)
		{
			oVRGrabbable.grabbedBy.OffhandGrabbed(oVRGrabbable);
		}
		m_grabbedObj = oVRGrabbable;
		m_grabbedObj.GrabBegin(this, grabPoint);
		m_lastPos = base.transform.position;
		m_lastRot = base.transform.rotation;
		if (m_grabbedObj.snapPosition)
		{
			m_grabbedObjectPosOff = m_gripTransform.localPosition;
			if ((bool)m_grabbedObj.snapOffset)
			{
				Vector3 position = m_grabbedObj.snapOffset.position;
				if (m_controller == OVRInput.Controller.LTouch)
				{
					position.x = 0f - position.x;
				}
				m_grabbedObjectPosOff += position;
			}
		}
		else
		{
			Vector3 vector2 = m_grabbedObj.transform.position - base.transform.position;
			vector2 = (m_grabbedObjectPosOff = Quaternion.Inverse(base.transform.rotation) * vector2);
		}
		if (!m_grabbedObj.snapOrientation)
		{
			Quaternion quaternion = (m_grabbedObjectRotOff = Quaternion.Inverse(base.transform.rotation) * m_grabbedObj.transform.rotation);
		}
		else
		{
			m_grabbedObjectRotOff = m_gripTransform.localRotation;
			if ((bool)m_grabbedObj.snapOffset)
			{
				m_grabbedObjectRotOff = m_grabbedObj.snapOffset.rotation * m_grabbedObjectRotOff;
			}
		}
		MoveGrabbedObject(m_lastPos, m_lastRot, forceTeleport: true);
		if (m_parentHeldObject)
		{
			m_grabbedObj.transform.parent = base.transform;
		}
	}

	protected void MoveGrabbedObject(Vector3 pos, Quaternion rot, bool forceTeleport = false)
	{
		if (!(m_grabbedObj == null))
		{
			Rigidbody grabbedRigidbody = m_grabbedObj.grabbedRigidbody;
			Vector3 position = pos + rot * m_grabbedObjectPosOff;
			Quaternion quaternion = rot * m_grabbedObjectRotOff;
			if (forceTeleport)
			{
				grabbedRigidbody.transform.position = position;
				grabbedRigidbody.transform.rotation = quaternion;
			}
			else
			{
				grabbedRigidbody.MovePosition(position);
				grabbedRigidbody.MoveRotation(quaternion);
			}
		}
	}

	protected void GrabEnd()
	{
		if (m_grabbedObj != null)
		{
			OVRPose oVRPose = default(OVRPose);
			oVRPose.position = OVRInput.GetLocalControllerPosition(m_controller);
			oVRPose.orientation = OVRInput.GetLocalControllerRotation(m_controller);
			OVRPose oVRPose2 = oVRPose;
			oVRPose = default(OVRPose);
			oVRPose.position = m_anchorOffsetPosition;
			oVRPose.orientation = m_anchorOffsetRotation;
			OVRPose oVRPose3 = oVRPose;
			oVRPose2 *= oVRPose3;
			OVRPose oVRPose4 = base.transform.ToOVRPose() * oVRPose2.Inverse();
			Vector3 linearVelocity = oVRPose4.orientation * OVRInput.GetLocalControllerVelocity(m_controller);
			Vector3 angularVelocity = oVRPose4.orientation * OVRInput.GetLocalControllerAngularVelocity(m_controller);
			GrabbableRelease(linearVelocity, angularVelocity);
		}
		GrabVolumeEnable(enabled: true);
	}

	protected void GrabbableRelease(Vector3 linearVelocity, Vector3 angularVelocity)
	{
		m_grabbedObj.GrabEnd(linearVelocity, angularVelocity);
		if (m_parentHeldObject)
		{
			m_grabbedObj.transform.parent = null;
		}
		m_grabbedObj = null;
	}

	protected void GrabVolumeEnable(bool enabled)
	{
		if (m_grabVolumeEnabled != enabled)
		{
			m_grabVolumeEnabled = enabled;
			for (int i = 0; i < m_grabVolumes.Length; i++)
			{
				Collider collider = m_grabVolumes[i];
				collider.enabled = m_grabVolumeEnabled;
			}
			if (!m_grabVolumeEnabled)
			{
				m_grabCandidates.Clear();
			}
		}
	}

	protected void OffhandGrabbed(OVRGrabbable grabbable)
	{
		if (m_grabbedObj == grabbable)
		{
			GrabbableRelease(Vector3.zero, Vector3.zero);
		}
	}
}
