using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(Camera))]
public class SteamVR_UpdatePoses : MonoBehaviour
{
	private void OnPreCull()
	{
		CVRCompositor compositor = OpenVR.Compositor;
		if (compositor != null)
		{
			SteamVR_Render instance = SteamVR_Render.instance;
			compositor.GetLastPoses(instance.poses, instance.gamePoses);
			SteamVR_Events.NewPoses.Send(instance.poses);
			SteamVR_Events.NewPosesApplied.Send();
		}
	}
}
