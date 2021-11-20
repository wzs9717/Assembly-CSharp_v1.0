using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(Interactable))]
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(VelocityEstimator))]
	public class Throwable : MonoBehaviour
	{
		[EnumFlags]
		[Tooltip("The flags used to attach this object to the hand.")]
		public Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.ParentToHand;

		[Tooltip("Name of the attachment transform under in the hand's hierarchy which the object should should snap to.")]
		public string attachmentPoint;

		[Tooltip("How fast must this object be moving to attach due to a trigger hold instead of a trigger press?")]
		public float catchSpeedThreshold;

		[Tooltip("When detaching the object, should it return to its original parent?")]
		public bool restoreOriginalParent;

		public bool attachEaseIn;

		public AnimationCurve snapAttachEaseInCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		public float snapAttachEaseInTime = 0.15f;

		public string[] attachEaseInAttachmentNames;

		private VelocityEstimator velocityEstimator;

		private bool attached;

		private float attachTime;

		private Vector3 attachPosition;

		private Quaternion attachRotation;

		private Transform attachEaseInTransform;

		public UnityEvent onPickUp;

		public UnityEvent onDetachFromHand;

		public bool snapAttachEaseInCompleted;

		private void Awake()
		{
			velocityEstimator = GetComponent<VelocityEstimator>();
			if (attachEaseIn)
			{
				attachmentFlags &= ~Hand.AttachmentFlags.SnapOnAttach;
			}
			Rigidbody component = GetComponent<Rigidbody>();
			component.maxAngularVelocity = 50f;
		}

		private void OnHandHoverBegin(Hand hand)
		{
			bool flag = false;
			if (!attached && hand.GetStandardInteractionButton())
			{
				Rigidbody component = GetComponent<Rigidbody>();
				if (component.velocity.magnitude >= catchSpeedThreshold)
				{
					hand.AttachObject(base.gameObject, attachmentFlags, attachmentPoint);
					flag = false;
				}
			}
			if (flag)
			{
				ControllerButtonHints.ShowButtonHint(hand, EVRButtonId.k_EButton_Axis1);
			}
		}

		private void OnHandHoverEnd(Hand hand)
		{
			ControllerButtonHints.HideButtonHint(hand, EVRButtonId.k_EButton_Axis1);
		}

		private void HandHoverUpdate(Hand hand)
		{
			if (hand.GetStandardInteractionButtonDown())
			{
				hand.AttachObject(base.gameObject, attachmentFlags, attachmentPoint);
				ControllerButtonHints.HideButtonHint(hand, EVRButtonId.k_EButton_Axis1);
			}
		}

		private void OnAttachedToHand(Hand hand)
		{
			attached = true;
			onPickUp.Invoke();
			hand.HoverLock(null);
			Rigidbody component = GetComponent<Rigidbody>();
			component.isKinematic = true;
			component.interpolation = RigidbodyInterpolation.None;
			if (hand.controller == null)
			{
				velocityEstimator.BeginEstimatingVelocity();
			}
			attachTime = Time.time;
			attachPosition = base.transform.position;
			attachRotation = base.transform.rotation;
			if (attachEaseIn)
			{
				attachEaseInTransform = hand.transform;
				if (!Util.IsNullOrEmpty(attachEaseInAttachmentNames))
				{
					float num = float.MaxValue;
					for (int i = 0; i < attachEaseInAttachmentNames.Length; i++)
					{
						Transform attachmentTransform = hand.GetAttachmentTransform(attachEaseInAttachmentNames[i]);
						float num2 = Quaternion.Angle(attachmentTransform.rotation, attachRotation);
						if (num2 < num)
						{
							attachEaseInTransform = attachmentTransform;
							num = num2;
						}
					}
				}
			}
			snapAttachEaseInCompleted = false;
		}

		private void OnDetachedFromHand(Hand hand)
		{
			attached = false;
			onDetachFromHand.Invoke();
			hand.HoverUnlock(null);
			Rigidbody component = GetComponent<Rigidbody>();
			component.isKinematic = false;
			component.interpolation = RigidbodyInterpolation.Interpolate;
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			Vector3 zero3 = Vector3.zero;
			if (hand.controller == null)
			{
				velocityEstimator.FinishEstimatingVelocity();
				zero2 = velocityEstimator.GetVelocityEstimate();
				zero3 = velocityEstimator.GetAngularVelocityEstimate();
				zero = velocityEstimator.transform.position;
			}
			else
			{
				zero2 = Player.instance.trackingOriginTransform.TransformVector(hand.controller.velocity);
				zero3 = Player.instance.trackingOriginTransform.TransformVector(hand.controller.angularVelocity);
				zero = hand.transform.position;
			}
			Vector3 rhs = base.transform.TransformPoint(component.centerOfMass) - zero;
			component.velocity = zero2 + Vector3.Cross(zero3, rhs);
			component.angularVelocity = zero3;
			float num = Time.fixedDeltaTime + Time.fixedTime - Time.time;
			base.transform.position += num * zero2;
			float num2 = 57.29578f * zero3.magnitude;
			Vector3 normalized = zero3.normalized;
			base.transform.rotation *= Quaternion.AngleAxis(num2 * num, normalized);
		}

		private void HandAttachedUpdate(Hand hand)
		{
			if (!hand.GetStandardInteractionButton())
			{
				StartCoroutine(LateDetach(hand));
			}
			if (attachEaseIn)
			{
				float num = Util.RemapNumberClamped(Time.time, attachTime, attachTime + snapAttachEaseInTime, 0f, 1f);
				if (num < 1f)
				{
					num = snapAttachEaseInCurve.Evaluate(num);
					base.transform.position = Vector3.Lerp(attachPosition, attachEaseInTransform.position, num);
					base.transform.rotation = Quaternion.Lerp(attachRotation, attachEaseInTransform.rotation, num);
				}
				else if (!snapAttachEaseInCompleted)
				{
					base.gameObject.SendMessage("OnThrowableAttachEaseInCompleted", hand, SendMessageOptions.DontRequireReceiver);
					snapAttachEaseInCompleted = true;
				}
			}
		}

		private IEnumerator LateDetach(Hand hand)
		{
			yield return new WaitForEndOfFrame();
			hand.DetachObject(base.gameObject, restoreOriginalParent);
		}

		private void OnHandFocusAcquired(Hand hand)
		{
			base.gameObject.SetActive(value: true);
			velocityEstimator.BeginEstimatingVelocity();
		}

		private void OnHandFocusLost(Hand hand)
		{
			base.gameObject.SetActive(value: false);
			velocityEstimator.FinishEstimatingVelocity();
		}
	}
}
