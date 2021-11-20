using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class Hand : MonoBehaviour
	{
		public enum HandType
		{
			Left,
			Right,
			Any
		}

		[Flags]
		public enum AttachmentFlags
		{
			SnapOnAttach = 0x1,
			DetachOthers = 0x2,
			DetachFromOtherHand = 0x4,
			ParentToHand = 0x8
		}

		public struct AttachedObject
		{
			public GameObject attachedObject;

			public GameObject originalParent;

			public bool isParentedToHand;
		}

		public const AttachmentFlags defaultAttachmentFlags = AttachmentFlags.SnapOnAttach | AttachmentFlags.DetachOthers | AttachmentFlags.DetachFromOtherHand | AttachmentFlags.ParentToHand;

		public Hand otherHand;

		public HandType startingHandType;

		public Transform hoverSphereTransform;

		public float hoverSphereRadius = 0.05f;

		public LayerMask hoverLayerMask = -1;

		public float hoverUpdateInterval = 0.1f;

		public Camera noSteamVRFallbackCamera;

		public float noSteamVRFallbackMaxDistanceNoItem = 10f;

		public float noSteamVRFallbackMaxDistanceWithItem = 0.5f;

		private float noSteamVRFallbackInteractorDistance = -1f;

		public SteamVR_Controller.Device controller;

		public GameObject controllerPrefab;

		private GameObject controllerObject;

		public bool showDebugText;

		public bool spewDebugText;

		private List<AttachedObject> attachedObjects = new List<AttachedObject>();

		private Interactable _hoveringInteractable;

		private TextMesh debugText;

		private int prevOverlappingColliders;

		private const int ColliderArraySize = 16;

		private Collider[] overlappingColliders;

		private Player playerInstance;

		private GameObject applicationLostFocusObject;

		private SteamVR_Events.Action inputFocusAction;

		public ReadOnlyCollection<AttachedObject> AttachedObjects => attachedObjects.AsReadOnly();

		public bool hoverLocked { get; private set; }

		public Interactable hoveringInteractable
		{
			get
			{
				return _hoveringInteractable;
			}
			set
			{
				if (!(_hoveringInteractable != value))
				{
					return;
				}
				if (_hoveringInteractable != null)
				{
					HandDebugLog("HoverEnd " + _hoveringInteractable.gameObject);
					_hoveringInteractable.SendMessage("OnHandHoverEnd", this, SendMessageOptions.DontRequireReceiver);
					if (_hoveringInteractable != null)
					{
						BroadcastMessage("OnParentHandHoverEnd", _hoveringInteractable, SendMessageOptions.DontRequireReceiver);
					}
				}
				_hoveringInteractable = value;
				if (_hoveringInteractable != null)
				{
					HandDebugLog("HoverBegin " + _hoveringInteractable.gameObject);
					_hoveringInteractable.SendMessage("OnHandHoverBegin", this, SendMessageOptions.DontRequireReceiver);
					if (_hoveringInteractable != null)
					{
						BroadcastMessage("OnParentHandHoverBegin", _hoveringInteractable, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
		}

		public GameObject currentAttachedObject
		{
			get
			{
				CleanUpAttachedObjectStack();
				if (attachedObjects.Count > 0)
				{
					return attachedObjects[attachedObjects.Count - 1].attachedObject;
				}
				return null;
			}
		}

		public Transform GetAttachmentTransform(string attachmentPoint = "")
		{
			Transform transform = null;
			if (!string.IsNullOrEmpty(attachmentPoint))
			{
				transform = base.transform.Find(attachmentPoint);
			}
			if (!transform)
			{
				transform = base.transform;
			}
			return transform;
		}

		public HandType GuessCurrentHandType()
		{
			if (startingHandType == HandType.Left || startingHandType == HandType.Right)
			{
				return startingHandType;
			}
			if (startingHandType == HandType.Any && otherHand != null && otherHand.controller == null)
			{
				return HandType.Right;
			}
			if (controller == null || otherHand == null || otherHand.controller == null)
			{
				return startingHandType;
			}
			if (controller.index == SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost))
			{
				return HandType.Left;
			}
			return HandType.Right;
		}

		public void AttachObject(GameObject objectToAttach, AttachmentFlags flags = AttachmentFlags.SnapOnAttach | AttachmentFlags.DetachOthers | AttachmentFlags.DetachFromOtherHand | AttachmentFlags.ParentToHand, string attachmentPoint = "")
		{
			if (flags == (AttachmentFlags)0)
			{
				flags = AttachmentFlags.SnapOnAttach | AttachmentFlags.DetachOthers | AttachmentFlags.DetachFromOtherHand | AttachmentFlags.ParentToHand;
			}
			CleanUpAttachedObjectStack();
			DetachObject(objectToAttach);
			if ((flags & AttachmentFlags.DetachFromOtherHand) == AttachmentFlags.DetachFromOtherHand && (bool)otherHand)
			{
				otherHand.DetachObject(objectToAttach);
			}
			if ((flags & AttachmentFlags.DetachOthers) == AttachmentFlags.DetachOthers)
			{
				while (attachedObjects.Count > 0)
				{
					DetachObject(attachedObjects[0].attachedObject);
				}
			}
			if ((bool)currentAttachedObject)
			{
				currentAttachedObject.SendMessage("OnHandFocusLost", this, SendMessageOptions.DontRequireReceiver);
			}
			AttachedObject item = default(AttachedObject);
			item.attachedObject = objectToAttach;
			item.originalParent = ((!(objectToAttach.transform.parent != null)) ? null : objectToAttach.transform.parent.gameObject);
			if ((flags & AttachmentFlags.ParentToHand) == AttachmentFlags.ParentToHand)
			{
				objectToAttach.transform.parent = GetAttachmentTransform(attachmentPoint);
				item.isParentedToHand = true;
			}
			else
			{
				item.isParentedToHand = false;
			}
			attachedObjects.Add(item);
			if ((flags & AttachmentFlags.SnapOnAttach) == AttachmentFlags.SnapOnAttach)
			{
				objectToAttach.transform.localPosition = Vector3.zero;
				objectToAttach.transform.localRotation = Quaternion.identity;
			}
			HandDebugLog("AttachObject " + objectToAttach);
			objectToAttach.SendMessage("OnAttachedToHand", this, SendMessageOptions.DontRequireReceiver);
			UpdateHovering();
		}

		public void DetachObject(GameObject objectToDetach, bool restoreOriginalParent = true)
		{
			int num = attachedObjects.FindIndex((AttachedObject l) => l.attachedObject == objectToDetach);
			if (num != -1)
			{
				HandDebugLog("DetachObject " + objectToDetach);
				GameObject gameObject = currentAttachedObject;
				Transform parent = null;
				if (attachedObjects[num].isParentedToHand)
				{
					if (restoreOriginalParent && attachedObjects[num].originalParent != null)
					{
						parent = attachedObjects[num].originalParent.transform;
					}
					attachedObjects[num].attachedObject.transform.parent = parent;
				}
				attachedObjects[num].attachedObject.SetActive(value: true);
				attachedObjects[num].attachedObject.SendMessage("OnDetachedFromHand", this, SendMessageOptions.DontRequireReceiver);
				attachedObjects.RemoveAt(num);
				GameObject gameObject2 = currentAttachedObject;
				if (gameObject2 != null && gameObject2 != gameObject)
				{
					gameObject2.SetActive(value: true);
					gameObject2.SendMessage("OnHandFocusAcquired", this, SendMessageOptions.DontRequireReceiver);
				}
			}
			CleanUpAttachedObjectStack();
		}

		public Vector3 GetTrackedObjectVelocity()
		{
			if (controller != null)
			{
				return base.transform.parent.TransformVector(controller.velocity);
			}
			return Vector3.zero;
		}

		public Vector3 GetTrackedObjectAngularVelocity()
		{
			if (controller != null)
			{
				return base.transform.parent.TransformVector(controller.angularVelocity);
			}
			return Vector3.zero;
		}

		private void CleanUpAttachedObjectStack()
		{
			attachedObjects.RemoveAll((AttachedObject l) => l.attachedObject == null);
		}

		private void Awake()
		{
			inputFocusAction = SteamVR_Events.InputFocusAction(OnInputFocus);
			if (hoverSphereTransform == null)
			{
				hoverSphereTransform = base.transform;
			}
			applicationLostFocusObject = new GameObject("_application_lost_focus");
			applicationLostFocusObject.transform.parent = base.transform;
			applicationLostFocusObject.SetActive(value: false);
		}

		private IEnumerator Start()
		{
			playerInstance = Player.instance;
			if (!playerInstance)
			{
				Debug.LogError("No player instance found in Hand Start()");
			}
			overlappingColliders = new Collider[16];
			if ((bool)noSteamVRFallbackCamera)
			{
				yield break;
			}
			while (true)
			{
				yield return new WaitForSeconds(1f);
				if (controller != null)
				{
					break;
				}
				if (startingHandType == HandType.Left || startingHandType == HandType.Right)
				{
					int deviceIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
					int deviceIndex2 = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
					if (deviceIndex != -1 && deviceIndex2 != -1 && deviceIndex != deviceIndex2)
					{
						int index = ((startingHandType != HandType.Right) ? deviceIndex : deviceIndex2);
						int index2 = ((startingHandType != HandType.Right) ? deviceIndex2 : deviceIndex);
						InitController(index);
						if ((bool)otherHand)
						{
							otherHand.InitController(index2);
						}
					}
					continue;
				}
				SteamVR instance = SteamVR.instance;
				for (int i = 0; (long)i < 16L; i++)
				{
					if (instance.hmd.GetTrackedDeviceClass((uint)i) == ETrackedDeviceClass.Controller)
					{
						SteamVR_Controller.Device device = SteamVR_Controller.Input(i);
						if (device.valid && (!(otherHand != null) || otherHand.controller == null || i != (int)otherHand.controller.index))
						{
							InitController(i);
						}
					}
				}
			}
		}

		private void UpdateHovering()
		{
			if ((noSteamVRFallbackCamera == null && controller == null) || hoverLocked || applicationLostFocusObject.activeSelf)
			{
				return;
			}
			float num = float.MaxValue;
			Interactable interactable = null;
			float x = playerInstance.transform.lossyScale.x;
			float num2 = hoverSphereRadius * x;
			float num3 = Mathf.Abs(base.transform.position.y - playerInstance.trackingOriginTransform.position.y);
			float num4 = Util.RemapNumberClamped(num3, 0f, 0.5f * x, 5f, 1f) * x;
			for (int i = 0; i < overlappingColliders.Length; i++)
			{
				overlappingColliders[i] = null;
			}
			Physics.OverlapBoxNonAlloc(hoverSphereTransform.position - new Vector3(0f, num2 * num4 - num2, 0f), new Vector3(num2, num2 * num4 * 2f, num2), overlappingColliders, Quaternion.identity, hoverLayerMask.value);
			int num5 = 0;
			Collider[] array = overlappingColliders;
			foreach (Collider collider in array)
			{
				if (collider == null)
				{
					continue;
				}
				Interactable contacting = collider.GetComponentInParent<Interactable>();
				if (contacting == null)
				{
					continue;
				}
				IgnoreHovering component = collider.GetComponent<IgnoreHovering>();
				if ((!(component != null) || (!(component.onlyIgnoreHand == null) && !(component.onlyIgnoreHand == this))) && attachedObjects.FindIndex((AttachedObject l) => l.attachedObject == contacting.gameObject) == -1 && (!otherHand || !(otherHand.hoveringInteractable == contacting)))
				{
					float num6 = Vector3.Distance(contacting.transform.position, hoverSphereTransform.position);
					if (num6 < num)
					{
						num = num6;
						interactable = contacting;
					}
					num5++;
				}
			}
			hoveringInteractable = interactable;
			if (num5 > 0 && num5 != prevOverlappingColliders)
			{
				prevOverlappingColliders = num5;
				HandDebugLog("Found " + num5 + " overlapping colliders.");
			}
		}

		private void UpdateNoSteamVRFallback()
		{
			if (!noSteamVRFallbackCamera)
			{
				return;
			}
			Ray ray = noSteamVRFallbackCamera.ScreenPointToRay(Input.mousePosition);
			if (attachedObjects.Count > 0)
			{
				base.transform.position = ray.origin + noSteamVRFallbackInteractorDistance * ray.direction;
				return;
			}
			Vector3 position = base.transform.position;
			base.transform.position = noSteamVRFallbackCamera.transform.forward * -1000f;
			if (Physics.Raycast(ray, out var hitInfo, noSteamVRFallbackMaxDistanceNoItem))
			{
				base.transform.position = hitInfo.point;
				noSteamVRFallbackInteractorDistance = Mathf.Min(noSteamVRFallbackMaxDistanceNoItem, hitInfo.distance);
			}
			else if (noSteamVRFallbackInteractorDistance > 0f)
			{
				base.transform.position = ray.origin + Mathf.Min(noSteamVRFallbackMaxDistanceNoItem, noSteamVRFallbackInteractorDistance) * ray.direction;
			}
			else
			{
				base.transform.position = position;
			}
		}

		private void UpdateDebugText()
		{
			if (showDebugText)
			{
				if (debugText == null)
				{
					debugText = new GameObject("_debug_text").AddComponent<TextMesh>();
					debugText.fontSize = 120;
					debugText.characterSize = 0.001f;
					debugText.transform.parent = base.transform;
					debugText.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
				}
				if (GuessCurrentHandType() == HandType.Right)
				{
					debugText.transform.localPosition = new Vector3(-0.05f, 0f, 0f);
					debugText.alignment = TextAlignment.Right;
					debugText.anchor = TextAnchor.UpperRight;
				}
				else
				{
					debugText.transform.localPosition = new Vector3(0.05f, 0f, 0f);
					debugText.alignment = TextAlignment.Left;
					debugText.anchor = TextAnchor.UpperLeft;
				}
				debugText.text = string.Format("Hovering: {0}\nHover Lock: {1}\nAttached: {2}\nTotal Attached: {3}\nType: {4}\n", (!hoveringInteractable) ? "null" : hoveringInteractable.gameObject.name, hoverLocked, (!currentAttachedObject) ? "null" : currentAttachedObject.name, attachedObjects.Count, GuessCurrentHandType().ToString());
			}
			else if (debugText != null)
			{
				UnityEngine.Object.Destroy(debugText.gameObject);
			}
		}

		private void OnEnable()
		{
			inputFocusAction.enabled = true;
			float time = ((!(otherHand != null) || otherHand.GetInstanceID() >= GetInstanceID()) ? 0f : (0.5f * hoverUpdateInterval));
			InvokeRepeating("UpdateHovering", time, hoverUpdateInterval);
			InvokeRepeating("UpdateDebugText", time, hoverUpdateInterval);
		}

		private void OnDisable()
		{
			inputFocusAction.enabled = false;
			CancelInvoke();
		}

		private void Update()
		{
			UpdateNoSteamVRFallback();
			GameObject gameObject = currentAttachedObject;
			if ((bool)gameObject)
			{
				gameObject.SendMessage("HandAttachedUpdate", this, SendMessageOptions.DontRequireReceiver);
			}
			if ((bool)hoveringInteractable)
			{
				hoveringInteractable.SendMessage("HandHoverUpdate", this, SendMessageOptions.DontRequireReceiver);
			}
		}

		private void LateUpdate()
		{
			if (controllerObject != null && attachedObjects.Count == 0)
			{
				AttachObject(controllerObject, AttachmentFlags.SnapOnAttach | AttachmentFlags.DetachOthers | AttachmentFlags.DetachFromOtherHand | AttachmentFlags.ParentToHand, string.Empty);
			}
		}

		private void OnInputFocus(bool hasFocus)
		{
			if (hasFocus)
			{
				DetachObject(applicationLostFocusObject);
				applicationLostFocusObject.SetActive(value: false);
				UpdateHandPoses();
				UpdateHovering();
				BroadcastMessage("OnParentHandInputFocusAcquired", SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				applicationLostFocusObject.SetActive(value: true);
				AttachObject(applicationLostFocusObject, AttachmentFlags.ParentToHand, string.Empty);
				BroadcastMessage("OnParentHandInputFocusLost", SendMessageOptions.DontRequireReceiver);
			}
		}

		private void FixedUpdate()
		{
			UpdateHandPoses();
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = new Color(0.5f, 1f, 0.5f, 0.9f);
			Transform transform = ((!hoverSphereTransform) ? base.transform : hoverSphereTransform);
			Gizmos.DrawWireSphere(transform.position, hoverSphereRadius);
		}

		private void HandDebugLog(string msg)
		{
			if (spewDebugText)
			{
				Debug.Log("Hand (" + base.name + "): " + msg);
			}
		}

		private void UpdateHandPoses()
		{
			if (controller == null)
			{
				return;
			}
			SteamVR instance = SteamVR.instance;
			if (instance != null)
			{
				TrackedDevicePose_t pOutputPose = default(TrackedDevicePose_t);
				TrackedDevicePose_t pOutputGamePose = default(TrackedDevicePose_t);
				if (instance.compositor.GetLastPoseForTrackedDeviceIndex(controller.index, ref pOutputPose, ref pOutputGamePose) == EVRCompositorError.None)
				{
					SteamVR_Utils.RigidTransform rigidTransform = new SteamVR_Utils.RigidTransform(pOutputGamePose.mDeviceToAbsoluteTracking);
					base.transform.localPosition = rigidTransform.pos;
					base.transform.localRotation = rigidTransform.rot;
				}
			}
		}

		public void HoverLock(Interactable interactable)
		{
			HandDebugLog("HoverLock " + interactable);
			hoverLocked = true;
			hoveringInteractable = interactable;
		}

		public void HoverUnlock(Interactable interactable)
		{
			HandDebugLog("HoverUnlock " + interactable);
			if (hoveringInteractable == interactable)
			{
				hoverLocked = false;
			}
		}

		public bool GetStandardInteractionButtonDown()
		{
			if ((bool)noSteamVRFallbackCamera)
			{
				return Input.GetMouseButtonDown(0);
			}
			if (controller != null)
			{
				return controller.GetHairTriggerDown();
			}
			return false;
		}

		public bool GetStandardInteractionButtonUp()
		{
			if ((bool)noSteamVRFallbackCamera)
			{
				return Input.GetMouseButtonUp(0);
			}
			if (controller != null)
			{
				return controller.GetHairTriggerUp();
			}
			return false;
		}

		public bool GetStandardInteractionButton()
		{
			if ((bool)noSteamVRFallbackCamera)
			{
				return Input.GetMouseButton(0);
			}
			if (controller != null)
			{
				return controller.GetHairTrigger();
			}
			return false;
		}

		private void InitController(int index)
		{
			if (controller == null)
			{
				controller = SteamVR_Controller.Input(index);
				HandDebugLog("Hand " + base.name + " connected with device index " + controller.index);
				controllerObject = UnityEngine.Object.Instantiate(controllerPrefab);
				controllerObject.SetActive(value: true);
				controllerObject.name = controllerPrefab.name + "_" + base.name;
				AttachObject(controllerObject, AttachmentFlags.SnapOnAttach | AttachmentFlags.DetachOthers | AttachmentFlags.DetachFromOtherHand | AttachmentFlags.ParentToHand, string.Empty);
				controller.TriggerHapticPulse(800);
				controllerObject.transform.localScale = controllerPrefab.transform.localScale;
				BroadcastMessage("OnHandInitialized", index, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
