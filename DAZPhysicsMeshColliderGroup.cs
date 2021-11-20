using System;
using UnityEngine;

[Serializable]
public class DAZPhysicsMeshColliderGroup
{
	public string name;

	public DAZBone bone;

	public DAZPhysicsMeshCapsuleCollider[] colliders;

	[SerializeField]
	private int _currentColliderIndex;

	[SerializeField]
	private DAZPhysicsMeshCapsuleCollider _currentCollider;

	public int currentColliderIndex
	{
		get
		{
			return _currentColliderIndex;
		}
		set
		{
			if (_currentColliderIndex != value && _currentColliderIndex >= 0 && _currentColliderIndex < colliders.Length)
			{
				_currentColliderIndex = value;
				_currentCollider = colliders[_currentColliderIndex];
			}
		}
	}

	public DAZPhysicsMeshCapsuleCollider currentCollider => _currentCollider;

	public DAZPhysicsMeshColliderGroup()
	{
		colliders = new DAZPhysicsMeshCapsuleCollider[0];
		_currentColliderIndex = -1;
		_currentCollider = null;
	}

	public void AddCollider()
	{
	}

	public void RemoveCollider(int index)
	{
	}

	public void MoveColider(int fromindex, int toindex)
	{
	}
}
