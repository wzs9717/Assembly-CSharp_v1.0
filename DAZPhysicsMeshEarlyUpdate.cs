using UnityEngine;

public class DAZPhysicsMeshEarlyUpdate : MonoBehaviour
{
	public DAZPhysicsMesh dazPhysicsMesh;

	private void Update()
	{
		if (dazPhysicsMesh != null)
		{
			dazPhysicsMesh.EarlyUpdate();
		}
	}
}
