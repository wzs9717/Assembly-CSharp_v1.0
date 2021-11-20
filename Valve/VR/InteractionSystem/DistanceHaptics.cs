using System.Collections;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class DistanceHaptics : MonoBehaviour
	{
		public Transform firstTransform;

		public Transform secondTransform;

		public AnimationCurve distanceIntensityCurve = AnimationCurve.Linear(0f, 800f, 1f, 800f);

		public AnimationCurve pulseIntervalCurve = AnimationCurve.Linear(0f, 0.01f, 1f, 0f);

		private IEnumerator Start()
		{
			while (true)
			{
				float distance = Vector3.Distance(firstTransform.position, secondTransform.position);
				SteamVR_TrackedObject trackedObject = GetComponentInParent<SteamVR_TrackedObject>();
				if ((bool)trackedObject)
				{
					float num = distanceIntensityCurve.Evaluate(distance);
					SteamVR_Controller.Input((int)trackedObject.index).TriggerHapticPulse((ushort)num);
				}
				float nextPulse = pulseIntervalCurve.Evaluate(distance);
				yield return new WaitForSeconds(nextPulse);
			}
		}
	}
}
