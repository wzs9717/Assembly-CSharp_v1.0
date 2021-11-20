using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(Interactable))]
	public class InteractableButtonEvents : MonoBehaviour
	{
		public UnityEvent onTriggerDown;

		public UnityEvent onTriggerUp;

		public UnityEvent onGripDown;

		public UnityEvent onGripUp;

		public UnityEvent onTouchpadDown;

		public UnityEvent onTouchpadUp;

		public UnityEvent onTouchpadTouch;

		public UnityEvent onTouchpadRelease;

		private void Update()
		{
			for (int i = 0; i < Player.instance.handCount; i++)
			{
				Hand hand = Player.instance.GetHand(i);
				if (hand.controller != null)
				{
					if (hand.controller.GetPressDown(EVRButtonId.k_EButton_Axis1))
					{
						onTriggerDown.Invoke();
					}
					if (hand.controller.GetPressUp(EVRButtonId.k_EButton_Axis1))
					{
						onTriggerUp.Invoke();
					}
					if (hand.controller.GetPressDown(EVRButtonId.k_EButton_Grip))
					{
						onGripDown.Invoke();
					}
					if (hand.controller.GetPressUp(EVRButtonId.k_EButton_Grip))
					{
						onGripUp.Invoke();
					}
					if (hand.controller.GetPressDown(EVRButtonId.k_EButton_Axis0))
					{
						onTouchpadDown.Invoke();
					}
					if (hand.controller.GetPressUp(EVRButtonId.k_EButton_Axis0))
					{
						onTouchpadUp.Invoke();
					}
					if (hand.controller.GetTouchDown(EVRButtonId.k_EButton_Axis0))
					{
						onTouchpadTouch.Invoke();
					}
					if (hand.controller.GetTouchUp(EVRButtonId.k_EButton_Axis0))
					{
						onTouchpadRelease.Invoke();
					}
				}
			}
		}
	}
}
