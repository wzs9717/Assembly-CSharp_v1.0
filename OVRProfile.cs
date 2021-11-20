using System;
using UnityEngine;

public class OVRProfile : UnityEngine.Object
{
	[Obsolete]
	public enum State
	{
		NOT_TRIGGERED,
		LOADING,
		READY,
		ERROR
	}

	[Obsolete]
	public string id => "000abc123def";

	[Obsolete]
	public string userName => "Oculus User";

	[Obsolete]
	public string locale => "en_US";

	public float ipd => Vector3.Distance(OVRPlugin.GetNodePose(OVRPlugin.Node.EyeLeft, OVRPlugin.Step.Render).ToOVRPose().position, OVRPlugin.GetNodePose(OVRPlugin.Node.EyeRight, OVRPlugin.Step.Render).ToOVRPose().position);

	public float eyeHeight => OVRPlugin.eyeHeight;

	public float eyeDepth => OVRPlugin.eyeDepth;

	public float neckHeight => eyeHeight - 0.075f;

	[Obsolete]
	public State state => State.READY;
}
