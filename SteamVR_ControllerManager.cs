using UnityEngine;
using Valve.VR;

public class SteamVR_ControllerManager : MonoBehaviour
{
	public GameObject left;

	public GameObject right;

	public GameObject[] objects;

	public bool assignAllBeforeIdentified;

	private uint[] indices;

	private bool[] connected = new bool[16];

	private uint leftIndex = uint.MaxValue;

	private uint rightIndex = uint.MaxValue;

	private SteamVR_Events.Action inputFocusAction;

	private SteamVR_Events.Action deviceConnectedAction;

	private SteamVR_Events.Action trackedDeviceRoleChangedAction;

	private static string[] labels = new string[2] { "left", "right" };

	public void UpdateTargets()
	{
		int num = ((objects != null) ? objects.Length : 0);
		GameObject[] array = new GameObject[2 + num];
		indices = new uint[2 + num];
		array[0] = right;
		indices[0] = uint.MaxValue;
		array[1] = left;
		indices[1] = uint.MaxValue;
		for (int i = 0; i < num; i++)
		{
			array[2 + i] = objects[i];
			indices[2 + i] = uint.MaxValue;
		}
		objects = array;
	}

	private void Awake()
	{
		UpdateTargets();
		inputFocusAction = SteamVR_Events.InputFocusAction(OnInputFocus);
		deviceConnectedAction = SteamVR_Events.DeviceConnectedAction(OnDeviceConnected);
		trackedDeviceRoleChangedAction = SteamVR_Events.SystemAction(EVREventType.VREvent_TrackedDeviceRoleChanged, OnTrackedDeviceRoleChanged);
	}

	private void OnEnable()
	{
		for (int i = 0; i < objects.Length; i++)
		{
			GameObject gameObject = objects[i];
			if (gameObject != null)
			{
				gameObject.SetActive(value: false);
			}
		}
		Refresh();
		for (int j = 0; j < SteamVR.connected.Length; j++)
		{
			if (SteamVR.connected[j])
			{
				OnDeviceConnected(j, connected: true);
			}
		}
		inputFocusAction.enabled = true;
		deviceConnectedAction.enabled = true;
		trackedDeviceRoleChangedAction.enabled = true;
	}

	private void OnDisable()
	{
		inputFocusAction.enabled = false;
		deviceConnectedAction.enabled = false;
		trackedDeviceRoleChangedAction.enabled = false;
	}

	private void OnInputFocus(bool hasFocus)
	{
		if (hasFocus)
		{
			for (int i = 0; i < objects.Length; i++)
			{
				GameObject gameObject = objects[i];
				if (gameObject != null)
				{
					string text = ((i >= 2) ? (i - 1).ToString() : labels[i]);
					ShowObject(gameObject.transform, "hidden (" + text + ")");
				}
			}
			return;
		}
		for (int j = 0; j < objects.Length; j++)
		{
			GameObject gameObject2 = objects[j];
			if (gameObject2 != null)
			{
				string text2 = ((j >= 2) ? (j - 1).ToString() : labels[j]);
				HideObject(gameObject2.transform, "hidden (" + text2 + ")");
			}
		}
	}

	private void HideObject(Transform t, string name)
	{
		Transform transform = new GameObject(name).transform;
		transform.parent = t.parent;
		t.parent = transform;
		transform.gameObject.SetActive(value: false);
	}

	private void ShowObject(Transform t, string name)
	{
		Transform parent = t.parent;
		if (!(parent.gameObject.name != name))
		{
			t.parent = parent.parent;
			Object.Destroy(parent.gameObject);
		}
	}

	private void SetTrackedDeviceIndex(int objectIndex, uint trackedDeviceIndex)
	{
		if (trackedDeviceIndex != uint.MaxValue)
		{
			for (int i = 0; i < objects.Length; i++)
			{
				if (i != objectIndex && indices[i] == trackedDeviceIndex)
				{
					GameObject gameObject = objects[i];
					if (gameObject != null)
					{
						gameObject.SetActive(value: false);
					}
					indices[i] = uint.MaxValue;
				}
			}
		}
		if (trackedDeviceIndex == indices[objectIndex])
		{
			return;
		}
		indices[objectIndex] = trackedDeviceIndex;
		GameObject gameObject2 = objects[objectIndex];
		if (gameObject2 != null)
		{
			if (trackedDeviceIndex == uint.MaxValue)
			{
				gameObject2.SetActive(value: false);
				return;
			}
			gameObject2.SetActive(value: true);
			gameObject2.BroadcastMessage("SetDeviceIndex", (int)trackedDeviceIndex, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnTrackedDeviceRoleChanged(VREvent_t vrEvent)
	{
		Refresh();
	}

	private void OnDeviceConnected(int index, bool connected)
	{
		bool flag = this.connected[index];
		this.connected[index] = false;
		if (connected)
		{
			CVRSystem system = OpenVR.System;
			if (system != null)
			{
				ETrackedDeviceClass trackedDeviceClass = system.GetTrackedDeviceClass((uint)index);
				if (trackedDeviceClass == ETrackedDeviceClass.Controller || trackedDeviceClass == ETrackedDeviceClass.GenericTracker)
				{
					this.connected[index] = true;
					flag = !flag;
				}
			}
		}
		if (flag)
		{
			Refresh();
		}
	}

	public void Refresh()
	{
		int num = 0;
		CVRSystem system = OpenVR.System;
		if (system != null)
		{
			leftIndex = system.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand);
			rightIndex = system.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand);
		}
		if (leftIndex == uint.MaxValue && rightIndex == uint.MaxValue)
		{
			for (uint num2 = 0u; num2 < connected.Length; num2++)
			{
				if (num >= objects.Length)
				{
					break;
				}
				if (connected[num2])
				{
					SetTrackedDeviceIndex(num++, num2);
					if (!assignAllBeforeIdentified)
					{
						break;
					}
				}
			}
		}
		else
		{
			SetTrackedDeviceIndex(num++, (rightIndex >= connected.Length || !connected[rightIndex]) ? uint.MaxValue : rightIndex);
			SetTrackedDeviceIndex(num++, (leftIndex >= connected.Length || !connected[leftIndex]) ? uint.MaxValue : leftIndex);
			if (leftIndex != uint.MaxValue && rightIndex != uint.MaxValue)
			{
				for (uint num3 = 0u; num3 < connected.Length; num3++)
				{
					if (num >= objects.Length)
					{
						break;
					}
					if (connected[num3] && num3 != leftIndex && num3 != rightIndex)
					{
						SetTrackedDeviceIndex(num++, num3);
					}
				}
			}
		}
		while (num < objects.Length)
		{
			SetTrackedDeviceIndex(num++, uint.MaxValue);
		}
	}
}
