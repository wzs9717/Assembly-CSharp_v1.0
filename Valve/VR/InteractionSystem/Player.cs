using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class Player : MonoBehaviour
	{
		[Tooltip("Virtual transform corresponding to the meatspace tracking origin. Devices are tracked relative to this.")]
		public Transform trackingOriginTransform;

		[Tooltip("List of possible transforms for the head/HMD, including the no-SteamVR fallback camera.")]
		public Transform[] hmdTransforms;

		[Tooltip("List of possible Hands, including no-SteamVR fallback Hands.")]
		public Hand[] hands;

		[Tooltip("Reference to the physics collider that follows the player's HMD position.")]
		public Collider headCollider;

		[Tooltip("These objects are enabled when SteamVR is available")]
		public GameObject rigSteamVR;

		[Tooltip("These objects are enabled when SteamVR is not available, or when the user toggles out of VR")]
		public GameObject rig2DFallback;

		[Tooltip("The audio listener for this player")]
		public Transform audioListener;

		public bool allowToggleTo2D = true;

		private static Player _instance;

		public static Player instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Object.FindObjectOfType<Player>();
				}
				return _instance;
			}
		}

		public int handCount
		{
			get
			{
				int num = 0;
				for (int i = 0; i < hands.Length; i++)
				{
					if (hands[i].gameObject.activeInHierarchy)
					{
						num++;
					}
				}
				return num;
			}
		}

		public Hand leftHand
		{
			get
			{
				for (int i = 0; i < hands.Length; i++)
				{
					if (hands[i].gameObject.activeInHierarchy && hands[i].GuessCurrentHandType() == Hand.HandType.Left)
					{
						return hands[i];
					}
				}
				return null;
			}
		}

		public Hand rightHand
		{
			get
			{
				for (int i = 0; i < hands.Length; i++)
				{
					if (hands[i].gameObject.activeInHierarchy && hands[i].GuessCurrentHandType() == Hand.HandType.Right)
					{
						return hands[i];
					}
				}
				return null;
			}
		}

		public SteamVR_Controller.Device leftController
		{
			get
			{
				Hand hand = leftHand;
				if ((bool)hand)
				{
					return hand.controller;
				}
				return null;
			}
		}

		public SteamVR_Controller.Device rightController
		{
			get
			{
				Hand hand = rightHand;
				if ((bool)hand)
				{
					return hand.controller;
				}
				return null;
			}
		}

		public Transform hmdTransform
		{
			get
			{
				for (int i = 0; i < hmdTransforms.Length; i++)
				{
					if (hmdTransforms[i].gameObject.activeInHierarchy)
					{
						return hmdTransforms[i];
					}
				}
				return null;
			}
		}

		public float eyeHeight
		{
			get
			{
				Transform transform = hmdTransform;
				if ((bool)transform)
				{
					return Vector3.Project(transform.position - trackingOriginTransform.position, trackingOriginTransform.up).magnitude / trackingOriginTransform.lossyScale.x;
				}
				return 0f;
			}
		}

		public Vector3 feetPositionGuess
		{
			get
			{
				Transform transform = hmdTransform;
				if ((bool)transform)
				{
					return trackingOriginTransform.position + Vector3.ProjectOnPlane(transform.position - trackingOriginTransform.position, trackingOriginTransform.up);
				}
				return trackingOriginTransform.position;
			}
		}

		public Vector3 bodyDirectionGuess
		{
			get
			{
				Transform transform = hmdTransform;
				if ((bool)transform)
				{
					Vector3 vector = Vector3.ProjectOnPlane(transform.forward, trackingOriginTransform.up);
					if (Vector3.Dot(transform.up, trackingOriginTransform.up) < 0f)
					{
						vector = -vector;
					}
					return vector;
				}
				return trackingOriginTransform.forward;
			}
		}

		public Hand GetHand(int i)
		{
			for (int j = 0; j < hands.Length; j++)
			{
				if (hands[j].gameObject.activeInHierarchy)
				{
					if (i <= 0)
					{
						return hands[j];
					}
					i--;
				}
			}
			return null;
		}

		private void Awake()
		{
			if (trackingOriginTransform == null)
			{
				trackingOriginTransform = base.transform;
			}
		}

		private void OnEnable()
		{
			_instance = this;
			if (SteamVR.instance != null)
			{
				ActivateRig(rigSteamVR);
			}
			else
			{
				ActivateRig(rig2DFallback);
			}
		}

		private void OnDrawGizmos()
		{
			if (this != instance)
			{
				return;
			}
			Gizmos.color = Color.white;
			Gizmos.DrawIcon(feetPositionGuess, "vr_interaction_system_feet.png");
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(feetPositionGuess, feetPositionGuess + trackingOriginTransform.up * eyeHeight);
			Gizmos.color = Color.blue;
			Vector3 vector = bodyDirectionGuess;
			Vector3 vector2 = Vector3.Cross(trackingOriginTransform.up, vector);
			Vector3 vector3 = feetPositionGuess + trackingOriginTransform.up * eyeHeight * 0.75f;
			Vector3 vector4 = vector3 + vector * 0.33f;
			Gizmos.DrawLine(vector3, vector4);
			Gizmos.DrawLine(vector4, vector4 - 0.033f * (vector + vector2));
			Gizmos.DrawLine(vector4, vector4 - 0.033f * (vector - vector2));
			Gizmos.color = Color.red;
			int num = handCount;
			for (int i = 0; i < num; i++)
			{
				Hand hand = GetHand(i);
				if (hand.startingHandType == Hand.HandType.Left)
				{
					Gizmos.DrawIcon(hand.transform.position, "vr_interaction_system_left_hand.png");
					continue;
				}
				if (hand.startingHandType == Hand.HandType.Right)
				{
					Gizmos.DrawIcon(hand.transform.position, "vr_interaction_system_right_hand.png");
					continue;
				}
				switch (hand.GuessCurrentHandType())
				{
				case Hand.HandType.Left:
					Gizmos.DrawIcon(hand.transform.position, "vr_interaction_system_left_hand_question.png");
					break;
				case Hand.HandType.Right:
					Gizmos.DrawIcon(hand.transform.position, "vr_interaction_system_right_hand_question.png");
					break;
				default:
					Gizmos.DrawIcon(hand.transform.position, "vr_interaction_system_unknown_hand.png");
					break;
				}
			}
		}

		public void Draw2DDebug()
		{
			if (!allowToggleTo2D || !SteamVR.active)
			{
				return;
			}
			int num = 100;
			int num2 = 25;
			int num3 = Screen.width / 2 - num / 2;
			int num4 = Screen.height - num2 - 10;
			string text = ((!rigSteamVR.activeSelf) ? "VR" : "2D Debug");
			if (GUI.Button(new Rect(num3, num4, num, num2), text))
			{
				if (rigSteamVR.activeSelf)
				{
					ActivateRig(rig2DFallback);
				}
				else
				{
					ActivateRig(rigSteamVR);
				}
			}
		}

		private void ActivateRig(GameObject rig)
		{
			rigSteamVR.SetActive(rig == rigSteamVR);
			rig2DFallback.SetActive(rig == rig2DFallback);
			if ((bool)audioListener)
			{
				audioListener.transform.parent = hmdTransform;
				audioListener.transform.localPosition = Vector3.zero;
				audioListener.transform.localRotation = Quaternion.identity;
			}
		}

		public void PlayerShotSelf()
		{
		}
	}
}
