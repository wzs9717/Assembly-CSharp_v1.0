using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class ControllerHoverHighlight : MonoBehaviour
	{
		public Material highLightMaterial;

		public bool fireHapticsOnHightlight = true;

		private Hand hand;

		private MeshRenderer bodyMeshRenderer;

		private MeshRenderer trackingHatMeshRenderer;

		private SteamVR_RenderModel renderModel;

		private bool renderModelLoaded;

		private SteamVR_Events.Action renderModelLoadedAction;

		private void Start()
		{
			hand = GetComponentInParent<Hand>();
		}

		private void Awake()
		{
			renderModelLoadedAction = SteamVR_Events.RenderModelLoadedAction(OnRenderModelLoaded);
		}

		private void OnEnable()
		{
			renderModelLoadedAction.enabled = true;
		}

		private void OnDisable()
		{
			renderModelLoadedAction.enabled = false;
		}

		private void OnHandInitialized(int deviceIndex)
		{
			renderModel = base.gameObject.AddComponent<SteamVR_RenderModel>();
			renderModel.SetDeviceIndex(deviceIndex);
			renderModel.updateDynamically = false;
		}

		private void OnRenderModelLoaded(SteamVR_RenderModel renderModel, bool success)
		{
			if (renderModel != this.renderModel)
			{
				return;
			}
			Transform transform = base.transform.Find("body");
			if (transform != null)
			{
				bodyMeshRenderer = transform.GetComponent<MeshRenderer>();
				bodyMeshRenderer.material = highLightMaterial;
				bodyMeshRenderer.enabled = false;
			}
			Transform transform2 = base.transform.Find("trackhat");
			if (transform2 != null)
			{
				trackingHatMeshRenderer = transform2.GetComponent<MeshRenderer>();
				trackingHatMeshRenderer.material = highLightMaterial;
				trackingHatMeshRenderer.enabled = false;
			}
			foreach (Transform item in base.transform)
			{
				if (item.name != "body" && item.name != "trackhat")
				{
					Object.Destroy(item.gameObject);
				}
			}
			renderModelLoaded = true;
		}

		private void OnParentHandHoverBegin(Interactable other)
		{
			if (base.isActiveAndEnabled && other.transform.parent != base.transform.parent)
			{
				ShowHighlight();
			}
		}

		private void OnParentHandHoverEnd(Interactable other)
		{
			HideHighlight();
		}

		private void OnParentHandInputFocusAcquired()
		{
			if (base.isActiveAndEnabled && (bool)hand.hoveringInteractable && hand.hoveringInteractable.transform.parent != base.transform.parent)
			{
				ShowHighlight();
			}
		}

		private void OnParentHandInputFocusLost()
		{
			HideHighlight();
		}

		public void ShowHighlight()
		{
			if (renderModelLoaded)
			{
				if (fireHapticsOnHightlight)
				{
					hand.controller.TriggerHapticPulse(500);
				}
				if (bodyMeshRenderer != null)
				{
					bodyMeshRenderer.enabled = true;
				}
				if (trackingHatMeshRenderer != null)
				{
					trackingHatMeshRenderer.enabled = true;
				}
			}
		}

		public void HideHighlight()
		{
			if (renderModelLoaded)
			{
				if (fireHapticsOnHightlight)
				{
					hand.controller.TriggerHapticPulse(300);
				}
				if (bodyMeshRenderer != null)
				{
					bodyMeshRenderer.enabled = false;
				}
				if (trackingHatMeshRenderer != null)
				{
					trackingHatMeshRenderer.enabled = false;
				}
			}
		}
	}
}
