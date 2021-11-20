using System.Collections.Generic;
using UnityEngine;

public class AutoColliderGroup : MonoBehaviour
{
	public bool controlChildAutoColliders = true;

	[SerializeField]
	private PhysicMaterial _colliderMaterial;

	[SerializeField]
	private float _jointLimit;

	[SerializeField]
	private float _jointLimitSpring;

	[SerializeField]
	private float _jointLimitDamper;

	[SerializeField]
	private float _jointSpringLook = 2000f;

	[SerializeField]
	private float _jointDamperLook = 10f;

	[SerializeField]
	private float _jointSpringUp = 2000f;

	[SerializeField]
	private float _jointDamperUp = 10f;

	[SerializeField]
	private float _jointSpringRight = 2000f;

	[SerializeField]
	private float _jointDamperRight = 10f;

	[SerializeField]
	private float _jointSpringMaxForce = 1E+23f;

	[SerializeField]
	private float _jointMass = 0.5f;

	[SerializeField]
	private float _jointBackForce = 1000f;

	[SerializeField]
	private float _jointBackForceThresholdDistance = 0.001f;

	[SerializeField]
	private float _jointBackForceMaxForce = 100f;

	[SerializeField]
	private float _autoRadiusMultiplier = 1f;

	[SerializeField]
	private bool _showUsedVerts = true;

	public Transform[] ignoreColliders;

	public AutoColliderGroup[] ignoreColliderGroups;

