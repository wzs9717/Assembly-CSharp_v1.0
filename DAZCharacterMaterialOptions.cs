using UnityEngine;

public class DAZCharacterMaterialOptions : MaterialOptions
{
	[HideInInspector]
	public Transform skinContainer;

	[HideInInspector]
	[SerializeField]
	protected DAZSkinV2 _skin;

	public bool useSimpleMaterial;

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
				SetAllParameters();
			}
		}
	}

	protected override void SetMaterialParam(string name, float value)
	{
		if (!(skin != null))
		{
			return;
		}
		if (useSimpleMaterial)
		{
			Material gPUsimpleMaterial = skin.GPUsimpleMaterial;
			if (gPUsimpleMaterial.HasProperty(name))
			{
				gPUsimpleMaterial.SetFloat(name, value);
			}
		}
		else
		{
			if (paramMaterialSlots == null)
			{
				return;
			}
			for (int i = 0; i < paramMaterialSlots.Length; i++)
			{
				int num = paramMaterialSlots[i];
				if (num < skin.numMaterials)
				{
					Material material = skin.GPUmaterials[num];
					if (material != null && material.HasProperty(name))
					{
						material.SetFloat(name, value);
					}
				}
			}
		}
	}

	protected override void SetMaterialColor(string name, Color c)
	{
		if (!(skin != null))
		{
			return;
		}
		if (useSimpleMaterial)
		{
			Material gPUsimpleMaterial = skin.GPUsimpleMaterial;
			if (gPUsimpleMaterial.HasProperty(name))
			{
				gPUsimpleMaterial.SetColor(name, c);
			}
		}
		else
		{
			if (paramMaterialSlots == null)
			{
				return;
			}
			for (int i = 0; i < paramMaterialSlots.Length; i++)
			{
				int num = paramMaterialSlots[i];
				if (num < skin.numMaterials)
				{
					Material material = skin.GPUmaterials[num];
					if (material != null && material.HasProperty(name))
					{
						material.SetColor(name, c);
					}
				}
			}
		}
	}

	protected override void SetMaterialTexture(int slot, string propName, Texture texture)
	{
		if (paramMaterialSlots != null && texture != null && skin != null && slot < skin.numMaterials)
		{
			Material material = skin.GPUmaterials[slot];
			if (material != null && material.HasProperty(propName))
			{
				material.SetTexture(propName, texture);
			}
		}
	}

	protected override void SetStartingValues()
	{
		if (wasInit)
		{
			return;
		}
		if (skin != null && materialForDefaults == null)
		{
			DAZMesh componentInChildren = skin.GetComponentInChildren<DAZMergedMesh>(includeInactive: true);
			if (componentInChildren == null)
			{
				componentInChildren = skin.GetComponentInChildren<DAZMesh>(includeInactive: true);
			}
			if (componentInChildren != null && paramMaterialSlots != null && paramMaterialSlots.Length > 0)
			{
				int num = paramMaterialSlots[0];
				if (num < componentInChildren.numMaterials)
				{
					materialForDefaults = componentInChildren.materials[num];
				}
			}
		}
		base.SetStartingValues();
	}
}
