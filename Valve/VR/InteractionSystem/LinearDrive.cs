using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(Interactable))]
	public class LinearDrive : MonoBehaviour
	{
		public Transform startPosition;

		public Transform endPosition;

		public LinearMapping linearMapping;

		public bool repositionGameObject = true;

		public bool maintainMomemntum = true;

		public float momemtumDampenRate = 5f;

		private float initialMappingOffset;

		private int numMappingChangeSamples = 5;

		private float[] mappingChangeSamples;

		private float prevMapping;

		private float mappingChangeRate;

		private int sampleCount;

		private void Awake()
		{
			mappingChangeSamples = new float[numMappingChangeSamples];
		}

		private void Start()
		{
			if (linearMapping == null)
			{
				linearMapping = GetComponent<LinearMapping>();
			}
			if (linearMapping == null)
			{
				linearMapping = base.gameObject.AddComponent<LinearMapping>();
			}
			initialMappingOffset = linearMapping.value;
			if (repositionGameObject)
			{
				UpdateLinearMapping(base.transform);
			}
		}

		private void HandHoverUpdate(Hand hand)
		{
			if (hand.GetStandardInteractionButtonDown())
			{
				hand.HoverLock(GetComponent<Interactable>());
				initialMappingOffset = linearMapping.value - CalculateLinearMapping(hand.transform);
				sampleCount = 0;
				mappingChangeRate = 0f;
			}
			if (hand.GetStandardInteractionButtonUp())
			{
				hand.HoverUnlock(GetComponent<Interactable>());
				CalculateMappingChangeRate();
			}
			if (hand.GetStandardInteractionButton())
			{
				UpdateLinearMapping(hand.transform);
			}
		}

		private void CalculateMappingChangeRate()
		{
			mappingChangeRate = 0f;
			int num = Mathf.Min(sampleCount, mappingChangeSamples.Length);
			if (num != 0)
			{
				for (int i = 0; i < num; i++)
				{
					mappingChangeRate += mappingChangeSamples[i];
				}
				mappingChangeRate /= num;
			}
		}

		private void UpdateLinearMapping(Transform tr)
		{
			prevMapping = linearMapping.value;
			linearMapping.value = Mathf.Clamp01(initialMappingOffset + CalculateLinearMapping(tr));
			mappingChangeSamples[sampleCount % mappingChangeSamples.Length] = 1f / Time.deltaTime * (linearMapping.value - prevMapping);
			sampleCount++;
			if (repositionGameObject)
			{
				base.transform.position = Vector3.Lerp(startPosition.position, endPosition.position, linearMapping.value);
			}
		}

		private float CalculateLinearMapping(Transform tr)
		{
			Vector3 rhs = endPosition.position - startPosition.position;
			float magnitude = rhs.magnitude;
			rhs.Normalize();
			Vector3 lhs = tr.position - startPosition.position;
			return Vector3.Dot(lhs, rhs) / magnitude;
		}

		private void Update()
		{
			if (maintainMomemntum && mappingChangeRate != 0f)
			{
				mappingChangeRate = Mathf.Lerp(mappingChangeRate, 0f, momemtumDampenRate * Time.deltaTime);
				linearMapping.value = Mathf.Clamp01(linearMapping.value + mappingChangeRate * Time.deltaTime);
				if (repositionGameObject)
				{
					base.transform.position = Vector3.Lerp(startPosition.position, endPosition.position, linearMapping.value);
				}
			}
		}
	}
}