	protected AutoCollider[] _autoColliders;

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
				SyncChildAutoColliders();
			}
		}
	}

	public float jointLimit
	{
		get
		{
			return _jointLimit;
		}
		set
		{
			if (_jointLimit != value)
			{
				_jointLimit = value;
				SyncChildAutoColliders();
			}
		}
	}

	public float jointLimitSpring
	{
		get
		{
			return _jointLimitSpring;
		}
		set
		{
			if (_jointLimitSpring != value)
			{
				_jointLimitSpring = value;
				SyncChildAutoColliders();
			}
		}
	}

	public float jointLimitDamper
	{
		get
		{
			return _jointLimitDamper;
		}
		set
		{
			if (_jointLimitDamper != value)
			{
				_jointLimitDamper = value;
				SyncChildAutoColliders();
			}
		}
	}

	public float jointSpringLook
	{
		get
		{
			return _jointSpringLook;
		}
		set
		{
			if (_jointSpringLook != value)
			{
				_jointSpringLook = value;
				SyncChildAutoColliders();
			}
		}
	}

	public float jointDamperLook
	{
		get
		{
			return _jointDamperLook;
		}
		set
		{
			if (_jointDamperLook != value)
			{
				_jointDamperLook = value;
				SyncChildAutoColliders();
			}
		}
	}

	public float jointSpringUp
	{
		get
		{
			return _jointSpringUp;
		}
		set
		{
			if (_jointSpringUp != value)
			{
				_jointSpringUp = value;
				SyncChildAutoColliders();
			}
		}
	}

	public float jointDamperUp
	{
		get
		{
			return _jointDamperUp;
		}
		set
		{
			if (_jointDamperUp != value)
			{
				_jointDamperUp = value;
				SyncChildAutoColliders();
			}
		}
	}

	public float jointSpringRight
	{
		get
		{
			return _jointSpringRight;
		}
		set
		{
			if (_jointSpringRight != value)
			{
				_jointSpringRight = value;
				SyncChildAutoColliders();
			}
		}
	}

	public float jointDamperRight
	{
		get
		{
			return _jointDamperRight;
		}
		set
		{
			if (_jointDamperRight != value)
			{
				_jointDamperRight = value;
				SyncChildAutoColliders();
			}
		}
	}

	public float jointSpringMaxForce
	{
		get
		{
			return _jointSpringMaxForce;
		}
		set
		{
			if (_jointSpringMaxForce != value)
			{
				_jointSpringMaxForce = value;
				SyncChildAutoColliders();
			}
		}
	}

	public float jointMass
	{
		get
		{
			return _jointMass;
		}
		set
		{
			if (_jointMass != value)
			{
				_jointMass = value;
				SyncChildAutoColliders();
			}
		}
	}

	public float jointBackForce
	{
		get
		{
			return _jointBackForce;
		}
		set
		{
			if (_jointBackForce != value)
			{
				_jointBackForce = value;
				SyncChildAutoColliders();
			}
		}
	}

	public float jointBackForceThresholdDistance
	{
		get
		{
			return _jointBackForceThresholdDistance;
		}
		set
		{
			if (_jointBackForceThresholdDistance != value)
			{
				_jointBackForceThresholdDistance = value;
				SyncChildAutoColliders();
			}
		}
	}

	public float jointBackForceMaxForce
	{
		get
		{
			return _jointBackForceMaxForce;
		}
		set
		{
			if (_jointBackForceMaxForce != value)
			{
				_jointBackForceMaxForce = value;
				SyncChildAutoColliders();
			}
		}
	}

	public float autoRadiusMultiplier
	{
		get
		{
			return _autoRadiusMultiplier;
		}
		set
		{
			if (_autoRadiusMultiplier != value)
			{
				_autoRadiusMultiplier = value;
				SyncChildAutoColliders();
			}
		}
	}

	public bool showUsedVerts
	{
		get
		{
			return _showUsedVerts;
		}
		set
		{
			if (_showUsedVerts != value)
			{
				_showUsedVerts = value;
				SyncChildAutoColliders();
			}
		}
	}

	protected void SyncChildAutoColliders()
	{
		if (controlChildAutoColliders)
		{
			AutoCollider[] autoColliders = GetAutoColliders();
			AutoCollider[] array = autoColliders;
			foreach (AutoCollider autoCollider in array)
			{
				autoCollider.colliderMaterial = _colliderMaterial;
				autoCollider.jointSpringLook = _jointSpringLook;
				autoCollider.jointDamperLook = _jointDamperLook;
				autoCollider.jointSpringUp = _jointSpringUp;
				autoCollider.jointDamperUp = _jointDamperUp;
				autoCollider.jointSpringRight = _jointSpringRight;
				autoCollider.jointDamperRight = _jointDamperRight;
				autoCollider.jointSpringMaxForce = _jointSpringMaxForce;
				autoCollider.jointMass = _jointMass;
				autoCollider.jointBackForce = _jointBackForce;
				autoCollider.jointBackForceThresholdDistance = _jointBackForceThresholdDistance;
				autoCollider.jointBackForceMaxForce = _jointBackForceMaxForce;
				autoCollider.softJointLimit = _jointLimit;
				autoCollider.softJointLimitSpring = _jointLimitSpring;
				autoCollider.softJointLimitDamper = _jointLimitDamper;
				autoCollider.showUsedVerts = _showUsedVerts;
				autoCollider.autoRadiusMultiplier = _autoRadiusMultiplier;
			}
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

	public AutoCollider[] GetAutoColliders()
	{
		_autoColliders = GetComponentsInChildren<AutoCollider>();
		return _autoColliders;
	}

	public void InitColliders()
	{
		AutoCollider[] componentsInChildren = GetComponentsInChildren<AutoCollider>();
		List<Collider> list = new List<Collider>();
		Transform[] array = ignoreColliders;
		foreach (Transform transform in array)
		{
			GetCollidersRecursive(transform, transform, list);
		}
		AutoCollider[] array2 = componentsInChildren;
		foreach (AutoCollider autoCollider in array2)
		{
			if (!(autoCollider.jointCollider != null))
			{
				continue;
			}
			AutoCollider[] array3 = componentsInChildren;
			foreach (AutoCollider autoCollider2 in array3)
			{
				if (autoCollider != autoCollider2 && autoCollider2.jointCollider != null)
				{
					Physics.IgnoreCollision(autoCollider.jointCollider, autoCollider2.jointCollider);
				}
			}
			foreach (Collider item in list)
			{
				Physics.IgnoreCollision(autoCollider.jointCollider, item);
			}
		}
		AutoColliderGroup[] array4 = ignoreColliderGroups;
		foreach (AutoColliderGroup autoColliderGroup in array4)
		{
			AutoCollider[] componentsInChildren2 = autoColliderGroup.GetComponentsInChildren<AutoCollider>();
			AutoCollider[] array5 = componentsInChildren;
			foreach (AutoCollider autoCollider3 in array5)
			{
				if (!(autoCollider3.jointCollider != null))
				{
					continue;
				}
				AutoCollider[] array6 = componentsInChildren2;
				foreach (AutoCollider autoCollider4 in array6)
				{
					if (autoCollider4.jointCollider != null)
					{
						Physics.IgnoreCollision(autoCollider3.jointCollider, autoCollider4.jointCollider);
					}
				}
			}
		}
	}

	private void OnEnable()
	{
		InitColliders();
	}
}
