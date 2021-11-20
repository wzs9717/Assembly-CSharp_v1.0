using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class Interactable : MonoBehaviour
	{
		public delegate void OnAttachedToHandDelegate(Hand hand);

		public delegate void OnDetachedFromHandDelegate(Hand hand);

		[HideInInspector]
		public event OnAttachedToHandDelegate onAttachedToHand;

		[HideInInspector]
		public event OnDetachedFromHandDelegate onDetachedFromHand;

		private void OnAttachedToHand(Hand hand)
		{
			if (this.onAttachedToHand != null)
			{
				this.onAttachedToHand(hand);
			}
		}

		private void OnDetachedFromHand(Hand hand)
		{
			if (this.onDetachedFromHand != null)
			{
				this.onDetachedFromHand(hand);
			}
		}
	}
}
