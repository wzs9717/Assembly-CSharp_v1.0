using System.Collections;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class ControllerHintsExample : MonoBehaviour
	{
		private Coroutine buttonHintCoroutine;

		private Coroutine textHintCoroutine;

		public void ShowButtonHints(Hand hand)
		{
			if (buttonHintCoroutine != null)
			{
				StopCoroutine(buttonHintCoroutine);
			}
			buttonHintCoroutine = StartCoroutine(TestButtonHints(hand));
		}

		public void ShowTextHints(Hand hand)
		{
			if (textHintCoroutine != null)
			{
				StopCoroutine(textHintCoroutine);
			}
			textHintCoroutine = StartCoroutine(TestTextHints(hand));
		}

		public void DisableHints()
		{
			if (buttonHintCoroutine != null)
			{
				StopCoroutine(buttonHintCoroutine);
				buttonHintCoroutine = null;
			}
			if (textHintCoroutine != null)
			{
				StopCoroutine(textHintCoroutine);
				textHintCoroutine = null;
			}
			Hand[] hands = Player.instance.hands;
			foreach (Hand hand in hands)
			{
				ControllerButtonHints.HideAllButtonHints(hand);
				ControllerButtonHints.HideAllTextHints(hand);
			}
		}

		private IEnumerator TestButtonHints(Hand hand)
		{
			ControllerButtonHints.HideAllButtonHints(hand);
			while (true)
			{
				ControllerButtonHints.ShowButtonHint(hand, EVRButtonId.k_EButton_ApplicationMenu);
				yield return new WaitForSeconds(1f);
				ControllerButtonHints.ShowButtonHint(hand, default(EVRButtonId));
				yield return new WaitForSeconds(1f);
				ControllerButtonHints.ShowButtonHint(hand, EVRButtonId.k_EButton_Grip);
				yield return new WaitForSeconds(1f);
				ControllerButtonHints.ShowButtonHint(hand, EVRButtonId.k_EButton_Axis1);
				yield return new WaitForSeconds(1f);
				ControllerButtonHints.ShowButtonHint(hand, EVRButtonId.k_EButton_Axis0);
				yield return new WaitForSeconds(1f);
				ControllerButtonHints.HideAllButtonHints(hand);
				yield return new WaitForSeconds(1f);
			}
		}

		private IEnumerator TestTextHints(Hand hand)
		{
			ControllerButtonHints.HideAllTextHints(hand);
			while (true)
			{
				ControllerButtonHints.ShowTextHint(hand, EVRButtonId.k_EButton_ApplicationMenu, "Application");
				yield return new WaitForSeconds(3f);
				ControllerButtonHints.ShowTextHint(hand, EVRButtonId.k_EButton_System, "System");
				yield return new WaitForSeconds(3f);
				ControllerButtonHints.ShowTextHint(hand, EVRButtonId.k_EButton_Grip, "Grip");
				yield return new WaitForSeconds(3f);
				ControllerButtonHints.ShowTextHint(hand, EVRButtonId.k_EButton_Axis1, "Trigger");
				yield return new WaitForSeconds(3f);
				ControllerButtonHints.ShowTextHint(hand, EVRButtonId.k_EButton_Axis0, "Touchpad");
				yield return new WaitForSeconds(3f);
				ControllerButtonHints.HideAllTextHints(hand);
				yield return new WaitForSeconds(3f);
			}
		}
	}
}
