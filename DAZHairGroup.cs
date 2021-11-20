using UnityEngine;

[ExecuteInEditMode]
public class DAZHairGroup : MonoBehaviour
{
	public string displayName;

	public DAZBone drawRigidOnBone;

	public Transform hairPrefab;

	[SerializeField]
	protected DAZSkinV2 _skin;

	public CapsuleCollider[] hairColliders;

	[SerializeField]
	protected Transform hairInstance;

	public DAZSkinV2 skin
	{
		get
		{
			return _skin;
		}
		set
		{
			if (_skin != value)
			{
				_skin = value;
				Connect();
			}
		}
	}

	protected void Connect()
	{
		DAZSkinWrap[] componentsInChildren = GetComponentsInChildren<DAZSkinWrap>(includeInactive: true);
		if (componentsInChildren != null && skin != null)
		{
			DAZSkinWrap[] array = componentsInChildren;
			foreach (DAZSkinWrap dAZSkinWrap in array)
			{
				dAZSkinWrap.skinTransform = skin.transform;
				dAZSkinWrap.skin = skin;
			}
		}
		DAZHairMesh[] componentsInChildren2 = GetComponentsInChildren<DAZHairMesh>(includeInactive: true);
		DAZHairMesh[] array2 = componentsInChildren2;
		foreach (DAZHairMesh dAZHairMesh in array2)
		{
			dAZHairMesh.capsuleColliders = hairColliders;
			DAZSkinV2MeshSelection component = dAZHairMesh.gameObject.GetComponent<DAZSkinV2MeshSelection>();
			if (component != null)
			{
				component.meshTransform = skin.transform;
				component.skin = skin;
			}
			dAZHairMesh.Reset();
		}
		if (!(drawRigidOnBone != null))
		{
			return;
		}
		DAZMesh[] componentsInChildren3 = GetComponentsInChildren<DAZMesh>(includeInactive: true);
		if (componentsInChildren3 != null)
		{
			DAZMesh[] array3 = componentsInChildren3;
			foreach (DAZMesh dAZMesh in array3)
			{
				dAZMesh.drawFromBone = drawRigidOnBone;
			}
		}
	}

	private void OnEnable()
	{
		if (hairPrefab != null && hairInstance == null)
		{
			hairInstance = Object.Instantiate(hairPrefab);
			hairInstance.parent = base.transform;
			hairInstance.localPosition = Vector3.zero;
			hairInstance.localRotation = Quaternion.identity;
			Connect();
		}
	}

	private void OnDisable()
	{
		if (hairInstance != null)
		{
			if (!Application.isPlaying)
			{
				Object.DestroyImmediate(hairInstance.gameObject);
			}
			else
			{
				Object.Destroy(hairInstance.gameObject);
			}
			hairInstance = null;
		}
	}
}
