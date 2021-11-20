using UnityEngine;

public class DAZSkinV2MeshSelection : DAZMeshSelection
{
	[SerializeField]
	private DAZSkinV2 _skin;

	public DAZSkinV2 skin
	{
		get
		{
			return _skin;
		}
		set
		{
			_skin = value;
			if (_skin != null)
			{
				mesh = _skin.dazMesh;
			}
		}
	}
}
