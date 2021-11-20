using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Valve.VR.InteractionSystem
{
	public class ControllerButtonHints : MonoBehaviour
	{
		private enum OffsetType
		{
			Up,
			Right,
			Forward,
			Back
		}

		private class ButtonHintInfo
		{
			public string componentName;

			public List<MeshRenderer> renderers;

			public Transform localTransform;

			public GameObject textHintObject;

			public Transform textStartAnchor;

			public Transform textEndAnchor;

			public Vector3 textEndOffsetDir;

			public Transform canvasOffset;

			public Text text;

			public TextMesh textMesh;

			public Canvas textCanvas;

			public LineRenderer line;

			public float distanceFromCenter;

			public bool textHintActive;
		}

		public Material controllerMaterial;

		public Color flashColor = new Color(1f, 0.557f, 0f);

		public GameObject textHintPrefab;

		[Header("Debug")]
		public bool debugHints;

		private SteamVR_RenderModel renderModel;

		private Player player;

		private List<MeshRenderer> renderers = new List<MeshRenderer>();

		private List<MeshRenderer> flashingRenderers = new List<MeshRenderer>();

		private float startTime;

		private float tickCount;

		private Dictionary<EVRButtonId, ButtonHintInfo> buttonHintInfos;

		private Transform textHintParent;

		private List<KeyValuePair<string, ulong>> componentButtonMasks = new List<KeyValuePair<string, ulong>>();

		private int colorID;

		private Vector3 centerPosition = Vector3.zero;

		private SteamVR_Events.Action renderModelLoadedAction;

		public bool initialized { get; private set; }

		private void Awake()
		{
			renderModelLoadedAction = SteamVR_Events.RenderModelLoadedAction(OnRenderModelLoaded);
			colorID = Shader.PropertyToID("_Color");
		}

		private void Start()
		{
			player = Player.instance;
		}

		private void HintDebugLog(string msg)
		{
			if (debugHints)
			{
				Debug.Log("Hints: " + msg);
			}
		}

		private void OnEnable()
		{
			renderModelLoadedAction.enabled = true;
		}

		private void OnDisable()
		{
			renderModelLoadedAction.enabled = false;
			Clear();
		}

		private void OnParentHandInputFocusLost()
		{
			HideAllButtonHints();
			HideAllText();
		}

		private void OnHandInitialized(int deviceIndex)
		{
			renderModel = new GameObject("SteamVR_RenderModel").AddComponent<SteamVR_RenderModel>();
			renderModel.transform.parent = base.transform;
			renderModel.transform.localPosition = Vector3.zero;
			renderModel.transform.localRotation = Quaternion.identity;
			renderModel.transform.localScale = Vector3.one;
			renderModel.SetDeviceIndex(deviceIndex);
			if (!initialized)
			{
				renderModel.gameObject.SetActive(value: true);
			}
		}

		private void OnRenderModelLoaded(SteamVR_RenderModel renderModel, bool succeess)
		{
			if (!(renderModel == this.renderModel))
			{
				return;
			}
			textHintParent = new GameObject("Text Hints").transform;
			textHintParent.SetParent(base.transform);
			textHintParent.localPosition = Vector3.zero;
			textHintParent.localRotation = Quaternion.identity;
			textHintParent.localScale = Vector3.one;
			using (SteamVR_RenderModel.RenderModelInterfaceHolder renderModelInterfaceHolder = new SteamVR_RenderModel.RenderModelInterfaceHolder())
			{
				CVRRenderModels instance = renderModelInterfaceHolder.instance;
				if (instance != null)
				{
					string text = "Components for render model " + renderModel.index;
					foreach (Transform item in renderModel.transform)
					{
						ulong componentButtonMask = instance.GetComponentButtonMask(renderModel.renderModelName, item.name);
						componentButtonMasks.Add(new KeyValuePair<string, ulong>(item.name, componentButtonMask));
						string text2 = text;
						text = text2 + "\n\t" + item.name + ": " + componentButtonMask;
					}
					HintDebugLog(text);
				}
			}
			buttonHintInfos = new Dictionary<EVRButtonId, ButtonHintInfo>();
			CreateAndAddButtonInfo(EVRButtonId.k_EButton_Axis1);
			CreateAndAddButtonInfo(EVRButtonId.k_EButton_ApplicationMenu);
			CreateAndAddButtonInfo(EVRButtonId.k_EButton_System);
			CreateAndAddButtonInfo(EVRButtonId.k_EButton_Grip);
			CreateAndAddButtonInfo(EVRButtonId.k_EButton_Axis0);
			CreateAndAddButtonInfo(EVRButtonId.k_EButton_A);
			ComputeTextEndTransforms();
			initialized = true;
			renderModel.gameObject.SetActive(value: false);
		}

		private void CreateAndAddButtonInfo(EVRButtonId buttonID)
		{
			Transform transform = null;
			List<MeshRenderer> list = new List<MeshRenderer>();
			string text = "Looking for button: " + buttonID;
			EVRButtonId eVRButtonId = buttonID;
			if (buttonID == EVRButtonId.k_EButton_Grip && SteamVR.instance.hmd_TrackingSystemName.ToLowerInvariant().Contains("oculus"))
			{
				eVRButtonId = EVRButtonId.k_EButton_Axis2;
			}
			ulong num = (ulong)(1L << (int)eVRButtonId);
			string text2;
			foreach (KeyValuePair<string, ulong> componentButtonMask in componentButtonMasks)
			{
				if ((componentButtonMask.Value & num) == num)
				{
					text2 = text;
					text = text2 + "\nFound component: " + componentButtonMask.Key + " " + componentButtonMask.Value;
					Transform transform2 = renderModel.FindComponent(componentButtonMask.Key);
					transform = transform2;
					text2 = text;
					text = string.Concat(text2, "\nFound componentTransform: ", transform2, " buttonTransform: ", transform);
					list.AddRange(transform2.GetComponentsInChildren<MeshRenderer>());
				}
			}
			text2 = text;
			text = text2 + "\nFound " + list.Count + " renderers for " + buttonID;
			foreach (MeshRenderer item in list)
			{
				text = text + "\n\t" + item.name;
			}
			HintDebugLog(text);
			if (transform == null)
			{
				HintDebugLog("Couldn't find buttonTransform for " + buttonID);
				return;
			}
			ButtonHintInfo buttonHintInfo = new ButtonHintInfo();
			buttonHintInfos.Add(buttonID, buttonHintInfo);
			buttonHintInfo.componentName = transform.name;
			buttonHintInfo.renderers = list;
			buttonHintInfo.localTransform = transform.Find("attach");
			OffsetType offsetType = OffsetType.Right;
			switch (buttonID)
			{
			case EVRButtonId.k_EButton_Axis1:
				offsetType = OffsetType.Right;
				break;
			case EVRButtonId.k_EButton_ApplicationMenu:
				offsetType = OffsetType.Right;
				break;
			case EVRButtonId.k_EButton_System:
				offsetType = OffsetType.Right;
				break;
			case EVRButtonId.k_EButton_Grip:
				offsetType = OffsetType.Forward;
				break;
			case EVRButtonId.k_EButton_Axis0:
				offsetType = OffsetType.Up;
				break;
			}
			switch (offsetType)
			{
			case OffsetType.Forward:
				buttonHintInfo.textEndOffsetDir = buttonHintInfo.localTransform.forward;
				break;
			case OffsetType.Back:
				buttonHintInfo.textEndOffsetDir = -buttonHintInfo.localTransform.forward;
				break;
			case OffsetType.Right:
				buttonHintInfo.textEndOffsetDir = buttonHintInfo.localTransform.right;
				break;
			case OffsetType.Up:
				buttonHintInfo.textEndOffsetDir = buttonHintInfo.localTransform.up;
				break;
			}
			Vector3 position = buttonHintInfo.localTransform.position + buttonHintInfo.localTransform.forward * 0.01f;
			buttonHintInfo.textHintObject = UnityEngine.Object.Instantiate(textHintPrefab, position, Quaternion.identity);
			buttonHintInfo.textHintObject.name = "Hint_" + buttonHintInfo.componentName + "_Start";
			buttonHintInfo.textHintObject.transform.SetParent(textHintParent);
			buttonHintInfo.textStartAnchor = buttonHintInfo.textHintObject.transform.Find("Start");
			buttonHintInfo.textEndAnchor = buttonHintInfo.textHintObject.transform.Find("End");
			buttonHintInfo.canvasOffset = buttonHintInfo.textHintObject.transform.Find("CanvasOffset");
			buttonHintInfo.line = buttonHintInfo.textHintObject.transform.Find("Line").GetComponent<LineRenderer>();
			buttonHintInfo.textCanvas = buttonHintInfo.textHintObject.GetComponentInChildren<Canvas>();
			buttonHintInfo.text = buttonHintInfo.textCanvas.GetComponentInChildren<Text>();
			buttonHintInfo.textMesh = buttonHintInfo.textCanvas.GetComponentInChildren<TextMesh>();
			buttonHintInfo.textHintObject.SetActive(value: false);
			buttonHintInfo.textStartAnchor.position = position;
			if (buttonHintInfo.text != null)
			{
				buttonHintInfo.text.text = buttonHintInfo.componentName;
			}
			if (buttonHintInfo.textMesh != null)
			{
				buttonHintInfo.textMesh.text = buttonHintInfo.componentName;
			}
			centerPosition += buttonHintInfo.textStartAnchor.position;
			buttonHintInfo.textCanvas.transform.localScale = Vector3.Scale(buttonHintInfo.textCanvas.transform.localScale, player.transform.localScale);
			buttonHintInfo.textStartAnchor.transform.localScale = Vector3.Scale(buttonHintInfo.textStartAnchor.transform.localScale, player.transform.localScale);
			buttonHintInfo.textEndAnchor.transform.localScale = Vector3.Scale(buttonHintInfo.textEndAnchor.transform.localScale, player.transform.localScale);
			buttonHintInfo.line.transform.localScale = Vector3.Scale(buttonHintInfo.line.transform.localScale, player.transform.localScale);
		}

		private void ComputeTextEndTransforms()
		{
			centerPosition /= (float)buttonHintInfos.Count;
			float num = 0f;
			foreach (KeyValuePair<EVRButtonId, ButtonHintInfo> buttonHintInfo in buttonHintInfos)
			{
				buttonHintInfo.Value.distanceFromCenter = Vector3.Distance(buttonHintInfo.Value.textStartAnchor.position, centerPosition);
				if (buttonHintInfo.Value.distanceFromCenter > num)
				{
					num = buttonHintInfo.Value.distanceFromCenter;
				}
			}
			foreach (KeyValuePair<EVRButtonId, ButtonHintInfo> buttonHintInfo2 in buttonHintInfos)
			{
				Vector3 vector = buttonHintInfo2.Value.textStartAnchor.position - centerPosition;
				vector.Normalize();
				vector = Vector3.Project(vector, renderModel.transform.forward);
				float num2 = buttonHintInfo2.Value.distanceFromCenter / num;
				float num3 = buttonHintInfo2.Value.distanceFromCenter * Mathf.Pow(2f, 10f * (num2 - 1f)) * 20f;
				float num4 = 0.1f;
				Vector3 position = buttonHintInfo2.Value.textStartAnchor.position + buttonHintInfo2.Value.textEndOffsetDir * num4 + vector * num3 * 0.1f;
				buttonHintInfo2.Value.textEndAnchor.position = position;
				buttonHintInfo2.Value.canvasOffset.position = position;
				buttonHintInfo2.Value.canvasOffset.localRotation = Quaternion.identity;
			}
		}

		private void ShowButtonHint(params EVRButtonId[] buttons)
		{
			renderModel.gameObject.SetActive(value: true);
			renderModel.GetComponentsInChildren(renderers);
			for (int i = 0; i < renderers.Count; i++)
			{
				Texture mainTexture = renderers[i].material.mainTexture;
				renderers[i].sharedMaterial = controllerMaterial;
				renderers[i].material.mainTexture = mainTexture;
				renderers[i].material.renderQueue = controllerMaterial.shader.renderQueue;
			}
			for (int j = 0; j < buttons.Length; j++)
			{
				if (!buttonHintInfos.ContainsKey(buttons[j]))
				{
					continue;
				}
				ButtonHintInfo buttonHintInfo = buttonHintInfos[buttons[j]];
				foreach (MeshRenderer renderer in buttonHintInfo.renderers)
				{
					if (!flashingRenderers.Contains(renderer))
					{
						flashingRenderers.Add(renderer);
					}
				}
			}
			startTime = Time.realtimeSinceStartup;
			tickCount = 0f;
		}

		private void HideAllButtonHints()
		{
			Clear();
			renderModel.gameObject.SetActive(value: false);
		}

		private void HideButtonHint(params EVRButtonId[] buttons)
		{
			Color color = controllerMaterial.GetColor(colorID);
			for (int i = 0; i < buttons.Length; i++)
			{
				if (!buttonHintInfos.ContainsKey(buttons[i]))
				{
					continue;
				}
				ButtonHintInfo buttonHintInfo = buttonHintInfos[buttons[i]];
				foreach (MeshRenderer renderer in buttonHintInfo.renderers)
				{
					renderer.material.color = color;
					flashingRenderers.Remove(renderer);
				}
			}
			if (flashingRenderers.Count == 0)
			{
				renderModel.gameObject.SetActive(value: false);
			}
		}

		private bool IsButtonHintActive(EVRButtonId button)
		{
			if (buttonHintInfos.ContainsKey(button))
			{
				ButtonHintInfo buttonHintInfo = buttonHintInfos[button];
				foreach (MeshRenderer renderer in buttonHintInfo.renderers)
				{
					if (flashingRenderers.Contains(renderer))
					{
						return true;
					}
				}
			}
			return false;
		}

		private IEnumerator TestButtonHints()
		{
			while (true)
			{
				ShowButtonHint(EVRButtonId.k_EButton_Axis1);
				yield return new WaitForSeconds(1f);
				ShowButtonHint(EVRButtonId.k_EButton_ApplicationMenu);
				yield return new WaitForSeconds(1f);
				ShowButtonHint(default(EVRButtonId));
				yield return new WaitForSeconds(1f);
				ShowButtonHint(EVRButtonId.k_EButton_Grip);
				yield return new WaitForSeconds(1f);
				ShowButtonHint(EVRButtonId.k_EButton_Axis0);
				yield return new WaitForSeconds(1f);
			}
		}

		private IEnumerator TestTextHints()
		{
			while (true)
			{
				ShowText(EVRButtonId.k_EButton_Axis1, "Trigger");
				yield return new WaitForSeconds(3f);
				ShowText(EVRButtonId.k_EButton_ApplicationMenu, "Application");
				yield return new WaitForSeconds(3f);
				ShowText(EVRButtonId.k_EButton_System, "System");
				yield return new WaitForSeconds(3f);
				ShowText(EVRButtonId.k_EButton_Grip, "Grip");
				yield return new WaitForSeconds(3f);
				ShowText(EVRButtonId.k_EButton_Axis0, "Touchpad");
				yield return new WaitForSeconds(3f);
				HideAllText();
				yield return new WaitForSeconds(3f);
			}
		}

		private void Update()
		{
			if (!(renderModel != null) || !renderModel.gameObject.activeInHierarchy || flashingRenderers.Count <= 0)
			{
				return;
			}
			Color color = controllerMaterial.GetColor(colorID);
			float f = (Time.realtimeSinceStartup - startTime) * (float)Math.PI * 2f;
			f = Mathf.Cos(f);
			f = Util.RemapNumberClamped(f, -1f, 1f, 0f, 1f);
			float num = Time.realtimeSinceStartup - startTime;
			if (num - tickCount > 1f)
			{
				tickCount += 1f;
				SteamVR_Controller.Input((int)renderModel.index)?.TriggerHapticPulse(500);
			}
			for (int i = 0; i < flashingRenderers.Count; i++)
			{
				Renderer renderer = flashingRenderers[i];
				renderer.material.SetColor(colorID, Color.Lerp(color, flashColor, f));
			}
			if (!initialized)
			{
				return;
			}
			foreach (KeyValuePair<EVRButtonId, ButtonHintInfo> buttonHintInfo in buttonHintInfos)
			{
				if (buttonHintInfo.Value.textHintActive)
				{
					UpdateTextHint(buttonHintInfo.Value);
				}
			}
		}

		private void UpdateTextHint(ButtonHintInfo hintInfo)
		{
			Transform hmdTransform = player.hmdTransform;
			Vector3 forward = hmdTransform.position - hintInfo.canvasOffset.position;
			Quaternion a = Quaternion.LookRotation(forward, Vector3.up);
			Quaternion b = Quaternion.LookRotation(forward, hmdTransform.up);
			float t = ((!(hmdTransform.forward.y > 0f)) ? Util.RemapNumberClamped(hmdTransform.forward.y, -0.8f, -0.6f, 1f, 0f) : Util.RemapNumberClamped(hmdTransform.forward.y, 0.6f, 0.4f, 1f, 0f));
			hintInfo.canvasOffset.rotation = Quaternion.Slerp(a, b, t);
			Transform transform = hintInfo.line.transform;
			hintInfo.line.useWorldSpace = false;
			hintInfo.line.SetPosition(0, transform.InverseTransformPoint(hintInfo.textStartAnchor.position));
			hintInfo.line.SetPosition(1, transform.InverseTransformPoint(hintInfo.textEndAnchor.position));
		}

		private void Clear()
		{
			renderers.Clear();
			flashingRenderers.Clear();
		}

		private void ShowText(EVRButtonId button, string text, bool highlightButton = true)
		{
			if (buttonHintInfos.ContainsKey(button))
			{
				ButtonHintInfo buttonHintInfo = buttonHintInfos[button];
				buttonHintInfo.textHintObject.SetActive(value: true);
				buttonHintInfo.textHintActive = true;
				if (buttonHintInfo.text != null)
				{
					buttonHintInfo.text.text = text;
				}
				if (buttonHintInfo.textMesh != null)
				{
					buttonHintInfo.textMesh.text = text;
				}
				UpdateTextHint(buttonHintInfo);
				if (highlightButton)
				{
					ShowButtonHint(button);
				}
				renderModel.gameObject.SetActive(value: true);
			}
		}

		private void HideText(EVRButtonId button)
		{
			if (buttonHintInfos.ContainsKey(button))
			{
				ButtonHintInfo buttonHintInfo = buttonHintInfos[button];
				buttonHintInfo.textHintObject.SetActive(value: false);
				buttonHintInfo.textHintActive = false;
				HideButtonHint(button);
			}
		}

		private void HideAllText()
		{
			foreach (KeyValuePair<EVRButtonId, ButtonHintInfo> buttonHintInfo in buttonHintInfos)
			{
				buttonHintInfo.Value.textHintObject.SetActive(value: false);
				buttonHintInfo.Value.textHintActive = false;
			}
			HideAllButtonHints();
		}

		private string GetActiveHintText(EVRButtonId button)
		{
			if (buttonHintInfos.ContainsKey(button))
			{
				ButtonHintInfo buttonHintInfo = buttonHintInfos[button];
				if (buttonHintInfo.textHintActive)
				{
					return buttonHintInfo.text.text;
				}
			}
			return string.Empty;
		}

		private static ControllerButtonHints GetControllerButtonHints(Hand hand)
		{
			if (hand != null)
			{
				ControllerButtonHints componentInChildren = hand.GetComponentInChildren<ControllerButtonHints>();
				if (componentInChildren != null && componentInChildren.initialized)
				{
					return componentInChildren;
				}
			}
			return null;
		}

		public static void ShowButtonHint(Hand hand, params EVRButtonId[] buttons)
		{
			ControllerButtonHints controllerButtonHints = GetControllerButtonHints(hand);
			if (controllerButtonHints != null)
			{
				controllerButtonHints.ShowButtonHint(buttons);
			}
		}

		public static void HideButtonHint(Hand hand, params EVRButtonId[] buttons)
		{
			ControllerButtonHints controllerButtonHints = GetControllerButtonHints(hand);
			if (controllerButtonHints != null)
			{
				controllerButtonHints.HideButtonHint(buttons);
			}
		}

		public static void HideAllButtonHints(Hand hand)
		{
			ControllerButtonHints controllerButtonHints = GetControllerButtonHints(hand);
			if (controllerButtonHints != null)
			{
				controllerButtonHints.HideAllButtonHints();
			}
		}

		public static bool IsButtonHintActive(Hand hand, EVRButtonId button)
		{
			ControllerButtonHints controllerButtonHints = GetControllerButtonHints(hand);
			if (controllerButtonHints != null)
			{
				return controllerButtonHints.IsButtonHintActive(button);
			}
			return false;
		}

		public static void ShowTextHint(Hand hand, EVRButtonId button, string text, bool highlightButton = true)
		{
			ControllerButtonHints controllerButtonHints = GetControllerButtonHints(hand);
			if (controllerButtonHints != null)
			{
				controllerButtonHints.ShowText(button, text, highlightButton);
			}
		}

		public static void HideTextHint(Hand hand, EVRButtonId button)
		{
			ControllerButtonHints controllerButtonHints = GetControllerButtonHints(hand);
			if (controllerButtonHints != null)
			{
				controllerButtonHints.HideText(button);
			}
		}

		public static void HideAllTextHints(Hand hand)
		{
			ControllerButtonHints controllerButtonHints = GetControllerButtonHints(hand);
			if (controllerButtonHints != null)
			{
				controllerButtonHints.HideAllText();
			}
		}

		public static string GetActiveHintText(Hand hand, EVRButtonId button)
		{
			ControllerButtonHints controllerButtonHints = GetControllerButtonHints(hand);
			if (controllerButtonHints != null)
			{
				return controllerButtonHints.GetActiveHintText(button);
			}
			return string.Empty;
		}
	}
}
