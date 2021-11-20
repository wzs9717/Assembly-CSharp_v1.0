using UnityEngine;

public class OVRTracker
{
	public struct Frustum
	{
		public float nearZ;

		public float farZ;

		public Vector2 fov;
	}

	public bool isPresent
	{
		get
		{
			if (!OVRManager.isHmdPresent)
			{
				return false;
			}
			return OVRPlugin.positionSupported;
		}
	}

	public bool isPositionTracked => OVRPlugin.positionTracked;

	public bool isEnabled
	{
		get
		{
			if (!OVRManager.isHmdPresent)
			{
				return false;
			}
			return OVRPlugin.position;
		}
		set
		{
			if (OVRManager.isHmdPresent)
			{
				OVRPlugin.position = value;
			}
		}
	}

	public int count
	{
		get
		{
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				if (GetPresent(i))
				{
					num++;
				}
			}
			return num;
		}
	}

	public Frustum GetFrustum(int tracker = 0)
	{
		if (!OVRManager.isHmdPresent)
		{
			return default(Frustum);
		}
		return OVRPlugin.GetTrackerFrustum((OVRPlugin.Tracker)tracker).ToFrustum();
	}

	public OVRPose GetPose(int tracker = 0)
	{
		if (!OVRManager.isHmdPresent)
		{
			return OVRPose.identity;
		}
		OVRPose oVRPose;
		switch (tracker)
		{
		case 0:
			oVRPose = OVRPlugin.GetNodePose(OVRPlugin.Node.TrackerZero, OVRPlugin.Step.Render).ToOVRPose();
			break;
		case 1:
			oVRPose = OVRPlugin.GetNodePose(OVRPlugin.Node.TrackerOne, OVRPlugin.Step.Render).ToOVRPose();
			break;
		case 2:
			oVRPose = OVRPlugin.GetNodePose(OVRPlugin.Node.TrackerTwo, OVRPlugin.Step.Render).ToOVRPose();
			break;
		case 3:
			oVRPose = OVRPlugin.GetNodePose(OVRPlugin.Node.TrackerThree, OVRPlugin.Step.Render).ToOVRPose();
			break;
		default:
			return OVRPose.identity;
		}
		OVRPose result = default(OVRPose);
		result.position = oVRPose.position;
		result.orientation = oVRPose.orientation * Quaternion.Euler(0f, 180f, 0f);
		return result;
	}

	public bool GetPoseValid(int tracker = 0)
	{
		if (!OVRManager.isHmdPresent)
		{
			return false;
		}
		return tracker switch
		{
			0 => OVRPlugin.GetNodePositionTracked(OVRPlugin.Node.TrackerZero), 
			1 => OVRPlugin.GetNodePositionTracked(OVRPlugin.Node.TrackerOne), 
			2 => OVRPlugin.GetNodePositionTracked(OVRPlugin.Node.TrackerTwo), 
			3 => OVRPlugin.GetNodePositionTracked(OVRPlugin.Node.TrackerThree), 
			_ => false, 
		};
	}

	public bool GetPresent(int tracker = 0)
	{
		if (!OVRManager.isHmdPresent)
		{
			return false;
		}
		return tracker switch
		{
			0 => OVRPlugin.GetNodePresent(OVRPlugin.Node.TrackerZero), 
			1 => OVRPlugin.GetNodePresent(OVRPlugin.Node.TrackerOne), 
			2 => OVRPlugin.GetNodePresent(OVRPlugin.Node.TrackerTwo), 
			3 => OVRPlugin.GetNodePresent(OVRPlugin.Node.TrackerThree), 
			_ => false, 
		};
	}
}
