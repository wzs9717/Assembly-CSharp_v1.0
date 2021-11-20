using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class SpawnRenderModel : MonoBehaviour
	{
		public Material[] materials;

		private SteamVR_RenderModel[] renderModels;

		private Hand hand;

		private List<MeshRenderer> renderers = new List<MeshRenderer>();

		private static List<SpawnRenderModel> spawnRenderModels = new List<SpawnRenderModel>();

		private static int lastFrameUpdated;

		private static int spawnRenderModelUpdateIndex;

		private SteamVR_Events.Action renderModelLoadedAction;

		private void Awake()
		{
			renderModels = new SteamVR_RenderModel[materials.Length];
			renderModelLoadedAction = SteamVR_Events.RenderModelLoadedAction(OnRenderModelLoaded);
		}

		private void OnEnable()
		{
			ShowController();
			renderModelLoadedAction.enabled = true;
			spawnRenderModels.Add(this);
		}

		private void OnDisable()
		{
			HideController();
			renderModelLoadedAction.enabled = false;
			spawnRenderModels.Remove(this);
		}

		private void OnAttachedToHand(Hand hand)
		{
			this.hand = hand;
			ShowController();
		}

		private void OnDetachedFromHand(Hand hand)
		{
			this.hand = null;
			HideController();
		}

		private void Update()
		{
			if (lastFrameUpdated == Time.renderedFrameCount)
			{
				return;
			}
			lastFrameUpdated = Time.renderedFrameCount;
			if (spawnRenderModelUpdateIndex >= spawnRenderModels.Count)
			{
				spawnRenderModelUpdateIndex = 0;
			}
			if (spawnRenderModelUpdateIndex < spawnRenderModels.Count)
			{
				SteamVR_RenderModel steamVR_RenderModel = spawnRenderModels[spawnRenderModelUpdateIndex].renderModels[0];
				if (steamVR_RenderModel != null)
				{
					steamVR_RenderModel.UpdateComponents(OpenVR.RenderModels);
				}
			}
			spawnRenderModelUpdateIndex++;
		}

		private void ShowController()
		{
			if (hand == null || hand.controller == null)
			{
				return;
			}
			for (int i = 0; i < renderModels.Length; i++)
			{
				if (renderModels[i] == null)
				{
					renderModels[i] = new GameObject("SteamVR_RenderModel").AddComponent<SteamVR_RenderModel>();
					renderModels[i].updateDynamically = false;
					renderModels[i].transform.parent = base.transform;
					Util.ResetTransform(renderModels[i].transform);
				}
				renderModels[i].gameObject.SetActive(value: true);
				renderModels[i].SetDeviceIndex((int)hand.controller.index);
			}
		}

		private void HideController()
		{
			for (int i = 0; i < renderModels.Length; i++)
			{
				if (renderModels[i] != null)
				{
					renderModels[i].gameObject.SetActive(value: false);
				}
			}
		}

		private void OnRenderModelLoaded(SteamVR_RenderModel renderModel, bool success)
		{
			for (int i = 0; i < renderModels.Length; i++)
			{
				if (renderModel == renderModels[i] && materials[i] != null)
				{
					renderers.Clear();
					renderModels[i].GetComponentsInChildren(renderers);
					for (int j = 0; j < renderers.Count; j++)
					{
						Texture mainTexture = renderers[j].material.mainTexture;
						renderers[j].sharedMaterial = materials[i];
						renderers[j].material.mainTexture = mainTexture;
					}
				}
			}
		}
	}
}
