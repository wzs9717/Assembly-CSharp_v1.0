using System;
using UnityEngine;

public class OVRGrabbable : MonoBehaviour
{
	[SerializeField]
	protected bool m_allowOffhandGrab = true;

	[SerializeField]
	protected bool m_snapPosition;

	[SerializeField]
	protected bool m_snapOrientation;

	[SerializeField]
	protected Transform m_snapOffset;

	[SerializeField]
	protected Collider[] m_grabPoints;

	protected bool m_grabbedKinematic;

	protected Collider m_grabbedCollider;

	protected OVRGrabber m_grabbedBy;

	public bool allowOffhandGrab => m_allowOffhandGrab;

	public bool isGrabbed => m_grabbedBy != null;

	public bool snapPosition => m_snapPosition;

	public bool snapOrientation => m_snapOrientation;

	public Transform snapOffset => m_snapOffset;

	public OVRGrabber grabbedBy => m_grabbedBy;

	public Transform grabbedTransform => m_grabbedCollider.transform;

	public Rigidbody grabbedRigidbody => m_grabbedCollider.attachedRigidbody;

	public Collider[] grabPoints => m_grabPoints;

	public virtual void GrabBegin(OVRGrabber hand, Collider grabPoint)
	{
		m_grabbedBy = hand;
		m_grabbedCollider = grabPoint;
		base.gameObject.GetComponent<Rigidbody>().isKinematic = true;
	}

	public virtual void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
	{
		Rigidbody component = base.gameObject.GetComponent<Rigidbody>();
		component.isKinematic = m_grabbedKinematic;
		component.velocity = linearVelocity;
		component.angularVelocity = angularVelocity;
		m_grabbedBy = null;
		m_grabbedCollider = null;
	}

	private void Awake()
	{
		if (m_grabPoints.Length == 0)
		{
			Collider component = GetComponent<Collider>();
			if (component == null)
			{
				throw new ArgumentException("Grabbables cannot have zero grab points and no collider -- please add a grab point or collider.");
			}
			m_grabPoints = new Collider[1] { component };
		}
	}

	private void Start()
	{
		m_grabbedKinematic = GetComponent<Rigidbody>().isKinematic;
	}

	private void OnDestroy()
	{
		if (m_grabbedBy != null)
		{
			m_grabbedBy.ForceRelease(this);
		}
	}
}
