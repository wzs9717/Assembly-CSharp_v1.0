using UnityEngine;
using UnityEngine.UI;

namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(Interactable))]
	public class UIElement : MonoBehaviour
	{
		public CustomEvents.UnityEventHand onHandClick;

		private Hand currentHand;

		private void Awake()
		{
			Button component = GetComponent<Button>();
			if ((bool)component)
			{
				component.onClick.AddListener(OnButtonClick);
			}
		}

		private void OnHandHoverBegin(Hand hand)
		{
			currentHand = hand;
			InputModule.instance.HoverBegin(base.gameObject);
			ControllerButtonHints.ShowButtonHint(hand, EVRButtonId.k_EButton_Axis1);
		}

		private void OnHandHoverEnd(Hand hand)
		{
			InputModule.instance.HoverEnd(base.gameObject);
			ControllerButtonHints.HideButtonHint(hand, EVRButtonId.k_EButton_Axis1);
			currentHand = null;
		}

		private void HandHoverUpdate(Hand hand)
		{
			if (hand.GetStandardInteractionButtonDown())
			{
				InputModule.instance.Submit(base.gameObject);
				ControllerButtonHints.HideButtonHint(hand, EVRButtonId.k_EButton_Axis1);
			}
		}

		private void OnButtonClick()
		{
			onHandClick.Invoke(currentHand);
		}
	}
}
