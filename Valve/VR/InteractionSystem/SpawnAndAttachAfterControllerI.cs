using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class SpawnAndAttachAfterControllerIsTracking : MonoBehaviour
	{
		private Hand hand;

		public GameObject itemPrefab;

		private void Start()
		{
			hand = GetComponentInParent<Hand>();
		}

		private void Update()
		{
			if (itemPrefab != null && hand.controller != null && hand.controller.hasTracking)
			{
				GameObject gameObject = Object.Instantiate(itemPrefab);
				gameObject.SetActive(value: true);
				hand.AttachObject(gameObject, Hand.AttachmentFlags.SnapOnAttach | Hand.AttachmentFlags.DetachOthers | Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.ParentToHand, string.Empty);
				hand.controller.TriggerHapticPulse(800);
				Object.Destroy(base.gameObject);
				gameObject.transform.localScale = itemPrefab.transform.localScale;
			}
		}
	}
}
