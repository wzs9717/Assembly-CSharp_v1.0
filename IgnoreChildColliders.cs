using System.Collections.Generic;
using UnityEngine;

public class IgnoreChildColliders : MonoBehaviour
{
	public Transform[] additionalIgnores;

	private void GetCollidersRecursive(Transform rootTransform, Transform t, List<Collider> colliders)
	{
		if (t != rootTransform && (bool)t.GetComponent<Rigidbody>())
		{
			return;
		}
		Collider[] components = t.GetComponents<Collider>();
		foreach (Collider collider in components)
		{
			if (collider != null && collider.gameObject.activeInHierarchy && collider.enabled)
			{
				colliders.Add(collider);
			}
		}
		foreach (Transform item in t)
		{
			GetCollidersRecursive(rootTransform, item, colliders);
		}
	}

	private void GetRigidbodyChildrenRecursive(Transform rootTransform, Transform t, List<Transform> children)
	{
		if (t != rootTransform && (bool)t.GetComponent<Rigidbody>())
		{
			children.Add(t);
			return;
		}
		foreach (Transform item in t)
		{
			GetRigidbodyChildrenRecursive(rootTransform, item, children);
		}
	}

	public void SyncColliders()
	{
		List<Collider> list = new List<Collider>();
		GetCollidersRecursive(base.transform, base.transform, list);
		List<Transform> list2 = new List<Transform>();
		GetRigidbodyChildrenRecursive(base.transform, base.transform, list2);
		List<Collider> list3 = new List<Collider>();
		foreach (Transform item in list2)
		{
			GetCollidersRecursive(item, item, list3);
		}
		foreach (Collider item2 in list)
		{
			foreach (Collider item3 in list3)
			{
				Physics.IgnoreCollision(item2, item3);
			}
		}
		if (additionalIgnores == null)
		{
			return;
		}
		List<Collider> list4 = new List<Collider>();
		Transform[] array = additionalIgnores;
		foreach (Transform transform in array)
		{
			GetCollidersRecursive(transform, transform, list4);
		}
		foreach (Collider item4 in list)
		{
			foreach (Collider item5 in list4)
			{
				Physics.IgnoreCollision(item4, item5);
			}
		}
	}

	private void OnEnable()
	{
		SyncColliders();
	}
}
