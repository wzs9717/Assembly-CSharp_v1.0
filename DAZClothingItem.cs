using UnityEngine;

[ExecuteInEditMode]
public class DAZClothingItem : MonoBehaviour
{
	public string displayName;

	public bool disableMaleAnatomy;

	public DAZBone drawRigidOnBone;

	public Transform itemPrefab;

	[SerializeField]
	protected DAZSkinV2 _skin;

	[SerializeField]
	protected Transform itemInstance;

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
		if (drawRigidOnBone != null)
		{
			DAZMesh[] componentsInChildren2 = GetComponentsInChildren<DAZMesh>(includeInactive: true);
			if (componentsInChildren2 != null)
			{
				DAZMesh[] array2 = componentsInChildren2;
				foreach (DAZMesh dAZMesh in array2)
				{
					dAZMesh.drawFromBone = drawRigidOnBone;
				}
			}
		}
		AutoCollider[] componentsInChildren3 = GetComponentsInChildren<AutoCollider>(includeInactive: true);
		if (componentsInChildren3 != null && skin != null)
		{
			AutoCollider[] array3 = componentsInChildren3;
			foreach (AutoCollider autoCollider in array3)
			{
				autoCollider.skinTransform = skin.transform;
				autoCollider.skin = skin;
			}
		}
	}

	private void OnEnable()
	{
		if (itemPrefab != null && itemInstance == null)
		{
			itemInstance = Object.Instantiate(itemPrefab);
			itemInstance.parent = base.transform;
			itemInstance.localPosition = Vector3.zero;
			itemInstance.localRotation = Quaternion.identity;
			Connect();
		}
	}

	private void OnDisable()
	{
		if (itemInstance != null)
		{
			Object.DestroyImmediate(itemInstance.gameObject);
			itemInstance = null;
		}
	}
}
