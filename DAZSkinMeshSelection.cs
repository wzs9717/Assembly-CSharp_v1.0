using UnityEngine;

public class DAZSkinMeshSelection : DAZMeshSelection
{
	[SerializeField]
	private DAZSkin _skin;

	public DAZSkin skin
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
