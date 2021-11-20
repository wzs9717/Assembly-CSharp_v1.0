using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DAZPhysicsMeshHardVerticesGroup
{
	[SerializeField]
	protected string _name;

	[SerializeField]
	protected DAZBone _bone;

	[SerializeField]
	protected int[] _vertices;

	[SerializeField]
	protected bool _useMorphedVertices;

	[SerializeField]
	protected float _colliderRadius = 0.003f;

	[SerializeField]
	private string _colliderLayer;

	[SerializeField]
	private Transform[] _ignoreColliders;

	[SerializeField]
	private PhysicMaterial _colliderMaterial;

	[SerializeField]
	private bool _offsetCenterByRadius;

	[SerializeField]
	private float _colliderOffset;

	protected DAZSkinV2 skin;

	protected SphereCollider[] _colliders;

	protected Transform _colliderTransform;

	protected Vector3 zeroPosition = Vector3.zero;

	protected Quaternion identityRotation = Quaternion.identity;

	public string name
	{
		get
		{
			return _name;
		}
		set
		{
			if (name != value)
			{
				_name = value;
			}
		}
	}

	public DAZBone bone
	{
		get
		{
			return _bone;
		}
		set
		{
			if (_bone != value)
			{
				_bone = value;
			}
		}
	}

	public int[] vertices => _vertices;

	public bool useMorphedVertices
	{
		get
		{
			return _useMorphedVertices;
		}
		set
		{
			if (_useMorphedVertices != value)
			{
				_useMorphedVertices = value;
			}
		}
	}

	public float colliderRadius
	{
		get
		{
			return _colliderRadius;
		}
		set
		{
			if (_colliderRadius != value)
			{
				_colliderRadius = value;
			}
		}
	}

	public string colliderLayer
	{
		get
		{
			return _colliderLayer;
		}
		set
		{
			if (_colliderLayer != value)
			{
				_colliderLayer = value;
			}
		}
	}

	public Transform[] ignoreColliders
	{
		get
		{
			return _ignoreColliders;
		}
		set
		{
			if (_ignoreColliders != value)
			{
				_ignoreColliders = value;
			}
		}
	}

	public PhysicMaterial colliderMaterial
	{
		get
		{
			return _colliderMaterial;
		}
		set
		{
			if (_colliderMaterial != value)
			{
				_colliderMaterial = value;
			}
		}
	}

	public bool offsetCenterByRadius
	{
		get
		{
			return _offsetCenterByRadius;
		}
		set
		{
			if (_offsetCenterByRadius != value)
			{
				_offsetCenterByRadius = value;
			}
		}
	}

	public float colliderOffset
	{
		get
		{
			return _colliderOffset;
		}
		set
		{
			if (_colliderOffset != value)
			{
				_colliderOffset = value;
			}
		}
	}

	public DAZPhysicsMeshHardVerticesGroup()
	{
		_vertices = new int[0];
	}

	public void AddVertex(int vid)
	{
		int[] array = new int[_vertices.Length + 1];
		bool flag = false;
		for (int i = 0; i < _vertices.Length; i++)
		{
			if (_vertices[i] == vid)
			{
				flag = true;
				break;
			}
			array[i] = _vertices[i];
		}
		if (!flag)
		{
			array[_vertices.Length] = vid;
			_vertices = array;
		}
	}

	public void RemoveVertex(int vid)
	{
		int[] array = new int[_vertices.Length - 1];
		bool flag = false;
		int num = 0;
		for (int i = 0; i < _vertices.Length; i++)
		{
			if (_vertices[i] == vid)
			{
				flag = true;
				continue;
			}
			array[num] = _vertices[i];
			num++;
		}
		if (flag)
		{
			_vertices = array;
		}
	}

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

	public void InitColliders()
	{
		if (_colliders == null)
		{
			return;
		}
		List<Collider> list = new List<Collider>();
		Transform[] array = _ignoreColliders;
		foreach (Transform transform in array)
		{
			GetCollidersRecursive(transform, transform, list);
		}
		foreach (Collider item in list)
		{
			SphereCollider[] colliders = _colliders;
			foreach (Collider collider in colliders)
			{
				Physics.IgnoreCollision(collider, item);
			}
		}
	}

	public void CreateColliders(Transform transform, DAZSkinV2 sk)
	{
		skin = sk;
		Vector3[] drawVerts = skin.drawVerts;
		Vector3[] drawNormals = skin.drawNormals;
		GameObject gameObject = new GameObject();
		_colliderTransform = gameObject.transform;
		gameObject.transform.SetParent(transform);
		if (_colliderLayer != null && _colliderLayer != string.Empty)
		{
			int num2 = (gameObject.layer = LayerMask.NameToLayer(_colliderLayer));
		}
		_colliders = new SphereCollider[_vertices.Length];
		for (int i = 0; i < _vertices.Length; i++)
		{
			int num3 = _vertices[i];
			SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
			_colliders[i] = sphereCollider;
			sphereCollider.radius = _colliderRadius;
			if (_offsetCenterByRadius || _colliderOffset != 0f)
			{
				float num4 = ((!_offsetCenterByRadius) ? _colliderOffset : (_colliderRadius + _colliderOffset));
				sphereCollider.center = drawVerts[num3] - drawNormals[num3] * num4;
			}
			else
			{
				sphereCollider.center = drawVerts[num3];
			}
			if (_colliderMaterial != null)
			{
				sphereCollider.material = _colliderMaterial;
			}
		}
		InitColliders();
	}

	public void UpdateColliders()
	{
		_colliderTransform.position = zeroPosition;
		_colliderTransform.rotation = identityRotation;
		Vector3[] array = ((!_useMorphedVertices) ? skin.rawSkinnedVerts : skin.drawVerts);
		Vector3[] drawNormals = skin.drawNormals;
		for (int i = 0; i < _vertices.Length; i++)
		{
			int num = _vertices[i];
			skin.postSkinVerts[num] = true;
			SphereCollider sphereCollider = _colliders[i];
			if (_offsetCenterByRadius || _colliderOffset != 0f)
			{
				float num2 = ((!_offsetCenterByRadius) ? _colliderOffset : (_colliderRadius + _colliderOffset));
				sphereCollider.center = array[num] - drawNormals[num] * num2;
			}
			else
			{
				sphereCollider.center = array[num];
			}
		}
	}
}
