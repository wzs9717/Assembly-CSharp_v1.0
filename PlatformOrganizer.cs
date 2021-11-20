using System.Collections.Generic;
using UnityEngine;

public class PlatformOrganizer : MonoBehaviour
{
	public string displayName;

	public Transform TargetsContainer;

	public Transform LookAt;

	public SkinnedMeshRenderer MorphTargetSkin;

	public Transform MorphTargetDAZMesh;

	public Material[] ModifyTargetMaterials;

	private Dictionary<string, ForceReceiver> FRMap;

	private Dictionary<string, ForceProducer> FPMap;

	public void AddObject(Transform prefab)
	{
		Transform transform = Object.Instantiate(prefab);
		AddToFRMap(transform);
		transform.SetParent(base.transform, worldPositionStays: false);
	}

	private void AddToFRMap(Transform t)
	{
		ForceReceiver[] componentsInChildren = t.GetComponentsInChildren<ForceReceiver>();
		ForceReceiver[] array = componentsInChildren;
		foreach (ForceReceiver forceReceiver in array)
		{
			FRMap.Add(forceReceiver.name, forceReceiver);
		}
	}

	private void InitFRMap()
	{
		FRMap = new Dictionary<string, ForceReceiver>();
		AddToFRMap(base.transform);
	}

	private void InitFPMap()
	{
		ForceProducer[] componentsInChildren = GetComponentsInChildren<ForceProducer>();
		FPMap = new Dictionary<string, ForceProducer>();
		ForceProducer[] array = componentsInChildren;
		foreach (ForceProducer forceProducer in array)
		{
			FPMap.Add(forceProducer.name, forceProducer);
		}
	}

	public string[] GetForceReceiverNames()
	{
		if (FRMap == null)
		{
			InitFRMap();
		}
		string[] array = new string[FRMap.Keys.Count];
		FRMap.Keys.CopyTo(array, 0);
		return array;
	}

	public ForceReceiver nameToForceReceiver(string forceReceiverName)
	{
		if (FRMap == null)
		{
			InitFRMap();
		}
		if (forceReceiverName != null)
		{
			if (FRMap.TryGetValue(forceReceiverName, out var value))
			{
				return value;
			}
			return null;
		}
		return null;
	}

	public ForceProducer nameToForceProducer(string forceProducerName)
	{
		if (FPMap == null)
		{
			InitFPMap();
		}
		if (forceProducerName != null)
		{
			if (FPMap.TryGetValue(forceProducerName, out var value))
			{
				return value;
			}
			return null;
		}
		return null;
	}

	private void Start()
	{
		if ((bool)MorphTargetDAZMesh)
		{
		}
		if ((bool)LookAt)
		{
			MoveAsOtherAndLookAt[] componentsInChildren = GetComponentsInChildren<MoveAsOtherAndLookAt>();
			MoveAsOtherAndLookAt[] array = componentsInChildren;
			foreach (MoveAsOtherAndLookAt moveAsOtherAndLookAt in array)
			{
				moveAsOtherAndLookAt.LookAtTransform = LookAt;
			}
		}
	}
}
