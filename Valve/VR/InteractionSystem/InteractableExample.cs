using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(Interactable))]
	public class InteractableExample : MonoBehaviour
	{
		private TextMesh textMesh;

		private Vector3 oldPosition;

		private Quaternion oldRotation;

		private float attachTime;

		private Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.ParentToHand;

		private void Awake()
		{
			textMesh = GetComponentInChildren<TextMesh>();
			textMesh.text = "No Hand Hovering";
		}

		private void OnHandHoverBegin(Hand hand)
		{
			textMesh.text = "Hovering hand: " + hand.name;
		}

		private void OnHandHoverEnd(Hand hand)
		{
			textMesh.text = "No Hand Hovering";
		}

		private void HandHoverUpdate(Hand hand)
		{
			if (hand.GetStandardInteractionButtonDown() || (hand.controller != null && hand.controller.GetPressDown(EVRButtonId.k_EButton_Grip)))
			{
				if (hand.currentAttachedObject != base.gameObject)
				{
					oldPosition = base.transform.position;
					oldRotation = base.transform.rotation;
					hand.HoverLock(GetComponent<Interactable>());
					hand.AttachObject(base.gameObject, attachmentFlags, string.Empty);
				}
				else
				{
					hand.DetachObject(base.gameObject);
					hand.HoverUnlock(GetComponent<Interactable>());
					base.transform.position = oldPosition;
					base.transform.rotation = oldRotation;
				}
			}
		}

		private void OnAttachedToHand(Hand hand)
		{
			textMesh.text = "Attached to hand: " + hand.name;
			attachTime = Time.time;
		}

		private void OnDetachedFromHand(Hand hand)
		{
			textMesh.text = "Detached from hand: " + hand.name;
		}

		private void HandAttachedUpdate(Hand hand)
		{
			textMesh.text = "Attached to hand: " + hand.name + "\nAttached time: " + (Time.time - attachTime).ToString("F2");
		}

		private void OnHandFocusAcquired(Hand hand)
		{
		}

		private void OnHandFocusLost(Hand hand)
		{
		}
	}
}
